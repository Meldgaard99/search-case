using Microsoft.AspNetCore.Mvc;
using ConsoleSearch;
using Shared;
using Shared.Interface;
using Shared.Model;

namespace SearchAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDatabase _database;

        public DocumentsController(IDatabase database)
        {
            _database = database;
        }

        [HttpGet("search")]
        public ActionResult<List<BEDocument>> SearchDocuments([FromQuery] string query)
        {
            var words = query.Split(' ');
            var wordIds = _database.GetWordIds(words, out var ignoredWords);
            var docIds = _database.GetDocuments(wordIds).Select(kvp => kvp.Key).ToList();
            var documents = _database.GetDocDetails(docIds);
            return Ok(documents);
        }
    }
}