using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace eCommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public override string? UserName { get => Email; set => Email = value; }        
    }
}

