using System.IO;
using System.Xml.Serialization;

namespace SpaceGame
{
    public static class SpaceGame {
        private static SpaceGameData daten = new SpaceGameData();

        public static SpaceGameData Daten
        {
            get
            {
                return daten;
            }
        }

        public static void Laden(string dateiname)
        {
            XmlSerializer sektorserializer = new XmlSerializer(typeof(SpaceGameData));
            TextReader savegamereader = new StreamReader(dateiname);
            daten = (SpaceGameData)sektorserializer.Deserialize(savegamereader);
            savegamereader.Close();
            ErstelleEinheitSektorReferenzen();
        }
        public static void Speichern(string dateiname)
        {
            XmlSerializer sektorserializer = new XmlSerializer(typeof(SpaceGameData));
            TextWriter savegamewriter = new StreamWriter(dateiname);
            sektorserializer.Serialize(savegamewriter, daten);
            savegamewriter.Close();
        }


        // Hilfsfunktionen zum Navigieren in den Daten
        public static Sektor FindeSektor(int x, int y)
        {
            foreach (Sektor kandidat in Daten.Sektoren)
            {
                if (kandidat.X == x && kandidat.Y == y)
                {
                    return kandidat;
                }
            }
            return null;
        }

        public static Faktion FindeFaktion(int nummer)
        {
            foreach (Faktion f in Daten.Faktionen)
            {
                if(f.Nummer == nummer)
                {
                    return f;
                }
            }
            return null;
        }

        public static Einheit FindeEinheit(int nummer)
        {
            foreach (Sektor s in Daten.Sektoren)
            {
                foreach (Einheit e in s.Einheiten)
                {
                    if (e.Nummer == nummer)
                    {
                        return e;
                    }
                }
            }
            return null;
        }

        public static void ErstelleEinheitSektorReferenzen()
        {
            foreach (Sektor s in SpaceGame.Daten.Sektoren)
            {
                foreach (Einheit e in s.Einheiten)
                {
                    e.Sektor = s;
                }
            }
        }
    }
}
