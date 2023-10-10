using System.ComponentModel.DataAnnotations;

namespace WebAPIUserApp.Models
{
    /// <summary>
    /// User info to create a new user
    /// </summary>
    public class UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }
}
