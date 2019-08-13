using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kare4uPaymentPlatform.Models
{
    public class PaytmRequest
    {
        public string MerchantKey { get; set; }
        public string MID { get; set; }
        public string CHANNEL_ID { get; set; }
        public string INDUSTRY_TYPE_ID { get; set; }
        public string WEBSITE { get; set; }
        public string CUST_ID { get; set; }
        public string ORDER_ID { get; set; }
        public string TXN_AMOUNT { get; set; }
        public string CALLBACK_URL { get; set; }
        public string PaytmURL { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE_NO { get; set; }
        public string CheckSum { get; set; }
        public string PaymentUrl { get; set; }
    }
    public class GMOPaymentRequest
    {
        public string SiteID { get; set; }
        public string SitePassword { get; set; }
        public string MemberID { get; set; }
        public string ShopID { get; set; }
        public string ShopPassword { get; set; }
        public string OrderID { get; set; }
        public string MemberPassString { get; set; }
        public string ShopPassString { get; set; }
        public string DateTime { get; set; }
        public string JobCd { get; set; }
        public string UseCredit { get; set; }
        public string Amount { get; set; }
        public string Tax { get; set; }
        public string Currency { get; set; }
        public string RetURL { get; set; }
        public string PaymentUrl { get; set; }
        public string PGPaymentUrl { get; set; }
    }
}