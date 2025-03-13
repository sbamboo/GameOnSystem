using Microsoft.EntityFrameworkCore;
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

namespace GameOnSystem.Pages.Parts {
    /// <summary>
    /// Interaction logic for AdminViewTable.xaml
    /// </summary>
    public partial class AdminViewTable : UserControl {

        private AdminView parentView;
        private MainWindow windowInstance;
        private Button senderButton;

        public AdminViewTable(MainWindow mainWindow, AdminView parentView, Button senderButton) {
            this.windowInstance = mainWindow;
            this.parentView = parentView;
            this.senderButton = senderButton;

            string tableName = (string)senderButton.Tag;

            InitializeComponent();

            parentView.AdminSelectedTableTitle.Text = tableName;

            switch (tableName) {
                // Options
                #region Options
                case "Options":
                    /*
                     * <ScrollViewer:Vertical>
                     *     <StackPanel:Vertical>
                     *         <DockPanel:Horizontal> // Field
                     *             <TextBlock>{Field}</TextBlock> 
                     *             <TextBlock>{Value}</TextBlock>
                     *         </DockPanel>
                     *         <DockPanel:Horizontal> // FeatureFlag (if field begins with ff_)
                     *             <TextBlock>{Field}</TextBlock>
                     *             <CheckBox>{Value}</CheckBox>
                     *         </DockPanel>
                     *     </StackPanel>
                     * </ScrollViewer>
                     */

                    // Create ScrollViewer
                    ScrollViewer scrollViewer = new ScrollViewer() {
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                    };

                    // Create StackPanel
                    StackPanel stackPanel = new StackPanel() {
                        Orientation = Orientation.Vertical
                    };
                    scrollViewer.Content = stackPanel;

                    // Iterate through DbSet<DbTableModel_Option>
                    foreach (DbTableModel_Option option in windowInstance.Shared.appDbContext.GetOptions()) {
                        // Create DockPanel
                        DockPanel dockPanel = new DockPanel() {
                            LastChildFill = true
                        };
                        // If field begins with ff_, create a CheckBox
                        if (option.Field.StartsWith("ff_")) {
                            TextBlock fieldTextBlock = new TextBlock() {
                                Text = option.Field.Replace("ff_", "FeatureFlag: "),
                                Width = 300,
                                Margin = new Thickness(0, 0, 0, 5)
                            };
                            fieldTextBlock.Style = (Style)FindResource("GrayedOutTextBlock");

                            CheckBox valueCheckBox = new CheckBox() {
                                IsChecked = option.Value == "true" ? true : false,
                                Tag = option.Field,
                                Margin = new Thickness(0, 0, 0, 5)
                            };
                            valueCheckBox.Checked += OptionsCheckBoxChanged;
                            valueCheckBox.Unchecked += OptionsCheckBoxChanged;
                            dockPanel.Children.Add(fieldTextBlock);
                            dockPanel.Children.Add(valueCheckBox);
                            stackPanel.Children.Add(dockPanel);
                        }
                        // Else, create a TextBlock for the value
                        else {
                            TextBlock fieldTextBlock = new TextBlock() {
                                Text = option.Field,
                                Width = 300,
                                Margin = new Thickness(0, 0, 0, 5)
                            };
                            fieldTextBlock.Style = (Style)FindResource("GrayedOutTextBlock");

                            TextBlock valueTextBlock = new TextBlock() {
                                Text = option.Value,
                                Margin = new Thickness(0, 0, 0, 5)
                            };
                            valueTextBlock.Style = (Style)FindResource("GrayedOutTextBlock");
                            dockPanel.Children.Add(fieldTextBlock);
                            dockPanel.Children.Add(valueTextBlock);
                            stackPanel.Children.Add(dockPanel);
                        }
                    }

                    // Add ScrollViewer to AdminTableViewWrapper
                    AdminTableViewWrapper.Children.Add(scrollViewer);

                    // Case break
                    break;
                #endregion

                // AppUsers
                #region AppUsers
                case "AppUsers":
                    break;
                #endregion

                // Editions
                #region Editions
                case "Editions":
                    break;
                #endregion

                // Categories
                #region Categories
                case "Categories":
                    break;
                #endregion

                // Groups
                #region Groups
                case "Groups":
                    break;
                #endregion

                // Participants
                #region Participants
                case "Participants":
                    break;
                #endregion

                // Grades
                #region Grades
                case "Grades":
                    break;
                #endregion
            }
        }
        private void OptionsCheckBoxChanged(object sender, RoutedEventArgs e) {
            bool isChecked = (bool)(((CheckBox)sender).IsChecked ?? false);
            string field = (string)((CheckBox)sender).Tag;

            if (isChecked) {
                windowInstance.Shared.appDbContext.SetOption(field, "true");
            } else {
                windowInstance.Shared.appDbContext.SetOption(field, "false");
            }
        }
    }
}
