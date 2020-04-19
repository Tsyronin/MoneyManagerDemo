using System.ComponentModel.DataAnnotations;


namespace MoneyManagerUI.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Year of birth")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Confirm your Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
