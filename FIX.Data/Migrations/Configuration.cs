namespace FIX.Data.Migrations
{
    using Core.Data;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FIX.Data.FIXDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FIX.Data.FIXDbContext context)
        {
            //context.Set<Gender>().AddOrUpdate(
            //    new Gender() { GenderId = 1, Description = "Male", CreatedTimestamp = DateTime.Now },
            //    new Gender() { GenderId = 2, Description = "Female", CreatedTimestamp = DateTime.Now },
            //    new Gender() { GenderId = 3, Description = "Other", CreatedTimestamp = DateTime.Now }
            //);

            //context.Set<Role>().AddOrUpdate(
            //    new Role() { RoleName = "Admin", CreatedTimestamp = DateTime.Now },
            //    new Role() { RoleName = "User", CreatedTimestamp = DateTime.Now }
            //);

            //context.Set<User>().AddOrUpdate(
            //    new User() { Username = "admin", Password = "admin", Email = "aarontee.tech@gmail.com", CreatedTimestamp = DateTime.Now, HasAcceptedTerms = false, HasEmailVerified = false }
            //);
        }
    }
}
