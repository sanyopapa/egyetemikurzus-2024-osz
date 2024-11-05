﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTeka
{
    public class Kolcsonzes
    {
        public string KolcsonzoNeve { get; set; }
        public Film KolcsonzottFilm { get; set; }
        public DateTime KolcsonzesDatuma { get; set; }
        public DateTime Hatarido { get; set; }
        public bool Lezarult { get; set; }

        public Kolcsonzes(Film film, DateTime kolcsonzesDatuma, DateTime hatarido)
        {
            KolcsonzottFilm = film;
            KolcsonzesDatuma = kolcsonzesDatuma;
            Hatarido = hatarido;
            Lezarult = false;
        }
    }
}

