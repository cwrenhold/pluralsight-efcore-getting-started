using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

var _context = new SamuraiContext();
_context.Database.EnsureCreated();
var _contextNT = new SamuraiContextNoTracking();

getSamurais("Before Add");
addSamuraiByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
queryAggregates();
retrieveAndUpdateSamurai();
retrieveAndUpdateMultipleSamurai();
multipleDatabaseOperations();
retrieveAndDeleteSamurai();
// getSamurais("After Add");
queryAndUpdateBattles_Disconnected();

// Run a single query without tracking on the entities returned
var untrackedSamurai = _context.Samurais.AsNoTracking().FirstOrDefault();

void addSamuraiByName(params string[] names)
{
    foreach (var name in names)
    {
        _contextNT.Samurais.Add(new Samurai { Name = name });
    }

    _context.SaveChanges();
}

void getSamurais(string text)
{
    var samurais = _contextNT.Samurais
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
    var samurai = _contextNT.Samurais.Find(2);
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

void queryAndUpdateBattles_Disconnected()
{
    List<Battle> disconnectedBattles;
    using (var context = new SamuraiContext())
    {
        disconnectedBattles = context.Battles.ToList();
    }

    disconnectedBattles.ForEach(b =>
    {
        b.StartDate = new DateTime(1570, 1, 1);
        b.EndDate = new DateTime(1570, 12, 1);
    });

    using (var context2 = new SamuraiContext())
    {
        context2.UpdateRange(disconnectedBattles);
        context2.SaveChanges();
    }
}