using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_manager_Project.Models
{
    public class Task1
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }
        [Required]
        [ForeignKey("Project")]
        public Guid Projectid { get; set; }
        public Project? Project { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(20)]
        public string Priority { get; set; }
        [DataType(DataType.Date)]
        public string Due_date {  get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Created_at { get; set; }
        public bool IsCompleted { get; set; }

        public Guid assignedToId { get; set; }
        public User? AssignedTo { get; set; }

    }
}
