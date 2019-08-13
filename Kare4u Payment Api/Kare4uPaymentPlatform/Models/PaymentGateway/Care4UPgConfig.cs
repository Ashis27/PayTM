using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kare4uPaymentPlatform.Models.PaymentGateway
{
    public class Care4UPgConfig
    {
        public int Care4UPGConfigID { get; set; }
        public string PaymentGateWayType { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }

        public string Paytm_MerchantKey { get; set; }
        public string Paytm_MID { get; set; }
        public string Paytm_CHANNEL_ID { get; set; }
        public string Paytm_INDUSTRY_TYPE_ID { get; set; }
        public string Paytm_WEBSITE { get; set; }

        //public string Paytm_EMAIL { get; set; }
        //public string Paytm_MOBILE_NO { get; set; }
        //public string Paytm_CUST_ID { get; set; }
        //public string Paytm_ORDER_ID { get; set; }
        //public string Paytm_TXN_AMOUNT { get; set; }

        public string Paytm_CALLBACK_URL { get; set; }
        public string Paytm_PaytmURL { get; set; }


        //public string vpc_AccessCode { get; set; }
        //      public string vpc_Merchant { get; set; }
        //      public string vpc_OrderInfo { get; set; }
        //      public string vpc_ReturnURL { get; set; }
        //      public string vpc_Locale { get; set; }
        //      public string vpc_URL { get; set; }
        //      public string vpc_SecureSecret { get; set; }

        //      public string TryAgainLink { get; set; }
        //      public string PayUKey { get; set; }
        //      public string PayUSalt { get; set; }
        //      public string HashSequence { get; set; }
        //      public string PayUPaymentURL { get; set; }
        //      public string PayUSuccessURL { get; set; }
        //      public string PayUFailureURL { get; set; }
        //      public string PayUCancelURL { get; set; }

        public bool IsActive { get; set; }
        public int GroupEntityId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class GMOPGConfigurations
    {
        [Key]
        public int Id { get; set; }
        public string SiteID { get; set; }
        public string SitePassword { get; set; }
        public string MemberID { get; set; }
        public string ShopID { get; set; }
        public string ShopPassword { get; set; }
        public string JobCd { get; set; }
        public string UseCredit { get; set; }
        public string CallbackURL { get; set; }
        public string GMOPGPaymentUrl { get; set; }
        public bool IsActive { get; set; }
        public int GroupEntityId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
