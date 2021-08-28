using System;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire_API_SQLite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpPost("Welcome")]
        public IActionResult Welcome()
        {
            var jobId = BackgroundJob.Enqueue(() => SendWelcomeEmail("Welcome to our app"));
            return Ok($"Job ID: { jobId }. Welcome email sent to the user");
        }

        [HttpPost("Discount")]
        public IActionResult Discount()
        {
            int timeInSeconds = 30;
            var jobId = BackgroundJob.Schedule(() => SendWelcomeEmail("Welcome to our app"), TimeSpan.FromSeconds(timeInSeconds));
            return Ok($"Job ID: { jobId }. Discount email will be sent in {timeInSeconds} seconds");
        }

        [HttpPost("Update")]
        public IActionResult Update()
        {
            RecurringJob.AddOrUpdate(() => SendWelcomeEmail("Database update"), Cron.Minutely);
            return Ok($"Database update job");
        }

        [HttpPost("Confirm")]
        public IActionResult Confirm()
        {
            int timeInSeconds = 30;
            var parentjobId = BackgroundJob.Schedule(() => SendWelcomeEmail("Database reload"), TimeSpan.FromSeconds(timeInSeconds));

            BackgroundJob.ContinueJobWith(parentjobId, () => SendWelcomeEmail("Database reload sucessfully"));
            return Ok($"Confirmation job created");
        }

        public IActionResult SendWelcomeEmail(string text)
        {
            return Content(text);
        }
    }
}