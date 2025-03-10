using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameOnSystem.DbTableTool_Group;

namespace GameOnSystem {

    internal class DbTableTool {
        protected AppDbContext appDbContext;

        public DbTableTool(AppDbContext appDbContext) {
            this.appDbContext = appDbContext;
        }

        public void SaveChanges() {
            appDbContext.SaveChanges();
        }
    }

    internal class DbTableTool_Edition : DbTableTool {
        private DbTableModel_Edition table;

        public DbTableTool_Edition(AppDbContext appDbContext, DbTableModel_Edition tableInstance) : base(appDbContext) {
            table = tableInstance;
        }

        public bool UpdateTable(string? Name = null, string? Theme = null, int? GradeMin = null, int? GradeMax = null, int? GradeType = null, bool? IsActive = null, DateTime? GradingDeadline = null) {
            if (Name != null) { table.Name = Name; }
            if (Theme != null) { table.Theme = Theme; }
            if (GradeMin != null) { table.GradeMin = (int)GradeMin; }
            if (GradeMax != null) { table.GradeMax = (int)GradeMax; }
            if (GradeType != null) { table.GradeType = (int)GradeType; }
            if (IsActive != null) { table.IsActive = (bool)IsActive; }
            if (GradingDeadline != null) { table.GradingDeadline = GradingDeadline; }
            appDbContext.SaveChanges();
            return true;
        }

        public int GetGroupCount() {
            return appDbContext.Groups.Where(g => g.EditionID == table.ID).Count();
        }

        public List<DbTableModel_Group> GetGroups() {
            return appDbContext.Groups.Where(g => g.EditionID == table.ID).ToList();
        }

        public List<DbTableTool_Group> GetGroupsAsTools() {
            List<DbTableTool_Group> groups = new List<DbTableTool_Group>();
            foreach (DbTableModel_Group group in appDbContext.Groups.Where(g => g.EditionID == table.ID)) {
                groups.Add(new DbTableTool_Group(appDbContext, group));
            }
            return groups;
        }

        public List<DbTableModel_Participant> GetParticipants() {
            return appDbContext.Participants.Where(p => p.EditionID == table.ID).ToList();
        }

        public List<DbTableTool_Participant> GetParticipantsAsTools() {
            List<DbTableTool_Participant> participants = new List<DbTableTool_Participant>();
            foreach (DbTableModel_Participant participant in appDbContext.Participants.Where(p => p.EditionID == table.ID)) {
                participants.Add(new DbTableTool_Participant(appDbContext, participant));
            }
            return participants;
        }

        public List<DbTableModel_Participant> GetGroupAssignedParticipants() {
            // Get all participants that exists in groups (check GroupParticipants)
            List<DbTableModel_Participant> participants = new List<DbTableModel_Participant>();
            foreach (DbTableModel_Group group in appDbContext.Groups.Where(g => g.EditionID == table.ID)) {
                foreach (DbTableModel_GroupParticipant gp in appDbContext.GroupParticipants.Where(gp => gp.GroupID == group.ID)) {
                    participants.Add(appDbContext.Participants.Where(p => p.ID == gp.ParticipantID).First());
                }
            }
            return participants;
        }

        public List<DbTableModel_Participant> GetGroupUnAssignedParticipants() {
            // Get all participants that are not in any group
            List<DbTableModel_Participant> participants = new List<DbTableModel_Participant>();
            foreach (DbTableModel_Participant participant in appDbContext.Participants.Where(p => p.EditionID == table.ID)) {
                if (appDbContext.GroupParticipants.Where(gp => gp.ParticipantID == participant.ID).Count() == 0) {
                    participants.Add(participant);
                }
            }
            return participants;
        }
    }

    internal class DbTableTool_AppUser : DbTableTool {
        private DbTableModel_AppUser table;

        public DbTableTool_AppUser(AppDbContext appDbContext, DbTableModel_AppUser tableInstance) : base(appDbContext) {
            table = tableInstance;
        }

