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

namespace GameOnSystem.Pages.Parts.Admin {
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : UserControl {

        private MainWindow windowInstance;
        private AdminView parentView;

        public Options(MainWindow windowInstance, AdminView parentView) {
            this.windowInstance = windowInstance;
            this.parentView = parentView;

            this.parentView.AdminSelectedTableTitle.Text = "Application options and metadata";

            InitializeComponent();

            List<DbTableModel_Option> options = windowInstance.Shared.appDbContext.GetOptions();

            // Sorts:
            // 1. meta_ fields
            // 2. no prefix fields
            // 3. opt_ fields
            // 4. ff_ fields
            // (Withing each category, original order is kept)
            options.Sort((x, y) => {
                int GetPriority(string field) {
                    if (field.StartsWith("meta_")) return 0;
                    if (field.StartsWith("opt_")) return 2;
                    if (field.StartsWith("ff_")) return 3;
                    return 1; // no special prefix
                }

                int prioX = GetPriority(x.Field);
                int prioY = GetPriority(y.Field);

                if (prioX != prioY) {
                    return prioX.CompareTo(prioY);
                } else {
                    return 0; // Keep original order within the same group
                }
            });

            // Add field names to OptionsKeys as TextBlock, and add values to OptionsValues as TextBox or CheckBok
            // If field name begins with ff_ in key repalce "ff_" with "" and make checkbox in values, rest is text-text
            foreach (DbTableModel_Option option in options) {
                if (!string.IsNullOrEmpty(option.Field)) {
                    // Feature flags
                    if (option.Field.StartsWith("ff_")) {
                        TextBlock key = new TextBlock() {
                            Text = "FeatureFlag:  " + option.Field.Replace("ff_", ""),
                            FontSize = 15,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        key.Style = (Style)FindResource("GrayedOutTextBlock");
                        OptionsKeys.Children.Add(key);


                        Rectangle rect = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsKeys.Children.Add(rect);

                        StackPanel horizontalValueWrapper = new StackPanel() {
                            Orientation = Orientation.Horizontal
                        };

                        CheckBox value = new CheckBox() {
                            IsChecked = option.Value == "true",
                            FontSize = 15,
                            Margin = new Thickness(10, 5, 0, 0)
                        };
                        horizontalValueWrapper.Children.Add(value);

                        TextBlock information = new TextBlock() {
                            Text = "",
                            FontSize = 13,
                            Margin = new Thickness(10, 1, 0, 0)
                        };

                        // Add event handler for checkbox to update the database
                        value.Checked += (s, e) => {
                            try {
                                windowInstance.Shared.appDbContext.SetOption(option.Field, "true");
                                information.Style = (Style)FindResource("GreenNoticeText");
                                information.Text = "Saved!";
                            } catch (Exception ex) {
                                information.Style = (Style)FindResource("RedNoticeText");
                                information.Text = "Error: " + ex.Message;
                            }
                            // Wait 1s and then clear the information
                            Task.Delay(1000).ContinueWith(_ => {
                                Dispatcher.Invoke(() => {
                                    information.Text = "";
                                });
                            });
                        };
                        value.Unchecked += (s, e) => {
                            try {
                                windowInstance.Shared.appDbContext.SetOption(option.Field, "false");
                                information.Style = (Style)FindResource("GreenNoticeText");
                                information.Text = "Saved!";
                            } catch (Exception ex) {
                                information.Style = (Style)FindResource("RedNoticeText");
                                information.Text = "Error: " + ex.Message;
                            }
                            // Wait 1s and then clear the information
                            Task.Delay(1000).ContinueWith(_ => {
                                Dispatcher.Invoke(() => {
                                    information.Text = "";
                                });
                            });
                        };

                        horizontalValueWrapper.Children.Add(information);

                        OptionsValues.Children.Add(horizontalValueWrapper);

                        Rectangle rect2 = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsValues.Children.Add(rect2);

                    }
                    // Options
                    else if (option.Field.StartsWith("opt_")) {

                        var dropdownOptionsMap = new Dictionary<string, string[]>() {
                            { "opt_group_grade_calculation", new string[] {
                                "average",
                                "avgs-of-category-avgs",
                                "average-rounded",
                                "avgs-of-category-avgs-rounded"
                            }}
                        };

                        TextBlock key = new TextBlock() {
                            Text = "Option:          " + option.Field.Replace("opt_", ""),
                            FontSize = 15,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        key.Style = (Style)FindResource("GrayedOutTextBlock");
                        OptionsKeys.Children.Add(key);

                        Rectangle rect = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsKeys.Children.Add(rect);

                        StackPanel valuePanel = new StackPanel() {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10, 0, 0, 0)
                        };

                        Button saveButton = new Button() {
                            Content = "Save",
                            Margin = new Thickness(10, 0, 0, 0)
                        };

                        TextBlock statusText = new TextBlock() {
                            Text = "",
                            FontSize = 13,
                            Margin = new Thickness(10, 1, 0, 0)
                        };

                        if (dropdownOptionsMap.ContainsKey(option.Field)) {
                            ComboBox dropdown = new ComboBox() {
                                FontSize = 15,
                                Width = 200
                            };

                            foreach (string choice in dropdownOptionsMap[option.Field]) {
                                dropdown.Items.Add(choice);
                            }

                            dropdown.SelectedItem = option.Value;

                            saveButton.Click += (s, e) => {
                                try {
                                    string selectedValue = dropdown.SelectedItem?.ToString() ?? "";
                                    windowInstance.Shared.appDbContext.SetOption(option.Field, selectedValue);
                                    statusText.Style = (Style)FindResource("GreenNoticeText");
                                    statusText.Text = "Saved!";
                                } catch (Exception ex) {
                                    statusText.Style = (Style)FindResource("RedNoticeText");
                                    statusText.Text = "Error: " + ex.Message;
                                }

                                Task.Delay(1000).ContinueWith(_ => {
                                    Dispatcher.Invoke(() => {
                                        statusText.Text = "";
                                    });
                                });
                            };

                            valuePanel.Children.Add(dropdown);
                        } else {
                            TextBox inputField = new TextBox() {
                                Text = option.Value,
                                FontSize = 15,
                                Width = 150
                            };

                            saveButton.Click += (s, e) => {
                                try {
                                    windowInstance.Shared.appDbContext.SetOption(option.Field, inputField.Text);
                                    statusText.Style = (Style)FindResource("GreenNoticeText");
                                    statusText.Text = "Saved!";
                                } catch (Exception ex) {
                                    statusText.Style = (Style)FindResource("RedNoticeText");
                                    statusText.Text = "Error: " + ex.Message;
                                }

                                Task.Delay(1000).ContinueWith(_ => {
                                    Dispatcher.Invoke(() => {
                                        statusText.Text = "";
                                    });
                                });
                            };

                            valuePanel.Children.Add(inputField);
                        }

                        valuePanel.Children.Add(saveButton);
                        valuePanel.Children.Add(statusText);
                        OptionsValues.Children.Add(valuePanel);

                        Rectangle rect2 = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsValues.Children.Add(rect2);
                    }
                    // Meta
                    else if (option.Field.StartsWith("meta_")) {
                        TextBlock key = new TextBlock() {
                            Text = "Meta:             " + option.Field.Replace("meta_", ""),
                            FontSize = 15,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        key.Style = (Style)FindResource("GrayedOutTextBlock");
                        OptionsKeys.Children.Add(key);


                        Rectangle rect = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsKeys.Children.Add(rect);

                        TextBlock value = new TextBlock() {
                            Text = option.Value,
                            FontSize = 15,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        value.Style = (Style)FindResource("GrayedOutTextBlock");
                        OptionsValues.Children.Add(value);

                        Rectangle rect2 = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsValues.Children.Add(rect2);
                    }
                    // Other
                    else {
                        TextBlock key = new TextBlock() {
                            Text = "DataBase:      " + option.Field,
                            FontSize = 15,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        key.Style = (Style)FindResource("GrayedOutTextBlock");
                        OptionsKeys.Children.Add(key);


                        Rectangle rect = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsKeys.Children.Add(rect);

                        TextBlock value = new TextBlock() {
                            Text = option.Value,
                            FontSize = 15,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        value.Style = (Style)FindResource("GrayedOutTextBlock");
                        OptionsValues.Children.Add(value);

                        Rectangle rect2 = new Rectangle() {
                            Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                            Height = 1,
                            Margin = new Thickness(0, 3, 0, 2)
                        };
                        OptionsValues.Children.Add(rect2);
                    }
                }
            }
        }
    }
}
