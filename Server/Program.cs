using Server.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using ZipcodeLib.Entity;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            const int PORT = 8080;
            const string HOST = "127.0.0.1";
            const int BACKLOG = 10;

            // получаем адреса для запуска сокета
            var ipPoint = new IPEndPoint(IPAddress.Parse(HOST), PORT);

            // создаем сокет
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(BACKLOG);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    var handler = listenSocket.Accept();

                    // получаем сообщение
                    var builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    // отправляем ответ
                    //string message = "ваше сообщение доставлено";
                    //data = Encoding.UTF8.GetBytes(message);

                    var packet = JsonConvert.DeserializeObject<PacketInfo>(builder.ToString());
                    var response = "";

                    if (packet != null)
                    {
                        switch (packet.Type)
                        {
                            case "GET_ZIPCODE":
                                {
                                    var getZipcodePacket = JsonConvert.DeserializeObject<GetZipcodePacketInfo>(builder.ToString());
                                    if (getZipcodePacket != null)
                                    {
                                        var zipcode = ZipcodeRepository.GetRepository().Find(p => { return p.zipcode == getZipcodePacket.zipcode; });
                                        if (zipcode != null)
                                        {
                                            var zipcodePacket = new ZipcodePacketInfo() { Zipcode = zipcode };
                                            response = JsonConvert.SerializeObject(zipcodePacket);
                                        }
                                        else
                                        {
                                            var errorPacket = new ErrorPacketInfo() { Message = "Can't find zipcode." };
                                            response = JsonConvert.SerializeObject(errorPacket);
                                        }
                                    }
                                    else
                                    {
                                        var errorPacket = new ErrorPacketInfo() { Message = "Incorrect packet data." };
                                        response = JsonConvert.SerializeObject(errorPacket);
                                    }
                                }
                                break;

                            default:
                                {
                                    var errorPacket = new ErrorPacketInfo() { Message = "Unknow packet type." };
                                    response = JsonConvert.SerializeObject(errorPacket);
                                }
                                break;
                        }
                    }
                    else
                    {
                        var errorPacket = new ErrorPacketInfo() { Message = "Incorrect packet type." };
                        response = JsonConvert.SerializeObject(errorPacket);
                    }

                    handler.Send(Encoding.UTF8.GetBytes(response));

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}



/*
            try
            {
                Console.WriteLine("Start");

                var zipcode = ZipcodeRepository.GetRepository().Find(p => { return p.zipcode == 211500; });
                if (zipcode != null)
                {
                    foreach (var i in zipcode.addressdata)
                    {
                        var str = $"{zipcode.zipcode} : {i.region_iid} ОБЛАСТЬ - {i.citytype_iid} {i.city_iid} {i.streettype_iid} {i.street_iid} {i.build_iid}";
                        Console.WriteLine(str);
                    }
                }

                Console.WriteLine("Finish");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey(false);
            
     */
