using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

    public class UITools_ParticipantsManager_Participant {
        public int ID { get; set; }
        public string Name { get; set; }

        public UITools_ParticipantsManager_Group? Parent;

        private RoutedEventHandler editButtonHandler;

        private TextBlock? textBlock;

        public UITools_ParticipantsManager_Participant(int ID, string Name, UITools_ParticipantsManager_Group Parent, RoutedEventHandler EditButtonHandler) {
            this.ID = ID;
            this.Name = Name;
            this.Parent = Parent;
            this.editButtonHandler = EditButtonHandler;
        }

        public Border GetElement() {
            /*
             * <Border>
             *     <TextBlock Text="<Name>" FontSize=15/>
             * </Border>
             */

            // Border
            Border border = new Border() {
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(0,5,0,5),
                Background = new SolidColorBrush(Color.FromRgb(44, 44, 44)),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                Width = 176,
                Height = 32
            };

            // Border > DockPanel
            DockPanel dockPanel = new DockPanel() {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            border.Child = dockPanel;

            // Border > DockPanel > TextBlock
            this.textBlock = new TextBlock() {
                Text = this.Name,
                FontSize = 15,
                Margin = new Thickness(10, 0, 0, 0)
            };
            dockPanel.Children.Add(this.textBlock);

            // Border > DockPanel > Button
            Button editButton = new Button() {
                Background = new SolidColorBrush(Color.FromRgb(53, 53, 53)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                IsEnabled = true,
                Focusable = true,
                IsHitTestVisible = true,
                Width = 25,
                Margin = new Thickness(10, 0, 0, 0)
            };
            editButton.Tag = this;
            editButton.Click += this.editButtonHandler;
            dockPanel.Children.Add(editButton);

            // Border > DockPanel > Button > Canvas
            Canvas outerCanvas = new Canvas() {
                Height = 25,
                Width = 17.5
            };
            ScaleTransform scaleTransform = new ScaleTransform() {
                ScaleX = 0.70,
                ScaleY = 0.70
            };
            outerCanvas.RenderTransform = scaleTransform;
            editButton.Content = outerCanvas;

            // Border > DockPanel > Button > Canvas > Canvas
            Canvas innerCanvas = new Canvas() {
                Margin = new Thickness(0, 1, 0, 0)
            };
            outerCanvas.Children.Add(innerCanvas);

            Path path = new Path() {
                Data = Geometry.Parse("M12 20H21M3.00003 20H4.67457C5.16376 20 5.40835 20 5.63852 19.9447C5.84259 19.8957 6.03768 19.8149 6.21663 19.7053C6.41846 19.5816 6.59141 19.4086 6.93732 19.0627L19.5001 6.49998C20.3285 5.67156 20.3285 4.32841 19.5001 3.49998C18.6716 2.67156 17.3285 2.67156 16.5001 3.49998L3.93729 16.0627C3.59139 16.4086 3.41843 16.5816 3.29475 16.7834C3.18509 16.9624 3.10428 17.1574 3.05529 17.3615C3.00003 17.5917 3.00003 17.8363 3.00003 18.3255V20Z"),
                Stroke = new SolidColorBrush(Color.FromRgb(211, 211, 211)),
                StrokeThickness = 2
            };
            innerCanvas.Children.Add(path);

            // Enable drag on the border element
            border.PreviewMouseDown += (sender, e) => {
                if (e.Source is Button) {
                    e.Handled = false;
                } else {
                    DragDrop.DoDragDrop(border, this, DragDropEffects.Move);
                }
            };

            return border;
        }

        public void UpdateText() {
            if (this.textBlock != null) {
                this.textBlock.Text = this.Name;
            }
        }
    }

    public class UITools_ParticipantsManager_Group {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<UITools_ParticipantsManager_Participant> Participants { get; set; }

        public UIElement? UIElement;

        public ListBox? ListBox;

        public UITools_ParticipantsManager_Group(int ID, string Name, List<UITools_ParticipantsManager_Participant> Participants) {
            this.ID = ID;
            this.Name = Name;
            this.Participants = Participants;
            this.UIElement = null;
        }

        public Border GetElement() {
            /*
             * <Border BorderThickness="1" BorderBrush="#323232" Margin="10">
             *     <Grid>
             *         <Grid.RowDefinitions>
             *             <RowDefinition Height="35"/>
             *             <RowDefinition Height="2"/>
             *             <RowDefinition Height="Auto"/>
             *         </Grid.RowDefinitions>
             *    
             *         <TextBlock Grid.Row="0" Text="Unassigned Participants" FontSize="16" Margin="10,0,0,0" VerticalAlignment="Center"/>
             * 
             *         <Rectangle Grid.Row="1" Fill="#282828"/>
             * 
             *         <ScrollViewer Grid.Row="2" VerticalScrollBarVisibility="Auto">
             *             <ListBox>   
             *         </ScrollViewer>
             *     </Grid>
             *</Border>
             */

            Border border = new Border() {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                Margin = new Thickness(10),
                Padding = new Thickness(2),
                Background = new SolidColorBrush(Color.FromRgb(44, 44, 44))
            };

            Grid grid = new Grid() {
                Width = 190
            };

            grid.RowDefinitions.Add(
                new RowDefinition() { Height = new GridLength(35) }
            );
            grid.RowDefinitions.Add(
                new RowDefinition() { Height = new GridLength(2) }
            );
            grid.RowDefinitions.Add(
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
            );

            TextBlock textBlock = new TextBlock() {
                Text = this.Name,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(textBlock);
            Grid.SetRow(textBlock, 0);

            Rectangle rectangle = new Rectangle() {
                Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40))
            };
            grid.Children.Add(rectangle);
            Grid.SetRow(rectangle, 1);

            ScrollViewer scrollViewer = new ScrollViewer() {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            grid.Children.Add(scrollViewer);
            Grid.SetRow(scrollViewer, 2);

            border.Child = grid;

            ListBox listBox = new ListBox() {
                MinHeight = 30,
                AllowDrop = true,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            // Assign the drop and drag-over event handlers
            listBox.Drop += ListBox_Drop;
            listBox.DragEnter += ListBox_DragEnter;

            // Link the StackPanel to the ScrollViewer
            scrollViewer.Content = listBox;

            // Link elements
            this.UIElement = border;
            this.ListBox = listBox;

            return border;
        }

        private void ListBox_Drop(object sender, DragEventArgs e) {
            var participant = e.Data.GetData(typeof(UITools_ParticipantsManager_Participant)) as UITools_ParticipantsManager_Participant;
            if (participant != null) {
                this.AddParticipant(participant);
            }
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(typeof(UITools_ParticipantsManager_Participant))) {
                e.Effects = DragDropEffects.Move;
            } else {
                e.Effects = DragDropEffects.None;
            }
        }

        public void UpdateParticipantElements() {
            if (this.UIElement == null || this.ListBox == null) {
                return;
            }
            // clear ListBox children
            this.ListBox.Items.Clear();
            foreach (UITools_ParticipantsManager_Participant participant in this.Participants) {
                this.ListBox.Items.Add(participant.GetElement());
            }
        }

        public void AddParticipant(UITools_ParticipantsManager_Participant participant) {
            if (participant.Parent != null) {
                participant.Parent.RemoveParticipant(participant);
            }

            participant.Parent = this;
            this.Participants.Add(participant);
            if (this.ListBox != null) {
                this.ListBox.Items.Add(participant.GetElement());
            }
        }

        public void RemoveParticipant(UITools_ParticipantsManager_Participant participant) {
            participant.Parent = null;
            this.Participants.Remove(participant);
            if (this.ListBox != null) {
                this.ListBox.Items.Remove(participant.GetElement());
            }
            this.UpdateParticipantElements();
        }
    }
}
