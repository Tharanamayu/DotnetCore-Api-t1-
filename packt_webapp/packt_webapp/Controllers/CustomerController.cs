using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using packt_webapp.Dtos;
using packt_webapp.Entities;
using packt_webapp.QueryParameters;
using packt_webapp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Controllers
{   
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly ILogger<CustomersController> _logger;
        private ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository, ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            
        }
        [HttpGet]
        [ProducesResponseType(typeof(Customer), 200)]
        public IActionResult GetAllCustomers(CustomerQueryParameters customerQueryParameters)
        {
            
            var allCustomers = _customerRepository.GetAll(customerQueryParameters).ToList();
            var allCustomersDto = allCustomers.Select(x => Mapper.Map<CustomerDto>(x));

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(new { totalCount = _customerRepository.Count() }));
            
            return Ok(allCustomersDto);
        }
        [HttpGet]
        [Route("{id}", Name = "GetSingleCustomer")]
        public IActionResult GetSingleCustomer(Guid id)
        {
            Customer customerFromRepo = _customerRepository.GetSingle(id);

            if (customerFromRepo == null)
            {
                return NotFound();
            }
            return Ok(Mapper.Map<CustomerDto>(customerFromRepo));
        }
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto),201)]
        [ProducesResponseType(typeof(CustomerDto), 400)]
        public IActionResult AddCustomer([FromBody] CustomerCreateDto customerCreateDto)
        {
            if (customerCreateDto == null)
            {
                return BadRequest("Customer create object was null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Customer toAdd = Mapper.Map<Customer>(customerCreateDto);
            _customerRepository.Add(toAdd);
            bool result = _customerRepository.Save();

            if (!result)
            {
                //return new StatusCodeResult(500);
                throw new Exception("something went wrong when adding a new customer");
            }
            else
            {
                //return Ok(Mapper.Map<CustomerDto>(toAdd));
                return CreatedAtRoute("GetSingleCustomer", new { id = toAdd.Id }, Mapper.Map<CustomerDto>(toAdd));
            }
        }
        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateCustomer(Guid id, [FromBody] CustomerUpdateDto CustomerUpdateDto)
        {
            if (CustomerUpdateDto == null)
            {
                return BadRequest("Customer create object was null");
            }
            
            var existingCustomer = _customerRepository.GetSingle(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Mapper.Map(CustomerUpdateDto, existingCustomer);
            _customerRepository.Update(existingCustomer);
            bool result = _customerRepository.Save();
            if (!result)
            {
                // return new StatusCodeResult(500);
                throw new Exception($"something went wrong when updating the customer with id: {id}");
            }
            return Ok(Mapper.Map<CustomerDto>(existingCustomer));

        }
        [HttpPatch]
        [Route("{id}")]
        public IActionResult PartiallyUpdate(Guid id, [FromBody] JsonPatchDocument<CustomerUpdateDto> customerPatchDoc)
        {
            if (customerPatchDoc == null)
            {
                return BadRequest();
            }
            var existingCustomer = _customerRepository.GetSingle(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }
            var customerToPatch = Mapper.Map<CustomerUpdateDto>(existingCustomer);
            customerPatchDoc.ApplyTo(customerToPatch,ModelState);

            TryValidateModel(customerToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(customerToPatch, existingCustomer);

            _customerRepository.Update(existingCustomer);
            bool result = _customerRepository.Save();

            if (!result)
            {
                //return new StatusCodeResult(500);
                throw new Exception($"something went wrong when updating the customer with id: {id}");
            }
            return Ok(Mapper.Map<CustomerDto>(existingCustomer));
        }
        [HttpDelete]
        [Route("{id}")]
        public IActionResult remove(Guid id)
        {
            var existingCustomer = _customerRepository.GetSingle(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }
            _customerRepository.Delete(id);
            bool result = _customerRepository.Save();
            if (!result)
            {
                //return new StatusCodeResult(500);
                throw new Exception($"something went wrong when deleting the customer with id: {id}");
            }
            return NoContent();
        }
    }
}
