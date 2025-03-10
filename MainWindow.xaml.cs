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

            MainContent.Content = new Pages.ModeSelect(this);
        }

        public void NavigateTo(UserControl view) {
            MainContent.Content = view;
        }
    }
}