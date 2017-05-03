using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame
{
    public class Sektor
    {
        public string Name;
        public int Spookies;
        public int Metall;
        public int Bewohner;
        public int X;
        public int Y;
        public List<Einheit> Einheiten = new List<Einheit>();

        public override string ToString()
        {
            string ausgabe = String.Format("Sektor {0} ({1},{2}) hat {3} Bewohner, {4} Spookies und {5} Metall." + System.Environment.NewLine, Name, X, Y, Bewohner, Spookies, Metall);
            if(Einheiten.Count == 0)
            {
                ausgabe += "Es sind keine Einheiten in diesem Sektor." + System.Environment.NewLine;
            } else
            {
                ausgabe += "Es sind folgende " + Einheiten.Count + " Einheiten in diesem Sektor:" + System.Environment.NewLine;
                foreach (Einheit einheit in Einheiten)
                {
                    ausgabe += einheit.ToString();
                }
            }
            return ausgabe;
        }
    }
}
