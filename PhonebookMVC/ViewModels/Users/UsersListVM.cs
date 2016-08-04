using PagedList;
using PhonebookMVC.Models;

namespace PhonebookMVC.ViewModels.Users
{
    public class UsersListVM : BaseListVM<User>
    {
        public PagedList<User> PagedUsers { get; set; }

        public int? Page { get; set; }

        public string SearchString { get; set; }

        public UserSorting? SortOrder { get; set; }
    }
}