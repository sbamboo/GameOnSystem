using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameOnSystem {

    public class UITools_GroupContentHolder {
        private StackPanel outerWrapper;
        private TextBlock noEntriesText;
        private ContentControl contentHolder;

        internal StackPanel OuterWrapper { get { return this.outerWrapper; } }
        internal TextBlock NoEntriesText { get { return this.noEntriesText; } }
        internal ContentControl ContentHolder { get { return this.contentHolder; } }

        public UITools_GroupContentHolder(StackPanel OuterWrapper, TextBlock NoEntriesText, ContentControl ContentHolder) {
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

}
