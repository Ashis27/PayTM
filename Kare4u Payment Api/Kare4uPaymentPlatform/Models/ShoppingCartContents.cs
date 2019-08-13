using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kare4uPaymentPlatform.Models
{
    [Serializable]
    [DataContract]
    public class ShoppingCartContents
    {
        [DataMember]
        public long TransactionID { get; set; }
        [DataMember]
        public int GroupEntityId { get; set; }
        [DataMember]
        public int ParentGroupEntityId { get; set; }
        [DataMember]
        public int LineItemId { get; set; }
        [DataMember]
        public string LabName { get; set; }
        [DataMember]
        public string CartItem { get; set; }
        [DataMember]
        public DateTime BookingDate { get; set; }
        [DataMember]
        public string BookingSlot { get; set; }

        //public bool HomeCollection { get; set; }
        [DataMember]
        public string Price { get; set; }
        [DataMember]
        public double Discount { get; set; }
        [DataMember]
        public string HomeCollectionCharges { get; set; }
        [DataMember]
        public string PriceAfterDiscount { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public string TimeStamp { get; set; }
        [DataMember]
        public int ConsumerId { get; set; }
        [DataMember]
        public int BookedFor { get; set; }
        [DataMember]
        public string BookedForName { get; set; }
        [DataMember]
        public int TestType { get; set; }
        [DataMember]
        public string CustomerAddress { get; set; }
        [DataMember]
        public string CustomerCity { get; set; }
        [DataMember]
        public string CustomerLandmark { get; set; }
        [DataMember]
        public string UniqueReference { get; set; }
        [DataMember]
        public string DiscountDescription { get; set; }
        [DataMember]
        public string CorporateName { get; set; }
        [DataMember]
        public int CorporateDiscountsId { get; set; }
        [DataMember]
        public string PaymentForDescription { get; set; }
        [DataMember]
        public int ProviderID { get; set; }
        [DataMember]
        public int HospitalID { get; set; }
        [DataMember]
        public string PaymentType { get; set; }
        [DataMember]
        public int Premium { get; set; }
        [DataMember]
        public string CouponCode { get; set; }
        //Fields required for provisional registration (while booking lab package)
        [DataMember]

        public string ContactNo { get; set; }
        [DataMember]

        public string Email { get; set; }
        [DataMember]

        public string CustomerName { get; set; }
        [DataMember]

        public int SpecializationId { get; set; }
        [DataMember]
        public string HospitalCity { get; set; }
        [DataMember]
        public string HospitalCenter { get; set; }
        [DataMember]
        public string CouponDiscountAmount { get; set; }
        public Boolean IsFollowupAppointment { get; set; }
        public int FollowedupAppointmentId { get; set; }
        public Boolean Status { get; set; }
        public string Message { get; set; }
        public int PaymentGateway { get; set; }
        [DataMember]
        public double OtherDiscount { get; set; }

    }

}