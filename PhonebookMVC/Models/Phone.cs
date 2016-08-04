namespace PhonebookMVC.Models
{
    public class Phone : BaseModel
    {
        public string PhoneNumber { get; set; }

        public int ContactID { get; set; }

        public virtual Contact Contact { get; set; }
    }
}