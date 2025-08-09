using Microsoft.AspNetCore.Identity;

namespace GenericsDirectDealTracker.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? PharmacyName { get; set; } = null;
        public string? PharmacyAddress { get; set; } = null;
        public string? Terms { get; set; } = null;
        public decimal? MinimumOrderValue { get; set; } = 0.00m;
    }
}
