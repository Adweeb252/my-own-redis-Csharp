using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Caching;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6380);
server.Start();
bool isServerRunning = true;
//Mapping for set and get commands
// Dictionary<string, string> dict = new Dictionary<string, string>();
var db = MemoryCache.Default;


while (isServerRunning)
{
    var acceptTask = server.AcceptSocketAsync(); // wait for client
    var client = await acceptTask;
    Console.WriteLine($"Client connected at {client.RemoteEndPoint}");
    Task.Run(() => handleClient(client)); // handle client in a separate thread

}
void handleClient(Socket client)
{
    while (true)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = client.Receive(buffer);
        if (bytesRead == 0)
            return; // client disconnected

        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        var command = message.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        string cmdsize = command[0].Substring(1);

        int argsize = int.Parse(cmdsize); //arguement size which is the first line

        string cmd = command[2].ToUpper();
        string response = "none";

        if (cmd == "SET" && argsize >= 3)
        {
            string key = command[4];
            string val = command[6];
            if (argsize == 5)
            {
                int expt = int.Parse(command[10]);
                db.Set(key, (object)val, DateTimeOffset.Now.AddMilliseconds(expt));
            }
            else
            {
                db.Set(key, (object)val, DateTimeOffset.MaxValue);
            }
            // dict[key] = val;
            response = "+OK\r\n";
        }
        else if (cmd == "GET" && argsize == 2)
        {
            string key = command[4];
            if (db[key] != null)
            {
                response = $"+{db[key]}\r\n";
            }
            else
            {
                response = "$-1\r\n";
            }
        }
        else if (cmd == "PING")
        {
            response = "+PONG\r\n";
        }
        else if (cmd == "ECHO" && argsize > 1)
        {
            string echo = "";
            for (int i = 4; i < command.Length; i += 2)
            {
                echo += command[i] + " ";
            }
            response = $"+{echo}\r\n";
        }
        else
        {
            response = "-ERR unknown command\r\n";
        }
        client.Send(Encoding.UTF8.GetBytes(response));
        Console.WriteLine($"Sent: {response}");
    }
}
