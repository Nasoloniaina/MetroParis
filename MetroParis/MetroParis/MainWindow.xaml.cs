using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MetroParis.Classes;
using System.Threading;
using System.Windows.Threading;

namespace MetroParis
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread thread;
        public static int[] randomMatrix;
       
        public MainWindow()
        {            
            InitializeComponent();
            Utilitaires.LoadBase();
            randomMatrix = GenerRdmMatrice(false);
            GenerateMap();
           
        }

        

        private void GenerateMap()
        {          
            #region Premiere generation de la carte
            Ratp ratp = new Ratp(this);

            for (int i = 0; i < Utilitaires.stations.Count; i++)
            {
                string nom = Utilitaires.stations[i].Nom;
                Point coord = Utilitaires.stations[i].Coordonee;
                int id = Utilitaires.stations[i].Numero;

                Grid g = new Grid();

                Ellipse el = new Ellipse();

                el.StrokeThickness = 2;
                el.Stroke = new SolidColorBrush(Colors.Red);

                double left = coord.X * 3 - 1;
                double top = coord.Y * 3 - 1;

                double bottom = 500 - top - 2;

                el.Margin = new Thickness(left, top, 0, 0);

                TextBlock tb = new TextBlock();
                tb.Text = nom.ToString();

                tb.Margin = new Thickness(left, top, 0, 0);

                g.Children.Add(tb);

                grid1.Children.Add(g);
            }

            for (int i = 0; i < Utilitaires.links.Count; i++)
            {
                Line l = new Line();
                l.Stroke = new SolidColorBrush(Colors.Black);
                l.StrokeThickness = 1;

                l.X1 = Utilitaires.links[i].StationSrc.Coordonee.X * 3;
                l.Y1 = Utilitaires.links[i].StationSrc.Coordonee.Y * 3;
                l.X2 = Utilitaires.links[i].StationDst.Coordonee.X * 3;
                l.Y2 = Utilitaires.links[i].StationDst.Coordonee.Y * 3;

                double leftX1 = Utilitaires.links[i].StationSrc.Coordonee.X * 1;
                double topX1 = Utilitaires.links[i].StationSrc.Coordonee.Y * 1;

                grid1.Children.Add(l);
            }
            #endregion
        }

        [STAThread]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            RecuitSimule rs = new RecuitSimule(this);

            thread = new Thread(new ThreadStart(rs.StartRecuit));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }


        /// <summary>
        /// Fonction de generation d'un vecteur tableau de nombre aleatoire pour le parcours de la matrice d'adjacence
        /// Ceci a pour but d'eviter de ralentir le programme lors de la recherche aleatoire du trajet
        /// </summary>
        /// <returns></returns>
        public static int[] GenerRdmMatrice(bool _Really)
        {
            int[] matrix = new int[376];

            if (_Really)
            {
                for (int i = 0; i < matrix.Length - 1; i++)
                {
                    int rd = 0;
                    do
                    {
                        rd = (new Random()).Next(0, 376);
                    }
                    while (matrix.Contains(rd));

                    matrix[i] = rd;
                }
                return matrix;
            }
            else
            {
                for (int i = 0; i < matrix.Length - 1; i++)
                {
                    matrix[i] = i;
                }
                return matrix;
            }
        }

        /// <summary>
        /// Fonction qui sert a supprimer les elements de la solution precedente de l'affichage graphique
        /// </summary>
        public void ClearUnusedControls()
        {
            grid1.Children.Clear();
            GenerateMap();
            //textBlockParcours.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (thread != null) if (thread.IsAlive) thread.Abort(); 
        }
    }
}
