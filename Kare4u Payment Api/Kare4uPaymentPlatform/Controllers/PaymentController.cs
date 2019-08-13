﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kare4uPaymentPlatform.Models;
using paytm;
using Kare4uPaymentPlatform.Models.PaymentGateway;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;

namespace Kare4uPaymentPlatform.Controllers
{
    public class PaymentController : Controller
    {
        private Care4UContext _context = new Care4UContext();
        public static String agentType = "";
        // GET: Payment
        public ActionResult Index()
        {
            Random generator = new Random();
            String randomNumber = generator.Next(0, 999999).ToString("D6");
            String randomNumber1 = generator.Next(0, 9999).ToString("D6");
            PaytmRequest PaytmRequest = new PaytmRequest();
            PaytmRequest.MerchantKey = "z2jgxVaU1n8eGFn5";
            PaytmRequest.MID = "KAHESO22899626174813";
            PaytmRequest.CHANNEL_ID = "WEB";
            PaytmRequest.INDUSTRY_TYPE_ID = "Retail";
            PaytmRequest.WEBSITE = "WEBSTAGING";
            PaytmRequest.EMAIL = "susant.patra.77@gmail.com";
            PaytmRequest.MOBILE_NO = "9439421021";
            PaytmRequest.CUST_ID = "CustIs102351241";
            PaytmRequest.ORDER_ID = randomNumber1 + randomNumber;
            PaytmRequest.TXN_AMOUNT = "10";
            PaytmRequest.CALLBACK_URL = "http://localhost:63661/Payment/PaymentResponse" + "?interimOrderDeatilsId=" + "aaaaa";
            PaytmRequest.PaytmURL = "https://securegw-stage.paytm.in/theia/processTransaction";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("MID", PaytmRequest.MID);
            parameters.Add("CHANNEL_ID", PaytmRequest.CHANNEL_ID);
            parameters.Add("INDUSTRY_TYPE_ID", PaytmRequest.INDUSTRY_TYPE_ID);
            parameters.Add("WEBSITE", PaytmRequest.WEBSITE);
            parameters.Add("EMAIL", PaytmRequest.EMAIL);
            parameters.Add("MOBILE_NO", PaytmRequest.MOBILE_NO);
            parameters.Add("CUST_ID", PaytmRequest.CUST_ID);
            parameters.Add("ORDER_ID", PaytmRequest.ORDER_ID);
            parameters.Add("TXN_AMOUNT", PaytmRequest.TXN_AMOUNT);
            parameters.Add("CALLBACK_URL", PaytmRequest.CALLBACK_URL); //This parameter is not mandatory. Use this to pass the callback url dynamically.
            string checksum = CheckSum.generateCheckSum(PaytmRequest.MerchantKey, parameters);
            PaytmRequest.CheckSum = checksum;
            string outputHTML = "<html>";
            outputHTML += "<head>";
            outputHTML += "<title>Merchant Check Out Page</title>";
            outputHTML += "</head>";
            outputHTML += "<body>";
            outputHTML += "<center><h1>Please do not refresh this page...</h1></center>";
            outputHTML += "<form method='post' action='" + PaytmRequest.PaytmURL + "' name='f1'>";
            outputHTML += "<table border='1'>";
            outputHTML += "<tbody>";
            foreach (string key in parameters.Keys)
            {
                outputHTML += "<input type='hidden' name='" + key + "' value='" + parameters[key] + "'>";
            }
            outputHTML += "<input type='hidden' name='CHECKSUMHASH' value='" + checksum + "'>";
            outputHTML += "</tbody>";
            outputHTML += "</table>";
            outputHTML += "<script type='text/javascript'>";
            outputHTML += "document.f1.submit();";
            outputHTML += "</script>";
            outputHTML += "</form>";
            outputHTML += "</body>";
            outputHTML += "</html>";

            PaytmRequest.PaymentUrl = outputHTML;
            ViewBag.PaymentUrl = outputHTML;
            return View(PaytmRequest);
        }


