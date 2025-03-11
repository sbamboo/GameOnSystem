using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOnSystem {
    internal class DbTableModel_Option {
        public int ID { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }

    internal class DbTableModel_Edition {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public int GradeMin { get; set; }
        public int GradeMax { get; set; }
        /*
         * NumGradeType:
         *   1 = "Integers" (i to i) {min-max-range: x > x}
         *   2 = "Letters A-F"       {min-max-range: 0 > 5}
         *   3 = "Letters A-Z"       {min-max-range: 0 > 25}
         *   4 = "Letters A-Ö"       {min-max-range: 0 > 28}
         *   5 = "Abbriv IG-MVG" (IG, G, VG, MVG) {min-max-range: 0 > 3}
         *   
         * When changing the GradeType the admin defined value will be reset,
         *   and for types other then "Integers" min/max are auto filled.
        */
        public int GradeType { get; set; }
        public bool IsActive { get; set; }
        public DateTime? GradingDeadline { get; set; }

        // Methods //
        public bool Update(AppDbContext appDbContext, string? name = null, string? theme = null, int? gradeMin = null, int? gradeMax = null, int? gradeType = null, bool? isActive = null, DateTime? gradingDeadline = null) {
            try {
                if (name != null) { this.Name = name; }
                if (theme != null) { this.Theme = theme; }
                if (gradeMin != null) { this.GradeMin = gradeMin.Value; }
                if (gradeMax != null) { this.GradeMax = gradeMax.Value; }
                if (gradeType != null) { this.GradeType = gradeType.Value; }
                if (isActive != null) { this.IsActive = isActive.Value; }
                if (gradingDeadline != null) { this.GradingDeadline = gradingDeadline.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_Edition.Update.Error: {e.Message}");
                return false;
            }
        }

        public int GetGroupCount(AppDbContext appDbContext) {
            return appDbContext.Groups.Count(g => g.EditionID == this.ID);
        }
        public List<DbTableModel_Group> GetGroups(AppDbContext appDbContext) {
            return appDbContext.Groups.Where(g => g.EditionID == this.ID).ToList();
        }
    }

    internal class DbTableModel_AppUser {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProtected { get; set; }
        public int? ActiveEditionID { get; set; }

        // Methods //
        public bool Update(AppDbContext appDbContext, string? name = null, string? email = null, string? password = null, bool? isAdmin = null, int? activeEditionID = null) {
            try {
                if (name != null) { this.Name = name; }
                if (email != null) { this.Email = email; }
                if (password != null) { this.Password = password; }
                if (isAdmin != null) { this.IsAdmin = isAdmin.Value; }
                if (activeEditionID != null) { this.ActiveEditionID = activeEditionID.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_AppUser.Update.Error: {e.Message}");
                return false;
            }
        }
        public void SetActiveEdition(int editionID) {
            this.ActiveEditionID = editionID;
        }

        public void ClearActiveEdition() {
            this.ActiveEditionID = null;
        }

        public bool HasFocusCategory(AppDbContext appDbContext, int categoryID) {
            return appDbContext.UserCats.Any(uc => uc.CategoryID == categoryID);
        }

        public List<DbTableModel_Category> GetFocusCategories(AppDbContext appDbContext) {
            List<DbTableModel_Category> categories = new List<DbTableModel_Category>();
            List<DbTableModel_UserCat> userCats = appDbContext.UserCats.Where(uc => uc.AppUserID == this.ID).ToList();
            foreach (DbTableModel_UserCat userCat in userCats) {
                categories.Add(appDbContext.Categories.First(c => c.ID == userCat.CategoryID));)
            }
            return categories;
        }

        public void AddFocusCategory(AppDbContext appDbContext, int categoryID) {
            // Add if not already
            if (!HasFocusCategory(appDbContext, categoryID)) {
                appDbContext.AddUserCat(this.ID, categoryID);
            }
        }

        public void RemoveFocusCategory(AppDbContext appDbContext, int categoryID) {
            // Remove if exists
            if (HasFocusCategory(appDbContext, categoryID)) {
                DbTableModel_UserCat userCat = appDbContext.UserCats.First(uc => uc.AppUserID == this.ID && uc.CategoryID == categoryID);
                appDbContext.RemoveUserCat(userCat.ID);
            }
        }

        public void CleanFocusCategories(AppDbContext appDbContext) {
            List<DbTableModel_UserCat> userCats = appDbContext.UserCats.Where(uc => uc.AppUserID == this.ID).ToList();
            foreach (DbTableModel_UserCat userCat in userCats) {
                appDbContext.RemoveUserCat(userCat.ID);
            }
        }
    }

    internal class DbTableModel_Category {
        public int ID { get; set; }
        public string Name { get; set; }

        // Methods //
        public bool Update(AppDbContext appDbContext, string? name = null) {
            try {
                if (name != null) { this.Name = name; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_Category.Update.Error: {e.Message}");
                return false;
            }
        }

        public bool HasFucusUser(AppDbContext appDbContext, int appUserID) {
            return appDbContext.UserCats.Any(uc => uc.AppUserID == appUserID);
        }

        public List<DbTableModel_AppUser> GetFocusUsers(AppDbContext appDbContext) {
            List<DbTableModel_AppUser> users = new List<DbTableModel_AppUser>();
            List<DbTableModel_UserCat> userCats = appDbContext.UserCats.Where(uc => uc.CategoryID == this.ID).ToList();
            foreach (DbTableModel_UserCat userCat in userCats) {
                users.Add(appDbContext.AppUsers.First(u => u.ID == userCat.AppUserID));
            }
            return users;
        }

        public void AddFocusUser(AppDbContext appDbContext, int appUserID) {
            // Add if not already
            if (!HasFucusUser(appDbContext, appUserID)) {
                appDbContext.AddUserCat(appUserID, this.ID);
            }
        }

        public void RemoveFocusUser(AppDbContext appDbContext, int appUserID) {
            // Remove if exists
            if (HasFucusUser(appDbContext, appUserID)) {
                DbTableModel_UserCat userCat = appDbContext.UserCats.First(uc => uc.AppUserID == appUserID && uc.CategoryID == this.ID);
                appDbContext.RemoveUserCat(userCat.ID);
            }
        }

        public void CleanFocusUsers(AppDbContext appDbContext) {
            List<DbTableModel_UserCat> userCats = appDbContext.UserCats.Where(uc => uc.CategoryID == this.ID).ToList();
            foreach (DbTableModel_UserCat userCat in userCats) {
                appDbContext.RemoveUserCat(userCat.ID);
            }
        }
    }

    internal class DbTableModel_UserCat {
        public int ID { get; set; }
        public int AppUserID { get; set; }
        public int CategoryID { get; set; }

        // Methods //
        public bool Update(AppDbContext appDbContext, int? appUserID = null, int? categoryId = null) {
            try {
                if (appUserID != null) { this.AppUserID = appUserID.Value; }
                if (categoryId != null) { this.CategoryID = categoryId.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_UserCat.Update.Error: {e.Message}");
                return false;
            }
        }
    }

    internal class DbTableModel_Group {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
        public int EditionID { get; set; }


        // Methods //
        public bool Update(AppDbContext appDbContext, string? name = null, string? gameName = null, string? gameUrl = null, string? gameBannerUrl = null, int? editionID = null) {
            try {
                if (name != null) { this.Name = name; }
                if (gameName != null) { this.GameName = gameName; }
                if (gameUrl != null) { this.GameUrl = gameUrl; }
                if (gameBannerUrl != null) { this.GameBannerUrl = gameBannerUrl; }
                if (editionID != null) { this.EditionID = editionID.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_Group.Update.Error: {e.Message}");
                return false;
            }
        }
    }

    internal class DbTableModel_Participant {
        public int ID { get; set; }
        public string Name { get; set; }
        public int EditionID { get; set; }


        // Methods //
        public bool Update(AppDbContext appDbContext, string? name = null, int? editionID = null) {
            try {
                if (name != null) { this.Name = name; }
                if (editionID != null) { this.EditionID = editionID.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_Participant.Update.Error: {e.Message}");
                return false;
            }
        }
    }

    internal class DbTableModel_GroupParticipant {
        public int ID { get; set; }
        public int ParticipantID { get; set; }
        public int GroupID { get; set; }

        // Methods //
        public bool Update(AppDbContext appDbContext, int? participantID, int? groupID) { 
            try {
                if (participantID != null) { this.ParticipantID = participantID.Value; }
                if (groupID != null) { this.GroupID = groupID.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_GroupParticipant.Update.Error: {e.Message}");
                return false;
            }
        }
    }

    internal class DbTableModel_Grade {
        public int ID { get; set; }
        public int NumValue { get; set; }
        public int GradeType { get; set; }
        public string Comment { get; set; }
        public int GroupId { get; set; }
        public int UserCatId { get; set; }

        // Methods //
        public bool Update(AppDbContext appDbContext, int? numValue = null, int? gradeType = null, string? comment = null, int? groupId = null, int? userCatId = null) {
            try {
                if (numValue != null) { this.NumValue = numValue.Value; }
                if (gradeType != null) { this.GradeType = gradeType.Value; }
                if (comment != null) { this.Comment = comment; }
                if (groupId != null) { this.GroupId = groupId.Value; }
                if (userCatId != null) { this.UserCatId = userCatId.Value; }
                appDbContext.SaveChanges();
                return true;
            } catch (Exception e) {
                Debug.Print($"DbTableModel_Grade.Update.Error: {e.Message}");
                return false;
            }
        }
    }
}
