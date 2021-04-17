using Enable_Now_Konnektor_Bibliothek.src.misc;
using Enable_Now_Konnektor_Editor.src.controls;
using System;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Enable_Now_Konnektor_Editor.src.helper;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Enable_Now_Konnektor_Editor.src.pages
{
    /// <summary>
    /// Interaktionslogik für SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly IJsonWriter writer;
        private readonly object data;

        public SettingsPage(string siteName, object data, IJsonWriter writer)
        {
            InitializeComponent();
            new PageCreator().CreatePage(this, siteName, data);
            this.data = data;
            this.writer = writer;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (data == null) { return; }
            bool isDataValide = true;

            try
            {
                Type t = data.GetType();
                foreach (FrameworkElement control in ContentGrid.Children)
                {
                    if (control.Tag is ControlTag tag)
                    {
                        string propertyName = tag.PropertyName;
                        string regexValue = tag.ValidationRegex;
                        PropertyInfo prop = t.GetProperty(propertyName);
                        string propertyType = prop.PropertyType.Name;
                        if (control is TextBox textbox)
                        {
                            string text = textbox.Text;

                            if ("Int32".Equals(propertyType))
                            {
                                if (Regex.IsMatch(text, regexValue))
                                {
                                    textbox.Style = Application.Current.Resources["PropertiesIntTextboxStyle"] as Style;
                                    if (int.TryParse(text, out int val))
                                    {
                                        prop.SetValue(data, val);
                                    }
                                }
                                else
                                {
                                    textbox.Style = Application.Current.Resources["PropertiesIntFailureTextboxStyle"] as Style;
                                    isDataValide = false;
                                }
                            }
                            else if ("String".Equals(propertyType))
                            {
                                if (Regex.IsMatch(text, regexValue))
                                {
                                    textbox.Style = Application.Current.Resources["PropertiesTextboxStyle"] as Style;
                                    prop.SetValue(data, text);
                                }
                                else
                                {
                                    textbox.Style = Application.Current.Resources["PropertiesFailureTextboxStyle"] as Style;
                                    isDataValide = false;
                                }
                            }


                        }
                        else if (control is CheckBox checkbox)
                        {
                            if ("Boolean".Equals(propertyType))
                            {
                                bool? val = checkbox.IsChecked;
                                prop.SetValue(data, val);
                            }
                        }
                        else if (control is ListControl listControl)
                        {
                            if ("String[]".Equals(propertyType))
                            {
                                var items = listControl.ItemsListbox.Items;
                                string[] val = new string[items.Count];
                                for (int i = 0; i < items.Count; i++)
                                {
                                    val[i] = items.GetItemAt(i).ToString();
                                }

                                prop.SetValue(data, val);
                            }
                        }
                        else if (control is DictionaryControl dictControl)
                        {
                            if ("Dictionary`2".Equals(propertyType))
                            {
                                var items = dictControl.ItemsListbox.Items;
                                Dictionary<string, string[]> val = new Dictionary<string, string[]>(items.Count);
                                for (int i = 0; i < items.Count; i++)
                                {
                                    PairValue pair = items[i] as PairValue;
                                    string[] itemValue = pair.Value.Split(';');
                                    val.Add(pair.Key, itemValue);
                                }
                                
                                prop.SetValue(data, val);
                            }
                        }
                    }
                }
                if (isDataValide)
                {
                    writer.Write(data);
                    SaveStatusTextBlock.Style = Application.Current.Resources["SaveSuccessTextBlock"] as Style;
                }
                else
                {
                    SaveStatusTextBlock.Style = Application.Current.Resources["SaveFailureTextBlock"] as Style;
                }
            }
            catch
            {
                SaveStatusTextBlock.Style = Application.Current.Resources["SaveFailureTextBlock"] as Style;
            }
        }
    }
}
