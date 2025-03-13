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

            // Update texts
            if (windowInstance.Shared.appDbContext.IsInited()) {
                AdminSideBarTitle.Text = "Select Table";
                NoSelectedContentText.Text = "Click a table in the sidebar to view/edit it.";
            } else {
                AdminSideBarTitle.Text = "Database not inited!";
                NoSelectedContentText.Text = "Database not inited, restart the program and try again!";
            }

            // Add table buttons to the sidebar
            AdminSidebar.Children.Add(new Button() {
                Content = "Options", Tag=windowInstance.Shared.appDbContext.Options,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
            AdminSidebar.Children.Add(new Button() {
                Content = "AppUsers", Tag=windowInstance.Shared.appDbContext.AppUsers,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
            AdminSidebar.Children.Add(new Button() {
                Content = "Editions", Tag = windowInstance.Shared.appDbContext.Editions,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
            AdminSidebar.Children.Add(new Button() {
                Content = "Categories", Tag = windowInstance.Shared.appDbContext.Categories,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
            AdminSidebar.Children.Add(new Button() {
                Content = "Groups", Tag = windowInstance.Shared.appDbContext.Groups,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
            AdminSidebar.Children.Add(new Button() {
                Content = "Participants", Tag = windowInstance.Shared.appDbContext.Participants,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
            AdminSidebar.Children.Add(new Button() {
                Content = "Grades", Tag = windowInstance.Shared.appDbContext.Grades,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(0, 5, 0, 5),
                FontSize = 16
            });
        }

        private void UserViewButtonClick(object sender, RoutedEventArgs e) {
            if (sendingView != null && sendingView is UserView) {
                windowInstance.NavigateTo(sendingView);
            } else {
                windowInstance.NavigateTo(new UserView(windowInstance, this));
            }
        }
    }
}
