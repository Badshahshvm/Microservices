using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Frontend.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string GatewayClientName = "gateway";
        private const string ApiPath = "api/customer/";

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient(GatewayClientName);
        }

        // Index view
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClient();
            List<Customer> customers = new List<Customer>();

            var response = await client.GetAsync(ApiPath);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
            }

            return View(customers);
        }

        [HttpGet]
        public IActionResult Create() => View();

        // AJAX Create - returns JSON
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var client = CreateClient();
            var payload = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ApiPath, payload);
            if (response.IsSuccessStatusCode)
            {
                var created = JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
                return Ok(created);
            }

            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }

        // Get single customer (used to populate edit/details/delete modal)
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync(ApiPath + id);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(json);
                return Ok(customer);
            }
            return NotFound();
        }

        // AJAX Edit
        [HttpPut]
        public async Task<IActionResult> EditAjax([FromBody] Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data");

            var client = CreateClient();
            var payload = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(ApiPath + customer.Id, payload);

            if (response.IsSuccessStatusCode) return Ok();
            var err = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, err);
        }

        // AJAX Delete
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(ApiPath + id);
            if (response.IsSuccessStatusCode) return Ok();
            var err = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, err);
        }

        // Optional: fallbacks for non-AJAX forms (if you want)
        [HttpGet]
        public IActionResult DetailsView() => View();
    }
}
