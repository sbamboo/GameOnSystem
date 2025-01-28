using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameOnSystem {
    /*

    // Table for storing database options/properties
    internal class Option {
        public int Id { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }

    // Table for storing the editions, i.e theme and name.
    internal class Edition {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public bool IsActive { get; set; }
    }

    // Table for storing the groups and information about their games
    internal class Group {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
        public int EditionId { get; set; } // Foreign key for n-1 relationship with the Edition table
    }

    // Table for storing members of a group
    internal class GroupMember {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; } // Foreign key for 1-n relationship with the Group table
    }

    // Table for storing users for the application
    internal class User {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProtected { get; set; }
    }

    // Table for storing grading categories
    internal class Category {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Table for storing grades, of a category, made by a user and for a group
    internal class Grade {
        // Id, Grade, UserId, GroupId, CategoryId
        public int Id { get; set; }
        public int Value { get; set; }
        public int UserId { get; set; } // Foreign key for n-1 relationship with the User table
        public int GroupId { get; set; } // Foreign key for n-1 relationship with the Group table
        public int CategoryId { get; set; } // Foreign key for n-1 relationship with the Category table
    }

    // Table for

    // Class for a grade that has had it's relations resolved, and thus include the user and category it belongs to. The group is not resolved since this object is returned inside ResolvedGroup.
    internal class ResolvedGrade {
        public int Id { get; set; }
        public int Value { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; } // Unresolved
        public Category Category { get; set; }

    }

    // Class for a group that has had it's relations resolved, and thus include its members and grades per category, each grade is also an instance of ResolvedGrade.
    internal class ResolvedGroup {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
        public int EditionId { get; set; }
        public List<GroupMember> GroupMembers { get; set; }
        public List<ResolvedGrade> Grades { get; set; }
    }

    internal class AppDbContext : DbContext {
        private string format_version = "2";

        public DbSet<Option> Options { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
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
            return Editions.FirstOrDefault(e => e.Id == id);
        }
        public List<Edition> GetEditions() {
            return Editions.ToList();
        }
        public int? AddEdition(string name, string theme, bool? isActive) {
            // Return id after creation, if exists just return id of existing entry
            Edition? edition = Editions.FirstOrDefault(e => e.Name == name && e.Theme == theme);
            if (edition != null) {
                return edition.Id;
            }
            Edition newEdition = new Edition { Name = name, Theme = theme, IsActive = isActive ?? false };
            Editions.Add(newEdition);
            this.SaveChanges();
            return newEdition.Id;
        }
        public bool ModifyEdition(int id, string? name, string? theme, bool? isActive) {
            Edition? edition = Editions.FirstOrDefault(e => e.Id == id);
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
            Edition? edition = Editions.FirstOrDefault(e => e.Id == id);
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
            Edition? edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                edition.IsActive = true;
                Editions.Update(edition);

                // Get al editions not matching id and that is active set them as inactive
                List<Edition> editions = Editions.Where(e => e.Id != id && e.IsActive == true).ToList();
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
            Edition? edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                edition.IsActive = false;
                Editions.Update(edition);

                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool IsEditionActive(int id) {
            Edition? edition = Editions.FirstOrDefault(e => e.Id == id);
            if (edition != null) {
                return edition.IsActive;
            }
            return false;
        }

        public ResolvedGroup? GetGroup(int id) {
            Group? group = Groups.FirstOrDefault(g => g.Id == id);
            if (group != null) {
                List<GroupMember> groupMembers = GroupMembers.Where(gm => gm.GroupId == id).ToList();
                List<ResolvedGrade> grades = Grades.Where(g => g.GroupId == id).Select(g => new ResolvedGrade {
                    Id = g.Id,
                    Value = g.Value,
                    User = Users.FirstOrDefault(u => u.Id == g.UserId),
                    GroupId = g.GroupId,
                    Category = Categories.FirstOrDefault(c => c.Id == g.CategoryId)
                }).ToList();
                return new ResolvedGroup {
                    Id = group.Id,
                    Name = group.Name,
                    GameName = group.GameName,
                    GameUrl = group.GameUrl,
                    GameBannerUrl = group.GameBannerUrl,
                    EditionId = group.EditionId,
                    GroupMembers = groupMembers,
                    Grades = grades
                };
            }
            return null;
        }
        public List<ResolvedGroup> GetGroups() {
            List<ResolvedGroup> resolvedGroups = new List<ResolvedGroup>();
            List<Group> groups = Groups.ToList();
            foreach (Group group in groups) {
                ResolvedGroup? resolvedGroup = GetGroup(group.Id);
                if (resolvedGroup != null) {
                    resolvedGroups.Add(resolvedGroup);
                }
            }
            return resolvedGroups;
        }

        // Dictionary is {<cat.Id>:{<user.Id>:<grade>}} where grade is clamped between option:num_grade_min_value and option:num_grade_max_value
        public int? AddGroup(string name, string gameName, string gameUrl, string gameBannerUrl, int editionId, List<int>? Members, Dictionary<int, Dictionary<int, int>>? GradesPerCategory) {
            int num_grade_min_value = int.Parse(this.GetOption("num_grade_min_value") ?? "");
            int num_grade_max_value = int.Parse(this.GetOption("num_grade_max_value") ?? "");

            // Validate the grade numbers between the min/max
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<int, Dictionary<int, int>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<int, int> grade in cat.Value) {
                        if (grade.Value < num_grade_min_value) {
                            return null; // Return null if grade is below min
                        } else if (grade.Value > num_grade_max_value) {
                            return null; // Return null if grade is above max
                        }
                    }
                }
            }

            // Create the group and get the id after creation
            Group newGroup = new Group { Name = name, GameName = gameName, GameUrl = gameUrl, GameBannerUrl = gameBannerUrl, EditionId = editionId };

            // If members is not null and not length 0, iterate the id's and if any id is missing return null
            if (Members != null && Members.Count > 0) {
                foreach (int member in Members) {
                    if (Users.FirstOrDefault(u => u.Id == member) == null) {
                        return null;
                    }
                }
            }

            // If GradesPerCategory != null, iterate the id's and if any id is missing return null
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<int, Dictionary<int, int>> cat in GradesPerCategory) {
                    if (Categories.FirstOrDefault(c => c.Id == cat.Key) == null) {
                        return null;
                    }
                    foreach (KeyValuePair<int, int> grade in cat.Value) {
                        if (Users.FirstOrDefault(u => u.Id == grade.Key) == null) {
                            return null;
                        }
                    }
                }
            }

            // Apply
            Groups.Add(newGroup);
            this.SaveChanges();

            // Add the Grade entries
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<int, Dictionary<int, int>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<int, int> grade in cat.Value) {
                        Grades.Add(new Grade { Value = grade.Value, UserId = grade.Key, GroupId = newGroup.Id, CategoryId = cat.Key });
                    }
                }
            }

            this.SaveChanges();
            return newGroup.Id;
        }
        // Dictionary is {<cat.Id>:{<user.Id>:<grade>}} where grade is clamped between option:num_grade_min_value and option:num_grade_max_value
        public bool ModifyGroup(int id, string? name, string? gameName, string? gameUrl, string? gameBannerUrl, int? editionId, List<int>? Members, Dictionary<int, Dictionary<int, int>>? GradesPerCategory) {
            int num_grade_min_value = int.Parse(this.GetOption("num_grade_min_value") ?? "");
            int num_grade_max_value = int.Parse(this.GetOption("num_grade_max_value") ?? "");

            // If GradesPerCategory != null validate the grade numbers between the min/max and return false if not valid
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<int, Dictionary<int, int>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<int, int> grade in cat.Value) {
                        if (grade.Value < num_grade_min_value) {
                            return false; // Return false if grade is below min
                        } else if (grade.Value > num_grade_max_value) {
                            return false; // Return false if grade is above max
                        }
                    }
                }
            }

            // Get the group and update any fields that are not null
            Group? group = Groups.FirstOrDefault(g => g.Id == id);
            if (group != null) {
                if (name != null) {
                    group.Name = name;
                }
                if (gameName != null) {
                    group.GameName = gameName;
                }
                if (gameUrl != null) {
                    group.GameUrl = gameUrl;
                }
                if (gameBannerUrl != null) {
                    group.GameBannerUrl = gameBannerUrl;
                }
                if (editionId != null) {
                    group.EditionId = (int)editionId;
                }
                Groups.Update(group);
                this.SaveChanges();
            } else {
                return false;
            }

            // If members is not null and not length 0, iterate the id's and if any id is missing return false
            if (Members != null && Members.Count > 0) {
                foreach (int member in Members) {
                    if (Users.FirstOrDefault(u => u.Id == member) == null) {
                        return false;
                    }
                }
            }

            // If GradesPerCategory != null, iterate the id's and if any id is missing return false
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<int, Dictionary<int, int>> cat in GradesPerCategory) {
                    if (Categories.FirstOrDefault(c => c.Id == cat.Key) == null) {
                        return false;
                    }
                    foreach (KeyValuePair<int, int> grade in cat.Value) {
                        if (Users.FirstOrDefault(u => u.Id == grade.Key) == null) {
                            return false;
                        }
                    }
                }
            }

            // Remove al the Grade entries for this group then re-add them
            List<Grade> grades = Grades.Where(g => g.GroupId == id).ToList();
            foreach (Grade g in grades) {
                Grades.Remove(g);
            }
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<int, Dictionary<int, int>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<int, int> grade in cat.Value) {
                        Grades.Add(new Grade { Value = grade.Value, UserId = grade.Key, GroupId = id, CategoryId = cat.Key });
                    }
                }
            }

            return true;
        }
        // Dictionary is {"<cat.Name>":{"<user.Name>":<grade>}} where grade is clamped between option:num_grade_min_value and option:num_grade_max_value
        public int? AssembleGroup(string name, string gameName, string gameUrl, string gameBannerUrl, int editionId, List<string> Members, Dictionary<string, Dictionary<string, int>>? GradesPerCategory) {
            int num_grade_min_value = int.Parse(this.GetOption("num_grade_min_value") ?? "");
            int num_grade_max_value = int.Parse(this.GetOption("num_grade_max_value") ?? "");

            // Validate the grade numbers between the min/max
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<string, Dictionary<string, int>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<string, int> grade in cat.Value) {
                        if (grade.Value < num_grade_min_value) {
                            return null; // Return null if grade is below min
                        } else if (grade.Value > num_grade_max_value) {
                            return null; // Return null if grade is above max
                        }
                    }
                }
            }

            // Create the group and get the id after creation
            Group newGroup = new Group { Name = name, GameName = gameName, GameUrl = gameUrl, GameBannerUrl = gameBannerUrl, EditionId = editionId };
            Groups.Add(newGroup);
            this.SaveChanges();

            // Create the members for the group from the list of member-names
            foreach (string member in Members) {
                GroupMembers.Add(new GroupMember { Name = member, GroupId = newGroup.Id });
            }

            // Create the grades for the group from the dictionary of grades
            // {"<cat.Name>":{"<user.Name>":<grade>}} so {string:{string:int}}
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<string, Dictionary<string, int>> cat in GradesPerCategory) {
                    Category? category = Categories.FirstOrDefault(c => c.Name == cat.Key);
                    if (category == null) {
                        category = new Category { Name = cat.Key };
                        Categories.Add(category);
                        this.SaveChanges();
                    }

                    foreach (KeyValuePair<string, int> grade in cat.Value) {
                        User? user = Users.FirstOrDefault(u => u.Name == grade.Key);
                        if (user == null) {
                            user = new User { Name = grade.Key, Email = "", Password = "", IsAdmin = false, IsProtected = false };
                            Users.Add(user);
                            this.SaveChanges();
                        }

                        Grades.Add(new Grade { Value = grade.Value, UserId = user.Id, GroupId = newGroup.Id, CategoryId = category.Id });
                    }
                }
            }

            // Save changes and return the id of the new group
            this.SaveChanges();
            return newGroup.Id;
        }
        // Dictionary is {"<cat.Name>":{"<user.Name>":<grade>}} where grade is clamped between option:num_grade_min_value and option:num_grade_max_value
        public bool AssembleModifyGroup(int id, string? name, string? gameName, string? gameUrl, string? gameBannerUrl, int? editionId, List<string>? Members, Dictionary<string, Dictionary<string, int>>? GradesPerCategory) {
            int num_grade_min_value = int.Parse(this.GetOption("num_grade_min_value") ?? "");
            int num_grade_max_value = int.Parse(this.GetOption("num_grade_max_value") ?? "");

            // If GradesPerCategory != null validate the grade numbers between the min/max and return false if not valid
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<string, Dictionary<string, int>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<string, int> grade in cat.Value) {
                        if (grade.Value < num_grade_min_value) {
                            return false; // Return false if grade is below min
                        } else if (grade.Value > num_grade_max_value) {
                            return false; // Return false if grade is above max
                        }
                    }
                }
            }

            // Get the group and update any fields that are not null
            Group? group = Groups.FirstOrDefault(g => g.Id == id);
            if (group != null) {
                if (name != null) {
                    group.Name = name;
                }
                if (gameName != null) {
                    group.GameName = gameName;
                }
                if (gameUrl != null) {
                    group.GameUrl = gameUrl;
                }
                if (gameBannerUrl != null) {
                    group.GameBannerUrl = gameBannerUrl;
                }
                if (editionId != null) {
                    group.EditionId = (int)editionId;
                }
                Groups.Update(group);
                this.SaveChanges();
            } else {
                return false;
            }

            // If members is more then 0 and not null, remove all members for the group and add the new members
            if (Members != null && Members.Count > 0) {
                List<GroupMember> groupMembers = GroupMembers.Where(gm => gm.GroupId == id).ToList();
                foreach (GroupMember gm in groupMembers) {
                    GroupMembers.Remove(gm);
                }
                foreach (string member in Members) {
                    GroupMembers.Add(new GroupMember { Name = member, GroupId = id });
                }
            }

            // If GradesPerCategory != null, remove all grades for the group and add the new grades
            if (GradesPerCategory != null) {
                List<Grade> grades = Grades.Where(g => g.GroupId == id).ToList();
                foreach (Grade g in grades) {
                    Grades.Remove(g);
                }
                foreach (KeyValuePair<string, Dictionary<string, int>> cat in GradesPerCategory) {
                    Category? category = Categories.FirstOrDefault(c => c.Name == cat.Key);
                    if (category != null) {
                        foreach (KeyValuePair<string, int> grade in cat.Value) {
                            User? user = Users.FirstOrDefault(u => u.Name == grade.Key);
                            if (user != null) {
                                Grades.Add(new Grade { Value = grade.Value, UserId = user.Id, GroupId = id, CategoryId = category.Id });
                            }
                        }
                    }
                }
            }

            return false;
        }
        public bool RemoveGroup(int id) {
            Group? group = Groups.FirstOrDefault(g => g.Id == id);
            if (group != null) {
                Groups.Remove(group);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public User? GetUser(int id) {
            return Users.FirstOrDefault(u => u.Id == id);
        }
        public List<User> GetUsers() {
            return Users.ToList();
        }
        public int? AddUser(string name, string email, string password, bool? isAdmin) {
            User newUser = new User { Name = name, Email = email, Password = password, IsAdmin = isAdmin ?? false, IsProtected = false };
            Users.Add(newUser);
            this.SaveChanges();
            return newUser.Id;
        }
        public bool ModifyUser(int id, string? name, string? email, string? password, bool? isAdmin) {
            User? user = Users.FirstOrDefault(u => u.Id == id);
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
                if (isAdmin != null) {
                    user.IsAdmin = (bool)isAdmin;
                }
                Users.Update(user);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveUser(int id) {
            User? user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null) {
                Users.Remove(user);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public bool GetUserIsAdmin(int id) {
            User? user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null) {
                return user.IsAdmin;
            }
            return false;
        }
        public bool GetUserIsProtected(int id) {
            User? user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null) {
                return user.IsProtected;
            }
            return false;
        }

        public int? ValidateUserLogin(string Email, string Password) {
            User? user = Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
            if (user != null) {
                return user.Id;
            }
            return null;
        }

        public bool GroupHasGradeForCategory(int groupId, int categoryId) {
            // Get the groupId to ensure it exists, return false if it does not
            Group? group = Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null) {
                return false;
            }
            
            // Get the categoryId to ensure it exists, return false if it does not
            Category? category = Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) {
                return false;
            }

            // Check if there is any grade entry that matches the groupId and categoryId
            return Grades.Any(g => g.GroupId == groupId && g.CategoryId == categoryId);
        }

        public Category? GetCategory(int id) {
            return Categories.FirstOrDefault(c => c.Id == id);
        }
        public List<Category> GetCategories() {
            return Categories.ToList();
        }

        public int? AddCategory(string name) {
            Category? category = Categories.FirstOrDefault(c => c.Name == name);
            if (category != null) {
                return category.Id;
            }
            Category newCategory = new Category { Name = name };
            Categories.Add(newCategory);
            this.SaveChanges();
            return newCategory.Id;
        }
        public bool ModifyCategory(int id, string? name) {
            Category? category = Categories.FirstOrDefault(c => c.Id == id);
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
            Category? category = Categories.FirstOrDefault(c => c.Id == id);
            if (category != null) {
                Categories.Remove(category);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public GroupMember? GetGroupMember(int id) {
            return GroupMembers.FirstOrDefault(gm => gm.Id == id);
        }
        public List<GroupMember> GetGroupMembers() {
            return GroupMembers.ToList();
        }
        public int? AddMember(string name, int groupId) {
            GroupMember newMember = new GroupMember { Name = name, GroupId = groupId };
            GroupMembers.Add(newMember);
            this.SaveChanges();
            return newMember.Id;
        }
        public bool ModifyMember(int id, string? name) {
            GroupMember? member = GroupMembers.FirstOrDefault(gm => gm.Id == id);
            if (member != null) {
                if (name != null) {
                    member.Name = name;
                }
                GroupMembers.Update(member);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveMember(int id) {
            GroupMember? member = GroupMembers.FirstOrDefault(gm => gm.Id == id);
            if (member != null) {
                GroupMembers.Remove(member);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public bool SetGradeForGroupInCategory(int groupId, int categoryId, int grade) {
            // Get the groupId to ensure it exists, return false if it does not
            Group? group = Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null) {
                return false;
            }

            // Get the categoryId to ensure it exists, return false if it does not
            Category? category = Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) {
                return false;
            }

            // Get the grade entry if it exists, otherwise create a new one
            Grade? gradeEntry = Grades.FirstOrDefault(g => g.GroupId == groupId && g.CategoryId == categoryId);
            if (gradeEntry != null) {
                gradeEntry.Value = grade;
                Grades.Update(gradeEntry);
            } else {
                Grades.Add(new Grade { Value = grade, UserId = 0, GroupId = groupId, CategoryId = categoryId });
            }

            this.SaveChanges();

            return true;
        }
        public bool RemoveGradeForGroupInCategory(int groupId, int categoryId) {
            // Get the groupId to ensure it exists, return false if it does not
            Group? group = Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null) {
                return false;
            }

            // Get the categoryId to ensure it exists, return false if it does not
            Category? category = Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) {
                return false;
            }

            // Remove any matching entry
            Grade? gradeEntry = Grades.FirstOrDefault(g => g.GroupId == groupId && g.CategoryId == categoryId);
            if (gradeEntry != null) {
                Grades.Remove(gradeEntry);
                return true;
            }

            this.SaveChanges();

            return false;
        }

        public bool ModifyGrade(int gradeId, int? userId, int? groupId, int? categoryId, int? value) {
            Grade? grade = Grades.FirstOrDefault(g => g.Id == gradeId);
            if (grade != null) {
                if (userId != null) {
                    grade.UserId = (int)userId;
                }
                if (groupId != null) {
                    grade.GroupId = (int)groupId;
                }
                if (categoryId != null) {
                    grade.CategoryId = (int)categoryId;
                }
                if (value != null) {
                    grade.Value = (int)value;
                }
                Grades.Update(grade);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion Method
    }
    */
}
