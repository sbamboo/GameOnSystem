using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameOnSystem {
    internal class AppDbContext : DbContext {
        private readonly bool _useSqlite;
        private readonly string _mysqlConnectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (_useSqlite) {
                optionsBuilder.UseSqlite("Data Source=gameon_v2.db");
            } else {
                optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=gameon_v2.db");
        }

        public AppDbContext(bool useSqlite, string mysqlConnectionString) {
            _useSqlite = useSqlite;
            _mysqlConnectionString = mysqlConnectionString;

            Database.EnsureCreated();
        }
    }
}
