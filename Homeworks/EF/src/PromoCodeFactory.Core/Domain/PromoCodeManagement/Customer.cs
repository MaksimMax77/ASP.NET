using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public Guid PromoCodeId { get; set; }
        public PromoCode PromoCode { get; set; }
        
        public List<Preference> Preferences { get; set; }
        public List<CustomerPreference> CustomerPreference { get; set; }
    }
}