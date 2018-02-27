using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZipcodeLib.Entity;

namespace ZipcodeClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //this.tbAddressdata.Text = String.Empty;

            if (String.IsNullOrWhiteSpace(this.tbZipcode.Text))
            {
                MessageBox.Show("Incorrect zipcode.", "Error");
            }

            try
            {
                const int port = 8080;
                const string address = "127.0.0.1";
                var ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(ipPoint);

                var getZipcodePacket = new GetZipcodePacketInfo() { zipcode = Int32.Parse(this.tbZipcode.Text.Trim()) };
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(getZipcodePacket));

                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                var packet = JsonConvert.DeserializeObject<PacketInfo>(builder.ToString());
                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case "ZIPCODE":
                            {
                                var zipcodePacket = JsonConvert.DeserializeObject<ZipcodePacketInfo>(builder.ToString());
                                if (zipcodePacket != null)
                                {
                                    if (zipcodePacket.Zipcode == null)
                                    {
                                        throw new NullReferenceException("Zipcode object is null");
                                    }
                                    var zipcode = zipcodePacket.Zipcode.zipcode;
                                    this.lstAddressdata.ItemsSource = zipcodePacket.Zipcode.addressdata
                                        .ConvertAll<string>(p => $"{zipcode} {p.region_iid } {p.citytype_iid} {p.city_iid} {p.streettype_iid} {p.street_iid} {p.build_iid}");
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect zipcode packet", "Error");
                                }
                            }
                            break;

                        case "ERROR":
                            {
                                var errorPacket = JsonConvert.DeserializeObject<ErrorPacketInfo>(builder.ToString());
                                if (String.IsNullOrWhiteSpace(errorPacket.Message))
                                {
                                    throw new NullReferenceException("Incorrect error message value.");
                                }
                                MessageBox.Show(errorPacket.Message, "Error");
                            }
                            break;

                        default:
                            {
                                MessageBox.Show("Unknow packet.", "Error");
                            }
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect packet.", "Error");
                }



                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }
    }
}



/*

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
 
namespace SocketTcpClient
{
    class Program
    {
        // адрес и порт сервера, к которому будем подключаться

        static void Main(string[] args)
        {
            try
            {
                
                static int port = 8005; // порт сервера
                static string address = "127.0.0.1"; // адрес сервера

                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
 
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                Console.Write("Введите сообщение:");
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);
 
                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
 
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                Console.WriteLine("ответ сервера: " + builder.ToString());
 
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}

 
*/
