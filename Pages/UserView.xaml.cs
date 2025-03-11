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

        private UITools_GroupContentHolder uITools_GroupContentHolder;

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

            // Setup UI tools
            uITools_GroupContentHolder = new UITools_GroupContentHolder(GroupContentHoldingWrapper, NoSelectedGroupText, GroupContentHolder);

            // Update texts
            DbTableModel_Edition? activeEdition = windowInstance.Shared.appDbContext.GetActiveEdition();
            if (activeEdition == null) {
                EditionTitle.Text = "No active editions at the moment!";
                GroupSidebarText.Text = "No active edition.";
                NoSelectedGroupText.Text = "No active editions at the moment, please come back later.";
                return;
            }

            EditionTitle.Text = $"{activeEdition.Name}";
            if (activeEdition.Theme != "") {
                EditionTitle.Text += $" (Theme: {activeEdition.Theme})";
            }

            List<DbTableModel_Group> groups = windowInstance.Shared.appDbContext.GetGroups();
            if (groups.Count == 0) {
                GroupSidebarText.Text = "No groups found.";
                NoSelectedGroupText.Text = $"No groups found for {activeEdition.Name}, please come back later.";
                return;
            } else {
                GroupSidebarText.Text = "Select group:";
                NoSelectedGroupText.Text = "Click a group on the side to view it.";
            }

            // Get groups and instantiate buttons under sidebar
            //// Clear sidebar
            GroupSidebar.Children.Clear();

            //// Generate buttons
            foreach (DbTableModel_Group group in groups) {
                Button groupBtn = new Button {
                    Content = group.Name,
                    Tag = group.ID,
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 2.5, 3)
                };
                groupBtn.Click += GroupButtonClick;
                GroupSidebar.Children.Add(groupBtn);
            }
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

        private void GroupButtonClick(object sender, RoutedEventArgs e) {
            // Get group by id
            Button btn = (Button)sender;
            int groupId = (int)btn.Tag;
            DbTableModel_Group? group = windowInstance.Shared.appDbContext.GetGroup(groupId);
            if (group == null) { return; }

            // Set main content to 
            uITools_GroupContentHolder.SetContent(
                new Pages.Parts.UserViewGroup(windowInstance, group)
            );
        }
    }
}
