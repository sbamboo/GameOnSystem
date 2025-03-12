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
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sendingView != null && sendingView is UserView) {
                windowInstance.NavigateTo(sendingView);
            } else {
                windowInstance.NavigateTo(new UserView(windowInstance, this));
            }
        }
    }
}
