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

            // Define dictionary mapping editionID to UITools_ParticipantsManager_Edition
            Dictionary<int, UITools_ParticipantsManager_Edition> editionManagers = new Dictionary<int, UITools_ParticipantsManager_Edition>();

            // Create a UITools_ParticipantsManager_Edition in UnassignedParticipantsWrapper
            UITools_ParticipantsManager_Edition unassignedParticipantsManager = new UITools_ParticipantsManager_Edition(0, "Unassigned participants", new List<UITools_ParticipantsManager_Participant>());
            editionManagers[0] = unassignedParticipantsManager;
            UnassignedParticipantsWrapper.Children.Add(unassignedParticipantsManager.GetElement());

            // Get all editions
            List<DbTableModel_Edition> editions = windowInstance.Shared.appDbContext.GetEditions();

            // Create UITools_ParticipantsManager_Edition for each edition inside the WrapPanel EditionsWrapper
            foreach (DbTableModel_Edition edition in editions) {
                UITools_ParticipantsManager_Edition editionManager = new UITools_ParticipantsManager_Edition(edition.ID, edition.Name, new List<UITools_ParticipantsManager_Participant>());
                editionManagers[edition.ID] = editionManager;
                EditionsWrapper.Children.Add(editionManager.GetElement());
            }

            // Get all the participants
            List<DbTableModel_Participant> participants = windowInstance.Shared.appDbContext.GetParticipants();

            // Assign each participant to the correct edition, by creating a UITools_ParticipantsManager_Participant for each participant and adding it to the correct UITools_ParticipantsManager_Edition using AddParticipant
            foreach (DbTableModel_Participant participant in participants) {
                UITools_ParticipantsManager_Participant participantManager = new UITools_ParticipantsManager_Participant(participant.ID, participant.Name, editionManagers[participant.EditionID], ShowParticipantNameEditPopup);
                editionManagers[participant.EditionID].AddParticipant(participantManager);
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
