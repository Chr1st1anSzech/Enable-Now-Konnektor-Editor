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
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static JObject s_properties;
        private readonly string _propertiesFilePath = Path.Combine(Util.GetApplicationRoot(), "properties.json");
        private readonly Grid _contentGrid;
        private readonly object _data;
        private readonly Type _dataType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="data"></param>
        public PageCreator(Grid grid, object data)
        {
            _contentGrid = grid;
            _data = data;
            if (data != null)
            {
                _dataType = data.GetType();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        internal void CreatePage(string categoryName)
        {
            JObject json = ReadProps();
            JToken config = json?[categoryName];
            if (config == null) { return; }

            List<JToken> subcategories = config.Children().ToList();
            int subcategoriesCount = subcategories.Count;
            int contentGridRow = 0;
            bool isFirstHeader = true;
            for (int categoryIndex = 0; categoryIndex < subcategoriesCount; categoryIndex++)
            {
                CreateCategoryContent(subcategories[categoryIndex], ref contentGridRow, isFirstHeader);
                isFirstHeader = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subcategory"></param>
        /// <param name="contentGridRow"></param>
        /// <param name="isFirstHeader"></param>
        private void CreateCategoryContent(JToken subcategory, ref int contentGridRow, bool isFirstHeader)
        {
            string subcategoryName = subcategory["FriendlyName"].Value<string>();
            AddSubcategoryHeader(subcategoryName, ref contentGridRow, isFirstHeader);
            List<JToken> properties = subcategory["Properties"].Children().ToList();
            int propertiesCount = properties.Count;

            for (int propertyIndex = 0; propertyIndex < propertiesCount; propertyIndex++)
            {
                AddControl(properties[propertyIndex], ref contentGridRow);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="isFirstHeader"></param>
        private void AddSubcategoryHeader(string categoryName, ref int rowIndex, bool isFirstHeader)
        {
            _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            TextBlock header = new()
            {
                Text = categoryName,
                Style = Application.Current.Resources["HeaderTextblockStyle"] as Style
            };
            if (isFirstHeader)
            {
                Thickness margin = header.Margin;
                margin.Top = 0d;
                header.Margin = margin;
            }
            Grid.SetColumnSpan(header, 2);
            Grid.SetRow(header, rowIndex);
            _contentGrid.Children.Add(header);
            rowIndex++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyToken"></param>
        /// <param name="rowIndex"></param>
        private void AddControl(JToken propertyToken, ref int rowIndex)
        {
            FrameworkElement control = new ControlFactory().CreateControl(propertyToken, _data);
            if (control == null)
            {
                return;
            }

            string controlLabel = propertyToken["FriendlyName"]?.Value<string>() ?? "";
            string controlExplanation = propertyToken["Explanation"]?.Value<string>();
            if (control is ListControl || control is DictionaryControl)
            {
                AddTwoRowControl(control, controlLabel, controlExplanation, ref rowIndex);
            }
            else
            {
                AddOneRowControl(control, controlLabel, controlExplanation, ref rowIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="controlLabel"></param>
        /// <param name="controlExplanation"></param>
        /// <param name="contentGridRow"></param>
        private void AddOneRowControl(FrameworkElement control, string controlLabel, string controlExplanation, ref int contentGridRow)
        {
            _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            CreateTitleLabel(controlLabel, ref contentGridRow);

            if (!string.IsNullOrWhiteSpace(controlExplanation))
            {
                CreateExplanationLabel(controlExplanation, ref contentGridRow);
            }

            if (control != null)
            {
                Grid.SetRow(control, contentGridRow);
                Grid.SetColumn(control, 1);
                _contentGrid.Children.Add(control);
            }


            contentGridRow++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="controlLabel"></param>
        /// <param name="controlExplanation"></param>
        /// <param name="contentGridRow"></param>
        private void AddTwoRowControl(FrameworkElement control, string controlLabel, string controlExplanation, ref int contentGridRow)
        {
            _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            CreateTitleLabel(controlLabel, ref contentGridRow, 2);

            if (!string.IsNullOrWhiteSpace(controlExplanation))
            {
                CreateExplanationLabel(controlExplanation, ref contentGridRow, 2);
            }

            if (control != null)
            {
                _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                contentGridRow++;
                AddControlToGrid(contentGridRow, 0, 2, control);
            }
            contentGridRow++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlLabel"></param>
        /// <param name="contentGridRow"></param>
        /// <param name="contentGridColumnspan"></param>
        private void CreateTitleLabel(string controlLabel, ref int contentGridRow, int contentGridColumnspan = 1)
        {
            TextBlock label = new()
            {
                Text = $"{controlLabel}:",
                Style = Application.Current.Resources["PropertiesTextBlockStyle"] as Style
            };

            AddControlToGrid(contentGridRow, 0, contentGridColumnspan, label);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlExplanation"></param>
        /// <param name="contentGridRow"></param>
        /// <param name="contentGridColumnspan"></param>
        private void CreateExplanationLabel(string controlExplanation, ref int contentGridRow, int contentGridColumnspan = 1)
        {
            TextBlock label = new()
            {
                Text = controlExplanation,
                Style = Application.Current.Resources["PropertiesExplanationTextblockStyle"] as Style
            };

            _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            contentGridRow++;
            AddControlToGrid(contentGridRow, 0, contentGridColumnspan, label);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentGridRow"></param>
        /// <param name="contentGridColumn"></param>
        /// <param name="contentGridColumnspan"></param>
        /// <param name="control"></param>
        private void AddControlToGrid(int contentGridRow, int contentGridColumn, int contentGridColumnspan, UIElement control)
        {
            Grid.SetRow(control, contentGridRow);
            Grid.SetColumn(control, contentGridColumn);
            Grid.SetColumnSpan(control, contentGridColumnspan);
            _contentGrid.Children.Add(control);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private JObject ReadProps()
        {
            if (s_properties != null) { return s_properties; }
            try
            {
                string jsonString = File.ReadAllText(_propertiesFilePath);
                return s_properties = JsonConvert.DeserializeObject<JObject>(jsonString);
            }
            catch
            {
                return null;
            }
        }
    }
}
