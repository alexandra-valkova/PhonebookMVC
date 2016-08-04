using System.Collections.Generic;

namespace PhonebookMVC.Models
{
    public class Group : BaseModel
    {
        public string Name { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }

        public virtual List<Contact> Contacts { get; set; }
    }
}