using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using GameOnSystem.Pages;

namespace GameOnSystem {

    internal class Shared {
        public string coreTitle = "GameOnSystem 0.0-alpha2";
        public AppDbContext appDbContext;
        public bool appDbIsInited = false;
        public bool appDbIsUsingSqlite;
        public DbTableModel_AppUser? user = null;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        internal Shared Shared = new Shared();

        public MainWindow() {
            this.Title = this.Shared.coreTitle;

            InitializeComponent();

            NavigateTo(new ModeView(this, null));
        }

        public void NavigateTo(UserControl view) {

            if (view.GetType().Name == "ModeView") {
                Title = Shared.coreTitle;
            } else {
                if (Shared.appDbIsUsingSqlite) {
                    Title = Shared.coreTitle + " | Local database";
                } else {
                    Title = Shared.coreTitle + $" | Connected to {SecretConfig.ExternalDbAdress}";
                }
            }
            
            switch (view.GetType().Name) {
                case "ModeView":
                    Title += " | Mode select";
                    break;
                case "LoginView":
                    Title += " | Login";
                    break;
                case "UserView":
                    Title += " | Grading View";
                    break;
                case "AdminView":
                    Title += " | Admin View";
                    break;
                default:
                    break;
            }

            MainContent.Content = view;
        }
    }
}