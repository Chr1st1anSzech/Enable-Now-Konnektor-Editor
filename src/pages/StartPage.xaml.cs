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
        public ExtJobManager MyJobManager { get; }

        private Action<JobConfig> OpenJob;

        public StartPage(Action<JobConfig> OpenJob)
        {
            InitializeComponent();
            MyJobManager = ExtJobManager.GetJobManager();
            InitDataBindings();
            this.OpenJob = OpenJob;
            SearchField.Callback = FilterRecentJobs;
        }

        private void InitDataBindings()
        {
            Binding jobListBinding = new()
            {
                Source = MyJobManager,
                Path = new PropertyPath(nameof(MyJobManager.RecentJobs)),
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

        private void RecentJobsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( sender is ListView recentJobsListView && 
                recentJobsListView.SelectedItem is JobConfig jobConfig)
            {
                OpenJob(jobConfig);
            }
        }

        private void FilterRecentJobs(string filter)
        {
            MyJobManager.RecentJobsFilter = filter;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewJobWindow window = new(OpenJob);
            window.ShowDialog();
        }
    }
}
