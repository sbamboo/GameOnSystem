using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl {

        private readonly MainWindow windowInstance;
        private readonly UserControl? sendingView;
        internal UITools_GroupContentHolder uITools_AdminContentHolder;

        public AdminView(MainWindow WindowInstance, UserControl? SendingView = null) {
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

            // Setup UI tools
            uITools_AdminContentHolder = new UITools_GroupContentHolder(AdminContentHolderWrapper, NoSelectedContentText, AdminContentHolder);

            // Update texts
            if (windowInstance.Shared.appDbContext.IsInited()) {
                AdminSideBarTitle.Text = "Select Table";
                NoSelectedContentText.Text = "Click a table in the sidebar to view/edit it.";
            } else {
                AdminSideBarTitle.Text = "Database not inited!";
                NoSelectedContentText.Text = "Database not inited, restart the program and try again!";
            }

            // Add table buttons to the sidebar
            Button optionsButton = new Button() {
                Content = "Options", Tag="Options",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            optionsButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(optionsButton);

            Button appUsersButton = new Button() {
                Content = "AppUsers", Tag="AppAsers",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            appUsersButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(appUsersButton);

            Button editionsButton = new Button() {
                Content = "Editions", Tag = "Editions",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            editionsButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(editionsButton);

            Button categoriesButton = new Button() {
                Content = "Categories", Tag = "Categories",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            categoriesButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(categoriesButton);

            Button groupsButton = new Button() {
                Content = "Groups", Tag = "Groups",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            groupsButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(groupsButton);

            Button participantsButton = new Button() {
                Content = "Participants", Tag = "Participants",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            participantsButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(participantsButton);

            Button gradesButton = new Button() {
                Content = "Grades", Tag = "Grades",
                Margin = new Thickness(10, 5, 10, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            };
            gradesButton.Click += TabelSelectClick;
            AdminSidebar.Children.Add(gradesButton);
        }

        private void UserViewButtonClick(object sender, RoutedEventArgs e) {
            if (sendingView != null && sendingView is UserView) {
                windowInstance.NavigateTo(sendingView);
            } else {
                windowInstance.NavigateTo(new UserView(windowInstance, this));
            }
        }

        private void TabelSelectClick(object sender, RoutedEventArgs e) {

            UserControl? newView = null;

            if (sender is Button) {
                switch (((Button)sender).Tag) {
                    case "Options":
                        newView = new Pages.Parts.Admin.Options(windowInstance, this);
                        break;

                    case "Participants":
                        newView = new Pages.Parts.Admin.Participants(windowInstance, this);
                        break;
                }
            }

            if (newView != null) {
                uITools_AdminContentHolder.SetContent(newView);
            }
        }
    }
}
