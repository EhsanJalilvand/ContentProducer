using Microsoft.Extensions.Logging;
using SharedDomain.ObjectValues;
using SharedDomainService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.Services
{
    public class MessageHandler : IMessageHandler
    {
        private readonly NetworkStream _handler;
        private readonly Action _createAgain;
        private readonly Action _close;
        private readonly Serilog.Core.Logger _logger;
        public MessageHandler(NetworkStream handler, Action createAgain,  Serilog.Core.Logger logger, Action close)
        {
            _createAgain = createAgain;
            _handler = handler;
            _logger = logger;
            _close = close;
        }
        public async void SendMessage(CrawlResponseStatus message)
        {
            if (message == null)
                return;
            var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);
          
            try
            {
                
                var data = jsonMessage;
                byte[] binaryData = Encoding.UTF8.GetBytes(data);

                int bufferSize = 1024;
                int totalBytesSent = 0;
                _logger.Information($"Start To Send Response Data {data.Length}");
                while (totalBytesSent < binaryData.Length)
                {
                    int bytesToSend = Math.Min(bufferSize, binaryData.Length - totalBytesSent);
                    await _handler.WriteAsync(binaryData, totalBytesSent, bytesToSend);
                    totalBytesSent += bytesToSend;
                }
                _logger.Information($"Finish To Send Response Data");
                //await _handler.WriteAsync(binaryData,0,binaryData.Length);
                await _handler.FlushAsync();
                if (_close != null)
                    _close();
            }
            catch (Exception ex)
            {
                _handler.Close();
                if (_createAgain != null)
                    _createAgain();
            }
        }
    }
}
