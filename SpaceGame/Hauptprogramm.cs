using System;
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

            SpaceGame.Laden("test.xml");
            Console.WriteLine("Spiel geladen");
            //TestdatenErzeugen();

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

            SpaceGame.Speichern("test.xml");
            Console.WriteLine("Spiel gespeichert");
        }

        static void SimuliereRunde()
        {
            SpaceGame.Daten.Runde++;
            Console.WriteLine("Simuliere Runde " + SpaceGame.Daten.Runde);
            // Hier kommen alle Teile der Simulation in der passenden Reihenfolge rein
            AlleBewohnerArbeiten();

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

            Sektor s1 = new Sektor();
            s1.Name = "Beteigeuze";
            s1.X = 0;
            s1.Y = 1;
            s1.Metall = 100000;
            s1.Spookies = 200;
            s1.Bewohner = 30;

            Sektor s2 = new Sektor();
            s2.Name = "Rigel";
            s2.X = 1;
            s2.Y = 0;
            s2.Metall = 50;
            s2.Spookies = 1000;
            s2.Bewohner = 300;

            SpaceGame.Daten.Sektoren.Add(s0);
            SpaceGame.Daten.Sektoren.Add(s1);
            SpaceGame.Daten.Sektoren.Add(s2);

            Faktion polizei = new Faktion("poli", "zei", "Raumpolizei");
            Faktion piraten = new Faktion("pi", "raten", "FiesRaumPiraten");
            SpaceGame.Daten.Faktionen.Add(polizei);
            SpaceGame.Daten.Faktionen.Add(piraten);

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

        }
    }
}
