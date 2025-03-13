using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOnSystem {

    /// <summary>
    /// Helper class for managing data in and out of the database. (Ex. GradeTypes)
    /// </summary>
    internal static class DbTableHelper {

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

        public static string GetGradeTypeName(int gradeType) {
            switch (gradeType) {
                case 1:
                    return "Integers";
                case 2:
                    return "Letters A-F";
                case 3:
                    return "Letters A-Z";
                case 4:
                    return "Letters A-Ö";
                case 5:
                    return "Abbriv IG-MVG";
                default:
                    return "Unknown";
            }
        }

        public static bool IsValidForType(int gradeType, int numValue, int? intMin = null, int? intMax = null) {
            switch (gradeType) {
                // Integers (i to i) {min-max-range: x > x}
                case 1:
                    if (intMin != null && intMax != null) {
                        return numValue >= intMin && numValue <= intMax;
                    }
                    return true;
                // Letters A-F {min-max-range: 0 > 5}
                case 2:
                    return numValue >= 0 && numValue <= 5;
                // Letters A-Z {min-max-range: 0 > 25}
                case 3:
                    return numValue >= 0 && numValue <= 25;
                // Letters A-Ö {min-max-range: 0 > 28}
                case 4:
                    return numValue >= 0 && numValue <= 28;
                // Abbriv IG-MVG (IG, G, VG, MVG) {min-max-range: 0 > 3}
                case 5:
                    return numValue >= 0 && numValue <= 3;
                // Unknown
                default:
                    return false;
            }
        }

        public static string GetGradeAsString(int gradeType, int numValue) {
            switch (gradeType) {
                // Integers (i to i) {min-max-range: x > x}
                case 1:
                    return numValue.ToString();
                // Letters A-F {min-max-range: 0 > 5}
                case 2:
                    if (numValue < 0 || numValue > 5) {
                        return numValue.ToString();
                    }
                    return ((char)(numValue + 65)).ToString();

                // Letters A-Z {min-max-range: 0 > 25}
                case 3:
                    if (numValue < 0 || numValue > 25) {
                        return numValue.ToString();
                    }
                    return ((char)(numValue + 65)).ToString();
                // Letters A-Ö {min-max-range: 0 > 28}
                case 4:
                    if (numValue < 0 || numValue > 28) {
                        return numValue.ToString();
                    }
                    if (numValue < 26) {
                        return ((char)(numValue + 65)).ToString();
                    } else {
                        switch (numValue) {
                            case 26:
                                return "Å";
                            case 27:
                                return "Ä";
                            case 28:
                                return "Ö";
                            default:
                                return numValue.ToString();
                        }
                    }

                // Abbriv IG-MVG (IG, G, VG, MVG) {min-max-range: 0 > 3}
                case 5:
                    switch (numValue) {
                        case 0:
                            return "IG";
                        case 1:
                            return "G";
                        case 2:
                            return "VG";
                        case 3:
                            return "MVG";
                        default:
                            return numValue.ToString();
                    }
                // Unknown
                default:
                    return numValue.ToString();
            }
        }

        public static int GetGradeFromString(int gradeType, string gradeString, int? defaultValue = null, int? intMin = null, int? intMax = null) {

            int _defaultValue = 0;

            List<string> az = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            List<string> ao = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Å", "Ä", "Ö" };

            if (defaultValue != null) {
                _defaultValue = defaultValue.Value;
            }

            if (string.IsNullOrEmpty(gradeString)) {
                return _defaultValue;
            }
            
            if (gradeType < 1 || gradeType > 5) {
                if (int.TryParse(gradeString, out int intValue)) {
                    // If min/max was given check so value is within range and if not return default
                    if (intMin != null && intMax != null) {
                        if (intValue < intMin || intValue > intMax) {
                            return _defaultValue;
                        }
                    }
                    return intValue;
                }
                Debug.Print($"DBTableHelper.GetGradeFromString.Error: Unable to turn '{gradeString}' into int.");
                return _defaultValue;
            }

            switch (gradeType) {
                // Integers (i to i) {min-max-range: x > x}
                case 1:
                    if (int.TryParse(gradeString, out int intValue)) {
                        return intValue;
                    }
                    return _defaultValue;
                // Letters A-F {min-max-range: 0 > 5}
                case 2:
                    switch (gradeString.ToUpper()) {
                        case "A":
                            return 0;
                        case "B":
                            return 1;
                        case "C":
                            return 2;
                        case "D":
                            return 3;
                        case "E":
                            return 4;
                        case "F":
                            return 5;
                        default:
                            return _defaultValue;
                    }

                // Letters A-Z {min-max-range: 0 > 25}
                case 3:
                    if (az.Contains(gradeString.ToUpper())) {
                        return az.IndexOf(gradeString.ToUpper());
                    }
                    return _defaultValue;

                // Letters A-Ö {min-max-range: 0 > 28}
                case 4:
                    if (ao.Contains(gradeString.ToUpper())) {
                        return ao.IndexOf(gradeString.ToUpper());
                    }
                    return _defaultValue;

                // Abbriv IG-MVG (IG, G, VG, MVG) {min-max-range: 0 > 3}
                case 5:
                    switch (gradeString.ToUpper()) {
                        case "IG":
                            return 0;
                        case "G":
                            return 1;
                        case "VG":
                            return 2;
                        case "MVG":
                            return 3;
                        default:
                            return _defaultValue;
                    }

                // Unknown
                default:
                    return _defaultValue;
            }
        }
    }

    /// <summary>
    /// Table for storing application options (mostly internal), versioning metadata and database state.
    /// </summary>
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

        public List<DbTableModel_Group> GetGroups(AppDbContext appDbContext) {
            return appDbContext.Groups.Where(g => g.EditionID == this.ID).ToList();
        }

        public List<DbTableModel_Participant> GetParticipants(AppDbContext appDbContext) {
            return appDbContext.Participants.Where(p => p.EditionID == this.ID).ToList();
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
            return appDbContext.UserCats.Any(uc => uc.AppUserID == this.ID && uc.CategoryID == categoryID);
        }

        public List<DbTableModel_Category> GetFocusCategories(AppDbContext appDbContext) {
            List<DbTableModel_Category> categories = new List<DbTableModel_Category>();
            List<DbTableModel_UserCat> userCats = appDbContext.UserCats.Where(uc => uc.AppUserID == this.ID).ToList();
            foreach (DbTableModel_UserCat userCat in userCats) {
                categories.Add(appDbContext.Categories.First(c => c.ID == userCat.CategoryID));
            }
            return categories;
        }

        public DbTableModel_UserCat AddFocusCategory(AppDbContext appDbContext, int categoryID) {

            DbTableModel_UserCat? foundCategory = appDbContext.UserCats.FirstOrDefault(uc => uc.AppUserID == this.ID && uc.CategoryID == categoryID);

            // Add if not already
            if (foundCategory == null) {
                return appDbContext.AddUserCat(this.ID, categoryID);
            }

            return foundCategory;
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

        public int? GetUserCatForFocusCategory(AppDbContext appDbContext, int categoryID) {
            DbTableModel_UserCat? userCat = appDbContext.UserCats.FirstOrDefault(uc => uc.AppUserID == this.ID && uc.CategoryID == categoryID);
            if (userCat == null) {
                return null;
            }
            return userCat.ID;
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
            }
            catch (Exception e) {
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

        public List<DbTableModel_Grade> GetGrades(AppDbContext appDbContext) {
            return appDbContext.Grades.Where(g => g.GetUserCat(appDbContext).CategoryID == this.ID).ToList();
        }

        public int GetAverageGrade(AppDbContext appDbContext) {
            List<DbTableModel_Grade> grades = GetGrades(appDbContext).Where(g => g.NumValue != -1).ToList();

            if (grades.Count == 0) { return 0; }
            int sum = 0;
            foreach (DbTableModel_Grade grade in grades) {
                sum += grade.NumValue;
            }
            return sum / grades.Count;
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

        public DbTableModel_AppUser GetAppUser(AppDbContext appDbContext) {
            return appDbContext.AppUsers.First(u => u.ID == this.AppUserID);
        }

        public DbTableModel_Category GetCategory(AppDbContext appDbContext) {
            return appDbContext.Categories.First(c => c.ID == this.CategoryID);
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

        public List<DbTableModel_Participant> GetParticipants(AppDbContext appDbContext) {
            List<DbTableModel_Participant> participants = new List<DbTableModel_Participant>();
            List<DbTableModel_GroupParticipant> groupParticipants = appDbContext.GroupParticipants.Where(gp => gp.GroupID == this.ID).ToList();
            foreach (DbTableModel_GroupParticipant groupParticipant in groupParticipants) {
                participants.Add(appDbContext.Participants.First(p => p.ID == groupParticipant.ParticipantID));
            }
            return participants;
        }

        public bool HasParticipant(AppDbContext appDbContext, int participantID) {
            return appDbContext.GroupParticipants.Any(gp => gp.ParticipantID == participantID && gp.GroupID == this.ID);
        }

        public void AddParticipant(AppDbContext appDbContext, int participantID) {
            // Add if not already
            if (!HasParticipant(appDbContext, participantID)) {
                appDbContext.AddGroupParticipant(this.ID, participantID);
            }
        }

        public void RemoveParticipant(AppDbContext appDbContext, int participantID) {
            // Remove if exists
            if (HasParticipant(appDbContext, participantID)) {
                DbTableModel_GroupParticipant groupParticipant = appDbContext.GroupParticipants.First(gp => gp.ParticipantID == participantID && gp.GroupID == this.ID);
                appDbContext.RemoveGroupParticipant(groupParticipant.ID);
            }
        }

        public void CleanParticipants(AppDbContext appDbContext) {
            List<DbTableModel_GroupParticipant> groupParticipants = appDbContext.GroupParticipants.Where(gp => gp.GroupID == this.ID).ToList();
            foreach (DbTableModel_GroupParticipant groupParticipant in groupParticipants) {
                appDbContext.RemoveGroupParticipant(groupParticipant.ID);
            }
        }

        public List<DbTableModel_Grade> GetGrades(AppDbContext appDbContext) {
            return appDbContext.Grades.Where(g => g.GroupId == this.ID).ToList();
        }

        public Dictionary<DbTableModel_Category, int> GetGradesPerCategory(AppDbContext appDbContext) { 
            return GetGrades(appDbContext).Where(g => g.NumValue != -1).GroupBy(g => g.GetCategory(appDbContext)).ToDictionary(g => g.Key, g => g.Sum(g => g.NumValue));
        }

        public Dictionary<DbTableModel_Category, int> GetAverageGradesPerCategory(AppDbContext appDbContext) {
            return GetGrades(appDbContext).Where(g => g.NumValue != -1).GroupBy(g => g.GetCategory(appDbContext)).ToDictionary(g => g.Key, g => g.Sum(g => g.NumValue) / g.Count());
        }

        // Gets the average grade using the average-per-category values
        public double GetAverageGrade(AppDbContext appDbContext) {
            Dictionary<DbTableModel_Category, int> averageGrades = GetAverageGradesPerCategory(appDbContext);
            if (averageGrades.Count == 0) { return 0; }
            int sum = 0;
            foreach (KeyValuePair<DbTableModel_Category, int> averageGrade in averageGrades) {
                sum += averageGrade.Value;
            }
            return sum / averageGrades.Count;
        }

        // Gets the average grade using the raw grade values
        public double GetAverageGrade_NoCatAverages(AppDbContext appDbContext) {
            List<DbTableModel_Grade> grades = GetGrades(appDbContext).Where(g => g.NumValue != -1).ToList();
            if (grades.Count == 0) { return 0; }
            int sum = 0;
            foreach (DbTableModel_Grade grade in grades) {
                sum += grade.NumValue;
            }
            return sum / grades.Count;
        }

        public int GetAverageGradeRounded(AppDbContext appDbContext) {
            return (int)Math.Round(GetAverageGrade(appDbContext));
        }

        public int GetAverageGradeRounded_NoCatAverages(AppDbContext appDbContext) {
            return (int)Math.Round(GetAverageGrade_NoCatAverages(appDbContext));
        }

        public DbTableModel_Edition GetEdition(AppDbContext appDbContext) {
            return appDbContext.Editions.First(e => e.ID == this.EditionID);
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

        public List<DbTableModel_Group> GetGroups(AppDbContext appDbContext) {
            List<DbTableModel_Group> groups = new List<DbTableModel_Group>();
            List<DbTableModel_GroupParticipant> groupParticipants = appDbContext.GroupParticipants.Where(gp => gp.ParticipantID == this.ID).ToList();
            foreach (DbTableModel_GroupParticipant groupParticipant in groupParticipants) {
                groups.Add(appDbContext.Groups.First(g => g.ID == groupParticipant.GroupID));
            }
            return groups;
        }

        public bool InGroup(AppDbContext appDbContext, int groupID) {
            return appDbContext.GroupParticipants.Any(gp => gp.GroupID == groupID && gp.ParticipantID == this.ID);
        }

        public void AddToGroup(AppDbContext appDbContext, int groupID) {
            // Add if not already
            if (!InGroup(appDbContext, groupID)) {
                appDbContext.AddGroupParticipant(this.ID, groupID);
            }
        }

        public void RemoveFromGroup(AppDbContext appDbContext, int groupID) {
            // Remove if exists
            if (InGroup(appDbContext, groupID)) {
                DbTableModel_GroupParticipant groupParticipant = appDbContext.GroupParticipants.First(gp => gp.GroupID == groupID && gp.ParticipantID == this.ID);
                appDbContext.RemoveGroupParticipant(groupParticipant.ID);
            }
        }

        public void CleanGroups(AppDbContext appDbContext) {
            List<DbTableModel_GroupParticipant> groupParticipants = appDbContext.GroupParticipants.Where(gp => gp.ParticipantID == this.ID).ToList();
            foreach (DbTableModel_GroupParticipant groupParticipant in groupParticipants) {
                appDbContext.RemoveGroupParticipant(groupParticipant.ID);
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

        public DbTableModel_Group GetGroup(AppDbContext appDbContext) {
            return appDbContext.Groups.First(g => g.ID == this.GroupID);
        }

        public DbTableModel_Participant GetParticipant(AppDbContext appDbContext) {
            return appDbContext.Participants.First(p => p.ID == this.ParticipantID);
        }
    }

    internal class DbTableModel_Grade {

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

        public DbTableModel_Group GetGroup(AppDbContext appDbContext) {
            return appDbContext.Groups.First(g => g.ID == this.GroupId);
        }

        public DbTableModel_UserCat GetUserCat(AppDbContext appDbContext) {
            return appDbContext.UserCats.First(uc => uc.ID == this.UserCatId);
        }

        public DbTableModel_AppUser GetAppUser(AppDbContext appDbContext) {
            return appDbContext.AppUsers.First(u => u.ID == GetUserCat(appDbContext).AppUserID);
        }

        public DbTableModel_Category GetCategory(AppDbContext appDbContext) {
            return appDbContext.Categories.First(c => c.ID == GetUserCat(appDbContext).CategoryID);
        }

        public string GetGradeTypeName() {
            return DbTableHelper.GetGradeTypeName(this.GradeType);
        }

        public bool IsValidForType(int numValue, bool checkEditionMinMax = false, AppDbContext? appDbContext = null) {
            if (checkEditionMinMax && appDbContext != null) {
                DbTableModel_Edition edition = GetGroup(appDbContext).GetEdition(appDbContext);
                return DbTableHelper.IsValidForType(this.GradeType, numValue, edition.GradeMin, edition.GradeMax);
            }
            return DbTableHelper.IsValidForType(this.GradeType, numValue);
        }

        public string GetGradeAsString() {
            return DbTableHelper.GetGradeAsString(this.GradeType, this.NumValue);
        }
    }
}
