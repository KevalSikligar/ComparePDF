using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Compare_Tool.ViewModel
{
    public class HighlightViewModel
    {
        public string Word { get; set; }
        public int PageNo { get; set; }
        public  int PositionNo { get; set; }

        public char Color { get; set; }
    }
}
