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

    internal class EditionAdminEditor {
        private MainWindow WindowInstance;
        private Edition Edition;
        private StackPanel Parent;
        private StackPanel? UiElementIntance = null;

        public EditionAdminEditor(MainWindow WindowInstance, Edition edition, StackPanel parent) {
            this.WindowInstance = WindowInstance;
            this.Edition = edition;
            this.Parent = parent;
        }

        private void UpdateData() {
            this.Edition = WindowInstance.DbContext.GetEdition(this.Edition.ID);
        }

        public void Instantiate() {
            List<object> tags = new List<object>() { Edition, this.Edition.ID };

            //StackPanel: ID |Name| |Theme| [Save] [Delete]
            StackPanel stackPanel = new StackPanel() {
                Name = $"edition_{this.Edition.ID}",
                Orientation = Orientation.Horizontal,
                Tag = tags,
            };

            TextBlock idTextBlock = new TextBlock() {
                Text = this.Edition.ID.ToString(),
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "ID"
            };

            TextBox nameTextBox = new TextBox() {
                Text = this.Edition.Name,
                Width = 200,
                Tag = "Theme"
            };

            TextBox themeTextBox = new TextBox() {
                Text = this.Edition.Theme,
                Width = 200,
                Tag = "Theme"
            };

            Button saveButton = new Button() {
                Content = "Save",
                Tag = tags,
            };
            saveButton.Click += this.Save;

            Button refreshButton = new Button() {
                Content = "Refresh",
                Tag = tags,
            };
            refreshButton.Click += this.Refresh;

            Button deleteButton = new Button() {
                Content = "Delete",
                Tag = tags,
            };
            deleteButton.Click += this.Delete;

            stackPanel.Children.Add(idTextBlock);
            stackPanel.Children.Add(nameTextBox);
            stackPanel.Children.Add(themeTextBox);
            stackPanel.Children.Add(saveButton);
            stackPanel.Children.Add(refreshButton);
            stackPanel.Children.Add(deleteButton);

            this.UiElementIntance = stackPanel;

            this.Parent.Children.Add(stackPanel);
        }

        private void Refresh(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((StackPanel)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Edition && senderId == this.Edition.ID) {
                this.UpdateData();
                // Get parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Update the textboxes
                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "ID") {
                            textBox.Text = this.Edition.ID.ToString();
                        } else if ((string)textBox.Tag == "Name") {
                            textBox.Text = this.Edition.Name;
                        } else if ((string)textBox.Tag == "Theme") {
                            textBox.Text = this.Edition.Theme;
                        }
                    }
                }
            }
        }
        private void Save(object sender, RoutedEventArgs e) { }
        private void Delete(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((StackPanel)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Edition && senderId == this.Edition.ID) {
                this.WindowInstance.DbContext.RemoveEdition(this.Edition.ID);

                // Remove the UI element
                if (this.UiElementIntance != null) {
                    this.Parent.Children.Remove(this.UiElementIntance);
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl {

        private readonly MainWindow WindowInstance;

        public UserControl? SendingView = null;
        public AdminView(MainWindow WindowInstance, UserControl? sendingView = null) {
            this.SendingView = sendingView;
            this.WindowInstance = WindowInstance;
            InitializeComponent();

            UpdateAdminView();
        }

        private void ChangeToUserView_Click(object sender, RoutedEventArgs e) {
            // If the sendinvView is UserView, then we want to go back to that view, else instantiate a new UserView
            if (this.SendingView != null && this.SendingView is UserView) {
                UserView sendingView = (UserView)this.SendingView;
                sendingView.SendingView = this;
                this.WindowInstance.NavigateTo(sendingView);
            } else {
                this.WindowInstance.NavigateTo(new UserView(this.WindowInstance, this));
            }
        }

        private void UpdateAdminView() {
            // Get entries
            List<Edition> editions = WindowInstance.DbContext.GetEditions();
            List<Category> categories = WindowInstance.DbContext.GetCategories();
            List<User> users = WindowInstance.DbContext.GetUsers();
            List<ResolvedGroup> groups = WindowInstance.DbContext.GetResolvedGroups();

            // Instantiate under editions editable fields
            StackPanel editionsStack = new StackPanel() {
                Orientation = Orientation.Vertical,
            };
            

            AdminSegment_Edition.Content = editionsStack;

            foreach (var edition in editions) {
                EditionAdminEditor editionAdminEditor = new EditionAdminEditor(WindowInstance, edition, editionsStack);
                editionAdminEditor.Instantiate();
            }
        }
    }
}