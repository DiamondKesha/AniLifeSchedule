using AniLifeSchedule.Contracts.Services;
using Polly;

namespace AniLifeSchedule.Common.HttpRequestHandlers;

/// <summary>
/// убрать vkauthhttprequesthandler
/// все методы send post перенести в отедльный метод в vkservice и там проверять токен
/// </summary>
/// <param name="vKAuthService"></param>
public class VKHttpRequestHandler(/*IVKAuthService vKAuthService*/) : DelegatingHandler
{
   // private readonly IVKAuthService _vkAuthService = vKAuthService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //var tokenValidateResult = await _vkAuthService.ValidateToken();

        //if (!tokenValidateResult.IsError && tokenValidateResult.Value)
        //{
            var retryPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>().RetryAsync(3);
            var policyResult = await retryPolicy.ExecuteAndCaptureAsync(() => base.SendAsync(request, cancellationToken));

            if (policyResult.Outcome == OutcomeType.Failure)
            {
                Console.WriteLine(policyResult.FinalException);
                return default!;
            }

            return policyResult.Result;
  //      }

//        return default!;
    }
}