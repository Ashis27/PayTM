using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kare4uPaymentPlatform.Models.PaymentGateway
{
	public class InterimOrderDeatils
	{
		public int InterimOrderDeatilsID { get; set; }
		public int GroupEntityID { get; set; }
		public string UniqueReference { get; set; }
		public string PaymentForModule { get; set; }
		public string OrderDetails { get; set; }
		public decimal AmountToBeCharged { get; set; }
		public bool PaymentStatus { get; set; }
		public int ProviderAppointmentDetailsID { get; set; }
		public string PaymentResponse { get; set; }
		public int ConsumerID { get; set; }
		public string CustomerAddressLine1 { get; set; }
		public string CustomerAddressLine2 { get; set; }
		public string CustomerLandmark { get; set; }
		public string CustomerCity { get; set; }
		public string CustomerState { get; set; }
		public string CustomerZip { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string PaymentGateWay { get; set; }

		public long? TransactionID { get; set; }
		//For Online discount deduction added on 16th Sep, 2016, Bijay
		//public decimal ActualAmount { get; set; }
		//public decimal BilledAmount { get; set; }
		//public double Kare4UDiscount { get; set; }
		//public double OtherDiscount { get; set; }

		//public string CustomerEmail { get; set; }
		//public string CustomerPhone { get; set; }
	}
}
