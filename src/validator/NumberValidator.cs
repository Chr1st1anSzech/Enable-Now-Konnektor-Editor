using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Enable_Now_Konnektor_Editor.Validator
{
    class NumberValidator
    {
        private static readonly Regex _numberRegex = new Regex("[^0-9.-]+");

        public static bool ValidateNumber(string text)
        {
            return !_numberRegex.IsMatch(text);
        }
    }
}
