using AlfaSoftWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;

namespace AlfaSoftWeb.Services;

public class DataService
{
    private readonly string _connStr;

    public DataService(IWebHostEnvironment env)
    {
        // База лежит в wwwroot/alfasoft.db
        var path = Path.Combine(env.WebRootPath, "alfasoft.db3");
        _connStr = $"Data Source={path}";
        Console.WriteLine($"DB path: {_connStr}");
    }

    public List<Client> GetClients()
    {

        
        var list = new List<Client>();
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT client_id, name, inn, phone 
                            FROM ""Клиент"" ORDER BY name";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Client
            {
                client_id = r.GetInt32(0),
                name = r.GetString(1),
                inn = r.GetString(2),
                phone = r.IsDBNull(3) ? null : r.GetString(3)
            });
        }
        return list;
    }

    public List<Ticket> GetTickets()
    {
        var list = new List<Ticket>();
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT t.ticket_id, c.name, tt.name, ts.name, t.subject
            FROM ""Заявка"" t
            JOIN ""Клиент"" c         ON c.client_id  = t.client_id
            JOIN ""Тип заявки"" tt    ON tt.type_id   = t.type_id
            JOIN ""Статус заявки"" ts ON ts.status_id = t.status_id
            ORDER BY t.ticket_id DESC";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Ticket
            {
                ticket_id = r.GetInt32(0),
                client_name = r.GetString(1),
                type_name = r.GetString(2),
                status_name = r.GetString(3),
                subject = r.GetString(4)
            });
        }
        return list;
        
    }
    // ---------- КЛИЕНТЫ ----------

    public Client? GetClient(int id)
    {
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT client_id, name, inn, phone 
                        FROM ""Клиент"" WHERE client_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new Client
        {
            client_id = r.GetInt32(0),
            name = r.GetString(1),
            inn = r.GetString(2),
            phone = r.IsDBNull(3) ? null : r.GetString(3)
        };
    }

    public void AddClient(Client c)
    {
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO ""Клиент""(name, inn, phone) 
                        VALUES(@n, @i, @p)";
        cmd.Parameters.AddWithValue("@n", c.name);
        cmd.Parameters.AddWithValue("@i", c.inn);
        cmd.Parameters.AddWithValue("@p", (object?)c.phone ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void UpdateClient(Client c)
    {
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"UPDATE ""Клиент"" SET name=@n, inn=@i, phone=@p 
                        WHERE client_id=@id";
        cmd.Parameters.AddWithValue("@n", c.name);
        cmd.Parameters.AddWithValue("@i", c.inn);
        cmd.Parameters.AddWithValue("@p", (object?)c.phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", c.client_id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteClient(int id)
    {
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"DELETE FROM ""Клиент"" WHERE client_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    // ---------- ЗАЯВКИ ----------

    public void AddTicket(int clientId, int typeId, int statusId, string subject)
    {
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO ""Заявка""
                        (client_id, type_id, status_id, subject, created_at)
                        VALUES(@c, @t, @s, @sub, datetime('now'))";
        cmd.Parameters.AddWithValue("@c", clientId);
        cmd.Parameters.AddWithValue("@t", typeId);
        cmd.Parameters.AddWithValue("@s", statusId);
        cmd.Parameters.AddWithValue("@sub", subject);
        cmd.ExecuteNonQuery();
    }

    public void DeleteTicket(int id)
    {
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"DELETE FROM ""Заявка"" WHERE ticket_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    // ---------- СПРАВОЧНИКИ ----------

    public List<(int id, string name)> GetTicketTypes()
    {
        var list = new List<(int, string)>();
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT type_id, name FROM ""Тип заявки"" ORDER BY name";
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add((r.GetInt32(0), r.GetString(1)));
        return list;
    }

    public List<(int id, string name)> GetTicketStatuses()
    {
        var list = new List<(int, string)>();
        using var conn = new SqliteConnection(_connStr);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT status_id, name FROM ""Статус заявки"" ORDER BY status_id";
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add((r.GetInt32(0), r.GetString(1)));
        return list;
    }

}