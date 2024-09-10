using System;
using System.Collections.Generic;
using System.IO;
using Shared.Model;

namespace Indexer
{
    public class Crawler
    {
        private readonly char[] separators = " \\\n\t\"$'!,?;.:-_**+=)([]{}<>/@&%€#".ToCharArray();
        private Dictionary<string, int> words = new Dictionary<string, int>();
        private Dictionary<string, int> wordfrequncy = new Dictionary<string, int>();
        private int documentCounter = 0;
        IDatabase mdatabase;

        public Crawler(IDatabase db) { mdatabase = db; }

        private ISet<string> ExtractWordsInFile(FileInfo f)
        {
            ISet<string> res = new HashSet<string>();
            var content = File.ReadAllLines(f.FullName);
            foreach (var line in content)
            {
                foreach (var aWord in line.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                {
                    res.Add(aWord);
                }
            }
            return res;
        }

        private ISet<int> GetWordIdFromWords(ISet<string> src)
        {
            ISet<int> res = new HashSet<int>();
            foreach (var p in src)
            {
                res.Add(words[p]);
            }
            return res;
        }

        public void IndexFilesIn(DirectoryInfo dir, List<string> extensions)
        {
            Console.WriteLine($"Crawling {dir.FullName}");

            foreach (var file in dir.EnumerateFiles())
                if (extensions.Contains(file.Extension))
                {
                    documentCounter++;
                    BEDocument newDoc = new BEDocument
                    {
                        mId = documentCounter,
                        mUrl = file.FullName,
                        mIdxTime = DateTime.Now.ToString(),
                        mCreationTime = file.CreationTime.ToString()
                    };

                    mdatabase.InsertDocument(newDoc);
                    Dictionary<string, int> newWords = new Dictionary<string, int>();
                    ISet<string> wordsInFile = ExtractWordsInFile(file);
                    foreach (var aWord in wordsInFile)
                    {
                        if (!words.ContainsKey(aWord))
                        {
                            words.Add(aWord, words.Count + 1);
                            newWords.Add(aWord, words[aWord]);
                        }
                        if (!wordfrequncy.ContainsKey(aWord))
                            wordfrequncy.Add(aWord, 1);
                        else
                            wordfrequncy[aWord]++;
                    }
                    mdatabase.InsertAllWords(newWords);
                    mdatabase.InsertAllOcc(newDoc.mId, GetWordIdFromWords(wordsInFile));
                }
            foreach (var d in dir.EnumerateDirectories())
                IndexFilesIn(d, extensions);
        }

        public Dictionary<string, int> GetWordFrequencies()
        {
            return wordfrequncy;
        }
    }
}