using PhonebookMVC.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhonebookMVC.ViewModels.Groups
{
    public class GroupsCreateEditVM : BaseVM
    {
        [Required]
        public string Name { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }

        public List<Group> Groups { get; set; }
    }
}