using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        }

        public void EnsureDbTables() {
            Database.EnsureCreated();
        }

        public void ClearDbTables() {
            // Disable foreign key constraints for safety
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
            // Disable foreign key constraints for safety
            Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");

            // Truncate each table (more efficient than DELETE FROM)
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

        public void SetOption(string field, string value) {
            DbTableModel_Option? option = Options.FirstOrDefault(o => o.Field == field);
            if (option == null) {
                Options.Add(new DbTableModel_Option { Field = field, Value = value });
            } else {
                option.Value = value;
            }
            SaveChanges();
        }

        #endregion

        #region Editions

        public DbTableModel_Edition? GetEdition(int ID) {
            return Editions.FirstOrDefault(e => e.ID == ID);
        }

        public List<DbTableModel_Edition> GetEditions() {
            return Editions.ToList();
        }

        public DbTableTool_Edition? GetEditionAsTool(int ID) {
            DbTableModel_Edition? edition = Editions.FirstOrDefault(e => e.ID == ID);
            if (edition == null) {
                return null;
            }
            return new DbTableTool_Edition(this, edition);
        }

        public List<DbTableTool_Edition> GetEditionsAsTools() {
            List<DbTableTool_Edition> editions = new List<DbTableTool_Edition>();
            foreach (DbTableModel_Edition edition in Editions) {
                editions.Add(new DbTableTool_Edition(this, edition));
            }
            return editions;
        }

        public bool AddEdition(string Name, string Theme, int GradeMin, int GradeMax, int GradeType, bool IsActive, DateTime? GradingDeadline) {
            Editions.Add(new DbTableModel_Edition { Name = Name, Theme = Theme, GradeMin = GradeMin, GradeMax = GradeMax, GradeType = GradeType, IsActive = IsActive, GradingDeadline = GradingDeadline });
            SaveChanges();
            return true;
        }

        public DbTableModel_Edition? GetActiveEdition() {
            return Editions.FirstOrDefault(e => e.IsActive == true);
        }

        #endregion

        #region AppUsers
        public int? ValidateUserLogin(string Email, string Password) {
            DbTableModel_AppUser? user = AppUsers.FirstOrDefault(u => u.Email == Email && u.Password == Password);
            if (user != null) {
                return user.ID;
            }
            return null;
        }

        public DbTableModel_AppUser? GetUser(int ID) {
            return AppUsers.FirstOrDefault(u => u.ID == ID);
        }
        #endregion
    }
}

