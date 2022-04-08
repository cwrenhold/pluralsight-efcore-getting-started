// See https://aka.ms/new-console-template for more information
using SamuraiApp.Data;
using SamuraiApp.Domain;

Console.WriteLine("Hello, World!");

var _context = new SamuraiContext();
_context.Database.EnsureCreated();

getSamurais("Before Add");
addSamurai();
getSamurais("After Add");

void addSamurai()
{
    var samurai = new Samurai { Name = "Sampson" };
    _context.Samurais.Add(samurai);
    _context.SaveChanges();
}

void getSamurais(string text)
{
    var samurais = _context.Samurais.ToList();
    System.Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
    foreach (var samurai in samurais)
    {
        System.Console.WriteLine(samurai.Name);
    }
}