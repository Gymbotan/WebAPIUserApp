using System.ComponentModel.DataAnnotations;

namespace WebAPIUserApp.Models
{
    /// <summary>
    /// User entity. Main class
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int Age { get; set; }
        public List<Role> Roles { get; set; }
    }
}
