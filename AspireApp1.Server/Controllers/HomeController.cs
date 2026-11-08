using Microsoft.AspNetCore.Mvc;
using AlfaSoftWeb.Models;
using AlfaSoftWeb.Services;

namespace AlfaSoftWeb.Controllers;

public class HomeController : Controller
{
    private readonly DataService _data;
    public HomeController(DataService data) => _data = data;

    public IActionResult Index()
    {
        ViewBag.Clients = _data.GetClients();
        ViewBag.Tickets = _data.GetTickets();
        return View();
    }

    // ----- КЛИЕНТЫ -----

    [HttpGet]
    public IActionResult CreateClient() => View(new Client());

    [HttpPost]
    public IActionResult CreateClient(Client model)
    {
        if (string.IsNullOrWhiteSpace(model.name) || string.IsNullOrWhiteSpace(model.inn))
        {
            ModelState.AddModelError("", "Название и ИНН обязательны");
            return View(model);
        }
        _data.AddClient(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult EditClient(int id)
    {
        var client = _data.GetClient(id);
        if (client == null) return NotFound();
        return View(client);
    }

    [HttpPost]
    public IActionResult EditClient(Client model)
    {
        if (string.IsNullOrWhiteSpace(model.name) || string.IsNullOrWhiteSpace(model.inn))
        {
            ModelState.AddModelError("", "Название и ИНН обязательны");
            return View(model);
        }
        _data.UpdateClient(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult DeleteClient(int id)
    {
        try { _data.DeleteClient(id); }
        catch { /* есть связанные записи */ }
        return RedirectToAction(nameof(Index));
    }

    // ----- ЗАЯВКИ -----

    [HttpGet]
    public IActionResult CreateTicket()
    {
        ViewBag.Clients = _data.GetClients();
        ViewBag.Types = _data.GetTicketTypes();
        ViewBag.Statuses = _data.GetTicketStatuses();
        return View(new Ticket());
    }

    [HttpPost]
    public IActionResult CreateTicket(Ticket model, int clientId, int typeId, int statusId)
    {
        if (string.IsNullOrWhiteSpace(model.subject))
        {
            ModelState.AddModelError("", "Тема обязательна");
            ViewBag.Clients = _data.GetClients();
            ViewBag.Types = _data.GetTicketTypes();
            ViewBag.Statuses = _data.GetTicketStatuses();
            return View(model);
        }
        _data.AddTicket(clientId, typeId, statusId, model.subject);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult DeleteTicket(int id)
    {
        try { _data.DeleteTicket(id); } catch { }
        return RedirectToAction(nameof(Index));
    }
}