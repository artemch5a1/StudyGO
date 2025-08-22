using System.Collections.Concurrent;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using StudyGO.infrastructure.EmailServices;
using StudyGO.infrastructure.SmtpClientFactory.SmtpClient;

namespace StudyGO.infrastructure.SmtpClient;

public class SmtpClientPool : IDisposable, ISmtpSender
{
    private readonly ConcurrentBag<ISmtpClient> _clientsPool = new();

    private readonly EmailServiceOptions _options;

    private readonly ILogger<SmtpClientPool> _logger;

    private readonly ISmtpClientFactory _factory;
    
    private readonly SmtpClientOptions _poolOptions;

    public SmtpClientPool(
        ISmtpClientFactory factory,
        IOptions<EmailServiceOptions> options, 
        ILogger<SmtpClientPool> logger, 
        IOptions<SmtpClientOptions> poolOptions)
    {
        _factory = factory;
        _options = options.Value;
        _logger = logger;
        _poolOptions = poolOptions.Value;
    }

    public async Task SendAsync(MimeMessage message, CancellationToken ct = default)
    {
        _logger.LogDebug("получение клиента из пула");
        
        var client = GetClient();
        
        _logger.LogDebug("Проверка и повторное прохождение подключения и аутентификации при необходимости");
        
        if (!client.IsConnected)
            await client.ConnectAsync(_options.SmtpServer, _options.Port, true, ct);

        if (!client.IsAuthenticated)
            await client.AuthenticateAsync(_options.Username, _options.Password, ct);
        
        _logger.LogDebug("Отправка сообщения...");
        
        await client.SendAsync(message, ct);
        
        _logger.LogDebug("Возврат клиента в пул");
        
        ReturnClient(client);
    }

    private ISmtpClient GetClient()
    {
        if (_clientsPool.TryTake(out var client))
        {
            _logger.LogInformation("Получен smtp клиент из пула, количество свободных пулов: {count}", _clientsPool.Count);
            return client;
        }
        
        return BasicImplementation();
    }

    private void ReturnClient(ISmtpClient client)
    {
        if(_clientsPool.Count < _poolOptions.PoolSize)
            _clientsPool.Add(client);
        else
        {
            _logger.LogInformation("Превышен допустимый размер пула, уничтожение клиента");
            client.Disconnect(true);
            client.Dispose();
        }
    }

    private ISmtpClient BasicImplementation()
    {
        _logger.LogInformation("Создан новый smtp клиент");
        
        return _factory.CreateClient();
    }

    public void Dispose()
    {
        while (_clientsPool.TryTake(out var client))
        {
            if (client.IsConnected)
                client.Disconnect(true);
            client.Dispose();
        }
    }
}