using B3.Api.Enums;
using B3.Api.Models;
using B3.Api.ViewModels;

namespace B3.Api.Interfaces
{
    public interface IBestPriceSimulatorService
    {
        Task<BestPrice> CreateAsync(HttpRequestBody HttpRequestBody);
        Task<List<BestPrice>> GetAllAsync();
    }
}