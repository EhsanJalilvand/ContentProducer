using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedDomain.Configs;
using SharedDomain.ObjectValues;
using SharedDomainService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedInfrastructure.Services
{
    public class CrawlClientHandler : ICrawlClientHandler
    {
        private readonly SocketInfo _socketInfo;
        private readonly ILogger<CrawlClientHandler> _logger;
        public CrawlClientHandler(IOptions<SocketInfo> socketInfoConfig, ILogger<CrawlClientHandler> logger)
        {
            _socketInfo = socketInfoConfig.Value;
            _logger = logger;
        }
        public async Task<bool> SendCommand(CrawlRequestCommand command, Action<CrawlResponseStatus> response)
        {
            var jsonCommand = System.Text.Json.JsonSerializer.Serialize(command);
            try
            {
                return await SendCommand(jsonCommand, async (message) =>
                {
                    if (message == null)
                        return;
                    if (response != null)
                        response(message);
                });
            }
            catch(Exception ex)
            {
                await Task.Delay(5000);
                return await SendCommand(command, response);
            }
        }
        private async Task<bool> SendCommand(string message, Func<CrawlResponseStatus, Task> response)
        {
            try
            {
            
                CrawlResponseStatus crawlResponseStatus = null;
                using (TcpClient client = new TcpClient(_socketInfo.Address, _socketInfo.Port))
                using (NetworkStream networkStream = client.GetStream())
                //using (StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8))
                //using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                {
                    byte[] binaryMessage = Encoding.UTF8.GetBytes(message);
                    //await networkStream.WriteAsync(binaryMessage,0, binaryMessage.Length);


                    int bufferSize = 1024;
                    int totalBytesSent = 0;
                    _logger.LogInformation($"Start Send Message {message}");
                    while (totalBytesSent < binaryMessage.Length)
                    {
                        int bytesToSend = Math.Min(bufferSize, binaryMessage.Length - totalBytesSent);
                        await networkStream.WriteAsync(binaryMessage, totalBytesSent, bytesToSend);
                        totalBytesSent += bytesToSend;
                    }
                    await networkStream.FlushAsync();

                    _logger.LogInformation($"Message Sended");
                    StringBuilder stringBuilder = new StringBuilder();

                    byte[] buffer = new byte[bufferSize];
                    while (true)
                    {
                        while (!networkStream.DataAvailable)
                            Thread.Sleep(100);
                        _logger.LogInformation($"Message Recieved");
                        int bytesRead =  await networkStream.ReadAsync(buffer, 0, buffer.Length);
                               if (bytesRead == 0)
                                 break;
                        _logger.LogInformation($"bytesRead {bytesRead}");



                        byte[] validData = buffer.Take(bytesRead).ToArray();
                        var str = System.Text.Encoding.UTF8.GetString(validData);

                        stringBuilder.Append(str, 0, bytesRead);
                        var dataString=stringBuilder.ToString();

                        crawlResponseStatus = dataString.IsValidJson<CrawlResponseStatus>();
                        if (crawlResponseStatus != null)
                            break;
                    }



                }
                if (crawlResponseStatus != null)
                {
                    await response(crawlResponseStatus);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

      
    }
}
