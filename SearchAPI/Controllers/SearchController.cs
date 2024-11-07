using Microsoft.AspNetCore.Mvc;
using Shared.Interface;
using Shared.Model;
using Microsoft.Extensions.Logging;
using NLog;


namespace SearchAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDatabase _database;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IDatabase database, ILogger<DocumentsController> logger)
        {
            _database = database;
            _logger = logger;
        }
        
        [HttpGet("search")]  
        public IActionResult SearchDocuments([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Query parameter is empty or null.");
                return BadRequest("Query parameter cannot be empty.");
            }

            _logger.LogInformation("Searching for query: {Query}", query);

            var wordIds = _database.GetWordIds(new[] { query }, out _);
            if (wordIds == null || !wordIds.Any())
            {
                _logger.LogInformation("No word IDs found for query: {Query}", query);
                return Ok(new { Results = new List<BEDocument>(), countMessage = "der er følgende antal hits ", Count = 0 });
            }

            var docIds = _database.GetDocuments(wordIds)
                .Select(kvp => kvp.Key)
                .ToList();
            if (!docIds.Any())
            {
                _logger.LogInformation("No document IDs found for word IDs.");
                return Ok(new { Results = new List<BEDocument>(), countMessage = "der er følgende antal hits ", Count = 0 });
            }

            var result = _database.GetDocDetails(docIds)
                .Select(document => new BEDocument
                {
                    mUrl = document.mUrl,
                    mIdxTime = document.mIdxTime,
                    mId = document.mId,
                    mCreationTime = document.mCreationTime
                }).ToList();
            
            var counter = result.Count;

            _logger.LogInformation("Found {Count} documents for query: {Query}", counter, query);
//            var logger = NLog.LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

            return Ok(new { Results = result, countMessage = "der er følgende antal hits ", Count = counter });
        }
    }
}