using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Api.Data;
using ReportService.Api.DTO;
using ReportService.Api.Models;

namespace ReportService.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public ItemController(AppDbContext dbContext, ILogger<ItemController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public async Task<RestDto<Item[]>> GetAllItems([FromQuery]RequestDto request)
        {
            var item = await _dbContext.Items.Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ToArrayAsync();

            return new RestDto<Item[]>
            {
                Data = item,
                Links = new List<LinkDto>
                {
                    new LinkDto(
                        Url.Action(null,
                            "GetAllItems",
                            new { request.PageIndex, request.PageSize},
                            Request.Scheme)!,
                        "self",
                        "GET")
                }
            };
        }

        [HttpGet("[action]")]
        public async Task<RestDto<Item>> GetItem(int id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

            return new RestDto<Item>
            {
                Data = item,
                Links = new List<LinkDto>
                {
                    new LinkDto(
                        Url.Action(null,
                            "GetItem",
                            id,
                            Request.Scheme)!,
                        "self",
                        "GET")
                }
            };
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PostItem([FromBody]Item item)
        {
            await _dbContext.Items.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            return Created();
        }
    }
}
