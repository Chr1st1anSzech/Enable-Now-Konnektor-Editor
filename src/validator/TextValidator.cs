using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Enable_Now_Konnektor_Editor.Validator
{
    class TextValidator
    {
        private static readonly Regex _emailRegex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

        public static bool ValidateEmail(string text)
        {
            return !_emailRegex.IsMatch(text);
        }
    }
}
