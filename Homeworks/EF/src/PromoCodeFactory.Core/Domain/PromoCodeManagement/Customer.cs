using System;

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
        
        public Guid CustomerPreferenceId { get; set; }
        public Preference CustomerPreference { get; set; }
        //TODO: Списки Preferences и Promocodes 
    }
}