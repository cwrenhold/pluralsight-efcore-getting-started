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
insertNewSamuraiWithAQuote();
addQuoteToExistingSamuraiWhileTracked();
explicitLoadQuotes();

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

void insertNewSamuraiWithAQuote()
{
    var samurai = new Samurai
    {
        Name = "Kambei Shimada",
        Quotes = new List<Quote>
        {
            new Quote { Text = "I've come to save you"}
        }
    };

    _context.Samurais.Add(samurai);
    _context.SaveChanges();
}

void addQuoteToExistingSamuraiWhileTracked()
{
    var samurai = _context.Samurais.First();

    samurai.Quotes.Add(new Quote
    {
        Text = "I bet you're happy that I've saved you!"
    });

    _context.SaveChanges();
}

void addQuoteToExistingSamuraiNotTracked(int samuraiId)
{
    var samurai = _contextNT.Samurais.Find(samuraiId);

    samurai.Quotes.Add(new Quote
    {
        Text = "Not that I saved you, will you feed me dinner?"
    });

    using (var context = new SamuraiContext())
    {
        // Add tracking to the entity
        context.Samurais.Update(samurai);

        // At this point, the quote should have the samurai ID added to it, even though the DB hasn't been hit, because EF Core knows the this should be the same as the parent samurai's ID

        // This will save the quote, but it will ALSO update the Samurai, even if nothing has changed, because EF doesn't know what has changed
        context.SaveChanges();
    }
}

void addQuoteToExistingSamuraiNotTrackedWithAttach(int samuraiId)
{
    var samurai = _contextNT.Samurais.Find(samuraiId);

    samurai.Quotes.Add(new Quote
    {
        Text = "Not that I saved you, will you feed me dinner?"
    });

    using (var context = new SamuraiContext())
    {
        // Add tracking to the entity, but assume the current entity is unmodified
        context.Samurais.Attach(samurai);

        // At this point, the quote should have the samurai ID added to it, even though the DB hasn't been hit, because EF Core knows the this should be the same as the parent samurai's ID

        // This will save the quote, but not the samurai as there were no changes on the Samurai
        context.SaveChanges();
    }
}

void addQuoteToExistingSamuraiNotTracked_Simpler(int samuraiId)
{
    var quote = new Quote 
    {
        Text = "Thanks for dinner!",
        SamuraiId = samuraiId
    };

    using var context = new SamuraiContext();
    context.Quotes.Add(quote);
    context.SaveChanges();
}

void eagerlyLoadSamuraiWithQuotes()
{
    // Retrieve all samurai and quotes in one query
    var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();

    // Can also use .ThenInclude to include children of children objects or Include(s => s.Child.Grandchild)
}

void eagerlyLoadSamuraiWithQuotesWithSplitQuery()
{
    // Retrieves all the samurai, then retrieves all the quotes for those samurai
    var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).AsSplitQuery().ToList();
}

void eargerlyLoadSamuraiWithFilteredQuotes()
{
    // Only include quotes which contain "Thanks"
    var samuraiWithQuotes = _context.Samurais
        .Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks")))
        .ToList();
}

void projectSomeProperties()
{
    var someProps = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
}

// NOTE: Anonymous types are NOT tracked, but child objects on an anonymous type CAN BE tracked if they are normal entity types
void projectSomePropertiesWithQuotes()
{
    var someProps = _context.Samurais
        .Select(s => new { s.Id, s.Name, s.Quotes })
        .ToList();
}

void projectSomePropertiesWithAggregatedQuotes()
{
    var someProps = _context.Samurais
        .Select(s => new { s.Id, s.Name, QuoteCount = s.Quotes.Count })
        .ToList();
}

void explicitLoadQuotes()
{
    // Ensure there's a horse for the first samurai
    _context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr. Ed" });
    _context.SaveChanges();
    
    // Reset th change tracker
    _context.ChangeTracker.Clear();

    var samurai = _context.Samurais.Find(1);

    // Load in the collection of quotes and the reference to the horse
    _context.Entry(samurai).Collection(s => s.Quotes).Load();
    _context.Entry(samurai).Reference(s => s.Horse).Load();
}

void filteringWithRelatedData()
{
    var samurais = _context.Samurais
                           .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                           .ToList();
}

void modifyingRelatedDataWhenNotTracked()
{
    // Note, if tracking is continued, EF will manage toe changes required, but this is specifically for an untracked change

    var samurai = _context.Samurais.Include(s => s.Quotes)
                                   .FirstOrDefault(s => s.Id == 2);

    var quote = samurai.Quotes[0];
    quote.Text += "Did you hear that again?";

    using var newContext = new SamuraiContext();
    // newContext.Quotes.Update(quote); // This will update ALL quotes for the samurai, even though only one was modified as there was no tracking in this context

    // Flag the specific instance as modified, not including any linked objects
    newContext.Entry(quote).State = EntityState.Modified;

    newContext.SaveChanges();
}

void removingSamuraiFromABattleWithInferredJoinTableRatherThanPayloadTable()
{
    var battleWithSamurai = _context.Battles
        .Include(b => b.Samurais.Where(s => s.Id == 12))
        .Single(s => s.BattleId == 1);
    var samurai = battleWithSamurai.Samurais[0];
    battleWithSamurai.Samurais.Remove(samurai);
    _context.SaveChanges();

    // // This WILL NOT WORK as the relationship is not tracked, so EF doesn't know to remove the link table entry
    // var notLinkedBattle = _context.Battles.Find(1);
    // var notLinkedSamurai = _context.Samurais.Find(12);
    // notLinkedBattle.Samurais.Remove(notLinkedSamurai);
    // _context.SaveChanges();
}

void removeSamuraiFromBattleWithPayloadTable()
{
    // Note: Using Set as there isn't an explicit DbSet, only an inferred one for this entity
    var b_s = _context.Set<BattleSamurai>()
                      .SingleOrDefault(bs => bs.BattleId == 1 && bs.SamuraiId == 10);

    if (b_s is not null)
    {
        _context.Remove(b_s);
        _context.SaveChanges();
    }
}

void getHorsesWithSamuraiWithoutNavigationPropertyFromHorse()
{
    var horseonly = _context.Set<Horse>().Find(3);

    var horseWithSamurai = _context.Samurais.Include(s => s.Horse)
                                            .FirstOrDefault(s => s.Horse.Id == 3);

    var horseSamuraiPairs = _context.Samurais
        .Where(s => s.Horse != null)
        .Select(s => new { s.Horse, Samurai = s })
        .ToList();
}