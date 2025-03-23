using Polly;

namespace AniLifeSchedule.Common.HttpRequestHandlers;

public class HttpRequestHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var retryPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>().RetryAsync(3);
        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(() => base.SendAsync(request, cancellationToken));

        if (policyResult.Outcome == OutcomeType.Failure)
        {
            Console.WriteLine(policyResult.FinalException);
            return default!;
        }

        return policyResult.Result;
    }
}
