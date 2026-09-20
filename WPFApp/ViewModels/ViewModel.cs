using System.Collections.ObjectModel;
using System.Windows.Threading;
using Applicazione.Dto;
using Applicazione.Interfacce;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WPFApp.ModelliVista;


public partial class ViewModel : ObservableObject
{
    private readonly IServizioGestoreAttivita _servizioAttivita;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private ObservableCollection<AttivitaDto> _attivita = [];

    [ObservableProperty]
    private AttivitaDto? _attivitaSelezionata;

    [ObservableProperty]
    private string _nomeNuovaAttivita = string.Empty;

    [ObservableProperty]
    private string _durataNuovaAttivita = string.Empty;

    [ObservableProperty]
    private string _messaggioErrore = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _numeroCompletate;

    [ObservableProperty]
    private int _numeroTotale;
    public string Riepilogo => $"Completate: {NumeroCompletate}/{NumeroTotale}";
    public bool PulsantiAbilitati => !IsLoading;

    public ViewModel(IServizioGestoreAttivita servizioAttivita)
    {
        _servizioAttivita = servizioAttivita;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _timer.Tick += async (_, _) => await CaricaAttivitaAsync();
        _ = CaricaAttivitaAsync(); // il _ = serve per dire che non mi interessa il risultato della chiamata,
                                   // quindi non voglio aspettare il completamento della chiamata,
                                   // ma voglio che venga eseguita in background.
    }

    partial void OnIsLoadingChanged(bool value)//OnNomeObservablePropertyChanged che esiste gia, quindi per implementare il metodo OnIsLoadingChanged, si deve usare partial
    {
        OnPropertyChanged(nameof(PulsantiAbilitati));//metodo della classe ObservableObject, che gli devo passare il nome della proprietà
                                                     //che voglio notificare, in questo caso PulsantiAbilitati, e 
                                                     //quando IsLoading cambia, voglio notificare che anche PulsantiAbilitati cambia, quindi lo faccio qui.
                                                     //Non lo fa in automatico perche' non e' un ObservableProperty, ma e' una proprietà calcolata, quindi devo notificare manualmente il cambiamento. 
    }

    [RelayCommand]
    private async Task CaricaAsync() => await CaricaAttivitaAsync();

    [RelayCommand]
    private async Task AggiungiAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeNuovaAttivita) ||
            !double.TryParse(DurataNuovaAttivita, out double durata) || durata < 0)
        {
            MessaggioErrore = "Inserisci nome e durata validi.";
            return;
        }

        await EseguiAsync(async () =>
        {
            await _servizioAttivita.CreaAttivitaAsync(
                new AttivitaDto(NomeNuovaAttivita, durata, new StatoDto("IN_ATTESA")));
            NomeNuovaAttivita = string.Empty;
            DurataNuovaAttivita = string.Empty;
            await CaricaAttivitaAsync();
        });
    }

    [RelayCommand]
    private async Task AvviaTutteAsync()
    {
        await EseguiAsync(async () =>
        {
            _timer.Start();//parte il timer di 0.5 secondi, quindi ogni 0.5 secondi viene richiamato il metodo CaricaAttivitaAsync, che ricarica la lista delle attività.
            try
            {
                await _servizioAttivita.AvviaTutteLeAttivitaAsync();
            }
            finally
            {
                _timer.Stop();
            }
            await CaricaAttivitaAsync();
        });
    }

    [RelayCommand]
    private async Task AvviaSelezionataAsync()
    {
        if (AttivitaSelezionata is null)
        {
            MessaggioErrore = "Seleziona un'attivita.";
            return;
        }

        await EseguiAsync(async () =>
        {
            _timer.Start();
            try
            {
                await _servizioAttivita.AvviaSingolaAttivitaAsync(AttivitaSelezionata);
            }
            finally
            {
                _timer.Stop();
            }
            await CaricaAttivitaAsync();
        });
    }

    private async Task CaricaAttivitaAsync()
    {
        var task = await _servizioAttivita.OttieniTutteLeAttivitaAsync();
        Attivita = new ObservableCollection<AttivitaDto>(task);
        NumeroTotale = Attivita.Count;
        NumeroCompletate = Attivita.Count(elemento => elemento.StatoAttivita.Stato == "COMPLETATA");
        OnPropertyChanged(nameof(Riepilogo));
    }

    private async Task EseguiAsync(Func<Task> azione)//vuo dire che come parametro voglio passare una funzione che non prende parametri e ritorna un Task,
                                                     //quindi una funzione asincrona
    {
        try
        {
            IsLoading = true;
            MessaggioErrore = string.Empty;
            await azione();
        }
        catch (Exception errore)
        {
            MessaggioErrore = errore.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
