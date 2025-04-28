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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {

        private readonly MainWindow windowInstance;
        private readonly UserControl? sendingView;

        public LoginView(MainWindow WindowInstance, UserControl? SendingView = null) {
            this.windowInstance = WindowInstance;
            this.sendingView = SendingView;

            InitializeComponent();

            LoginViewEmailBox.Text = "";
            LoginViewPasswordBox.Password = "";

            // Focus email field and begin typing
            this.Loaded += (s, e) => {
                LoginViewEmailBox.Focus();
            };

        }

        private void LoginViewDisconnect(object sender, RoutedEventArgs e) {
            windowInstance.Shared.appDbContext.Dispose();

            windowInstance.NavigateTo(new ModeView(windowInstance, this));
        }

        private void LoginViewLogin(object sender, RoutedEventArgs e) {

            // Get values
            string email = LoginViewEmailBox.Text;
            string password = LoginViewPasswordBox.Password;

            LoginViewInfoText.Style = (Style)FindResource("ErrorTextBlock");

            if (string.IsNullOrEmpty(email)) {
                LoginViewInfoText.Text = "Email cannot be empty!";
                return;
            }
            if (string.IsNullOrEmpty(password)) {
                LoginViewInfoText.Text = "Password cannot be empty!";
                return;
            }

            try {
                int? userId = windowInstance.Shared.appDbContext.ValidateUserLogin(email, password);

                if (userId == null) {
                    LoginViewInfoText.Text = "Invalid email or password!" + " (Forgotten password? Contact your administrator)";
                    return;
                }

                DbTableModel_AppUser? user = windowInstance.Shared.appDbContext.GetAppUser(userId.Value);

                if (user == null) {
                    LoginViewInfoText.Text = "Failed to login! (No user-data tied to the id)";
                    return;
                }

                windowInstance.Shared.user = user;

                windowInstance.NavigateTo(new Pages.UserView(windowInstance, this));
            }
            catch (Exception ex) {
                LoginViewInfoText.Text = ex.Message;
                return;
            }
        }

        private void Login_InputField_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                // If sender is email field, focus password field else prompt login
                if (sender == LoginViewEmailBox) {
                    LoginViewPasswordBox.Focus();
                } else {
                    LoginViewLogin(sender, e);
                }
            }
        }
    }
}
