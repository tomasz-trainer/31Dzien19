using System;
using System.Collections.Generic;
using System.Text;

namespace P06Shop.Shared.Auth
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Role { get; set; } = "Customer";

        public string UserName { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