        public ActionResult PaytmPamentGateway(string interimOrderDeatilsId, string agent = null)
        {
            agentType = agent;
            // Get the IntermediarOrderDetail with the id provided
            InterimOrderDeatils interimOrderDetail = _context.InterimOrderDeatils.FirstOrDefault(t => t.UniqueReference.ToLower() == interimOrderDeatilsId.ToLower());
            if (interimOrderDetail == null)
                throw new Exception("No record found.");

            var response = System.Web.Helpers.Json.Decode(interimOrderDetail.OrderDetails.ToString())[0];
            //Get the Care4uPgConfig with the groupEntityId available in the IntermediarOrderDetail fetched.
            // Get configuaration by ApplicationName,ModuleName,GroupEntityID,PaymentGateWay
            Care4UPgConfig pgConfig = _context.Care4uPgConfig.FirstOrDefault(t => t.Module.ToUpper().Equals(interimOrderDetail.PaymentForModule.ToUpper())
            && t.PaymentGateWayType.ToUpper().Equals("PAYTM")
            && t.IsActive);

            if (pgConfig == null)
                throw new Exception("Payment for this module has not been enabled.");

            //Form the PaytmRequest

            Random generator = new Random();
            String randomNumber = generator.Next(0, 999999).ToString("D6");

            PaytmRequest PaytmRequest = new PaytmRequest();
            PaytmRequest.MerchantKey = pgConfig.Paytm_MerchantKey;//"jBIWVeuQ0AKxUU%R";
            PaytmRequest.MID = pgConfig.Paytm_MID;//"Kare4U04848812903368";
            PaytmRequest.CHANNEL_ID = pgConfig.Paytm_CHANNEL_ID; // "WAP";
            PaytmRequest.INDUSTRY_TYPE_ID = pgConfig.Paytm_INDUSTRY_TYPE_ID; // "Retail";
            PaytmRequest.WEBSITE = pgConfig.Paytm_WEBSITE; // "APPSTAGING";
            PaytmRequest.MOBILE_NO = response.ContactNo != null ? response.ContactNo : "0123456789"; // "9439421021";
            PaytmRequest.EMAIL = !String.IsNullOrEmpty(response.Email) ? response.Email : "healthpro@kare4u.in"; // "susant.patra.77@gmail.com";
            PaytmRequest.CUST_ID = (interimOrderDetail.ConsumerID.ToString() != "" && interimOrderDetail.ConsumerID.ToString() != null) ? "Cust" + interimOrderDetail.ConsumerID.ToString() : "Cust" + randomNumber; // interimOrderDetail.ConsumerID.ToString();
                                                                                                                                                                                                                     // PaytmRequest.CUST_ID = "Cust1023515"; // interimOrderDetail.ConsumerID.ToString();            //This property may change if any problem occurs //"Cust10235123"
            PaytmRequest.ORDER_ID = randomNumber;// interimOrderDetail.InterimOrderDeatilsID.ToString();//This property may change if any problem occurs
            PaytmRequest.TXN_AMOUNT = Math.Round(interimOrderDetail.AmountToBeCharged, 0).ToString();
            PaytmRequest.CALLBACK_URL = pgConfig.Paytm_CALLBACK_URL + "?interimOrderDeatilsId=" + interimOrderDeatilsId; // "http://localhost:63661/Payment/PaymentResponse?interimOrderDeatilsId=" + interimOrderDeatilsId;
            PaytmRequest.PaytmURL = pgConfig.Paytm_PaytmURL; // "https://securegw-stage.paytm.in/theia/processTransaction";

            ////PaytmRequest PaytmRequest = new PaytmRequest();
            ////PaytmRequest.MerchantKey = "jBIWVeuQ0AKxUU%R";
            ////PaytmRequest.MID = "Kare4U04848812903368";
            ////PaytmRequest.CHANNEL_ID = "WAP";
            ////PaytmRequest.INDUSTRY_TYPE_ID = "Retail";
            ////PaytmRequest.WEBSITE = "APPSTAGING";
            ////PaytmRequest.EMAIL = "susant.patra.77@gmail.com";
            ////PaytmRequest.MOBILE_NO = "9439421021";
            ////PaytmRequest.CUST_ID = "Cust10235124";
            ////PaytmRequest.ORDER_ID = randomNumber;
            ////PaytmRequest.TXN_AMOUNT = "10";
            ////PaytmRequest.CALLBACK_URL = "http://localhost:63661/Payment/PaymentResponse1";
            ////PaytmRequest.PaytmURL = "https://securegw-stage.paytm.in/theia/processTransaction";


            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("REQUEST_TYPE", "DEFAULT");
            //parameters.Add("AUTH_MODE", "3D");
            // parameters.Add("PAYMENT_TYPE_ID", "DC");

            parameters.Add("MID", PaytmRequest.MID);
            parameters.Add("CHANNEL_ID", PaytmRequest.CHANNEL_ID);
            parameters.Add("INDUSTRY_TYPE_ID", PaytmRequest.INDUSTRY_TYPE_ID);
            parameters.Add("WEBSITE", PaytmRequest.WEBSITE);
            parameters.Add("EMAIL", PaytmRequest.EMAIL);
            parameters.Add("MOBILE_NO", PaytmRequest.MOBILE_NO);
            parameters.Add("CUST_ID", PaytmRequest.CUST_ID);
            parameters.Add("ORDER_ID", PaytmRequest.ORDER_ID);
            parameters.Add("TXN_AMOUNT", PaytmRequest.TXN_AMOUNT);
            parameters.Add("CALLBACK_URL", PaytmRequest.CALLBACK_URL); //This parameter is not mandatory. Use this to pass the callback url dynamically.


            //parameters.Add("MerchantKey", PaytmRequest.MerchantKey);
            //parameters.Add("InterimOrderDetailsId", interimOrderDeatilsId.ToString());


            string checksum = CheckSum.generateCheckSum(PaytmRequest.MerchantKey, parameters);

            PaytmRequest.CheckSum = checksum;


            //  string paytmURL = "https://securegw-stage.paytm.in/theia/processTransaction?orderid=" + r;
            // string paytmURL = "https://securegw-stage.paytm.in/theia/processTransaction";
            string outputHTML = "<html>";
            outputHTML += "<head>";
            outputHTML += "<title>Merchant Check Out Page</title>";
            outputHTML += "</head>";
            outputHTML += "<body>";
            outputHTML += "<center><h1>Please do not refresh this page...</h1></center>";
            outputHTML += "<form method='post' action='" + PaytmRequest.PaytmURL + "' name='f1'>";
            outputHTML += "<table border='1'>";
            outputHTML += "<tbody>";
            foreach (string key in parameters.Keys)
            {
                outputHTML += "<input type='hidden' name='" + key + "' value='" + parameters[key] + "'>";
            }
            outputHTML += "<input type='hidden' name='CHECKSUMHASH' value='" + checksum + "'>";
            outputHTML += "</tbody>";
            outputHTML += "</table>";
            outputHTML += "<script type='text/javascript'>";
            outputHTML += "document.f1.submit();";
            outputHTML += "</script>";
            outputHTML += "</form>";
            outputHTML += "</body>";
            outputHTML += "</html>";
            PaytmRequest.PaymentUrl = outputHTML;
            ViewBag.PaymentUrl = outputHTML;
            return View(PaytmRequest);
        }

