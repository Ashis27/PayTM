using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kare4uPaymentPlatform.Models.PaymentGateway
{
    public class PaymentGatewayTransactions
    {
        public PaymentGatewayTransactions()
        {
            Breakup = new List<PGTransactionBreakup>();
        }
        public int PaymentGatewayTransactionsId { get; set; }
        public string MerchTxnRef { get; set; }
        public string OrderInfo {get;set;}
        public decimal TransAmount { get; set; }
        public int TransRespCode { get; set; }
        public string TransRespDesc { get; set; }
        public string TransMessage {get;set;}
        public string TransNumber {get;set;}
        public string TransBatchNumber {get;set;}
        public string TransAcqRespCode {get;set;}
        public string TransReceiptNumber {get;set;}
        public string AuthId {get;set;}
        public string CreatedBy {get;set;}
        public DateTime? CreatedDate {get;set;}
        public string UpdatedBy {get;set;}
        public DateTime? UpdatedDate {get;set;}
        public bool AlreadyPrinted { get; set; }
        public string ReceiptAWSPath { get; set; }
        public List<PGTransactionBreakup> Breakup { get; set; }
        public int GroupEntityID { get; set; }
        public string PaymentGateway { get; set; }
        public string PaymentMode { get; set; }
      }
    public class PGTransactionBreakup
    {
        public int PGTransactionBreakupId { get; set; }
        public int PaymentGatewayTransactionsId { get; set; }
        public int GroupEntityId { get; set; }
        public int PaidByConsumerId { get; set; }
        public string BreakupDesc { get; set; }
        public decimal BreakupAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    
}
