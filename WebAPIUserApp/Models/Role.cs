using System.ComponentModel.DataAnnotations;

namespace WebAPIUserApp.Models
{
    /// <summary>
    /// A role that a user can get
    /// </summary>
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public List<User> Users { get; set; }
    }
}
