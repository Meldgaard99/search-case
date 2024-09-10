using System;
using System.Collections.Generic;
using Shared.Interface;

namespace Shared
{
    public class SearchLogic
    {
        private readonly IDatabase mDatabase;
        private readonly Config mConfig; 

        public SearchLogic(IDatabase database, Config config)
        {
            mDatabase = database;
            mConfig = config;
            
        }

        /* Perform search of documents containing words from query. The result will
         * contain details about at most maxAmount of documents.
         */
        public SearchResult Search(string[] query, int maxAmount)
        {
            List<string> ignored;

            DateTime start = DateTime.Now;

            // Apply case sensitivity if enabled
            if (!mConfig.CaseSensitive)
            {
                for (int i = 0; i < query.Length; i++)
                {
                    query[i] = query[i].ToLower();
                }
            }
            
            

            // Convert words to wordIds
            var wordIds = mDatabase.GetWordIds(query, out ignored);

            // Perform the search - get all docIds
            var docIds = mDatabase.GetDocuments(wordIds);

            // Get ids for the first maxAmount             
            var top = new List<int>();
            foreach (var p in docIds.GetRange(0, Math.Min(maxAmount, docIds.Count)))
                top.Add(p.Key);

            // Compose the result
            List<DocumentHit> docresult = new List<DocumentHit>();
            int idx = 0;
            foreach (var doc in mDatabase.GetDocDetails(top))
            {
                var missing = mDatabase.WordsFromIds(mDatabase.getMissing(doc.mId, wordIds));
                  
                docresult.Add(new DocumentHit(doc, docIds[idx++].Value, missing));
            }

            return new SearchResult(query, docIds.Count, docresult, ignored, DateTime.Now - start);
        }
    }
}