        public ActionResult GMOPamentGateway(string uniqueId, string agent = null)
        {
            Random generator = new Random();
            String randomNumber = generator.Next(0, 999999).ToString("D6");
            String randomNumber1 = generator.Next(0, 9999).ToString("D6");
            GMOPaymentRequest PaymentRequest = new GMOPaymentRequest();
            //PaymentRequest.SiteID = "tsite00034783";
            //PaymentRequest.SitePassword = "r6e3v4dv";
            //PaymentRequest.MemberID = "300028";
            PaymentRequest.ShopID = "tshop00039254";
            PaymentRequest.ShopPassword = "3fdzr473";
            PaymentRequest.OrderID = "3845384583548";
            PaymentRequest.Amount = "10000"; //Convert.ToDecimal("10000").ToString("N");
            //PaymentRequest.Currency = "JPY";
            PaymentRequest.Tax = "800";//Convert.ToDecimal("200").ToString("N");
            PaymentRequest.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            PaymentRequest.JobCd = "CHECK";
            PaymentRequest.UseCredit = "1";
            PaymentRequest.RetURL = "http://localhost:63661/Payment/PaymentResponse" + "?interimOrderDeatilsId=" + "aaaaa";
            PaymentRequest.PaymentUrl = "https://pt01.mul-pay.jp/link/tshop00039254/Multi/Entry";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("SiteID", PaymentRequest.SiteID);
            //parameters.Add("SitePassword", PaymentRequest.SitePassword);
            //parameters.Add("MemberID", PaymentRequest.MemberID);
            parameters.Add("ShopID", PaymentRequest.ShopID);
            //parameters.Add("ShopPassword", PaymentRequest.ShopPassword);
            parameters.Add("OrderID", PaymentRequest.OrderID);

            parameters.Add("Amount", PaymentRequest.Amount);
            parameters.Add("Tax", PaymentRequest.Tax);
            parameters.Add("DateTime", PaymentRequest.DateTime);
            //parameters.Add("Currency", PaymentRequest.Currency);
            //parameters.Add("Currency", PaymentRequest.Currency);
            parameters.Add("RetURL", PaymentRequest.RetURL); //This parameter is not mandatory. Use this to pass the callback url dynamically.
            parameters.Add("UseCredit", PaymentRequest.UseCredit);
            parameters.Add("JobCd", PaymentRequest.JobCd);
            string shop_hash_string = string.Empty;
            string member_hash_string = string.Empty;

            //[Shop ID+ Order ID+ Amount of money+ Tax/shipping+ Shop password+ Date information] in 
            // MD5 hashed character string.
            // shop information
            shop_hash_string = PaymentRequest.ShopID + "|" + PaymentRequest.OrderID + "|" + PaymentRequest.Amount + "|" + PaymentRequest.Tax + "|" + PaymentRequest.ShopPassword + "|" + PaymentRequest.DateTime;
            string shopPassString = GenerateMD5SignatureForGMO(shop_hash_string).ToLower();
            //[Site ID+ Member ID+ Site password+ Date information] in MD5 hashed character string.
            // member information
            //member_hash_string = PaymentRequest.SiteID + PaymentRequest.MemberID + PaymentRequest.SitePassword + PaymentRequest.DateTime;
            //string memberPassString = GenerateMD5SignatureForPayU(member_hash_string).ToLower();

            PaymentRequest.ShopPassString = shopPassString;
            //PaymentRequest.MemberPassString = memberPassString;
            parameters.Add("ShopPassString", shopPassString);

            //parameters.Add("MemberPassString", memberPassString);
            string outputHTML = "<html>";
            outputHTML += "<head>";
            outputHTML += "<title>Merchant Check Out Page</title>";
            outputHTML += "</head>";
            outputHTML += "<body>";
            outputHTML += "<center><h1>Please do not refresh this page...</h1></center>";
            outputHTML += "<form method='post' action='" + PaymentRequest.PaymentUrl + "' name='f1'>";
            outputHTML += "<table border='1'>";
            outputHTML += "<tbody>";
            foreach (string key in parameters.Keys)
            {
                outputHTML += "<input type='hidden' name='" + key + "' value='" + parameters[key] + "'>";
            }
            //outputHTML += "<input type='hidden' name='SHOPPASSSTRING' value='" + shopPassString + "'>";
            outputHTML += "</tbody>";
            outputHTML += "</table>";
            outputHTML += "<script type='text/javascript'>";
            outputHTML += "document.f1.submit();";
            outputHTML += "</script>";
            outputHTML += "</form>";
            outputHTML += "</body>";
            outputHTML += "</html>";

            PaymentRequest.PGPaymentUrl = outputHTML;
            ViewBag.PGPaymentUrl = outputHTML;
            return View(PaymentRequest);
        }
        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult PaymentResponse(string interimOrderDeatilsId)
        //{
        //          try
        //          {
        //              // Get the IntermediarOrderDetail with the id provided
        //              InterimOrderDeatils interimOrderDetail = _context.InterimOrderDeatils.FirstOrDefault(t => t.UniqueReference.ToLower() == interimOrderDeatilsId.ToLower());
        //              if (interimOrderDetail == null)
        //                  throw new Exception("No record found.");

