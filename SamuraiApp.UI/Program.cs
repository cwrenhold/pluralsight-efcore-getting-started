using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

var _context = new SamuraiContext();
_context.Database.EnsureCreated();

getSamurais("Before Add");
addSamuraiByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
// getSamurais("After Add");

void addSamuraiByName(params string[] names)
{
    foreach (var name in names)
    {
        _context.Samurais.Add(new Samurai { Name = name });
    }

    _context.SaveChanges();
}

void getSamurais(string text)
{
    var samurais = _context.Samurais
        .TagWith("ConsoleApp.Program.GetSamurais method")
        .ToList();

    System.Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
    foreach (var samurai in samurais)
    {
        System.Console.WriteLine(samurai.Name);
    }
}