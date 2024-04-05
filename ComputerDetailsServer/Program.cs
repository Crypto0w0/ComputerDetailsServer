using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

class Program
{
    static void Main()
    {
        try
        {
            byte[] bytes;
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            
            Console.WriteLine("Enter the port");
            int p = Convert.ToInt32(Console.ReadLine());
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, p);
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Udp);
            
            listener.Bind(localEndPoint);
            listener.Listen(100);
            
            Console.WriteLine("Waiting for connection");
            Socket handler = listener.Accept();
            DateTime lastRequest = DateTime.Now;
            
            while (true)
            {
                if ((DateTime.Now - lastRequest).TotalMinutes < 1) // Обмеження на час (1 хв)
                {
                    Console.WriteLine("You overused your time limit");
                    Console.WriteLine("Press any key to update");
                    Console.ReadKey();
                    continue;
                }
                Console.WriteLine("Enter the details name");
                string req = Console.ReadLine();
                string data = null;
                lastRequest = DateTime.Now;
                while (true)
                {
                    bytes = Encoding.ASCII.GetBytes(req);
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
                byte[] msg = Encoding.ASCII.GetBytes(GetPrice(data));
                handler.Send(msg);
                Console.WriteLine("Price: " + msg);
                int ans;
                Console.WriteLine("1 - stop, any key - continue");
                ans = Convert.ToInt32(Console.ReadLine());
                if (ans == 1)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static string GetPrice(string d)
    {
        if (d == "processor")
        {
            return "200$";
        }
        else if (d == "SSD")
        {
            return "100$";
        }
        else if (d == "video card")
        {
            return "1000$";
        }
        else if (d == "HDD")
        {
            return "70$";
        }
        else if (d == "RAM")
        {
            return "50$";
        }
        else
        {
            return "None in stock";
        }
    }
}