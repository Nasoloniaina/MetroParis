using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MetroParis.Classes;

namespace MetroParis
{
    class Graphe
    {
        double totalCout;
        string allText;
        List<Station> stationsVisitees;
        double[,] adjacence;
        bool[] visited;

        public Graphe() 
        {            
            totalCout = 0;
            allText = "";
            stationsVisitees = new List<Station>();
            visited = new bool[376];
            for (int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;   
            }
        }




        private void Init()
        {        
            string filePath = Directory.GetCurrentDirectory() + "\\data_tp\\";

            string stationText = File.ReadAllText(filePath + "station.txt");
            string coordText = File.ReadAllText(filePath + "coords.txt");
            string arcsText = File.ReadAllText(filePath + "arcs.txt");

            List<Classes.Station> stations = new List<Classes.Station>();
            List<Classes.Link> links = new List<Classes.Link>();

            stations = Utilitaires.GetStationsFromText(stationText, coordText);
            links = Utilitaires.GetArcsFromText(arcsText, stations);

            // Definition de la matrice d'adjacence
            adjacence = adjacence = Utilitaires.GenerAdjacencyMatrix(links);

            // Generation de la premiere station
            Random rnd = new Random();
            int first = rnd.Next(0, 375);
            first = 89;

            // Clear
            allText = "";           

            Station noeudCourant = stations.Where(x => x.Numero == first).First();
            allText += string.Format("Station de depart = {0} ({1})", noeudCourant.Nom, noeudCourant.Numero);
            stationsVisitees.Add(noeudCourant);

            // test de parcours // a supprimer
            for (int i = 0; i < 200; i++)
            {
                noeudCourant = ComputeNext(stations, noeudCourant);
                stationsVisitees.Add(noeudCourant);
            }
            allText += string.Format("\nCout Total = " + totalCout);            
        }


        private Station ComputeNext(List<Station> _stations, Station _noeudCourant)
        {
            for (int i = 0; i < _stations.Count; i++)
            {
                int indexCourant = _stations.IndexOf(_noeudCourant);


                // Test si il y a liaison, si -1 donc pas de liaison
                if (adjacence[indexCourant, i] >= 0)
                {
                    Station nextStation = _stations[i];
                    // Si la liste des stations deja parcourues ne contient pas la stations etudié

                    if (!stationsVisitees.Contains(nextStation))
                    {
                        _noeudCourant = nextStation;
                        totalCout += adjacence[indexCourant, i];
                        allText += string.Format("\nStation = " + _noeudCourant.Nom + " (" + _noeudCourant.Numero + ")");
                        break;
                    }
                    else
                    {
                        continue;                       
                    }
                }
            }
            return _noeudCourant;
        }
       
    }
}
