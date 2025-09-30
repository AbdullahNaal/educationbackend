using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace EducationPlatformBackend;

public record Account(string Guid, string Name, string? DeviceId);
// The following record is not being used yet; it would be used like the one above it
public record Purchase(string Guid, string ItemId);
public static class DatabaseHelper
{
    private static readonly string ConnectionString = "Data Source=education.db";

    public static IDbConnection GetConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    public static void InitializeDatabase()
    {
        using var connection = GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Accounts (
                    Guid TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    DeviceId TEXT
                );
                
                CREATE TABLE IF NOT EXISTS Purchases (
                    Guid TEXT,
                    ItemId TEXT,
                    FOREIGN KEY(Guid) REFERENCES Accounts(Guid)
                );";
        command.ExecuteNonQuery();
    }

    public static void CreateAccount(string guid, string name)
    {
        using var connection = GetConnection();
        connection.Execute(
            "INSERT INTO Accounts (Guid, Name) VALUES (@Guid, @Name)",
            new { Guid = guid, Name = name });
    }
    
    public static bool AttachDevice(string guid, string deviceId)
    {
        using var connection = GetConnection();
        var existingDevice = connection.QueryFirstOrDefault<string>(
            "SELECT DeviceId FROM Accounts WHERE Guid = @Guid",
            new { Guid = guid });
    
        if (existingDevice != null)
            return false;
    
        connection.Execute(
            "UPDATE Accounts SET DeviceId = @DeviceId WHERE Guid = @Guid",
            new { DeviceId = deviceId, Guid = guid });
        return true;
    }
    
    public static void DetachDevice(string guid)
    {
        using var connection = GetConnection();
        connection.Execute(
            "UPDATE Accounts SET DeviceId = NULL WHERE Guid = @Guid",
            new { Guid = guid });
    }
    
    public static string? GetAccountGuidByDevice(string deviceId)
    {
        using var connection = GetConnection();
        return connection.QueryFirstOrDefault<string>(
            "SELECT Guid FROM Accounts WHERE DeviceId = @DeviceId",
            new { DeviceId = deviceId });
    }
    
    public static void AddPurchase(string guid, string itemId)
    {
        using var connection = GetConnection();
        connection.Execute(
            "INSERT INTO Purchases (Guid, ItemId) VALUES (@Guid, @ItemId)",
            new { Guid = guid, ItemId = itemId });
    }
    
    public static List<string> GetPurchasesByDevice(string deviceId)
    {
        using var connection = GetConnection();
        var guid = GetAccountGuidByDevice(deviceId);
        if (guid == null)
            return new List<string>();
    
        return connection.Query<string>(
            "SELECT ItemId FROM Purchases WHERE Guid = @Guid",
            new { Guid = guid }).ToList();
    }
    public static List<Account> GetAllAccounts()
    {
        using var connection = GetConnection();
        return connection.Query<Account>(
            "SELECT Guid, Name, DeviceId FROM Accounts"
        ).ToList();
    }
}