        //              //Get the Care4uPgConfig with the groupEntityId available in the IntermediarOrderDetail fetched.
        //              // Get configuaration by ApplicationName,ModuleName,GroupEntityID,PaymentGateWay
        //              Care4UPgConfig pgConfig = _context.Care4uPgConfig.FirstOrDefault(t => t.Module.ToUpper().Equals(interimOrderDetail.PaymentForModule.ToUpper())
        //              && t.PaymentGateWayType.ToUpper().Equals("PAYTM")
        //              && t.IsActive);

        //              if (pgConfig == null)
        //                  throw new Exception("Payment for this module has not been enabled.");


        //              PaytmResponse paytmResponse = new PaytmResponse();
        //              String merchantKey = pgConfig.Paytm_MerchantKey;// "jBIWVeuQ0AKxUU%R"; 

        //              Dictionary<string, string> parameters = new Dictionary<string, string>();
        //              string paytmChecksum = "";
        //              foreach (string key in Request.Form.Keys)
        //              {
        //                  parameters.Add(key.Trim(), Request.Form[key].Trim());
        //              }

        //              if (parameters.ContainsKey("CHECKSUMHASH"))
        //              {
        //                  paytmChecksum = parameters["CHECKSUMHASH"];
        //                  parameters.Remove("CHECKSUMHASH");
        //              }

        //              paytmResponse.BANKNAME = parameters["BANKNAME"];
        //              paytmResponse.BANKTXNID = parameters["BANKTXNID"];
        //              paytmResponse.CHECKSUMHASH = paytmChecksum;
        //              paytmResponse.CURRENCY = parameters["CURRENCY"];
        //              paytmResponse.GATEWAYNAME = parameters["BANKNAME"];
        //              paytmResponse.MID = parameters["MID"];
        //              paytmResponse.ORDERID = parameters["ORDERID"];
        //              paytmResponse.PAYMENTMODE = parameters["PAYMENTMODE"];
        //              // PaytmResponse.PROMO_CAMP_ID = parameters["PROMO_CAMP_ID"]; 
        //              //PaytmResponse.PROMO_RESPCODE = parameters["PROMO_RESPCODE"];
        //              // PaytmResponse.PROMO_STATUS = parameters["PROMO_STATUS"];
        //              paytmResponse.RESPCODE = parameters["RESPCODE"];
        //              paytmResponse.RESPMSG = parameters["RESPMSG"];
        //              paytmResponse.STATUS = parameters["STATUS"];
        //              paytmResponse.TXNAMOUNT = parameters["TXNAMOUNT"];
        //              paytmResponse.TXNDATE = parameters["TXNDATE"];
        //              paytmResponse.TXNID = parameters["TXNID"];

