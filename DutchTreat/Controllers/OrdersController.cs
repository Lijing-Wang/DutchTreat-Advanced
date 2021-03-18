﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using DutchTreatAdvanced.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreatAdvanced.Controllers
{
    [Route("api/[Controller]")]
    public class OrdersController: Controller

    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IDutchRepository repository, ILogger<OrdersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IActionResult Get()
        {
            try
            {
                return Ok(_repository.GetOrders());
            }
            catch (Exception e)
            {
                _logger.LogError($"Fail to get orders: {e}");
                return BadRequest();
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _repository.GetOrderById(id);
                if (order != null) return Ok(order);
                // Not found makes more sense than a bad request
                else return NotFound();

            }
            catch (Exception e)
            {
                _logger.LogError($"Fail to get orders: {e}");
                return BadRequest();
            }
        }

        [HttpPost]
        // without [FromBody], it assumes that the input is coming from the query string
        public IActionResult Post([FromBody]Order model)
        {
            try
            {
                // push any data attached to the context so they will be saved to DB
                _repository.AddEntity(model);
                if (_repository.SaveAll())
                {
                    // In HTTP, in a POST, if an object is created, it must return a "created"
                    // Pass ID back since there could be more fields generated by the server so users will know this is the actual latest version of the object created
                    return Created($"/api/orders/{model.Id}", model);
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to save a new order: {e}");
            }

            return BadRequest("Failed to save new order.");

        }
    }
}
