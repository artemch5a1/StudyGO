using System.Runtime.CompilerServices;
using System.Threading.Channels;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Contracts;

namespace StudyGO.infrastructure.Extensions.VerificationJobQueue;

public class InMemoryVerificationJobQueue : IVerificationJobQueue
{
    private readonly Channel<VerificationJob> _channel;

    public InMemoryVerificationJobQueue(Channel<VerificationJob> channel)
    {
        _channel = channel;
    }
    
    public ValueTask EnqueueAsync(VerificationJob job, CancellationToken cancellationToken)
    {
        return _channel.Writer.WriteAsync(job, cancellationToken);
    }

    public async IAsyncEnumerable<VerificationJob> DequeueAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return job;
        }
    }
}