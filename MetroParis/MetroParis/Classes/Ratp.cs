using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;

namespace MetroParis.Classes
{
    class Ratp
    {
        MainWindow window0;

        public static List<Solution> sols = new List<Solution>();
        

        string allText;       

        List<Station> StationsVisitees; 

        double totalCout;
        bool[] visited;
        Station startStation;

        /// <summary>
        /// Constructeur
        /// </summary>
        public Ratp(MainWindow _w0) 
        {
            this.window0 = _w0;
            if (!Init()) return; 
  
            int strtStationIndex = (new Random()).Next(0, Utilitaires.stations.Count() - 1);

            // Generation aleatoire de la station de depart
            while (this.startStation == null)
            {
                this.startStation = Utilitaires.stations.Where(x => x.Numero == strtStationIndex).First();
                strtStationIndex = (new Random()).Next(0, Utilitaires.stations.Count() - 1);
            }
                        
            //this.startStation = this.Utilitaires.stations.Where(x => x.Numero == 67).First();
        }


        public void SetStartStation(Station _s) 
        {
            this.startStation = _s;
        }


        public Station GetStartStation() 
        {
            return this.startStation;
        }


        /// <summary>
        /// Fonction de recuperation d'un voisin proche et qui n'est pas encore visité
        /// </summary>
        /// <param name="_s"></param>
        /// <param name="arcValue"></param>
        /// <returns></returns>
        private Station GetUnvisitedChildNode(Station _s, out double arcValue)
        {
            int index = Utilitaires.stations.IndexOf(_s);
            arcValue = -1;            
            int j = 0;           

            int rnd = new Random().Next(0, 2);

            // Gestion de hasard > 0 ou < max

            switch (rnd)
            {
                case 0:
                    while (j < MainWindow.randomMatrix.Length)
                    {
                        int currentIndex = MainWindow.randomMatrix[j];
                        arcValue = Utilitaires.adjacence[index, currentIndex];

                        if (arcValue > -1)
                        {
                            if ((Utilitaires.stations.ElementAt(currentIndex)).WasVisited == false)
                            {
                                return Utilitaires.stations.ElementAt(currentIndex);
                            }
                        }
                        j++;
                    }
                    break;

                case 1:
                    j = MainWindow.randomMatrix.Length;
                    while (j > 0)
                    {
                        int currentIndex = MainWindow.randomMatrix[j - 1];
                        arcValue = Utilitaires.adjacence[index, currentIndex];

                        if (arcValue > -1)
                        {
                            if ((Utilitaires.stations.ElementAt(currentIndex)).WasVisited == false)
                            {
                                return Utilitaires.stations.ElementAt(currentIndex);
                            }
                        }
                        j--;
                    }
                    break;                    
            }       
            return null;
        }

	
        private bool IsAllVisited(Dictionary<int, bool> _dict) 
        {
            foreach (var a in _dict) 
            {
                if (!a.Value)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Fonction principale de parcours
        /// </summary>
        /// <param name="totalval"></param>
        /// <returns></returns>
        public Solution ParcourirMetros(out double totalval)
        {
            window0.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => window0.ClearUnusedControls()));
                     
            Solution solution = new Solution();

            double arcval = -1;

            // Parcours en profondeur // Ajout modification a l'algorithme : random parcours au lieu de gauche
            Stack stack = new Stack();
            stack.Push(this.startStation);
            while (this.startStation == null)
            {
                int strtStationIndex = (new Random()).Next(0, Utilitaires.stations.Count() - 1); ;
                this.startStation = Utilitaires.stations.Where(x => x.Numero == strtStationIndex).First();
            }
            this.startStation.WasVisited = true;
            allText += PrintStation(this.startStation);
            solution.Stations.Add(this.startStation);

            while (stack.Count > 0)
            {
                if (stack.Count <= 0)
                {

                }

                Station n = (Station)stack.Peek();
                
                Station child = GetUnvisitedChildNode(n, out arcval);

                // Si il y a un fils, donc parcourir le fils
                if (child != null && arcval >= 0)
                {
                    int ind = Utilitaires.stations.IndexOf(child);

                    Utilitaires.stations[ind].WasVisited = true;                  
                    allText += PrintStation(child);
                    solution.Stations.Add(child);
                    stack.Push(child);

                    #region Tracage lignes en rouge                   

                    window0.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => window0.grid1.Children.Add(
                        new Line()
                        {
                            Stroke = new SolidColorBrush(Colors.Red),
                            StrokeThickness = 5,
                            X1 = n.Coordonee.X * 3,
                            Y1 = n.Coordonee.Y * 3,
                            X2 = child.Coordonee.X * 3,
                            Y2 = child.Coordonee.Y * 3
                        })
                     ));

                    #endregion

                    #region Marquage
                    double left = child.Coordonee.X * 3 - 1;
                    double top = child.Coordonee.Y * 3 - 1;


                    window0.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => window0.grid1.Children.Add(
                        new TextBlock()
                        {
                            Text = child.Nom,
                            Foreground = new SolidColorBrush(Colors.Blue),
                            Margin = new Thickness(left, top, 0, 0),
                        })
                     ));
                    #endregion
                    totalCout += arcval > -1 ? arcval : 0;
                }
                else
                {
                    // Il n'y a pas de noeud fils, on redescend
                    Station before = (Station)stack.Peek();
                    stack.Pop();                    

                    if (stack.Count > 0)
                    {
                        Station after = (Station)stack.Peek();

                        double distance = Utilitaires.adjacence[before.Numero, after.Numero];
                        totalCout += distance;

                        allText += PrintStation((Station)stack.Peek());
                        solution.Stations.Add((Station)stack.Peek());
                    }
                }                
            }
            totalval = totalCout;

            solution.CoutTotal = totalCout;
            Ratp.sols.Add(solution);
            solution.Texte = allText;

            // Nettoyage de l'etat WasVisited de chaque node
            ClearStation();
            return solution;
        }
        

        /// <summary>
        /// Reset de l'etat des Utilitaires.stations a non visité
        /// </summary>
        private void ClearStation() 
        {
            int i = 0;
            while (i < Utilitaires.stations.Count)
            {
                Utilitaires.stations[i].WasVisited = false;               
                i++;
            }
        }

        /// <summary>
        /// Affichage / Ajout du parcours a la liste dediée
        /// </summary>
        /// <param name="_s"></param>
        /// <returns></returns>
        private string PrintStation(Station _s) 
        {
            return string.Format("{0} ({1})\n", _s.Numero, _s.Nom);
        }


        /// <summary>
        /// Affichage / Ajout du parcours a la liste dediée
        /// </summary>
        /// <param name="_s"></param>
        /// <returns></returns>
        private string PrintStation(Station _s, double _cout)
        {
            return string.Format("{0} ({1})\n", _s.Numero, _s.Nom);
        }

        /// <summary>
        /// Initialisation des valeurs a traiter
        /// </summary>
        /// <returns></returns>
        public bool Init() 
        {
            try
            {
                totalCout = 0;
                allText = "";
                StationsVisitees = new List<Station>();
                visited = new bool[376];  
              
                for (int i = 0; i < visited.Length; i++)
                {
                    visited[i] = false;
                    Utilitaires.stations[i].WasVisited = false;
                }

                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }
        

        /// <summary>
        /// Fonction qui sert a supprimer les elements de la solution precedente de l'affichage graphique
        /// </summary>
        private void ClearUnusedControls()
        {

            for (int i = 0; i < window0.grid1.Children.Count; i++)
            {
                var actualChild = window0.grid1.Children[i];
                if (actualChild is Line)
                {
                    SolidColorBrush controlColor = (SolidColorBrush)(((Line)actualChild).Stroke);
                    SolidColorBrush referenceColor = new SolidColorBrush(Colors.Red);

                    if (controlColor == referenceColor)
                    {
                        window0.grid1.Children.Remove(actualChild);
                    }
                    else if (((Line)actualChild).StrokeThickness <= 7 && ((Line)actualChild).StrokeThickness >= 3)
                    {
                        window0.grid1.Children.Remove(actualChild);
                    }
                }
                else if (actualChild is TextBlock)
                {
                    SolidColorBrush controlColor = (SolidColorBrush)(((TextBlock)actualChild).Foreground);
                    SolidColorBrush referenceColor = new SolidColorBrush(Colors.Blue);

                    if (controlColor.Color == referenceColor.Color)
                    {
                        window0.grid1.Children.Remove(actualChild);
                    }
                }
            }

            window0.textBlockParcours.Text = "";
        }

    }
}
