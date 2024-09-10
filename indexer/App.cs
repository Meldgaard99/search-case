// Modify the App class in `indexer/App.cs`
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Indexer
{
    public class App
    {
        public void Run()
        {
            Database db = new Database();
            Crawler crawler = new Crawler(db);

            var root = new DirectoryInfo(Paths.FOLDER);

            DateTime start = DateTime.Now;

            crawler.IndexFilesIn(root, new List<string> { ".txt" });

            TimeSpan used = DateTime.Now - start;
            Console.WriteLine("DONE! used " + used.TotalMilliseconds);

            var allWords = db.GetAllWords();
            var wordFrequencies = crawler.GetWordFrequencies();

            Console.WriteLine($"Indexed {db.GetDocumentCounts()} documents");
            Console.WriteLine($"Indexed {wordFrequencies.Count} unique words");

            // Ask the user how many words they want to see
            Console.WriteLine("How many words do you want to see? Enter a number:");
            if (int.TryParse(Console.ReadLine(), out int numWords))
            {
                // Sort the words by frequency in descending order
                var sortedWords = wordFrequencies.OrderByDescending(p => p.Value).Take(numWords);

                Console.WriteLine($"The {numWords} most frequent words are:");
                foreach (var p in sortedWords)
                {
                    Console.WriteLine($"<{p.Key}, {allWords[p.Key]}> - {p.Value}");
                }
            }
        }
    }
}