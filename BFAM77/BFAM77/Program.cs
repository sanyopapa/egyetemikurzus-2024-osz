namespace VideoTeka
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var commands = new Dictionary<string, Action>
        {
            { "help", ShowHelp },
            { "exit", Exit }
        };

            Console.WriteLine("Parancssoros alkalmazás. Írj be egy parancsot ('help' a parancsok listájához, 'exit' a kilépéshez).");

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
            Console.WriteLine("help - Parancsok listájának megjelenítése");
            Console.WriteLine("exit - Kilépés az alkalmazásból");
        }

        private static void Exit()
        {
            Console.WriteLine("Kilépés...");
            Environment.Exit(0);
        }
    }
}

