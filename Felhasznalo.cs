namespace VideoTeka
{
    public class Felhasznalo
    {
        public string Nev { get; set; }
        public List<Kolcsonzes> Kolcsonzesek { get; set; } = new List<Kolcsonzes>();

        public Felhasznalo(string nev)
        {
            Nev = nev;
        }

        public void UjKolcsonzes(Kolcsonzes kolcsonzes)
        {
            Kolcsonzesek.Add(kolcsonzes);
        }
    }
}

