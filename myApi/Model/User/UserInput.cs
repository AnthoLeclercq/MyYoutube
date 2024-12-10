using System.ComponentModel.DataAnnotations;

namespace myApi.Model.User
{
    public class UserInput
    {
        [Required]
        public string Username { get; set; }
        public string Pseudo { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }


    }
}