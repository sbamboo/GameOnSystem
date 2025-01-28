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
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl {

        private readonly MainWindow WindowInstance;

        public UserControl? SendingView = null;
        public AdminView(MainWindow WindowInstance, UserControl? sendingView = null) {
            this.SendingView = sendingView;
            this.WindowInstance = WindowInstance;
            InitializeComponent();
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
    }
}
