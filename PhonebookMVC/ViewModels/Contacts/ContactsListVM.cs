using PagedList;
using PhonebookMVC.Models;

namespace PhonebookMVC.ViewModels.Contacts
{
    public class ContactsListVM : BaseListVM<Contact>
    {
        public PagedList<Contact> PagedContacts { get; set; }

        public int? Page { get; set; }

        public string SearchString { get; set; }

        public ContactSorting? SortOrder { get; set; }
    }
}