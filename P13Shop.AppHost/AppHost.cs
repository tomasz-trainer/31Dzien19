var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.P05Shop_API>("api");

// ApiBaseUrl klienta WASM pochodzi z wwwroot/appsettings.Development.json (port API z launchSettings: 7127).
builder.AddProject<Projects.P11BlazorWebAssembly_Client>("blazor")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
