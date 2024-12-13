using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Stampa un messaggio introduttivo
        Console.WriteLine("Certificate Analyzer");
        Console.WriteLine("---------------------");

        // Lista dei domini da analizzare
        string[] domains = { "google.com", "microsoft.com", "github.com", "apple.com", "amazon.com", "facebook.com", "twitter.com", "linkedin.com", "youtube.com", "wikipedia.org" };

        Console.WriteLine("Inizio analisi dei certificati...");

        // Ciclo per ogni dominio
        foreach (string domain in domains)
        {
            Console.WriteLine($"\nAnalisi per il dominio: {domain}");
            await CertificateAnalyzer.AnalyzeDomainCertificates(domain);
        }

        Console.WriteLine("\nAnalisi completata. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}

public class CertificateAnalyzer
{
    private const string API_ENDPOINT = "https://crt.sh/?q={0}&output=json";

    // Metodo per analizzare i certificati di un dominio specifico
    public static async Task AnalyzeDomainCertificates(string domain)
    {
        try
        {
            // Ottieni i dati dei certificati
            List<Certificate> certificates = await FetchCertificatesAsync(domain);

            // Analisi statistica
            CertificateAnalysis analysis = PerformAnalysis(certificates);

            // Mostra i risultati
            DisplayResults(domain, analysis);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore nell'analisi di {domain}: {ex.Message}");
        }
    }

    // Ottieni i certificati dall'API crt.sh
    private static async Task<List<Certificate>> FetchCertificatesAsync(string domain)
    {
        using HttpClient client = new HttpClient();
        string url = string.Format(API_ENDPOINT, domain);
        string response = await client.GetStringAsync(url);

        // Analizza la risposta JSON
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        List<Certificate> certData =
            JsonSerializer.Deserialize<List<Certificate>>(response, options);

        return certData;
    }

    // Esegui l'analisi statistica sui certificati
    private static CertificateAnalysis PerformAnalysis(List<Certificate> certificates)
    {
        CertificateAnalysis analysis = new CertificateAnalysis();

        // Calcola la distribuzione degli emittenti dei certificati
        analysis.IssuerDistribution =
            certificates.GroupBy(c => c.Issuer_name)
                        .Select(g => new IssuerInfo
                        {
                            Issuer = g.Key,
                            Count = g.Count()
                        })
                        .OrderByDescending(x => x.Count)
                        .ToList();

        // Calcola il periodo medio di validità
        analysis.AverageValidityDays =
            certificates.Average(c => (c.Not_after - c.Not_before).TotalDays);

        // Conta le lunghezze delle chiavi uniche
        analysis.KeyLengthDistribution =
            certificates.GroupBy(c => c.Pubkey_size)
                        .Select(g => new KeyLengthInfo
                        {
                            KeyLength = g.Key,
                            Count = g.Count()
                        })
                        .OrderBy(x => x.KeyLength)
                        .ToList();

        return analysis;
    }

    // Mostra i risultati dell'analisi
    private static void DisplayResults(string domain, CertificateAnalysis analysis)
    {
        Console.WriteLine($"Analisi dei certificati per {domain}");
        Console.WriteLine("----------------------------");

        // Mostra la distribuzione degli emittenti
        Console.WriteLine("Distribuzione degli emittenti:");
        foreach (IssuerInfo issuer in analysis.IssuerDistribution)
        {
            Console.WriteLine($"{issuer.Issuer}: {issuer.Count} certificati");
        }

        // Mostra la validità media
        Console.WriteLine($"Periodo medio di validità: {analysis.AverageValidityDays:F2} giorni");

        // Mostra la distribuzione delle lunghezze delle chiavi
        Console.WriteLine("Distribuzione delle lunghezze delle chiavi:");
        foreach (KeyLengthInfo keyLength in analysis.KeyLengthDistribution)
        {
            Console.WriteLine($"{keyLength.KeyLength} bit: {keyLength.Count} certificati");
        }
    }

    // Modello dei dati del certificato
    public class Certificate
    {
        public string Issuer_name { get; set; }
        public DateTime Not_before { get; set; }
        public DateTime Not_after { get; set; }
        public int Pubkey_size { get; set; }
    }

    // Contenitore per i risultati dell'analisi
    public class CertificateAnalysis
    {
        public List<IssuerInfo> IssuerDistribution { get; set; }
        public double AverageValidityDays { get; set; }
        public List<KeyLengthInfo> KeyLengthDistribution { get; set; }
    }

    // Classi helper per i risultati dell'analisi
    public class IssuerInfo
    {
        public string Issuer { get; set; }
        public int Count { get; set; }
    }

    public class KeyLengthInfo
    {
        public int KeyLength { get; set; }
        public int Count { get; set; }
    }
}
