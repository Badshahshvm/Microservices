using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Frontend.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string GatewayClientName = "gateway";
        private const string ApiPath = "api/product/";

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient(GatewayClientName);

        // Index view
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClient();
            List<Product> products = new();

            var response = await client.GetAsync(ApiPath);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => View();

        // AJAX Create
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Product product)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data");

            var client = CreateClient();
            var payload = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(ApiPath, payload);

            if (response.IsSuccessStatusCode)
            {
                var created = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());
                return Ok(created);
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        // Get single product
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync(ApiPath + id);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(json);
                return Ok(product);
            }
            return NotFound();
        }

        // AJAX Edit
        [HttpPut]
        public async Task<IActionResult> EditAjax([FromBody] Product product)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data");

            var client = CreateClient();
            var payload = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(ApiPath + product.Id, payload);

            if (response.IsSuccessStatusCode) return Ok();
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        // AJAX Delete
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(ApiPath + id);
            if (response.IsSuccessStatusCode) return Ok();
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
