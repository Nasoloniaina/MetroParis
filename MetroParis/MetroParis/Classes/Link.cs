using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroParis.Classes
{
    class Link
    {
        public int Number { get; set; }
        public Station StationSrc { get; set; }
        public Station StationDst { get; set; }
        public double ArcValue { get; set; }
    }
}
