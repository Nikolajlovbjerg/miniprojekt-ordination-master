namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen)
    {
        // Tjek om datoen for givning ligger inden for ordinationens gyldighedsperiode
        // Vi bruger .Date for at ignorere klokkeslæt i sammenligningen
        if (givesDen.dato.Date >= startDen.Date && givesDen.dato.Date <= slutDen.Date)
        {
            dates.Add(givesDen);
            return true;
        }

        return false;
    }

    public override double doegnDosis()
    {
        // Hvis der ikke er givet nogen dosis endnu, er døgndosis 0
        if (dates.Count == 0)
        {
            return 0;
        }

        // Find første og sidste dato medicinen er givet
        DateTime førsteGivning = dates.Min(d => d.dato).Date;
        DateTime sidsteGivning = dates.Max(d => d.dato).Date;

        // Beregn antal dage mellem første og sidste givning (inklusiv begge dage)
        // TimeSpan.Days giver forskellen, så vi lægger 1 til for at få antal dage i alt
        double antalDage = (sidsteGivning - førsteGivning).TotalDays + 1;

        // Formel: (antal gange anvendt * antal enheder) / antal dage
        return (dates.Count * antalEnheder) / antalDage;
    }


    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}
