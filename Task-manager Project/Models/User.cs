using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Task_manager_Project.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(255)]
        public string? Password_hash { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password_hash")]
        public string? ConfirmPassword { get; set; }
        [Required]
        [StringLength(20)]
        public string? role { get; set; } = "User";
        [DataType(DataType.DateTime)]
        public DateTime created_at { get; set; }
        [DataType(DataType.DateTime)]

        public DateTime last_login { get; set; }
    }
}
