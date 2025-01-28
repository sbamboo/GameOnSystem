using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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
using static System.Net.Mime.MediaTypeNames;

namespace GameOnSystem {

    /// <summary>
    /// Interaction logic for UserView_Group.xaml
    /// </summary>
    public partial class UserView_Group : UserControl {
        static void OpenUrl(string url) {
            try {
                ProcessStartInfo processInfo = new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true // default browser
                };
                Process.Start(processInfo);
            } catch (Exception ex) {
                ;
            }
        }

        internal ResolvedGroup? resolvedGroup;

        internal Border? totalGradeGolder = null;

        internal MainWindow WindowInstance;

        public UserView_Group(MainWindow WindowInstance, UserControl? sendingView = null, object group = null) {
            this.WindowInstance = WindowInstance;
            this.resolvedGroup = (ResolvedGroup)group;
            InitializeComponent();


            // GENERATE GROUP/GAME CONTENT
            GroupTitle.Text = $"{this.resolvedGroup.GameName} ({resolvedGroup.Name})";

            if (resolvedGroup.GameBannerUrl != null) {
                // Add image element to GroupGamePresentWrapper
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(resolvedGroup.GameBannerUrl);
                bitmap.DecodePixelWidth = 300;
                bitmap.DecodePixelHeight = 160;
                bitmap.EndInit();

                GroupGamePresentWrapper.Content = new System.Windows.Controls.Image {
                    Source = bitmap,
                    Width = 300,
                    Height = 169
                };
            }

            UpdateGroupGradesView();
        }

        private void UpdateGroupGradesView() {
            if (this.resolvedGroup == null) {
                return;
            }

            // Iterate resolvedGroup.Grades and for each place elements under GroupGameGradesWrapper
            GroupGameGradesWrapper.Children.Clear();
            foreach (var grade in resolvedGroup.Grades) {
                // Create a new Border
                var border = new Border {
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#606060")),
                    BorderThickness = new Thickness(0.5),
                    Margin = new Thickness(0,5,5,5),
                    CornerRadius = new CornerRadius(18),
                    Padding = new Thickness(15, 10, 15, 10),
                    Height = 65,
                    Width = 300
                };
                // Inside the border, create a StackPanel vertical
                var stackPanel = new StackPanel {
                    Orientation = Orientation.Vertical
                };
                // Inside the above stackpanel, create a StackPanel horizontal
                var innerStackPanel = new StackPanel {
                    Orientation = Orientation.Horizontal
                };
                // Add a TextBlock with the grade.Category.Name
                TextBlock categoryTextBlock;
                TextBlock catValueInfoTextBlock;
                System.Windows.UIElement gradeTextBlock;
                if (grade.User.Id == WindowInstance.Shared.user.Id) {
                    categoryTextBlock = new TextBlock {
                        Text = grade.Category.Name + ":",
                        FontSize = 15,
                        Margin = new Thickness(0, 0, 10, 0)
                    };

                    // Add a TextBlock with the grade.Value
                    gradeTextBlock = new TextBox {
                        Text = grade.Value.ToString(),
                        FontSize = 15,
                        Tag = $"GroupCatInput_{grade.Id}"
                    };
                    ((TextBox)gradeTextBlock).TextChanged += UpdateValueChanged;

                    // Add box to fill with warn
                    catValueInfoTextBlock = new TextBlock {
                        Text = "(0-6)",
                        FontSize = 15,
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = (Style)FindResource("GrayedOut"),
                        Tag = $"GroupCatInfo_{grade.Id}"
                    };
                } else {
                    categoryTextBlock = new TextBlock {
                        Text = grade.Category.Name + ":",
                        FontSize = 15,
                        Margin = new Thickness(0, 0, 10, 0),
                        Style = (Style)FindResource("GrayedOut")
                    };

                    // Add a TextBlock with the grade.Value
                    gradeTextBlock = new TextBlock {
                        Text = grade.Value.ToString(),
                        FontSize = 15,
                        Style = (Style)FindResource("GrayedOut")
                    };

                    catValueInfoTextBlock = new TextBlock {
                    };
                }

                // Add it al together
                stackPanel.Children.Add(categoryTextBlock);
                innerStackPanel.Children.Add(gradeTextBlock);
                innerStackPanel.Children.Add(catValueInfoTextBlock);
                stackPanel.Children.Add(innerStackPanel);
                border.Child = stackPanel;
                GroupGameGradesWrapper.Children.Add(border);
            }

            // Get all categories for the WindowInstance.Shared.user
            var categories = WindowInstance.DbContext.GetCategories();
            List<Category> userCategories = categories.Where(c => c.userId == WindowInstance.Shared.user.Id).ToList();

            UpdateTotalGrade();
        }

