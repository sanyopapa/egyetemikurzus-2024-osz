using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTeka
{
    public class Kolcsonzes
    {
        public string FilmCim { get; set; }
        public DateTime KolcsonzesDatuma { get; set; }
        public DateTime Hatarido { get; set; }

        public Kolcsonzes(string filmCim, DateTime kolcsonzesDatuma, DateTime hatarido)
        {
            FilmCim = filmCim;
            KolcsonzesDatuma = kolcsonzesDatuma;
            Hatarido = hatarido;
        }
    }
}

