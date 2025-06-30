using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_manager_Project.Models
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [ForeignKey("Owner")]
        public Guid owner_id { get; set; }
        public User? Owner { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime created_at { get; set; } = DateTime.Now;
        [DataType(DataType.DateTime)]
        public DateTime due_date { get; set; }
    }
}
