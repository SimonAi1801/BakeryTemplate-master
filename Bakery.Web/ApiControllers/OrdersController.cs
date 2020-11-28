using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bakery.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public OrdersController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Orders
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderWithItemsDto>>> GetOrders()
        {
            return Ok();
        }

        // GET: api/Orders/ordersByCustomerId/2
        [HttpGet("ordersByCustomerId/{id}")]
        public async Task<ActionResult<IEnumerable<OrderWithItemsDto>>> GetOrdersByCustomer(int id)
        {
            throw new NotImplementedException();
        }

    }
}
