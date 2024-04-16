using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SharedDomain.Configs;
using SharedDomain.ObjectValues;
using SharedDomainService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedInfrastructure.Services
{
    public class CrawlServerHandler : ICrawlServerHandler
    {
        private readonly SocketInfo _socketInfo;
        private readonly Serilog.Core.Logger _logger;
        public CrawlServerHandler(IOptions<SocketInfo> options)
        {
            _socketInfo = options.Value;
            _logger = new LoggerConfiguration()
.WriteTo.File("logApp_service.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();
        }
        public async Task<bool> StartService(Action<CrawlRequestCommand, IMessageHandler> message)
        {
            return await StartService(1024, async (command, handler) =>
            {


                // var command = a.IsValidJson<CrawlRequestCommand>();

                if (message != null)
                    message(command, handler);
            });
        }

        public async Task<bool> StartService(int messageLength, Action<CrawlRequestCommand, IMessageHandler> message)
        {
            IPHostEntry host = await Dns.GetHostEntryAsync(_socketInfo.Address);
            IPAddress ipAddress = host.AddressList.Last();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _socketInfo.Port);
            Socket _listener = null;
            Socket _handler = null;
            try
            {
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(1);
                _listener = listener;
                while (true)
                {
                 
                    bool close = false;
                    Socket handler = await listener.AcceptAsync();
                    _handler = handler;
                    close = false;
                    _logger.Information("Start  Recieved Message");
                 
                    using (NetworkStream networkStream = new NetworkStream(handler))
                    // using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8, false, 1024*4))
                    //using (StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        while (true)
                        {
                         
                            while ( !networkStream.DataAvailable)
                            {
                                Thread.Sleep(100);
                                if(close)
                                    break;
                            }
                            if (close)
                                break;
                            byte[] buffer = new byte[messageLength];
                            _logger.Information("Message Recieved");


                            int bytesRead = await networkStream.ReadAsync(buffer, 0, messageLength);
                            _logger.Information($"bytesRead {bytesRead}");
                            if (bytesRead == 0)
                                break;

                            //string data = new string(buffer, 0, bytesRead);


                            byte[] validData = buffer.Take(bytesRead).ToArray();
                            var str = System.Text.Encoding.UTF8.GetString(validData);
                            _logger.Information($"str {str}");
                            stringBuilder.Append(str, 0, bytesRead);
                            var dataString = stringBuilder.ToString();
                            _logger.Information($"dataString {dataString}");
                            var request = dataString.IsValidJson<CrawlRequestCommand>();
                            if (request == null)
                            {
                                _logger.Information($"Request Is Null");
                                continue;
                            }
                            else
                            {
                                networkStream.Flush();
                                _logger.Information($"Request Ok");
                            }
                            if (message != null)
                                message(request, new MessageHandler(networkStream, async () =>
                                {
                                    if (_listener != null)
                                    {
                                        _listener.Close();
                                    }
                                    await Task.Delay(5000);
                                    await StartService(messageLength, message);
                                }, _logger,
                                () =>
                                {
                                    networkStream.Flush();
                                    close = true;
                                }));
                        }
                    }
                    
                   // handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {

                if (_listener != null)
                {
                    _listener.Close();
                }
                if (_handler != null)
                {
                    _handler.Close();
                }
                await Task.Delay(5000);
                return await StartService(messageLength, message);
            }
        }

    }
}
