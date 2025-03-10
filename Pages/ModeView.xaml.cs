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
