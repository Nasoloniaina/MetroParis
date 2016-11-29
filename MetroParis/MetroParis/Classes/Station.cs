using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetroParis.Classes
{
    // 120 dans un lien est consideré comme sans attente en correspondance
    class Station
    {
        public int Numero { get; set; }
        public string Nom { get; set; }
        public Point Coordonee { get; set; }
        public bool WasVisited { get; set; }
    }
}
