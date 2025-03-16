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

            // Sort options to put feature flags last, rest are in the original order
            options.Sort((x, y) => {
                if (x.Field.StartsWith("ff_") && !y.Field.StartsWith("ff_")) {
                    return 1;
                } else if (!x.Field.StartsWith("ff_") && y.Field.StartsWith("ff_")) {
                    return -1;
                } else {
                    return 0;
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

                        CheckBox value = new CheckBox() {
                            IsChecked = option.Value == "true",
                            FontSize = 15,
                            Margin = new Thickness(10, 5, 0, 0)
                        };
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
                            Text = "DataBase:      "+option.Field,
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
