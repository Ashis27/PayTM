using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kare4uPaymentPlatform.Models.PaymentGateway.AppoPayments
{
	public class FlattenedAppoChargesForPG
    {
        [Key]
        public int AppointmentChargesConfigID { get; set; }
        public int GroupentityID { get; set; }
        public int ProviderID { get; set; }
        public string Currency { get; set; }
        public string PreConfMessage { get; set; }
        public string PostConfMessage { get; set; }
        public string AppoTypeDecription { get; set; }
        public decimal RegistrationCharges { get; set; }
        public decimal PremiumCharges { get; set; }
        public decimal ConvenienceCharges { get; set; }
        public int FollowupChargesApplicableFor { get; set; }
        public int RegistrationValidFor { get; set; }
        public bool ChargeRegistrationFees { get; set; }
        public bool ChargeConvinienceFees { get; set; }
        public bool ChargeConsultancyFees { get; set; }
        public bool ChargePremiumFees { get; set; }
        public double ConsultationCharge { get; set; }
        public double Followupcharge { get; set; }
        public double ConsultationDiscount { get; set; }
        public double FirstVisitCorporatePrice { get; set; }
        public int OnlineAppointmentMinimumDelay { get; set; }
        [NotMapped]
        public decimal PriceToShow { get; set; }
        //Added By Ashis on 14-july-2016 for Appointment Discount
        public double AppoDiscount { get; set; }
        //pay the appointment fee later in HealthPro app as per Subhankar's req: Bijay
        public bool PayLaterEnabled { get; set; }
    }
}
