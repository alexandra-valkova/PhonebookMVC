using PhonebookMVC.Models;
using PhonebookMVC.Repositories;
using PhonebookMVC.Services;
using PhonebookMVC.ViewModels.Groups;
using System.Web.Mvc;

namespace PhonebookMVC.Controllers
{
    public class GroupsController : Controller
    {
        public ActionResult Index()
        {
            GroupsListVM model = new GroupsListVM();

            TryUpdateModel(model);

            GroupRepository groupRepo = new GroupRepository();

            model.Entities = groupRepo.GetAll(g => g.UserID == AuthenticationService.LoggedUser.ID);

            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                GroupRepository groupRepo = new GroupRepository();

                Group group = groupRepo.GetById(id.Value);

                if (group != null && group.UserID == AuthenticationService.LoggedUser.ID)
                {
                    GroupsCreateEditVM g = new GroupsCreateEditVM()
                    {
                        ID = group.ID,
                        Name = group.Name,
                        UserID = group.UserID
                    };

                    return View(g);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateEdit(int? id)
        {
            if (id == null) // Create
            {
                return View(new GroupsCreateEditVM()
                {
                    UserID = AuthenticationService.LoggedUser.ID
                });
            }

            else if (id > 0) // Edit
            {
                GroupRepository groupRepo = new GroupRepository();

                Group group = groupRepo.GetById(id.Value);

                if (group != null && group.UserID == AuthenticationService.LoggedUser.ID)
                {
                    GroupsCreateEditVM g = new GroupsCreateEditVM()
                    {
                        ID = group.ID,
                        Name = group.Name,
                        UserID = group.UserID
                    };

                    return View(g);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(GroupsCreateEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Group group;

            GroupRepository groupRepo = new GroupRepository();

            if (model.ID > 0) // Edit
            {
                group = groupRepo.GetById(model.ID);

                if (group == null || group.UserID != model.UserID)
                {
                    return HttpNotFound();
                }
            }

            else // Create
            {
                group = new Group()
                {
                    UserID = AuthenticationService.LoggedUser.ID
                };
            }

            if (group.UserID == AuthenticationService.LoggedUser.ID)
            {
                group.Name = model.Name;

                groupRepo.Save(group);

                return RedirectToAction("Index");
            }

            return HttpNotFound();
        }

        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                GroupRepository groupRepo = new GroupRepository();

                Group group = groupRepo.GetById(id.Value);

                if (group != null && group.UserID == AuthenticationService.LoggedUser.ID)
                {
                    GroupsCreateEditVM g = new GroupsCreateEditVM()
                    {
                        ID = group.ID,
                        Name = group.Name
                    };

                    return View(g);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                GroupRepository groupRepo = new GroupRepository();

                Group group = groupRepo.GetById(id.Value);

                if (group != null && group.UserID == AuthenticationService.LoggedUser.ID)
                {
                    group.Contacts.Clear();
                    groupRepo.Delete(group);

                    return RedirectToAction("Index");
                }
            }

            return HttpNotFound();
        }
    }
}