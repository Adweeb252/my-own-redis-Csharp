using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var client = server.AcceptSocket(); // wait for client
Console.WriteLine("Client connected at " + client.RemoteEndPoint);
client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
byte[] buffer = new byte[1024];
int bytesRead = client.Receive(buffer);
string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
Console.WriteLine($"Received: {message}");
// client.SendAsync(Encoding.UTF8.GetBytes("+PONG\r\n"), SocketFlags.None);
