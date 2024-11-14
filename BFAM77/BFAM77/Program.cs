using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using BFAM77;

namespace VideoTeka
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { "help", ShowHelp },
                { "add_new_film", AddNewFilm },
                { "show_all_films", ShowAllFilms },
                { "show_shortest_film", ShowMinFilm },
                { "show_longest_film", ShowMaxFilm },
                { "rent_film", RentFilm },
                { "return_film", ReturnFilm },
                { "search_film", SearchFilm },
                { "delete_film", DeleteFilm },
                { "show_all_rents", ShowAllRents },
                { "show_unreturned_rents", ShowUnreturnedRents },
                { "show_returned_rents", ShowReturnedRents },
                { "show_overdue_rents", ShowOverdueRents },
                { "show_active_rents", ShowActiveRents },
                { "search_rents", SearchRents },
                { "exit", Exit }
            };

            Console.WriteLine("Ez itt a Képszínház videotéka alkalmazása.");
            Console.WriteLine("A 'help' parancs segítségével megnézheted, milyen parancsok adhatók ki.");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (commands.ContainsKey(input))
                {
                    commands[input].Invoke();
                }
                else
                {
                    Console.WriteLine("Ismeretlen parancs. Írj be 'help'-et a parancsok listájához.");
                }
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Elérhető parancsok:");
            Console.WriteLine("\thelp - Parancsok listájának megjelenítése");
            Console.WriteLine("\tadd_new_film - Új film hozzáadása");
            Console.WriteLine("\tshow_all_films - Összes film kiírása");
            Console.WriteLine("\tshow_shortest_film - Legrövidebb film adatainak kiírása");
            Console.WriteLine("\tshow_longest_film - Leghosszabb film adatainak kiírása");
            Console.WriteLine("\trent_film - Film kölcsönzése");
            Console.WriteLine("\treturn_film - Film visszahozása");
            Console.WriteLine("\tsearch_film - Film keresése");
            Console.WriteLine("\tdelete_film - Film törlése");
            Console.WriteLine("\tshow_all_rents - Összes kölcsönzés megjelenítése");
            Console.WriteLine("\tshow_unreturned_rents - Vissza nem hozott kölcsönzések megjelenítése");
            Console.WriteLine("\tshow_returned_rents - Visszahozott kölcsönzések megjelenítése");
            Console.WriteLine("\tshow_overdue_rents - Lejárt kölcsönzések megjelenítése (Amik lejártak, de még nem hozták őket vissza)");
            Console.WriteLine("\tshow_active_rents - Aktív kölcsönzések megjelenítése (Amiket még nem hoztak vissza, és nem jártak le)");
            Console.WriteLine("\tsearch_rents - Kölcsönzések keresése (a bérlő neve szerint)");
            Console.WriteLine("\texit - Kilépés az alkalmazásból");
        }

        private static void Exit()
        {
            Console.WriteLine("Kilépés...");
            Environment.Exit(0);
        }

        private static void AddNewFilm()
        {
            try
            {
                string cim = InputHelper.GetStringInput("Add meg a film címét:");
                string mufaj = InputHelper.GetStringInput("Add meg a film műfaját:");
                string rendezo = InputHelper.GetStringInput("Add meg a film rendezőjét:");
                int hossz = InputHelper.GetIntInput("Add meg a film hosszát (percben):");
                int kiadasEve;

                while (true)
                {
                    kiadasEve = InputHelper.GetIntInput("Add meg a film kiadás évét:");
                    if (kiadasEve <= DateTime.Now.Year)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Hiba: A kiadás éve nem lehet nagyobb, mint {DateTime.Now.Year}!");
                    }
                }

                string leiras = InputHelper.GetStringInput("Add meg a film leírását:");

                var newFilm = new Film(cim, mufaj, rendezo, hossz, kiadasEve, leiras);
                var filmWithAvailability = new FilmWithAvailability(newFilm, true);

                string filmsFilePath = "films.json";

                List<FilmWithAvailability> filmsWithAvailability = new List<FilmWithAvailability>();

                if (File.Exists(filmsFilePath))
                {
                    string existingFilmsWithAvailabilityJson = File.ReadAllText(filmsFilePath);
                    filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(existingFilmsWithAvailabilityJson);
                }

                if (filmsWithAvailability.Any(f => f.Film.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase) &&
                                                   f.Film.Mufaj.Equals(mufaj, StringComparison.OrdinalIgnoreCase) &&
                                                   f.Film.Rendezo.Equals(rendezo, StringComparison.OrdinalIgnoreCase) &&
                                                   f.Film.Hossz == hossz &&
                                                   f.Film.KiadasEve == kiadasEve &&
                                                   f.Film.Leiras.Equals(leiras, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Már létezik ilyen film a rendszerben.");
                    return;
                }

                filmsWithAvailability.Add(filmWithAvailability);

                string newFilmsWithAvailabilityJson = JsonSerializer.Serialize(filmsWithAvailability, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filmsFilePath, newFilmsWithAvailabilityJson);

                Console.WriteLine("A film sikeresen hozzáadva!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a film hozzáadása során: {ex.Message}");
            }
        }

        private static void ShowAllFilms()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek elérhető filmek.");
                return;
            }

            var json = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(json);

            foreach (var filmWithAvailability in filmsWithAvailability)
            {
                filmWithAvailability.Film.Adatok();
                Console.WriteLine($"\tElérhető: {(filmWithAvailability.Elerheto ? "Igen" : "Nem")}\n");
            }
        }

        private static void ShowMinFilm()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var json = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(json);

            if (filmsWithAvailability == null || !filmsWithAvailability.Any())
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var legrövidebbFilmHossz = filmsWithAvailability.Min(f => f.Film.Hossz);
            var legrövidebbFilmek = filmsWithAvailability.Where(f => f.Film.Hossz == legrövidebbFilmHossz).ToList();

            Console.WriteLine($"Legrövidebb film(ek) ({legrövidebbFilmHossz} perc):");
            foreach (var filmWithAvailability in legrövidebbFilmek)
            {
                Console.WriteLine($"\t{filmWithAvailability.Film.Cim} - {filmWithAvailability.Film.Rendezo}\n");
            }
        }

        private static void ShowMaxFilm()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var json = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(json);

            if (filmsWithAvailability == null || !filmsWithAvailability.Any())
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var leghosszabbFilmHossz = filmsWithAvailability.Max(f => f.Film.Hossz);
            var leghosszabbFilmek = filmsWithAvailability.Where(f => f.Film.Hossz == leghosszabbFilmHossz).ToList();

            Console.WriteLine($"Leghosszabb film(ek) ({leghosszabbFilmHossz} perc):");
            foreach (var filmWithAvailability in leghosszabbFilmek)
            {
                Console.WriteLine($"\t{filmWithAvailability.Film.Cim} - {filmWithAvailability.Film.Rendezo}\n");
            }
        }

        private static void RentFilm()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek elérhető filmek.");
                return;
            }

            string kolcsonzoNeve;
            while (true)
            {
                Console.Write("Add meg a kölcsönző nevét: ");
                kolcsonzoNeve = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(kolcsonzoNeve))
                {
                    break;
                }

                Console.WriteLine("Hiba: A név megadása kötelező.");
            }

            string cim;
            while (true)
            {
                Console.Write("Add meg a film címét: ");
                cim = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(cim))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Hiba: A cím megadása kötelező!");
                }
            }

            var json = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(json);

            var matchingFilms = filmsWithAvailability.Where(f => f.Film.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingFilms.Any())
            {
                Console.WriteLine("Nincs ilyen című film.");
                return;
            }

            var availableFilms = matchingFilms.Where(f => f.Elerheto).ToList();

            if (!availableFilms.Any())
            {
                Console.WriteLine("Ez a film nem elérhető.");
                return;
            }

            if (availableFilms.Count == 1)
            {
                RentSelectedFilm(availableFilms.First(), kolcsonzoNeve);
            }
            else
            {
                Console.WriteLine("Több elérhető film található:");
                for (int i = 0; i < availableFilms.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {availableFilms[i].Film.Cim} - {availableFilms[i].Film.Rendezo} ({availableFilms[i].Film.KiadasEve})");
                }

                Console.Write("Válassz egy filmet a szám alapján: ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= availableFilms.Count)
                {
                    RentSelectedFilm(availableFilms[selectedIndex - 1], kolcsonzoNeve);
                }
                else
                {
                    Console.WriteLine("Érvénytelen választás.");
                }
            }
        }

        private static void ReturnFilm()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincs film, amit vissza tudsz hozni");
                return;
            }

            string kolcsonzoNeve;
            while (true)
            {
                Console.Write("Ki szeretne filmet visszahozni?(név): ");
                kolcsonzoNeve = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(kolcsonzoNeve))
                {
                    break;
                }

                Console.WriteLine("Hiba: A név megadása kötelező.");
            }

            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var userKolcsonzesek = kolcsonzesek
                .Where(k => k.KolcsonzoNeve.Equals(kolcsonzoNeve, StringComparison.OrdinalIgnoreCase) && !k.Lezarult)
                .ToList();

            if (!userKolcsonzesek.Any())
            {
                Console.WriteLine("Ez az ember még nem vett ki nálunk filmet vagy minden kölcsönzése lezárult.");
                return;
            }

            Console.WriteLine("Az alábbi kölcsönzéseid vannak:");
            for (int i = 0; i < userKolcsonzesek.Count; i++)
            {
                var kolcsonzes = userKolcsonzesek[i];
                Console.WriteLine($"{i + 1}. {kolcsonzes.KolcsonzottFilm.Cim} - Határidő: {kolcsonzes.Hatarido}");
            }

            Console.Write("Válassz egy kölcsönzést a szám alapján, amit vissza szeretnél hozni: ");
            if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= userKolcsonzesek.Count)
            {
                var selectedKolcsonzes = userKolcsonzesek[selectedIndex - 1];

                if (DateTime.Now > selectedKolcsonzes.Hatarido)
                {
                    Console.WriteLine("A kölcsönzés határideje lejárt, pótdíjat kell fizetni.");
                }

                selectedKolcsonzes.Lezarult = true;

                var filmsJson = File.ReadAllText(filePath);
                var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(filmsJson);

                var filmToUpdate = filmsWithAvailability.First(f => f.Film.Cim.Equals(selectedKolcsonzes.KolcsonzottFilm.Cim, StringComparison.OrdinalIgnoreCase));
                var updatedFilmWithAvailability = new FilmWithAvailability(filmToUpdate.Film, true);

                filmsWithAvailability.Remove(filmToUpdate);
                filmsWithAvailability.Add(updatedFilmWithAvailability);

                var updatedFilmsJson = JsonSerializer.Serialize(filmsWithAvailability, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedFilmsJson);

                var updatedKolcsonzesJson = JsonSerializer.Serialize(kolcsonzesek, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(kolcsonzesFilePath, updatedKolcsonzesJson);

                Console.WriteLine("A filmet sikeresen visszahoztad.");
            }
            else
            {
                Console.WriteLine("Érvénytelen választás.");
            }
        }

        private static void SearchFilm()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek elérhető filmek.");
                return;
            }

            string cim;
            while (true)
            {
                Console.Write("Add meg a film címét: ");
                cim = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(cim))
                {
                    break;
                }

                Console.WriteLine("Hiba: A cím megadása kötelező.");
            }

            var json = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(json);

            var matchingFilms = filmsWithAvailability.Where(f => f.Film.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingFilms.Any())
            {
                Console.WriteLine("Nincs ilyen című film.");
            }
            else
            {
                Console.WriteLine("Talált filmek:");
                foreach (var filmWithAvailability in matchingFilms)
                {
                    filmWithAvailability.Film.Adatok();
                    Console.WriteLine($"\tElérhető: {(filmWithAvailability.Elerheto ? "Igen" : "Nem")}\n");
                }
            }
        }

        private static void DeleteFilm()
        {
            var filePath = "films.json";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek elérhető filmek.");
                return;
            }

            string cim;
            while (true)
            {
                Console.Write("Add meg a film címét: ");
                cim = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(cim))
                {
                    break;
                }

                Console.WriteLine("Hiba: A cím megadása kötelező.");
            }

            var filmsJson = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(filmsJson);

            var matchingFilms = filmsWithAvailability.Where(f => f.Film.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingFilms.Any())
            {
                Console.WriteLine("Nincs ilyen című film.");
                return;
            }

            if (matchingFilms.Count == 1)
            {
                var filmWithAvailability = matchingFilms.First();
                ConfirmAndDeleteFilm(filmWithAvailability, filmsWithAvailability, filePath);
            }
            else
            {
                Console.WriteLine("Több ilyen című film található:");
                for (int i = 0; i < matchingFilms.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {matchingFilms[i].Film.Cim} - {matchingFilms[i].Film.Rendezo} ({matchingFilms[i].Film.KiadasEve})");
                }

                Console.Write("Válassz egy filmet a szám alapján: ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= matchingFilms.Count)
                {
                    var selectedFilmWithAvailability = matchingFilms[selectedIndex - 1];
                    ConfirmAndDeleteFilm(selectedFilmWithAvailability, filmsWithAvailability, filePath);
                }
                else
                {
                    Console.WriteLine("Érvénytelen választás.");
                }
            }
        }

        private static void ShowAllRents()
        {
            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var allRents = kolcsonzesek
                .OrderBy(k => k.KolcsonzesDatuma)
                .ToList();

            if (!allRents.Any())
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            Console.WriteLine("Összes kölcsönzés:");
            foreach (var kolcsonzes in allRents)
            {
                kolcsonzes.Adatok();
            }
        }

        private static void SearchRents()
        {
            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            string kolcsonzoNeve;
            while (true)
            {
                Console.Write("Add meg a kölcsönző nevét: ");
                kolcsonzoNeve = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(kolcsonzoNeve))
                {
                    break;
                }

                Console.WriteLine("Hiba: A név megadása kötelező.");
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var userRents = kolcsonzesek
                .Where(k => k.KolcsonzoNeve.Equals(kolcsonzoNeve, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k.KolcsonzesDatuma)
                .ToList();

            if (!userRents.Any())
            {
                Console.WriteLine("Nincsenek kölcsönzések ehhez a névhez.");
                return;
            }

            Console.WriteLine($"{kolcsonzoNeve} kölcsönzései:");
            foreach (var kolcsonzes in userRents)
            {
                kolcsonzes.Adatok();
            }
        }

        private static void ShowOverdueRents()
        {
            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var overdueRents = kolcsonzesek
                .Where(k => !k.Lezarult && DateTime.Now > k.Hatarido)
                .OrderBy(k => k.KolcsonzesDatuma)
                .ToList();

            if (!overdueRents.Any())
            {
                Console.WriteLine("Nincsenek határidőből kifutott bérlések.");
                return;
            }

            Console.WriteLine("Határidőből kifutott bérlések:");
            foreach (var kolcsonzes in overdueRents)
            {
                kolcsonzes.Adatok();
            }
        }

        private static void ShowActiveRents()
        {
            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var activeRents = kolcsonzesek
                .Where(k => !k.Lezarult && DateTime.Now <= k.Hatarido)
                .OrderBy(k => k.KolcsonzesDatuma)
                .ToList();

            if (!activeRents.Any())
            {
                Console.WriteLine("Nincsenek aktív kölcsönzések.");
                return;
            }

            Console.WriteLine("Aktív kölcsönzések:");
            foreach (var kolcsonzes in activeRents)
            {
                kolcsonzes.Adatok();
            }
        }

        private static void ShowReturnedRents()
        {
            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var returnedRents = kolcsonzesek
                .Where(k => k.Lezarult)
                .OrderBy(k => k.KolcsonzesDatuma)
                .ToList();

            if (!returnedRents.Any())
            {
                Console.WriteLine("Nincsenek lezárult kölcsönzések.");
                return;
            }

            Console.WriteLine("Lezárult kölcsönzések:");
            foreach (var kolcsonzes in returnedRents)
            {
                kolcsonzes.Adatok();
            }
        }

        private static void ShowUnreturnedRents()
        {
            var kolcsonzesFilePath = "kolcsonzesek.json";
            if (!File.Exists(kolcsonzesFilePath))
            {
                Console.WriteLine("Nincsenek kölcsönzések.");
                return;
            }

            var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
            var kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);

            var unreturnedRents = kolcsonzesek
                .Where(k => !k.Lezarult)
                .OrderBy(k => k.KolcsonzesDatuma)
                .ToList();

            if (!unreturnedRents.Any())
            {
                Console.WriteLine("Nincsenek még nem lezárult kölcsönzések.");
                return;
            }

            Console.WriteLine("Még nem lezárult kölcsönzések:");
            foreach (var kolcsonzes in unreturnedRents)
            {
                kolcsonzes.Adatok();
            }
        }

        private static void RentSelectedFilm(FilmWithAvailability filmWithAvailability, string kolcsonzoNeve)
        {
            var filePath = "films.json";
            var json = File.ReadAllText(filePath);
            var filmsWithAvailability = JsonSerializer.Deserialize<List<FilmWithAvailability>>(json);

            var filmToUpdate = filmsWithAvailability.First(f => f.Film.Cim.Equals(filmWithAvailability.Film.Cim, StringComparison.OrdinalIgnoreCase));
            var updatedFilmWithAvailability = new FilmWithAvailability(filmToUpdate.Film, false);

            filmsWithAvailability.Remove(filmToUpdate);
            filmsWithAvailability.Add(updatedFilmWithAvailability);

            var updatedJson = JsonSerializer.Serialize(filmsWithAvailability, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJson);

            var kolcsonzesFilePath = "kolcsonzesek.json";
            var kolcsonzesek = new List<Kolcsonzes>();

            if (File.Exists(kolcsonzesFilePath))
            {
                var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
                kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);
            }

            var newKolcsonzes = new Kolcsonzes(filmWithAvailability.Film, DateTime.Now, DateTime.Now.AddDays(7))
            {
                KolcsonzoNeve = kolcsonzoNeve
            };
            kolcsonzesek.Add(newKolcsonzes);

            var updatedKolcsonzesJson = JsonSerializer.Serialize(kolcsonzesek, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(kolcsonzesFilePath, updatedKolcsonzesJson);

            Console.WriteLine("A film sikeresen kibérelve.");
        }

        private static void ConfirmAndDeleteFilm(FilmWithAvailability filmWithAvailability, List<FilmWithAvailability> filmsWithAvailability, string filePath)
        {
            while (true)
            {
                Console.WriteLine("Biztosan törölni szeretnéd a következő filmet?");
                filmWithAvailability.Film.Adatok();
                Console.Write("('igen' vagy 'nem'): ");
                var confirmation = Console.ReadLine();

                if (confirmation.Equals("igen", StringComparison.OrdinalIgnoreCase))
                {
                    filmsWithAvailability.Remove(filmWithAvailability);
                    var updatedFilmsJson = JsonSerializer.Serialize(filmsWithAvailability, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, updatedFilmsJson);

                    Console.WriteLine("Film sikeresen törölve.");
                    return;
                }
                else if (confirmation.Equals("nem", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("A film törlése megszakítva.");
                    return;
                }
                else
                {
                    Console.WriteLine("Ezt nem értem.");
                }
            }
        }
    }
}
