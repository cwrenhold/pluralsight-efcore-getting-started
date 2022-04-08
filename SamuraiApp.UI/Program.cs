using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

var _context = new SamuraiContext();
_context.Database.EnsureCreated();

getSamurais("Before Add");
addSamuraiByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
queryAggregates();
retrieveAndUpdateSamurai();
retrieveAndUpdateMultipleSamurai();
multipleDatabaseOperations();
retrieveAndDeleteSamurai();
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

void queryAggregates()
{
    var samurai = _context.Samurais.Find(2);
}

void retrieveAndUpdateSamurai()
{
    var samurai = _context.Samurais.FirstOrDefault();
    
    if (samurai is not null)
    {
        samurai.Name += "San";
        _context.SaveChanges();
    }
}

void retrieveAndUpdateMultipleSamurai()
{
    var samurai = _context.Samurais.Skip(1).Take(5).ToList();
    samurai.ForEach(s => s.Name += "San");
    _context.SaveChanges();
}

void multipleDatabaseOperations()
{
    var samurai = _context.Samurais.FirstOrDefault();

    if (samurai is not null)
    {
        samurai.Name += "San";
    }

    _context.Add(new Samurai { Name = "Shino"});
    _context.SaveChanges();
}

void retrieveAndDeleteSamurai()
{
    var samurai = _context.Samurais.Find(5);
    
    if (samurai is not null)
    {
        _context.Remove(samurai);
        _context.SaveChanges();
    }
}