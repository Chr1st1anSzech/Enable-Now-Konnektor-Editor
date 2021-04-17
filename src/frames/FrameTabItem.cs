using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Enable_Now_Konnektor_Editor.src.frames
{
    public class FrameTabItem
    {
        public string Header { get; }
        public string SelectedPage { get; set; } = "Allgemein";
        public Frame Content { get; }

        public FrameTabItem(string header, Frame frame)
        {
            Header = header;
            Content = frame;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is FrameTabItem))
            {
                return false;
            }
            return (Header == ((FrameTabItem)obj).Header);
        }

        public override int GetHashCode()
        {
            return Header.GetHashCode();
        }
    }
}
