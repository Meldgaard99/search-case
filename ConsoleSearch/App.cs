using System;
using System.Collections.Generic;
using Shared;
using Shared.Database;


namespace ConsoleSearch
{
    public class App
    {
        private Config _config = new Config();  // Instance of the config class

        public App()
        {
        }

        public void Run()
        {
            SearchLogic mSearchLogic = new SearchLogic(new Database(), _config);

            Console.WriteLine("Console Search");

            while (true)
            {
                Console.WriteLine("Enter search terms - 'q' for quit, 'cs' to toggle case sensitivity, '/timestamp=on' or '/timestamp=off' to show/hide timestamp\")");
                string input = Console.ReadLine();
                
                if (input.Equals("q")) break;

                if (input.Equals("cs"))
                {
                    _config.CaseSensitive = !_config.CaseSensitive;  
                    Console.WriteLine("Case sensitivity is now " + (_config.CaseSensitive ? "ON" : "OFF"));
                    continue;
                }
                
                if (input.Equals("/timestamp=on"))
                {
                    _config.ShowTimeStamps = true;
                    Console.WriteLine("Timestamp display is now ON");
                    continue;
                }
                if (input.Equals("/timestamp=off"))
                {
                    _config.ShowTimeStamps = false;
                    Console.WriteLine("Timestamp display is now OFF");
                    continue;
                    
                }

                var query = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var result = mSearchLogic.Search(query, 10);

                if (result.Ignored.Count > 0)
                {
                    Console.WriteLine($"Ignored: {string.Join(',', result.Ignored)}");
                }

                int idx = 1;
                foreach (var doc in result.DocumentHits)
                {
                    Console.WriteLine($"{idx} : {doc.Document.mUrl} -- contains {doc.NoOfHits} search terms");
                    Console.WriteLine("Index time: " + doc.Document.mIdxTime);
                    Console.WriteLine($"Missing: {ArrayAsString(doc.Missing.ToArray())}");
                    idx++;
                }
                Console.WriteLine("Documents: " + result.Hits + ". Time: " + result.TimeUsed.TotalMilliseconds);
            }
        }

        string ArrayAsString(string[] s)
        {
            return s.Length == 0 ? "[]" : $"[{String.Join(',', s)}]";
        }
    }
}