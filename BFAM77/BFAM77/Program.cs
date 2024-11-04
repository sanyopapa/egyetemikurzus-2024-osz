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
                    { "rent_film", RentFilm },
                    { "return_film", ReturnFilm },
                    { "search_film", SearchFilm },
                    { "delete_film", DeleteFilm },
                    { "modify_film", ModifyFilm },
                    { "exit", Exit }
                };

            Console.WriteLine("Ez itt a Képszínház videotéka alkalmazása.");
            Console.WriteLine("Írj be 'help'-et a parancsok listájához.");

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
            Console.WriteLine("\trent_film - Film kölcsönzése");
            Console.WriteLine("\treturn_film - Film visszahozása");
            Console.WriteLine("\tsearch_film - Film keresése");
            Console.WriteLine("\tdelete_film - Film törlése");
            Console.WriteLine("\tmodify_film - Film módosítása");
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

                if (films.Any(f => f.Cim.Equals(newFilm.Cim, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Hiba: Már létezik ilyen című film.");
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

        private static void RentFilm()
        {
            var filePath = "films.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nincsenek elérhető filmek.");
                return;
            }

            Console.Write("Add meg a film címét: ");
            var cim = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cim))
            {
                Console.WriteLine("Hiba: A cím megadása kötelező.");
                return;
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
                RentSelectedFilm(availableFilms.First());
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
                    RentSelectedFilm(availableFilms[selectedIndex - 1]);
                }
                else
                {
                    Console.WriteLine("Érvénytelen választás.");
                }
            }
        }

        private static void ReturnFilm()
        {
            Console.WriteLine("Film visszahozása");
        }

        private static void SearchFilm()
        {
            Console.WriteLine("Film keresése");
        }

        private static void DeleteFilm()
        {
            Console.WriteLine("Film törlése");
        }

        private static void ModifyFilm()
        {
            Console.WriteLine("Film módosítása");
        }

        private static void RentSelectedFilm(Film film)
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

            var newKolcsonzes = new Kolcsonzes(film, DateTime.Now, DateTime.Now.AddDays(7));
            kolcsonzesek.Add(newKolcsonzes);

            var updatedKolcsonzesJson = JsonSerializer.Serialize(kolcsonzesek, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(kolcsonzesFilePath, updatedKolcsonzesJson);

            Console.WriteLine("A film sikeresen kibérelve.");
        }
    }
}