        //              paytmResponse.UniqueReference = interimOrderDetail.UniqueReference;

        //              //String merchantKey = parameters["MerchantKey"];

        //              if (CheckSum.verifyCheckSum(merchantKey, parameters, paytmChecksum))
        //              {
        //                  string redirectAction = "";

        //                  if (paytmResponse.STATUS == "TXN_SUCCESS")
        //                  {
        //                      //Update the IntermediarOrderDetails payment status, response and other records.
        //                      interimOrderDetail.PaymentStatus = true;
        //                      //interimOrderDetail.PaymentResponse = "Success";

        //                      Response.Write("Checksum Matched");
        //                      redirectAction = "PaymentSuccessResponse";
        //                      //return RedirectToAction("PaymentSuccessResponse", PaytmResponse);
        //                  }
        //                  else if (paytmResponse.STATUS == "TXN_FAILURE")
        //                  {
        //                      //Update the IntermediarOrderDetails payment status, response and other records.
        //                      interimOrderDetail.PaymentStatus = false;
        //                      //interimOrderDetail.PaymentResponse = "Failure";

        //                      redirectAction = "PaymentFailureResponse";
        //                      //return RedirectToAction("PaymentFailureResponse", PaytmResponse);
        //                  }
        //                  else
        //                  {
        //                      //Update the IntermediarOrderDetails payment status, response and other records.
        //                      interimOrderDetail.PaymentStatus = false;
        //                      //interimOrderDetail.PaymentResponse = "Pending";

        //                      redirectAction = "PaymentPendingResponse";
        //                      //return RedirectToAction("PaymentPendingResponse", PaytmResponse);
        //                  }

        //                  //using (var db = new Care4UContext())
        //                  //{
        //                  //	db.InterimOrderDeatils.Attach(interimOrderDetail);
        //                  //	db.Entry(interimOrderDetail).Property(p => p.PaymentStatus).IsModified = true;
        //                  //	db.SaveChanges();
        //                  //}

        //                  _context.InterimOrderDeatils.Attach(interimOrderDetail);
        //                  _context.Entry(interimOrderDetail).Property(p => p.PaymentStatus).IsModified = true;
        //                  //_context.Entry(interimOrderDetail).Property(p => p.PaymentResponse).IsModified = true;
        //                  _context.SaveChanges();

