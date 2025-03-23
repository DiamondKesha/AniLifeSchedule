using AniLifeSchedule.Common;
using AniLifeSchedule.Common.Base;
using AniLifeSchedule.Common.Extensions;
using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Models.VK.Group;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace AniLifeSchedule.Components.Pages;

public partial class Auth
{
    [Inject] protected IOptions<VKConfiguration> VKConfiguration { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IVKService VKService { get; set; } = default!;
    [Inject] protected IVKAuthService VKAuthService { get; set; } = default!;
    [Inject] protected ILocalStorageService LocalStorageService { get; set; } = default!;

    private bool _isAuthorized = false;
    private bool _isParametersProvided = false;
    private bool _isInitialize = false;

    private GroupsGetByIdResponse _groupData = default!;

    private string _code { get; set; } = string.Empty;
    private string _state { get; set; } = string.Empty;
    private string _type { get; set; } = string.Empty;
    private string _deviceId { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool onFirstRenderer)
    {
        if (onFirstRenderer)
        {
            await CheckIfAuthorized();
            await CheckParameters();

            _isInitialize = true;
            StateHasChanged();
        }
    }

    public async Task OnButtonAuthorizePressed()
    {
        var (challenge, verifier) = Pkce.Generate();

        await LocalStorageService.SetItemAsStringAsync(Constants.LOCALSTORAGE_PKCE_VERIFIER, verifier);

        NavigateTo(
            VKConfiguration.Value.AuthorizeUrl + "authorize?"
            + $"client_id={VKConfiguration.Value.ClientId}"
            + $"&redirect_uri={VKConfiguration.Value.RedirectUrl}"
            + $"&scope={VKConfiguration.Value.Scope}"
            + $"&response_type={VKConfiguration.Value.ResponseType}"
            + $"&state={Guid.NewGuid().ToString()[..12]}"
            + $"&code_challenge_method=S256"
            + $"&code_challenge={challenge}");
    }

    private async Task CheckParameters()
    {
        var query = NavigationManager.QueryString();

        _code = query["code"]!;
        _state = query["state"]!;
        _type = query["type"]!;
        _deviceId = query["device_id"]!;

        if (!string.IsNullOrWhiteSpace(_code)
            && !string.IsNullOrWhiteSpace(_state)
            && !string.IsNullOrWhiteSpace(_type)
            && !string.IsNullOrWhiteSpace(_deviceId))
        {
            _isParametersProvided = true;

            var accessTokenResult = await VKAuthService.Authorize(_code, _deviceId);

            if (accessTokenResult.IsError)
            {
                await CheckIfAuthorized();
                return;
            }

            NavigateTo("/");
        }
    }

    private async Task CheckIfAuthorized()
    {
        var groupInfo = await VKService.GetGroupInformation();

        _groupData = groupInfo.Match(
            value =>
            {
                if (value.Count <= 0) _isAuthorized = false;

                _isAuthorized = true;
                return value[0];
            },
            errors =>
            {
                _isAuthorized = false;
                return default!;
            }
         );
    }

    private async Task Logout()
    {
        await VKAuthService.Logout();

        _isAuthorized = false;
        _isParametersProvided = false;

        StateHasChanged();
    }

    private void NavigateTo(string url = "")
    {
        if (string.IsNullOrWhiteSpace(url))
            NavigationManager.NavigateTo(NavigationManager.BaseUri);
        else
            NavigationManager.NavigateTo(url);
    }
}