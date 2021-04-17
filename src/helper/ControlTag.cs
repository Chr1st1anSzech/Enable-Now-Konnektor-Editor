using System;
using System.Collections.Generic;
using System.Text;

namespace Enable_Now_Konnektor_Editor.src.helper
{
    internal class ControlTag
    {
        public string PropertyName { get; set; }
        public string ValidationRegex { get; set; }

        public ControlTag(string name, string regex)
        {
            PropertyName = name;
            ValidationRegex = regex;
        }
    }
}
