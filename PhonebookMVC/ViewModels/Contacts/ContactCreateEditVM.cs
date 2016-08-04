using PhonebookMVC.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhonebookMVC.ViewModels.Contacts
{
    public class ContactCreateEditVM : BaseVM
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }

        public List<CheckBoxListItem> Groups { get; set; }

        public int[] GroupIds { get; set; }

        public ContactCreateEditVM()
        {
            Groups = new List<CheckBoxListItem>();
        }
    }
}