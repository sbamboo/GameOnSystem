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

        private Border GetElementsForGroup(ResolvedGrade grade) {
            // Create a new Border
            var border = new Border {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#606060")),
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(0, 5, 5, 5),
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
            if (grade.User.ID == WindowInstance.Shared.user.ID) {
                categoryTextBlock = new TextBlock {
                    Text = grade.Category.Name + ":",
                    FontSize = 15,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                // Add a TextBlock with the grade.Value
                gradeTextBlock = new TextBox {
                    Text = grade.Value.ToString(),
                    FontSize = 15,
                    Tag = $"GroupCatInput_{grade.ID}"
                };
                ((TextBox)gradeTextBlock).TextChanged += UpdateValueChanged;

                // Add box to fill with warn
                catValueInfoTextBlock = new TextBlock {
                    Text = "(0-6)",
                    FontSize = 15,
                    Margin = new Thickness(10, 0, 0, 0),
                    Style = (Style)FindResource("GrayedOut"),
                    Tag = $"GroupCatInfo_{grade.ID}"
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

            return border;
        }

        private void UpdateGroupGradesView() {
            if (this.resolvedGroup == null) {
                return;
            }

            // Iterate resolvedGroup.Grades and for each place elements under GroupGameGradesWrapper
            List<Category> existingCategories = new List<Category>();
            GroupGameGradesWrapper.Children.Clear();
            foreach (var grade in resolvedGroup.Grades) {

                Border border = GetElementsForGroup(grade);

                GroupGameGradesWrapper.Children.Add(border);

                existingCategories.Add(grade.Category);
            }

            // Get all categories for the WindowInstance.Shared.user
            List<Category> userFocusCategories = WindowInstance.DbContext.GetCategoriesForUser(WindowInstance.Shared.user.ID);
            // Iterate userFocusCategories and for each if not exists in existingCategories add new to existingCategories
            foreach (var userFocusCategory in userFocusCategories) {
                if (existingCategories.Contains(userFocusCategory) == false) {
                    string defaultValue = "";
                    if (userFocusCategory.CategoryType.Contains("int")) {
                        defaultValue = "0";
                    }
                    int? newGrade = WindowInstance.DbContext.AddGradeResolving(defaultValue, this.resolvedGroup.ID, WindowInstance.Shared.user.ID, userFocusCategory.ID);
                    if (newGrade != null) {
                        ResolvedGrade newResolvedGrade = new ResolvedGrade {
                            ID = (int)newGrade,
                            User = WindowInstance.Shared.user,
                            Category = userFocusCategory,
                            Value = defaultValue
                        };
                        Border border = GetElementsForGroup(newResolvedGrade);
                        GroupGameGradesWrapper.Children.Add(border);
                    }
                }
            }

            UpdateTotalGrade();
        }

        private void UpdateTotalGrade() {
            // totalGrade is median of all grades
            var totalGrade = 0;
            if (resolvedGroup != null) {
                foreach (var grade in resolvedGroup.Grades) {
                    if (grade.Value.All(char.IsDigit) == true) {
                        totalGrade += int.Parse(grade.Value);
                    }
                }
                if (resolvedGroup.Grades.Count > 0) {
                    totalGrade /= resolvedGroup.Grades.Count;
                } else {
                    totalGrade = 0;
                }
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
                        var innerStackPanel = new StackPanel();
                        foreach (var grandChild in stackPanel.Children) {
                            if (grandChild is StackPanel) {
                                innerStackPanel = (StackPanel)grandChild;
                                break;
                            }
                        }
                        // Iterate grandchild for children of type TextBlock and find with Tag $"GroupCatInfo_{grade.Id}"
                        foreach (var grandGrandChild in (UIElementCollection)((StackPanel)innerStackPanel).Children) {
                            if (grandGrandChild is TextBlock) {
                                if (((TextBlock)grandGrandChild).Tag != null) {
                                    if (((string)((TextBlock)grandGrandChild).Tag).Split('_')[1] == gradeId.ToString()) {
                                        infoBox = (TextBlock)grandGrandChild;
                                    }
                                }
                            }
                        }

                    }
                }
            }

            // Validate input
            string categoryType = "int_0-6";
            string categoryTypeName = "0-6";
            if (gradeId != null) {
                ResolvedGrade? resolvedGrade = this.resolvedGroup.Grades.Where(rg => rg.ID == gradeId).FirstOrDefault();
                if (resolvedGrade != null) {
                    categoryType = resolvedGrade.Category.CategoryType;
                }
            }
            if (categoryType.Contains("int_")) {
                categoryTypeName = categoryType.Replace("int_", "");
            }
            var input_string = ((TextBox)sender).Text;
            if (infoBox != null) {
                if (input_string != null && input_string != "") {
                    infoBox.Text = $"Värde {categoryTypeName}!";
                }

                if (input_string == "") {
                    infoBox.Text = $"Värde {categoryTypeName}! (Tom)";
                    return;
                }

                try {
                    int.Parse(input_string);
                } catch {
                    infoBox.Text = $"Värde {categoryTypeName}! (Felaktig)";
                    return;
                }

                try {
                    var (result, reason) = WindowInstance.DbContext.ValidateCategoryValueBasedOnType($"{input_string}", categoryType);

                    if (reason != null && reason.Contains($"{categoryType}_")) {
                        reason = reason.Replace($"{categoryType}_", "");
                    }

                    if (result == false) {
                        infoBox.Text = $"Värde {categoryTypeName}! ({reason})";
                        return;
                    } else {
                        infoBox.Text = $"({categoryTypeName})";
                    }
                } catch (System.Exception ex) {
                    infoBox.Text = $"Värde {categoryTypeName}! ({ex.Message})";
                    return;
                }
            } else {
                return;
            }

            // Update db
            if (gradeId != null && resolvedGroup != null) {
                ResolvedGrade? resolvedGrade = this.resolvedGroup.Grades.Where(rg => rg.ID == gradeId).FirstOrDefault();
                if (resolvedGrade != null) {
                    //WindowInstance.DbContext.ModifyGrade(gradeId ?? -1, resolvedGrade.User.ID, resolvedGrade.ID, resolvedGrade.Category.ID, int.Parse(input_string));
                    WindowInstance.DbContext.ModifyGradeResolving(gradeId ?? -1, input_string, resolvedGroup.ID, resolvedGrade.User.ID, resolvedGrade.Category.ID);
                }

                // Re-Calc total grade
                UpdateTotalGrade();

                infoBox.Text = $"({categoryTypeName})";
            } else {
                infoBox.Text = $"Värde {categoryTypeName}! (dbset)";
            }
        }

        private void GroupGameOpenUrl(object sender, RoutedEventArgs e) {
            if (this.resolvedGroup != null) {
                OpenUrl(this.resolvedGroup.GameUrl);
            }
        }
    }
}
