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

namespace GameOnSystem {
    internal class EditionAdminEditor {
        private MainWindow WindowInstance;
        private Edition Edition;
        private StackPanel Parent;
        private StackPanel? UiElementIntance = null;
        private TextBlock? InfoTextBlock = null;
        internal void MarkInfoTextError() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Red style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Red"];
            }
        }
        internal void MarkInfoTextSuccess() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Green style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Green"];
            }
        }
        internal void MarkInfoTextNeutral() {
            if (this.InfoTextBlock != null) {
                // Remove style
                this.InfoTextBlock.Style = null;
            }
        }

        public EditionAdminEditor(MainWindow WindowInstance, Edition edition, StackPanel parent) {
            this.WindowInstance = WindowInstance;
            this.Edition = edition;
            this.Parent = parent;
        }

        private void UpdateData() {
            this.Edition = WindowInstance.DbContext.GetEdition(this.Edition.ID);
        }

        public void Instantiate() {
            List<object> tags = new List<object>() { Edition, this.Edition.ID };

            //StackPanel: ID |Name| |Theme| "Aktiv" [x] [Save] [Refresh] [Delete]
            StackPanel stackPanel = new StackPanel() {
                Name = $"edition_{this.Edition.ID}",
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = tags,
            };

            TextBlock idTextBlock = new TextBlock() {
                Text = this.Edition.ID.ToString(),
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "ID"
            };

            TextBox nameTextBox = new TextBox() {
                Text = this.Edition.Name,
                Width = 200,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Name"
            };

            TextBox themeTextBox = new TextBox() {
                Text = this.Edition.Theme,
                Width = 200,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Theme"
            };

            // IsActive Checkbox with label
            TextBlock isActiveTextBlock = new TextBlock() {
                Text = "Aktiv: "
            };
                CheckBox isActiveCheckBox = new CheckBox() {
                    IsChecked = this.Edition.IsActive,
                    Margin = new Thickness(0, 0, 10, 0),
                    Tag = "IsActive"
                };

            Button saveButton = new Button() {
                Content = "Save",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            saveButton.Click += this.Save;

            Button refreshButton = new Button() {
                Content = "Refresh",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            refreshButton.Click += this.Refresh;

            Button deleteButton = new Button() {
                Content = "Delete",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            deleteButton.Click += this.Delete;

            TextBlock infoTextBlock = new TextBlock() {
                Text = "",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Info"
            };

            this.InfoTextBlock = infoTextBlock;

            stackPanel.Children.Add(idTextBlock);
            stackPanel.Children.Add(nameTextBox);
            stackPanel.Children.Add(themeTextBox);
            stackPanel.Children.Add(isActiveTextBlock);
            stackPanel.Children.Add(isActiveCheckBox);
            stackPanel.Children.Add(saveButton);
            stackPanel.Children.Add(refreshButton);
            stackPanel.Children.Add(deleteButton);
            stackPanel.Children.Add(infoTextBlock);

            this.UiElementIntance = stackPanel;

            this.Parent.Children.Add(stackPanel);
        }

        private void Refresh(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Edition && senderId == this.Edition.ID) {
                if (this.InfoTextBlock != null) { this.InfoTextBlock.Text = ""; }
                this.MarkInfoTextNeutral();
                this.UpdateData();
                // Get parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Update the textboxes
                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "ID") {
                            textBox.Text = this.Edition.ID.ToString();
                        } else if ((string)textBox.Tag == "Name") {
                            textBox.Text = this.Edition.Name;
                        } else if ((string)textBox.Tag == "Theme") {
                            textBox.Text = this.Edition.Theme;
                        }
                    } else if (child is CheckBox checkBox) {
                        if ((string)checkBox.Tag == "IsActive") {
                            checkBox.IsChecked = this.Edition.IsActive;
                        }
                    }
                }
            }
        }
        private void Save(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Edition && senderId == this.Edition.ID) {
                // Get the parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Get the values of the textboxes into a Editon instance
                string? name = null;
                string? theme = null;
                bool? isActive = null;

                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "Name") {
                            name = textBox.Text;
                        } else if ((string)textBox.Tag == "Theme") {
                            theme = textBox.Text;
                        }
                    } else if (child is CheckBox checkBox) {
                        if ((string)checkBox.Tag == "IsActive") {
                            isActive = checkBox.IsChecked;
                        }
                    }
                }

                bool success = this.WindowInstance.DbContext.ModifyEdition(this.Edition.ID, name, theme, isActive);
                if (success == false) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Failed to save!";
                        this.MarkInfoTextError();
                    }
                } else {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Saved!";
                        this.MarkInfoTextSuccess();
                    }
                }
            }
        }
        private void Delete(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Edition && senderId == this.Edition.ID) {
                bool success = this.WindowInstance.DbContext.RemoveEdition(this.Edition.ID);
                if (success == false) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Failed to delete!";
                        this.MarkInfoTextError();
                    }
                } else {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Deleted!";
                        this.MarkInfoTextSuccess();
                    }
                }

                // Remove the UI element
                if (this.UiElementIntance != null) {
                    this.Parent.Children.Remove(this.UiElementIntance);
                }
            }
        }
    }

    internal class CategoryAdminEditor {
        private MainWindow WindowInstance;
        private Category Category;
        private StackPanel Parent;
        private StackPanel? UiElementIntance = null;
        private TextBlock? InfoTextBlock = null;
        internal void MarkInfoTextError() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Red style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Red"];
            }
        }
        internal void MarkInfoTextSuccess() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Green style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Green"];
            }
        }
        internal void MarkInfoTextNeutral() {
            if (this.InfoTextBlock != null) {
                // Remove style
                this.InfoTextBlock.Style = null;
            }
        }

        public CategoryAdminEditor(MainWindow WindowInstance, Category category, StackPanel parent) {
            this.WindowInstance = WindowInstance;
            this.Category = category;
            this.Parent = parent;
        }

        private void UpdateData() {
            this.Category = WindowInstance.DbContext.GetCategory(this.Category.ID);
        }

        public void Instantiate() {
            List<object> tags = new List<object>() { Category, this.Category.ID };

            //StackPanel: ID |Name| |CategoryType| [Save] [Refresh] [Delete]
            StackPanel stackPanel = new StackPanel() {
                Name = $"category_{this.Category.ID}",
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = tags,
            };

            TextBlock idTextBlock = new TextBlock() {
                Text = this.Category.ID.ToString(),
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "ID"
            };

            TextBox nameTextBox = new TextBox() {
                Text = this.Category.Name,
                Width = 300,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Name"
            };

            TextBox typeTextBox = new TextBox() {
                Text = this.Category.CategoryType,
                Width = 75,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "CategoryType"
            };

            Button saveButton = new Button() {
                Content = "Save",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            saveButton.Click += this.Save;

            Button refreshButton = new Button() {
                Content = "Refresh",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            refreshButton.Click += this.Refresh;

            Button deleteButton = new Button() {
                Content = "Delete",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            deleteButton.Click += this.Delete;

            TextBlock infoTextBlock = new TextBlock() {
                Text = "",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Info"
            };

            this.InfoTextBlock = infoTextBlock;

            stackPanel.Children.Add(idTextBlock);
            stackPanel.Children.Add(nameTextBox);
            stackPanel.Children.Add(typeTextBox);
            stackPanel.Children.Add(saveButton);
            stackPanel.Children.Add(refreshButton);
            stackPanel.Children.Add(deleteButton);
            stackPanel.Children.Add(infoTextBlock);

            this.UiElementIntance = stackPanel;

            this.Parent.Children.Add(stackPanel);
        }

        private void Refresh(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Category && senderId == this.Category.ID) {
                if (this.InfoTextBlock != null) { this.InfoTextBlock.Text = ""; }
                this.MarkInfoTextNeutral();
                this.UpdateData();
                // Get parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Update the textboxes
                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "ID") {
                            textBox.Text = this.Category.ID.ToString();
                        } else if ((string)textBox.Tag == "Name") {
                            textBox.Text = this.Category.Name;
                        } else if ((string)textBox.Tag == "CategoryType") {
                            textBox.Text = this.Category.CategoryType;
                        }
                    }
                }
            }
        }
        private void Save(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Category && senderId == this.Category.ID) {
                // Get the parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Get the values of the textboxes into a Editon instance
                string? name = null;
                string? categoryType = null;

                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "Name") {
                            name = textBox.Text;
                        } else if ((string)textBox.Tag == "CategoryType") {
                            categoryType = textBox.Text;
                        }
                    }
                }

                // Validate categoryType
                bool catTypeIsValid = this.WindowInstance.DbContext.IsAllowedCategoryType(categoryType);
                if (catTypeIsValid == false) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "CategoryType is not valid!";
                        this.MarkInfoTextError();
                    }
                } else {
                    bool success = this.WindowInstance.DbContext.ModifyCategory(this.Category.ID, name, categoryType);
                    if (success == false) {
                        if (this.InfoTextBlock != null) {
                            this.InfoTextBlock.Text = "Failed to save!";
                        }
                        this.MarkInfoTextError();
                    } else {
                        if (this.InfoTextBlock != null) {
                            this.InfoTextBlock.Text = "Saved!";
                        }
                        this.MarkInfoTextSuccess();
                    }
                }
            }
        }
        private void Delete(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == Category && senderId == this.Category.ID) {
                bool success = this.WindowInstance.DbContext.RemoveCategory(this.Category.ID);
                if (success == false) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Failed to delete!";
                        this.MarkInfoTextError();
                    }
                } else {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Deleted!";
                        this.MarkInfoTextSuccess();
                    }
                }

                // Remove the UI element
                if (this.UiElementIntance != null) {
                    this.Parent.Children.Remove(this.UiElementIntance);
                }
            }
        }
    }

    internal class UserAdminEditor {
        private MainWindow WindowInstance;
        private User User;
        private StackPanel Parent;
        private StackPanel? UiElementIntance = null;
        private TextBlock? InfoTextBlock = null;
        private StackPanel? FocusCategoriesExisting = null;
        internal void MarkInfoTextError() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Red style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Red"];
            }
        }
        internal void MarkInfoTextSuccess() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Green style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Green"];
            }
        }
        internal void MarkInfoTextNeutral() {
            if (this.InfoTextBlock != null) {
                // Remove style
                this.InfoTextBlock.Style = null;
            }
        }

        public UserAdminEditor(MainWindow WindowInstance, User user, StackPanel parent) {
            this.WindowInstance = WindowInstance;
            this.User = user;
            this.Parent = parent;
        }

        private void UpdateData() {
            User user = WindowInstance.DbContext.GetUser(this.User.ID);
        }

        public void Instantiate() {
            List<object> tags = new List<object>() { User, this.User.ID };

            //StackPanel: ID |Name| |Email| |Password| "IsAdmin" [x] [Save] [Refresh] [Delete]
            //               FucusCategories: |...| [-]   |...| [-]
            //                                |...| [Add]
            StackPanel stackPanel = new StackPanel() {
                Name = $"user_{this.User.ID}",
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = tags,
            };

            StackPanel innerStackPanel = new StackPanel() {
                Orientation = Orientation.Horizontal,
                Tag = "InnerStackPanel"
            };

                // FocusCategories
                StackPanel focusCategoriesOuterOuterWrapper = new StackPanel() {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 10),
                    Tag = "FocusCategoriesOuterOuterWrapper"
                };

                    TextBlock focusCategoriesOuterTitle = new TextBlock() {
                        Text = "FocusCategories: ",
                        Margin = new Thickness(30, 0, 10, 0),
                        Tag = "FocusCategoriesOuterTitle"
                    };

                        StackPanel focusCategoriesOuterWrapper = new StackPanel() {
                            Orientation = Orientation.Vertical,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "FocusCategoriesOuterWrapper"
                        };

                            StackPanel focusCategoriesExisting = new StackPanel() {
                                Orientation = Orientation.Vertical,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = "FocusCategoriesExisting"
                            };

                            StackPanel focusCategoriesNew = new StackPanel() {
                                Orientation = Orientation.Horizontal,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = "FocusCategoriesNew"
                            };

                            TextBox focusCategoriesNewCatId = new TextBox() {
                                Width = 20,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = "FocusCategoriesNewCatId"
                            };

                            Button focusCategoriesNewAddButton = new Button() {
                                Content = "Add",
                                Width = 30,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = tags
                            };
                            focusCategoriesNewAddButton.Click += this.FocusCategoryaddCatEvent;

                    TextBlock focusCategoriesInfoTextBlock = new TextBlock() {
                        Text = "(Categories are saved on change)",
                        Tag = "FocusCategoriesInfo",
                        Style = (Style)Application.Current.Resources["GrayedOut"]
                    };

            TextBlock idTextBlock = new TextBlock() {
                Text = this.User.ID.ToString(),
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "ID"
            };

            TextBox nameTextBox = new TextBox() {
                Text = this.User.Name,
                Width = 150,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Name"
            };

            TextBox emailTextBox = new TextBox() {
                Text = this.User.Email,
                Width = 200,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Email"
            };

            TextBox passwordTextBox = new TextBox() {
                Text = this.User.Password,
                Width = 150,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Password"
            };

            // IsAdmin Checkbox with label
            TextBlock isAdminTextBlock = new TextBlock() {
                Text = "IsAdmin: "
            };
            CheckBox isAdminCheckBox = new CheckBox() {
                IsChecked = this.User.IsAdmin,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "IsAdmin"
            };

            Button saveButton = new Button() {
                Content = "Save",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            saveButton.Click += this.Save;

            Button refreshButton = new Button() {
                Content = "Refresh",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            refreshButton.Click += this.Refresh;

            Button deleteButton = new Button() {
                Content = "Delete",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            deleteButton.Click += this.Delete;

            TextBlock infoTextBlock = new TextBlock() {
                Text = "",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Info"
            };

            this.InfoTextBlock = infoTextBlock;

            this.FocusCategoriesExisting = focusCategoriesExisting;

                innerStackPanel.Children.Add(idTextBlock);
                innerStackPanel.Children.Add(nameTextBox);
                innerStackPanel.Children.Add(emailTextBox);
                innerStackPanel.Children.Add(passwordTextBox);
                innerStackPanel.Children.Add(isAdminTextBlock);
                innerStackPanel.Children.Add(isAdminCheckBox);
                innerStackPanel.Children.Add(saveButton);
                innerStackPanel.Children.Add(refreshButton);
                innerStackPanel.Children.Add(deleteButton);
                innerStackPanel.Children.Add(infoTextBlock);

            stackPanel.Children.Add(innerStackPanel);
                
                focusCategoriesOuterOuterWrapper.Children.Add(focusCategoriesOuterTitle);

                    focusCategoriesOuterWrapper.Children.Add(focusCategoriesExisting);

                        focusCategoriesNew.Children.Add(focusCategoriesNewCatId);
                        focusCategoriesNew.Children.Add(focusCategoriesNewAddButton);

                    focusCategoriesOuterWrapper.Children.Add(focusCategoriesNew);
                
                focusCategoriesOuterOuterWrapper.Children.Add(focusCategoriesOuterWrapper);

                focusCategoriesOuterOuterWrapper.Children.Add(focusCategoriesInfoTextBlock);

            stackPanel.Children.Add(focusCategoriesOuterOuterWrapper);

            this.UiElementIntance = stackPanel;

            this.Parent.Children.Add(stackPanel);

            UpdateFocusCategories();
        }

        private void Refresh(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == User && senderId == this.User.ID) {
                if (this.InfoTextBlock != null) { this.InfoTextBlock.Text = ""; }
                this.MarkInfoTextNeutral();
                this.UpdateData();

                // Get parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Update the textboxes
                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "ID") {
                            textBox.Text = this.User.ID.ToString();
                        } else if ((string)textBox.Tag == "Name") {
                            textBox.Text = this.User.Name;
                        } else if ((string)textBox.Tag == "Email") {
                            textBox.Text = this.User.Email;
                        } else if ((string)textBox.Tag == "Password") {
                            textBox.Text = this.User.Password;
                        }
                    } else if (child is CheckBox checkBox) {
                        if ((string)checkBox.Tag == "IsAdmin") {
                            checkBox.IsChecked = this.User.IsAdmin;
                        }

                    }
                }

                this.UpdateFocusCategories();
            }
        }
        private void Save(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == User && senderId == this.User.ID) {
                // Get the parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Get the values of the textboxes into a Editon instance
                string? name = null;
                string? categoryType = null;

                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "Name") {
                            name = textBox.Text;
                        } else if ((string)textBox.Tag == "Email") {
                            categoryType = textBox.Text;
                        } else if ((string)textBox.Tag == "Password") {
                            categoryType = textBox.Text;
                        }
                    } else if (child is CheckBox checkBox) {
                        if ((string)checkBox.Tag == "IsAdmin") {
                            categoryType = checkBox.IsChecked.ToString();
                        }
                    }
                }

            }
        }
        private void Delete(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == User && senderId == this.User.ID) {

                if (this.WindowInstance.DbContext.GetUserIsProtected(this.User.ID)) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "User is protected!";
                        this.MarkInfoTextError();
                        return;
                    }
                }

                bool success = this.WindowInstance.DbContext.RemoveUser(this.User.ID);
                if (success == false) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Failed to delete!";
                        this.MarkInfoTextError();
                    }
                } else {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Deleted!";
                        this.MarkInfoTextSuccess();
                    }
                }

                // Remove the UI element
                if (this.UiElementIntance != null) {
                    this.Parent.Children.Remove(this.UiElementIntance);
                }
            }
        }

        private void UpdateFocusCategories() {
            if (this.FocusCategoriesExisting != null) {
                this.FocusCategoriesExisting.Children.Clear();

                List<Category> focusCategories = this.WindowInstance.DbContext.GetCategoriesForUser(this.User.ID);
                foreach (var category in focusCategories) {
                    // |...| [-]
                    StackPanel focusCategory = new StackPanel() {
                        Orientation = Orientation.Horizontal,
                        Tag = $"usercat_{category.ID}"
                    };

                    TextBlock focusCategoryId = new TextBlock() {
                        Text = category.ID.ToString(),
                        Width = 30,
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "FocusCategoryId",
                        //Style = (Style)Application.Current.Resources["GrayedOut"]
                    };

                    Button focusCategoryRemButton = new Button() {
                        Content = "-",
                        Width = 20,
                        Tag = category.ID
                    };
                    focusCategoryRemButton.Click += this.FocusCategoryRemCatEvent;

                    focusCategory.Children.Add(focusCategoryId);
                    focusCategory.Children.Add(focusCategoryRemButton);
                    this.FocusCategoriesExisting.Children.Add(focusCategory);
                }
            }
        }

        private void FocusCategoryaddCatEvent(object sender, RoutedEventArgs e) {
            List<object> tags = (List<object>)((Button)sender).Tag;
            object senderType = tags[0];
            int senderId = (int)tags[1];

            if (senderType == User && senderId == this.User.ID) {
                // Get sibling that is TextBox
                foreach (UIElement child in ((StackPanel)((Button)sender).Parent).Children ) {
                    if (child is TextBox) {
                        int categoryId = -1;
                        try {
                            categoryId = int.Parse(((TextBox)child).Text);
                        } catch {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Category ID must be a number!";
                                this.MarkInfoTextError();
                            }
                        }

                        // Add category then UpdateFocusCategories()
                        bool success = this.WindowInstance.DbContext.AddCategoryForUser(this.User.ID, categoryId);
                        if (success) {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Category added!";
                                this.MarkInfoTextSuccess();
                            }
                        } else {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Failed to add category!";
                                this.MarkInfoTextError();
                            }
                        }

                        this.UpdateFocusCategories();
                    }
                }
            }
        }

        private void FocusCategoryRemCatEvent(object sender, RoutedEventArgs e) {
            // Check so parent of sender is StackPanel with tag begining with "usercat_"
            if ( ((Button)sender).Parent is StackPanel ) {
                // Check starts with "usercat_"
                if ( ((string)((StackPanel)((Button)sender).Parent).Tag).StartsWith("usercat_") ) {
                    if ( ((Button)sender).Tag is int ) {
                        int categoryId = (int)((Button)sender).Tag;
                        // Remove the category then UpdateFocusCategories()
                        bool success = this.WindowInstance.DbContext.RemoveCategoryForUser(this.User.ID, categoryId);
                        if (success) {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Category removed!";
                                this.MarkInfoTextSuccess();
                            }
                        } else {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Failed to remove category!";
                                this.MarkInfoTextError();
                            }
                        }
                        this.UpdateFocusCategories();
                    }
                }
            }
        }
    }

    internal class GroupAdminEditor {
        private MainWindow WindowInstance;
        private ResolvedGroup Group;
        private StackPanel Parent;
        private StackPanel? UiElementIntance = null;
        private TextBlock? InfoTextBlock = null;

        private StackPanel? GroupMembersExistingWrapper = null;
        private StackPanel? GroupGradesExistingWrapper = null;

        internal void MarkInfoTextError() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Red style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Red"];
            }
        }
        internal void MarkInfoTextSuccess() {
            if (this.InfoTextBlock != null) {
                // AdminViewInfo_Green style static resource, get resource in App.xaml where we have <ResourceDictionary Source="Styles.xaml"/>
                this.InfoTextBlock.Style = (Style)Application.Current.Resources["AdminViewInfo_Green"];
            }
        }
        internal void MarkInfoTextNeutral() {
            if (this.InfoTextBlock != null) {
                // Remove style
                this.InfoTextBlock.Style = null;
            }
        }

        public GroupAdminEditor(MainWindow WindowInstance, ResolvedGroup group, StackPanel parent) {
            this.WindowInstance = WindowInstance;
            this.Group = group;
            this.Parent = parent;
        }

        private void UpdateData() {
            this.Group = WindowInstance.DbContext.GetResolvedGroup(this.Group.ID);
        }

        public void Instantiate() {
            List<object> tags = new List<object>() { typeof(ResolvedGroup), this.Group.ID };

            //StackPanel: ID |Name| |GameName| |EditionId| [Save] [Refresh] [Delete]
            //               |GameUrl|
            //               |GameBannerUrl|
            //               Members: |Name| [Save] [-] (Members are saved on change)
            //                        |Name| [Add]
            //               Grades:  |Value| |UserId| |CategoryId| [Save] [-]   (Grades are saved on change)
            //                        |Value| |UserId| |CategoryId| [Add]

            StackPanel stackPanel = new StackPanel() {
                Name = $"group_{this.Group.ID}",
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = tags,
            };

            StackPanel innerStackPanel = new StackPanel() {
                Orientation = Orientation.Horizontal,
                Tag = "InnerStackPanel"
            };

            TextBlock idTextBlock = new TextBlock() {
                Text = this.Group.ID.ToString(),
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "ID"
            };

            TextBox nameTextBox = new TextBox() {
                Text = this.Group.Name,
                Width = 150,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Name"
            };

            TextBox gameNameTextBox = new TextBox() {
                Text = this.Group.GameName,
                Width = 150,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "GameName"
            };

            TextBox editionIdTextBox = new TextBox() {
                Text = this.Group.Edition.ID.ToString(),
                Width = 50,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "EditionId"
            };

            StackPanel lowerFieldsWrapper = new StackPanel() {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "LowerFieldsWrapper",
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 550
            };

                TextBox gameUrlTextBox = new TextBox() {
                    Text = this.Group.GameUrl,
                    Width = 300,
                    Tag = "GameUrl",
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBox gameBannerUrlTextBox = new TextBox() {
                    Text = this.Group.GameBannerUrl,
                    Width = 300,
                    Tag = "GameBannerUrl",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10),
                };

                StackPanel membersSegmentWrapper = new StackPanel() {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 10),
                    Tag = "MembersSegmentWrapper",
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                    TextBlock membersSegmentTitle = new TextBlock() {
                        Text = "Members: ",
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "MembersSegmentTitle"
                    };

                    StackPanel membersSegmentInnerWrapper = new StackPanel() {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "MembersSegmentInnerWrapper"
                    };

                        StackPanel membersSegmentExisting = new StackPanel() {
                            Orientation = Orientation.Vertical,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "MembersSegmentExisting"
                        };

                        StackPanel membersSegmentNew = new StackPanel() {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "MembersSegmentNew"
                        };

                            TextBox membersSegmentNewGroupMemberName = new TextBox() {
                                Width = 150,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = "MembersSegmentNewGroupMemberName"
                            };

                            Button membersSegmentNewAddButton = new Button() {
                                Content = "Add",
                                Width = 30,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = tags
                            };

                        TextBlock membersSegmentInfo = new TextBlock() {
                            Text = "(Members are saved on change)",
                            Tag = "MembersSegmentInfo",
                            Style = (Style)Application.Current.Resources["GrayedOut"]
                        };

                StackPanel gradesSegmentWrapper = new StackPanel() {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 10),
                    Tag = "GradesSegmentWrapper",
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                    TextBlock gradesSegmentTitle = new TextBlock() {
                        Text = "Grades: ",
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "GradesSegmentTitle"
                    };

                    StackPanel gradesSegmentInnerWrapper = new StackPanel() {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "GradesSegmentInnerWrapper"
                    };

                        StackPanel gradesSegmentExisting = new StackPanel() {
                            Orientation = Orientation.Vertical,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "GradesSegmentExisting"
                        };

                        StackPanel groupSegmentNew = new StackPanel() {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "GroupSegmentNew"
                        };

                        TextBox groupSegmentNewValue = new TextBox() {
                            Width = 40,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "GroupSegmentNewValue"
                        };
                            TextBox groupSegmentNewUserId = new TextBox() {
                                Width = 20,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = "GroupSegmentNewUserId"
                            };
                            TextBox groupSegmentNewCategoryId = new TextBox() {
                                Width = 20,
                                Margin = new Thickness(0, 0, 10, 0),
                                Tag = "GroupSegmentNewCategoryId"
                            };
                            Button groupSegmentNewAddButton = new Button() {
                                Content = "Add",
                                Width = 30,
                                Tag = tags
                            };

                    TextBlock gradesSegmentInfo = new TextBlock() {
                        Text = "(Grades are saved on change)",
                        Tag = "GradesSegmentInfo",
                        Style = (Style)Application.Current.Resources["GrayedOut"]
                    };

            Button saveButton = new Button() {
                Content = "Save",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            saveButton.Click += this.Save;

            Button refreshButton = new Button() {
                Content = "Refresh",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            refreshButton.Click += this.Refresh;

            Button deleteButton = new Button() {
                Content = "Delete",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = tags,
            };
            deleteButton.Click += this.Delete;

            TextBlock infoTextBlock = new TextBlock() {
                Text = "",
                Margin = new Thickness(0, 0, 10, 0),
                Tag = "Info"
            };

            this.InfoTextBlock = infoTextBlock;
            this.GroupMembersExistingWrapper = membersSegmentExisting;
            this.GroupGradesExistingWrapper = gradesSegmentExisting;

            innerStackPanel.Children.Add(idTextBlock);
            innerStackPanel.Children.Add(nameTextBox);
            innerStackPanel.Children.Add(gameNameTextBox);
            innerStackPanel.Children.Add(editionIdTextBox);
            innerStackPanel.Children.Add(saveButton);
            innerStackPanel.Children.Add(refreshButton);
            innerStackPanel.Children.Add(deleteButton);
            innerStackPanel.Children.Add(infoTextBlock);

            stackPanel.Children.Add(innerStackPanel);
                lowerFieldsWrapper.Children.Add(gameUrlTextBox);
                lowerFieldsWrapper.Children.Add(gameBannerUrlTextBox);

                    membersSegmentWrapper.Children.Add(membersSegmentTitle);
                        membersSegmentInnerWrapper.Children.Add(membersSegmentExisting);
                            membersSegmentNew.Children.Add(membersSegmentNewGroupMemberName);
                            membersSegmentNew.Children.Add(membersSegmentNewAddButton);
                        membersSegmentInnerWrapper.Children.Add(membersSegmentNew);
                    membersSegmentWrapper.Children.Add(membersSegmentInnerWrapper);
                    membersSegmentWrapper.Children.Add(membersSegmentInfo);
                lowerFieldsWrapper.Children.Add(membersSegmentWrapper);

                    gradesSegmentWrapper.Children.Add(gradesSegmentTitle);
                        gradesSegmentInnerWrapper.Children.Add(gradesSegmentExisting);
                            groupSegmentNew.Children.Add(groupSegmentNewValue);
                            groupSegmentNew.Children.Add(groupSegmentNewUserId);
                            groupSegmentNew.Children.Add(groupSegmentNewCategoryId);
                            groupSegmentNew.Children.Add(groupSegmentNewAddButton);
                        gradesSegmentInnerWrapper.Children.Add(groupSegmentNew);
                    gradesSegmentWrapper.Children.Add(gradesSegmentInnerWrapper);
                    gradesSegmentWrapper.Children.Add(gradesSegmentInfo);   
                lowerFieldsWrapper.Children.Add(gradesSegmentWrapper);

            stackPanel.Children.Add(lowerFieldsWrapper);

            this.UiElementIntance = stackPanel;

            this.Parent.Children.Add(stackPanel);

            this.UpdateGroupMembersSegment();
            this.UpdateGroupGradesSegment();
        }

        private void Refresh(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            Type senderType = (Type)tags[0];
            int senderId = (int)tags[1];

            if (senderType == typeof(ResolvedGroup) && senderId == this.Group.ID) {
                if (this.InfoTextBlock != null) { this.InfoTextBlock.Text = ""; }
                this.MarkInfoTextNeutral();
                this.UpdateData();

                // Get parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Update the textboxes
                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                        if ((string)textBox.Tag == "ID") {
                            textBox.Text = this.Group.ID.ToString();
                        } else if ((string)textBox.Tag == "Name") {
                            textBox.Text = this.Group.Name;
                        } else if ((string)textBox.Tag == "GameName") {
                            textBox.Text = this.Group.GameName;
                        } else if ((string)textBox.Tag == "EditionId") {
                            textBox.Text = this.Group.Edition.ID.ToString();
                        } else if ((string)textBox.Tag == "GameUrl") {
                            textBox.Text = this.Group.GameUrl;
                        } else if ((string)textBox.Tag == "GameBannerUrl") {
                            textBox.Text = this.Group.GameBannerUrl;
                        }
                    }
                }

                this.UpdateGroupMembersSegment();
                this.UpdateGroupGradesSegment();
            }
        }
        private void Save(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            Type senderType = (Type)tags[0];
            int senderId = (int)tags[1];

            if (senderType == typeof(ResolvedGroup) && senderId == this.Group.ID) {
                // Get the parent of the sender
                StackPanel parent = (StackPanel)((Button)sender).Parent;
                // Get the values of the textboxes into a Editon instance
                string? name = null;
                string? categoryType = null;

                foreach (var child in parent.Children) {
                    if (child is TextBox textBox) {
                    }
                }
            }
        }
        private void Delete(object sender, RoutedEventArgs e) {
            // Update the textboxes
            List<object> tags = (List<object>)((Button)sender).Tag;
            Type senderType = (Type)tags[0];
            int senderId = (int)tags[1];

            if (senderType == typeof(ResolvedGroup) && senderId == this.Group.ID) {
                bool success = this.WindowInstance.DbContext.RemoveGroupAndLinkedEntries(this.Group.ID);
                if (success == false) {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Failed to delete!";
                        this.MarkInfoTextError();
                    }
                } else {
                    if (this.InfoTextBlock != null) {
                        this.InfoTextBlock.Text = "Deleted!";
                        this.MarkInfoTextSuccess();
                    }
                }

                // Remove the UI element
                if (this.UiElementIntance != null) {
                    this.Parent.Children.Remove(this.UiElementIntance);
                }
            }
        }

        private void UpdateGroupMembersSegment() {
            if (this.GroupMembersExistingWrapper != null) {
                this.GroupMembersExistingWrapper.Children.Clear();

                this.UpdateData();

                List<GroupMember> groupMembers = this.Group.GroupMembers;
                foreach (var groupMember in groupMembers) {
                    // |...| [-]
                    StackPanel groupMemberWrapper = new StackPanel() {
                        Orientation = Orientation.Horizontal,
                        Tag = $"groupmember_{groupMember.ID}"
                    };

                        TextBlock groupMemberId = new TextBlock() {
                            Text = groupMember.ID.ToString(),
                            Width = 30,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "GroupMemberId",
                            //Style = (Style)Application.Current.Resources["GrayedOut"]
                        };

                        TextBlock groupMemberName = new TextBlock() {
                            Text = groupMember.Name,
                            Width = 150,
                            Margin = new Thickness(0, 0, 10, 0),
                            Tag = "GroupMemberName",
                            //Style = (Style)Application.Current.Resources["GrayedOut"]
                        };

                        Button groupMemberRemButton = new Button() {
                            Content = "-",
                            Width = 20,
                            Tag = groupMember.ID
                        };
                        groupMemberRemButton.Click += this.GroupMemberRemMemberEvent;

                    groupMemberWrapper.Children.Add(groupMemberId);
                    groupMemberWrapper.Children.Add(groupMemberName);
                    groupMemberWrapper.Children.Add(groupMemberRemButton);
                    this.GroupMembersExistingWrapper.Children.Add(groupMemberWrapper);
                }
            }
        }
        private void GroupMemberRemMemberEvent(object sender, RoutedEventArgs e) {
            // Check so parent of sender is StackPanel with tag begining with "groupmember_"
            if (((Button)sender).Parent is StackPanel) {
                // Check starts with "groupmember_"
                if (((string)((StackPanel)((Button)sender).Parent).Tag).StartsWith("groupmember_")) {
                    if (((Button)sender).Tag is int) {
                        int groupMemberId = (int)((Button)sender).Tag;
                        bool success = WindowInstance.DbContext.RemoveGroupMemberEntry(groupMemberId);
                        if (success) {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Group member removed!";
                                this.MarkInfoTextSuccess();
                            }
                        } else {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Failed to remove group member!";
                                this.MarkInfoTextError();
                            }
                        }
                        this.UpdateGroupMembersSegment();
                    }
                }
            }
        }
        private void GroupMemberAddMemberEvent(object sender, RoutedEventArgs e) {
            List<object> tags = (List<object>)((Button)sender).Tag;
            Type senderType = (Type)tags[0];
            int senderId = (int)tags[1];

            if (senderType == typeof(ResolvedGroup) && senderId == this.Group.ID) {
                int groupMemberId = -1;
                string groupMemberName = "";
                // Get sibling that is TextBox
                foreach (UIElement child in ((StackPanel)((Button)sender).Parent).Children) {
                    if (child is TextBox) {
                        if ((string)((TextBox)child).Tag == "MembersSegmentNewGroupMemberId") {
                            try {
                                groupMemberId = int.Parse(((TextBox)child).Text);
                            } catch {
                                if (this.InfoTextBlock != null) {
                                    this.InfoTextBlock.Text = "Group member ID must be a number!";
                                    this.MarkInfoTextError();
                                }
                            }
                        } else if ((string)((TextBox)child).Tag == "MembersSegmentNewGroupMemberName") {
                            groupMemberName = ((TextBox)child).Text;
                        }
                    }
                }

                // Add group member then UpdateGroupMembersSegment()
            }
        }

        private void UpdateGroupGradesSegment() {
            if (this.GroupGradesExistingWrapper != null) {
                this.GroupGradesExistingWrapper.Children.Clear();

                this.UpdateData();

                List<ResolvedGrade> groupGrades = this.Group.Grades;
                foreach (var groupGrade in groupGrades) {
                    // |...| [-]
                    StackPanel groupGradeWrapper = new StackPanel() {
                        Orientation = Orientation.Horizontal,
                        Tag = $"groupgrade_{groupGrade.ID}"
                    };

                    TextBlock groupGradeValue = new TextBlock() {
                        Text = groupGrade.Value.ToString(),
                        Width = 30,
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "GroupGradeValue",
                        //Style = (Style)Application.Current.Resources["GrayedOut"]
                    };

                    TextBlock groupGradeUserId = new TextBlock() {
                        Text = groupGrade.User.ID.ToString(),
                        Width = 20,
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "GroupGradeUserId",
                        //Style = (Style)Application.Current.Resources["GrayedOut"]
                    };

                    TextBlock groupGradeCategoryId = new TextBlock() {
                        Text = groupGrade.Category.ID.ToString(),
                        Width = 20,
                        Margin = new Thickness(0, 0, 10, 0),
                        Tag = "GroupGradeCategoryId",
                        //Style = (Style)Application.Current.Resources["GrayedOut"]
                    };

                    Button groupGradeRemButton = new Button() {
                        Content = "-",
                        Width = 20,
                        Tag = groupGrade.ID
                    };
                    groupGradeRemButton.Click += this.GroupGradeRemGradeEvent;

                    groupGradeWrapper.Children.Add(groupGradeValue);
                    groupGradeWrapper.Children.Add(groupGradeUserId);
                    groupGradeWrapper.Children.Add(groupGradeCategoryId);
                    groupGradeWrapper.Children.Add(groupGradeRemButton);
                    this.GroupGradesExistingWrapper.Children.Add(groupGradeWrapper);
                }
            }
        }
        private void GroupGradeRemGradeEvent(object sender, RoutedEventArgs e) {
            // Check so parent of sender is StackPanel with tag begining with "groupgrade_"
            if (((Button)sender).Parent is StackPanel) {
                // Check starts with "groupgrade_"
                if (((string)((StackPanel)((Button)sender).Parent).Tag).StartsWith("groupgrade_")) {
                    if (((Button)sender).Tag is int) {
                        int groupGradeId = (int)((Button)sender).Tag;
                        bool success = WindowInstance.DbContext.RemoveGrade(groupGradeId);
                        if (success) {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Group grade removed!";
                                this.MarkInfoTextSuccess();
                            }
                        } else {
                            if (this.InfoTextBlock != null) {
                                this.InfoTextBlock.Text = "Failed to remove group grade!";
                                this.MarkInfoTextError();
                            }
                        }
                        this.UpdateGroupGradesSegment();
                    }
                }
            }
        }
        private void GroupGradeAddGradeEvent(object sender, RoutedEventArgs e) { }
    }

    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView {

        private readonly MainWindow WindowInstance;

        public UserControl? SendingView = null;
        public AdminView(MainWindow WindowInstance, UserControl? sendingView = null) {
            this.SendingView = sendingView;
            this.WindowInstance = WindowInstance;
            InitializeComponent();

            UpdateAdminView();
        }

        private void ChangeToUserView_Click(object sender, RoutedEventArgs e) {

            if (this.WindowInstance.Shared.userView != null) {
                this.WindowInstance.Shared.userView.GenerateUserView();
            }

            // If the sendinvView is UserView, then we want to go back to that view, else instantiate a new UserView
            if (this.SendingView != null && this.SendingView is UserView) {
                UserView sendingView = (UserView)this.SendingView;
                sendingView.SendingView = this;
                this.WindowInstance.NavigateTo(sendingView);
            } else {
                if (this.WindowInstance.Shared.userView != null) {
                    this.WindowInstance.NavigateTo(this.WindowInstance.Shared.userView);
                } else {
                    this.WindowInstance.NavigateTo(new UserView(this.WindowInstance, this));
                }
            }
        }

        private void UpdateAdminView() {
            // Get entries
            List<Edition> editions = WindowInstance.DbContext.GetEditions();
            List<Category> categories = WindowInstance.DbContext.GetCategories();
            List<User> users = WindowInstance.DbContext.GetUsers();
            List<ResolvedGroup> groups = WindowInstance.DbContext.GetResolvedGroups();

            
            // Instantiate under editions editable fields
            StackPanel editionsStack = new StackPanel() {
                Orientation = Orientation.Vertical,
            };
            
            AdminSegment_Editions.Content = editionsStack;

            foreach (var edition in editions) {
                EditionAdminEditor editionAdminEditor = new EditionAdminEditor(WindowInstance, edition, editionsStack);
                editionAdminEditor.Instantiate();
            }

            
            // Instantiate under categories editable fields
            StackPanel categoriesStack = new StackPanel() {
                Orientation = Orientation.Vertical,
            };

            AdminSegment_Categories.Content = categoriesStack;

            foreach (var category in categories) {
                CategoryAdminEditor categoryAdminEditor = new CategoryAdminEditor(WindowInstance, category, categoriesStack);
                categoryAdminEditor.Instantiate();
            }


            // Instantiate under users editable fields
            StackPanel usersStack = new StackPanel() {
                Orientation = Orientation.Vertical,
            };

            AdminSegment_Users.Content = usersStack;

            foreach (var user in users) {
                UserAdminEditor userAdminEditor = new UserAdminEditor(WindowInstance, user, usersStack);
                userAdminEditor.Instantiate();
            }

            // Instantiate under groups editable fields
            StackPanel groupsStack = new StackPanel() {
                Orientation = Orientation.Vertical,
            };

            AdminSegment_Groups.Content = groupsStack;

            foreach (var group in groups) {
                GroupAdminEditor groupAdminEditor = new GroupAdminEditor(WindowInstance, group, groupsStack);
                groupAdminEditor.Instantiate();
            }
        }
    }
}