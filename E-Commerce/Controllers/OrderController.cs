using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class OrderController : ApiBaseController
    {
        private readonly IOrderServices orderServices;

        public OrderController(IOrderServices orderServices)
        {
            this.orderServices = orderServices;
        }

        #region Create
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder([FromBody] OrderDto orderDto, [FromQuery] string email, CancellationToken ct)
        => ToActionResult(await orderServices.CreateOrderAsync(orderDto, email, ct));
        #endregion

        #region DeliveryMethods
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethods(CancellationToken ct)
           => ToActionResult(await orderServices.GetAllDeliveryMethodsAsync(ct));
        #endregion

        #region All Orders
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders([FromQuery] string email, CancellationToken ct)
         => ToActionResult(await orderServices.GetAllOrdersAsync(email, ct));
        #endregion

        #region OrderById
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(Guid id, [FromQuery] string email, CancellationToken ct)
         => ToActionResult(await orderServices.GetOrderByIdAsync(id, email, ct));
        #endregion

    }
}
