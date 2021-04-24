using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Enable_Now_Konnektor_Editor.src.frames
{
    internal class FrameManager
    {
        private protected static FrameManager s_manager;
        private readonly TabControl _tabControl;

        public FrameTabItem ActiveTab { get; set; }
        public ObservableCollection<FrameTabItem> FrameTabItems { get; } = new();



        /// <summary>
        /// 
        /// </summary>
        private protected FrameManager(TabControl control) : base()
        {
            _tabControl = control;
        }



        /// <summary>
        /// Gibt das FrameManager-Objekt zurück.
        /// </summary>
        /// <returns>Das FrameManager-Objekt.</returns>
        public static FrameManager GetFrameManager(TabControl control)
        {
            s_manager ??= new FrameManager(control);
            return s_manager;
        }



        /// <summary>
        /// Erstellt einen neuen Tab mit dem übergebenen Namen.
        /// </summary>
        /// <param name="header">Der Name des Tabs.</param>
        /// <returns>Das neu erstellte FrameTabItem-Objekt.</returns>
        public FrameTabItem OpenNewTab(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return null;

            Frame contentFrame = new()
            {
                NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden
            };
            FrameTabItem item = new(header, contentFrame);
            FrameTabItems.Add(item);
            ActiveTab = item;
            return item;
        }



        /// <summary>
        /// Erstellt und öffnet einen neuen Tab mit dem übergebenen Namen.
        /// </summary>
        /// <param name="header">Der Name des Tabs.</param>
        /// <returns>Das neu geöffnete FrameTabItem-Objekt.</returns>
        public FrameTabItem OpenAndSelectNewTab(string header)
        {
            FrameTabItem item = OpenNewTab(header);
            _tabControl.SelectedItem = item;
            return item;
        }



        /// <summary>
        /// Gibt den Tab mit dem Namen zurück.
        /// </summary>
        /// <param name="header">Der Name des betreffenden Tabs.</param>
        /// <returns>Das FrameTabItem-Objekt mit dem übergebenen Namen.</returns>
        public FrameTabItem GetTabItem(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return null;

            return FrameTabItems.FirstOrDefault(item => header.Equals(item.Header) );
        }



        /// <summary>
        /// Navigert das Frame des aktuellen Tabs zu einer Seite.
        /// </summary>
        /// <param name="page">Das Page-Objekt, zu dem das Frame navigieren soll.</param>
        public void NavigateTabToPage(Page page)
        {
            ActiveTab.Content.Navigate(page);
        }



        /// <summary>
        /// Navigert das Frame des Tabs zu einer Seite.
        /// </summary>
        /// <param name="tabName">Der Name des betreffenden Tabs.</param>
        /// <param name="page">Das Page-Objekt, zu dem das Frame navigieren soll.</param>
        public void NavigateTabToPage(string tabName, Page page)
        {
            GetTabItem(tabName).Content.Navigate(page);
        }



        /// <summary>
        /// Wechselt zu den Tab mit dem übergebenen Header.
        /// </summary>
        /// <param name="header">Der Name des zu öffnenden Tabs.</param>
        public void SwitchToTab(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return;

            _tabControl.SelectedItem = FrameTabItems.FirstOrDefault(item => header.Equals(item.Header));
        }



        /// <summary>
        /// Schließt den Tab mit dem übergebenen Header.
        /// </summary>
        /// <param name="header">Der Name des zu schließenden Tabs</param>
        public void CloseTab(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return;

            FrameTabItem item = FrameTabItems.FirstOrDefault(item => header.Equals(item.Header));
            CloseTab(item);
        }



        /// <summary>
        /// Schließt den übergebenen Tab.
        /// </summary>
        /// <param name="item"></param>
        public void CloseTab(FrameTabItem item)
        {
            if (item == null) return;

            FrameTabItems.Remove(item);
        }



        /// <summary>
        /// Den letzten Tab ermitteln.
        /// </summary>
        /// <returns></returns>
        public FrameTabItem GetLastTab()
        {
            int lastItemIndex = FrameTabItems.Count - 1;
            if (lastItemIndex < 0) return null;

            return FrameTabItems[lastItemIndex];
        }
    }
}
