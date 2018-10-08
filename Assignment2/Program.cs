using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    class Program
    {
        private static readonly int PORT = 7;

        static void Main(string[] args)
        {
            IPAddress localAddress = IPAddress.Loopback;
            TcpListener serverSocket = new TcpListener(localAddress, PORT);

            serverSocket.Start();

            Console.WriteLine("TCP server is running on port number: " + PORT);

            while (true)
            {
                try
                {
                    TcpClient client = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Incoming client");
                    Task.Run((() => Communicate(client)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

        }
        private static void Communicate(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            while (true)
            {
                Assignment1.Class1 obj = new Assignment1.Class1();   // The class library (DLL) with the converter

                string request = reader.ReadLine();

                if (request != null)
                {
                    Console.WriteLine("Request: " + request);

                    string response = null;
                    string[] myArray = request.Split(' ');
                    if (request.Split(' ').Length == 2)
                    {
                        if ((myArray[0] == "TOGRAM" || myArray[0] == "TOOUNCE") && myArray[1] != null)
                        {
                            try
                            {
                                double value = double.Parse(myArray[1]);
                                if (myArray[1] == "TOGRAM")
                                {
                                    response = obj.ConvertToGrams(value).ToString();
                                }
                                else
                                {
                                    response = obj.ConvertToOunces(value).ToString();
                                }

                                Console.WriteLine("Response: " + response);
                                writer.WriteLine(response + "\n ");
                                Console.WriteLine();
                                Console.WriteLine();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Wrong");
                                writer.WriteLine("Please insert a valid number");
                                Console.WriteLine();
                                writer.WriteLine();
                                writer.Flush();
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Available actions: \nTOGRAM [number] \nTOOUNCE [number]");
                            writer.WriteLine("This action is unavailable");
                            Console.WriteLine();
                            writer.WriteLine();
                        }
                    }
                    if (request == "STOP")
                    {
                        break;
                    }
                    else
                    {
                        if (request.Split(' ').Length != 2)
                        {
                            Console.WriteLine("This action is unavailable");
                            Console.WriteLine();
                            writer.WriteLine("This action is unavailable");
                            writer.WriteLine();
                        }
                    }
                    writer.Flush();
                }
            }
            client.Close();
            Console.WriteLine("Client has disconnected. \nWaiting for a new connection...");
        }
    }
}
