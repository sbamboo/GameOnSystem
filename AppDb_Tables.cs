using System;
using System.Collections.Generic;
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
    }

    internal class DBTableModel_AppUser {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProtected { get; set; }
    }

    internal class DbTableModel_Category {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    internal class DbTableModel_UserCat {
        public int ID { get; set; }
        public int AppUserID { get; set; }
        public int CategoryID { get; set; }
    }

    internal class DbTableModel_Group {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public string GameBannerUrl { get; set; }
        public int EditionID { get; set; }
    }
    internal class DbTableModel_Participants {
        public int ID { get; set; }
        public string Name { get; set; }
        public int EditionId { get; set; }
    }
    internal class DbTableModel_GroupParticipants {
        public int ID { get; set; }
        public int ParticipantID { get; set; }
        public int GroupID { get; set; }
    }
    internal class DbTableModel_Grade {
        public int ID { get; set; }
        public int NumValue { get; set; }
        public int GradeType { get; set; }
        public string Comment { get; set; }
        public int GroupId { get; set; }
        public int UserCatId { get; set; }
    }
}
