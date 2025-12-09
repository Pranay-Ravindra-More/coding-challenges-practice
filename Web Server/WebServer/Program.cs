// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

Console.WriteLine("Secure web server started");

const int Port = 8080;
const int MaxHeadersBytes = 8192;
const int ClientReceiveTimeoutsMs = 5000;
const int ClientSendTimeoutMs = 5000;
string baseDir = AppContext.BaseDirectory;

var routes = new Dictionary<string, Func<string>>
{
    {"/",() => File.ReadAllText(baseDir+"index.html")  },
    {"/index",() => File.ReadAllText(baseDir+"index.html")  }
};

Socket listener = null;

try
{
    listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    listener.Bind(new IPEndPoint(IPAddress.Loopback,Port)); // IpAddress.Loopback provides localhost address 
    listener.Listen(10);
    Console.WriteLine($"Server is listening on {IPAddress.Loopback}:{Port} (baseDir: {baseDir})");

    while (true)
    {
        Socket clientSocket = listener.Accept(); //accept connection

        System.Threading.ThreadPool.QueueUserWorkItem(_ => HandleClient(clientSocket));
    }
}catch(Exception ex)
{
    Console.WriteLine($"Server Error: {ex.Message}");
}
finally
{
    listener?.Close();
}


void HandleClient(Socket client)
{
    Console.WriteLine($"Thread running: {Environment.CurrentManagedThreadId}");

    using (client)
    {
        try
        {
            client.ReceiveTimeout = ClientReceiveTimeoutsMs;
            client.SendTimeout = ClientSendTimeoutMs;

            byte[] buffer = new byte[1024];
            int totalRead = 0;
            var headerBuilder = new StringBuilder();
            bool headerComplete = false;

            while(!headerComplete && totalRead < MaxHeadersBytes)
            {
                int read = client.Receive(buffer, 0, Math.Min(buffer.Length, MaxHeadersBytes - totalRead), SocketFlags.None);
                if (read == 0) break;
                totalRead += read;
                string chunk = Encoding.UTF8.GetString(buffer, 0, read);
                headerBuilder.Append(chunk);

                if (headerBuilder.ToString().Contains("\r\n\r\n"))
                {
                    headerComplete = true;
                    break;
                }
            }

            if(!headerComplete) {
                //too  large header
                Console.WriteLine("Too large headers");
                return;
            }

            string headerText = headerBuilder.ToString();
            int headerEndIndex = headerText.IndexOf("\r\n\r\n");
            headerText = headerText.Substring(0, headerEndIndex + 4);

            string[] requestLines = headerText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (requestLines.Length == 0) return;

            string requestLine = requestLines[0];
            string[] parts = requestLine.Split(" ");

            if(parts.Length < 2)
            {
                SendSimpleResponse(client, 404, "Bad Request");
                return;
            }

            string method = parts[0].ToUpperInvariant();
            string rawPath = parts[1];

            if (method != "GET")
            {
                SendSimpleResponse(client, 405, "Method Not Allowed", additionalHeaders: "Allow:  GET\r\n");
                return;
            }

            string path;
            try
            {
                path = Uri.UnescapeDataString(rawPath);
            }
            catch
            {
                SendSimpleResponse(client, 400, "Bad Request");
                return;
            }

            int qidx = path.IndexOf("?");
            if (qidx > 0) path = path.Substring(0, qidx);
            int hashIdx = path.IndexOf("#");
            if (hashIdx > 0) path = path.Substring(0, hashIdx);

            if(routes.TryGetValue(path, out var handler))
            {
                string body = handler();
                SendSimpleResponse(client, 200, body);
            }
            else
            {
                SendSimpleResponse(client, 400, "Not Found", "<h1>404 Not Found</h1>");
            }

            //string requestedFile = Path.Combine(baseDir+$"{path.Substring(1)}.html")

            //if(path=="/" || path.Equals("/index", StringComparison.OrdinalIgnoreCase))
            //{
            //    string requestedFile = Path.Combine(baseDir + "index.html");
            //    string fullPath;

            //    try
            //    {
            //        fullPath = Path.GetFullPath(requestedFile);
            //    }
            //    catch
            //    {
            //        SendSimpleResponse(client, 500, "Internal Server Error");
            //        return;
            //    }

            //    if (!File.Exists(fullPath))
            //    {
            //        SendSimpleResponse(client, 404, "Not Found");
            //        return;
            //    }

            //    byte[] fileBytes = File.ReadAllBytes(fullPath);

            //    var headers = new StringBuilder();
            //    headers.AppendLine("HTTP/1.1 200 OK");
            //    headers.AppendLine("Content-Type: text/html; charset=utf-8");
            //    headers.AppendLine($"Content-Length: {fileBytes.Length}");
            //    headers.AppendLine("Connection: close");
            //    headers.AppendLine();

            //    byte[] headersByte = Encoding.UTF8.GetBytes(headers.ToString());

            //    client.Send(headersByte);
            //    client.Send(fileBytes);
            //}
            //else
            //{
            //    SendSimpleResponse(client, 404, "<h1>404 Not Found!</h1>");
            //}
           
        }catch(Exception e)
        {
            Console.WriteLine($"Client handling error: {e.Message}");
            SendSimpleResponse(client, 500, "Internal Server Error");
        }
    }
}

void SendSimpleResponse(Socket socket, int statusCode, string body, string? additionalHeaders = null)
{
    string reason = statusCode switch
    {
        200 => "OK",
        400 => "Bad Request",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        500 => "Internal Server Error",
        _ => "Error"
    };

    byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
    var sb = new StringBuilder();
    sb.AppendLine($"HTTP/1.1 {statusCode} {reason}");
    sb.AppendLine("Content-Type: text/html; charset=utf-8");

    if (!string.IsNullOrEmpty(additionalHeaders))
    {
        sb.AppendLine(additionalHeaders.TrimEnd());
    }

    sb.AppendLine($"Content-Length: {bodyBytes.Length}");
    sb.AppendLine("Connection: close");
    sb.AppendLine();

    byte[] headersBytes = Encoding.UTF8.GetBytes(sb.ToString());

    try
    {
        socket.Send(headersBytes);
        socket.Send(bodyBytes);
    }
    catch
    {
        Console.WriteLine("Error while sending response");
    }
    
    
}


//void RouteToEndpoint(Socket client, string path)