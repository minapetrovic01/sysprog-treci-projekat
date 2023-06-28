using Projekat3;

internal class Program
{
    private static readonly string _urlServer="http://localhost:5000/";
    private static string _urlAPI = "https://www.reddit.com/api/v1/";
    private static async Task Main(string[] args)
    {
        WebServer server = new WebServer(_urlServer, _urlAPI);
        await server.Run();
        Console.ReadKey();
    }
}