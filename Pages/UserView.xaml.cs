using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl {

        private readonly MainWindow windowInstance;
        private readonly UserControl? sendingView;

        public UserView(MainWindow WindowInstance, UserControl? SendingView = null) {
            this.windowInstance = WindowInstance;
            this.sendingView = SendingView;

            if (windowInstance.Shared.appDbIsUsingSqlite) {
                windowInstance.Title = windowInstance.Shared.coreTitle + " | UserView | Local database";
            } else {
                windowInstance.Title = windowInstance.Shared.coreTitle + $" | UserView | Connected to {SecretConfig.ExternalDbAdress}";
            }

            InitializeComponent();

            if (windowInstance.Shared.user == null) {
                if (sendingView != null && sendingView is LoginView) {
                    windowInstance.NavigateTo(sendingView);
                } else {
                    windowInstance.NavigateTo(new LoginView(windowInstance, this));
                }
            }

            if (windowInstance.Shared.user.IsAdmin) {
                AdminViewBtn.Visibility = Visibility.Visible;
            }

            // Update texts
            DbTableModel_Edition? activeEdition = windowInstance.Shared.appDbContext.GetActiveEdition();
            if (activeEdition == null) {
                EditionTitle.Text = "No active editions at the moment!";
                GroupClickMessage.Text = "No active editions at the moment, please come back later.";
                return;
            }

            DbTableTool_Edition activeEditionTool = new DbTableTool_Edition(windowInstance.Shared.appDbContext, activeEdition);

            EditionTitle.Text = $"{activeEdition.Name}";
            if (activeEdition.Theme != "") {
                EditionTitle.Text += $" (Theme: {activeEdition.Theme})";
            }

            int groupCount = activeEditionTool.GetGroupCount();
            if (groupCount == 0) {
                GroupSelectionTitle.Text = "No groups found.";
                GroupSelectionTitle.Text = $"No groups found for {activeEdition.Name}, please come back later.";
                return;
            } else {
                GroupSelectionTitle.Text = "Select group";
            }

            // Get groups
            List<DbTableTool_Group> groups = activeEditionTool.GetGroupsAsTools();
        }

        private void AdminViewBtnClick(object sender, RoutedEventArgs e) {
            if (sendingView != null && sendingView is AdminView) {
                windowInstance.NavigateTo(sendingView);
            } else {
                windowInstance.NavigateTo(new AdminView(windowInstance, this));
            }
        }

        private void LogoutBtnClick(object sender, RoutedEventArgs e) {
            windowInstance.Shared.user = null;
            if (sendingView != null && sendingView is LoginView) {
                windowInstance.NavigateTo(sendingView);
            } else {
                windowInstance.NavigateTo(new LoginView(windowInstance, this));
            }
        }
    }
}
