using System.Collections.Concurrent;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using StudyGO.Contracts.CustomExceptions;
using StudyGO.infrastructure.EmailServices;
using StudyGO.infrastructure.SmtpClientFactory.SmtpClient;

namespace StudyGO.infrastructure.SmtpClient;

public class SmtpClientPool : IDisposable, ISmtpSender
{
    private readonly ConcurrentBag<ISmtpClient> _clientsPool = new();

    private EmailServiceOptions _options;

    private readonly ILogger<SmtpClientPool> _logger;

    private readonly ISmtpClientFactory _factory;
    
    private readonly SmtpClientOptions _poolOptions;
    
    private readonly IOptionsMonitor<EmailServiceOptions> _optionsMonitor;
    
    private readonly object _reloadLock = new();

    public SmtpClientPool(
        ISmtpClientFactory factory,
        IOptionsMonitor<EmailServiceOptions> options, 
        ILogger<SmtpClientPool> logger, 
        IOptions<SmtpClientOptions> poolOptions)
    {
        _factory = factory;
        _optionsMonitor = options;
        _options = options.CurrentValue;
        _logger = logger;
        _poolOptions = poolOptions.Value;
        
        _optionsMonitor.OnChange(o =>
        {
            _logger.LogWarning("������������ SMTP ��������� �����");
            _options = o;
            ClearPool();
        });
    }

    public async Task SendAsync(MimeMessage message, CancellationToken ct = default)
    {
        _logger.LogDebug("��������� ������� �� ����");
        
        var client = GetClient();
        
        try
        {
            _logger.LogDebug("�������� � ��������� ����������� ����������� � �������������� ��� �������������");

            client = await EnsureConnectedAndAuthenticatedAsync(client, ct);

            _logger.LogDebug("�������� ���������...");

            await client.SendAsync(message, ct);

            _logger.LogDebug("������� ������� � ���");
            
            ReturnClient(client);
        }
        catch (SmtpConfigurationException ex)
        {
            _logger.LogError(ex, "������ ������������ SMTP. ��������� ������������� ������ � ����������� ��������");
            
            ReloadConfiguration();
            
            var newClient = await EnsureConnectedAndAuthenticatedAsync(BasicImplementation(), ct);
            await newClient.SendAsync(message, ct);
            ReturnClient(newClient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� �������� ���������");
            throw;
        }
    }

    private ISmtpClient GetClient()
    {
        if (_clientsPool.TryTake(out var client))
        {
            _logger.LogInformation("������� smtp ������ �� ����, ���������� ��������� ��������: {count}", _clientsPool.Count);
            return client;
        }
        
        return BasicImplementation();
    }

    private void ReturnClient(ISmtpClient client)
    {
        if (_clientsPool.Count < _poolOptions.PoolSize)
        {
            _clientsPool.Add(client);
            _logger.LogInformation("Smtp ������ ��������� � ���");
        }
        else
        {
            _logger.LogInformation("�������� ���������� ������ ����, ����������� �������");
            client.Disconnect(true);
            client.Dispose();
        }
    }

    private ISmtpClient BasicImplementation()
    {
        _logger.LogInformation("������ ����� smtp ������");
        
        return _factory.CreateClient();
    }
    
    private void ReloadConfiguration()
    {
        lock (_reloadLock)
        {
            ClearPool();

            // ���������� ������� �����
            _options = _optionsMonitor.CurrentValue;

            _logger.LogInformation("������������ SMTP �������������");
        }
    }

    private void ClearPool()
    {
        while (_clientsPool.TryTake(out var client))
        {
            try
            {
                if (client.IsConnected) client.Disconnect(true);
                client.Dispose();
            }
            catch { }
        }

        _logger.LogInformation("��� �������� ������");
    }
    
    private async Task<ISmtpClient> EnsureConnectedAndAuthenticatedAsync(ISmtpClient client, CancellationToken ct)
    {
        try
        {
            if (client.IsConnected)
            {
                try
                {
                    await client.NoOpAsync(ct);
                }
                catch
                {
                    await client.DisconnectAsync(true, ct);
                }
            }

            if (!client.IsConnected)
            {
                await client.ConnectAsync(_options.SmtpServer, _options.Port, true, ct);
            }

            if (!client.IsAuthenticated)
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, ct);
            }

            return client;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Smtp ������ �������� � ���������� ���������, ������������...");
            
            try { await client.DisconnectAsync(true, ct); } catch {}
            client.Dispose();
            
            var newClient = _factory.CreateClient();
            
            try
            {
                await newClient.ConnectAsync(_options.SmtpServer, _options.Port, true, ct);
                await newClient.AuthenticateAsync(_options.Username, _options.Password, ct);

                return newClient;
            }
            catch (Exception ex2)
            {
                throw new SmtpConfigurationException("������ ������������ SMTP", ex2);
            }
        }
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