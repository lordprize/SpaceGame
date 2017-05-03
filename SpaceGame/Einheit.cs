using System.Collections.Generic;

namespace SpaceGame
{
    public class Einheit
    {
        public int FaktionsNummer;
        public string Name;
        public int Nummer;

        public int Spookies;
        public int Metall;
        public int Mitglieder;

        private string langerBefehl = null;
        private List<string> befehle = new List<string>();

        public Einheit()
        {

        }

        public Einheit(Faktion faktion, string name)
        {
            FaktionsNummer = faktion.Nummer;
            Name = name;
            Nummer = SpaceGame.Daten.NächsteEinheitenNummer;
            SpaceGame.Daten.NächsteEinheitenNummer++;
        }

        public Faktion Faktion
        {
            get
            {
                return SpaceGame.FindeFaktion(FaktionsNummer);
            }
        }

        public string LangerBefehl
        {
            get
            {
                return langerBefehl;
            }

            set
            {
                langerBefehl = value;
            }
        }

        public List<string> Befehle
        {
            get
            {
                return befehle;
            }

            set
            {
                befehle = value;
            }
        }

        public override string ToString()
        {
            return "  Einheit " + Name + " (" + Nummer + "), " + Faktion.ToString() + ", " + Mitglieder + " Personen, " + Spookies + " Spookies, " + Metall + " Metall." + System.Environment.NewLine;
        }
    }
}
