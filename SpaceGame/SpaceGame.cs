using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SpaceGame
{
    public static class SpaceGame {
        private static SpaceGameData daten = new SpaceGameData();
        private static System.Random zufall;

        public static SpaceGameData Daten
        {
            get
            {
                return daten;
            }
        }

        public static Random Zufall
        {
            get
            {
                if(zufall == null)
                {
                    zufall = new Random(Daten.NächsterRandomSeed);
                }
                return zufall;
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
            Daten.NächsterRandomSeed = Zufall.Next();
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

        /// <summary>
        /// Liefert eine zufällig sortiere Liste aller Einheiten im Spiel
        /// </summary>
        /// <returns>eine zufällig sortiere Liste aller Einheiten im Spiel</returns>
        public static List<Einheit> HoleAlleEinheiten()
        {
            List<Einheit> ergebnis = new List<Einheit>();
            foreach (Sektor s in SpaceGame.Daten.Sektoren)
            {
                foreach (Einheit e in s.Einheiten)
                {
                    ergebnis.Add(e);
                }
            }
            MischenListe(ergebnis);
            return ergebnis;
        }

        /// <summary>
        /// Mischt eine Liste mit Hilfe des spieleigenen Zufallsgenerators
        /// nach dem Mischalgorithmus von Fisher und Yates
        /// </summary>
        /// <typeparam name="T">Der Datentyp der Objekte in der Liste</typeparam>
        /// <param name="list">Die zu mischende Liste</param>
        public static void MischenListe<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Zufall.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
