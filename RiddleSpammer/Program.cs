using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddleSpammer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<RiddleClient> clients = new List<RiddleClient>();
            for (int i = 0; i < 5; i++)
            {
                var client = new RiddleClient(207024, 5);
                client.Start();
                clients.Add(client);
            }

            Console.ReadLine();

            var sum = clients.Sum(i => i.Sent);
            clients.ForEach(i => i.Dispose());
            Console.WriteLine($"Requests sent: {sum}");
        }
    }
}
