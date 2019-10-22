
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using NEventSocket;
using NEventSocket.FreeSwitch;

namespace test_mod_event_socket
{
    class Program
    {
        private static void Main(string[] args)
        {
            //Reserve.Reserve1();
            Run();
            
            System.Console.ReadKey();
        }

        public static async void Run()
        {
            var client = await InboundSocket.Connect("192.168.99.66", 8021, "12345");
            var res = await client.Api("status");
            Console.WriteLine(res.BodyText);
            await client.SubscribeEvents(new EventName[] { EventName.PlaybackStop });
            client.Events.Subscribe(evnt => {
                Console.WriteLine(evnt.EventName);
                foreach(var v in evnt.Headers)
                {
                    Console.WriteLine(v.Key + " " + v.Value);
                }
                Console.WriteLine();
                Console.WriteLine();
            });
            
        }
    }

    public class ChannelAnswerObserver : IObserver<ChannelEvent>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(ChannelEvent value)
        {
            Console.WriteLine("Channel ChannelCreate Event " + value.UUID);
        }
    }

    public class Reserve
    {
        public static void Reserve1()
        {
            TcpClient v = new TcpClient("192.168.99.66", 8021);
            Thread.Sleep(100);
            if (v.Connected)
            {
                int dataCnt = v.Available;
                byte[] buff = new byte[dataCnt];
                using (var s = v.GetStream())
                {
                    using (var sw = new StreamWriter(s, ASCIIEncoding.ASCII))
                    {
                        var res = s.Read(buff, 0, dataCnt);
                        var str = Encoding.ASCII.GetString(buff);
                        Console.WriteLine(str);
                        if (str.Contains("auth"))
                        {
                            var auth_str = "auth 12345";
                            sw.WriteLine(auth_str);
                            sw.WriteLine("");
                            sw.Flush();
                            Thread.Sleep(200);
                            if (v.Available > 0)
                            {
                                byte[] buff1 = new byte[v.Available];
                                var res1 = s.Read(buff1, 0, buff1.Length);
                                var str1 = Encoding.ASCII.GetString(buff1);
                                Console.WriteLine(str1);
                            }
                        }
                        sw.WriteLine("api version;;status");
                        sw.WriteLine("console_execute: true");
                        sw.WriteLine("");
                        sw.Flush();
                        Thread.Sleep(300);
                        if (v.Available > 0)
                        {
                            byte[] buff1 = new byte[v.Available];
                            var res1 = s.Read(buff1, 0, buff1.Length);
                            var str1 = Encoding.ASCII.GetString(buff1);
                            Console.WriteLine(str1);
                        }
                        sw.WriteLine("events plain all");
                        sw.WriteLine("console_execute: true");
                        sw.WriteLine("");
                        sw.Flush();
                        Thread.Sleep(300);
                        
                        while (true)
                        {
                            if (v.Available > 0)
                            {
                                byte[] buff1 = new byte[v.Available];
                                var res1 = s.Read(buff1, 0, buff1.Length);
                                var str1 = Encoding.ASCII.GetString(buff1);
                                Console.WriteLine(str1);
                            }
                        }
                        
                    }
                }
            }
            else
            {
                Console.WriteLine("pizdos");
            }
        }
    }
}
