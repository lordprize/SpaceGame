namespace SpaceGame
{
    public class Faktion
    {
        public string Benutzername;
        public string Passwort;
        public string Name;
        public int Nummer;

        public Faktion()
        {

        }

        public Faktion(string benutzername, string passwort, string name)
        {
            Benutzername = benutzername;
            Passwort = passwort;
            Name = name;
            Nummer = SpaceGame.Daten.NächsteFaktionsNummer;
            SpaceGame.Daten.NächsteFaktionsNummer++;
        }

        public override string ToString()
        {
            return "Faktion " + Name + " (" + Nummer + ")";
        }
    }
}