        public bool UpdateTable(string? Name = null, string? Email = null, string? Password = null, bool? IsAdmin = null, bool? IsProtected = null) {
            if (Name != null) { table.Name = Name; }
            if (Email != null) { table.Email = Email; }
            if (Password != null) { table.Password = Password; }
            if (IsAdmin != null) { table.IsAdmin = (bool)IsAdmin; }
            if (IsProtected != null) { table.IsProtected = (bool)IsProtected; }
            appDbContext.SaveChanges();
            return true;
        }

        public List<DbTableModel_Category> GetCategories() {
            List<DbTableModel_Category> categories = new List<DbTableModel_Category>();
            foreach (DbTableModel_UserCat userCat in appDbContext.UserCats.Where(uc => uc.AppUserID == table.ID)) {
                categories.Add(appDbContext.Categories.Where(c => c.ID == userCat.CategoryID).First());
            }
            return categories;
        }

        public List<DbTableTool_Category> GetCategoriesAsTools() {
            List<DbTableTool_Category> categories = new List<DbTableTool_Category>();
            foreach (DbTableModel_UserCat userCat in appDbContext.UserCats.Where(uc => uc.AppUserID == table.ID)) {
                categories.Add(new DbTableTool_Category(appDbContext, appDbContext.Categories.Where(c => c.ID == userCat.CategoryID).First()));
            }
            return categories;
        }
    }

    internal class DbTableTool_Category : DbTableTool {
        private DbTableModel_Category table;

        public DbTableTool_Category(AppDbContext appDbContext, DbTableModel_Category tableInstance) : base(appDbContext) {
            table = tableInstance;
        }

        public bool UpdateTable(string? Name = null) {
            if (Name != null) { table.Name = Name; }
            appDbContext.SaveChanges();
            return true;
        }

        public List<DbTableModel_AppUser> GetUsers() {
            List<DbTableModel_AppUser> users = new List<DbTableModel_AppUser>();
            foreach (DbTableModel_UserCat userCat in appDbContext.UserCats.Where(uc => uc.CategoryID == table.ID)) {
                users.Add(appDbContext.AppUsers.Where(u => u.ID == userCat.AppUserID).First());
            }
            return users;
        }

        public List<DbTableTool_AppUser> GetUsersAsTools() {
            List<DbTableTool_AppUser> users = new List<DbTableTool_AppUser>();
            foreach (DbTableModel_UserCat userCat in appDbContext.UserCats.Where(uc => uc.CategoryID == table.ID)) {
                users.Add(new DbTableTool_AppUser(appDbContext, appDbContext.AppUsers.Where(u => u.ID == userCat.AppUserID).First()));
            }
            return users;
        }
    }

    internal class DbTableTool_Group : DbTableTool {
        private DbTableModel_Group table;

        public DbTableTool_Group(AppDbContext appDbContext, DbTableModel_Group tableInstance) : base(appDbContext) {
            table = tableInstance;
        }

        public bool UpdateTable(string? Name = null, string? GameName = null, string? GameUrl = null, string? GameBannerUrl = null, int? EditionID = null) {
            if (Name != null) { table.Name = Name; }
            if (GameName != null) { table.GameName = GameName; }
            if (GameUrl != null) { table.GameUrl = GameUrl; }
            if (GameBannerUrl != null) { table.GameBannerUrl = GameBannerUrl; }
            if (EditionID != null) { table.EditionID = (int)EditionID; }
            appDbContext.SaveChanges();
            return true;
        }

        public List<DbTableModel_Participant> GetParticipants() {
            List<DbTableModel_Participant> participants = new List<DbTableModel_Participant>();
            foreach (DbTableModel_GroupParticipant gp in appDbContext.GroupParticipants.Where(gp => gp.GroupID == table.ID)) {
                participants.Add(appDbContext.Participants.Where(p => p.ID == gp.ParticipantID).First());
            }
            return participants;
        }

        public List<DbTableTool_Participant> GetParticipantsAsTools() {
            List<DbTableTool_Participant> participants = new List<DbTableTool_Participant>();
            foreach (DbTableModel_GroupParticipant gp in appDbContext.GroupParticipants.Where(gp => gp.GroupID == table.ID)) {
                participants.Add(new DbTableTool_Participant(appDbContext, appDbContext.Participants.Where(p => p.ID == gp.ParticipantID).First()));
            }
            return participants;
        }

