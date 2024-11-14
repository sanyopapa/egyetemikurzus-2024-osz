using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VideoTeka;

namespace BFAM77
{
    public class FilmWithAvailability
    {
        public Film Film { get; }
        public bool Elerheto { get; }

        public FilmWithAvailability(Film film, bool elerheto)
        {
            Film = film;
            Elerheto = elerheto;
        }
    }


}
