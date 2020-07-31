using PagedList;
using PhonebookMVC.Filters;
using PhonebookMVC.Models;
using PhonebookMVC.Repositories;
using PhonebookMVC.Services;
using PhonebookMVC.ViewModels;
using PhonebookMVC.ViewModels.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace PhonebookMVC.Controllers
{
    [Authenticate]
    public class ContactsController : Controller
    {
        public ActionResult Index()
        {
            ContactsListVM model = new ContactsListVM();

            TryUpdateModel(model);

            ContactRepository contactRepo = new ContactRepository();

            User user = AuthenticationService.LoggedUser;

            Expression<Func<Contact, bool>> filter = null;

            if (!string.IsNullOrEmpty(model.SearchString)) // With Searching
            {
                string[] searchArray = model.SearchString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                filter = c => (c.UserID == user.ID) && (searchArray.Any(word => c.FirstName.Contains(word))
                                                     || searchArray.Any(word => c.LastName.Contains(word))
                                                     || searchArray.Any(word => c.Email.Contains(word)));
            }

            else // Without Searching
            {
                filter = c => c.UserID == user.ID;
            }

            model.Entities = contactRepo.GetAll(filter);

            // Sorting

            model.RouteDictionary = new RouteValueDictionary
            {
                { "SearchString", model.SearchString }
            };

            if (model.SortOrder == null)
            {
                model.SortOrder = ContactSorting.FirstNameAsc;
            }

            switch (model.SortOrder)
            {
                case ContactSorting.FirstNameAsc:
                default:
                    model.Entities = model.Entities.OrderBy(c => c.FirstName).ToList();
                    break;
                case ContactSorting.FirstNameDesc:
                    model.Entities = model.Entities.OrderByDescending(c => c.FirstName).ToList();
                    break;
                case ContactSorting.LastNameAsc:
                    model.Entities = model.Entities.OrderBy(c => c.LastName).ToList();
                    break;
                case ContactSorting.LastNameDesc:
                    model.Entities = model.Entities.OrderByDescending(c => c.LastName).ToList();
                    break;
                case ContactSorting.EmailAsc:
                    model.Entities = model.Entities.OrderBy(c => c.Email).ToList();
                    break;
                case ContactSorting.EmailDesc:
                    model.Entities = model.Entities.OrderByDescending(c => c.Email).ToList();
                    break;
            }

            // Paging

            int pageSize = 2;
            int pageNumber = (model.Page ?? 1);

            model.PagedContacts = new PagedList<Contact>(model.Entities, pageNumber, pageSize);

            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                ContactRepository contactRepo = new ContactRepository();

                Contact contact = contactRepo.GetById(id.Value);

                if (contact != null && contact.UserID == AuthenticationService.LoggedUser.ID)
                {
                    ContactCreateEditVM model = new ContactCreateEditVM()
                    {
                        ID = contact.ID,
                        FirstName = contact.FirstName,
                        LastName = contact.LastName,
                        Email = contact.Email
                    };

                    GroupRepository groupRepo = new GroupRepository();

                    List<Group> contactGroups = contactRepo.GetAll(c => c.ID == id).First().Groups;

                    if (contactGroups.Count > 0)
                    {
                        List<CheckBoxListItem> checkBoxListItems = new List<CheckBoxListItem>();

                        foreach (Group group in contactGroups)
                        {
                            checkBoxListItems.Add(new CheckBoxListItem()
                            {
                                Text = group.Name
                            });
                        }

                        model.Groups = checkBoxListItems;
                    }

                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateEdit(int? id)
        {
            if (id == null) // Create
            {
                ContactCreateEditVM model = new ContactCreateEditVM()
                {
                    UserID = AuthenticationService.LoggedUser.ID
                };

                GroupRepository groupRepo = new GroupRepository();

                List<Group> allGroups = groupRepo.GetAll(g => g.UserID == AuthenticationService.LoggedUser.ID);

                List<CheckBoxListItem> checkBoxListItems = new List<CheckBoxListItem>();

                foreach (Group group in allGroups)
                {
                    checkBoxListItems.Add(new CheckBoxListItem()
                    {
                        ID = group.ID,
                        Text = group.Name,
                        IsChecked = false
                    });
                }

                model.Groups = checkBoxListItems;

                return View(model);
            }

            if (id > 0) // Edit
            {
                ContactRepository contactRepo = new ContactRepository();

                Contact contact = contactRepo.GetById(id.Value);

                if (contact != null && contact.UserID == AuthenticationService.LoggedUser.ID)
                {
                    ContactCreateEditVM model = new ContactCreateEditVM()
                    {
                        ID = contact.ID,
                        UserID = contact.UserID,
                        FirstName = contact.FirstName,
                        LastName = contact.LastName,
                        Email = contact.Email
                    };

                    GroupRepository groupRepo = new GroupRepository();

                    List<Group> allGroups = groupRepo.GetAll(g => g.UserID == AuthenticationService.LoggedUser.ID);
                    List<Group> contactGroups = contactRepo.GetAll(c => c.ID == id).First().Groups;

                    List<CheckBoxListItem> checkBoxListItems = new List<CheckBoxListItem>();

                    foreach (Group group in allGroups)
                    {
                        checkBoxListItems.Add(new CheckBoxListItem()
                        {
                            ID = group.ID,
                            Text = group.Name,
                            IsChecked = contactGroups.Where(g => g.ID == group.ID).Any()
                        });
                    }

                    model.Groups = checkBoxListItems;

                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(ContactCreateEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Contact contact;

            ContactRepository contactRepo = new ContactRepository();

            if (model.ID > 0) // Edit
            {
                contact = contactRepo.GetById(model.ID);

                if (contact == null || contact.UserID != model.UserID)
                {
                    return HttpNotFound();
                }
            }

            else // Create
            {
                contact = new Contact()
                {
                    UserID = model.UserID,
                    Groups = new List<Group>()
                };
            }

            if (contact.UserID == AuthenticationService.LoggedUser.ID)
            {
                if (model.GroupIds != null)
                {
                    GroupRepository groupRepo = new GroupRepository
                    {
                        Context = contactRepo.Context
                    };
                    groupRepo.DbSet = groupRepo.Context.Set<Group>();

                    if (contact.Groups != null)
                    {
                        contact.Groups.Clear();
                    }

                    foreach (int groupId in model.GroupIds)
                    {
                        Group group = groupRepo.GetById(groupId);
                        contact.Groups.Add(group);
                    }
                }

                else
                {
                    contact.Groups.Clear();
                }

                contact.FirstName = model.FirstName;
                contact.LastName = model.LastName;
                contact.Email = model.Email;

                contactRepo.Save(contact);

                return RedirectToAction("Index");
            }

            return HttpNotFound();
        }

        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                ContactRepository contactRepo = new ContactRepository();

                Contact contact = contactRepo.GetById(id.Value);

                if (contact != null && contact.UserID == AuthenticationService.LoggedUser.ID)
                {
                    ContactCreateEditVM model = new ContactCreateEditVM()
                    {
                        ID = contact.ID,
                        FirstName = contact.FirstName,
                        LastName = contact.LastName,
                        Email = contact.Email
                    };

                    GroupRepository groupRepo = new GroupRepository();

                    List<Group> contactGroups = contactRepo.GetAll(c => c.ID == id).First().Groups;

                    if (contactGroups.Count > 0)
                    {
                        List<CheckBoxListItem> checkBoxListItems = new List<CheckBoxListItem>();

                        foreach (Group group in contactGroups)
                        {
                            checkBoxListItems.Add(new CheckBoxListItem()
                            {
                                Text = group.Name
                            });
                        }

                        model.Groups = checkBoxListItems;
                    }

                    return View(model);
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
                ContactRepository contactRepo = new ContactRepository();

                Contact contact = contactRepo.GetById(id.Value);

                if (contact != null && contact.UserID == AuthenticationService.LoggedUser.ID)
                {
                    contact.Groups.Clear();
                    contactRepo.Delete(contact);

                    return RedirectToAction("Index");
                }
            }

            return HttpNotFound();
        }
    }
}
