using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetroParis.Classes
{
    static class Utilitaires
    {
        public static double[,] adjacence;
        public static List<Station> stations { get; set; }
        public static List<Link> links { get; set; }


        public static void LoadBase()
        {
            string filePath = Directory.GetCurrentDirectory() + "\\data_tp\\";

            string stationText = File.ReadAllText(filePath + "station.txt");
            string coordText = File.ReadAllText(filePath + "coords.txt");
            string arcsText = File.ReadAllText(filePath + "arcs.txt");

            Utilitaires.stations = new List<Station>();
            Utilitaires.links = new List<Link>();

            Utilitaires.stations = Utilitaires.GetStationsFromText(stationText, coordText);
            Utilitaires.links = Utilitaires.GetArcsFromText(arcsText, Utilitaires.stations);

            // Definition de la matrice d'adjacence
            Utilitaires.adjacence = adjacence = Utilitaires.GenerAdjacencyMatrix(links);                       
        }


        public static double[,] GenerAdjacencyMatrix(List<Link> links) 
        {
            // Definition de la matrice d'adjacence
            double[,] adjacence = new double[376, 376];
            for (int i = 0; i < 376; i++)
            {
                for (int j = 0; j < 376; j++)
                {
                    adjacence[i, j] = -1;
                }
            }

            foreach (Classes.Link cl in links)
            {
                int st1 = cl.StationSrc.Numero;
                int st2 = cl.StationDst.Numero;
                double val = cl.ArcValue;

                // Test que j'ai voulu 
                if (st1 == 16 || st2 == 16)
                {
                
                }

                adjacence[st1, st2] = val;
            }

            return adjacence;
        }

        /// <summary>
        /// Test de la liaison entre 2 Utilitaires.stations
        /// </summary>
        /// <param name="s1">station numero 1</param>
        /// <param name="s2">station numero 2</param>
        /// <param name="ll">la liste des liaisons de station</param>
        /// <returns> >=0 si il y a une liaison, sinon -1 </returns>
        public static double IsThereALink(Station s1, Station s2, List<Link> ll) 
        {
            foreach (Link l in ll) 
            {
                if ((l.StationSrc == s1 && l.StationDst == s2) || (l.StationSrc == s2 && l.StationDst == s1)) 
                {
                    double value = l.ArcValue;
                    return value;
                }
            }

            return -1;
        }


        /// <summary>
        /// Fonction de recuperation de la liste des Utilitaires.stations a partir d'un texte brut
        /// </summary>
        /// <param name="_stationText">le texte brut de Utilitaires.stations</param>
        /// <param name="_coordText">le texte brut des coordonnées de Utilitaires.stations</param>
        /// <returns>liste des Utilitaires.stations apres traitement du texte</returns>
        public static List<Station> GetStationsFromText(string _stationText, string _coordText) 
        {
            string[] lesStations = _stationText.Split('\n');
            string[] lescoords = _coordText.Split('\n');

            Dictionary<int, string> nodes = new Dictionary<int, string>(); // Numéro / Nom
            Dictionary<int, Point> coords = new Dictionary<int, Point>(); // Numéro / Coord
            
            List<Station> Stations = new List<Station>();

            // recuperation des Utilitaires.stations
            foreach (string a in lesStations)
            {
                string[] elts = a.Split(' ');
                int num = 0;
                if (!int.TryParse((elts[0]), out num))
                    continue;

                string nom = "";

                if (elts.Length > 2)
                {
                    for (int i = 1; i < elts.Length; i++)
                    {
                        nom = nom + " " + elts[i];
                    }
                }
                else if (elts.Length == 2)
                    nom = elts[1];

                if (!string.IsNullOrEmpty(nom))
                    nodes.Add(num, nom);
            }

            // recuperation des coordonnées
            foreach (string a in lescoords)
            {
                string[] elts = a.Split(' ');

                if (elts.Length == 3)
                {
                    int num = int.Parse(elts[0]);
                    double x = int.Parse(elts[1]);
                    double y = (-1) * int.Parse(elts[2]) + 750; // inversion des axes des Y

                    coords.Add(num, new Point(x, y));
                }
            }

            // Creation de la liste d'objets Utilitaires.stations
            for (int i = 0; i < nodes.Count; i++)
            {
                int num = nodes.ElementAt(i).Key;
                string nom = nodes.ElementAt(i).Value;
                Point coord;

                if (coords.Where(x => x.Key == num) != null && coords.Where(x => x.Key == num).Count() > 0)
                {
                    coord = coords.Where(x => x.Key == num).First().Value;

                    if (!string.IsNullOrEmpty(nom) && coord != null)
                    {
                        Station c = new Station();
                        c.Coordonee = coord;
                        c.Numero = num;
                        c.Nom = nom.Replace("\n","").Replace("\r","");
                        c.WasVisited = false;

                        Utilitaires.stations.Add(c);
                    }
                }
            }

            return Utilitaires.stations;
        }


        /// <summary>
        /// Fonction de recuperation de la liste des arcs de liaisons a partir d'un fichier brut
        /// </summary>
        /// <param name="_arcsText"></param>
        /// <param name="_Utilitaires.stations"></param>
        /// <returns></returns>
        public static List<Link> GetArcsFromText(string _arcsText, List<Station> _Stations) 
        {
            List<Link> links = new List<Link>();

            string[] lesarcs = _arcsText.Split('\n'); 

            int incrNumber = 1;
            foreach (string a in lesarcs)
            {
                string[] elts = a.Split(' ');

                if (elts.Length == 3)
                {
                    int n1 = 0;
                    if (!int.TryParse((elts[0]), out n1))
                        continue;
                    int n2 = 0;
                    if (!int.TryParse((elts[1]), out n2))
                        continue;
                    double val = 0;
                    if (!double.TryParse((elts[2].Replace(".",",")), out val))
                        continue;                  

                    var s1 = _Stations.Where(x => x.Numero == n1);
                    var s2 = _Stations.Where(x => x.Numero == n2);

                    if (s1 != null && s1.Count() > 0)
                    {
                        if (s2 != null && s2.Count() > 0)
                        {
                            Classes.Station st1 = s1.First();
                            Classes.Station st2 = s2.First();

                            Link tempLink = new Link() { StationSrc = st1, StationDst = st2, ArcValue = val, Number = incrNumber };
                            links.Add(tempLink);
                            incrNumber++;
                        }
                    }
                }
            }
            return links;
        }
    }
}