        public List<DbTableModel_Grade> GetGrades() {
            return appDbContext.Grades.Where(g => g.GroupId == table.ID).ToList();
        }

        public List<DbTableTool_Grade> GetGradesAsTools() {
            List<DbTableTool_Grade> grades = new List<DbTableTool_Grade>();
            foreach (DbTableModel_Grade grade in appDbContext.Grades.Where(g => g.GroupId == table.ID)) {
                grades.Add(new DbTableTool_Grade(appDbContext, grade));
            }
            return grades;
        }

        public List<DbTableModel_Grade> GetGradesInCategory(int CategoryID) {
            DbTableModel_UserCat userCat = appDbContext.UserCats.Where(uc => uc.CategoryID == CategoryID).First();
            return appDbContext.Grades.Where(g => g.GroupId == table.ID && g.UserCatId == userCat.ID).ToList();
        }

        public List<DbTableTool_Grade> GetGradesInCategoryAsTools(int CategoryID) {
            DbTableModel_UserCat userCat = appDbContext.UserCats.Where(uc => uc.CategoryID == CategoryID).First();
            List<DbTableTool_Grade> grades = new List<DbTableTool_Grade>();
            foreach (DbTableModel_Grade grade in appDbContext.Grades.Where(g => g.GroupId == table.ID && g.UserCatId == userCat.ID)) {
                grades.Add(new DbTableTool_Grade(appDbContext, grade));
            }
            return grades;
        }
    }

    internal class DbTableTool_Participant : DbTableTool {
        private DbTableModel_Participant table;

        public DbTableTool_Participant(AppDbContext appDbContext, DbTableModel_Participant tableInstance) : base(appDbContext) {
            table = tableInstance;
        }

        public bool UpdateTable(string? Name = null, int? EditionID = null) {
            if (Name != null) { table.Name = Name; }
            if (EditionID != null) { table.EditionID = (int)EditionID; }
            appDbContext.SaveChanges();
            return true;
        }

        public void Unassign() {
            // Remove this participant from all groups
            foreach (DbTableModel_GroupParticipant gp in appDbContext.GroupParticipants.Where(gp => gp.ParticipantID == table.ID)) {
                appDbContext.GroupParticipants.Remove(gp);
            }
        }

        public void Unassign(int GroupID) {
            // Remove this participant from a specific group
            foreach (DbTableModel_GroupParticipant gp in appDbContext.GroupParticipants.Where(gp => gp.ParticipantID == table.ID && gp.GroupID == GroupID)) {
                appDbContext.GroupParticipants.Remove(gp);
            }
        }

        public void Assign(int GroupID) {
            // Assign this participant to a group
            appDbContext.GroupParticipants.Add(new DbTableModel_GroupParticipant { ParticipantID = table.ID, GroupID = GroupID });
        }

        public bool IsAssigned(int GroupID) {
            return appDbContext.GroupParticipants.Where(gp => gp.ParticipantID == table.ID && gp.GroupID == GroupID).Count() > 0;
        }
    }

    internal class DbTableTool_Grade : DbTableTool {
        private AppDbContext appDbContext;
        private DbTableModel_Grade table;

        public DbTableTool_Grade(AppDbContext appDbContext, DbTableModel_Grade tableInstance) : base(appDbContext) {
            table = tableInstance;
        }

        public bool UpdateTable(int? NumValue = null, int? GradeType = null, string? Comment = null, int? GroupId = null, int? UserCatId = null) {
            if (NumValue != null) { table.NumValue = (int)NumValue; }
            if (GradeType != null) { table.GradeType = (int)GradeType; }
            if (Comment != null) { table.Comment = Comment; }
            if (GroupId != null) { table.GroupId = (int)GroupId; }
            if (UserCatId != null) { table.UserCatId = (int)UserCatId; }
            appDbContext.SaveChanges();
            return true;
        }
    }
}
