using WorkerService1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<ChatConnector>();
        services.AddTransient<Nested>();
    })
    .Build();

await host.RunAsync();
