using PhonebookMVC.Models;
using PhonebookMVC.Repositories;
using System.Linq;
using System.Web;

namespace PhonebookMVC.Services
{
    public static class AuthenticationService
    {
        public static bool IsLogged
        {
            get
            {
                return LoggedUser != null;
            }
        }

        public static User LoggedUser
        {
            get
            {
                return (User)HttpContext.Current.Session[typeof(AuthenticationService).Name];
            }
            private set
            {
                HttpContext.Current.Session[typeof(AuthenticationService).Name] = value;
            }
        }

        public static void AuthenticateUser(string username, string password)
        {
            UserRepository userRepo = new UserRepository();

            User user = userRepo.GetAll(u => u.Username == username && u.Password == password).FirstOrDefault();

            if (user != null)
            {
                LoggedUser = user;
            }
        }

        public static void LogOut()
        {
            LoggedUser = null;
        }
    }
}