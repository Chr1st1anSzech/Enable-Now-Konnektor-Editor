using Enable_Now_Konnektor_Bibliothek.src.jobs;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Enable_Now_Konnektor_Editor.src.windows
{
    /// <summary>
    /// Interaktionslogik für NewJobWindow.xaml
    /// </summary>
    public partial class NewJobWindow : Window
    {
        private Action<JobConfig> OpenJob;

        public NewJobWindow(Action<JobConfig> OpenJob)
        {
            InitializeComponent();
            this.OpenJob = OpenJob;
            JobNameTextBox.Focus();
        }

        #region Window Command Buttons

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void SwitchWindowSize()
        {
            if (WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(this);
            }
            else
            {
                SystemCommands.MaximizeWindow(this);
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateJob();
        }


        private void JobNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateJob();
            }
        }

        private void CreateJob()
        {
            JobConfig jobConfig = ExtJobManager.GetJobManager().CreateJob(JobNameTextBox.Text);
            if (jobConfig!= null)
            {
                Close();
                ExtJobManager.GetJobManager().AddRecentJob(jobConfig);
                OpenJob(jobConfig);
            }
            else
            {
                JobNameTextBox.Style = Application.Current.Resources["PropertiesFailureTextboxStyle"] as Style;
            }
        }
    }
}
