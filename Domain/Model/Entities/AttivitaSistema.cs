using Dominio.Modello.OggettiValore;

namespace Dominio.Modello.Entita;

public class AttivitaSistema
{
    private string _nome = string.Empty;
    private double _durata;
    private Stato _stato;

    public AttivitaSistema(string nome, double durata, Stato stato = Stato.IN_ATTESA)
    {
        Nome = nome;
        Durata = durata;
        StatoAttivita = stato;
    }

    public string Nome
    {
        get => _nome;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Il nome non puo essere nullo, vuoto o composto da soli spazi.");
            _nome = value;
        }
    }

    public double Durata
    {
        get => _durata;
        private set
        {
            if (value < 0)
                throw new ArgumentException("La durata non puo essere negativa.");
            _durata = value;
        }
    }

    public Stato StatoAttivita { get => _stato; private set => _stato = value; }

    public async Task AvviaAsync()
    {
        if (StatoAttivita == Stato.IN_CORSO)
            throw new InvalidOperationException("L'attivita e gia in corso.");
        if (StatoAttivita == Stato.COMPLETATA)
            throw new InvalidOperationException("L'attivita e gia completata e non puo essere riavviata.");
        StatoAttivita = Stato.IN_CORSO;
        await Task.Delay(TimeSpan.FromSeconds(Durata));
        StatoAttivita = Stato.COMPLETATA;
    }
}
