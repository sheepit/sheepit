using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;

namespace SheepIt.Api.Infrastructure.Logger.CloudWatch
{
    public class JsonTextFormatter : ITextFormatter
    {
        private readonly string _hostname;

        public JsonTextFormatter()
        {
            _hostname = Dns.GetHostName();
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            try
            {
                var renderedMessage = logEvent.RenderMessage();

                var message = new
                {
                    RenderedMessage = renderedMessage,
                    logEvent.Properties,
                    Level = logEvent.Level.ToString(),
                    Exception = logEvent.Exception?.ToString(),
                    Hostname = _hostname
                };

                output.Write(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                try
                {
                    var message = new
                    {
                        RenderedMessage = "Failed to render log message.",
                        MessageTemplate = logEvent.MessageTemplate.Text, Exception = exception.ToString()
                    };

                    output.Write(JsonConvert.SerializeObject(message));
                }
                catch (Exception ex)
                {
                    output.Write($"Unable to render log message. Reason was {ex}");
                }
            }
        }
    }
}
