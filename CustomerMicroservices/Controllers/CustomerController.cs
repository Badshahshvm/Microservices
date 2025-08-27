using Customer_microservices.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer_microservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static List<Customer> customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Shivam Raj", Email = "shivam@example.com", Phone = "9876543210", Address = "Bihar" },
            new Customer { Id = 2, Name = "Anjali Sharma", Email = "anjali@example.com", Phone = "9123456780", Address = "Delhi" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> GetById(int id)
        {
            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public ActionResult<Customer> Create(Customer customer)
        {
            customer.Id = customers.Max(c => c.Id) + 1;
            customers.Add(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Customer updatedCustomer)
        {
            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();

            customer.Name = updatedCustomer.Name;
            customer.Email = updatedCustomer.Email;
            customer.Phone = updatedCustomer.Phone;
            customer.Address = updatedCustomer.Address;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();

            customers.Remove(customer);
            return NoContent();
        }
    }
}
