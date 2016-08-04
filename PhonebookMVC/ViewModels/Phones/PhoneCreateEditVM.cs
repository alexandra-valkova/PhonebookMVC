using PhonebookMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace PhonebookMVC.ViewModels.Phones
{
    public class PhoneCreateEditVM : BaseVM
    {
        [Required]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "The phone number must contain only digits")]
        public string PhoneNumber { get; set; }

        public int ContactID { get; set; }

        public Contact Contact { get; set; }
    }
}