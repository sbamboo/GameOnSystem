using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GameOnSystem.Pages.Parts.Admin {
    public partial class Participants : UserControl {

        private MainWindow windowInstance;
        private AdminView parentView;

        public Participants(MainWindow windowInstance, AdminView parentView) {
            this.windowInstance = windowInstance;
            this.parentView = parentView;

            this.parentView.AdminSelectedTableTitle.Text = "Participants (Drag-n-drop participants to change)";

            InitializeComponent();

            // Define dictionary mapping groupID to UITools_ParticipantsManager_Group
            Dictionary<int, UITools_ParticipantsManager_Group> groupManagers = new Dictionary<int, UITools_ParticipantsManager_Group>();

            // Create a UITools_ParticipantsManager_Group in UnassignedParticipantsWrapper
            UITools_ParticipantsManager_Group unassignedParticipantsManager = new UITools_ParticipantsManager_Group(0, "Unassigned participants", new List<UITools_ParticipantsManager_Participant>());
            groupManagers[0] = unassignedParticipantsManager;
            UnassignedParticipantsWrapper.Children.Add(unassignedParticipantsManager.GetElement());

            // Get all editions
            List<DbTableModel_Group> groups = windowInstance.Shared.appDbContext.GetGroups();

            // Create UITools_ParticipantsManager_Group for each group inside the WrapPanel GroupsWrapper
            foreach (DbTableModel_Group group in groups) {
                UITools_ParticipantsManager_Group editionManager = new UITools_ParticipantsManager_Group(group.ID, group.Name, new List<UITools_ParticipantsManager_Participant>());
                groupManagers[group.ID] = editionManager;
                GroupsWrapper.Children.Add(editionManager.GetElement());
            }

            // Get all the participants
            List<DbTableModel_Participant> participants = windowInstance.Shared.appDbContext.GetParticipants();

            // Assign each participant to the correct group, by creating a UITools_ParticipantsManager_Participant for each participant and adding it to the correct UITools_ParticipantsManager_Group using AddParticipant
            foreach (DbTableModel_Participant participant in participants) {
                List<DbTableModel_Group> participantGroups = participant.GetGroups(windowInstance.Shared.appDbContext);

                //MARK: For now this UI only supports a participant being in one group, thus we use participantGroups[0]
                int selectedGroup = 0;
                if (participantGroups.Count > 0) {
                    selectedGroup = participantGroups[0].ID;
                }

                UITools_ParticipantsManager_Participant participantManager = new UITools_ParticipantsManager_Participant(participant.ID, participant.Name, groupManagers[selectedGroup], ShowParticipantNameEditPopup);
                groupManagers[selectedGroup].AddParticipant(participantManager);
            }
        }

        private void ShowParticipantNameEditPopup(object sender, RoutedEventArgs e) {
            UITools_ParticipantsManager_Participant participant = (UITools_ParticipantsManager_Participant)((Button)sender).Tag;
            ParticipantPopupInput.Text = participant.Name;

            ParticipantPopupInput.Tag = participant;

            ParticipantPopup.Visibility = Visibility.Visible;
        }

        private void ParticipantPopupCancel(object sender, RoutedEventArgs e) {
            ParticipantPopup.Visibility = Visibility.Collapsed;
        }

        private void ParticipantPopupConfirm(object sender, RoutedEventArgs e) {
            ParticipantPopup.Visibility = Visibility.Collapsed;

            UITools_ParticipantsManager_Participant participant = (UITools_ParticipantsManager_Participant)ParticipantPopupInput.Tag;

            participant.Name = ParticipantPopupInput.Text;
            participant.UpdateText();
        }
    }
}
