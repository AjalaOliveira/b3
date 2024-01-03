using B3.Api.Interfaces;
using B3.Api.Models;
using B3.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace B3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestPriceSimulatorController : ControllerBase
    {
        private readonly IBestPriceSimulatorService _bestPriceSimulatorService;

        public BestPriceSimulatorController(
            IBestPriceSimulatorService bestPriceSimulatorService)
        {
            _bestPriceSimulatorService = bestPriceSimulatorService;
        }

        [HttpPost]
        public async Task<BestPrice> SimulateBestPrice(HttpRequestBody HttpRequestBody)
        {
            return await _bestPriceSimulatorService.CreateAsync(HttpRequestBody);
        }

        [HttpGet]
        public async Task<IList<BestPrice>> GetAllSimulations()
            => await _bestPriceSimulatorService.GetAllAsync();
    }
}
