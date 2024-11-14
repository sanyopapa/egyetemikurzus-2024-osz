using System;

namespace VideoTeka
{
    public record Film(string Cim, string Mufaj, string Rendezo, int Hossz, int KiadasEve, string Leiras)
    {
        public void Adatok()
        {
            Console.WriteLine($"\tCím: {Cim}\n\tMűfaj: {Mufaj}\n\tRendező: {Rendezo}\n\tHossz: {Hossz} perc\n\tKiadás Éve: {KiadasEve}\n\tLeírás: {Leiras}\n\t");
        }
    }
}
