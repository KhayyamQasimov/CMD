using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{
    static void Main()
    {
        var ip = IPAddress.Parse("127.0.0.1");
        var port = 27001;

        TcpListener tcpListener = new TcpListener(ip, port);
        tcpListener.Start(10);

        Console.WriteLine($"{tcpListener.Server.LocalEndPoint} listening.....");

        var client = tcpListener.AcceptTcpClient();
        Console.WriteLine($"{client.Client.RemoteEndPoint} connected server....");

        var stream = client.GetStream();
        var br = new BinaryReader(stream);
        var bw = new BinaryWriter(stream);

        bool davam = true;

        try
        {
            while (davam)
            {
                string command = br.ReadString().Trim().ToLower();
                Console.WriteLine($"Client command: {command}");

                switch (command)
                {
                    case "proclist":
                        bw.Write(GetAllProcesses());
                        break;

                    case "run":
                        string runName = br.ReadString();
                        bw.Write(CreateNewProcess(runName));
                        break;

                    case "kill":
                        string killName = br.ReadString();
                        bw.Write(KillProcess(killName));
                        break;

                    case "message":
                        string message = br.ReadString();
                        Console.WriteLine($"Client message: {message}");
                        bw.Write("Message received");
                        break;

                    case "exit":
                        bw.Write("Goodbye!");
                        davam = false;
                        break;

                    default:
                        bw.Write("Unknown command!");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection closed: {ex.Message}");
        }

        client.Close();
        tcpListener.Stop();
    }

    static string GetAllProcesses()
    {
        var sb = new StringBuilder();
        Process[] processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            sb.AppendLine("ID- " + process.Id);
            sb.AppendLine("Name- " + process.ProcessName);
            try
            {
                sb.AppendLine("HandleCount- " + process.HandleCount);
                sb.AppendLine("ThreadCount- " + process.Threads.Count);
            }
            catch
            {
                sb.AppendLine("HandleCount/ThreadCount- access denied");
            }
            sb.AppendLine("MachineName- " + process.MachineName);
            sb.AppendLine("-------------------------------------");
        }

        return sb.ToString();
    }

    static string CreateNewProcess(string name)
    {
        try
        {
            Process.Start(name);
            return $"{name} process started...";
        }
        catch (Exception ex)
        {
            return $"Process could not start: {ex.Message}";
        }
    }

    static string KillProcess(string name)
    {
        if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - 4);

        Process[] processes = Process.GetProcessesByName(name);

        if (processes.Length == 0)
            return $"{name} process not found.";

        int count = 0;
        foreach (Process process in processes)
        {
            try
            {
                process.Kill();
                count++;
            }
            catch (Exception ex)
            {
                return $"Process could not be killed: {ex.Message}";
            }
        }

        return $"{name} process killed... ({count} total)";
    }
}