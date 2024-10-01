using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoadBalancerController : ControllerBase
    {
        // List of server URLs with localhost on different ports
        private static readonly string[] backendUrls = {
            "http://localhost:5102/Documents/search",
            "http://localhost:5103/Documents/search",
            "http://localhost:5104/Documents/search"
        };

        private static int currentServerIndex = 0;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            // Get the next server based on round-robin scheduling strategy
            var backendUrl = GetNextBackendUrl();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send request to the selected backend server
                    var response = await client.GetAsync($"{backendUrl}?query={query}");
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
            
                    // Include information about which server handled the request
                    return Ok(new 
                    { 
                        Server = backendUrl, 
                        Response = content 
                    });
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error in request: {ex.Message}");
                }
            }
        }
        

        // Round-robin strategy to select the next server
        private string GetNextBackendUrl()
        {
            var url = backendUrls[currentServerIndex];
            currentServerIndex = (currentServerIndex + 1) % backendUrls.Length;
            return url;
        }
    }
}