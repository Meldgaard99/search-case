using Microsoft.AspNetCore.Mvc;
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
        public IActionResult SearchDocuments([FromQuery] string query)
        {
            //out skal være sat? men returnerer ikke noget jeg skal bruge??
            // Hent word ID baseret på det ene ord og sætter det i et array 
            var wordIds = _database.GetWordIds(new[] { query }, out _);
    
            // Hent dokument-IDs baseret på word IDs
            var docIds = _database.GetDocuments(wordIds)
                .Select(kvp => kvp.Key)
                .ToList();

            // Hent dokumentdetaljer og opret BEDocument-objekter
            var result = _database.GetDocDetails(docIds)
                .Select(document => new BEDocument
                {
                    mUrl = document.mUrl,
                    mIdxTime = document.mIdxTime,
                    mId = document.mId,
                    mCreationTime = document.mCreationTime
                }).ToList();
            
            var counter = result.Count;

            return Ok(new { Results = result, countMessage = "der er følgende antal hits ", Count = counter });        }
        }
    }