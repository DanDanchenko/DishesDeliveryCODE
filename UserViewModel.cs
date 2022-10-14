using Microsoft.AspNetCore.Http;

namespace DishesDelivery.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? Email { get; set; }
        public int Age { get; set; }
        public int RoleId { get; set; }
        public string? Password { get; set; }

        public byte[]? Image { get; set; }
        public IFormFile Avatar { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
