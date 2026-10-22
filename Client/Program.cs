using System.Net;
using System.Net.Sockets;

namespace Client;

class Program
{
    static void Main()
    {
        var client = new TcpClient("127.0.0.1", 27001);

        var stream = client.GetStream();
        var bw = new BinaryWriter(stream);
        var br = new BinaryReader(stream);

        string[] menuItems = { "Proclist", "Run", "Kill", "Message", "Exit" };

        bool davam = true;
        while (davam)
        {
            int selected = ShowMenu(menuItems);

            Console.Clear();

            switch (selected)
            {
                case 0:
                    bw.Write("proclist");
                    Console.WriteLine("Server answer:");
                    Console.WriteLine(br.ReadString());
                    break;

                case 1:
                    Console.Write("Process name to run: ");
                    string runName = Console.ReadLine();
                    bw.Write("run");
                    bw.Write(runName);
                    Console.WriteLine($"Server answer: {br.ReadString()}");
                    break;

                case 2:
                    Console.Write("Process name to kill: ");
                    string killName = Console.ReadLine();
                    bw.Write("kill");
                    bw.Write(killName);
                    Console.WriteLine($"Server answer: {br.ReadString()}");
                    break;

                case 3:
                    Console.Write("Your message: ");
                    string message = Console.ReadLine();
                    bw.Write("message");
                    bw.Write(message);
                    Console.WriteLine($"Server answer: {br.ReadString()}");
                    break;

                case 4:
                    bw.Write("exit");
                    Console.WriteLine($"Server answer: {br.ReadString()}");
                    davam = false;
                    break;
            }

            if (davam)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey(true);
            }
        }

        client.Close();
    }

    static int ShowMenu(string[] items)
    {
        int selected = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("CMD");
            Console.WriteLine();

            for (int i = 0; i < items.Length; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($" > {items[i]} ");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"   {items[i]}");
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
                selected = (selected == 0) ? items.Length - 1 : selected - 1;
            else if (key == ConsoleKey.DownArrow)
                selected = (selected == items.Length - 1) ? 0 : selected + 1;

        } while (key != ConsoleKey.Enter);

        return selected;
    }
}