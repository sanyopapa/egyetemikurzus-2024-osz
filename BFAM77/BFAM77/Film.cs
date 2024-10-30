using System;

namespace VideoTeka
{
    public class Film
    {
        public string Cim { get; set; }
        public string Mufaj { get; set; }
        public bool Elerheto { get; set; } = true;

        public Film(string cim, string mufaj)
        {
            Cim = cim;
            Mufaj = mufaj;
        }

        public void Adatok()
        {
            Console.WriteLine($"Cím: {Cim}, Műfaj: {Mufaj}, Elérhető: {Elerheto}");
        }
    }
}
