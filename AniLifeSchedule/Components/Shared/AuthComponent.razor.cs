using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.VK.Group;
using Microsoft.AspNetCore.Components;

namespace AniLifeSchedule.Components.Shared;

public partial class AuthComponent
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IVKService VKService { get; set; } = default!;

    private bool _isAuthorized = false;
    private bool _isInitialize = false;

    private GroupsGetByIdResponse _groupData = default!;

    protected override async Task OnAfterRenderAsync(bool onFirstRenderer)
    {
        if (onFirstRenderer)
        {
            await CheckIfAuthorized();

            _isInitialize = true;
            StateHasChanged();
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

    private void NavigateToAuthPage()
    {
        NavigationManager.NavigateTo("auth");
    }
}