using System;
using System.Collections.Generic;
using System.Runtime;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode
        : BaseEntity
    {
        public string Code { get; set; }

        public string ServiceInfo { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }

        public string PartnerName { get; set; }

        public Guid PartnerManagerId { get; set; }
        public Employee PartnerManager { get; set; }

        public Guid PreferenceId { get; set; }
        public Preference Preference { get; set; } 
        
        public List<Customer> Customers { get; set; }
    }
}