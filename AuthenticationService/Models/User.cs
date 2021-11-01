using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models {
    public class User {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int ExternalID { get; set; }
    }
}
