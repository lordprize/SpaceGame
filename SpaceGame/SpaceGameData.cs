﻿using System.Collections.Generic;

namespace SpaceGame
{
    // Diese Klasse enthält alles, was zum aktuellen Spielstand gehört
    public class SpaceGameData
    {
        // Hier kommen alle Daten des Spiels rein
        public int Runde = 0;
        public int NächsteEinheitenNummer = 1;
        public int NächsteFaktionsNummer = 1;
        public List<Sektor> Sektoren = new List<Sektor>();
        public List<Faktion> Faktionen = new List<Faktion>();
        public int NächsterRandomSeed = 1;
    }
}
