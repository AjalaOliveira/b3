using B3.Worker.Data;
using B3.Worker.Data.Interfaces;
using B3.Worker.HostedService;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Service.Process;
using B3.Worker.Shared.Settings;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
        services.Configure<MongoDBSettings>(hostContext.Configuration.GetSection("MongoDBSettings"));

        services.AddHostedService<BtcUsdHostedService>();
        services.AddHostedService<EthUsdHostedService>();
        services.AddHostedService<MonitorHostedService>();

        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IOrderRepository, OrderRepository>();
    })
    .Build();

host.Run();