using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace GameOnSystem {
    internal class AppDbContext : DbContext {
        private readonly bool _useSqlite;
        private readonly string _connectionString;

        private string format_version = "4";

        public DbSet<DbTableModel_Option> Options { get; set; }
        public DbSet<DbTableModel_Edition> Editions { get; set; }
        public DbSet<DbTableModel_AppUser> AppUsers { get; set; }
        public DbSet<DbTableModel_Category> Categories { get; set; }
        public DbSet<DbTableModel_UserCat> UserCats { get; set; }
        public DbSet<DbTableModel_Group> Groups { get; set; }
        public DbSet<DbTableModel_Participant> Participants { get; set; }
        public DbSet<DbTableModel_GroupParticipant> GroupParticipants { get; set; }
        public DbSet<DbTableModel_Grade> Grades { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (_useSqlite) {
                optionsBuilder.UseSqlite(_connectionString);
            } else {
                if (_connectionString == null) {
                    throw new ArgumentNullException(nameof(_connectionString), "MySQL connection string is required when not using a local SQLite database.");
                }
                optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            }
        }

        /// <summary>
        /// Inits the dbContext with a SQLite database file or a MySQL server.
        /// SQLite: new AppDbContext(true, "Data Source=gameon_v2.db");
        /// MySQL: new AppDbContext(false, "server=localhost;user=root;password=;database=gameon_v2");
        /// </summary>
        public AppDbContext(bool useSqlite = true, string connectionString = "") {
            _useSqlite = useSqlite;
            _connectionString = connectionString;

            Database.EnsureCreated();

            // Ensure database version
            if (!this.HasOption("version")) {
                this.SetOption("version", format_version);
            } else {
                string? version = this.GetOption("version");
                if (version != format_version) {
                    if (_useSqlite || version == null) {
                        throw new Exception($"Database version mismatch! Found {version} expected {format_version}");
                    } else {
                        if (int.Parse(version) > int.Parse(format_version)) {
                            throw new Exception($"Database version mismatch, found {version} expected {format_version}! (Consider updating your app)");
                        } else {
                            throw new Exception($"Database version mismatch, found {version} expected {format_version}! (Consider updating your server)");
                        }
                    }
                }
            }

            // Ensure admin user
            // Match Users with email "admin" and IsProtected true
            if (AppUsers != null) {
                var admin_user = AppUsers.FirstOrDefault(u => u.Email == "admin" && u.IsProtected == true);
                if (admin_user == null) {
                    AppUsers.Add(new DbTableModel_AppUser { Name = "Admin", Email = "admin", Password = "admin", IsAdmin = true, IsProtected = true });
                    SaveChanges();
                }
            }

            // Add created
            if (!this.HasOption("created")) {
                this.SetOption("created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            // Add creation metadata
            if (!this.HasOption("created.DotNet")) {
                this.SetOption("created.DotNet", Environment.Version.ToString());
            }
            if (!this.HasOption("created.Microsoft_EntityFrameworkCore")) {
                this.SetOption("created.Microsoft_EntityFrameworkCore", GetAssemblyVersion("Microsoft.EntityFrameworkCore"));
            }
            if (!this.HasOption("created.Pomelo_EntityFrameworkCore_MySql")) {
                this.SetOption("created.Pomelo_EntityFrameworkCore_MySql", GetAssemblyVersion("Pomelo.EntityFrameworkCore.MySql"));
            }
        }

        static string GetAssemblyVersion(string assemblyName) {
            try {
                // Load the assembly by its name and get the version
                var assembly = Assembly.Load(new AssemblyName(assemblyName));
                return assembly.GetName().Version.ToString();
            }
            catch (Exception) {
                return $"{assemblyName} not found";
            }
        }

        public void EnsureDbTables() {
            Database.EnsureCreated();
        }

        public void ClearDbTables() {
            // Disable foreign key constraints
            Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");

            // Drop each table
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(Options)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(Editions)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(AppUsers)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(Categories)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(UserCats)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(Groups)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(Participants)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(GroupParticipants)}`");
            Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS `{nameof(Grades)}`");

            // Re-enable foreign key constraints
            Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
        }

        public void ClearDbEntries() {
            // Disable foreign key constraints
            Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");

            // Truncate each table
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(Options)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(Editions)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(AppUsers)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(Categories)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(UserCats)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(Groups)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(Participants)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(GroupParticipants)}`");
            Database.ExecuteSqlRaw($"TRUNCATE TABLE `{nameof(Grades)}`");

            // Re-enable foreign key constraints
            Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
        }

        #region Options

        public bool HasOption(string field) {
            return Options.Any(o => o.Field == field);
        }

        public string? GetOption(string field) {
            DbTableModel_Option? option = Options.FirstOrDefault(o => o.Field == field);
            return option?.Value;
        }

        public List<DbTableModel_Option> GetOptions() {
            return Options.ToList();
        }

        public void SetOption(string field, string value) {
            DbTableModel_Option? option = Options.FirstOrDefault(o => o.Field == field);
            if (option == null) {
                Options.Add(new DbTableModel_Option { Field = field, Value = value });
            } else {
                option.Value = value;
            }
            SaveChanges();
        }

        public bool RemoveOption(string field) {
            DbTableModel_Option? option = Options.FirstOrDefault(o => o.Field == field);
            if (option != null) {
                Options.Remove(option);
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool SetFlag(string field, bool value=true) {
            SetOption(field, value ? "true" : "false");
            return value;
        }

        public bool UnsetFlag(string field) {
            SetOption(field, "false");
            return false;
        }

        public bool GetFlag(string field) {
            return GetOption(field) == "true";
        }

        public object GetFlagWdef(string field, object? defaultValue=null) {
            string? value = GetOption(field);
            if (value == null) {
                return defaultValue ?? false;
            }
            return value == "true";
        }

        public bool IsInited() {
            return GetOption("inited") == "true";
        }

        public bool MarkAsInited() {
            SetOption("inited", "true");
            return true;
        }

        public bool MarkAsNotInited() {
            SetOption("inited", "false");
            return true;
        }
        #endregion

        #region Editions

        public DbTableModel_Edition? GetEdition(int ID) {
            return Editions.FirstOrDefault(e => e.ID == ID);
        }

        public List<DbTableModel_Edition> GetEditions() {
            return Editions.ToList();
        }

        public DbTableModel_Edition AddEdition(string name, string theme, int gradeMin, int gradeMax, int gradeType, bool isActive, DateTime? gradingDeadline) {
            DbTableModel_Edition edition = new DbTableModel_Edition { Name = name, Theme = theme, GradeMin = gradeMin, GradeMax = gradeMax, GradeType = gradeType, IsActive = isActive, GradingDeadline = gradingDeadline };
            Editions.Add(edition);
            SaveChanges();
            return edition;
        }

        public bool RemoveEdition(int editionID) {
            DbTableModel_Edition? edition = Editions.FirstOrDefault(e => e.ID == editionID);
            if (edition != null) {
                Editions.Remove(edition);
                SaveChanges();
                return true;
            }
            return false;
        }

        public DbTableModel_Edition? GetActiveEdition() {
            return Editions.FirstOrDefault(e => e.IsActive == true);
        }

        #endregion

        #region AppUsers
        public DbTableModel_AppUser? GetAppUser(int ID) {
            return AppUsers.FirstOrDefault(u => u.ID == ID);
        }
        public List<DbTableModel_AppUser> GetAppUsers() {
            return AppUsers.ToList();
        }
        public DbTableModel_AppUser AddAppUser(string name, string email, string password, bool isAdmin, bool isProtected = false) {
            DbTableModel_AppUser user = new DbTableModel_AppUser { Name = name, Email = email, Password = password, IsAdmin = isAdmin, IsProtected = isProtected };
            AppUsers.Add(user);
            SaveChanges();
            return user;
        }

        public bool RemoveAppUser(int userID) {
            DbTableModel_AppUser? user = AppUsers.FirstOrDefault(u => u.ID == userID);
            if (user != null) {
                // Check if user is protected
                if (user.IsProtected) {
                    Debug.Print("Warn: Attempted removal of protected user!");
                    return false;
                }
                // Remove UserCat entry
                user.CleanFocusCategories(this);
                // Remove user
                AppUsers.Remove(user);
                SaveChanges();
                return true;
            }
            return false;
        }

        public int? ValidateUserLogin(string email, string password) {
            DbTableModel_AppUser? user = AppUsers.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null) {
                return user.ID;
            }
            return null;
        }
        #endregion

        #region Categories
        public DbTableModel_Category? GetCategory(int ID) {
            return Categories.FirstOrDefault(c => c.ID == ID);
        }
        public List<DbTableModel_Category> GetCategories() {
            return Categories.ToList();
        }
        public DbTableModel_Category AddCategory(string name) {
            DbTableModel_Category category = new DbTableModel_Category { Name = name };
            Categories.Add(category);
            SaveChanges();
            return category;
        }
        public bool RemoveCategory(int categoryID) {
            DbTableModel_Category? category = Categories.FirstOrDefault(c => c.ID == categoryID);
            if (category != null) {
                // Remove UserCat entries
                category.CleanFocusUsers(this);
                // Remove the category
                Categories.Remove(category);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region UserCats
        public DbTableModel_UserCat? GetUserCat(int ID) {
            return UserCats.FirstOrDefault(uc => uc.ID == ID);
        }
        public List<DbTableModel_UserCat> GetUserCats() {
            return UserCats.ToList();
        }
        public DbTableModel_UserCat AddUserCat(int appUserID, int categoryID) {
            DbTableModel_UserCat userCat = new DbTableModel_UserCat { AppUserID = appUserID, CategoryID = categoryID };
            UserCats.Add(userCat);
            SaveChanges();
            return userCat;
        }
        public bool RemoveUserCat(int userCatID) {
            DbTableModel_UserCat? userCat = UserCats.FirstOrDefault(uc => uc.ID == userCatID);
            if (userCat != null) {
                UserCats.Remove(userCat);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Groups
        public DbTableModel_Group? GetGroup(int ID) {
            return Groups.FirstOrDefault(g => g.ID == ID);
        }
        public List<DbTableModel_Group> GetGroups() {
            return Groups.ToList();
        }

        public DbTableModel_Group AddGroup(string name, string gameName, string gameUrl, int editionID, string? gameBannerUrl = null) {
            DbTableModel_Group group = new DbTableModel_Group { Name = name, GameName = gameName, GameUrl = gameUrl, EditionID = editionID };
            if (gameBannerUrl != null) {
                group.GameBannerUrl = gameBannerUrl;
            }
            Groups.Add(group);
            SaveChanges();
            return group;
        }
        public bool RemoveGroup(int groupID) {
            DbTableModel_Group? group = Groups.FirstOrDefault(g => g.ID == groupID);
            if (group != null) {
                Groups.Remove(group);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Participants
        public DbTableModel_Participant? GetParticipant(int ID) {
            return Participants.FirstOrDefault(p => p.ID == ID);
        }
        public List<DbTableModel_Participant> GetParticipants() {
            return Participants.ToList();
        }
        public DbTableModel_Participant AddParticipant(string name, int? editionID = null) {
            DbTableModel_Participant participant = new DbTableModel_Participant { Name = name };
            if (editionID != null) {
                participant.EditionID = editionID.Value;
            }
            Participants.Add(participant);
            SaveChanges();
            return participant;
        }
        public bool RemoveParticipant(int participantID) {
            DbTableModel_Participant? participant = Participants.FirstOrDefault(p => p.ID == participantID);
            if (participant != null) {
                Participants.Remove(participant);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region GroupParticipants
        public DbTableModel_GroupParticipant? GetGroupParticipant(int ID) {
            return GroupParticipants.FirstOrDefault(gp => gp.ID == ID);
        }

        public List<DbTableModel_GroupParticipant> GetGroupParticipants() {
            return GroupParticipants.ToList();
        }

        public DbTableModel_GroupParticipant AddGroupParticipant(int groupID, int participantID) {
            DbTableModel_GroupParticipant groupParticipant = new DbTableModel_GroupParticipant { GroupID = groupID, ParticipantID = participantID };
            GroupParticipants.Add(groupParticipant);
            SaveChanges();
            return groupParticipant;
        }
        public bool RemoveGroupParticipant(int groupID) {
            DbTableModel_GroupParticipant? groupParticipant = GroupParticipants.FirstOrDefault(gp => gp.ID == groupID);
            if (groupParticipant != null) {
                GroupParticipants.Remove(groupParticipant);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Grades
        public DbTableModel_Grade? GetGrade(int ID) {
            return Grades.FirstOrDefault(g => g.ID == ID);
        }
        public List<DbTableModel_Grade> GetGrades() {
            return Grades.ToList();
        }

        public DbTableModel_Grade AddGrade(int numValue, string comment, int groupID, int userCatID, int? gradeType = null) {
            if (gradeType == null) {
                // Get the group and its editionID
                DbTableModel_Group? group = this.GetGroup(groupID);
                if (group != null) {
                    // Get the edition by editionID and its GradeType
                    DbTableModel_Edition? edition = this.GetEdition(group.EditionID);
                    if (edition != null) {
                        gradeType = edition.GradeType;
                    }
                }

                if (gradeType == null) {
                    throw new Exception("Unable to retrieve gradeType from group->edition!");
                }
            }

            DbTableModel_Grade grade = new DbTableModel_Grade { NumValue = numValue, Comment = comment, GroupId = groupID, UserCatId = userCatID, GradeType = gradeType.Value };
            Grades.Add(grade);
            SaveChanges();
            return grade;
        }

        public bool RemoveGrade(int gradeID) {
            DbTableModel_Grade? grade = Grades.FirstOrDefault(g => g.ID == gradeID);
            if (grade != null) {
                Grades.Remove(grade);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Resolvers
        #endregion
    }
}

