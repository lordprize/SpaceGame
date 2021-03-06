﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SpaceGame
{
    class Hauptprogramm
    {
        static void Main(string[] args)
        {
            // ToDo:
            // - Befehle definieren
            //   - Befehl NACH zum Bewegen
            //   - Befehl ATTACKIERE zum Angreifen
            //   - Befehl ERPRESSE gibt eigenen Fraktion mehr Resourcen
            // - Befehle einlesen und bearbeiten
            // - Auswertungen für jede Faktion einzeln ausgeben
            // - Neue Resource: Sprit, benötigt zum Truppen bewegen

            if (File.Exists("savegame.xml")) {
                SpaceGame.Laden("savegame.xml");
            } else {
                TestdatenErzeugen();
            }
            Console.WriteLine("Spiel geladen, starte Auswertung mit Random-Seed " + SpaceGame.Daten.NächsterRandomSeed);

            LeseBefehle("testbefehle.txt");
            SimuliereRunde();
            
            StreamWriter auswertung = File.CreateText("auswertung-" + SpaceGame.Daten.Runde + ".txt");
            auswertung.WriteLine("Auswertung für SpaceGame, Runde " + SpaceGame.Daten.Runde);
            foreach(Sektor s in SpaceGame.Daten.Sektoren)
            {
                auswertung.WriteLine(s);
            }
            auswertung.WriteLine("Ende der Auswertung.");
            auswertung.Close();
            Console.WriteLine("Auswertung fertig geschrieben");

            SpaceGame.Speichern("savegame.xml");
            Console.WriteLine("Spiel gespeichert");
        }

        /// <summary>
        /// Liest die Befehle ein, prüft Logindaten und ordnet die Befehle den richtigen Einheiten zu.
        /// Zeilen, die mit # beginnen, sind Kommentarzeilen. Leere Zeilen werden ignoriert.
        /// </summary>
        /// <param name="dateiname">Dateiname der Befehlsdatei</param>
        static void LeseBefehle(string dateiname)
        {
            StreamReader befehle = File.OpenText(dateiname);
            Faktion f = null;
            Einheit e = null;
            int zeilennummer = 0;
            while (!befehle.EndOfStream)
            {
                string zeile = befehle.ReadLine();
                zeilennummer++;
                // Ersetze allen Whitespace (Tab, Leerzeichen) durch ein einzelnes Leerzeichen
                zeile = System.Text.RegularExpressions.Regex.Replace(zeile, @"\s+", " ");
                // Entferne Leerzeichen am Anfang und am Ende der Zeile
                zeile = zeile.Trim();
                if (zeile.Length == 0)
                {
                    // Überspringe leere Befehlszeilen
                    continue;
                }
                string[] teile = zeile.Split(' ');

                string befehl = teile[0].ToUpper();
                if(befehl.StartsWith("#"))
                {
                    // Kommentar überspringen
                    continue;
                }
                switch (befehl)
                {
                    case "FAKTION":
                        f = PrüfeBefehlFaktion(teile);
                        if (f == null)
                        {
                            Console.WriteLine("Fehler in Befehlszeile " + zeilennummer + ". Der Befehl war: " + zeile);
                        }
                        break;
                    case "FAKTIONSENDE":
                        // Logout als Faktion
                        f = null;
                        break;
                    case "EINHEIT":
                        // Einheit auswählen
                        e = PrüfeBefehlEinheit(teile, f);
                        if(e == null)
                        {
                            Console.WriteLine("Fehler in Befehlszeile " + zeilennummer + ". Der Befehl war: " + zeile);
                        }
                        break;
                    case "NACH":
                        // Einheit bewegen
                        if(e != null)
                        {
                            e.LangerBefehl = (string[])teile.Clone();
                        } else
                        {
                            Console.WriteLine("Fehler in Befehlszeile " + zeilennummer + " (Keine Einheit ausgewählt). Der Befehl war: " + zeile);
                        }
                        break;
                    case "REKRUTIERE":
                        if (e != null)
                        {
                            e.Befehle.Add((string[])teile.Clone());
                        }
                        else
                        {
                            Console.WriteLine("Fehler in Befehlszeile " + zeilennummer + " (Keine Einheit ausgewählt). Der Befehl war: " + zeile);
                        }
                        break;
                    default:
                        Console.WriteLine("Unbekannter Befehl in Zeile " + zeilennummer + ". Der Befehl war: " + zeile);
                        break;
                }

            }
        }

        /// <summary>
        /// Prüft, ob der Befehl "FAKTION faktionsnr username passwort" passt.
        /// Liefert bei Erfolg die neue Faktion zurück, ansonsten null.
        /// </summary>
        /// <param name="teile">Ein string-Array mit den vier Bestandteilen des Befehls</param>
        /// <returns>Die passende Faktion, sonst null</returns>
        static Faktion PrüfeBefehlFaktion(string[] teile)
        {
            if (teile.Length != 4)
            {
                return null;
            }
            int faktionsnummer = -1;
            if (!int.TryParse(teile[1], out faktionsnummer))
            {
                return null;
            }
            Faktion kandidat = SpaceGame.FindeFaktion(faktionsnummer);
            if (kandidat == null)
            {
                return null;
            }
            if (kandidat.Benutzername != teile[2] || kandidat.Passwort != teile[3])
            {
                Console.WriteLine("Falsche Logindaten für Faktion " + kandidat.Nummer);
                return null;
            }
            return kandidat;
        }

        /// <summary>
        /// Prüft, ob der Befehl "EINHEIT einheitennr" passt und die entsprechende Faktion
        /// berechtigt ist, diese Einheit zu befehligen.
        /// Liefert bei Erfolg die passende Einheit zurück, sonst null.
        /// </summary>
        /// <param name="teile">Ein string-Array mit den vier Bestandteilen des Befehls</param>
        /// <param name="f">Die aktive Faktion</param>
        /// <returns>Die passende Einheit, sonst null</returns>
        static Einheit PrüfeBefehlEinheit(string[] teile, Faktion f)
        {
            if(teile.Length < 2 || f == null)
            {
                return null;
            }
            int einheitennummer = -1;
            if (!int.TryParse(teile[1], out einheitennummer))
            {
                return null;
            }
            Einheit einheit = SpaceGame.FindeEinheit(einheitennummer);
            if(einheit == null)
            {
                return null;
            }
            if(einheit.Faktion != f)
            {
                Console.WriteLine("Falsche Faktion für Einheit " + einheit.Nummer);
                return null;
            }
            return einheit;
        }

        static void SimuliereRunde()
        {
            SpaceGame.Daten.Runde++;
            Console.WriteLine("Simuliere Runde " + SpaceGame.Daten.Runde);
            // Hier kommen alle Teile der Simulation in der passenden Reihenfolge rein
            AlleBewohnerArbeiten();
            EinheitenRekrutierenPersonen();
            EinheitenBewegen();

            Console.WriteLine("Simulation der Runde " + SpaceGame.Daten.Runde + " fertig.");
        }


        static void AlleBewohnerArbeiten()
        {
            // Alle Bewohner arbeiten und erzeugen damit Spookies und Metall
            // Falls die Bewohner weniger als 10% des vorhandenen Metalls sind,
            // können sie sich um 3% vermehren
            foreach (Sektor s in SpaceGame.Daten.Sektoren)
            {
                s.Spookies += s.Bewohner;
                s.Metall += s.Bewohner;
                if(s.Bewohner < (int)(0.1*s.Metall))
                {
                    s.Bewohner += (int)(0.03 * s.Bewohner);
                }
            }
        }

        static void EinheitenRekrutierenPersonen()
        {
            foreach (Einheit e in SpaceGame.HoleAlleEinheiten())
            {
                foreach(string[] befehl in e.Befehle)
                {
                    if(befehl[0].ToUpper() == "REKRUTIERE")
                    {
                        // Bingo - diese Einheit will rekrutieren
                        int anzahl = 0;
                        if(!int.TryParse(befehl[1], out anzahl))
                        {
                            Console.WriteLine("Fehler im REKRUTIERE-Befehl: keine gültige Anzahl");
                            continue; // Zeile überspringen
                        }
                        if(anzahl < 1)
                        {
                            Console.WriteLine("Fehler im REKRUTIERE-Befehl: keine gültige Anzahl");
                            continue; // Zeile überspringen
                        }
                        // die Einheit kann nicht mehr rekrutieren, als es Bewohner gibt
                        anzahl = Math.Min(anzahl, e.Sektor.Bewohner);
                        // die Einheit kann nicht mehr rekrutieren, als sie sich leisten kann (10 Spookies pro Person)
                        anzahl = Math.Min(anzahl, e.Spookies / 10);

                        // Führe die Rekrutierung durch
                        e.Sektor.Bewohner -= anzahl;
                        e.Mitglieder += anzahl;
                        e.Sektor.Spookies += anzahl * 10;
                        e.Spookies -= anzahl * 10;
                        Console.WriteLine("Einheit " + e.Nummer + " in (" + e.Sektor.X + "/" + e.Sektor.Y + ") rekrutiert " + anzahl + " Mitglieder.");
                    }
                }
            }
        }

        /// <summary>
        /// Bewegt alle Einheiten, die sich bewegen wollten
        /// </summary>
        static void EinheitenBewegen()
        {
            foreach(Einheit e in SpaceGame.HoleAlleEinheiten())
            {
                if(e.LangerBefehl == null || e.LangerBefehl[0].ToUpper() != "NACH" || e.LangerBefehl.Length != 2)
                {
                    // Einheit wollte sich gar nicht bewegen - überspringen
                    continue;
                }
                string richtung = e.LangerBefehl[1].ToUpper();
                int dx = 0;
                int dy = 0;
                switch(richtung)
                {
                    case "OBEN": dy = -1; break;
                    case "UNTEN": dy = 1; break;
                    case "LINKS": dx = -1; break;
                    case "RECHTS": dx = 1; break;
                    default:
                        Console.WriteLine("Fehler im NACH-Befehl von Einheit " + e.Nummer + ". Richtung " + richtung + " nicht erkannt.");
                        continue;
                }
                int x = e.Sektor.X + dx;
                int y = e.Sektor.Y + dy;
                Sektor ziel = SpaceGame.FindeSektor(x, y);
                if(ziel == null)
                {
                    Console.WriteLine("Fehler im NACH-Befehl von Einheit " + e.Nummer + ". Bei (" + x + "," + y + ") gibt es keinen Sektor.");
                    continue;
                }
                // Wir haben einen gültigen Zielsektor. Bewege die Einheit!
                e.Sektor = ziel;
                ziel.Einheiten.Add(e);
            }
        }

        static void TestdatenErzeugen()
        {
            SpaceGame.Daten.Sektoren = new List<Sektor>();
            SpaceGame.Daten.Faktionen = new List<Faktion>();
            SpaceGame.Daten.NächsteFaktionsNummer = 1;
            SpaceGame.Daten.NächsteEinheitenNummer = 1;

            Sektor s0 = new Sektor();
            s0.Name = "Milchstrasse";
            s0.X = 0;
            s0.Y = 0;
            s0.Metall = 100000;
            s0.Spookies = 20000;
            s0.Bewohner = 3244;
            SpaceGame.Daten.Sektoren.Add(s0);

            Sektor s1 = new Sektor();
            s1.Name = "Beteigeuze";
            s1.X = 0;
            s1.Y = 1;
            s1.Metall = 100000;
            s1.Spookies = 200;
            s1.Bewohner = 30;
            SpaceGame.Daten.Sektoren.Add(s1);

            Sektor s2 = new Sektor();
            s2.Name = "Rigel";
            s2.X = 1;
            s2.Y = 0;
            s2.Metall = 50;
            s2.Spookies = 1000;
            s2.Bewohner = 300;
            SpaceGame.Daten.Sektoren.Add(s2);

            Sektor s3 = new Sektor();
            s3.Name = "Azeroth";
            s3.X = 1;
            s3.Y = 1;
            s3.Metall = 350;
            s3.Spookies = 1430;
            s3.Bewohner = 250;
            SpaceGame.Daten.Sektoren.Add(s3);

            Faktion polizei = new Faktion("poli", "zei", "Raumpolizei");
            Faktion piraten = new Faktion("pi", "raten", "FiesRaumPiraten");
            Faktion moench = new Faktion("moe", "nch", "FriedvollMoenche");
            SpaceGame.Daten.Faktionen.Add(polizei);
            SpaceGame.Daten.Faktionen.Add(piraten);
            SpaceGame.Daten.Faktionen.Add(moench);

            Einheit streifealpha = new Einheit(polizei, "Weltraumstreife Alpha");
            streifealpha.Mitglieder = 10;
            streifealpha.Metall = 1000;
            streifealpha.Spookies = 1000;
            s0.Einheiten.Add(streifealpha);

            Einheit pirat1 = new Einheit(piraten, "Raumbart der Schreckliche");
            pirat1.Mitglieder = 1;
            pirat1.Metall = 100000;
            pirat1.Spookies = 333333;
            s0.Einheiten.Add(pirat1);

            Einheit pirat2 = new Einheit(piraten, "Quatschibus Crew");
            pirat2.Mitglieder = 8;
            pirat2.Metall = 10;
            pirat2.Spookies = 80;
            s0.Einheiten.Add(pirat2);

            Einheit pirat3 = new Einheit(piraten, "Schutzcrew Beteigeuze");
            pirat3.Mitglieder = 10;
            pirat3.Metall = 100;
            pirat3.Spookies = 800;
            s1.Einheiten.Add(pirat3);
            
            Einheit moench1 = new Einheit(moench, "Zen Moenche");
            moench1.Mitglieder = 5;
            moench1.Metall = 100;
            moench1.Spookies = 150;
            s0.Einheiten.Add(moench1);

            SpaceGame.ErstelleEinheitSektorReferenzen();
        }
    }
}
