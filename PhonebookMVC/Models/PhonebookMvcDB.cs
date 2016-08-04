using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PhonebookMVC.Models
{
    public class PhonebookMvcDB : DbContext
    {
        public PhonebookMvcDB() : base("PhonebookMvcDB")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Phone> Phones { get; set; }

        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}