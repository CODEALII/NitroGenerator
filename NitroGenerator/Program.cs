using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static readonly string ValidLogPath = "valid.log";
    private static readonly string InvalidLogPath = "invalid.log";
    private static readonly int CodeLength = 16;

    static async Task Main()
    {
        var program = new Program();
        await program.StartMain();
    }

    public async Task StartMain()
    {
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "💗 Nitro Generator 💗";

        PrintBanner();

        Console.Write("\nHow many links would you like to generate and check? → ");
        if (!int.TryParse(Console.ReadLine(), out int totalCodes) || totalCodes <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Invalid input!");
            Console.ResetColor();
            await StartMain();
            return;
        }

        Console.WriteLine("\n⏳ Starting code generation and validation...\n");

        using var client = new HttpClient();

        for (int i = 0; i < totalCodes; i++)
        {
            string code = GenerateCode(CodeLength);
            string fullLink = $"https://discord.gift/{code}";

            bool isValid = await CheckDiscordCode(client, code);

            lock (Console.Out)
            {
                Console.ForegroundColor = isValid ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write(isValid ? "[VALID] " : "[INVALID] ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(fullLink);
                Console.ResetColor();
            }

            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {fullLink}{Environment.NewLine}";
            string logFile = isValid ? ValidLogPath : InvalidLogPath;
            lock (logFile)
            {
                File.AppendAllText(logFile, logEntry);
            }
        }

        Console.WriteLine("\n✅ Process completed! Results saved in 'valid.log' and 'invalid.log'");
        Console.WriteLine("\nPress any key to restart...");
        Console.ReadKey();
        await StartMain();
    }

    static string GenerateCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var rand = new Random();
        char[] code = new char[length];
        for (int i = 0; i < length; i++)
            code[i] = chars[rand.Next(chars.Length)];
        return new string(code);
    }

    static async Task<bool> CheckDiscordCode(HttpClient client, string code)
    {
        string url = $"https://discord.com/api/v9/entitlements/gift-codes/{code}?with_application=false&with_subscription_plan=true";

        try
        {
            var response = await client.GetAsync(url);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(@"
   _  __                __                                
  / |/ /()/7 _   _    ,'_/  __ _     __ _   _   /7  _   _ 
 / || //7/_7//7,'o|  / /_n,'o// \/7,'o///7,'o| /_7,'o| //7
/_/|_///// //  |_,'  |__,'|_(/_n_/ |_(//  |_,7//  |_,'//  
                                                          
");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("                      💜 by Your Mum 💗");
        Console.ResetColor();
    }
}
