using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Enable_Now_Konnektor_Editor.src.controls
{
    /// <summary>
    /// Interaktionslogik für SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public Action<string> Callback { get; set; }

        public SearchControl()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Callback(SearchTextbox.Text);
        }

        private void SearchTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Callback(SearchTextbox.Text);
            }
        }
    }
}
