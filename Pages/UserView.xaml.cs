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

            List<DbTableModel_Edition> activeEditions = windowInstance.Shared.appDbContext.GetActiveEditions();

            if (activeEditions.Count == 0) {
                EditionTitle.Text = "No active editions at the moment!";
                GroupSidebarText.Text = "No active edition.";
                NoSelectedGroupText.Text = "No active editions at the moment, please come back later.";
                EditionTitle.Visibility = Visibility.Visible;
                EditionSelector.Visibility = Visibility.Collapsed;
                return;
            }

            if (activeEditions.Count == 1) {
                LoadEdition(activeEditions[0]);
                EditionTitle.Visibility = Visibility.Visible;
                EditionSelector.Visibility = Visibility.Collapsed;
            } else {
                EditionTitle.Visibility = Visibility.Collapsed;
                EditionSelector.Visibility = Visibility.Visible;

                EditionSelector.Items.Clear();
                foreach (var edition in activeEditions) {
                    EditionSelector.Items.Add(new ComboBoxItem {
                        Content = $"{edition.Name} - {edition.Theme}",
                        Tag = edition
                    });
                }

                EditionSelector.SelectedIndex = activeEditions.Count - 1;
            }
        }

        private void LoadEdition(DbTableModel_Edition activeEdition) {
            // Reset UI
            GroupSidebar.Children.Clear();
            if (uITools_GroupContentHolder != null) {
                uITools_GroupContentHolder.ClearContent();
            }
            NoSelectedGroupText.Visibility = Visibility.Visible;
            // Remove uITools_GroupContentHolder instance
            uITools_GroupContentHolder = null;

            EditionTitle.Text = $"{activeEdition.Name} - {activeEdition.Theme}";

            // Setup UI tools
            uITools_GroupContentHolder = new UITools_GroupContentHolder(GroupContentHoldingWrapper, NoSelectedGroupText, GroupContentHolder);

            EditionTitle.Text = $"{activeEdition.Name}";
            if (activeEdition.Theme != "") {
                EditionTitle.Text += $" (Theme: {activeEdition.Theme})";
            }

            bool isDeadlineAvailable = true;
            if (activeEdition.GradingDeadline != null && DateTime.Now > activeEdition.GradingDeadline) {
                isDeadlineAvailable = false;
            }

            int? gildedGroupId = null;
            if (!isDeadlineAvailable) {
                var topGroup = activeEdition.GetGroupWithMaxGrade(windowInstance.Shared.appDbContext);
                if (topGroup != null) {
                    gildedGroupId = topGroup.ID;
                }
            }

            List<DbTableModel_Group> groups = activeEdition.GetGroups(windowInstance.Shared.appDbContext);
            if (groups.Count == 0) {
                GroupSidebarText.Text = "No groups found.";
                NoSelectedGroupText.Text = $"No groups found for {activeEdition.Name}, please come back later.";
                return;
            } else {
                GroupSidebarText.Text = "Select group:";
                NoSelectedGroupText.Text = "Click a group in the sidebar to view it.";
            }

            // Get groups and instantiate buttons under sidebar
            //// Clear sidebar
            GroupSidebar.Children.Clear();

            //// Generate buttons
            foreach (DbTableModel_Group group in groups) {

                // set var for isWinner
                bool isWinner = false;
                if (gildedGroupId.HasValue && group.ID == gildedGroupId.Value) {
                    isWinner = true;
                }

                Button groupBtn = new Button {
                    Content = group.Name,
                    Tag = (group.ID, isWinner),
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 2.5, 3)
                };

                if (isWinner) {
                    groupBtn.Style = (Style)FindResource("WinnerGroupButton");
                    groupBtn.Content = new TextBlock { Text = group.Name, Style = (Style)FindResource("GoldTextBlock") };
                }

                groupBtn.Click += GroupButtonClick;
                GroupSidebar.Children.Add(groupBtn);
            }
        }
        private void EditionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (EditionSelector.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is DbTableModel_Edition selectedEdition) {
                LoadEdition(selectedEdition);
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
            (int groupId, bool isWinner) = ((int, bool))btn.Tag;
            DbTableModel_Group? group = windowInstance.Shared.appDbContext.GetGroup(groupId);
            if (group == null) { return; }

            // Set main content to 
            uITools_GroupContentHolder.SetContent(
                new Pages.Parts.UserViewGroup(windowInstance, group, isWinner)
            );
        }
    }
}
