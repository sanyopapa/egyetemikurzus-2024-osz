using System;

namespace VideoTeka
{
    public class Film
    {
        public string Cim { get; set; }
        public string Mufaj { get; set; }
        public string Rendezo { get; set; }
        public int Hossz { get; set; }
        public int KiadasEve { get; set; }
        public string Leiras { get; set; }
        public bool Elerheto { get; set; } = true;

        public Film(string cim, string mufaj, string rendezo, int hossz, int kiadasEve, string leiras, bool elerheto)
        {
            Cim = cim;
            Mufaj = mufaj;
        }
        public Film() { }

        public void Adatok()
        {
            Console.WriteLine($"Cím: {Cim}\nMűfaj: {Mufaj}\nRendező: {Rendezo}\nHossz: {Hossz} perc\nKiadás Éve: {KiadasEve}\nLeírás: {Leiras}\nElérhető: {Elerheto}");
        }
    }
}
