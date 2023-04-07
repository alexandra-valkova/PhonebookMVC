namespace PhonebookMVC.Migrations
{
    using Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<PhonebookMvcDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(PhonebookMvcDB context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            context.Users.AddOrUpdate(
                u => u.Username,
                new User
                {
                    FirstName = "Admin",
                    LastName = "Adminov",
                    Username = "admin",
                    Password = "password",
                    IsAdmin = true
                }
            );
        }
    }
}