        //                  var result = PerformPostPaymentJobs(paytmResponse);
        //                  if (paytmResponse.STATUS == "TXN_SUCCESS")
        //                  {
        //                      // return View("PaymentSuccessResponse");
        //                      return RedirectToAction("PaymentSuccessResponse");
        //                  }
        //                  else if (paytmResponse.STATUS == "TXN_FAILURE")
        //                  {
        //                      // return View("PaymentFailureResponse");
        //                      return RedirectToAction("PaymentFailureResponse");
        //                  }
        //                  else
        //                  {
        //                      //return View("PaymentPendingResponse");
        //                      return RedirectToAction("PaymentPendingResponse");
        //                  }
        //                  //if (res.Data)
        //                  //	return RedirectToAction(redirectAction, paytmResponse);
        //                  //else
        //                  //	return RedirectToAction("PaymentFailureResponse", paytmResponse);
        //              }
        //              else
        //              {
        //                  Response.Write("Checksum MisMatch");
        //              }
        //              return View();
        //          }
        //          catch (Exception ex)
        //          {
        //              return null;
        //          }
        //}

       
       public static string GenerateMD5SignatureForGMO(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static string GenerateMD5SignatureForGMO1(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult PaymentResponse(string interimOrderDeatilsId)
        {
            try
            {

                InterimOrderDeatils interimOrderDetail = _context.InterimOrderDeatils.FirstOrDefault(t => t.UniqueReference.ToLower() == interimOrderDeatilsId.ToLower());
                var response = System.Web.Helpers.Json.Decode(interimOrderDetail.OrderDetails.ToString())[0];

                if (interimOrderDetail == null)
                    throw new Exception("No record found.");

                //Get the Care4uPgConfig with the groupEntityId available in the IntermediarOrderDetail fetched.
                // Get configuaration by ApplicationName,ModuleName,GroupEntityID,PaymentGateWay
                Care4UPgConfig pgConfig = _context.Care4uPgConfig.FirstOrDefault(t => t.Module.ToUpper().Equals(interimOrderDetail.PaymentForModule.ToUpper())
                && t.PaymentGateWayType.ToUpper().Equals("PAYTM")
                && t.IsActive);

                if (pgConfig == null)
                    throw new Exception("Payment for this module has not been enabled.");

                PaytmResponse paytmResponse = new PaytmResponse();
                String merchantKey = pgConfig.Paytm_MerchantKey;// "jBIWVeuQ0AKxUU%R"; 

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string paytmChecksum = "";
                foreach (string key in Request.Form.Keys)
                {
                    parameters.Add(key.Trim(), Request.Form[key].Trim());
                }

                if (parameters.ContainsKey("CHECKSUMHASH"))
                {
                    paytmChecksum = parameters["CHECKSUMHASH"];
                    parameters.Remove("CHECKSUMHASH");
                }

                paytmResponse.BANKNAME = parameters["BANKNAME"];
                paytmResponse.BANKTXNID = parameters["BANKTXNID"];
                paytmResponse.CHECKSUMHASH = paytmChecksum;
                paytmResponse.CURRENCY = parameters["CURRENCY"];
                paytmResponse.GATEWAYNAME = parameters["BANKNAME"];
                paytmResponse.MID = parameters["MID"];
                paytmResponse.ORDERID = parameters["ORDERID"];
                paytmResponse.PAYMENTMODE = parameters["PAYMENTMODE"];
                // PaytmResponse.PROMO_CAMP_ID = parameters["PROMO_CAMP_ID"]; 
                //PaytmResponse.PROMO_RESPCODE = parameters["PROMO_RESPCODE"];
                // PaytmResponse.PROMO_STATUS = parameters["PROMO_STATUS"];
                paytmResponse.RESPCODE = parameters["RESPCODE"];
                paytmResponse.RESPMSG = parameters["RESPMSG"];
                paytmResponse.STATUS = parameters["STATUS"];
                paytmResponse.TXNAMOUNT = parameters["TXNAMOUNT"];
                paytmResponse.TXNDATE = parameters["TXNDATE"];
                paytmResponse.TXNID = parameters["TXNID"];
                //string checksum = CheckSum.generateCheckSum(PaytmRequest.MerchantKey, parameters);
                paytmResponse.UniqueReference = interimOrderDeatilsId;


                //String merchantKey = parameters["MerchantKey"];



                Dictionary<string, string> parameters1 = new Dictionary<string, string>();
                //parameters.Add("REQUEST_TYPE", "DEFAULT");
                //parameters.Add("AUTH_MODE", "3D");
                // parameters.Add("PAYMENT_TYPE_ID", "DC");

                parameters1.Add("MID", paytmResponse.MID);

                parameters1.Add("ORDER_ID", paytmResponse.ORDERID);

                string checksum = CheckSum.generateCheckSum(merchantKey, parameters1);

                paytmResponse.CHECKSUMHASH = checksum;
                return View(paytmResponse);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [HttpPost]
        public ActionResult AfterCheckResponse(PaytmResponse paytmResponse)
        {

            try
            {
                // Get the IntermediarOrderDetail with the id provided
                InterimOrderDeatils interimOrderDetail = _context.InterimOrderDeatils.FirstOrDefault(t => t.UniqueReference.ToLower() == paytmResponse.UniqueReference.ToLower());
                if (interimOrderDetail == null)
                    throw new Exception("No record found.");

                //Get the Care4uPgConfig with the groupEntityId available in the IntermediarOrderDetail fetched.
                // Get configuaration by ApplicationName,ModuleName,GroupEntityID,PaymentGateWay
                Care4UPgConfig pgConfig = _context.Care4uPgConfig.FirstOrDefault(t => t.Module.ToUpper().Equals(interimOrderDetail.PaymentForModule.ToUpper())
                && t.PaymentGateWayType.ToUpper().Equals("PAYTM")
                && t.IsActive);

                if (pgConfig == null)
                    throw new Exception("Payment for this module has not been enabled.");


                //PaytmResponse paytmResponse = new PaytmResponse();
                String merchantKey = pgConfig.Paytm_MerchantKey;// "jBIWVeuQ0AKxUU%R"; 

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string paytmChecksum = "";
                foreach (string key in Request.Form.Keys)
                {
                    parameters.Add(key.Trim(), Request.Form[key].Trim());
                }

                if (parameters.ContainsKey("CHECKSUMHASH"))
                {
                    paytmChecksum = parameters["CHECKSUMHASH"];
                    parameters.Remove("CHECKSUMHASH");
                }

                //paytmResponse.BANKNAME = parameters["BANKNAME"];
                //paytmResponse.BANKTXNID = parameters["BANKTXNID"];
                //paytmResponse.CHECKSUMHASH = paytmChecksum;
                //paytmResponse.CURRENCY = parameters["CURRENCY"];
                //paytmResponse.GATEWAYNAME = parameters["BANKNAME"];
                //paytmResponse.MID = parameters["MID"];
                //paytmResponse.ORDERID = parameters["ORDERID"];
                //paytmResponse.PAYMENTMODE = parameters["PAYMENTMODE"];
                //// PaytmResponse.PROMO_CAMP_ID = parameters["PROMO_CAMP_ID"]; 
                ////PaytmResponse.PROMO_RESPCODE = parameters["PROMO_RESPCODE"];
                //// PaytmResponse.PROMO_STATUS = parameters["PROMO_STATUS"];
                //paytmResponse.RESPCODE = parameters["RESPCODE"];
                //paytmResponse.RESPMSG = parameters["RESPMSG"];
                //paytmResponse.STATUS = parameters["STATUS"];
                //paytmResponse.TXNAMOUNT = parameters["TXNAMOUNT"];
                //paytmResponse.TXNDATE = parameters["TXNDATE"];
                //paytmResponse.TXNID = parameters["TXNID"];

                //paytmResponse.UniqueReference = interimOrderDetail.UniqueReference;

                //String merchantKey = parameters["MerchantKey"];


                string redirectAction = "";

                if (paytmResponse.STATUS == "TXN_SUCCESS")
                {
                    //Update the IntermediarOrderDetails payment status, response and other records.
                    interimOrderDetail.PaymentStatus = true;
                    interimOrderDetail.PaymentResponse = "Success";

                  //  Response.Write("Checksum Matched");
                    redirectAction = "PaymentSuccessResponse";
                    //return RedirectToAction("PaymentSuccessResponse", PaytmResponse);
                }
                else if (paytmResponse.STATUS == "TXN_FAILURE")
                {
                    //Update the IntermediarOrderDetails payment status, response and other records.
                    interimOrderDetail.PaymentStatus = false;
                    interimOrderDetail.PaymentResponse = "Failure";

                    redirectAction = "PaymentFailureResponse";
                    //return RedirectToAction("PaymentFailureResponse", PaytmResponse);
                }
                else
                {
                    //Update the IntermediarOrderDetails payment status, response and other records.
                    interimOrderDetail.PaymentStatus = false;
                    interimOrderDetail.PaymentResponse = "Pending";

                    redirectAction = "PaymentPendingResponse";
                    //return RedirectToAction("PaymentPendingResponse", PaytmResponse);
                }

                //using (var db = new Care4UContext())
                //{
                //	db.InterimOrderDeatils.Attach(interimOrderDetail);
                //	db.Entry(interimOrderDetail).Property(p => p.PaymentStatus).IsModified = true;
                //	db.SaveChanges();
                //}

                _context.InterimOrderDeatils.Attach(interimOrderDetail);
                _context.Entry(interimOrderDetail).Property(p => p.PaymentStatus).IsModified = true;
                //_context.Entry(interimOrderDetail).Property(p => p.PaymentResponse).IsModified = true;
                _context.SaveChanges();
                var result = PerformPostPaymentJobs(paytmResponse);
                if (paytmResponse.STATUS == "TXN_SUCCESS")
                {
                    if (agentType == "MobileApp")
                        return RedirectToAction("PaymentSuccessResponse");
                    else if (agentType == "WebSite")
                        return View("../Payment/HealthProWebAfterPgResponse", interimOrderDetail);
                    else
                        return RedirectToAction("PaymentSuccessResponse");
                }
                else if (paytmResponse.STATUS == "TXN_FAILURE")
                {
                    if (agentType == "MobileApp")
                        return RedirectToAction("PaymentFailureResponse");
                    else if (agentType == "WebSite")
                        return View("../Payment/HealthProWebAfterPgResponse", interimOrderDetail);
                    else
                        return RedirectToAction("PaymentSuccessResponse");
                }
                else
                {
                    if (agentType == "MobileApp")
                        return RedirectToAction("PaymentPendingResponse");
                    else if (agentType == "WebSite")
                        return View("../Payment/HealthProWebAfterPgResponse", interimOrderDetail);
                    else
                        return RedirectToAction("PaymentSuccessResponse");
                }
                //if (res.Data)
                //	return RedirectToAction(redirectAction, paytmResponse);
                //else
                //	return RedirectToAction("PaymentFailureResponse", paytmResponse);

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

            //return View();
        }
        public ActionResult PaymentSuccessResponse()
        {
            return View();
        }
        public ActionResult PaymentFailureResponse()
        {
            return View();
        }
        public ActionResult PaymentPendingResponse()
        {
            return View();
        }

        private HttpResponseMessage PerformPostPaymentJobs(PaytmResponse response)
        {
            HttpClient httpClient = new HttpClient();

            string baseUrl = ConfigurationManager.AppSettings["PaymentResponseJobsUrl"];// "http://localhost:1873/api/AfterPaymentJobs/ProcessOrderAfterPayment";

            var content = new StringContent(new JavaScriptSerializer().Serialize(response), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync(baseUrl, content).Result;
            //result.Content.
            // paytmResponse.RESPMSG = parameters["RESPMSG"];
            //paytmResponse.STATUS = parameters["STATUS"];
            //var data = new { PaytmStatus = response.STATUS, PaytmStatusMessage = response.RESPMSG };

            //@Susant, Please check the result and perform the task accordingly. This books the appointment and returns the status as Success, Failure and Restricted.
            return result;
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult PaymentResponse1()
        {


            PaytmResponse paytmResponse = new PaytmResponse();
            String merchantKey = "z2jgxVaU1n8eGFn5";// "jBIWVeuQ0AKxUU%R"; 

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string paytmChecksum = "";
            foreach (string key in Request.Form.Keys)
            {
                parameters.Add(key.Trim(), Request.Form[key].Trim());
            }

            if (parameters.ContainsKey("CHECKSUMHASH"))
            {
                paytmChecksum = parameters["CHECKSUMHASH"];
                parameters.Remove("CHECKSUMHASH");
            }

            paytmResponse.BANKNAME = parameters["BANKNAME"];
            paytmResponse.BANKTXNID = parameters["BANKTXNID"];
            paytmResponse.CHECKSUMHASH = paytmChecksum;
            paytmResponse.CURRENCY = parameters["CURRENCY"];
            paytmResponse.GATEWAYNAME = parameters["BANKNAME"];
            paytmResponse.MID = parameters["MID"];
            paytmResponse.ORDERID = parameters["ORDERID"];
            paytmResponse.PAYMENTMODE = parameters["PAYMENTMODE"];
            // PaytmResponse.PROMO_CAMP_ID = parameters["PROMO_CAMP_ID"]; 
            //PaytmResponse.PROMO_RESPCODE = parameters["PROMO_RESPCODE"];
            // PaytmResponse.PROMO_STATUS = parameters["PROMO_STATUS"];
            paytmResponse.RESPCODE = parameters["RESPCODE"];
            paytmResponse.RESPMSG = parameters["RESPMSG"];
            paytmResponse.STATUS = parameters["STATUS"];
            paytmResponse.TXNAMOUNT = parameters["TXNAMOUNT"];
            paytmResponse.TXNDATE = parameters["TXNDATE"];
            paytmResponse.TXNID = parameters["TXNID"];

            // paytmResponse.UniqueReference = interimOrderDetail.UniqueReference;

            //String merchantKey = parameters["MerchantKey"];

            if (CheckSum.verifyCheckSum(merchantKey, parameters, paytmChecksum))
            {
                // string redirectAction = "";

                if (paytmResponse.STATUS == "TXN_SUCCESS")
                {
                    //Update the IntermediarOrderDetails payment status, response and other records.
                    //  interimOrderDetail.PaymentStatus = true;
                    //interimOrderDetail.PaymentResponse = "Success";

                    // Response.Write("Checksum Matched");
                    //redirectAction = "PaymentSuccessResponse";
                    //return RedirectToAction("PaymentSuccessResponse", PaytmResponse);
                }
                else if (paytmResponse.STATUS == "TXN_FAILURE")
                {
                    //Update the IntermediarOrderDetails payment status, response and other records.
                    // interimOrderDetail.PaymentStatus = false;
                    //interimOrderDetail.PaymentResponse = "Failure";

                    // redirectAction = "PaymentFailureResponse";
                    //return RedirectToAction("PaymentFailureResponse", PaytmResponse);
                }
                else
                {
                    //Update the IntermediarOrderDetails payment status, response and other records.
                    // interimOrderDetail.PaymentStatus = false;
                    // //interimOrderDetail.PaymentResponse = "Pending";

                    // redirectAction = "PaymentPendingResponse";
                    //return RedirectToAction("PaymentPendingResponse", PaytmResponse);
                }

                //using (var db = new Care4UContext())
                //{
                //	db.InterimOrderDeatils.Attach(interimOrderDetail);
                //	db.Entry(interimOrderDetail).Property(p => p.PaymentStatus).IsModified = true;
                //	db.SaveChanges();
                //}


                if (paytmResponse.STATUS == "TXN_SUCCESS")
                {
                    // return View("PaymentSuccessResponse");
                    return RedirectToAction("PaymentSuccessResponse");
                }
                else if (paytmResponse.STATUS == "TXN_FAILURE")
                {
                    // return View("PaymentFailureResponse");
                    return RedirectToAction("PaymentFailureResponse");
                }
                else
                {
                    //return View("PaymentPendingResponse");
                    return RedirectToAction("PaymentPendingResponse");
                }
                //if (res.Data)
                //	return RedirectToAction(redirectAction, paytmResponse);
                //else
                //	return RedirectToAction("PaymentFailureResponse", paytmResponse);
            }
            else
            {
                Response.Write("Checksum MisMatch");
            }
            return View();
        }
    }
}