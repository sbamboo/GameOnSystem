using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameOnSystem {
  
    internal class Option {
        public int ID { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
    internal class Edition {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public bool IsActive { get; set; }
    }
    internal class User {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProtected { get; set; }
    } 
    internal class Category {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CategoryType { get; set; }
    }
    internal class UserCategory {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
    }
    internal class Group {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameURL { get; set; }
        public string GameBannerUrl { get; set; }
        public int EditionId { get; set; }
    }
    internal class GroupMember {
        public int ID { get; set; }
        public int Name { get; set; }
        public int GroupId { get; set; }
    }
    internal class Grade {
        public int ID { get; set; }
        public int Value { get; set; }
        public int GroupId { get; set; }
        public int UserCategoryId { get; set; }
    }

    internal class AppDbContext : DbContext {
        private string format_version = "1";
        public DbSet<Option> Options { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserCategory> UserCategories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Grade> Grades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=gameon_v1.db");
        }

        public AppDbContext() {
            Database.EnsureCreated();

            // DB-Version Check
            if (!this.HasOption("version")) {
                this.SetOption("version", format_version);
            } else {
                string? version = this.GetOption("version");
                if (version != format_version) {
                    throw new Exception($"Database version mismatch: {version} != {format_version}");
                }
            }

            // Options
            if (!this.HasOption("num_grade_min_value")) {
                this.SetOption("num_grade_min_value", "1");
            }
            if (!this.HasOption("num_grade_max_value")) {
                this.SetOption("num_grade_max_value", "6");
            }

            // Ensure admin user
            // Match Users with email "admin" and IsProtected true
            var admin_user = Users.FirstOrDefault(u => u.Email == "admin" && u.IsProtected == true);
            if (admin_user == null) {
                Users.Add(new User { Name = "Admin", Email = "admin", Password = "admin", IsAdmin = true, IsProtected = true });
            }
        }

        #region Methods
        public void SetOption(string field, string value) {
            Option? option = Options.FirstOrDefault(o => o.Field == field);
            if (option != null) {
                option.Value = value;
                Options.Update(option);
            } else {
                Options.Add(new Option { Field = field, Value = value });
            }
            this.SaveChanges();
        }
        public string? GetOption(string field) {
            Option? option = Options.FirstOrDefault(o => o.Field == field);
            if (option != null) {
                return option.Value;
            }
            return null;
        }
        public bool HasOption(string field) {
            return Options.Any(o => o.Field == field);
        }

        public void MarkAsInited() {
            this.SetOption("is_inited", "true");
        }
        public void MarkAsNotInited() {
            this.SetOption("is_inited", "false");
        }
        public bool IsInited() {
            return this.GetOption("is_inited") == "true";
        }

        // Editions
        public Edition? GetEdition(int id) {
            return Editions.FirstOrDefault(e => e.ID == id);
        }
        public List<Edition> GetEditions() {
            return Editions.ToList();
        }
        public int? AddEdition(string name, string theme, bool isActive = false) {
            // Return id after creation, if exists just return id of existing entry
            Edition? edition = Editions.FirstOrDefault(e => e.Name == name && e.Theme == theme);
            if (edition != null) {
                return edition.ID;
            }
            Edition newEdition = new Edition { Name = name, Theme = theme, IsActive = isActive };
            Editions.Add(newEdition);

            this.SaveChanges();
            return newEdition.ID;
        }
        public bool ModifyEdition(int id, string? name = null, string? theme = null, bool? isActive = null) {
            Edition? edition = Editions.FirstOrDefault(e => e.ID == id);
            if (edition != null) {
                if (name != null) {
                    edition.Name = name;
                }
                if (theme != null) {
                    edition.Theme = theme;
                }
                if (isActive != null) {
                    edition.IsActive = (bool)isActive;
                }
                Editions.Update(edition);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveEdition(int id) {
            Edition? edition = Editions.FirstOrDefault(e => e.ID == id);
            if (edition != null) {
                Editions.Remove(edition);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public List<Edition> GetActiveEdtions() {
            return Editions.Where(e => e.IsActive == true).ToList();
        }
        public bool MarkEditionAsActive(int id) {
            // get the edition matching the id and set it active
            Edition? edition = Editions.FirstOrDefault(e => e.ID == id);
            if (edition != null) {
                edition.IsActive = true;
                Editions.Update(edition);

                // Get al editions not matching id and that is active set them as inactive
                List<Edition> editions = Editions.Where(e => e.ID != id && e.IsActive == true).ToList();
                foreach (Edition e in editions) {
                    e.IsActive = false;
                    Editions.Update(e);
                }

                // Save and return
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool MarkEditionAsInactive(int id) {
            // get the edition matching the id and set it inactive
            Edition? edition = Editions.FirstOrDefault(e => e.ID == id);
            if (edition != null) {
                edition.IsActive = false;
                Editions.Update(edition);

                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool IsEditionActive(int id) {
            Edition? edition = Editions.FirstOrDefault(e => e.ID == id);
            if (edition != null) {
                return edition.IsActive;
            }
            return false;
        }

        // Users
        public User? GetUser(int id) {
            return Users.FirstOrDefault(u => u.ID == id);
        }
        public List<User> GetUsers() {
            return Users.ToList();
        }
        public int? AddUser(string name, string email, string password, List<int> focusCategories, bool isAdmin = false) {
            // Return id after creation, if exists just return id of existing entry
            User? user = Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null) {
                return user.ID;
            }
            User newUser = new User { Name = name, Email = email, Password = password, IsAdmin = isAdmin };
            Users.Add(newUser);

            // Add focus categories to UserCategory table
            foreach (int category_id in focusCategories) {
                UserCategories.Add(new UserCategory { UserID = newUser.ID, CategoryID = category_id });
            }

            this.SaveChanges();
            return newUser.ID;
        }
        public bool ModifyUser(int id, string? name = null, string? email = null, string? password = null, List<int>? focusCategories = null, bool isAdmin = false) {
            User? user = Users.FirstOrDefault(u => u.ID == id);
            if (user != null) {
                if (name != null) {
                    user.Name = name;
                }
                if (email != null) {
                    user.Email = email;
                }
                if (password != null) {
                    user.Password = password;
                }
                if (focusCategories != null) {
                    // Remove all focus categories
                    List<UserCategory> userCategories = UserCategories.Where(uc => uc.UserID == id).ToList();
                    foreach (UserCategory uc in userCategories) {
                        UserCategories.Remove(uc);
                    }
                    // Add new focus categories
                    foreach (int category_id in focusCategories) {
                        UserCategories.Add(new UserCategory { UserID = id, CategoryID = category_id });
                    }
                }
                user.IsAdmin = isAdmin;
                Users.Update(user);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveUser(int id) {
            User? user = Users.FirstOrDefault(u => u.ID == id);
            if (user != null) {
                Users.Remove(user);

                // Remove all focus categories
                List<UserCategory> userCategories = UserCategories.Where(uc => uc.UserID == id).ToList();

                this.SaveChanges();
                return true;
            }
            return false;
        }

        // Categories
        public Category? GetCategory(int id) {
            return Categories.FirstOrDefault(c => c.ID == id);
        }
        public List<Category> GetCategories() {
            return Categories.ToList();
        }
        public int? AddCategory(string name) {
            // Return id after creation, if exists just return id of existing entry
            Category? category = Categories.FirstOrDefault(c => c.Name == name);
            if (category != null) {
                return category.ID;
            }
            Category newCategory = new Category { Name = name };
            Categories.Add(newCategory);
            this.SaveChanges();
            return newCategory.ID;
        }
        public bool ModifyCategory(int id, string? name = null) {
            Category? category = Categories.FirstOrDefault(c => c.ID == id);
            if (category != null) {
                if (name != null) {
                    category.Name = name;
                }
                Categories.Update(category);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveCategory(int id) {
            Category? category = Categories.FirstOrDefault(c => c.ID == id);
            if (category != null) {
                Categories.Remove(category);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        // Groups
        public Group? GetGroup(int id) {
            return Groups.FirstOrDefault(g => g.ID == id);
        }
        public List<Group> GetGroups() {
            return Groups.ToList();
        }
        
        #endregion Methods
    }
}


