﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kare4uPaymentPlatform.Models
{
	public class PaytmResponse
	{

		public string MID { get; set; }
		public string TXNID { get; set; }
		public string BANKTXNID { get; set; }
		public string TXNAMOUNT { get; set; }
		public string CURRENCY { get; set; }
		public string ORDERID { get; set; }
		public string STATUS { get; set; }
		public string RESPCODE { get; set; }
		public string RESPMSG { get; set; }
		public string TXNDATE { get; set; }
		public string GATEWAYNAME { get; set; }
		public string BANKNAME { get; set; }
		public string PAYMENTMODE { get; set; }
		public string PROMO_CAMP_ID { get; set; }
		public string PROMO_STATUS { get; set; }
		public string PROMO_RESPCODE { get; set; }
		public string CHECKSUMHASH { get; set; }

		public string UniqueReference { get; set; }
	}
}