// This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Enable_Now_Konnektor_Editor.src.controls;
using Enable_Now_Konnektor_Editor.src.helper;
using Newtonsoft.Json.Linq;

namespace Enable_Now_Konnektor_Editor.src.pages
{
    internal class ControlFactory
    {
        #region internal-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal FrameworkElement CreateControl(JToken property, object data)
        {
            string name = property["Name"]?.Value<string>();
            string regex = property["Validate"]?.Value<string>();
            bool? isReadonly = property["Readonly"]?.Value<bool>();
            object value = data.GetType().GetProperty(name).GetValue(data);
            return CreateSpecificControl(name, regex, isReadonly, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regex"></param>
        /// <param name="isReadonly"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private FrameworkElement CreateSpecificControl(string name, string regex, bool? isReadonly, object value)
        {
            FrameworkElement control = null;
            switch (value)
            {
                case string stringValue:
                    control = CreateTextbox(name, regex, isReadonly, stringValue, "PropertiesTextboxStyle");
                    break;
                case bool boolValue:

                    control = CreateCheckBox(name, regex, isReadonly, boolValue);
                    break;
                case int intValue:

                    control = CreateTextbox(name, regex, isReadonly, intValue.ToString(), "PropertiesIntTextboxStyle");
                    break;
                case string[] stringArray:

                    control = CreateListControl(name, regex, stringArray);
                    break;
                case Dictionary<string, string>:
                case Dictionary<string, string[]>:
                    control = CreateDictionaryControl(name, regex, value);
                    break;
            }
            return control;
        }
        #endregion

        #region private-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regex"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private FrameworkElement CreateDictionaryControl(string name, string regex, object dictionary)
        {
            DictionaryControl dictControl = new()
            {
                Tag = new ControlTag(name, regex)
            };
            FillDictionary(dictionary, dictControl);
            return dictControl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dictControl"></param>
        private void FillDictionary(object obj, DictionaryControl dictControl)
        {
            if (obj is Dictionary<string, string> stringDictionary)
            {
                foreach (KeyValuePair<string, string> item in stringDictionary)
                {
                    dictControl.AddItem(item.Key, item.Value);
                }
            }
            else if (obj is Dictionary<string, string[]> stringArrayDictionary)
            {
                foreach (KeyValuePair<string, string[]> item in stringArrayDictionary)
                {
                    dictControl.AddItem(item.Key, string.Join(';', item.Value));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regex"></param>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        private FrameworkElement CreateListControl(string name, string regex, string[] stringArray)
        {
            ListControl list = new()
            {
                Tag = new ControlTag(name, regex)
            };
            foreach (string item in stringArray)
            {
                list.AddItem(item);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regex"></param>
        /// <param name="isReadonly"></param>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        private FrameworkElement CreateCheckBox(string name, string regex, bool? isReadonly, bool boolValue)
        {
            CheckBox checkbox = new()
            {
                Style = Application.Current.Resources["BaseCheckboxStyle"] as Style,
                Tag = new ControlTag(name, regex),
                IsEnabled = isReadonly ?? true,
                IsChecked = boolValue
            };
            return checkbox;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regex"></param>
        /// <param name="isReadonly"></param>
        /// <param name="stringValue"></param>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private FrameworkElement CreateTextbox(string name, string regex, bool? isReadonly, string stringValue, string styleName)
        {
            TextBox textBox = new()
            {
                Style = Application.Current.Resources[styleName] as Style,
                Tag = new ControlTag(name, regex),
                IsReadOnly = isReadonly ?? false,
                Text = stringValue ?? ""
            };
            return textBox;
        }
        #endregion
    }
}
