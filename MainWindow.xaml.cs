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
        private struct JobPageButtonTemplate
        {
            public string Content { get; set; }
            public string Icon { get; set; }
        }

        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _startPageName = "Startseite";
        private readonly string _konnektorPageName = "Konnektor";

        private bool _jobExplorerExpanded = false;
        private string _settingsStartPage;
        private ToggleButton _checkedToggleButton;
        private ToggleButton[] _jobPageButtons;

        public ExtJobManager MyJobManager { get; }
        public ConfigManager MyConfigManager { get; }
        internal FrameManager MyFrameManager { get; }


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
            _settingsStartPage = jobPageButtonTemplates[0].Content;
            _jobPageButtons = new ToggleButton[jobPageButtonTemplates.Length];
            int i = 0;
            foreach (JobPageButtonTemplate buttonTemplate in jobPageButtonTemplates)
            {
                ToggleButton button = new()
                {
                    Tag = buttonTemplate.Icon,
                    Content = buttonTemplate.Content,
                    Visibility = Visibility.Hidden,
                    Style = Application.Current.Resources["BaseToggleButtonStyle"] as Style
                };
                _jobPageButtons[i] = button;
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
                Path = new PropertyPath("JobIds"),
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
                FrameTabItem item = OpenPage(jobConfig.Id, page, pageName);
                item.IsJobPage = true;
            }
        }

        #region JobExpandButton

        private void JobExpandButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchJobbExplorerExpanded();
        }

        private void SwitchJobbExplorerExpanded()
        {
            JobExplorerBorder.Visibility = _jobExplorerExpanded ? Visibility.Collapsed : Visibility.Visible;
            _jobExplorerExpanded = !_jobExplorerExpanded;
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
                string jobId = jobListView.SelectedItem as string;
                JobConfig jobConfig = MyJobManager.GetJobConfig(jobId);
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
                if( selectedItem.IsJobPage)
                {
                    JobConfig jobConfig = MyJobManager.GetJobConfig(selectedItem.Header);
                    MyJobManager.SelectedJobConfig = jobConfig;
                    SwitchJobButtonsVisibility(jobConfig != null);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KonnektorPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                Page page = new SettingsPage(_konnektorPageName, MyConfigManager.ConnectorConfig, new ConfigWriter());
                OpenPage(_konnektorPageName, page);
                SwitchJobButtonsVisibility(false);
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
            SwitchJobButtonsVisibility(true);

            if (!MyJobManager.IsJobOpened(jobConfig))
            {
                MyJobManager.AddOpenedJob(jobConfig);
            }

            string path = MyJobManager.GetJobConfigPath(jobConfig);
            Page page = new SettingsPage(_settingsStartPage, jobConfig, new JobWriter(path));
            FrameTabItem item = OpenPage(jobConfig.Id, page, _settingsStartPage);
            item.IsJobPage = true;

            if (_jobExplorerExpanded)
            {
                SwitchJobbExplorerExpanded();
            }
        }



        /// <summary>
        /// Zeigt oder versteckt die Navigations-Schaltflächen, die zur Job-Konfiguration gehören.
        /// </summary>
        /// <param name="visible">Wahr, wenn die Schaltflächen angezeigt werden sollen.</param>
        private void SwitchJobButtonsVisibility(bool visible)
        {
            foreach (ToggleButton button in _jobPageButtons)
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
        /// Schließt den entsprechenden Tab.
        /// </summary>
        /// <param name="item"></param>
        private void CloseTab(FrameTabItem item)
        {
            MyJobManager.RemoveOpenedJob(item.Header);
            MyFrameManager.CloseTab(item);
        }



        /// <summary>
        /// Hebt die zugehörige Navigations-Schaltfläche der geöffneten Seite hervor.
        /// </summary>
        /// <param name="pageName">Der Name der Seite, deren Navigations-Schaltfläche hervorgehoben werden soll.</param>
        private void HighlightNavigationButton(string pageName)
        {
            if (_checkedToggleButton != null)
            {
                _checkedToggleButton.IsChecked = false;
            }
            foreach (object control in NavigationPanel.Children)
            {
                if ((control is ToggleButton navButton) && navButton.Content.Equals(pageName))
                {
                    _checkedToggleButton = navButton;
                    navButton.IsChecked = true;
                    break;
                }
            }
        }



        /// <summary>
        /// Öffnet die Startseite.
        /// </summary>
        private void OpenStartPage()
        {
            Page page = new StartPage(OpenJob);
            OpenPage(_startPageName, page);
            SwitchJobButtonsVisibility(false);
        }



        /// <summary>
        ///         /// Öffnet oder erstellt einen Tab mit entsprechenden Namen. Navigiert das Frame zur Seite und hebt
        /// die zugehörige Navigations-Schaltfläche hervor.
        /// </summary>
        /// <param name="tabName">Der Name des zu öffnenden oder zu erstellenden Tabs.</param>
        /// <param name="page">Die Seite, die geöffnet werden soll.</param>
        /// <param name="pageName"></param>
        /// <returns>Der geöffnete Tab.</returns>
        private FrameTabItem OpenPage(string tabName, Page page, string pageName = null)
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
            return item;
        }
    }
}
