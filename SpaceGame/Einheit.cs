using System.Collections.Generic;
using System.Xml.Serialization;

namespace SpaceGame
{
    public class Einheit
    {
        // Nicht ignorierte Public-Felder werden im Savegame gespeichert
        public int FaktionsNummer;
        public string Name;
        public int Nummer;
        public int Spookies;
        public int Metall;
        public int Mitglieder;

        // private und XmlIgnore Felder werden nicht mitgespeichert, sondern dienen als temporäre Variablen für die aktuelle Runde
        [XmlIgnore]
        public string LangerBefehl = null;
        [XmlIgnore]
        public List<string> Befehle = new List<string>();
        private Sektor sektor = null;

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

        // Der Sektor darf nicht extra in XML gespeichert werden
        [XmlIgnore]
        public Sektor Sektor
        {
            get
            {
                return sektor;
            }

            set
            {
                // Beim Setzen muss die Einheit aus dem alten Sektor entfernt werden
                if(sektor != null)
                {
                    sektor.Einheiten.Remove(this);
                }
                sektor = value;
            }
        }

        public override string ToString()
        {
            return "  Einheit " + Name + " (" + Nummer + "), " + Faktion.ToString() + ", " + Mitglieder + " Personen, " + Spookies + " Spookies, " + Metall + " Metall." + System.Environment.NewLine;
        }
    }
}
