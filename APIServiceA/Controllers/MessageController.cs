using Microsoft.AspNetCore.Mvc;
using RabbitMQ;

namespace APIServiceA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public MessageController(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost("send")]
        public async Task<ActionResult<string>> SendMessageAsync([FromBody] string message)
        {
            // Call the async method to send a message and wait for a response asynchronously
            await _rabbitMQService.PublishMessage(RabbitMQQueues.ApiBQueue, RabbitMQQueues.ApiAQueue ,message);
            return Ok();
        }
    }
}
