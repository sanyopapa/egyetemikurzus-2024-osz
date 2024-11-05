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
                { "modify_film", ModifyFilm },
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
            Console.WriteLine("\tmodify_film - Film módosítása");
            Console.WriteLine("\tshow_all_rents - Összes kölcsönzés megjelenítése");
            Console.WriteLine("\tshow_unreturned_rents - Vissza nem hozott kölcsönzések megjelenítése");
            Console.WriteLine("\tshow_returned_rents - Visszahozott kölcsönzések megjelenítése");
            Console.WriteLine("\tshow_overdue_rents - Lejárt kölcsönzések megjelenítése(Amik lejártak, de még nem hozták őket vissza)");
            Console.WriteLine("\tshow_active_rents - Aktív kölcsönzések megjelenítése(Amiket még nem hoztak vissza, és nem jártak le)");
            Console.WriteLine("\tsearch_rents - Kölcsönzések keresése(a bérlő neve szerint)");
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
                var cim = InputHelper.GetStringInput("Film címe: ");
                var mufaj = InputHelper.GetStringInput("Műfaj: ");
                var rendezo = InputHelper.GetStringInput("Rendező: ");
                var kiadasEve = InputHelper.GetIntInput("Kiadás éve: ");
                var hossz = InputHelper.GetIntInput("Hossz (percben): ");
                var leiras = InputHelper.GetStringInput("Leírás: ");
                var elerheto = InputHelper.GetBoolInput("Elérhető (igen/nem): ");

                var newFilm = new Film
                {
                    Cim = cim,
                    Mufaj = mufaj,
                    Rendezo = rendezo,
                    KiadasEve = kiadasEve,
                    Hossz = hossz,
                    Leiras = leiras,
                    Elerheto = elerheto
                };

                var filePath = "films.json";
                var films = new List<Film>();

                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    films = JsonSerializer.Deserialize<List<Film>>(json);
                }

                if (films.Any(f => f.Cim.Equals(newFilm.Cim, StringComparison.OrdinalIgnoreCase) &&
                                   f.Mufaj.Equals(newFilm.Mufaj, StringComparison.OrdinalIgnoreCase) &&
                                   f.Rendezo.Equals(newFilm.Rendezo, StringComparison.OrdinalIgnoreCase) &&
                                   f.KiadasEve == newFilm.KiadasEve &&
                                   f.Hossz == newFilm.Hossz &&
                                   f.Leiras.Equals(newFilm.Leiras, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Hiba: Már létezik ilyen film.");
                    return;
                }

                films.Add(newFilm);
                var updatedJson = JsonSerializer.Serialize(films, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);

                Console.WriteLine("Film sikeresen hozzáadva.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt: {ex.Message}");
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
            var films = JsonSerializer.Deserialize<List<Film>>(json);

            foreach (var film in films)
            {
                film.Adatok();
                Console.WriteLine();
            }
        }

        private static void ShowMinFilm()
        {
            var filmFilePath = "films.json";
            if (!File.Exists(filmFilePath))
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var filmJson = File.ReadAllText(filmFilePath);
            var filmek = JsonSerializer.Deserialize<List<Film>>(filmJson);

            if (filmek == null || !filmek.Any())
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var legrövidebbFilmHossz = filmek.Min(f => f.Hossz);
            var legrövidebbFilmek = filmek.Where(f => f.Hossz == legrövidebbFilmHossz).ToList();

            Console.WriteLine($"Legrövidebb film(ek) ({legrövidebbFilmHossz} perc):");
            foreach (var film in legrövidebbFilmek)
            {
                Console.WriteLine($"\t{film.Cim} - {film.Rendezo}\n");
            }
        }

        private static void ShowMaxFilm()
        {
            var filmFilePath = "films.json";
            if (!File.Exists(filmFilePath))
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var filmJson = File.ReadAllText(filmFilePath);
            var filmek = JsonSerializer.Deserialize<List<Film>>(filmJson);

            if (filmek == null || !filmek.Any())
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var leghosszabbFilmHossz = filmek.Max(f => f.Hossz);
            var leghosszabbFilmek = filmek.Where(f => f.Hossz == leghosszabbFilmHossz).ToList();

            Console.WriteLine($"Leghosszabb film(ek) ({leghosszabbFilmHossz} perc):");
            foreach (var film in leghosszabbFilmek)
            {
                Console.WriteLine($"\t{film.Cim} - {film.Rendezo}\n");
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
            var films = JsonSerializer.Deserialize<List<Film>>(json);

            var matchingFilms = films.Where(f => f.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase)).ToList();

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
                    Console.WriteLine($"{i + 1}. {availableFilms[i].Cim} - {availableFilms[i].Rendezo} ({availableFilms[i].KiadasEve})");
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

                var filmJson = File.ReadAllText(filePath);
                var films = JsonSerializer.Deserialize<List<Film>>(filmJson);

                var filmToUpdate = films.First(f => f.Cim.Equals(selectedKolcsonzes.KolcsonzottFilm.Cim, StringComparison.OrdinalIgnoreCase));
                filmToUpdate.Elerheto = true;

                var updatedFilmJson = JsonSerializer.Serialize(films, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedFilmJson);

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
            var films = JsonSerializer.Deserialize<List<Film>>(json);

            var matchingFilms = films.Where(f => f.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingFilms.Any())
            {
                Console.WriteLine("Nincs ilyen című film.");
            }
            else
            {
                Console.WriteLine("Talált filmek:");
                foreach (var film in matchingFilms)
                {
                    film.Adatok();
                    Console.WriteLine();
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

            var json = File.ReadAllText(filePath);
            var films = JsonSerializer.Deserialize<List<Film>>(json);

            var matchingFilms = films.Where(f => f.Cim.Equals(cim, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingFilms.Any())
            {
                Console.WriteLine("Nincs ilyen című film.");
                return;
            }

            if (matchingFilms.Count == 1)
            {
                var film = matchingFilms.First();
                ConfirmAndDeleteFilm(film, films, filePath);
            }
            else
            {
                Console.WriteLine("Több ilyen című film található:");
                for (int i = 0; i < matchingFilms.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {matchingFilms[i].Cim} - {matchingFilms[i].Rendezo} ({matchingFilms[i].KiadasEve})");
                }

                Console.Write("Válassz egy filmet a szám alapján: ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= matchingFilms.Count)
                {
                    var selectedFilm = matchingFilms[selectedIndex - 1];
                    ConfirmAndDeleteFilm(selectedFilm, films, filePath);
                }
                else
                {
                    Console.WriteLine("Érvénytelen választás.");
                }
            }
        }

        private static void ModifyFilm()
        {
            var filmFilePath = "films.json";
            if (!File.Exists(filmFilePath))
            {
                Console.WriteLine("Nincsenek filmek.");
                return;
            }

            var filmJson = File.ReadAllText(filmFilePath);
            var filmek = JsonSerializer.Deserialize<List<Film>>(filmJson);

            string filmCim;
            while (true)
            {
                Console.Write("Add meg a film címét: ");
                filmCim = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(filmCim))
                {
                    break;
                }

                Console.WriteLine("Hiba: A cím megadása kötelező.");
            }

            var talaltFilmek = filmek
                .Where(f => f.Cim.Equals(filmCim, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!talaltFilmek.Any())
            {
                Console.WriteLine("Nincs ilyen című film.");
                return;
            }

            if (talaltFilmek.Count > 1)
            {
                Console.WriteLine("Több ilyen című film található:");
                for (int i = 0; i < talaltFilmek.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {talaltFilmek[i].Cim} - {talaltFilmek[i].Rendezo} - {talaltFilmek[i].KiadasEve}");
                }

                int filmIndex;
                while (true)
                {
                    Console.Write("Add meg a módosítani kívánt film számát: ");
                    if (int.TryParse(Console.ReadLine(), out filmIndex) && filmIndex > 0 && filmIndex <= talaltFilmek.Count)
                    {
                        filmIndex--; 
                        break;
                    }

                    Console.WriteLine("Hiba: Érvénytelen szám.");
                }

                ModifyFilmDetails(talaltFilmek[filmIndex]);
            }
            else
            {
                ModifyFilmDetails(talaltFilmek[0]);
            }

            File.WriteAllText(filmFilePath, JsonSerializer.Serialize(filmek));
            Console.WriteLine("A film adatai sikeresen módosítva.");
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

        private static void RentSelectedFilm(Film film, string kolcsonzoNeve)
        {
            film.Elerheto = false;

            var filePath = "films.json";
            var json = File.ReadAllText(filePath);
            var films = JsonSerializer.Deserialize<List<Film>>(json);

            var filmToUpdate = films.First(f => f.Cim.Equals(film.Cim, StringComparison.OrdinalIgnoreCase));
            filmToUpdate.Elerheto = false;

            var updatedJson = JsonSerializer.Serialize(films, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJson);

            var kolcsonzesFilePath = "kolcsonzesek.json";
            var kolcsonzesek = new List<Kolcsonzes>();

            if (File.Exists(kolcsonzesFilePath))
            {
                var kolcsonzesJson = File.ReadAllText(kolcsonzesFilePath);
                kolcsonzesek = JsonSerializer.Deserialize<List<Kolcsonzes>>(kolcsonzesJson);
            }

            var newKolcsonzes = new Kolcsonzes(film, DateTime.Now, DateTime.Now.AddDays(7))
            {
                KolcsonzoNeve = kolcsonzoNeve
            };
            kolcsonzesek.Add(newKolcsonzes);

            var updatedKolcsonzesJson = JsonSerializer.Serialize(kolcsonzesek, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(kolcsonzesFilePath, updatedKolcsonzesJson);

            Console.WriteLine("A film sikeresen kibérelve.");
        }

        private static void ConfirmAndDeleteFilm(Film film, List<Film> films, string filePath)
        {
            while (true)
            {
                Console.WriteLine("Biztosan törölni szeretnéd a következő filmet?");
                film.Adatok();
                Console.Write("('igen' vagy 'nem'): ");
                var confirmation = Console.ReadLine();

                if (confirmation.Equals("igen", StringComparison.OrdinalIgnoreCase))
                {
                    films.Remove(film);
                    var updatedJson = JsonSerializer.Serialize(films, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, updatedJson);
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

        private static void ModifyFilmDetails(Film film)
        {
            Console.WriteLine("Melyik adatot szeretnéd módosítani?");
            Console.WriteLine("1. Cím");
            Console.WriteLine("2. Műfaj");
            Console.WriteLine("3. Rendező");
            Console.WriteLine("4. Hossz");
            Console.WriteLine("5. Kiadás éve");
            Console.WriteLine("6. Leírás");

            int choice;
            while (true)
            {
                Console.Write("Add meg a számot: ");
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 6)
                {
                    break;
                }

                Console.WriteLine("Hiba: Érvénytelen szám.");
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Új cím: ");
                        film.Cim = Console.ReadLine();
                        break;
                    case 2:
                        Console.Write("Új műfaj: ");
                        film.Mufaj = Console.ReadLine();
                        break;
                    case 3:
                        Console.Write("Új rendező: ");
                        film.Rendezo = Console.ReadLine();
                        break;
                    case 4:
                        Console.Write("Új hossz: ");
                        film.Hossz = int.Parse(Console.ReadLine());
                        break;
                    case 5:
                        Console.Write("Új kiadás éve: ");
                        film.KiadasEve = int.Parse(Console.ReadLine());
                        break;
                    case 6:
                        Console.Write("Új leírás: ");
                        film.Leiras = Console.ReadLine();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a módosítás során: {ex.Message}");
            }
        }
    }
}
