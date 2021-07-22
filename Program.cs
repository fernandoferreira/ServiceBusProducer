using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ServiceBusProducer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                                .CreateLogger();

            if (args.Length < 3)
            {
                logger.Error(
                    "Informe ao menos 3 parametros: " +
                    "no primeiro a string de conexao com o Azure Service Bus, " +
                    "no segundo a Fila/Queue a que recebera as mensagens..." +
                    "ja no terceito em diante as mensagens a serem " +
                    "enviadas a Queue do Azure Service Bus...");
                return;
            }

            var connectionString = args[0];
            var queue = args[1];

            logger.Information($"Fila: {queue}");

            QueueClient client = null;

            try
            {
                client = new QueueClient(connectionString, queue, ReceiveMode.ReceiveAndDelete);

                for (int i = 2; i < args.Length; i++)
                {
                    await client.SendAsync(
                        new Message(Encoding.UTF8.GetBytes(args[i])));

                    logger.Information($"A mensagem '{args[i]}' foi enviada");
                }

            }
            catch (Exception ex)
            {
                logger.Error($"Exceção: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");
            }
            finally
            {
                if (client is not null)
                {
                    await client.CloseAsync();
                    logger.Information("Conexão com Azure Service Bus encerrada.");
                }

            }

        }
    }
}
