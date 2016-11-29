using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroParis.Classes
{
    class Solution
    {
        public List<Station> Stations { get; set; }
        public double CoutTotal { get; set; }
        public string Texte { get; set; }

        public Solution() 
        {
            Stations = new List<Station>();
        }
    }
}
    