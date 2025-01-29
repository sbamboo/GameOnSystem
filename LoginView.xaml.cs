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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {

        private readonly MainWindow WindowInstance;

        public UserControl? SendingView = null;

        public LoginView(MainWindow WindowInstance, UserControl? sendingView = null) {
            this.SendingView = sendingView;
            this.WindowInstance = WindowInstance;
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            string emailValue = Email_InputField.Text;
            string passwordValue = Password_InputField.Password;
            int? id = WindowInstance.DbContext.ValidateUserLogin(emailValue, passwordValue);

            if (id != null) {
                User? user = WindowInstance.DbContext.GetUser((int)id);
                if (user != null) {
                    WindowInstance.Shared.user = user;

                    WindowInstance.NavigateTo(new UserView(WindowInstance));
                }
            } else {
                ResultInfo.Text = "Invalid email or password";
            }
        }

        private void Login_InputField_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                LoginButton_Click(sender, e);
            }
        }
    }
}
