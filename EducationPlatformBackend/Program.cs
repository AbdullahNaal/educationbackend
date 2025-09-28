using EducationPlatformBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

DatabaseHelper.InitializeDatabase();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/create-account", (string Name) =>
{
    string guid = Guid.NewGuid().ToString();
    DatabaseHelper.CreateAccount(guid, Name);
    return guid;
});

app.MapPut("/attach-device", (string deviceId, string guid) =>
{
    if (DatabaseHelper.AttachDevice(guid, deviceId))
        return Results.Ok();
    return Results.Forbid();
});

app.MapPut("/detach-device", (string guid) =>
{
    DatabaseHelper.DetachDevice(guid);
    return Results.Ok();
});

app.MapPost("/add-purchase", (string guid, string purchaseItemId) =>
{
    DatabaseHelper.AddPurchase(guid, purchaseItemId);
    return Results.Ok();
});

app.MapGet("/purchases", (string deviceId) =>
{
    return DatabaseHelper.GetPurchasesByDevice(deviceId);
});

app.Run();