using Enable_Now_Konnektor_Bibliothek.src.jobs;
using Enable_Now_Konnektor_Editor.src.windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Enable_Now_Konnektor_Editor.src.pages
{
    /// <summary>
    /// Interaktionslogik für StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private readonly Action<JobConfig> _openJob;

        public ExtJobManager MyJobManager { get; }

        public StartPage(Action<JobConfig> OpenJob)
        {
            InitializeComponent();
            MyJobManager = ExtJobManager.GetJobManager();
            InitDataBindings();
            _openJob = OpenJob;
            SearchField.Callback = FilterRecentJobs;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitDataBindings()
        {
            Binding jobListBinding = new()
            {
                Source = MyJobManager,
                Path = new PropertyPath(nameof(MyJobManager.RecentJobIds)),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(RecentJobsListView, ItemsControl.ItemsSourceProperty, jobListBinding);

            Binding filterBinding = new()
            {
                Source = MyJobManager,
                Path = new PropertyPath(nameof(MyJobManager.RecentJobsFilter)),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(SearchField.SearchTextbox, TextBox.TextProperty, filterBinding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecentJobsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView recentJobsListView &&
                recentJobsListView.SelectedItem is string jobId)
            {
                JobConfig jobConfig = MyJobManager.GetJobConfig(jobId);
                _openJob(jobConfig);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        private void FilterRecentJobs(string filter)
        {
            MyJobManager.RecentJobsFilter = filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewJobWindow window = new(_openJob);
            window.ShowDialog();
        }
    }
}
