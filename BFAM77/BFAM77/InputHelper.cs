using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFAM77
{
    internal class InputHelper
    {
        public static string GetStringInput(string prompt, bool isMandatory = false)
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

        public static int GetIntInput(string prompt)
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

        public static bool GetBoolInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (input.Equals("igen", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if (input.Equals("nem", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Érvénytelen válasz. Kérem, válaszoljon 'igen' vagy 'nem'.");
                }
            }
        }
    }
}
