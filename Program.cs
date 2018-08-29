using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    class Program
    {
        static Options CommandOptions;
        static FileSystem FileSystem;
        static void Main(string[] args) => Main(args, false).GetAwaiter().GetResult();

        static async Task Main(string[] args, bool sync)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>((Options) =>
            {
                CommandOptions = Options;
            })
            .WithNotParsed((err) =>
            {
                //error code 10022: Argument invalid.
                Environment.Exit(10022);
            });

            Console.ForegroundColor = ConsoleColor.White;
            FileSystem = new FileSystem(CommandOptions.Root);

            Console.WriteLine($"Exploring root folder. ({FileSystem.RootFolder.Path})");

            Console.ForegroundColor = ConsoleColor.DarkGray;

            FileSystem.RootFolder.ToString("", true);

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Processing Completed.");
            Console.WriteLine($"Starting web server on port {CommandOptions.Port}.");

            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Request to close webserver accepted.");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Web server closed.");
                Console.ForegroundColor = ConsoleColor.White;
                e.Cancel = false;
                cts.Cancel();
            };
            try
            {
                await Listen($"http://*:{CommandOptions.Port}/", CommandOptions.MaxRequests, cts.Token);
            }
            catch(HttpListenerException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not open port, check if the port is not used or if you have access to the port. (code: {e.ErrorCode})");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey(true);
            }
            catch(System.IO.IOException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Some files can not be parsed. {e.Message} (code: {e.HResult})");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey(true);
            }
        }
        static public async Task Listen(string prefix, int maxConcurrentRequests, CancellationToken token)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            var requests = new HashSet<Task>();
            for (int i = 0; i < maxConcurrentRequests; i++)
                requests.Add(listener.GetContextAsync());

            while (!token.IsCancellationRequested)
            {
                Task t = await Task.WhenAny(requests);
                requests.Remove(t);

                if (t is Task<HttpListenerContext>)
                {
                    var context = (t as Task<HttpListenerContext>).Result;
                    requests.Add(ProcessRequestAsync(context));
                    requests.Add(listener.GetContextAsync());
                }
            }
            listener.Stop();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Webserver closed.");
            Environment.Exit(0);
        }

        static public async Task ProcessRequestAsync(HttpListenerContext context)
        {
            Console.WriteLine($"Request to {context.Request.Url.LocalPath} from {context.Request.RemoteEndPoint}.");
            try
            {
                byte[] bytes = FileSystem.GetFile(context.Request.Url.LocalPath).ReadBytes();
                await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                context.Response.Close();
            }
            catch (System.IO.FileNotFoundException fnf)
            {
                string html = $"<body style=\"text-align: center;\"><h1>404</h1><hr><p>File not found.<br>Filepath: {fnf.Message}</p></body>";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(html);
                await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                context.Response.Close();

            }
        }
        class Options
        {
            [Option('r', "root", HelpText = "The root of the webserver.")]
            public string Root { get; set; } = Environment.CurrentDirectory;
            [Option('p', "port", Default = 80, HelpText = "The port to be opened.")]
            public int Port { get; set; }
            [Option("maxrequests", Default = 25, HelpText = "The maximum requests to be handeled.")]
            public int MaxRequests { get; set; }
            [Usage]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Normal scenario", new Options { Root = "TestSite" });
                }
            }
        }
    }
}
