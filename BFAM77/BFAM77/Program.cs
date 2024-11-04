using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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
            string cim, mufaj, rendezo, leiras;
            int kiadasEve, hossz;
            bool elerheto;

            string GetInput(string prompt, bool isMandatory = false)
            {
                while (true)
                {
                    Console.Write(prompt);
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        if (isMandatory)
                        {
                            Console.WriteLine("Ez a mező kötelező!");
                            continue;
                        }

                        Console.Write("Biztos üresen akarja hagyni? (igen/nem): ");
                        var confirm = Console.ReadLine();
                        if (confirm.Equals("igen", StringComparison.OrdinalIgnoreCase))
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return input;
                    }
                }
            }

            int GetIntInput(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.Write("Biztos üresen akarja hagyni? (igen/nem): ");
                        var confirm = Console.ReadLine();
                        if (confirm.Equals("igen", StringComparison.OrdinalIgnoreCase))
                        {
                            return 0;
                        }
                        if (confirm.Equals("nem", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else if (int.TryParse(input, out var result))
                    {
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("Érvénytelen szám. Próbálja újra.");
                    }
                }
            }

            try
            {
                cim = GetInput("Film címe: ");
                mufaj = GetInput("Műfaj: ");
                rendezo = GetInput("Rendező: ");
                kiadasEve = GetIntInput("Kiadás éve: ");
                hossz = GetIntInput("Hossz (percben): ");
                leiras = GetInput("Leírás: ");

                while (true)
                {
                    Console.Write("Elérhető (igen/nem): ");
                    var availableInput = Console.ReadLine();
                    if (availableInput.Equals("igen", StringComparison.OrdinalIgnoreCase))
                    {
                        elerheto = true;
                        break;
                    }
                    else if (availableInput.Equals("nem", StringComparison.OrdinalIgnoreCase))
                    {
                        elerheto = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Érvénytelen válasz. Kérem, válaszoljon 'igen' vagy 'nem'.");
                    }
                }

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
            Console.WriteLine("Film kölcsönzése");
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
    }
}
