using Microsoft.Extensions.Options;
using System.ComponentModel;
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

namespace GameOnSystem
{

    class Shared {
        public User user;
        public UserView? userView;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        internal Shared Shared = new Shared();

        internal AppDbContext DbContext = new AppDbContext();

        public MainWindow() {
            // Create a few groups with al propeties filled out
            // public int AddGroup(string Name, string GameName, string GameUrl, string GameBannerUrl, List<string> Members, Dictionary<string, int> Grades) {}
            // Grades are 0-6
            // Categories are:
            /*
            - "Underhållning"
            - "Uppfyllande av tema"
            - "Grafikens sammanhållning"
            - "Grafikens koppling till temat"
            - "Programmeringskodens struktur och dokumentation"
            - "Implementering av fysik / matematik"
            - "Spelbarhet"
            - "Kommentar"
            */

            if (DbContext.IsInited() != true) {
                int? edition_id = DbContext.AddEdition("GameOn 2024", "Reflections", true);
                if (edition_id != null) {

                    int entertainment = DbContext.AddCategory("Underhållning","int_1-6") ?? -1;
                    int theme_keeping = DbContext.AddCategory("Uppfyllande av tema", "int_1-6") ?? -1;
                    int graphics_coherence = DbContext.AddCategory("Grafikens sammanhållning", "int_1-6") ?? -1;
                    int graphics_theme_keeping = DbContext.AddCategory("Grafikens koppling till temat", "int_1-6") ?? -1;
                    int code_structure_and_documentation = DbContext.AddCategory("Programmeringskodens struktur och dokumentation", "int_1-6") ?? -1;
                    int physics_mathematics_implementation = DbContext.AddCategory("Implementering av fysik / matematik", "int_1-6") ?? -1;
                    int playability = DbContext.AddCategory("Spelbarhet", "int_1-6") ?? -1;
                    int comment = DbContext.AddCategory("Kommentar", "string") ?? -1;

                    // TODO: Maybe the assemble-dict should use id's for categories instead of names, to avoid duplicates, test if current makes duplicates?

                    DbContext.AddUser("User1", "user1@example.com", "user1", new List<int> { entertainment, physics_mathematics_implementation });
                    DbContext.AddUser("User2", "user2@example.com", "user2", new List<int> { code_structure_and_documentation, comment });

                    DbContext.AssembleGroup(
                        "Group 1",
                        "Game 1",
                        "https://go2024.ntigskovde.se/gr1/index.html",
                        "https://picsum.photos/seed/go2024_gr1/1280/720.jpg",
                        edition_id ?? -1,
                        new List<string> { "User 1", "User 2" },
                        new Dictionary<string, Dictionary<string, string>> {
                            {
                                "int_1-6:Underhållning", new Dictionary<string, string> {
                                    { "User1", "1" }
                                }
                            }, {
                                "int_1-6:Implementering av fysik / matematik", new Dictionary<string, string> {
                                    { "User1", "5" }
                                }
                            }, {
                                "int_1-6:Programmeringskodens struktur och dokumentation", new Dictionary<string, string> {
                                    { "User2", "5" }
                                }
                            }, {
                                "string:Kommentar", new Dictionary<string, string> {
                                    { "User2", "5" }
                                }
                            }
                        }
                    );
                    DbContext.AssembleGroup(
                        "Group 2",
                        "Game 2",
                        "https://go2024.ntigskovde.se/gr2/index.html",
                        "https://picsum.photos/seed/go2024_gr2/1280/720.jpg",
                        edition_id ?? -1,
                        new List<string> { "User 3", "User 4" },
                        new Dictionary<string, Dictionary<string, string>> {
                            {
                                "int_1-6:Underhållning", new Dictionary<string, string> {
                                    { "User1", "6" }
                                }
                            }, {
                                "int_1-6:Implementering av fysik / matematik", new Dictionary<string, string> {
                                    { "User1", "4" }
                                }
                            }, {
                                "int_1-6:Programmeringskodens struktur och dokumentation", new Dictionary<string, string> {
                                    { "User2", "6" }
                                }
                            }, {
                                "string:Kommentar", new Dictionary<string, string> {
                                    { "User2", "4" }
                                }
                            }
                        }
                    );
                    DbContext.AssembleGroup(
                        "Group 3",
                        "Game 3",
                        "https://go2024.ntigskovde.se/gr3/index.html",
                        "https://picsum.photos/seed/go2024_gr3/1280/720.jpg",
                        edition_id ?? -1,
                        new List<string> { "User 5", "User 6" },
                        new Dictionary<string, Dictionary<string, string>> {
                            {
                                "int_1-6:Underhållning", new Dictionary<string, string> {
                                    { "User1", "2" }
                                }
                            }, {
                                "int_1-6:Implementering av fysik / matematik", new Dictionary<string, string> {
                                    { "User1", "4" }
                                }
                            }, {
                                "int_1-6:Programmeringskodens struktur och dokumentation", new Dictionary<string, string> {
                                    { "User2", "2" }
                                }
                            }, {
                                "string:Kommentar", new Dictionary<string, string> {
                                    { "User2", "4" }
                                }
                            }
                        }
                    );
                    DbContext.AssembleGroup(
                        "Group 4",
                        "Game 4",
                        "https://go2024.ntigskovde.se/gr4/index.html",
                        "https://picsum.photos/seed/go2024_gr4/1280/720.jpg",
                        edition_id ?? -1,
                        new List<string> { "User 7", "User 8" },
                        new Dictionary<string, Dictionary<string, string>> {
                            {
                                "int_1-6:Underhållning", new Dictionary<string, string> {
                                    { "User1", "4" }
                                }
                            }, {
                                "int_1-6:Implementering av fysik / matematik", new Dictionary<string, string> {
                                    { "User1", "2" }
                                }
                            }, {
                                "int_1-6:Programmeringskodens struktur och dokumentation", new Dictionary<string, string> {
                                    { "User2", "4" }
                                }
                            }, {
                                "string:Kommentar", new Dictionary<string, string> {
                                    { "User2", "2" }
                                }
                            }
                        }
                    );
                }

                DbContext.MarkAsInited();
            }

            InitializeComponent();

            MainContent.Content = new LoginView(this);
        }

        public void NavigateTo(object view) {
            MainContent.Content = view;
        }
    }
}