namespace AlfaSoftWeb.Models;

public class Ticket
{
    public int ticket_id { get; set; }
    public string client_name { get; set; } = "";
    public string type_name { get; set; } = "";
    public string status_name { get; set; } = "";
    public string subject { get; set; } = "";
}
