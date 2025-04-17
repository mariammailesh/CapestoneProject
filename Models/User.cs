using System;

namespace CapestoneProject.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Full_Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Role_Id { get; set; }
        public bool IsActive { get; set; }
    }
}