using PagedList;
using PhonebookMVC.Models;
using PhonebookMVC.Repositories;
using PhonebookMVC.Services;
using PhonebookMVC.ViewModels.Users;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace PhonebookMVC.Controllers
{
    [Filters.Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            UsersListVM model = new UsersListVM();

            TryUpdateModel(model);

            UserRepository userRepo = new UserRepository();

            model.Entities = userRepo.GetAll();

            if (!string.IsNullOrEmpty(model.SearchString)) // Search
            {
                model.Entities = userRepo.GetAll(u => u.FirstName.Contains(model.SearchString) ||
                                                      u.LastName.Contains(model.SearchString) ||
                                                      u.Username.Contains(model.SearchString) ||
                                                      u.Email.Contains(model.SearchString));
            }

            // Sorting

            model.RouteDictionary = new RouteValueDictionary
            {
                { "SearchString", model.SearchString }
            };

            if (model.SortOrder == null)
            {
                model.SortOrder = UserSorting.FirstNameAsc;
            }

            switch (model.SortOrder)
            {
                case UserSorting.FirstNameAsc:
                    model.Entities = model.Entities.OrderBy(u => u.FirstName).ToList();
                    break;
                case UserSorting.FirstNameDesc:
                    model.Entities = model.Entities.OrderByDescending(u => u.FirstName).ToList();
                    break;
                case UserSorting.LastNameAsc:
                    model.Entities = model.Entities.OrderBy(u => u.LastName).ToList();
                    break;
                case UserSorting.LastNameDesc:
                    model.Entities = model.Entities.OrderByDescending(u => u.LastName).ToList();
                    break;
                case UserSorting.UsernameAsc:
                    model.Entities = model.Entities.OrderBy(u => u.Username).ToList();
                    break;
                case UserSorting.UsernameDesc:
                    model.Entities = model.Entities.OrderByDescending(u => u.Username).ToList();
                    break;
                case UserSorting.EmailAsc:
                    model.Entities = model.Entities.OrderBy(u => u.Email).ToList();
                    break;
                case UserSorting.EmailDesc:
                    model.Entities = model.Entities.OrderByDescending(u => u.Email).ToList();
                    break;
                default:
                    model.Entities = model.Entities.OrderBy(u => u.FirstName).ToList();
                    break;
            }

            // Paging

            int pageSize = 3;
            int pageNumber = (model.Page ?? 1);

            model.PagedUsers = new PagedList<User>(model.Entities, pageNumber, pageSize);

            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            UserRepository userRepo = new UserRepository();

            User user = userRepo.GetById(id.Value);

            if (user == null)
            {
                return HttpNotFound();
            }

            UsersDetailsVM u = new UsersDetailsVM()
            {
                ID = user.ID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email
            };

            if (user.ImageName != null)
            {
                u.ImageName = user.ImageName;
            }

            return View(u);
        }

        public ActionResult CreateEdit(int? id)
        {
            if (id == null) // Create
            {
                return View(new UsersCreateEditVM());
            }

            if (id > 0) // Edit
            {
                UserRepository userRepo = new UserRepository();

                User user = userRepo.GetById(id.Value);

                if (user == null)
                {
                    return HttpNotFound();
                }

                UsersCreateEditVM u = new UsersCreateEditVM()
                {
                    ID = user.ID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin
                };

                if (user.ImageName != null)
                {
                    u.ImageName = user.ImageName;
                }

                return View(u);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(UsersCreateEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user;

            UserRepository userRepo = new UserRepository();

            if (model.ID > 0) // Edit
            {
                user = userRepo.GetById(model.ID);

                if (user == null)
                {
                    return HttpNotFound();
                }
            }

            else // Create
            {
                user = new User();
            }

            if (model.Image != null && model.Image.ContentLength > 0)
            {
                if (model.Image.ContentType.Contains("image"))
                {
                    model.ImageName = Path.GetFileName(model.Image.FileName);
                    model.ImageName = model.Username + Path.GetExtension(model.ImageName);
                    string imagePath = Server.MapPath("~/Images/" + model.ImageName);
                    model.Image.SaveAs(imagePath);

                    user.ImageName = model.ImageName;
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Uploaded file isn't an image!");
                    return View(model);
                }
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Password = model.Password;
            user.IsAdmin = model.IsAdmin;

            bool userDoesntExist = true;

            if (model.Username != user.Username)
            {
                User userUsername = userRepo.GetAll(us => us.Username == model.Username).FirstOrDefault();

                if (userUsername != null)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    userDoesntExist = false;
                }

                user.Username = model.Username;
            }

            if (model.Email != user.Email)
            {
                User userEmail = userRepo.GetAll(us => us.Email == model.Email).FirstOrDefault();

                if (userEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    userDoesntExist = false;
                }

                user.Email = model.Email;
            }

            if (userDoesntExist)
            {
                userRepo.Save(user);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null || AuthenticationService.LoggedUser.ID == id)
            {
                return RedirectToAction("Index");
            }

            UserRepository userRepo = new UserRepository();

            User user = userRepo.GetById(id.Value);

            if (user == null)
            {
                return HttpNotFound();
            }

            UsersDetailsVM u = new UsersDetailsVM()
            {
                ID = user.ID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email
            };

            if (user.ImageName != null)
            {
                u.ImageName = user.ImageName;
            }

            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            UserRepository userRepo = new UserRepository();

            User user = userRepo.GetById(id.Value);

            if (id != AuthenticationService.LoggedUser.ID)
            {
                userRepo.Delete(user);

                if (user.ImageName != null)
                {
                    string imagePath = Request.MapPath("~/Images/" + user.ImageName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            else
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeletePicture(string imageName, int? id)
        {
            string imagePath = Request.MapPath("~/Images/" + imageName);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);

                UserRepository userRepo = new UserRepository();

                User user = userRepo.GetById(id.Value);

                user.ImageName = null;

                userRepo.Save(user);

                return RedirectToAction("CreateEdit", new { id = id.Value });
            }

            return HttpNotFound();
        }
    }
}