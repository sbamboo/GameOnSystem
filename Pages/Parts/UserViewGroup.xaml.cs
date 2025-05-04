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

namespace GameOnSystem.Pages.Parts {
    /// <summary>
    /// Interaction logic for UserViewGroup.xaml
    /// </summary>
    public partial class UserViewGroup : UserControl {

        internal MainWindow windowInstance;
        internal DbTableModel_Group group;

        static void OpenUrl(string url) {
            try {
                ProcessStartInfo processInfo = new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true // default browser
                };
                Process.Start(processInfo);
            }
            catch (Exception ex) {
                ;
            }
        }

        internal UserViewGroup(MainWindow windowInstance, DbTableModel_Group group) {
            this.windowInstance = windowInstance;
            this.group = group;

            InitializeComponent();

            // Set the group title
            GroupTitle.Text = $"{group.Name} - {group.GameName}";

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

            // Add the categories and their grades as
            bool gradeShowsAppUser = (bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_grade_shows_username", true);
            bool gradeCommentAfterDeadline = (bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_grade_comment_after_deadline", false);
            /*
             * <ScrollViewer:Vertical x:Name="GradingSpace" Grid.Column="2">
             *     <StackPanel x:Name="GroupGradesList">
             *         <Border>
             *             <ScrollViewer>
             *                 <StackPanel:Vertical>
             *                     <TextBlock>{category}</TextBlock>
             *                     <StackPanel:Vertical>
             *                         <Border>
             *                             <DockPanel (rec-is-2px-wide)>
             *                                 <TextBlock>{grade_appuser_name}</TextBlock>
             *                                 <Rectangle Width=2/>
             *                                 <TextBlock>{grade_formatted_value}</TextBlock>
             *                                 <Rectangle Width=2/>
             *                                 <TextBlock>{grade_comment}</TextBlock>
             *                             </DockPanel>
             *                         </Border>
             *                     </StackPanel>
             *                     <TextBlock:Collapsed></TextBlock>
             *                 </StackPanel>
             *             </ScrollViewer>
             *         </Border>
             *     </StackPanel>
             * </ScrollViewer>
             * <ContentControl x:Name="GroupTotalGradeSpace" Grid.Row="1">
             *     <Border>
             *         <StackPanel:Vertical>
             *             <DockPanel:Horizontal>
             *                 <TextBlock>Total: </TextBlock>
             *                 <TextBlock>{total_average_for_group}</TextBlock>
             *                 <TextBlock> (average)</TextBlock>
             *             </DockPanel>
             *         </StackPanel>
             *     </Border>
             * </ContentControl>
             */

            List<DbTableModel_Grade> grades = group.GetGrades(windowInstance.Shared.appDbContext);
            DbTableModel_Edition edition = group.GetEdition(windowInstance.Shared.appDbContext);

            bool isDeadlineAvaliable = true;
            // If the edition has a deadline and the deadline has passed set to false
            if (edition.GradingDeadline != null) {
                if (DateTime.Now > edition.GradingDeadline) {
                    isDeadlineAvaliable = false;
                }
            }

            if (grades.Count == 0) {
                TextBlock noGradesText = new TextBlock {
                    Text = "No grades found for this group.",
                    FontSize = 18
                };

                GroupTotalGradeSpace.Content = noGradesText;
            } else {
                Dictionary<DbTableModel_Category, UITools_GroupGradeCategory> categoryStackPanels = new Dictionary<DbTableModel_Category, UITools_GroupGradeCategory>();
                Dictionary<DbTableModel_Category, List<Border>> gradeBordersPerCategory = new Dictionary<DbTableModel_Category, List<Border>>();

                // Get al the players focus categories, for each check if there are any grades for them, if there is not add a input field for it.
                List<DbTableModel_Category> ungradedFocusCategories = windowInstance.Shared.user.GetFocusCategories(windowInstance.Shared.appDbContext);

                // Iterate al grades if a grade is found under a category in focusCategories remove it from the list
                foreach (DbTableModel_Grade grade in grades) {
                    DbTableModel_Category category = grade.GetCategory(windowInstance.Shared.appDbContext);
                    if (ungradedFocusCategories.Contains(category) && grade.GetAppUser(windowInstance.Shared.appDbContext).ID == windowInstance.Shared.user.ID) {
                        ungradedFocusCategories.Remove(category);
                    }
                }

                // Now left we have a list of categories that the user has not graded, add input fields for them
                foreach (DbTableModel_Category category in ungradedFocusCategories) {
                    // Check if the category already exists in categoryStackPanels
                    if (!categoryStackPanels.ContainsKey(category)) {

                        // Create the UITools instance
                        UITools_GroupGradeCategory uiToolsInstance = new UITools_GroupGradeCategory(category);

                        categoryStackPanels.Add(category, uiToolsInstance);

                        // Add the border to GroupGradesList
                        GroupGradesList.Children.Add(uiToolsInstance.Border);
                    }

                    // Now add the input field
                    // Get the stackpanel from the categoryStackPanels
                    StackPanel stackpanel = categoryStackPanels[category].InnerStackPanel;

                    // Create the border element
                    Border border = new Border {
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
                    };
                    if (!gradeBordersPerCategory.ContainsKey(category)) {
                        gradeBordersPerCategory.Add(category, new List<Border>());
                    }
                    gradeBordersPerCategory[category].Add(border);

                    // Create the dockpanel for the entry
                    DockPanel entryDockPanel = new DockPanel();

                    // If isDeadlineAvaliable is false show an error text unless ff_grade_comment_after_deadline is enabled
                    if (isDeadlineAvaliable == false && !gradeCommentAfterDeadline) {
                        TextBlock deadlineText = new TextBlock {
                            Text = "Grading deadline has passed!",
                            FontSize = 15
                        };
                        deadlineText.Style = (Style)FindResource("GrayedOutTextBlock");
                        entryDockPanel.Children.Add(deadlineText);

                    // Else show the input fields
                    } else {
                        // Add the save button
                        Button gradeSaveButton = new Button {
                            Content = "Save",
                            FontSize = 15
                        };
                        /*
                        // Set content to textblock containg "Save"
                        TextBlock gradeSaveButtonText = new TextBlock {
                            Text = "Save",
                            FontSize = 15
                        };
                        gradeSaveButtonText.Style = (Style)FindResource("StdTextBlock");
                        gradeSaveButton.Content = gradeSaveButtonText;
                        */
                        gradeSaveButton.IsEnabled = false;
                        entryDockPanel.Children.Add(gradeSaveButton);

                        // Add the divider rectangle #343434
                        Rectangle divider1 = new Rectangle {
                            Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                            Width = 2,
                            Height = 20,
                            Margin = new Thickness(5, 0, 5, 0)
                        };
                        entryDockPanel.Children.Add(divider1);

                        // Add the grade value textbox/textblock
                        TextBox grade_value = new TextBox {
                            Text = "",
                            FontSize = 15,
                            MinWidth = 20
                        };
                        // If the grade type is "Abbriv IG-MVG" it must be wider
                        if (edition.GradeType == 5) {
                            grade_value.MinWidth = 40;
                        }
                        // If ff_grade_comment_after_deadline is enabled disable the input field
                        if (gradeCommentAfterDeadline) {
                            grade_value.IsEnabled = false;
                        }
                        entryDockPanel.Children.Add(grade_value);

                        // Add the grade type textblock
                        TextBlock grade_type = new TextBlock {
                            Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                            FontSize = 15
                        };
                        grade_type.Style = (Style)FindResource("GrayedOutTextBlock");
                        entryDockPanel.Children.Add(grade_type);

                        // Add the divider rectangle #343434
                        Rectangle divider2 = new Rectangle {
                            Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                            Width = 2,
                            Height = 20,
                            Margin = new Thickness(5, 0, 5, 0)
                        };
                        entryDockPanel.Children.Add(divider2);

                        // Add the grade comment textblock
                        TextBox grade_comment = new TextBox {
                            Text = "",
                            FontSize = 15,
                            MinWidth = 300
                        };
                        entryDockPanel.Children.Add(grade_comment);

                        // Link the save button
                        int? gradeUserCat = windowInstance.Shared.user.GetUserCatForFocusCategory(windowInstance.Shared.appDbContext, category.ID);
                        if (gradeUserCat != null) { // Null if for some reason the user does not have a usercat for the category
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

                    // Add the entryDockPanel to the border
                    border.Child = entryDockPanel;

                    // Add the border to the stackpanel
                    stackpanel.Children.Add(border);
                }


                // Add grades to the GroupGradesList, creating categories if they are not created and adding grades to them if they are
                foreach (DbTableModel_Grade grade in grades) {
                    // Check if the category already exists in categoryStackPanels
                    DbTableModel_Category category = grade.GetCategory(windowInstance.Shared.appDbContext);
                    if (!categoryStackPanels.ContainsKey(category)) {

                        // Create the UITools instance
                        UITools_GroupGradeCategory uiToolsInstance = new UITools_GroupGradeCategory(category);

                        categoryStackPanels.Add(category, uiToolsInstance);

                        // Add the border to GroupGradesList
                        GroupGradesList.Children.Add(uiToolsInstance.Border);
                    }


                    // Find AppUsers focus grade
                    bool isUsersGrade = false;
                    if (windowInstance.Shared.user.HasFocusCategory(windowInstance.Shared.appDbContext, category.ID)) {
                        DbTableModel_AppUser? gradePlacer = grade.GetAppUser(windowInstance.Shared.appDbContext);
                        if (gradePlacer != null && gradePlacer.ID == windowInstance.Shared.user.ID) {
                            isUsersGrade = true;
                        }
                    }


                    // Get the stackpanel from the categoryStackPanels
                    StackPanel stackpanel = categoryStackPanels[category].InnerStackPanel;

                    // Create the border element
                    Border border = new Border {
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
                    };
                    if (!gradeBordersPerCategory.ContainsKey(category)) {
                        gradeBordersPerCategory.Add(category, new List<Border>());
                    }
                    gradeBordersPerCategory[category].Add(border);

                    // Create the dockpanel for the grade entry
                    DockPanel entryDockPanel = new DockPanel();

                    // Add the grade_appuser_name textblock if enabled
                    if (gradeShowsAppUser) {
                        TextBlock grade_appuser_name = new TextBlock {
                            Text = grade.GetAppUser(windowInstance.Shared.appDbContext).Name,
                            FontSize = 15,
                            Padding = new Thickness(3, 0, 3, 0)
                        };
                        entryDockPanel.Children.Add(grade_appuser_name);

                        // Add the divider rectangle #343434
                        Rectangle divider = new Rectangle {
                            Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                            Width = 2,
                            Height = 20,
                            Margin = new Thickness(5, 0, 5, 0)
                        };
                        entryDockPanel.Children.Add(divider);
                    }

                    // Hide -1 grades
                    if (grade.NumValue != -1) {
                        // Add the grade value textbox/textblock
                        TextBlock grade_value = new TextBlock {
                            Text = grade.GetGradeAsString(),
                            FontSize = 15
                        };
                        if (isUsersGrade) {
                            grade_value.FontWeight = FontWeights.Bold;
                            grade_value.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        }
                        entryDockPanel.Children.Add(grade_value);

                        // Add the grade type textblock
                        TextBlock grade_type = new TextBlock {
                            Text = $" ({grade.GetGradeTypeName()})",
                            FontSize = 15
                        };
                        grade_type.Style = (Style)FindResource("GrayedOutTextBlock");
                        entryDockPanel.Children.Add(grade_type);
                    }

                    // Add the grade comment textblock if the comment is not empty
                    if (!string.IsNullOrEmpty(grade.Comment)) {

                        // Add the divider rectangle #343434
                        if (grade.NumValue != -1) {
                            Rectangle divider2 = new Rectangle {
                                Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                                Width = 2,
                                Height = 20,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            entryDockPanel.Children.Add(divider2);
                        }

                        // Add the grade comment textblock
                        TextBlock grade_comment = new TextBlock {
                            Text = '"'+grade.Comment+'"',
                            FontSize = 15
                        };
                        entryDockPanel.Children.Add(grade_comment);
                    }

                    // Add the entryDockPanel to the border
                    border.Child = entryDockPanel;

                    // Add the border to the stackpanel
                    stackpanel.Children.Add(border);
                }

                // For each category set the border thickness to 0 on the last element
                foreach (var category in gradeBordersPerCategory) {
                    category.Value.Last().BorderThickness = new Thickness(0, 0, 0, 0);
                    category.Value.Last().Margin = new Thickness(0, 0, 0, 5);
                }

                UpdateAverageValue();
            }
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
            // Open the group.GameUrl in the default browser
            if (!string.IsNullOrEmpty(group.GameUrl)) {
                // if PlayButtonPath is hovered open the url
                if (PlayButtonPath.IsMouseOver || PlayButtonInnerPath.IsMouseOver || PlayButton.IsKeyboardFocused) {
                    OpenUrl(group.GameUrl);
                }
            }
        }

        private void GroupTitle_LayoutUpdated(object sender, EventArgs e) {
            // Attempt to autoscale the font size
            double availableWidth = GroupTitleWrapper.ActualWidth;
            double currentWidth = GroupTitle.ActualWidth;
            // Calculate the new fontsize so the text fits in the available space
            if (currentWidth > availableWidth) {
                double fontSize = GroupTitle.FontSize * (availableWidth / currentWidth);
                try {
                    GroupTitle.FontSize = fontSize;
                }
                catch {
                    ;
                }
            }
        }

        private void GroupMembers_LayoutUpdated(object sender, EventArgs e) {
            // Attempt to autoscale the font size
            double availableWidth = GroupTitleWrapper.ActualWidth;
            double currentWidth = GroupMembers.ActualWidth;
            // Calculate the new fontsize so the text fits in the available space
            if (currentWidth > availableWidth) {
                double fontSize = GroupMembers.FontSize * (availableWidth / currentWidth);
                try {
                    GroupMembers.FontSize = fontSize;
                }
                catch {
                    ;
                }
            }
        }

        // When grade save button is pressed
        private void GradeSave_Click(object sender, RoutedEventArgs e) {
            Button senderButton = (Button)sender;
            UITools_GroupGradeSaveButtonTag tag = (UITools_GroupGradeSaveButtonTag)senderButton.Tag;

            TextBlock gradeSaveButtonText = tag.buttonText;
            int groupID = tag.groupID;
            int userCatID = tag.userCatID;
            DbTableModel_Edition edition = tag.edition;
            bool numValueIsEnabled = tag.numValueIsEnabled;
            string gradeValue = tag.valueBox.Text;
            string gradeComment = tag.commentBox.Text;
            UITools_GroupGradeCategory uITools_GroupGradeCategory = tag.categoryUiTools;

            int gradeIntValue = -1;

            // If the numeral value is enabled parse it (also empty check)
            if (numValueIsEnabled) {
                // Check if the gradeValue is empty
                if (string.IsNullOrEmpty(gradeValue)) {
                    //gradeSaveButtonText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Text = "Grade value cannot be empty!";
                    uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Visible;
                    return;
                }

                gradeIntValue = DbTableHelper.GetGradeFromString(edition.GradeType, gradeValue, -1, edition.GradeMin, edition.GradeMax);
                // If the gradeIntValue is -1, the gradeValue was not able to be parsed
                if (gradeIntValue == -1) {
                    //gradeSaveButtonText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Text = "Invalid grade value! (UnParsable)";
                    uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Visible;
                    return;
                }

                // Validate the gradeValue
                if(!DbTableHelper.IsValidForType(edition.GradeType, gradeIntValue, edition.GradeMin, edition.GradeMax)) {
                    //gradeSaveButtonText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Style = (Style)FindResource("RedNoticeText");
                    uITools_GroupGradeCategory.InformationText.Text = "Invalid grade value!";
                    uITools_GroupGradeCategory.InformationText.Visibility = Visibility.Visible;
                    return;
                }
            }

            // If comment is empty set to empty string to ensure comapt
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

            // Disable button and inputs and set button foreground to green
            Dispatcher.Invoke(() => {
                senderButton.Content = "✓";
                //gradeSaveButtonText.Style = (Style)FindResource("GreenNoticeText");
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
                TextBlock noGradesText = new TextBlock {
                    Text = "No grades found for this group.",
                    FontSize = 18
                };
                GroupTotalGradeSpace.Content = noGradesText;
                return;
            }

            (int? totalAverage_int, double? totalAverage_double) = group.GetAverageGrade_Prefered(windowInstance.Shared.appDbContext);
            string totalAverage = totalAverage_int != null
                ? DbTableHelper.GetGradeAsString(edition.GradeType, (int)totalAverage_int)
                : totalAverage_double != null
                    ? DbTableHelper.GetDoubleGradeAsString(edition.GradeType, (double)totalAverage_double)
                    : "?";

            Border totalBorder = new Border {
                BorderThickness = new Thickness(1, 1, 1, 1),
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
                Text = totalAverage,
                FontSize = 18
            });

            var averageText = new TextBlock {
                Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                FontSize = 18,
                Style = (Style)FindResource("GrayedOutTextBlock")
            };
            dockPanel.Children.Add(averageText);

            // Add grading deadline info
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
                totalStackPanel.Children.Add(deadlineText);
            }

            totalBorder.Child = totalStackPanel;
            GroupTotalGradeSpace.Content = totalBorder;
        }

    }
}
