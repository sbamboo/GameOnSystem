using System;
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

            windowInstance.Title = windowInstance.Shared.coreTitle + " | Select Mode";

            InitializeComponent();
        }

        private void InitDatabaseValues() {
            DbTableModel_Edition go2024 = windowInstance.Shared.appDbContext.AddEdition(
                "GameOn 2024",            // Name
                "Reflections",            // Theme
                1,                        // GradeMin
                6,                        // GradeMax
                1,                        // GradeType
                false,                    // IsActive
                new DateTime(2025, 3, 12) // GradingDeadline
            );

            DbTableModel_Category entertainment = windowInstance.Shared.appDbContext.AddCategory("Underhållning");
            DbTableModel_Category theme_keeping = windowInstance.Shared.appDbContext.AddCategory("Uppfyllande av tema");
            DbTableModel_Category graphics_coherence = windowInstance.Shared.appDbContext.AddCategory("Grafikens sammanhållning");
            DbTableModel_Category graphics_theme_keeping = windowInstance.Shared.appDbContext.AddCategory("Grafikens koppling till temat");
            DbTableModel_Category code_structure_and_docs = windowInstance.Shared.appDbContext.AddCategory("Programmeringskodens struktur och dokumentation");
            DbTableModel_Category physics_math_implementation = windowInstance.Shared.appDbContext.AddCategory("Implementering av fysik / matematik");
            DbTableModel_Category playability = windowInstance.Shared.appDbContext.AddCategory("Spelbarhet");

            DbTableModel_AppUser user1 = windowInstance.Shared.appDbContext.AddAppUser("User1", "user1@example.com", "user1", false);
            user1.AddFocusCategory(windowInstance.Shared.appDbContext, entertainment.ID);
            user1.AddFocusCategory(windowInstance.Shared.appDbContext, physics_math_implementation.ID);
            DbTableModel_AppUser user2 = windowInstance.Shared.appDbContext.AddAppUser("User2", "user2@example.com", "user2", false);
            user2.AddFocusCategory(windowInstance.Shared.appDbContext, code_structure_and_docs.ID);
            user2.AddFocusCategory(windowInstance.Shared.appDbContext, playability.ID);

            DbTableModel_Group go2024_group1 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 1",
                "The Adventures Of Lucy Speculum",
                "https://go2024.ntigskovde.se/gr1/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group1.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group2 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp p2",
                "Unchained",
                "https://go2024.ntigskovde.se/gr2/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group2.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group3 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 3",
                "Flintiga Vampyren",
                "https://go2024.ntigskovde.se/gr3/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group3.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group4 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 4",
                "Game 4",
                "https://go2024.ntigskovde.se/gr4/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group4.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group5 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 5",
                "MirrorRhythm",
                "https://go2024.ntigskovde.se/gr5/Meny.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group5.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group6 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 6",
                "Game 6",
                "https://go2024.ntigskovde.se/gr6/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group6.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group7 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 7",
                "Parry On",
                "https://go2024.ntigskovde.se/gr7/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group7.png",
                go2024.ID
            );

            DbTableModel_Group go2024_group8 = windowInstance.Shared.appDbContext.AddGroup(
                "Grupp 8",
                "Crystal Adventure",
                "https://go2024.ntigskovde.se/gr8/index.html",
                "https://raw.githubusercontent.com/sbamboo/GameOnSystem/refs/heads/main/Assets/2024_group8.png",
                go2024.ID
            );
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
                    windowInstance.Shared.appDbContext = new AppDbContext(false, $"server={SecretConfig.ExternalDbAdress};user={SecretConfig.ExternalDbUser};password={SecretConfig.ExternalDbPassword};database=gameon_v2");

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
                    windowInstance.Shared.appDbContext = new AppDbContext(true, "Data Source=gameon_v2.db");

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

                // DEBUG
                windowInstance.Shared.user = windowInstance.Shared.appDbContext.GetAppUser(1);
                windowInstance.NavigateTo(new Pages.UserView(windowInstance, this));
                //DEBUG

                //windowInstance.NavigateTo(new Pages.LoginView(windowInstance, this));
            }

            // Enable buttons
            Dispatcher.Invoke(() => {
                ModeSelectLocalBtn.IsEnabled = true;
                ModeSelectExternalBtn.IsEnabled = true;
            });
        }
    }
}
