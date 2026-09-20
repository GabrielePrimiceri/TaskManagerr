using Applicazione.Interfacce;
using Applicazione.CasiUso;
using Infrastruttura.Archivi;
using Infrastruttura.Configurazione;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WPFApp.ModelliVista;

namespace WPFApp;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs argomentiAvvio)
    {
        base.OnStartup(argomentiAvvio);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configurazione =>
            {
                configurazione.Sources.Clear();
                configurazione.SetBasePath(AppContext.BaseDirectory);
                configurazione.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((contesto, servizi) =>
            {
                var impostazioni = contesto.Configuration.GetSection("Firebase").Get<ImpostazioniFirebase>()
                    ?? throw new InvalidOperationException("Manca la configurazione Firebase.");

                servizi.AddSingleton<IArchivioAttivita>(_ => new ArchivioFirebase(impostazioni.DatabaseUrl));
                servizi.AddSingleton<IServizioGestoreAttivita, ServizioGestoreAttivita>();
                servizi.AddSingleton<ViewModel>();
                servizi.AddSingleton<FinestraPrincipale>();
            })
            .Build();

        await _host.StartAsync();
        _host.Services.GetRequiredService<FinestraPrincipale>().Show();
    }

    protected override async void OnExit(ExitEventArgs argomentiUscita)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(argomentiUscita);
    }
}
