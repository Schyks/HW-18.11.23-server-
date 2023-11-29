using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    static public IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);

    private static async Task Main(string[] args)
    {
        using Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(ipEndPoint);
        listener.Listen(10);
        Console.WriteLine("Сервер запущено. Очiкування клiєнтiв...");

        List<string> serverResponses = new List<string>
        {
            "Зрозумiло.",
            "Цiкаво!",
            "Продовжуйте розповiдь.",
            "Що ви думаєте про це?",
            "Ваша думка важлива для мене.",
            "Цiкавий пiдхiд до питання!",
            "Захоплююче розмовляти з вами.",
            "Ваша думка додає iнтриги нашiй бесiдi.",
            "Як вам вдалося придумати таке цiкаве питання?",
            "Продовжуйте, це цiкаво!",
            "Ви точно маєте непересiчний погляд на речi.",
            "Якщо ви бажаєте, можемо розглянути це глибше.",
            "Вашi думки дiйсно цiннi для мене.",
            "Ви привносите свою унiкальну думку у наше спiлкування.",
            "Це дуже цiкавий пiдхiд до теми!"
        };

        while (true)
        {
            var handler = await listener.AcceptAsync();

            _ = HandleClientAsync(handler, serverResponses);
        }
    }

    private static async Task HandleClientAsync(Socket handler, List<string> serverResponses)
    {
        Console.WriteLine($"Клiєнт пiдключено: {handler.RemoteEndPoint}");

        try
        {
            while (true)
            {
                var buffer = new byte[1024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                Console.WriteLine($"Отримано повiдомлення вiд {handler.RemoteEndPoint}: \"{response}\"");

                if (response.ToLower().Contains("bye"))
                {
                    var answer = Encoding.UTF8.GetBytes("До зустрiчi!");
                    await handler.SendAsync(answer, SocketFlags.None);
                    Console.WriteLine($"Отримано повiдомлення 'bye' вiд {handler.RemoteEndPoint}. Закриття з'єднання.");
                    break;
                }

                string randomResponse = GetRandomResponse(serverResponses);
                var echoBytes = Encoding.UTF8.GetBytes(randomResponse);
                await handler.SendAsync(echoBytes, SocketFlags.None);
                Console.WriteLine($"Вiдправлено вiдповiдь клiєнту {handler.RemoteEndPoint}: \"{randomResponse}\"");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при обробцi клiєнта {handler.RemoteEndPoint}: {ex.Message}");
        }
        finally
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
    private static string GetRandomResponse(List<string> responses)
    {
        Random random = new Random();
        int index = random.Next(responses.Count);
        return responses[index];
    }
}
