using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace B3.Worker.Service.Process
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private readonly IOrderRepository _orderRepository;

        public OrderService(
            ILogger<OrderService> logger,
            IOptionsMonitor<AppSettings> appSettings,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _appSettings = appSettings;
            _orderRepository = orderRepository;
        }

        public async Task BtcUsdExecute()
        {
            try
            {
                await ConnectWebSocketAsync(_orderRepository, _appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.BtcUsdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("BTC/USD.BtcUsdExecute - Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        public async Task EthUsdExecute()
        {
            try
            {
                await ConnectWebSocketAsync(_orderRepository, _appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.EthUsdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("ETH/USD.EthUsdExecute - Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        public async Task MonitorExecute()
        {
            try
            {
                IList<OrderEntity>? lastRegister = null;
                var orderList = await GetMonitorValues();

                if (orderList != null)
                    lastRegister = GetLastRegister(orderList);

                if (lastRegister != null)
                    foreach (var item in lastRegister)
                        ShowHighestAndlowestPriceOfEachAsset(item);

                if (lastRegister != null)
                    foreach (var item in lastRegister)
                        ShowAveragePriceOfEachAsset(item);

                if (orderList != null)
                    ShowAveragePriceOfEachAssetAccumulatedInTheLastSeconds(orderList);

                if (orderList != null)
                    ShowAverageQuantityOfEachAssetAccumulatedInTheLastSeconds(orderList);

            }
            catch (Exception ex)
            {
                _logger.LogError("OrderService.MonitorExecute - Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        #region Private Methods

        private static async Task ConnectWebSocketAsync(IOrderRepository _orderRepository, string serverUri, string channel)
        {
            using (ClientWebSocket webSocket = new())
            {
                await webSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);

                while (webSocket.State == WebSocketState.Open)
                {
                    _ = Task.Run(async () =>
                    {
                        while (webSocket.State == WebSocketState.Open)
                        {
                            WebSocketReceiveResult result;
                            ArraySegment<byte> buffer = new(new byte[20000]);
                            do
                            {
                                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                                string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                                var orderEntity = JsonConvert.DeserializeObject<OrderEntity>(message);

                                if (message.StartsWith("{\"data") && orderEntity != null)
                                {
                                    orderEntity.dateTime = DateTime.Now;
                                    await _orderRepository.SaveOrder(orderEntity);
                                }
                            }
                            while (!result.EndOfMessage);
                        }
                    });

                    string sendMessage = channel;
                    ArraySegment<byte> sendBuffer = new(Encoding.UTF8.GetBytes(sendMessage));
                    await webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private async Task<IList<OrderEntity>> GetMonitorValues()
        {
            return await _orderRepository.GetMonitorValues();
        }

        private IList<OrderEntity> GetLastRegister(IList<OrderEntity> orderEntity)
        {
            var orderList = new List<OrderEntity>();

            var btcusd = orderEntity.Where(x => x.channel == "order_book_btcusd").OrderByDescending(x => x.dateTime).FirstOrDefault();
            var ethusd = orderEntity.Where(x => x.channel == "order_book_ethusd").OrderByDescending(x => x.dateTime).FirstOrDefault();

            if (btcusd != null)
                orderList.Add(btcusd);
            else
                _logger.LogWarning("BTC/USD - Atualização não encontrada");

            if (ethusd != null)
                orderList.Add(ethusd);
            else
                _logger.LogWarning("ETH/USD - Atualização não encontrada");

            if (orderList.Count < 2)
                return null;
            else return orderList;
        }

        private void ShowHighestAndlowestPriceOfEachAsset(OrderEntity order)
        {
            var lowestPrice = Convert.ToDecimal(order.data.bids[0].LastOrDefault()) / Convert.ToDecimal(order.data.bids[0].FirstOrDefault());
            var highestPrice = lowestPrice;

            foreach (var item in order.data.bids)
            {
                var price = Convert.ToDecimal(item[1]) / Convert.ToDecimal(item[0]);

                if (price < lowestPrice)
                    lowestPrice = price;

                if (price > highestPrice)
                    highestPrice = price;
            }

            Console.WriteLine("");
            Console.WriteLine(order.channel.ToUpper());
            Console.WriteLine("Order datetime" + order.dateTime);
            Console.WriteLine($"Menor Preço: {lowestPrice}");
            Console.WriteLine($"Maior Preço: {highestPrice}");
            Console.WriteLine($"Média de Preço entre maior e menor valor: {(lowestPrice + highestPrice)/2}");
            Console.WriteLine("");
        }

        private void ShowAveragePriceOfEachAsset(OrderEntity order)
        {
            var averagePrice = Convert.ToDecimal(order.data.bids[0].LastOrDefault()) / Convert.ToDecimal(order.data.bids[0].FirstOrDefault());

            foreach (var item in order.data.bids)
                averagePrice += Convert.ToDecimal(item[1]) / Convert.ToDecimal(item[0]);

            Console.WriteLine("");
            Console.WriteLine(order.channel.ToUpper());
            Console.WriteLine("Order datetime" + order.dateTime);
            Console.WriteLine($"Média de preço entre todos: {averagePrice / order.data.bids.Count}");
            Console.WriteLine("");
        }

        private void ShowAveragePriceOfEachAssetAccumulatedInTheLastSeconds(IList<OrderEntity> orderList)
        {
        }

        private void ShowAverageQuantityOfEachAssetAccumulatedInTheLastSeconds(IList<OrderEntity> orderList)
        {
        }

        #endregion
    }
}