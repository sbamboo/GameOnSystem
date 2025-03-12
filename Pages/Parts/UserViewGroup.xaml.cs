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
            } catch (Exception ex) {
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
            bool gradeShowsAppUser = (bool)windowInstance.Shared.appDbContext.GetFlagWdef("ff_grade_shows_username", false);
            /*
             * <ScrollViewer:Vertical x:Name="GradingSpace" Grid.Column="2">
             *     <StackPanel x:Name="GroupGradesList">
             *         <Border>
             *             <StackPanel:Vertical>
             *                 <TextBlock>{category}</TextBlock>
             *                 <ScrollViewer>
             *                     <StackPanel:Vertical>
             *                         <Border>
             *                             <Grid:Horizontal (rec-is-2px-wide)>
             *                                 <TextBlock>{grade_appuser_name}</TextBlock>
             *                                 <Rectangle></Rectangle>
             *                                 <TextBlock>{grade_formatted_value}</TextBlock>
             *                                 <Rectangle></Rectangle>
             *                                 <TextBlock>{grade_comment}</TextBlock>
             *                             </Grid>
             *                         </Border>
             *                     </StackPanel>
             *                 </ScrollViewer>
             *             </Border>
             *         </StackPanel>
             *     </StackPanel>
             * </ScrollViewer>
             * <ContentControl x:Name="GroupTotalGradeSpace" Grid.Row="1">
             *     <Border>
             *         <DockPanel:Horizontal>
             *             <TextBlock>Total: </TextBlock>
             *             <TextBlock>{total_average_for_group}</TextBlock>
             *             <TextBlock> (average)</TextBlock>
             *         </DockPanel>
             *     </Border>
             * </ContentControl>
             */

            List<DbTableModel_Grade> grades = group.GetGrades(windowInstance.Shared.appDbContext);
            DbTableModel_Edition edition = group.GetEdition(windowInstance.Shared.appDbContext);

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
                    if (ungradedFocusCategories.Contains(category)) {
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

                    // Create the grid for the grade entry
                    Grid grid = new Grid { };

                    // Make the grid have Auto 10 Auto Auto 10 Auto
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Pixel) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Pixel) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                    // Add the save button
                    Button gradeSaveButton = new Button {
                        Content = "Save",
                        FontSize = 15
                    };
                    Grid.SetColumn(gradeSaveButton, 0);
                    grid.Children.Add(gradeSaveButton);

                    // Add the divider rectangle #343434
                    Rectangle divider1 = new Rectangle {
                        Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                        Width = 2
                    };
                    Grid.SetColumn(divider1, 1);
                    grid.Children.Add(divider1);

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
                    Grid.SetColumn(grade_value, 2);
                    grid.Children.Add(grade_value);

                    // Add the grade type textblock
                    TextBlock grade_type = new TextBlock {
                        Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                        FontSize = 15
                    };
                    grade_type.Style = (Style)FindResource("GrayedOutTextBlock");
                    Grid.SetColumn(grade_type, 3);
                    grid.Children.Add(grade_type);

                    // Add the divider rectangle #343434
                    Rectangle divider2 = new Rectangle {
                        Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                        Width = 2
                    };
                    Grid.SetColumn(divider2, 4);
                    grid.Children.Add(divider2);

                    // Add the grade comment textblock
                    TextBox grade_comment = new TextBox {
                        Text = "",
                        FontSize = 15,
                        MinWidth = 300
                    };
                    Grid.SetColumn(grade_comment, 5);
                    grid.Children.Add(grade_comment);

                    // Add the grid to the border
                    border.Child = grid;

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

                    // Create the grid for the grade entry
                    Grid grid = new Grid {};

                    // Setup entry based on gradeShowsAppUser
                    if (gradeShowsAppUser) {
                        // Make the grid have Auto Auto 10 Auto 10 Auto
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Pixel) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Pixel) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    } else {
                        // Make the grid have Auto Auto 10 Auto
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Pixel) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    }

                    // Add the grade_appuser_name textblock if enabled
                    if (gradeShowsAppUser) {
                        TextBlock grade_appuser_name = new TextBlock {
                            Text = grade.GetAppUser(windowInstance.Shared.appDbContext).Name,
                            FontSize = 15,
                            Padding = new Thickness(3, 0, 3, 0)
                        };
                        Grid.SetColumn(grade_appuser_name, 0);
                        grid.Children.Add(grade_appuser_name);

                        // Add the divider rectangle #343434
                        Rectangle divider = new Rectangle {
                            Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                            Width = 2
                        };
                        Grid.SetColumn(divider, 1);
                        grid.Children.Add(divider);
                    }

                    // Add the grade value textbox/textblock
                    TextBlock grade_value = new TextBlock {
                        Text = grade.GetGradeAsString(),
                        FontSize = 15
                    };
                    if (isUsersGrade) {
                        grade_value.FontWeight = FontWeights.Bold;
                        grade_value.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    }
                    Grid.SetColumn(grade_value, gradeShowsAppUser ? 2 : 0);
                    grid.Children.Add(grade_value);

                    // Add the grade type textblock
                    TextBlock grade_type = new TextBlock {
                        Text = $" ({grade.GetGradeTypeName()})",
                        FontSize = 15
                    };
                    grade_type.Style = (Style)FindResource("GrayedOutTextBlock");
                    Grid.SetColumn(grade_type, gradeShowsAppUser ? 3 : 1);
                    grid.Children.Add(grade_type);

                    // Add the grade comment textblock if the comment is not empty
                    if (!string.IsNullOrEmpty(grade.Comment)) {

                        // Add the divider rectangle #343434
                        Rectangle divider2 = new Rectangle {
                            Fill = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                            Width = 2
                        };
                        Grid.SetColumn(divider2, gradeShowsAppUser ? 4 : 2);
                        grid.Children.Add(divider2);

                        // Add the grade comment textblock
                        TextBlock grade_comment = new TextBlock {
                            Text = grade.Comment,
                            FontSize = 15
                        };
                        Grid.SetColumn(grade_comment, gradeShowsAppUser ? 5 : 3);
                        grid.Children.Add(grade_comment);
                    }

                    // Add the grid to the border
                    border.Child = grid;

                    // Add the border to the stackpanel
                    stackpanel.Children.Add(border);
                }

                // For each category set the border thickness to 0 on the last element
                foreach (var category in gradeBordersPerCategory) {
                    category.Value.Last().BorderThickness = new Thickness(0, 0, 0, 0);
                }

                // Calculate the total average and display that inside GroupTotalGradeSpace
                int totalAverage = group.GetAverageGradeRounded(windowInstance.Shared.appDbContext);

                // Create the border
                Border totalBorder = new Border {
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
                };
                // Create the dockpanel
                DockPanel dockPanel = new DockPanel();
                // Create the title TextBlock
                TextBlock title = new TextBlock {
                    Text = "Total Average: ",
                    FontSize = 18
                };
                dockPanel.Children.Add(title);
                // Create the value TextBlock
                TextBlock value = new TextBlock {
                    Text = DbTableHelper.GetGradeAsString(edition.GradeType, totalAverage),
                    FontSize = 18
                };
                dockPanel.Children.Add(value);
                // Create the average TextBlock
                TextBlock average = new TextBlock {
                    Text = $" ({DbTableHelper.GetGradeTypeName(edition.GradeType)})",
                    FontSize = 18
                };
                average.Style = (Style)FindResource("GrayedOutTextBlock");
                dockPanel.Children.Add(average);
                totalBorder.Child = dockPanel;

                // Set the border as the content of GroupTotalGradeSpace
                GroupTotalGradeSpace.Content = totalBorder;
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
                if (PlayButtonPath.IsMouseOver || PlayButtonInnerPath.IsMouseOver) {
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
                } catch {
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
    }
}
