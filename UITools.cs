using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOnSystem {

    public class UITools_GroupContentHolder {
        private Grid outerWrapper;
        private TextBlock noEntriesText;
        private ContentControl contentHolder;

        internal Grid OuterWrapper { get { return this.outerWrapper; } }
        internal TextBlock NoEntriesText { get { return this.noEntriesText; } }
        internal ContentControl ContentHolder { get { return this.contentHolder; } }

        public UITools_GroupContentHolder(Grid OuterWrapper, TextBlock NoEntriesText, ContentControl ContentHolder) {
            this.outerWrapper = OuterWrapper;
            this.noEntriesText = NoEntriesText;
            this.contentHolder = ContentHolder;
        }

        public void SetContent(UserControl content) {
            this.contentHolder.Content = content;
            this.noEntriesText.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void ClearContent() {
            this.contentHolder.Content = null;
            this.noEntriesText.Visibility = System.Windows.Visibility.Visible;
        }
    }

    public class UITools_GroupGradeCategory {
        private DbTableModel_Category category;
        private Border border;
        private ScrollViewer scrollViewer;
        private StackPanel outerStackPanel;
        private TextBlock categoryTitle;
        private ScrollViewer innerScrollViewer;
        private StackPanel innerStackPanel;
        private TextBlock informationText;

        public Border Border { get { return this.border; } }
        public ScrollViewer ScrollViewer { get { return this.scrollViewer; } }
        public StackPanel OuterStackPanel { get { return this.outerStackPanel; } }
        public TextBlock CategoryTitle { get { return this.categoryTitle; } }
        public ScrollViewer InnerScrollViewer { get { return this.innerScrollViewer; } }
        public StackPanel InnerStackPanel { get { return this.innerStackPanel; } }
        public TextBlock InformationText { get { return this.informationText; } }


        internal UITools_GroupGradeCategory(DbTableModel_Category Category) {
            this.category = Category;

            // Create the border
            this.border = new Border {
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52)),
                Margin = new System.Windows.Thickness(0, 0, 0, 15),
                Padding = new System.Windows.Thickness(2),
                // Background #2C2C2C
                Background = new SolidColorBrush(Color.FromRgb(44, 44, 44))
            };

            // Create the scrollviewer (horizontal)
            this.scrollViewer = new ScrollViewer {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            this.border.Child = this.scrollViewer;

            // Create the outer stackpanel
            this.outerStackPanel = new StackPanel {
                Orientation = Orientation.Vertical
            };
            this.scrollViewer.Content = this.outerStackPanel;

            // Create the category textblock
            this.categoryTitle = new TextBlock {
                Text = category.Name,
                FontSize = 16
            };
            this.categoryTitle.Style = (System.Windows.Style)System.Windows.Application.Current.FindResource("GradeCategoryTitle");
            this.outerStackPanel.Children.Add(this.categoryTitle);

            // Create the inner scrollviewer
            this.innerScrollViewer = new ScrollViewer {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            this.outerStackPanel.Children.Add(this.innerScrollViewer);

            // Create the inner stackpanel
            this.innerStackPanel = new StackPanel {
                Orientation = Orientation.Vertical
            };
            this.innerScrollViewer.Content = this.innerStackPanel;

            // Create the information textblock
            this.informationText = new TextBlock {
                Text = "",
                FontSize = 12,
                Visibility = System.Windows.Visibility.Collapsed
            };
            this.outerStackPanel.Children.Add(this.informationText);
        }
    }

    internal struct UITools_GroupGradeSaveButtonTag {
        public TextBlock buttonText;
        public int groupID;
        public int userCatID;
        public DbTableModel_Edition edition;
        public bool numValueIsEnabled;
        public TextBox valueBox;
        public TextBox commentBox;
        public UITools_GroupGradeCategory categoryUiTools;
    }
}
