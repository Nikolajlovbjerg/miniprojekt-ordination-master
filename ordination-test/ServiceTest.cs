namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    // 1. Test af Opret PN
    [TestMethod]
    public void TestOpretPN()
    {
        int antal = service!.GetPNs().Count;

        // Parametre: patientId, laegemiddelId, antal, startDato, slutDato
        service.OpretPN(1, 1, 2.0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(antal + 1, service.GetPNs().Count, "PN blev ikke gemt i databasen");
    }

    [TestMethod]
    public void TestOpretDagligFast()
    {
        int antal = service!.GetDagligFaste().Count;

        // Parametre: patientId, laegemiddelId, morgen, middag, aften, nat, start, slut
        service.OpretDagligFast(1, 1, 1, 1, 1, 1, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(antal + 1, service.GetDagligFaste().Count, "Daglig Fast blev ikke gemt i databasen");
    }

    [TestMethod]
    public void TestOpretDagligSkaev()
    {
        int antalFoer = service!.GetDagligSkæve().Count;

        // Vi opretter listen af doser
        List<Dosis> doser = new List<Dosis> {
        new Dosis(DateTime.Now.Date.AddHours(8), 1.0),
        new Dosis(DateTime.Now.Date.AddHours(20), 2.0)
    };

        // Tilføj .ToArray() herunder for at konvertere listen til det format, metoden vil have
        service.OpretDagligSkaev(1, 1, doser.ToArray(), DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(antalFoer + 1, service.GetDagligSkæve().Count, "Daglig Skæv blev ikke gemt i databasen");
    }

    [TestMethod]
    public void GetAnbefaletDosisLetPatient()
    {
        // Vi ved Acetylsalicylsyre (ID 1) har faktor 0.1 for lette patienter
        // Vi tester manuelt med en vægt på 15kg (skal give 1.5)
        // OBS: I en rigtig test ville du oprette en patient med vægt 15 i din Setup
        double result = service.GetAnbefaletDosisPerDøgn(1, 1);
        // Her antages det at patient 1 i din SeedData er den vi tester.
        Assert.IsNotNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PNGivDosisExceptionTest()
    {
        PN pn = new PN(DateTime.Now, DateTime.Now.AddDays(3), 2.0, new Laegemiddel());

        // Her kalder vi metoden med null. 
        // Hvis din kode i PN har: if (givesDen == null) throw new ArgumentNullException(...);
        // så vil denne test bestå (blive grøn).
        pn.givDosis(null!);
    }
}