        private void UpdateTotalGrade() {
            // totalGrade is median of all grades
            var totalGrade = 0;
            if (resolvedGroup != null) {
                foreach (var grade in resolvedGroup.Grades) {
                    totalGrade += grade.Value;
                }
                totalGrade /= resolvedGroup.Grades.Count;
            }

            // If totalGradeGolder is null create element into GroupGameGradesWrapper.Children
            if (totalGradeGolder == null) {
                totalGradeGolder = new Border {
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#606060")),
                    BorderThickness = new Thickness(0.5),
                    Margin = new Thickness(5),
                    CornerRadius = new CornerRadius(18),
                    Padding = new Thickness(15, 10, 15, 10),
                    Height = 42,
                    Width = 300
                };
                // Inside the border, create a StackPanel horizontal
                var stackPanel = new StackPanel {
                    Orientation = Orientation.Horizontal
                };
                // Add a TextBlock with the grade.Category.Name
                var categoryTextBlock = new TextBlock {
                    Text = "Total:",
                    FontSize = 15,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                // Add a TextBlock with the grade.Value
                var gradeTextBlock = new TextBlock {
                    Text = totalGrade.ToString(),
                    FontSize = 15
                };
                var calcNoticeTextBlock = new TextBlock {
                    Text = "(Medelvärde)",
                    FontSize = 15,
                    Margin = new Thickness(10, 0, 0, 0),
                    Style = (Style)FindResource("GrayedOut")
                };
                // Add it al together
                stackPanel.Children.Add(categoryTextBlock);
                stackPanel.Children.Add(gradeTextBlock);
                stackPanel.Children.Add(calcNoticeTextBlock);
                totalGradeGolder.Child = stackPanel;
                GroupGameGradesWrapper.Children.Add(totalGradeGolder);
            } else {
                // else update the value
                ((TextBlock)((StackPanel)totalGradeGolder.Child).Children[1]).Text = totalGrade.ToString();
            }
        }

        private void UpdateValueChanged(object sender, TextChangedEventArgs e) {
            TextBlock? infoBox = null;
            int? gradeId = null;

            // Get info field, sender has Tag $"GroupCatInput_{grade.Id}" and infobox has Tag $"GroupCatInfo_{grade.Id}"
            if (((TextBox)sender).Tag != null) {
                gradeId = int.Parse( ((string)((TextBox)sender).Tag).Split('_')[1] );

                // find infoBox by iterating children of GroupGameGradesWrapper that is of the type TextBlock to find one with the Tag $"GroupCatInfo_{grade.Id}"
                foreach (var child in GroupGameGradesWrapper.Children) {
                    if (child is Border) {
                        // Get child StackPanel
                        var stackPanel = (StackPanel)((Border)child).Child;
                        // Get inner stackPanel
                        var innerStackPanel = (StackPanel)((StackPanel)child).Children[0];
                        // Iterate grandchild for children of type TextBlock and find with Tag $"GroupCatInfo_{grade.Id}"
                        foreach (var grandChild in innerStackPanel.Children) {
                            if (grandChild is TextBlock) {
                                if (((TextBlock)grandChild).Tag != null) {
                                    if (((string)((TextBlock)grandChild).Tag).Split('_')[1] == gradeId.ToString()) {
                                        infoBox = (TextBlock)grandChild;
                                    }
                                }
                            }
                        }

                    }
                }
            }

            // Validate input to be between option:num_grade_min_value and option:num_grade_max_value
            var input_string = ((TextBox)sender).Text;
            if (infoBox != null) {
                if (input_string != null && input_string != "") {
                    infoBox.Text = "Värde 0-6!";
                }

                try {
                    int num_grade_min_value = int.Parse(WindowInstance.DbContext.GetOption("num_grade_min_value") ?? "");
                    int num_grade_max_value = int.Parse(WindowInstance.DbContext.GetOption("num_grade_max_value") ?? "");
                    int new_value = int.Parse(input_string);
                    if (new_value < num_grade_min_value) {
                        infoBox.Text = "Värde 0-6! (low)";
                        return;
                    } else if (new_value > num_grade_max_value) {
                        infoBox.Text = "Värde 0-6! (high)";
                        return;
                    } else {
                        infoBox.Text = "(0-6)";
                    }
                } catch {
                    infoBox.Text = "Värde 0-6! (exc)";
                    return;
                }
            } else {
                return;
            }

            // Update db
            if (gradeId != null && resolvedGroup != null) {
                ResolvedGrade? resolvedGrade = this.resolvedGroup.Grades.Where(rg => rg.Id == gradeId).FirstOrDefault();
                if (resolvedGrade != null) {
                    WindowInstance.DbContext.ModifyGrade(gradeId ?? -1, resolvedGrade.User.Id, resolvedGrade.Id, resolvedGrade.Category.Id, int.Parse(input_string));
                }

                // Re-Calc total grade
                UpdateTotalGrade();

                infoBox.Text = "(0-6)";
            } else {
                infoBox.Text = "Värde 0-6! (dbset)";
            }
        }

        private void GroupGameOpenUrl(object sender, RoutedEventArgs e) {
            if (this.resolvedGroup != null) {
                OpenUrl(this.resolvedGroup.GameUrl);
            }
        }
    }
}
