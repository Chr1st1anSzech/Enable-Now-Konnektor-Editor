using Enable_Now_Konnektor_Bibliothek.src.misc;
using Enable_Now_Konnektor_Editor.src.controls;
using Enable_Now_Konnektor_Editor.src.helper;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Enable_Now_Konnektor_Editor.src.pages
{
    class PageCreator
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string propertiesFilePath = Path.Combine(Util.GetApplicationRoot(), "properties.json");
        private static JObject Properties { get; set; }

        private object data;
        private Grid contentGrid;
        private Type dataType;

        internal void CreatePage(SettingsPage page, string category, object data)
        {
            this.data = data;
            if (data != null)
            {
                dataType = data.GetType();
            }
            contentGrid = page.ContentGrid;
            CreateContent(category);
        }

        private void CreateContent(string category)
        {

            JObject json = ReadProps();
            JToken config = json?[category];
            if( config == null) { return; }

            List<JToken> subcategories = config.Children().ToList();
            int subcategoriesCount = subcategories.Count;
            int rowIndex = 0;
            bool isFirstHeader = true;
            for (int categoryIndex = 0; categoryIndex < subcategoriesCount; categoryIndex++)
            {
                var subcategory = subcategories[categoryIndex];
                string subcategoryName = subcategory["FriendlyName"].Value<string>();
                AddSubcategoryHeader(subcategoryName, ref rowIndex, isFirstHeader);
                isFirstHeader = false;
                List<JToken> properties = subcategory["Properties"].Children().ToList();
                int propertiesCount = properties.Count;                

                for (int propertyIndex = 0; propertyIndex < propertiesCount; propertyIndex++)
                {
                    JToken propertyToken = properties[propertyIndex];
                    AddControl(propertyToken, ref rowIndex);
                }
            }
        }

        private void AddSubcategoryHeader(string categoryName, ref int rowIndex, bool isFirstHeader)
        {
            contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            TextBlock header = new TextBlock()
            {
                Text = categoryName,
                Style = Application.Current.Resources["HeaderTextblockStyle"] as Style
            };
            if( isFirstHeader)
            {
                var margin = header.Margin;
                margin.Top = 0d;
                header.Margin = margin;
            }
            Grid.SetColumnSpan(header, 2);
            Grid.SetRow(header, rowIndex);
            contentGrid.Children.Add(header);
            rowIndex++;
        }

        private void AddControl(JToken propertyToken, ref int rowIndex)
        {
            string friendlyName = propertyToken["FriendlyName"]?.Value<string>() ?? "";
            string explanationText = propertyToken["Explanation"]?.Value<string>();
            FrameworkElement control = CreateControl(propertyToken);
            if( control == null)
            {
                return;
            }
            else if( control is ListControl || control is DictionaryControl)
            {
                AddTwoRowControl(control, friendlyName, explanationText, ref rowIndex);
            }
            else
            {
                AddOneRowControl(control, friendlyName, explanationText, ref rowIndex);
            }
        }

        private void AddOneRowControl(FrameworkElement control, string friendlyName, string explanationText, ref int rowIndex)
        {
            contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            CreateTitleLabel(friendlyName, ref rowIndex);

            if(control!= null)
            {
                Grid.SetRow(control, rowIndex);
                Grid.SetColumn(control, 1);
                contentGrid.Children.Add(control);
            }

            if( !string.IsNullOrWhiteSpace(explanationText) )
            {
                CreateExplanationLabel(explanationText, ref rowIndex);
            }

            rowIndex++;
        }

        private void AddTwoRowControl(FrameworkElement control, string friendlyName, string explanationText, ref int rowIndex)
        {
            contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            CreateTitleLabel(friendlyName, ref rowIndex, 2);

            if (!string.IsNullOrWhiteSpace(explanationText))
            {
                CreateExplanationLabel(explanationText, ref rowIndex, 2 );
            }

            if (control != null)
            {
                contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                rowIndex++;
                Grid.SetRow(control, rowIndex);
                Grid.SetColumn(control, 0);
                Grid.SetColumnSpan(control, 2);
                contentGrid.Children.Add(control);
            }
            rowIndex++;
        }

        private void CreateTitleLabel(string friendlyName, ref int rowIndex, int columnspan = 1)
        {
            TextBlock label = new TextBlock()
            {
                Text = $"{friendlyName}:",
                Style = Application.Current.Resources["PropertiesTextBlockStyle"] as Style
            };
            Grid.SetRow(label, rowIndex);
            Grid.SetColumn(label, 0);
            Grid.SetColumnSpan(label, columnspan);
            contentGrid.Children.Add(label);
        }

        private void CreateExplanationLabel(string explanationText, ref int rowIndex, int columnspan = 1)
        {
            TextBlock label = new TextBlock()
            {
                Text = explanationText,
                Style = Application.Current.Resources["PropertiesExplanationTextblockStyle"] as Style
            };
            contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            rowIndex++;
            Grid.SetRow(label, rowIndex);
            Grid.SetColumn(label, 0);
            Grid.SetColumnSpan(label, columnspan);
            contentGrid.Children.Add(label);
        }

        private FrameworkElement CreateControl(JToken property)
        {
            string name = property["Name"]?.Value<string>();
            string regex = property["Validate"]?.Value<string>();
            bool? isReadonly = property["Readonly"]?.Value<bool>();
            FrameworkElement control = null;
            var value = dataType.GetProperty(name).GetValue(data);


            if (value is string stringValue)
            {
                TextBox textBox = new TextBox()
                {
                    Style = Application.Current.Resources["PropertiesTextboxStyle"] as Style,
                    Tag = new ControlTag(name, regex),
                    IsReadOnly = isReadonly ?? false,
                    Text = stringValue ?? ""
                };
                control = textBox;

            }
            else if (value is bool boolValue)
            {
                CheckBox checkbox = new CheckBox() {
                    Style = Application.Current.Resources["BaseCheckboxStyle"] as Style,
                    Tag = new ControlTag(name, regex),
                    IsEnabled = isReadonly ?? true,
                    IsChecked = boolValue
                };
                control = checkbox;
            }
            else if (value is int inValue)
            {
                TextBox textBox = new TextBox()
                {
                    Style = Application.Current.Resources["PropertiesIntTextboxStyle"] as Style,
                    Tag = new ControlTag(name, regex),
                    IsReadOnly = isReadonly ?? false,
                    Text = inValue.ToString()
                };
                control = textBox;
            }
            else if (value is string[] stringArray)
            {
                ListControl list = new ListControl()
                {
                    Tag = new ControlTag(name, regex)
                };
                foreach (string item in stringArray)
                {
                    list.AddItem(item);
                }
                control = list;
            }
            else if (value is Dictionary<string,string> stringDictionary)
            {
                DictionaryControl dictControl = new DictionaryControl()
                {
                    Tag = new ControlTag(name, regex)
                };
                foreach (KeyValuePair<string,string> item in stringDictionary)
                {
                    dictControl.AddItem(item.Key, item.Value);
                }
                control = dictControl;
            }
            else if (value is Dictionary<string, string[]> stringArrayDictionary)
            {
                DictionaryControl dictControl = new DictionaryControl()
                {
                    Tag = new ControlTag(name, regex)
                };
                foreach (KeyValuePair<string, string[]> item in stringArrayDictionary)
                {
                    dictControl.AddItem(item.Key, string.Join(';',item.Value) );
                }
                control = dictControl;
            }
            return control;
        }

        private JObject ReadProps()
        {
            if( Properties != null) { return Properties; }
            try
            {
                string jsonString = File.ReadAllText(propertiesFilePath);
                return  Properties = JsonConvert.DeserializeObject<JObject>(jsonString) ;
            }
            catch
            {
                return null;
            }
        }
    }
}
