using System.IO;
using System.Text.Json;
using Applicazione.Dto;
using Applicazione.CasiUso;
using Infrastruttura.Archivi;

namespace ConsoleApp1;

internal class Programma
{
    private static async Task Main()
    {
        var indirizzoFirebase = OttieniIndirizzoFirebase();
        if (string.IsNullOrWhiteSpace(indirizzoFirebase))
        {
            Console.WriteLine("Indirizzo Firebase non trovato. Incolla DatabaseUrl di Firebase:");
            indirizzoFirebase = Console.ReadLine()?.Trim() ?? string.Empty;
        }

        var archivio = new ArchivioFirebase(indirizzoFirebase);
        var servizio = new ServizioGestoreAttivita(archivio);
        await new InterfacciaConsole(servizio).EseguiAsync();
    }

    private static string? OttieniIndirizzoFirebase()
    {
        var cartellaCorrente = new DirectoryInfo(Directory.GetCurrentDirectory());
        for (var indice = 0; indice < 6 && cartellaCorrente is not null; indice++)
        {
            var percorsoCandidato = Path.Combine(cartellaCorrente.FullName, "WPFApp", "appsettings.json");
            if (File.Exists(percorsoCandidato))
            {
                try
                {
                    using var flusso = File.OpenRead(percorsoCandidato);
                    using var documento = JsonDocument.Parse(flusso);
                    if (documento.RootElement.TryGetProperty("Firebase", out var firebase) && firebase.TryGetProperty("DatabaseUrl", out var indirizzo))
                        return indirizzo.GetString();
                }
                catch
                {
                    return null;
                }
            }
            cartellaCorrente = cartellaCorrente.Parent;
        }
        return null;
    }
}

internal class InterfacciaConsole
{
    private readonly ServizioGestoreAttivita _servizio;

    public InterfacciaConsole(ServizioGestoreAttivita servizio) => _servizio = servizio;

    public async Task EseguiAsync()
    {
        while (true)
        {
            MostraMenu();
            var scelta = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (scelta)
            {
                case "1": await ElencaAttivitaAsync(); break;
                case "2": await AggiungiAttivitaAsync(); break;
                case "3": await AvviaAttivitaAsync(); break;
                case "4": await AvviaTutteAsync(); break;
                case "0": return;
                default: Console.WriteLine("Opzione non valida. Premi Invio per continuare."); Console.ReadLine(); break;
            }
        }
    }

    private static void MostraMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Gestore attivita ===");
        Console.WriteLine("1) Elenca attivita");
        Console.WriteLine("2) Aggiungi attivita");
        Console.WriteLine("3) Avvia attivita per nome");
        Console.WriteLine("4) Avvia tutte le attivita");
        Console.WriteLine("0) Esci");
        Console.Write("Scegli un'opzione: ");
    }

    private async Task ElencaAttivitaAsync()
    {
        Console.Clear();
        var elenco = (await _servizio.OttieniTutteLeAttivitaAsync()).ToList();
        if (!elenco.Any())
            Console.WriteLine("Non ci sono attivita.");
        else
            foreach (var attivita in elenco)
                Console.WriteLine($"- {attivita.Nome} | Durata: {attivita.Durata} | Stato: {attivita.StatoAttivita.Stato}");
        AttendiInvio();
    }

    private async Task AggiungiAttivitaAsync()
    {
        Console.Clear();
        Console.Write("Nome: ");
        var nome = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Il nome non puo essere vuoto.");
            AttendiInvio();
            return;
        }

        Console.Write("Durata in secondi: ");
        if (!double.TryParse(Console.ReadLine(), out var durata))
            durata = 0;

        await _servizio.CreaAttivitaAsync(new AttivitaDto(nome, durata, new StatoDto("IN_ATTESA")));
        Console.WriteLine("Attivita salvata.");
        AttendiInvio();
    }

    private async Task AvviaAttivitaAsync()
    {
        Console.Clear();
        Console.Write("Nome dell'attivita da avviare: ");
        var nome = Console.ReadLine()?.Trim() ?? string.Empty;
        try
        {
            await _servizio.AvviaSingolaAttivitaAsync(new AttivitaDto(nome, 0, new StatoDto("IN_ATTESA")));
            Console.WriteLine("Attivita avviata.");
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore nell'avvio dell'attivita: {eccezione.Message}");
        }
        AttendiInvio();
    }

    private async Task AvviaTutteAsync()
    {
        await _servizio.AvviaTutteLeAttivitaAsync();
        Console.WriteLine("Tutte le attivita sono state avviate.");
        AttendiInvio();
    }

    private static void AttendiInvio()
    {
        Console.WriteLine("Premi Invio per tornare al menu.");
        Console.ReadLine();
    }
}
