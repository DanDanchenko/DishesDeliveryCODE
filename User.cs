using System;
using System.Collections.Generic;

namespace DishesDelivery.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? Email { get; set; }
        public int Age { get; set; }
        public int RoleId { get; set; }
        public string? Password { get; set; }
        public byte[]? Image { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
