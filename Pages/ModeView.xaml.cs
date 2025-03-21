﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOnSystem.Pages {
    /// <summary>
    /// Interaction logic for ModeSelect.xaml
    /// </summary>
    public partial class ModeSelect : UserControl {

        private readonly MainWindow windowInstance;
        private readonly UserControl? sendingView;

        public ModeSelect(MainWindow WindowInstance, UserControl? SendingView = null) {
            this.windowInstance = WindowInstance;
            this.sendingView = SendingView;

            InitializeComponent();
        }

        private void InitDatabaseValues() {

            // Editions

            DbTableModel_Edition go2023 = windowInstance.Shared.appDbContext.AddEdition(
                "GameOn 2023",            // Name
                "???",                    // Theme
                1,                        // GradeMin
                6,                        // GradeMax
                1,                        // GradeType
                false,                    // IsActive
                new DateTime(2023, 3, 12) // GradingDeadline
            );

            DbTableModel_Edition go2024 = windowInstance.Shared.appDbContext.AddEdition(
                "GameOn 2024",            // Name
                "Reflections",            // Theme
                1,                        // GradeMin
                6,                        // GradeMax
                1,                        // GradeType
                true,                     // IsActive
                new DateTime(2026, 3, 12) // GradingDeadline
            );

            // Categories

            DbTableModel_Category entertainment = windowInstance.Shared.appDbContext.AddCategory("Underhållning");
            DbTableModel_Category theme_keeping = windowInstance.Shared.appDbContext.AddCategory("Uppfyllande av tema");
            DbTableModel_Category graphics_coherence = windowInstance.Shared.appDbContext.AddCategory("Grafikens sammanhållning");
            DbTableModel_Category graphics_theme_keeping = windowInstance.Shared.appDbContext.AddCategory("Grafikens koppling till temat");
            DbTableModel_Category code_structure_and_docs = windowInstance.Shared.appDbContext.AddCategory("Programmeringskodens struktur och dokumentation");
            DbTableModel_Category physics_math_implementation = windowInstance.Shared.appDbContext.AddCategory("Implementering av fysik / matematik");
            DbTableModel_Category playability = windowInstance.Shared.appDbContext.AddCategory("Spelbarhet");

            // AppUsers

            DbTableModel_AppUser user1 = windowInstance.Shared.appDbContext.AddAppUser("AppUser1", "user1@example.com", "user1", false);
            DbTableModel_UserCat user1_entertainment = user1.AddFocusCategory(windowInstance.Shared.appDbContext, entertainment.ID);
            DbTableModel_UserCat user1_phymath       = user1.AddFocusCategory(windowInstance.Shared.appDbContext, physics_math_implementation.ID);
            DbTableModel_UserCat user1_theme_keeping = user1.AddFocusCategory(windowInstance.Shared.appDbContext, theme_keeping.ID);

            DbTableModel_AppUser user2 = windowInstance.Shared.appDbContext.AddAppUser("AppUser2", "user2@example.com", "user2", false);
            DbTableModel_UserCat user2_codestrcdocs  = user2.AddFocusCategory(windowInstance.Shared.appDbContext, code_structure_and_docs.ID);
            DbTableModel_UserCat user2_playability   = user2.AddFocusCategory(windowInstance.Shared.appDbContext, playability.ID);
            DbTableModel_UserCat user2_theme_keeping = user2.AddFocusCategory(windowInstance.Shared.appDbContext, theme_keeping.ID);

            // Participants

            DbTableModel_Participant participant1 = windowInstance.Shared.appDbContext.AddParticipant("Participant 1", go2024.ID);
            DbTableModel_Participant participant2 = windowInstance.Shared.appDbContext.AddParticipant("Participant 2", go2024.ID);
            DbTableModel_Participant participant3 = windowInstance.Shared.appDbContext.AddParticipant("Participant 3", go2024.ID);
            DbTableModel_Participant participant4 = windowInstance.Shared.appDbContext.AddParticipant("Participant 4", go2024.ID);
            DbTableModel_Participant participant5 = windowInstance.Shared.appDbContext.AddParticipant("Participant 5", go2024.ID);
            DbTableModel_Participant participant6 = windowInstance.Shared.appDbContext.AddParticipant("Participant 6", go2024.ID);
            DbTableModel_Participant participant7 = windowInstance.Shared.appDbContext.AddParticipant("Participant 7", go2024.ID);
            DbTableModel_Participant participant8 = windowInstance.Shared.appDbContext.AddParticipant("Participant 8", go2024.ID);
            DbTableModel_Participant participant9 = windowInstance.Shared.appDbContext.AddParticipant("Participant 9", go2024.ID);
            DbTableModel_Participant participant10 = windowInstance.Shared.appDbContext.AddParticipant("Participant 10");
            DbTableModel_Participant participant11 = windowInstance.Shared.appDbContext.AddParticipant("Participant 11"); // Unasigned in example
            DbTableModel_Participant participant12 = windowInstance.Shared.appDbContext.AddParticipant("Participant 12"); // Unasigned in example
            DbTableModel_Participant participant13 = windowInstance.Shared.appDbContext.AddParticipant("Participant 13", go2023.ID); // Unasigned in example
            DbTableModel_Participant participant14 = windowInstance.Shared.appDbContext.AddParticipant("Participant 14", go2023.ID); // Unasigned in example

            // Groups

            DbTableModel_Group go2024_group1 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 1",
                "The Adventures Of Lucy Speculum",
                "https://go2024.ntigskovde.se/gr1/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group1.png"
            );
            go2024_group1.AddParticipant(windowInstance.Shared.appDbContext, participant1.ID);

            DbTableModel_Group go2024_group2 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 2",
                "Unchained",
                "https://go2024.ntigskovde.se/gr2/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group2.png"
            );
            go2024_group2.AddParticipant(windowInstance.Shared.appDbContext, participant2.ID);

            DbTableModel_Group go2024_group3 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 3",
                "Flintiga Vampyren",
                "https://go2024.ntigskovde.se/gr3/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group3.png"
            );
            go2024_group3.AddParticipant(windowInstance.Shared.appDbContext, participant3.ID);

            DbTableModel_Group go2024_group4 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 4",
                "Game 4",
                "https://go2024.ntigskovde.se/gr4/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group4.png"
            );
            go2024_group4.AddParticipant(windowInstance.Shared.appDbContext, participant4.ID);

            DbTableModel_Group go2024_group5 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 5",
                "MirrorRhythm",
                "https://go2024.ntigskovde.se/gr5/Meny.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group5.png"
            );
            go2024_group5.AddParticipant(windowInstance.Shared.appDbContext, participant5.ID);
            go2024_group5.AddParticipant(windowInstance.Shared.appDbContext, participant9.ID);

            DbTableModel_Group go2024_group6 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 6",
                "Game 6",
                "https://go2024.ntigskovde.se/gr6/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group6.png"
            );
            go2024_group6.AddParticipant(windowInstance.Shared.appDbContext, participant6.ID);

            DbTableModel_Group go2024_group7 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 7",
                "Parry On",
                "https://go2024.ntigskovde.se/gr7/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group7.png"
            );
            go2024_group7.AddParticipant(windowInstance.Shared.appDbContext, participant7.ID);

            DbTableModel_Group go2024_group8 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 8",
                "Crystal Adventure",
                "https://go2024.ntigskovde.se/gr8/index.html",
                go2024.ID,
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group8.png"
            );
            go2024_group8.AddParticipant(windowInstance.Shared.appDbContext, participant8.ID);
            go2024_group8.AddParticipant(windowInstance.Shared.appDbContext, participant10.ID);

            // Grades
            windowInstance.Shared.appDbContext.AddGrade(
                1,
                "",
                go2024_group1.ID,
                user1_entertainment.ID,
                go2024.GradeType
            );
            windowInstance.Shared.appDbContext.AddGrade(
                1,
                "   ",
                go2024_group2.ID,
                user2_codestrcdocs.ID,
                go2024.GradeType
            );
            windowInstance.Shared.appDbContext.AddGrade(
                2,
                "Comment",
                go2024_group3.ID,
                user1_phymath.ID,
                go2024.GradeType
            );
            windowInstance.Shared.appDbContext.AddGrade(
                3,
                "This is a longer comment because why not",
                go2024_group1.ID,
                user2_codestrcdocs.ID,
                go2024.GradeType
            );
            windowInstance.Shared.appDbContext.AddGrade(
                4,
                "",
                go2024_group4.ID,
                user2_playability.ID,
                go2024.GradeType
            );
            windowInstance.Shared.appDbContext.AddGrade(
                3,
                "",
                go2024_group5.ID,
                user2_theme_keeping.ID,
                go2024.GradeType
            );

            // Feature Flags
            windowInstance.Shared.appDbContext.SetFlag("ff_grade_shows_username", true);
            windowInstance.Shared.appDbContext.SetFlag("ff_hover_for_playbutton", false);
            windowInstance.Shared.appDbContext.SetFlag("ff_grade_comment_after_deadline", false);
        }

        private async void ModeSelectExternal(object sender, RoutedEventArgs e) {
            ModeSelectInfoText.Style = (Style)FindResource("StdTextBlock");
            ModeSelectInfoText.Text = $"Connecting to {SecretConfig.ExternalDbAdress}...";

            // Make text change before continuing
            //Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.ContextIdle, null);

            /*
            // Init the windowInstance.appDbContext with mysql server with details from SecretConfig
            try {
                windowInstance.appDbContext = new AppDbContext(false, $"server={SecretConfig.ExternalDbAdress};user={SecretConfig.ExternalDbUser};password={SecretConfig.ExternalDbPassword};database=gameon_v2");
            }
            catch (Exception ex) {
                ModeSelectInfoText.Style = (Style)FindResource("ErrorTextBlock");
                ModeSelectInfoText.Text = ex.Message;
            }
            */

            // Disable buttons
            ModeSelectLocalBtn.IsEnabled = false;
            ModeSelectExternalBtn.IsEnabled = false;

            await Task.Run(() => {
                try {
                    windowInstance.Shared.appDbContext = new AppDbContext(false, $"server={SecretConfig.ExternalDbAdress};user={SecretConfig.ExternalDbUser};password={SecretConfig.ExternalDbPassword};database={SecretConfig.ExternalDbName}");

                    if (!windowInstance.Shared.appDbContext.IsInited()) {
                        InitDatabaseValues();
                        windowInstance.Shared.appDbContext.MarkAsInited();
                    }

                    windowInstance.Shared.appDbIsInited = true;
                }
                catch (Exception ex) {
                    windowInstance.Shared.appDbIsInited = false;
                    Dispatcher.Invoke(() => {
                        ModeSelectInfoText.Style = (Style)FindResource("ErrorTextBlock");
                        ModeSelectInfoText.Text = ex.Message;
                    });
                }
            });

            if (windowInstance.Shared.appDbIsInited) {
                
                Dispatcher.Invoke(() => {
                    ModeSelectInfoText.Style = (Style)FindResource("StdTextBlock");
                    ModeSelectInfoText.Text = $"Connected to external database at {SecretConfig.ExternalDbAdress}!";
                });

                windowInstance.Shared.appDbIsUsingSqlite = false;
                windowInstance.NavigateTo(new Pages.LoginView(windowInstance, this));
            }

            // Enable buttons
            Dispatcher.Invoke(() => {
                ModeSelectLocalBtn.IsEnabled = true;
                ModeSelectExternalBtn.IsEnabled = true;
            });
        }

        private async void ModeSelectLocal(object sender, RoutedEventArgs e) {
            ModeSelectInfoText.Style = (Style)FindResource("StdTextBlock");
            ModeSelectInfoText.Text = $"Loading local database...";

            // Disable buttons
            ModeSelectLocalBtn.IsEnabled = false;
            ModeSelectExternalBtn.IsEnabled = false;

            await Task.Run(() => {
                // Init the windowInstance.appDbContext with local db file
                try {
                    windowInstance.Shared.appDbContext = new AppDbContext(true, $"Data Source={SecretConfig.LocalDbFile}");

                    if (!windowInstance.Shared.appDbContext.IsInited()) {
                        InitDatabaseValues();
                        windowInstance.Shared.appDbContext.MarkAsInited();
                    }

                    windowInstance.Shared.appDbIsInited = true;
                }
                catch (Exception ex) {
                    windowInstance.Shared.appDbIsInited = false;
                    Dispatcher.Invoke(() => {
                        ModeSelectInfoText.Style = (Style)FindResource("ErrorTextBlock");
                        ModeSelectInfoText.Text = ex.Message + " (If backed-up, remove gameon_v2.db and try again)";
                    });
                }
            });

            if (windowInstance.Shared.appDbIsInited) {
                
                Dispatcher.Invoke(() => {
                    ModeSelectInfoText.Style = (Style)FindResource("StdTextBlock");
                    ModeSelectInfoText.Text = "Connected to local database!";
                });

                windowInstance.Shared.appDbIsUsingSqlite = true;

                windowInstance.NavigateTo(new Pages.LoginView(windowInstance, this));
            }

            // Enable buttons
            Dispatcher.Invoke(() => {
                ModeSelectLocalBtn.IsEnabled = true;
                ModeSelectExternalBtn.IsEnabled = true;
            });
        }
    }
}
