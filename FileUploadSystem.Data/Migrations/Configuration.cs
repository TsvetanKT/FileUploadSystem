namespace FileUploadSystem.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FileUploadSystem.Models;
    using FileUploadSystem.Common;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.IO;
    using System.Reflection;
    using System.Web;

    public sealed class Configuration : DbMigrationsConfiguration<FileUploadSystem.Data.FileUploadSystemDbContext>
    {
        private UserManager<User> userManager;

        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(FileUploadSystem.Data.FileUploadSystemDbContext context)
        {
            //GlobalConstants.UploadDirectory = AssemblyHelpers.UploadDirectoryLocation(Assembly.GetExecutingAssembly());
            GlobalConstants.UploadDirectory = AssemblyHelpers.GetDirectoryForAssembyl(Assembly.GetExecutingAssembly()) + "/Uploads";
            //var test = GlobalConstants.UploadDirectory;

            //this.userManager = new UserManager<User>(new UserStore<User>(context));
            //this.SeedRoles(context);
            //this.SeedUsers(context);
            //this.SeedFiles(context);
            context.SaveChanges();
        }

        private void SeedRoles(FileUploadSystemDbContext context)
        {
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(GlobalConstants.AdminRole));
            context.SaveChanges();
        }

        private void SeedUsers(FileUploadSystemDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var users = new User[]
            {
                new User
                {
                    Email = "test1@test.com",
                    UserName = "TestUser",
                    SecurityStamp = Guid.NewGuid().ToString() 
                },
                new User
                {
                    Email = "test2@test.com",
                    UserName = "AnotherUser",
                    SecurityStamp = Guid.NewGuid().ToString() 
                },
                new User
                {
                    Email = "test3@gmail.com",
                    UserName = "TestUser3",
                    SecurityStamp = Guid.NewGuid().ToString() 
                }
            };

            for (int i = 0; i < users.Length; i++)
            {
                this.userManager.Create(users[i], "pass123");
            }

            var admin = new User { Email = "kokoZ@koko.com", UserName = "Administrator" };
            this.userManager.Create(admin, "kokokoko");

            this.userManager.AddToRole(admin.Id, GlobalConstants.AdminRole);
        }

        private void SeedFiles(FileUploadSystemDbContext context)
        {
            FileManipulator.GenerateStorage();

            if (context.Files.Any())
            {
                return;
            }

            var sampleData = this.GetSampleFiles("Cats");
            var admin = context.Users.Where(u => u.UserName == "Administrator").FirstOrDefault();

            AddFilesToUser(context, sampleData, admin);

            var dir = new DirectoryEntity
            {
                Name = "Directory Test",
                LocationId = -1,
                Owner = admin,
                OwnerId = admin.Id
            };

            admin.Files.Add(dir);
            context.SaveChanges();

            var innerDirectory = new DirectoryEntity
            {
                Name = "Inner Directory Test",
                LocationId = dir.Id,
                Owner = admin,
                OwnerId = admin.Id
            };

            dir.FilesContained.Add(innerDirectory);
            context.SaveChanges();

            var someFileAgain = new BinaryFile
                    {
                        Name = "someFileAgain" + Path.GetExtension(sampleData[0]),
                        LocationId = innerDirectory.Id,
                        Owner = admin,
                        OwnerId = admin.Id,
                        Size = new FileInfo(sampleData[0]).Length,
                        Type = Path.GetExtension(sampleData[0])
                    };

            innerDirectory.FilesContained.Add(someFileAgain);
            context.SaveChanges();

            FileManipulator.UploadFile(sampleData[0], someFileAgain.Id, Path.GetExtension(sampleData[0]));

            var anotherUSer = context.Users.Where(u => u.UserName == "AnotherUser").FirstOrDefault();
            var dogs = this.GetSampleFiles("Dogs");
            AddFilesToUser(context, dogs, anotherUSer);
            context.SaveChanges();
        }

        private static void AddFilesToUser(FileUploadSystemDbContext context, string[] locations, User currentUser)
        {
            for (int i = 0; i < locations.Length; i++)
            {
                var extension = Path.GetExtension(locations[i]);
                var someFile = new BinaryFile
                {
                    Name = Path.GetFileName(locations[i]),
                    LocationId = -1,
                    Owner = currentUser,
                    OwnerId = currentUser.Id,
                    Size = new FileInfo(locations[i]).Length,
                    Type = extension
                };

                currentUser.Files.Add(someFile);
                context.SaveChanges();

                FileManipulator.UploadFile(locations[i], someFile.Id, extension);
            }
        }

        private string[] GetSampleFiles(string subDir)
        {
            subDir = subDir == null  ? string.Empty : "/" + subDir;

            var directory = AssemblyHelpers.GetDirectoryForAssembyl(Assembly.GetExecutingAssembly());
            string[] locations = Directory.GetFiles(directory + "/Migrations/SeedFiles" + subDir);

            return locations;
        }
    }
}
