using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }
        public string PartnerName { get; set; }
        public string PartnerManagerId { get; set; }
        public string PreferenceId { get; set; }
    }
}