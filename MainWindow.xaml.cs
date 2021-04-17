using Enable_Now_Konnektor_Bibliothek.src.config;
using Enable_Now_Konnektor_Bibliothek.src.jobs;
using Enable_Now_Konnektor_Editor.src.pages;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using log4net;
using System.Reflection;
using System.Windows.Data;
using Enable_Now_Konnektor_Editor.src.frames;

namespace Enable_Now_Konnektor_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ExtJobManager MyJobManager { get; }
        public ConfigManager MyConfigManager { get; }
        internal FrameManager MyFrameManager { get; }

        private bool jobExplorerExpanded = false;
        private ToggleButton checkedToggleButton;

        private readonly string startPageName = "Startseite";
        private readonly string konnektorPageName = "Konnektor";
        private string settingsStartPage;
        private struct JobPageButtonTemplate
        {
            public string Content { get; set; }
            public string Icon { get; set; }
        }
        private ToggleButton[] jobPageButtons;



        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MyJobManager = ExtJobManager.GetJobManager();
            MyConfigManager = ConfigManager.GetConfigManager();
            MyFrameManager = FrameManager.GetFrameManager(ContentTabControl);

            InitDataBindings();

            NavigationExpandButton.Click += NavigationExpandButton_Click;
            JobExpandButton.Click += JobExpandButton_Click;
            StateChanged += MainWindowStateChangeRaised;
            ContentTabControl.SelectionChanged += TabControl_SelectionChanged;

            CreateNavigationButtons();

            OpenStartPage();
        }

        #region Window Command Buttons

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            SwitchWindowProperties();
        }



        /// <summary>
        /// 
        /// </summary>
        private void SwitchWindowProperties()
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        #endregion



        private void CreateNavigationButtons()
        {
            JobPageButtonTemplate[] jobPageButtonTemplates = new JobPageButtonTemplate[] {
                new JobPageButtonTemplate(){ Content = "Allgemein", Icon="\uf013"},
                new JobPageButtonTemplate(){ Content = "Autostart", Icon="\uf011"},
                new JobPageButtonTemplate(){ Content = "Indexierung", Icon="\uf019"},
                new JobPageButtonTemplate(){ Content = "E-Mail", Icon="\uf0e0"},
                new JobPageButtonTemplate(){ Content = "Anmeldung", Icon="\uf577"},
                new JobPageButtonTemplate(){ Content = "Global", Icon="\uf7a2"},
                new JobPageButtonTemplate(){ Content = "Gruppe", Icon="\uf187"},
                new JobPageButtonTemplate(){ Content = "Projekt", Icon="\uf1c8"},
                new JobPageButtonTemplate(){ Content = "Buch", Icon="\uf02d"},
                new JobPageButtonTemplate(){ Content = "Buchseite", Icon="\uf518"},
                new JobPageButtonTemplate(){ Content = "Textseite", Icon="\uf15c"}
            };
            settingsStartPage = jobPageButtonTemplates[0].Content;
            jobPageButtons = new ToggleButton[jobPageButtonTemplates.Length];
            int i = 0;
            foreach (JobPageButtonTemplate buttonTemplate in jobPageButtonTemplates)
            {
                ToggleButton button = new ToggleButton()
                {
                    Tag = buttonTemplate.Icon,
                    Content = buttonTemplate.Content,
                    Visibility = Visibility.Hidden,
                    Style = Application.Current.Resources["BaseToggleButtonStyle"] as Style
                };
                jobPageButtons[i] = button;
                button.Click += NavButton_Click;
                NavigationPanel.Children.Add(button);
                i++;
            }
        }

        private void InitDataBindings()
        {
            Binding jobListBinding = new()
            {
                Source = MyJobManager,
                Path = new PropertyPath("AllJobs"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(JobListView, ItemsControl.ItemsSourceProperty, jobListBinding);

            Binding currentJobBinding = new()
            {
                Source = MyJobManager,
                Path = new PropertyPath("SelectedJobConfig.Id"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = ""
            };
            BindingOperations.SetBinding(CurrentJobText, TextBlock.TextProperty, currentJobBinding);

            Binding openJobBinding = new()
            {
                Source = MyFrameManager,
                Path = new PropertyPath("FrameTabItems"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            BindingOperations.SetBinding(ContentTabControl, ItemsControl.ItemsSourceProperty, openJobBinding);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton navButton)
            {
                string pageName = navButton.Content.ToString();
                JobConfig jobConfig = MyJobManager.SelectedJobConfig;
                string path = MyJobManager.GetJobConfigPath(jobConfig);
                Page page = new SettingsPage(pageName, jobConfig, new JobWriter(path));
                OpenPage(jobConfig.Id, page, pageName);
            }
        }

        #region JobExpandButton

        private void JobExpandButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchJobbExplorerExpanded();
        }

        private void SwitchJobbExplorerExpanded()
        {
            JobExplorerBorder.Visibility = jobExplorerExpanded ? Visibility.Collapsed : Visibility.Visible;
            jobExplorerExpanded = !jobExplorerExpanded;
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationExpandButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationPanel.Tag = string.IsNullOrWhiteSpace(NavigationPanel.Tag?.ToString()) ? "Expanded" : null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JobListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView jobListView)
            {
                JobConfig jobConfig = jobListView.SelectedItem as JobConfig;
                OpenJob(jobConfig);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JobTemplateListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button closeButton && closeButton.DataContext is FrameTabItem item)
            {
                CloseTab(item);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl && tabControl.SelectedItem is FrameTabItem selectedItem)
            {
                MyFrameManager.ActiveTab = selectedItem;
                HighlightNavigationButton(MyFrameManager.ActiveTab.SelectedPage);
                JobConfig jobConfig = MyJobManager.GetJobConfig(selectedItem.Header);
                MyJobManager.SelectedJobConfig = jobConfig;
                ShowJobSettingButtons(jobConfig != null);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KonnektorPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton startPageButton)
            {
                Page page = new SettingsPage(konnektorPageName, MyConfigManager.ConnectorConfig, new ConfigWriter(ConfigIO.FilePath));
                OpenPage(konnektorPageName, page);
                ShowJobSettingButtons(false);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && sender is Border tabBorder && tabBorder.DataContext is FrameTabItem item)
            {
                CloseTab(item);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                OpenStartPage();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobConfig"></param>
        private void OpenJob(JobConfig jobConfig)
        {
            if (jobConfig == null) return;

            MyJobManager.SelectedJobConfig = jobConfig;
            MyJobManager.AddRecentJob(jobConfig);
            ShowJobSettingButtons(true);

            if (!MyJobManager.IsJobOpened(jobConfig))
            {
                MyJobManager.AddOpenedJob(jobConfig);
            }

            string path = MyJobManager.GetJobConfigPath(jobConfig);
            Page page = new SettingsPage(settingsStartPage, jobConfig, new JobWriter(path));
            OpenPage(jobConfig.Id, page, settingsStartPage);

            if (jobExplorerExpanded)
            {
                SwitchJobbExplorerExpanded();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private void ShowJobSettingButtons(bool visible)
        {
            foreach (ToggleButton button in jobPageButtons)
            {
                if (visible)
                {
                    button.Visibility = Visibility.Visible;
                }
                else
                {
                    button.Visibility = Visibility.Collapsed;
                }

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void CloseTab(FrameTabItem item)
        {
            MyJobManager.RemoveOpenedJob(item.Header);
            MyFrameManager.CloseTab(item);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageName"></param>
        private void HighlightNavigationButton(string pageName)
        {
            if (checkedToggleButton != null)
            {
                checkedToggleButton.IsChecked = false;
            }
            foreach (var control in NavigationPanel.Children)
            {
                if ((control is ToggleButton navButton) && navButton.Content.Equals(pageName))
                {
                    checkedToggleButton = navButton;
                    navButton.IsChecked = true;
                    break;
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private void OpenStartPage()
        {
            Page page = new StartPage(OpenJob);
            OpenPage(startPageName, page);
            ShowJobSettingButtons(false);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="page"></param>
        private void OpenPage(string tabName, Page page, string pageName = null)
        {
            if (pageName == null) pageName = tabName;

            FrameTabItem item = MyFrameManager.GetTabItem(tabName);
            if (item == null)
            {
                item = MyFrameManager.OpenAndSelectNewTab(tabName);
                MyFrameManager.NavigateTabToPage(page);
            }
            else
            {
                MyFrameManager.SwitchToTab(tabName);
            }
            if (!pageName.Equals(item.SelectedPage))
            {
                MyFrameManager.NavigateTabToPage(page);
            }
            item.SelectedPage = pageName;
            HighlightNavigationButton(pageName);
        }
    }
}
