using Enable_Now_Konnektor_Bibliothek.src.config;
using Enable_Now_Konnektor_Bibliothek.src.jobs;
using Enable_Now_Konnektor_Editor.src.helper;
using Enable_Now_Konnektor_Editor.src.pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Enable_Now_Konnektor_Editor.src.frames
{
    internal class FrameManager
    {
        private protected static FrameManager Manager;
        private readonly TabControl tabControl;

        public FrameTabItem ActiveTab { get; set; }
        public ObservableCollection<FrameTabItem> FrameTabItems { get; } = new();



        /// <summary>
        /// 
        /// </summary>
        private protected FrameManager(TabControl control) : base()
        {
            tabControl = control;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FrameManager GetFrameManager(TabControl control)
        {
            Manager ??= new FrameManager(control);
            return Manager;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public FrameTabItem OpenAndSelectNewTab(string header)
        {
            FrameTabItem item = OpenNewTab(header);
            tabControl.SelectedItem = item;
            return item;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public FrameTabItem GetTabItem(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return null;

            return FrameTabItems.FirstOrDefault(item => header.Equals(item.Header) );
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public void NavigateTabToPage(Page page)
        {
            ActiveTab.Content.Navigate(page);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public void NavigateTabToPage(string tabName, Page page)
        {
            GetTabItem(tabName).Content.Navigate(page);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public void SwitchToTab(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return;

            tabControl.SelectedItem = FrameTabItems.FirstOrDefault(item => header.Equals(item.Header));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public void CloseTab(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return;

            FrameTabItem item = FrameTabItems.FirstOrDefault(item => header.Equals(item.Header));
            CloseTab(item);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public void CloseTab(FrameTabItem item)
        {
            if (item == null) return;

            FrameTabItems.Remove(item);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public FrameTabItem GetLastTab()
        {
            int lastItemIndex = FrameTabItems.Count - 1;
            if (lastItemIndex < 0) return null;

            return FrameTabItems[lastItemIndex];
        }
    }
}
