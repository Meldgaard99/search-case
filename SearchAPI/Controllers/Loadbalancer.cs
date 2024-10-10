using Microsoft.AspNetCore.Mvc;


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

                    return Ok(new
                    {
                        BackendServer = backendUrl,
                        SearchResult = content
                    });
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error in request: {ex.Message}");
                }
            }
        }
        

        private string GetNextBackendUrl()
        {
            var url = backendUrls[currentServerIndex];

            // Increment the index to point to the next server
            currentServerIndex++;

            // If the index is out of bounds, reset it to 0
            if (currentServerIndex >= backendUrls.Length)
            {
                currentServerIndex = 0;
            }

            return url;
        }
    }
}
