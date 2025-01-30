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
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
        public int EditionId { get; set; }
    }
    internal class GroupMember {
        public int ID { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
    }
    internal class Grade {
        public int ID { get; set; }
        public string Value { get; set; }
        public int GroupId { get; set; }
        public int UserCategoryId { get; set; }
    }

    // Class for a grade that has had it's relations resolved, and thus include the user and category it belongs to. The group is not resolved since this object is returned inside ResolvedGroup.
    internal class ResolvedGrade {
        public int ID { get; set; }
        public string Value { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; }
        public Category Category { get; set; }
    }

    // Class for a group that has had it's relations resolved, and thus include the grades, members and edition it belongs to.
    internal class ResolvedGroup {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
        public Edition Edition { get; set; }
        public List<GroupMember> GroupMembers { get; set; }
        public List<ResolvedGrade> Grades { get; set; }
    }

    internal class AppDbContext : DbContext {
        private string format_version = "3";
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
            if (!this.HasOption("allowed_category_types")) {
                this.SetAllowedCategoryTypes(new List<string> {
                    "int_0-6",
                    "int_1-6",
                    "string",
                });
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

        public List<string> GetAllowedCategoryTypes() {
            if (this.HasOption("allowed_category_types")) {
                string? allowed_category_types = this.GetOption("allowed_category_types");
                if (allowed_category_types != null) {
                    return allowed_category_types.Split(',').ToList();
                }
            }
            return new List<string>();
        }
        public void SetAllowedCategoryTypes(List<string> allowedCategoryTypes) {
            this.SetOption("allowed_category_types", string.Join(",", allowedCategoryTypes));
        }
        public bool IsAllowedCategoryType(string categoryType) {
            List<string> allowedCategoryTypes = this.GetAllowedCategoryTypes();
            return allowedCategoryTypes.Contains(categoryType);
        }
        public (bool,string?) ValidateCategoryValueBasedOnType(string value, string categoryType) {
            List<string> allowedCategoryTypes = this.GetAllowedCategoryTypes();
            if (!allowedCategoryTypes.Contains(categoryType)) {
                return (false,"InvalidCat");
            }

            // Validate based on type
            if (categoryType == "int_0-6") {
                if (int.TryParse(value, out int intValue)) {
                    if (intValue < 0) {
                        return (false, $"int_0-6_Lågt");
                    } else if (intValue > 6) {
                        return (false, "int_0-6_Högt");
                    }
                }
            } else if (categoryType == "int_1-6") {
                if (int.TryParse(value, out int intValue)) {
                    if (intValue < 1) {
                        return (false, "int_1-6_Lågt");
                    } else if (intValue > 6) {
                        return (false, "int_1-6_Högt");
                    }
                }
            }

            return (true,null);
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
            this.SaveChanges();

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
                // Check protect
                if (user.IsProtected) {
                    throw new AttempedRemoveOnProtected("Attempted remove on protected user!");
                }

                Users.Remove(user);

                // Remove all focus categories
                List<UserCategory> userCategories = UserCategories.Where(uc => uc.UserID == id).ToList();

                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool GetUserIsAdmin(int id) {
            User? user = Users.FirstOrDefault(u => u.ID == id);
            if (user != null) {
                return user.IsAdmin;
            }
            return false;
        }
        public bool GetUserIsProtected(int id) {
            User? user = Users.FirstOrDefault(u => u.ID == id);
            if (user != null) {
                return user.IsProtected;
            }
            return false;
        }

        public int? ValidateUserLogin(string Email, string Password) {
            User? user = Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
            if (user != null) {
                return user.ID;
            }
            return null;
        }

        // Categories
        public Category? GetCategory(int id) {
            return Categories.FirstOrDefault(c => c.ID == id);
        }
        public List<Category> GetCategories() {
            return Categories.ToList();
        }
        public int? AddCategory(string name, string? categoryType = "int_0-6") {

            // Validate category type
            if (this.IsAllowedCategoryType(categoryType) == false) {
                return null;
            }

            // Return id after creation, if exists just return id of existing entry
            Category? category = Categories.FirstOrDefault(c => c.Name == name && c.CategoryType == categoryType);
            if (category != null) {
                return category.ID;
            }
            Category newCategory = new Category { Name = name, CategoryType = categoryType };
            Categories.Add(newCategory);
            this.SaveChanges();
            return newCategory.ID;
        }
        public bool ModifyCategory(int id, string? name = null, string? categoryType = null) {
            Category? category = Categories.FirstOrDefault(c => c.ID == id);
            if (category != null) {
                if (name != null) {
                    category.Name = name;
                }
                if (categoryType != null) {
                    if (this.IsAllowedCategoryType(categoryType) == false) {
                        return false;
                    }
                    category.CategoryType = categoryType;
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

        // UserCategories
        public UserCategory? GetUserCategory(int id) {
            return UserCategories.FirstOrDefault(uc => uc.ID == id);
        }
        public List<UserCategory> GetUserCategories() {
            return UserCategories.ToList();
        }
        public (User?,Category?) GetResolvedUserCategory(int id) {
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.ID == id);
            if (userCategory != null) {
                User? user = Users.FirstOrDefault(u => u.ID == userCategory.UserID);
                Category? category = Categories.FirstOrDefault(c => c.ID == userCategory.CategoryID);
                if (user != null && category != null) {
                    return (user, category);
                }
            }
            return (null, null);
        }
        public List<Category> GetCategoriesForUser(int userId) {
            List<UserCategory> userCategories = UserCategories.Where(uc => uc.UserID == userId).ToList();
            List<Category> categories = new List<Category>();
            // Resolve Category instances for al the category ids
            foreach (UserCategory uc in userCategories) {
                Category? category = Categories.FirstOrDefault(c => c.ID == uc.CategoryID);
                if (category != null) {
                    categories.Add(category);
                }
            }
            return categories;
        }
        public bool AddCategoryForUser(int userId, int categoryId) {
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
            if (userCategory == null) {
                UserCategories.Add(new UserCategory { UserID = userId, CategoryID = categoryId });
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveCategoryForUser(int userId, int categoryId) {
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
            if (userCategory != null) {
                UserCategories.Remove(userCategory);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool IsCategoryForUser(int userId, int categoryId) {
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
            if (userCategory != null) {
                return true;
            }
            return false;
        }
        public int? AddUserCategory(int userId, int categoryId) {
            // Return id after creation, if exists just return id of existing entry
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
            if (userCategory != null) {
                return userCategory.ID;
            }
            UserCategory newUserCategory = new UserCategory { UserID = userId, CategoryID = categoryId };
            UserCategories.Add(newUserCategory);
            this.SaveChanges();
            return newUserCategory.ID;
        }
        public bool ModifyUserCategory(int id, int? userId = null, int? categoryId = null) {
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.ID == id);
            if (userCategory != null) {
                if (userId != null) {
                    userCategory.UserID = (int)userId;
                }
                if (categoryId != null) {
                    userCategory.CategoryID = (int)categoryId;
                }
                UserCategories.Update(userCategory);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveUserCategory(int id) {
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.ID == id);
            if (userCategory != null) {
                UserCategories.Remove(userCategory);
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
        public int? AddGroup(string name, string gameName, string GameUrl, int editionId, string? gameBannerUrl = null) {
            Group? newGroup = null;
            // Create the group and get he id after creation
            if (!string.IsNullOrEmpty(gameBannerUrl)) {
                newGroup = new Group() { Name = name, GameName = gameName, GameUrl = GameUrl, EditionId = editionId, GameBannerUrl = gameBannerUrl};
            } else {
                newGroup = new Group() { Name = name, GameName = gameName, GameUrl = GameUrl, EditionId = editionId };
            }

            Groups.Add(newGroup);
            this.SaveChanges();
            return newGroup.ID;
        }
        public bool ModifyGroup(int id, string? name = null, string? gameName = null, string? GameUrl = null, int? editionId = null, string? gameBannerUrl = null) {
            Group? group = Groups.FirstOrDefault(g => g.ID == id);
            if (group != null) {
                if (name != null) {
                    group.Name = name;
                }
                if (gameName != null) {
                    group.GameName = gameName;
                }
                if (GameUrl != null) {
                    group.GameUrl = GameUrl;
                }
                if (editionId != null) {
                    group.EditionId = (int)editionId;
                }
                if (gameBannerUrl != null) {
                    group.GameBannerUrl = gameBannerUrl;
                }
                Groups.Update(group);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        // Dictionary is {"<cat.Name>":{"<user.Name>":<grade>}} where grade is validated with ValidateCategoryValueBasedOnType
        public int? AssembleGroup(string name, string gameName, string GameUrl, string gameBannerUrl, int editionId, List<string> Members, Dictionary<string, Dictionary<string, string>>? GradesPerCategory) {

            // Validate the grade numbers between the min/max
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<string, Dictionary<string, string>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<string, string> grade in cat.Value) {
                        string? category_type = "int_0-6";
                        // Check for ":" in category_name, if exists split to category_type,category_name
                        if (cat.Key.Contains(":")) {
                            string[] parts = cat.Key.Split(':');
                            category_type = parts[0];
                        }
                        var (result, reason) = this.ValidateCategoryValueBasedOnType($"{grade.Value}", category_type);
                        if (result == false) {
                            return null;
                        }
                    }
                }
            }

            // Create the group and get the id after creation
            Group newGroup = new Group() { Name = name, GameName = gameName, GameUrl = GameUrl, EditionId = editionId, GameBannerUrl = gameBannerUrl };
            Groups.Add(newGroup);
            this.SaveChanges();

            // Ensure al members
            foreach (string member in Members) {
                GroupMember? groupMember = GroupMembers.FirstOrDefault(u => u.Name == member);
                if (groupMember == null) {
                    GroupMembers.Add(new GroupMember { Name = member, GroupId = newGroup.ID });
                }
            }

            // Create the grade entries
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<string, Dictionary<string, string>> cat in GradesPerCategory) {
                    int categoryId;
                    // Ensure category entries (al keys in the dictionary)
                    Category? category = Categories.FirstOrDefault(c => c.Name == cat.Key);
                    if (category == null) {
                        // Create new category with name
                        string category_name = cat.Key;
                        string? category_type = null;
                        // Check for ":" in category_name, if exists split to category_type,category_name
                        if (category_name.Contains(":")) {
                            string[] parts = category_name.Split(':');
                            category_type = parts[0];
                            category_name = parts[1];
                        }
                        categoryId = AddCategory(category_name, category_type) ?? -1;
                    } else {
                        categoryId = category.ID;
                    }

                    foreach (KeyValuePair<string, string> grade in cat.Value) {
                        int? userId;
                        // Ensure al user entries (al keys of the values in the dictionary)
                        User? user = Users.FirstOrDefault(u => u.Name == grade.Key);
                        if (user == null) {
                            List<int> user_focusCategories = new List<int>() { categoryId };
                            string user_name = grade.Key;
                            string user_email = user_name;
                            string user_password = user_name;
                            // Incase 3st ":" in username split to user_name,user_email,user_password,focusCategories
                            // Incase 2st ":" in username split to user_name,user_email,user_password
                            // Incase 1st ":" in username split to user_name,user_email
                            // focusCategories is a string with comma split ids to be appended to the user_focusCategories list.
                            int colon_count = user_name.Count(c => c == ':');
                            if (colon_count == 3) {
                                string[] parts = user_name.Split(':');
                                user_name = parts[0];
                                user_email = parts[1];
                                user_password = parts[2];
                                user_focusCategories.AddRange(parts[3].Split(',').Select(int.Parse));

                            } else if (colon_count == 2) {
                                string[] parts = user_name.Split(':');
                                user_name = parts[0];
                                user_email = parts[1];
                                user_password = parts[2];

                            } else if (colon_count == 1) {
                                string[] parts = user_name.Split(':');
                                user_name = parts[0];
                                user_email = parts[1];
                            }

                            // Create new user with name
                            userId = AddUser(user_name, user_email, user_password, user_focusCategories);
                        } else {
                            userId = user.ID;
                        }
                        // Ensure al UserCategories link-entries
                        int? userCategoryId;
                        UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
                        if (userCategory == null) {
                            userCategoryId = AddUserCategory(userId ?? -1, categoryId);
                        } else {
                            userCategoryId = userCategory.ID;
                        }
                        // Ensure al grade entries
                        Grade? gradeEntry = Grades.FirstOrDefault(g => g.GroupId == newGroup.ID && g.UserCategoryId == userCategoryId);
                        if (gradeEntry == null) {
                            Grades.Add(new Grade { Value = grade.Value, GroupId = newGroup.ID, UserCategoryId = userCategoryId ?? -1 });
                        } else {
                            gradeEntry.Value = grade.Value;
                            Grades.Update(gradeEntry);
                        }
                    }
                }
            }

            // Apply
            this.SaveChanges();
            return newGroup.ID;
        }
        public bool AssembleModifyGroup(string? name = null, string? gameName = null, string? GameUrl = null, string? gameBannerUrl = null, int? editionId = null, List<string>? Members = null, Dictionary<string, Dictionary<string, string>>? GradesPerCategory = null) {

            // Validate the grade numbers between the min/max
            if (GradesPerCategory != null) {
                foreach (KeyValuePair<string, Dictionary<string, string>> cat in GradesPerCategory) {
                    foreach (KeyValuePair<string, string> grade in cat.Value) {
                        string? category_type = "int_0-6";
                        // Check for ":" in category_name, if exists split to category_type,category_name
                        if (cat.Key.Contains(":")) {
                            string[] parts = cat.Key.Split(':');
                            category_type = parts[0];
                        }
                        var (result, reason) = this.ValidateCategoryValueBasedOnType($"{grade.Value}", category_type);
                        if (result == false) {
                            return false;
                        }
                    }
                }
            }

            // Get the group and update any fields that are not null
            Group? group = Groups.FirstOrDefault(g => g.Name == name);
            if (group != null) {
                if (name != null) {
                    group.Name = name;
                }
                if (gameName != null) {
                    group.GameName = gameName;
                }
                if (GameUrl != null) {
                    group.GameUrl = GameUrl;
                }
                if (editionId != null) {
                    group.EditionId = (int)editionId;
                }
                if (gameBannerUrl != null) {
                    group.GameBannerUrl = gameBannerUrl;
                }
                Groups.Update(group);

                // If members is more then 0 and not null, remove all members for the group and add the new members
                if (Members != null && Members.Count > 0) {
                    List<GroupMember> groupMembers = GroupMembers.Where(gm => gm.GroupId == group.ID).ToList();
                    foreach (GroupMember gm in groupMembers) {
                        GroupMembers.Remove(gm);
                    }
                    foreach (string member in Members) {
                        GroupMember? groupMember = GroupMembers.FirstOrDefault(u => u.Name == member);
                        if (groupMember == null) {
                            GroupMembers.Add(new GroupMember { Name = member, GroupId = group.ID });
                        }
                    }
                }

                // If GradesPerCategory != null, remove all grades for the group and add the new grades
                if (GradesPerCategory != null) {
                    List<Grade> grades = Grades.Where(g => g.GroupId == group.ID).ToList();
                    foreach (Grade g in grades) {
                        Grades.Remove(g);
                    }

                    // Iterate the dictionary and ensure
                    foreach (KeyValuePair<string, Dictionary<string, string>> cat in GradesPerCategory) {
                        int categoryId;
                        // Ensure category entries (al keys in the dictionary)
                        Category? category = Categories.FirstOrDefault(c => c.Name == cat.Key);
                        if (category == null) {
                            // Create new category with name
                            string category_name = cat.Key;
                            string? category_type = null;
                            // Check for ":" in category_name, if exists split to category_type,category_name
                            if (category_name.Contains(":")) {
                                string[] parts = category_name.Split(':');
                                category_type = parts[0];
                                category_name = parts[1];
                            }
                            categoryId = AddCategory(category_name, category_type) ?? -1;
                        } else {
                            categoryId = category.ID;
                        }

                        foreach (KeyValuePair<string, string> grade in cat.Value) {
                            int? userId;
                            // Ensure al user entries (al keys of the values in the dictionary)
                            User? user = Users.FirstOrDefault(u => u.Name == grade.Key);
                            if (user == null) {
                                List<int> user_focusCategories = new List<int>() { categoryId };
                                string user_name = grade.Key;
                                string user_email = user_name;
                                string user_password = user_name;
                                // Incase 3st ":" in username split to user_name,user_email,user_password,focusCategories
                                // Incase 2st ":" in username split to user_name,user_email,user_password
                                // Incase 1st ":" in username split to user_name,user_email
                                // focusCategories is a string with comma split ids to be appended to the user_focusCategories list.
                                int colon_count = user_name.Count(c => c == ':');
                                if (colon_count == 3) {
                                    string[] parts = user_name.Split(':');
                                    user_name = parts[0];
                                    user_email = parts[1];
                                    user_password = parts[2];
                                    user_focusCategories.AddRange(parts[3].Split(',').Select(int.Parse));

                                } else if (colon_count == 2) {
                                    string[] parts = user_name.Split(':');
                                    user_name = parts[0];
                                    user_email = parts[1];
                                    user_password = parts[2];

                                } else if (colon_count == 1) {
                                    string[] parts = user_name.Split(':');
                                    user_name = parts[0];
                                    user_email = parts[1];
                                }

                                // Create new user with name
                                userId = AddUser(user_name, user_email, user_password, user_focusCategories);
                            } else {
                                userId = user.ID;
                            }
                            // Ensure al UserCategories link-entries
                            int? userCategoryId;
                            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
                            if (userCategory == null) {
                                userCategoryId = AddUserCategory(userId ?? -1, categoryId);
                            } else {
                                userCategoryId = userCategory.ID;
                            }
                            // Ensure al grade entries
                            Grade? gradeEntry = Grades.FirstOrDefault(g => g.GroupId == group.ID && g.UserCategoryId == userCategoryId);
                            if (gradeEntry == null) {
                                Grades.Add(new Grade { Value = grade.Value, GroupId = group.ID, UserCategoryId = userCategoryId ?? -1 });
                            } else {
                                gradeEntry.Value = grade.Value;
                                Grades.Update(gradeEntry);
                            }
                        }
                    }
                }
            }

            // Apply
            this.SaveChanges();
            return true;
        }
        public bool RemoveGroup(int id)
        {
            Group? group = Groups.FirstOrDefault(c => c.ID == id);
            if (group != null)
            {
                Groups.Remove(group);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public ResolvedGroup? GetResolvedGroup(int id) {
            Group? group = Groups.FirstOrDefault(g => g.ID == id);
            if (group != null) {
                List<GroupMember> groupMembers = GroupMembers.Where(gm => gm.GroupId == group.ID).ToList();
                
                List<ResolvedGrade> resolvedGrades = new List<ResolvedGrade>();
                List<Grade> grades = Grades.Where(g => g.GroupId == group.ID).ToList();
                foreach (Grade grade in grades) {
                    // Call GetResolvedGrade
                    ResolvedGrade? resolvedGrade = GetResolvedGrade(grade.ID);
                    if (resolvedGrade != null) {
                        resolvedGrades.Add(resolvedGrade);
                    }
                }

                Edition? edition = Editions.FirstOrDefault(e => e.ID == group.EditionId);
                if (edition != null) {
                    return new ResolvedGroup { ID = group.ID, Name = group.Name, GameName = group.GameName, GameUrl = group.GameUrl, GameBannerUrl = group.GameBannerUrl, Edition = edition, GroupMembers = groupMembers, Grades = resolvedGrades };
                }
            }
            return null;
        }

        public List<ResolvedGroup> GetResolvedGroups() {
            List<ResolvedGroup> resolvedGroups = new List<ResolvedGroup>();
            List<Group> groups = Groups.ToList();
            foreach (Group group in groups) {
                ResolvedGroup? resolvedGroup = GetResolvedGroup(group.ID);
                if (resolvedGroup != null) {
                    resolvedGroups.Add(resolvedGroup);
                }
            }
            return resolvedGroups;
        }

        public List<ResolvedGroup> GetResolvedGroupsForEdition(int editionId) {
            List<ResolvedGroup> resolvedGroups = new List<ResolvedGroup>();
            List<Group> groups = Groups.Where(g => g.EditionId == editionId).ToList();
            foreach (Group group in groups) {
                ResolvedGroup? resolvedGroup = GetResolvedGroup(group.ID);
                if (resolvedGroup != null) {
                    resolvedGroups.Add(resolvedGroup);
                }
            }
            return resolvedGroups;
        }

        // Grade
        public Grade? GetGrade(int id) {
            return Grades.FirstOrDefault(g => g.ID == id);
        }
        public List<Grade> GetGrades() {
            return Grades.ToList();
        }
        public int? AddGrade(string value, int groupId, int userCategoryId) {
            // Return id after creation, if exists just return id of existing entry
            Grade? grade = Grades.FirstOrDefault(g => g.Value == value && g.GroupId == groupId && g.UserCategoryId == userCategoryId);
            if (grade != null) {
                return grade.ID;
            }
            Grade newGrade = new Grade { Value = value, GroupId = groupId, UserCategoryId = userCategoryId };
            Grades.Add(newGrade);
            this.SaveChanges();
            return newGrade.ID;
        }
        public int? AddGradeResolving(string value, int groupId, int userId, int categoryId) {
            // Check if userCategory instance exists, if not create one
            int? userCategoryId;
            UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.UserID == userId && uc.CategoryID == categoryId);
            if (userCategory == null) {
                userCategory = new UserCategory { UserID = userId, CategoryID = categoryId };
                UserCategories.Add(userCategory);
                this.SaveChanges();
                userCategoryId = userCategory.ID;
            } else {
                userCategoryId = userCategory.ID;
            }

            if (userCategoryId == null) {
                return null;
            }
            return AddGrade(value, groupId, (int)userCategoryId);
        }
        public bool ModifyGrade(int id, string? value = null, int? groupId = null, int? userCategoryId = null) {
            Grade? grade = Grades.FirstOrDefault(g => g.ID == id);
            if (grade != null) {
                if (value != null) {
                    grade.Value = (string)value;
                }
                if (groupId != null) {
                    grade.GroupId = (int)groupId;
                }
                if (userCategoryId != null) {
                    grade.UserCategoryId = (int)userCategoryId;
                }
                Grades.Update(grade);
                this.SaveChanges();
                return true;
            }
            return false;
        }
        public bool ModifyGradeResolving(int id, string? value = null, int? groupId = null, int? userId = null, int? categoryId = null) {
            Grade? grade = Grades.FirstOrDefault(g => g.ID == id);
            if (grade != null) {
                // Modify userCategory?
                bool success = ModifyUserCategory(grade.UserCategoryId, userId, categoryId);
                if (success == false) {
                    return false;
                }

                // Modify grade
                return ModifyGrade(id, value, groupId);
            }
            return false;
        }
        public ResolvedGrade? GetResolvedGrade(int id) {
            Grade? grade = Grades.FirstOrDefault(g => g.ID == id);
            if (grade != null) {
                UserCategory? userCategory = UserCategories.FirstOrDefault(uc => uc.ID == grade.UserCategoryId);
                if (userCategory != null) {
                    User? user = Users.FirstOrDefault(u => u.ID == userCategory.UserID);
                    Category? category = Categories.FirstOrDefault(c => c.ID == userCategory.CategoryID);
                    if (user != null && category != null) {
                        return new ResolvedGrade { ID = grade.ID, Value = grade.Value, User = user, GroupId = grade.GroupId, Category = category };
                    }
                }
            }
            return null;
        }
        #endregion Methods
    }
}


