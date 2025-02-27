using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace GameOnSystem {
    internal class AppDbContext : DbContext {
        private readonly bool _useSqlite;
        private readonly string? _mysqlConnectionString;

        public DbSet<DbTableModel_Option> Options { get; set; }
        public DbSet<DbTableModel_Edition> Editions { get; set; }
        public DbSet<DBTableModel_AppUser> AppUsers { get; set; }
        public DbSet<DbTableModel_Category> Categories { get; set; }
        public DbSet<DbTableModel_UserCat> UserCats { get; set; }
        public DbSet<DbTableModel_Group> Groups { get; set; }
        public DbSet<DbTableModel_Participant> Participants { get; set; }
        public DbSet<DbTableModel_GroupParticipant> GroupParticipants { get; set; }
        public DbSet<DbTableModel_Grade> Grades { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (_useSqlite) {
                optionsBuilder.UseSqlite("Data Source=gameon_v2.db");
            } else {
                if (_mysqlConnectionString == null) {
                    throw new ArgumentNullException(nameof(_mysqlConnectionString), "MySQL connection string is required when not using a local SQLite database.");
                }
                optionsBuilder.UseMySql(_mysqlConnectionString, ServerVersion.AutoDetect(_mysqlConnectionString));
            }
        }

        public AppDbContext(bool useSqlite=true, string? mysqlConnectionString=null) {
            _useSqlite = useSqlite;
            _mysqlConnectionString = mysqlConnectionString;

            Database.EnsureCreated();
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
    }
}

