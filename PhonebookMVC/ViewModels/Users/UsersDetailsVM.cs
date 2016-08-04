using System.ComponentModel.DataAnnotations;

namespace PhonebookMVC.ViewModels.Users
{
    public class UsersDetailsVM : BaseVM
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Profile Picture")]
        public string ImageName { get; set; }
    }
}