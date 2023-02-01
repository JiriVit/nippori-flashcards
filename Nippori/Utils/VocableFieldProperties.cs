using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Nippori.Utils
{
    public class VocableFieldProperties
    {
        public string Text { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public FontFamily FontFamily { get; set; } = SystemFonts.MessageFontFamily;
    }
}
