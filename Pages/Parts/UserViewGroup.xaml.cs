using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameOnSystem.Pages.Parts {
    /// <summary>
    /// Interaction logic for UserViewGroup.xaml
    /// </summary>
    public partial class UserViewGroup : UserControl {

        internal MainWindow windowInstance;
        internal DbTableModel_Group group;
        internal bool isWinner = false;

        static void OpenUrl(string url) {
            try {
                ProcessStartInfo processInfo = new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true // default browser
                };
                Process.Start(processInfo);
            } catch (Exception) {
                // Ignore exceptions here
            }
        }

        internal UserViewGroup(MainWindow windowInstance, DbTableModel_Group group, bool isWinner) {
            this.windowInstance = windowInstance;
            this.group = group;
            this.isWinner = isWinner;

            InitializeComponent();

            // Set the group title
            GroupTitle.Text = $"{group.Name} - {group.GameName}";

            // Set winner badge
            if (isWinner) {
                WinnerGroupText.Visibility = Visibility.Visible;
                WinnerGroupText.Text = "Winner!";
            }

            // Add image element to GroupGamePresentWrapper if GameBannerUrl is available
            if (!string.IsNullOrEmpty(group.GameBannerUrl)) {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(group.GameBannerUrl);
                bitmap.DecodePixelWidth = 300;
                bitmap.DecodePixelHeight = 169;
                bitmap.EndInit();

                // Set the Image source
                GameImage.Source = bitmap;
            }

            // If GameUrl is available, show the play button
            if (!string.IsNullOrEmpty(group.GameUrl) && !(bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_hover_for_playbutton", false)) {
                PlayButton.Visibility = Visibility.Visible;
            }

            // Add group-participants
            GroupMembers.Text = "";
            foreach (var participant in group.GetParticipants(windowInstance.Shared.appDbContext)) {
                GroupMembers.Text += $"{participant.Name}, ";
            }
            GroupMembers.Text = GroupMembers.Text.TrimEnd(',', ' ');

            // Load grades and input fields
            LoadGradesAndInputs();
        }

        private void LoadGradesAndInputs() {
            List<DbTableModel_Grade> grades = group.GetGrades(windowInstance.Shared.appDbContext);
            DbTableModel_Edition edition = group.GetEdition(windowInstance.Shared.appDbContext);

            bool isDeadlineAvailable = true;
            if (edition.GradingDeadline != null && DateTime.Now > edition.GradingDeadline) {
                isDeadlineAvailable = false;
            }

            // Clear previous UI elements
            GroupGradesList.Children.Clear();

            Dictionary<DbTableModel_Category, UITools_GroupGradeCategory> categoryStackPanels = new Dictionary<DbTableModel_Category, UITools_GroupGradeCategory>();
            Dictionary<DbTableModel_Category, List<Border>> gradeBordersPerCategory = new Dictionary<DbTableModel_Category, List<Border>>();

            // Get user's focus categories
            List<DbTableModel_Category> ungradedFocusCategories = windowInstance.Shared.user.GetFocusCategories(windowInstance.Shared.appDbContext);

            // Remove categories that already have grades by this user
            foreach (var grade in grades) {
                var category = grade.GetCategory(windowInstance.Shared.appDbContext);
                if (ungradedFocusCategories.Contains(category) && grade.GetAppUser(windowInstance.Shared.appDbContext).ID == windowInstance.Shared.user.ID) {
                    ungradedFocusCategories.Remove(category);
                }
            }

            // Add input fields for ungraded focus categories
            bool gradeCommentAfterDeadline = (bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_grade_comment_after_deadline", false); ;

            // Show "No grades found" only if no grades AND no input fields to show
            if (grades.Count == 0 && ungradedFocusCategories.Count == 0) {
                TextBlock noGradesText = new TextBlock {
                    Text = "No grades found for this group.",
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                GroupGradesList.Children.Add(noGradesText);
            }

            foreach (var category in ungradedFocusCategories) {
                if (!categoryStackPanels.ContainsKey(category)) {
                    var uiToolsInstance = new UITools_GroupGradeCategory(category);
                    categoryStackPanels.Add(category, uiToolsInstance);
                    GroupGradesList.Children.Add(uiToolsInstance.Border);
                }

                var stackpanel = categoryStackPanels[category].InnerStackPanel;

                Border border = new Border {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
                };
                if (!gradeBordersPerCategory.ContainsKey(category)) {
                    gradeBordersPerCategory.Add(category, new List<Border>());
                }
                gradeBordersPerCategory[category].Add(border);

                DockPanel entryDockPanel = new DockPanel();

                if (!isDeadlineAvailable && !gradeCommentAfterDeadline) {
                    TextBlock deadlineText = new TextBlock {
                        Text = "Grading deadline has passed!",
                        FontSize = 15,
                        Style = (Style)FindResource("GrayedOutTextBlock")
                    };
                    entryDockPanel.Children.Add(deadlineText);
                } else {
                    Button gradeSaveButton = new Button {
                        Content = "Save",
                        FontSize = 15,
                        IsEnabled = false
                    };
                    entryDockPanel.Children.Add(gradeSaveButton);

                    Rectangle divider1 = new Rectangle {
                        Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                        Width = 2,
                        Height = 20,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    entryDockPanel.Children.Add(divider1);

                    TextBox grade_value = new TextBox {
                        Text = "",
                        FontSize = 15,
                        MinWidth = edition.GradeType == 5 ? 40 : 20,
                        IsEnabled = !gradeCommentAfterDeadline
                    };
                    entryDockPanel.Children.Add(grade_value);

                    TextBlock grade_type = new TextBlock {
                        Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                        FontSize = 15,
                        Style = (Style)FindResource("GrayedOutTextBlock")
                    };
                    entryDockPanel.Children.Add(grade_type);

                    Rectangle divider2 = new Rectangle {
                        Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                        Width = 2,
                        Height = 20,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    entryDockPanel.Children.Add(divider2);

                    TextBox grade_comment = new TextBox {
                        Text = "",
                        FontSize = 15,
                        MinWidth = 300
                    };
                    entryDockPanel.Children.Add(grade_comment);

                    int? gradeUserCat = windowInstance.Shared.user.GetUserCatForFocusCategory(windowInstance.Shared.appDbContext, category.ID);
                    if (gradeUserCat != null) {
                        gradeSaveButton.IsEnabled = true;
                        gradeSaveButton.Tag = new UITools_GroupGradeSaveButtonTag {
                            groupID = group.ID,
                            userCatID = (int)gradeUserCat,
                            edition = edition,
                            numValueIsEnabled = !gradeCommentAfterDeadline,
                            valueBox = grade_value,
                            commentBox = grade_comment,
                            categoryUiTools = categoryStackPanels[category]
                        };
                        gradeSaveButton.Click += GradeSave_Click;
                    }
                }

                border.Child = entryDockPanel;
                stackpanel.Children.Add(border);
            }

            // Add existing grades to UI
            bool gradeShowsAppUser = (bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_grade_shows_username", true);

            foreach (var grade in grades) {
                var category = grade.GetCategory(windowInstance.Shared.appDbContext);
                if (!categoryStackPanels.ContainsKey(category)) {
                    var uiToolsInstance = new UITools_GroupGradeCategory(category);
                    categoryStackPanels.Add(category, uiToolsInstance);
                    GroupGradesList.Children.Add(uiToolsInstance.Border);
                }

                bool isUsersGrade = false;
                if (windowInstance.Shared.user.HasFocusCategory(windowInstance.Shared.appDbContext, category.ID)) {
                    var gradePlacer = grade.GetAppUser(windowInstance.Shared.appDbContext);
                    if (gradePlacer != null && gradePlacer.ID == windowInstance.Shared.user.ID) {
                        isUsersGrade = true;
                    }
                }

                var stackpanel = categoryStackPanels[category].InnerStackPanel;

                Border border = new Border {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
                };
                if (!gradeBordersPerCategory.ContainsKey(category)) {
                    gradeBordersPerCategory.Add(category, new List<Border>());
                }
                gradeBordersPerCategory[category].Add(border);

                DockPanel entryDockPanel = new DockPanel();

                if (gradeShowsAppUser) {
                    TextBlock grade_appuser_name = new TextBlock {
                        Text = grade.GetAppUser(windowInstance.Shared.appDbContext).Name,
                        FontSize = 15,
                        Padding = new Thickness(3, 0, 3, 0)
                    };
                    entryDockPanel.Children.Add(grade_appuser_name);

                    Rectangle divider = new Rectangle {
                        Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                        Width = 2,
                        Height = 20,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    entryDockPanel.Children.Add(divider);
                }

                if (grade.NumValue != -1) {
                    TextBlock grade_value = new TextBlock {
                        Text = grade.GetGradeAsString(),
                        FontSize = 15
                    };
                    if (isUsersGrade) {
                        grade_value.FontWeight = FontWeights.Bold;
                        grade_value.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    }
                    entryDockPanel.Children.Add(grade_value);

                    TextBlock grade_type = new TextBlock {
                        Text = $" ({grade.GetGradeTypeName()})",
                        FontSize = 15,
                        Style = (Style)FindResource("GrayedOutTextBlock")
                    };
                    entryDockPanel.Children.Add(grade_type);
                }

                if (!string.IsNullOrEmpty(grade.Comment)) {
                    if (grade.NumValue != -1) {
                        Rectangle divider2 = new Rectangle {
                            Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                            Width = 2,
                            Height = 20,
                            Margin = new Thickness(5, 0, 5, 0)
                        };
                        entryDockPanel.Children.Add(divider2);
                    }

                    TextBlock grade_comment = new TextBlock {
                        Text = '"' + grade.Comment + '"',
                        FontSize = 15
                    };
                    entryDockPanel.Children.Add(grade_comment);
                }

                border.Child = entryDockPanel;
                stackpanel.Children.Add(border);
            }

            // For each category set the border thickness to 0 on the last element
            foreach (var category in gradeBordersPerCategory) {
                category.Value.Last().BorderThickness = new Thickness(0, 0, 0, 0);
                category.Value.Last().Margin = new Thickness(0, 0, 0, 5);
            }

            UpdateAverageValue();
        }

        // Show the play button when hovering over the image
        private void ImageWrapper_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            if (!string.IsNullOrEmpty(group.GameBannerUrl) && (bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_hover_for_playbutton", false)) {
                PlayButton.Visibility = Visibility.Visible;
            }
        }

        // Hide the play button when mouse leaves the image
        private void ImageWrapper_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            if ((bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_hover_for_playbutton", false)) {
                PlayButton.Visibility = Visibility.Collapsed;
            }
        }

        // When the play button is clicked, open the game URL
        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(group.GameUrl)) {
                if (PlayButtonPath.IsMouseOver || PlayButtonInnerPath.IsMouseOver || PlayButton.IsKeyboardFocused) {
                    OpenUrl(group.GameUrl);
                }
            }
        }

        private void GroupTitle_LayoutUpdated(object sender, EventArgs e) {
            double availableWidth = GroupTitleWrapper.ActualWidth;
            double currentWidth = GroupTitle.ActualWidth;
            if (currentWidth > availableWidth) {
                double fontSize = GroupTitle.FontSize * (availableWidth / currentWidth);
                try {
                    GroupTitle.FontSize = fontSize;
                } catch {
                    // Ignore exceptions
                }
            }
        }

        private void GroupMembers_LayoutUpdated(object sender, EventArgs e) {
            double availableWidth = GroupTitleWrapper.ActualWidth;
            double currentWidth = GroupMembers.ActualWidth;
            if (currentWidth > availableWidth) {
                double fontSize = GroupMembers.FontSize * (availableWidth / currentWidth);
                try {
                    GroupMembers.FontSize = fontSize;
                } catch {
                    // Ignore exceptions
                }
            }
        }

        // When grade save button is pressed
        private void GradeSave_Click(object sender, RoutedEventArgs e) {
            Button senderButton = (Button)sender;
            UITools_GroupGradeSaveButtonTag tag = (UITools_GroupGradeSaveButtonTag)senderButton.Tag;

            int groupID = tag.groupID;
            int userCatID = tag.userCatID;
            DbTableModel_Edition edition = tag.edition;
            bool numValueIsEnabled = tag.numValueIsEnabled;
            string gradeValue = tag.valueBox.Text;
            string gradeComment = tag.commentBox.Text;
            UITools_GroupGradeCategory uITools_GroupGradeCategory = tag.categoryUiTools;

            int gradeIntValue = -1;

            if (numValueIsEnabled) {
                if (string.IsNullOrEmpty(gradeValue)) {
                    uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Text = "Grade value cannot be empty!";
                    uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Visible;
                    return;
                }

                gradeIntValue = DbTableHelper.GetGradeFromString(edition.GradeType, gradeValue, -1, edition.GradeMin, edition.GradeMax);
                if (gradeIntValue == -1) {
                    uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Text = "Invalid grade value! (UnParsable)";
                    uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Visible;
                    return;
                }

                if (!DbTableHelper.IsValidForType(edition.GradeType, gradeIntValue, edition.GradeMin, edition.GradeMax)) {
                    uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Text = "Invalid grade value!";
                    uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Visible;
                    return;
                }
            }

            if (string.IsNullOrEmpty(gradeComment)) {
                gradeComment = "";
            }

            windowInstance.Shared.appDbContext.AddGrade(
                gradeIntValue,
                gradeComment,
                groupID,
                userCatID,
                edition.GradeType
            );

            Dispatcher.Invoke(() => {
                senderButton.Content = "✓";
                senderButton.Style = (Style)FindResource("GreenableButton");
                uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("StdTextBlock");
                uITools_GroupGradeCategory.InformationText.Text = "";
            });
            Dispatcher.Invoke(() => {
                uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Collapsed;
                senderButton.IsEnabled = false;
                tag.valueBox.IsEnabled = false;
                tag.commentBox.IsEnabled = false;
            });
            Dispatcher.Invoke(() => {
                UpdateAverageValue();
            });
        }

        private void UpdateAverageValue() {
            List<DbTableModel_Grade> grades = group.GetGrades(windowInstance.Shared.appDbContext);
            DbTableModel_Edition edition = group.GetEdition(windowInstance.Shared.appDbContext);

            if (grades.Count == 0) {
                Border totalBorder = new Border {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
                };

                StackPanel totalStackPanel = new StackPanel {
                    Orientation = Orientation.Vertical
                };

                DockPanel dockPanel = new DockPanel();
                totalStackPanel.Children.Add(dockPanel);

                dockPanel.Children.Add(new TextBlock {
                    Text = "Total Average: ",
                    FontSize = 18
                });

                dockPanel.Children.Add(new TextBlock {
                    Text = "0",
                    FontSize = 18
                });

                dockPanel.Children.Add(new TextBlock {
                    Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                    FontSize = 18,
                    Style = (Style)FindResource("GrayedOutTextBlock")
                });

                if (edition.GradingDeadline != null) {
                    TextBlock deadlineText = new TextBlock {
                        Text = DateTime.Now > edition.GradingDeadline
                            ? "Grading deadline has passed!"
                            : $"Grading deadline: {edition.GradingDeadline.Value:yyyy-MM-dd HH:mm}",
                        FontSize = 15,
                        Style = (Style)FindResource(
                            DateTime.Now > edition.GradingDeadline
                                ? "RedNoticeText"
                                : "GradeDeadlineText")
                    };
                    if (isWinner && DateTime.Now > edition.GradingDeadline) {
                        deadlineText.Text = "Grading deadline has passed! (Winning Group)";
                        deadlineText.Style = (Style)FindResource("GoldTextBlock");
                    }
                    totalStackPanel.Children.Add(deadlineText);
                }

                totalBorder.Child = totalStackPanel;
                GroupTotalGradeSpace.Content = totalBorder;
                return;
            }

            (int? totalAverage_int, double? totalAverage_double) = group.GetAverageGrade_Prefered(windowInstance.Shared.appDbContext);
            string totalAverage = totalAverage_int != null
                ? DbTableHelper.GetGradeAsString(edition.GradeType, (int)totalAverage_int)
                : totalAverage_double != null
                    ? DbTableHelper.GetDoubleGradeAsString(edition.GradeType, (double)totalAverage_double)
                    : "?";

            Border totalBorderExisting = new Border {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
            };

            StackPanel totalStackPanelExisting = new StackPanel {
                Orientation = Orientation.Vertical
            };

            DockPanel dockPanelExisting = new DockPanel();
            totalStackPanelExisting.Children.Add(dockPanelExisting);

            dockPanelExisting.Children.Add(new TextBlock {
                Text = "Total Average: ",
                FontSize = 18
            });

            dockPanelExisting.Children.Add(new TextBlock {
                Text = totalAverage,
                FontSize = 18
            });

            dockPanelExisting.Children.Add(new TextBlock {
                Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                FontSize = 18,
                Style = (Style)FindResource("GrayedOutTextBlock")
            });

            if (edition.GradingDeadline != null) {
                TextBlock deadlineText = new TextBlock {
                    Text = DateTime.Now > edition.GradingDeadline
                        ? "Grading deadline has passed!"
                        : $"Grading deadline: {edition.GradingDeadline.Value:yyyy-MM-dd HH:mm}",
                    FontSize = 15,
                    Style = (Style)FindResource(
                        DateTime.Now > edition.GradingDeadline
                            ? "RedNoticeText"
                            : "GradeDeadlineText")
                };
                if (isWinner && DateTime.Now > edition.GradingDeadline) {
                    deadlineText.Text = "Grading deadline has passed! (Winning Group)";
                    deadlineText.Style = (Style)FindResource("GoldTextBlock");
                }
                totalStackPanelExisting.Children.Add(deadlineText);
            }

            totalBorderExisting.Child = totalStackPanelExisting;
            GroupTotalGradeSpace.Content = totalBorderExisting;
        }
    }
}
