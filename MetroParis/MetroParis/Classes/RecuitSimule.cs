using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace MetroParis.Classes
{
    class RecuitSimule
    {
        private MainWindow Window0;


        public Solution bestSolution { get; set; }
        public Solution currentSolution { get; set; }
        public double InitialTemperature { get; set; }
      
        private double T = 0;
        private double decroissanceT;

        private int acceptedNumber;
        private int triedNumber;

        
        private  readonly int TOTALNODES = 376;
        private  int acceptationMultiplier = 12;
        private  int tryingMultipilier = 100;
        private double tauxZero;        

        public RecuitSimule(MainWindow _w0)
        {
            this.Window0 = _w0;
            bestSolution = new Solution();
            //this.bestSolution = bestSolution;
            this.InitialTemperature = 200;
            this.decroissanceT = 0.999;

            this.triedNumber = 0;
            this.acceptedNumber = 0;

            this.tauxZero = 0.8;

            LoadConfig();
        }

        public RecuitSimule(double initialTemp)
        {
            bestSolution = new Solution();
            //this.bestSolution = bestSolution;            
            this.InitialTemperature = initialTemp;
            this.decroissanceT = 0.999;

            this.triedNumber = 0;
            this.acceptedNumber = 0;

            this.tauxZero = 0.8;

            LoadConfig();
        }

        private void LoadConfig()
        {
            string taux0 = "";
            string decrTemp = "";
            string maxAcceptation = "";
            string maxIteration = "";

            taux0 = Window0.tbTauxZero.Text;
            decrTemp = Window0.tbTempDecr.Text;
            maxAcceptation = Window0.tbAccpt.Text;
            maxIteration = Window0.tbTEntatve.Text;

            this.acceptationMultiplier = int.Parse(maxAcceptation);
            this.tryingMultipilier = int.Parse(maxIteration);
            this.decroissanceT = double.Parse(decrTemp);
            this.tauxZero = double.Parse(taux0);
            
            // Definition de temperature initiale en fonction de Taux Zero
            this.InitialTemperature = -1200 / Math.Log(tauxZero);            
        }

        [STAThread]
        public void StartRecuit()
        {
            T = InitialTemperature;



            // Condition d'arret
            while (acceptedNumber < acceptationMultiplier * TOTALNODES && triedNumber < tryingMultipilier * TOTALNODES)
            {
                Window0.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Window0.tbTemperatureInitiale.Text = ((int)this.T).ToString()));
                Ratp ratp = new Ratp(Window0);

                double val = -1;
                Solution s = ratp.ParcourirMetros(out val);
                this.currentSolution = s;

                // Si c'est le debut du traitement, on initialise a la premiere boucle bestsolution
                if (this.bestSolution.CoutTotal < 100)
                    this.bestSolution = this.currentSolution;                

                // Acceptée
                if (this.currentSolution.CoutTotal < this.bestSolution.CoutTotal)
                {
                    // adopter directement la solution
                    T = T * this.decroissanceT;
                    triedNumber++;
                    acceptedNumber++;
                    this.bestSolution = this.currentSolution;
                }
                else
                {
                    // si sup deltaE ok                    

                    double r = new Random().NextDouble();
                    double deltaE = this.currentSolution.CoutTotal - this.bestSolution.CoutTotal;

                    double tauxZero = Math.Exp((-1) * deltaE / T);

                    // Acceptée a exp(-deltaE/T)
                    if (r <= tauxZero)
                    {
                        T = T * this.decroissanceT;
                        triedNumber++;
                        acceptedNumber++;
                        this.bestSolution = this.currentSolution;
                    }
                    // Refusée
                    else
                    {
                        T = T * this.decroissanceT;
                        triedNumber++;                        
                    }
                }

                Window0.textBlockParcours.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => Window0.textBlockParcours.Text = s.Texte));

                Window0.tbBestSolution.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => Window0.textBoxCoutTotal.Text = val.ToString()));

                Window0.textBoxCoutTotal.Dispatcher.BeginInvoke(DispatcherPriority.Background, 
                    new Action(() => Window0.tbBestSolution.Text = bestSolution.CoutTotal.ToString()));                

                Window0.tbAcceptes.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => Window0.tbAcceptes.Text = acceptedNumber.ToString()));

                Window0.tbTentes.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => Window0.tbTentes.Text = triedNumber.ToString()));
            }                   
        }


    }
}
