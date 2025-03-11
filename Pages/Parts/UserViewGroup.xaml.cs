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

            // Add image element to GroupGamePresentWrapper
            if (!string.IsNullOrEmpty(group.GameBannerUrl)) {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(group.GameBannerUrl);
                bitmap.DecodePixelWidth = 300;
                bitmap.DecodePixelHeight = 160;
                bitmap.EndInit();

                GroupGamePresentWrapper.Content = new System.Windows.Controls.Image {
                    Source = bitmap,
                    Width = 300,
                    Height = 169
                };
            }

            // Add group-participants
            GroupMembers.Text = "Group Members: ";
            foreach (var participant in group.GetParticipants(windowInstance.Shared.appDbContext)) {
                GroupMembers.Text += $"{participant.Name}, ";
            }
            GroupMembers.Text = GroupMembers.Text.TrimEnd(',', ' ');
        }

        private void GroupTitle_LayoutUpdated(object sender, EventArgs e) {
            // Attempt to autoscale the font size
            double availableWidth = GameLeftWrapper.ActualWidth;
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
    }
}
