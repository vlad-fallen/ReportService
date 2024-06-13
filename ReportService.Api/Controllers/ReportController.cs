using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Api.Constants;
using ReportService.Api.Data;
using ReportService.Api.DTO;
using ReportService.Api.Models;
using ReportService.Api.RabbitMq;

namespace ReportService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IMessageProducer _messageProducer;
        private readonly AppDbContext _dbContext;

        public ReportController(
            ILogger<ReportController> logger,
            IMessageProducer messageProducer,
            AppDbContext dbContext)
        {
            _logger = logger;
            _messageProducer = messageProducer;
            _dbContext = dbContext;
        }

        [HttpGet("[action]")]
        public async Task<RestDto<ReportDto>> Get([FromQuery]int itemId, [FromQuery]DateTime start, [FromQuery]DateTime end)
        {
            _logger.LogInformation(CustomLogEvents.ReportController_Get, "Get method started.");

            var items = await _dbContext.Items.Where(i => i.ItemId == itemId).Where(i => i.VisitDay >= start).Where(i => i.VisitDay <= end).ToListAsync();

            var viewCount = items.Count;
            var paymentCount = items.Where(i => i.Payment == true).Count();

            var report = new ReportDto
            {
                ViewPayment = (double)viewCount / (double)paymentCount,
                Pyment = paymentCount
            };

            _messageProducer.SendingMessage(report);

            return new RestDto<ReportDto>
            {
                Data = report,
                Links = new List<LinkDto>
                {
                    new LinkDto(
                        Url.Action(null,
                            "ReportAPI",
                            itemId,
                            Request.Scheme)!,
                        "self",
                        "GET"),
                }
            };
        }
    }
}
