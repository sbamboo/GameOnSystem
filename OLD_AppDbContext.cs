/*
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GameOnSystem {

    internal class Option {
        public int Id { get; set; } // Primary Key
        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal class Edition {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Theme { get; set; }
        public bool IsActive { get; set; }
    }

    internal class User {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProtected { get; set; }
    }

    internal class UserCategory {
        public int Id { get; set; } // Primary Key
        public int? UserID { get; set; } // Foreign Key to User
        public string Category { get; set; }
    }

    internal class Group {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
    }

    internal class ReturnedGroup {

        public int Id { get; set; } // Primary Key
        public string? Name { get; set; }
        public string? GameName { get; set; }
        public string? GameUrl { get; set; }
        public string? GameBannerUrl { get; set; }
        public List<string>? Members { get; set; }
        public Dictionary<string, int>? Grades { get; set; }
    }

    internal class ReturnedUser {
        public int ID { get; set; } // Primary Key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Categories { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProtected { get; set; }
    }

    internal class GroupMember {
        public int ID { get; set; } // Primary Key
        public string Name { get; set; }
        public int? GroupID { get; set; } // Foreign Key to Group
    }

    internal class GroupGrade {
        public int ID { get; set; } // Primary Key
        public string Category { get; set; }
        private int value { get; set; }
        public int Value { get { return value; } set { value = Math.Clamp(value, 0, 6); } }
        public int? GroupID { get; set; } // Foreign Key to Group
    }

    internal class AppDbContext : DbContext {

        private string format_version = "1";

        private DbSet<Option> Options { get; set; }
        private DbSet<Edition> Editions { get; set; }
        private DbSet<User> Users { get; set; }
        private DbSet<UserCategory> UserCategories { get; set; }
        private DbSet<Group> Groups { get; set; }
        private DbSet<GroupMember> GroupMembers { get; set; }
        private DbSet<GroupGrade> GroupGrades { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=gameon_v1.db");
        }

        public AppDbContext() {
            Database.EnsureCreated();

            // Set format
            var format = Options.FirstOrDefault(o => o.Key == "format");
            if (format == null) {
                Options.Add(new Option { Key = "format", Value = format_version });
            } else {

                if (format.Value != format_version) {
                    throw new Exception("Database format is not supported by this version of the application!");
                }

                format.Value = format_version;
            }

            // Ensure admin user
            // Match Users with email "admin" and IsProtected true
            var admin_user = Users.FirstOrDefault(u => u.Email == "admin" && u.IsProtected == true);
            if (admin_user == null) {
                Users.Add(new User { Name = "Admin", Email = "admin", Password = "admin", IsAdmin = true, IsProtected = true });
            }

            this.SaveChanges();
        }


        // METHODS //
        public void MarkAsInited() {
            var inited = Options.FirstOrDefault(o => o.Key == "inited");
            if (inited == null) {
                Options.Add(new Option { Key = "inited", Value = "true" });
            } else {
                inited.Value = "true";
            }
            this.SaveChanges();
        }

        public void MarkAsNotInited() {
            var inited = Options.FirstOrDefault(o => o.Key == "inited");
            if (inited != null) {
                inited.Value = "false";
            }
            this.SaveChanges();
        }

        public bool IsInited() {
            var inited = Options.FirstOrDefault(o => o.Key == "inited");
            if (inited != null) {
                return inited.Value == "true";
            }
            return false;
        }

        public Edition? GetEdition(int id) {
            // If Editions.Any() find the first edition that has the id and return it, else return null result
            if (Editions.Any()) {
                var edition = Editions.FirstOrDefault(e => e.Id == id);
                if (edition != null) {
                    return edition;
                }
            }
            return null;
        }

        public int? SetEdition(int? id, string? Name, string? Theme, bool? IsActive) {
            var edition = Editions.FirstOrDefault(e => e.Id == id);

            if (id == null) {
                id = Editions.Count() + 1;
                Editions.Add(new Edition { Id = id ?? 0, Name = Name ?? "", Theme = Theme ?? "", IsActive = IsActive ?? false });
            } else {

                if (edition == null) {
                    Editions.Add(new Edition { Id = id ?? 0, Name = Name ?? "", Theme = Theme ?? "", IsActive = IsActive ?? false });
                } else {
                    if (Name != null) {
                        edition.Name = Name;
                    }
                    if (Theme != null) {
                        edition.Theme = Theme;
                    }
                    if (IsActive != null) {
                        edition.IsActive = IsActive ?? false;
                    }
                }
            }

            this.SaveChanges();
            return id;
        }

        public int AddEdition(string Name, string Theme, bool IsActive) {
            var id = Editions.Count() + 1;
            Editions.Add(new Edition { Id = id, Name = Name, Theme = Theme, IsActive = IsActive });
            this.SaveChanges();
            return id;
        }

        public bool ModifyEdition(int id, string? Name, string? Theme, bool? IsActive) {
            var edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                if (Name != null) {
                    edition.Name = Name;
                }
                if (Theme != null) {
                    edition.Theme = Theme;
                }
                if (IsActive != null) {
                    edition.IsActive = IsActive ?? false;
                }
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public void RemoveEdition(int id) {
            var edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                Editions.Remove(edition);
            }

            this.SaveChanges();
        }

        public bool HasEdition(int id) {
            if (Editions.Any()) {
                var edition = Editions.FirstOrDefault(e => e.Id == id);
                if (edition != null) {
                    return true;
                }
            }
            return false;
        }

        public bool ActivateEdition(int id) {
            var edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                edition.IsActive = true;
                // Deactivate all other editions
                foreach (var e in Editions) {
                    if (e.Id != id) {
                        e.IsActive = false;
                    }
                }
                // Save
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool DeactivateEdition(int id) {
            var edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                edition.IsActive = false;
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Edition>? GetActiveEdtions() {
            if (Editions.Any()) {
                return Editions.Where(e => e.IsActive == true).ToList();
            }
            return null;
        }

        public List<Edition> GetEditions() {
            return Editions.ToList();
        }

        public ReturnedUser? GetUser(int id) {
            if (Users.Any()) {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user != null) {
                    var categories = UserCategories
                        .Where(c => c.UserID == id)
                        .Select(c => c.Category)
                        .ToList();
                    return new ReturnedUser() {
                        ID = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Password = user.Password,
                        Categories = categories,
                        IsAdmin = user.IsAdmin,
                        IsProtected = user.IsProtected
                    };
                }
            }
            return null;
        }

        public int? SetUser(int? id, string? Name, string? Email, string? Password, List<string>? Categories, bool? IsAdmin) {
            var user = Users.FirstOrDefault(u => u.Id == id);

            if (id == null) {
                id = Editions.Count() + 1;
                Users.Add(new User { Id = id ?? 0, Name = Name ?? "", Email = Email ?? "", Password = Password ?? "", IsAdmin = IsAdmin ?? false });
                // Add category entries
                if (Categories != null) {
                    foreach (var category in Categories) {
                        UserCategories.Add(new UserCategory { UserID = id, Category = category });
                    }
                }
            } else {
                if (user == null) {
                    Users.Add(new User { Id = id ?? 0, Name = Name ?? "", Email = Email ?? "", Password = Password ?? "", IsAdmin = IsAdmin ?? false });
                    // Add category entries
                    if (Categories != null) {
                        foreach (var category in Categories) {
                            UserCategories.Add(new UserCategory { UserID = id, Category = category });
                        }
                    }
                } else {
                    if (Name != null) {
                        user.Name = Name;
                    }
                    if (Email != null) {
                        user.Email = Email;
                    }
                    if (Password != null) {
                        user.Password = Password;
                    }
                    if (IsAdmin != null) {
                        user.IsAdmin = IsAdmin ?? false;
                    }
                    // Update category entries, by finding al categories for this userId, then iterate the given categories unless null and check if exists, if not create else update
                    if (Categories != null) {
                        var userCategories = UserCategories
                            .Where(c => c.UserID == id)
                            .ToList();
                        foreach (var category in Categories) {
                            var userCategory = userCategories.FirstOrDefault(c => c.Category == category);
                            if (userCategory == null) {
                                UserCategories.Add(new UserCategory { UserID = id, Category = category });
                            } else {
                                userCategory.Category = category;
                            }
                        }
                    }
                }
            }

            this.SaveChanges();
            return id;
        }

        public int AddUser(string Name, string Email, string Password, List<string> Categories, bool? IsAdmin) {
            if (IsAdmin == null) {
                IsAdmin = false;
            }
            var id = Users.Count() + 1;
            Users.Add(new User { Id = id, Name = Name, Email = Email, Password = Password, IsAdmin = IsAdmin ?? false });

            foreach (var category in Categories) {
                UserCategories.Add(new UserCategory { UserID = id, Category = category });
            }

            this.SaveChanges();
            return id;
        }

        public bool ModifyUser(int id, string? Name, string? Email, string? Password, List<string>? Categories, bool? IsAdmin) {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null) {
                if (Name != null) {
                    user.Name = Name;
                }
                if (Email != null) {
                    user.Email = Email;
                }
                if (Password != null) {
                    user.Password = Password;
                }
                if (Categories != null) {
                    var userCategories = UserCategories
                        .Where(c => c.UserID == id)
                        .ToList();
                    foreach (var category in Categories) {
                        var userCategory = userCategories.FirstOrDefault(c => c.Category == category);
                        if (userCategory == null) {
                            UserCategories.Add(new UserCategory { UserID = id, Category = category });
                        } else {
                            userCategory.Category = category;
                        }
                    }
                }
                if (IsAdmin != null) {
                    user.IsAdmin = IsAdmin ?? false;
                }
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public void RemoveUser(int id) {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null) {
                if (user.IsProtected) {
                    throw new AttempedRemoveOnProtected("Attempted remove on protected user!");
                } else {
                    Users.Remove(user);
                    // Remove categories
                    var categories = UserCategories
                        .Where(c => c.UserID == id)
                        .ToList();
                    foreach (var category in categories) {
                        UserCategories.Remove(category);
                    }
                }
            }

            this.SaveChanges();
        }

        public bool? GetUserIsAdmin(int id) {
            if (Users.Any()) {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user != null) {
                    return user.IsAdmin;
                }
            }
            return null;
        }

        public int? ValidateUserLogin(string Email, string Password) {
            if (Users.Any()) {
                var user = Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
                if (user != null) {
                    return user.Id;
                }
            }
            return null;
        }

        public bool HasUser(int id) {
            if (Users.Any()) {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user != null) {
                    return true;
                }
            }
            return false;
        }

        public List<User> GetUsers() {
            return Users.ToList();
        }

        public ReturnedGroup? GetGroup(int id) {
            // For members we need to get the 'GroupMembers' set and filter by the group id, then return as list
            // For grades we need to get the 'GroupGrades' set and filter by the group id, then return as dictionary
            if (Groups.Any()) {
                var group = Groups.FirstOrDefault(g => g.Id == id);
                if (group != null) {
                    var members = GroupMembers
                        .Where(m => m.GroupID == id)
                        .Select(m => m.Name)
                        .ToList();
                    var grades = GroupGrades
                        .Where(g => g.GroupID == id)
                        .ToDictionary(g => g.Category, g => g.Value);
                    return new ReturnedGroup() {
                        Id = group.Id,
                        Name = group.Name,
                        GameName = group.GameName,
                        GameUrl = group.GameUrl,
                        GameBannerUrl = group.GameBannerUrl,
                        Members = members,
                        Grades = grades
                    };
                }
            }
            return null;
        }

        public int? SetGroup(int? id, string? Name, string? GameName, string? GameUrl, string? GameBannerUrl, List<string>? Members, Dictionary<string, int>? Grades) {
            // For members we need to ensure that each member exists in the 'GroupMembers' set with the correct group id, if not add them
            // For grades we need to ensure that each grade exists in the 'GroupGrades' set with the correct group id Category and Value, if not add them
            var group = Groups.FirstOrDefault(g => g.Id == id);

            if (id == null) {
                id = Editions.Count() + 1;
                Groups.Add(new Group { Id = id ?? 0, Name = Name ?? "", GameName = GameName ?? "", GameUrl = GameUrl ?? "", GameBannerUrl = GameBannerUrl ?? "" });
            } else {

                if (group == null) {
                    Groups.Add(new Group { Id = id ?? 0, Name = Name ?? "", GameName = GameName ?? "", GameUrl = GameUrl ?? "", GameBannerUrl = GameBannerUrl ?? "" });
                } else {
                    if (Name != null) {
                        group.Name = Name;
                    }
                    if (GameName != null) {
                        group.GameName = GameName;
                    }
                    if (GameUrl != null) {
                        group.GameUrl = GameUrl;
                    }
                    if (GameBannerUrl != null) {
                        group.GameBannerUrl = GameBannerUrl;
                    }
                }
            }

            if (Members != null) {
                foreach (var member in Members) {
                    var groupMember = GroupMembers.FirstOrDefault(m => m.Name == member && m.GroupID == id);
                    if (groupMember == null) {
                        GroupMembers.Add(new GroupMember { Name = member, GroupID = id });
                    }
                }
            }

            if (Grades != null) {
                foreach (var (category, value) in Grades) {
                    var groupGrade = GroupGrades.FirstOrDefault(g => g.Category == category && g.GroupID == id);
                    if (groupGrade == null) {
                        GroupGrades.Add(new GroupGrade { Category = category, Value = value, GroupID = id });
                    } else {
                        groupGrade.Value = value;
                    }
                }
            }

            this.SaveChanges();
            return id;
        }

        public int AddGroup(string Name, string GameName, string GameUrl, string GameBannerUrl, List<string> Members, Dictionary<string, int> Grades) {
            var id = Groups.Count() + 1;
            Groups.Add(new Group { Id = id, Name = Name, GameName = GameName, GameUrl = GameUrl, GameBannerUrl = GameBannerUrl });
            this.SaveChanges();

            if (Members != null) {
                foreach (var member in Members) {
                    GroupMembers.Add(new GroupMember { Name = member, GroupID = id });
                }
            }
            if (Grades != null) {
                foreach (var (category, value) in Grades) {
                    GroupGrades.Add(new GroupGrade { Category = category, Value = value, GroupID = id });
                }
            }
            this.SaveChanges();
            return id;
        }

        public bool ModifyGroup(int id, string? Name, string? GameName, string? GameUrl, string? GameBannerUrl, List<string>? Members, Dictionary<string, int>? Grades) {
            var group = Groups.FirstOrDefault(g => g.Id == id);
            if (group != null) {
                if (Name != null) {
                    group.Name = Name;
                }
                if (GameName != null) {
                    group.GameName = GameName;
                }
                if (GameUrl != null) {
                    group.GameUrl = GameUrl;
                }
                if (GameBannerUrl != null) {
                    group.GameBannerUrl = GameBannerUrl;
                }
                if (Members != null) {
                    foreach (var member in Members) {
                        var groupMember = GroupMembers.FirstOrDefault(m => m.Name == member && m.GroupID == id);
                        if (groupMember == null) {
                            GroupMembers.Add(new GroupMember { Name = member, GroupID = id });
                        }
                    }
                }
                if (Grades != null) {
                    foreach (var (category, value) in Grades) {
                        var groupGrade = GroupGrades.FirstOrDefault(g => g.Category == category && g.GroupID == id);
                        if (groupGrade == null) {
                            GroupGrades.Add(new GroupGrade { Category = category, Value = value, GroupID = id });
                        } else {
                            groupGrade.Value = value;
                        }
                    }
                }
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public void RemoveGroup(int id) {
            // For members we have to remove each entry in the 'GroupMembers' set that has the group id
            // For grades we have to remove each entry in the 'GroupGrades' set that has the group id
            var group = Groups.FirstOrDefault(g => g.Id == id);
            if (group != null) {
                Groups.Remove(group);
            }

            var members = GroupMembers
                .Where(m => m.GroupID == id)
                .ToList();
            foreach (var member in members) {
                GroupMembers.Remove(member);
            }

            var grades = GroupGrades
                .Where(g => g.GroupID == id)
                .ToList();
            foreach (var grade in grades) {
                GroupGrades.Remove(grade);
            }

            this.SaveChanges();
        }
        public bool HasGroup(int id) {
            if (Groups.Any()) {
                var group = Groups.FirstOrDefault(g => g.Id == id);
                if (group != null) {
                    return true;
                }
            }
            return false;
        }

        public List<ReturnedGroup> GetGroups() {
            // For each group we need to get the 'GroupMembers' set and filter by the group id, then return as list
            // For each group we need to get the 'GroupGrades' set and filter by the group id, then return as dictionary
            var groups = new List<ReturnedGroup>();
            if (Groups.Any()) {
                foreach (var group in Groups) {
                    var members = GroupMembers
                        .Where(m => m.GroupID == group.Id)
                        .Select(m => m.Name)
                        .ToList();
                    var grades = GroupGrades
                        .Where(g => g.GroupID == group.Id)
                        .ToDictionary(g => g.Category, g => g.Value);
                    groups.Add(new ReturnedGroup() {
                        Id = group.Id,
                        Name = group.Name,
                        GameName = group.GameName,
                        GameUrl = group.GameUrl,
                        GameBannerUrl = group.GameBannerUrl,
                        Members = members,
                        Grades = grades
                    });
                }
            }
            return groups;
        }

        public int? GetTotalGradeForGroup(int id) {
            if (GroupGrades.Any()) {
                var grades = GroupGrades
                    .Where(g => g.GroupID == id)
                    .Select(g => g.Value)
                    .ToList();
                return grades.Sum();
            }
            return null;
        }
    }
}
*/