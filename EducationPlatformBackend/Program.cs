using EducationPlatformBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

DatabaseHelper.InitializeDatabase();

var app = builder.Build();

// Use the PORT environment variable provided by Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/", () => "API is running!");

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

app.MapGet("/accounts", () =>
{
    return DatabaseHelper.GetAllAccounts();
});

app.Run(url);