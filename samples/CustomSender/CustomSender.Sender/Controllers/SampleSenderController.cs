using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CustomSender.Sender.Controllers
{
    [ApiController]
    public class SampleSenderController : ControllerBase
    {
        private readonly ILogger<SampleSenderController> _logger;
        private readonly IWebHookManager _webhookManager;

        public SampleSenderController(
            ILogger<SampleSenderController> logger,
            IWebHookManager webhookManager)
        {
            this._logger = logger;
            this._webhookManager = webhookManager;
        }

        [HttpPost(), Route("/api/SampleSender/Send/{eventName}")]
        [Authorize()]
        public async Task<ActionResult> SendAsync([FromRoute]string eventName, [FromBody] dynamic body)
        {
            _logger.LogInformation($"Current user is {this.User.Identity.Name}");

            string bodyJson = JsonConvert.SerializeObject(body);
            _logger.LogInformation($"Sending webhook on event {eventName} with data {bodyJson}.");

            var notifications = new List<Notification>()
            {
                new Notification(eventName, body)
            };
            int sendCount = await _webhookManager.NotifyAllAsync(notifications, (w, s) => true);

            _logger.LogInformation($"Webhook sent to {sendCount} subscriptions.");

            return Ok(sendCount);
        }
    }
}
