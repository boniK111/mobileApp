namespace AlfaSoftWeb.Models;

public class Client
{
    public int client_id { get; set; }
    public string name { get; set; } = "";
    public string inn { get; set; } = "";
    public string? phone { get; set; }
}