using PhonebookMVC.Models;

namespace PhonebookMVC.ViewModels.Phones
{
    public class PhonesListVM : BaseListVM<Phone>
    {
        public Contact Contact { get; set; }
    }
}