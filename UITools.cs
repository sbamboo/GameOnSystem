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
        private StackPanel outerStackPanel;
        private TextBlock categoryTitle;
        private ScrollViewer innerScrollViewer;
        private StackPanel innerStackPanel;

        public Border Border { get { return this.border; } }
        public StackPanel OuterStackPanel { get { return this.outerStackPanel; } }
        public TextBlock CategoryTitle { get { return this.categoryTitle; } }
        public ScrollViewer InnerScrollViewer { get { return this.innerScrollViewer; } }
        public StackPanel InnerStackPanel { get { return this.innerStackPanel; } }


        internal UITools_GroupGradeCategory(DbTableModel_Category Category) {
            this.category = Category;

            // Create the border
            this.border = new Border {
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(52, 52, 52))
            };

            // Create the outer stackpanel
            this.outerStackPanel = new StackPanel {
                Orientation = Orientation.Vertical
            };
            this.border.Child = this.outerStackPanel;

            // Create the category textblock
            this.categoryTitle = new TextBlock {
                Text = category.Name,
                FontSize = 16
            };
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
        }

    }

}
