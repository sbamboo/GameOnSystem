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

namespace GameOnSystem {
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl {

        private readonly MainWindow WindowInstance;

        private List<ResolvedGroup> Groups;

        public UserControl? SendingView = null;

        public UserView(MainWindow WindowInstance, UserControl? sendingView = null) {
            this.SendingView = sendingView;
            this.WindowInstance = WindowInstance;
            this.WindowInstance.Shared.userView = this;

            InitializeComponent();

            GenerateUserView();
        }

        public void GenerateUserView() {
            if (this.WindowInstance.Shared.user.IsAdmin == true) {
                ChangeToAdminView.Visibility = Visibility.Visible;
            }

            List<Edition>? editions = this.WindowInstance.DbContext.GetActiveEdtions();
            if (editions != null) {
                // If length more then 0 get first one
                if (editions.Count > 0) {
                    Edition edition = editions[0];
                    TitleTextBlock.Text = $"{edition.Name} (Tema: {edition.Theme})";
                    if (GroupClickMessage != null) {
                        GroupClickMessage.Text = "Click a group on the side to view it.";
                    }

                    this.Groups = this.WindowInstance.DbContext.GetResolvedGroupsForEdition(edition.ID);
                } else {
                    this.Groups = new List<ResolvedGroup>();
                    TitleTextBlock.Text = "Inga aktiva utgåvor";
                    GroupSelectionTitle.Text = "";
                    if (GroupClickMessage != null) {
                        GroupClickMessage.Text = "Inga aktiva utgåvor, kontakta en administratör.";
                    }
                }
            }

            GenerateGroupButtons();
        }

        private void UpdateGroups() {
            this.Groups = this.WindowInstance.DbContext.GetResolvedGroups();
        }

        private void UserView_Logout_Click(object sender, RoutedEventArgs e) {
            // The login view shall be reset on logout, thus we instantiate a new LoginView rathern then using senderView
            this.WindowInstance.Shared.user = null;
            this.WindowInstance.NavigateTo(new LoginView(this.WindowInstance));
        }

        private void GenerateGroupButtons() {
            GroupSelectionsWrapper.Children.Clear();
            foreach (var group in this.Groups) {
                // Create button
                Button button = new Button {
                    Content = group.Name,
                    Tag = group.ID,        // Store Id
                    FontSize = 15,
                    Height = 25,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                button.Click += GroupButton_Click;

                // Add the button to the wrapper
                GroupSelectionsWrapper.Children.Add(button);
            }
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e) {
            this.UpdateGroups();
            // Retrieve the button that was clicked
            if (sender is Button clickedButton && clickedButton.Tag is int clickedButtonTag) {
                ResolvedGroup? group = this.Groups.FirstOrDefault(g => g.ID == clickedButtonTag);
                
                if (group != null) {
                    //TitleTextBlock.Text = $"Clicked {group.Name} (tag={clickedButtonTag}; id={group.Id})";
                    PerGroupContentHolder.Children.Clear();

                    // Create Wrapper
                    ContentControl contentControl = new ContentControl {
                        Content = "Loading...",
                        FontSize = 20,
                        Margin = new Thickness(10)
                    };

                    // Instantiate UserView_Group
                    UserView_Group userViewGroup = new UserView_Group(this.WindowInstance, this, group);
                    
                    // Add
                    contentControl.Content = userViewGroup;
                    PerGroupContentHolder.Children.Add(contentControl);
                }
            }
        }

        private void ChangeToAdminView_Click(object sender, RoutedEventArgs e) {
            if (this.WindowInstance.Shared.user.IsAdmin == true) {
                // If the sendinvView is AdminView, then we want to go back to that view, else instantiate a new AdminView
                if (this.SendingView != null && this.SendingView is AdminView) {
                    AdminView sendingView = (AdminView)this.SendingView;
                    sendingView.SendingView = this;
                    this.WindowInstance.NavigateTo(sendingView);
                } else {
                    this.WindowInstance.NavigateTo(new AdminView(this.WindowInstance, this));
                }
            }
        }
    }
}
