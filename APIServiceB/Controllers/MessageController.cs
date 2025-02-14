using Microsoft.AspNetCore.Mvc;
using RabbitMQ;

namespace APIServiceB.Controllers
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
            await _rabbitMQService.PublishMessage(RabbitMQQueues.ApiAQueue, RabbitMQQueues.ApiBQueue,message);
            return Ok();
        }
    }
}
