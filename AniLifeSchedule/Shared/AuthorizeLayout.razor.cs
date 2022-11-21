namespace AniLifeSchedule.Shared;

public partial class AuthorizeLayout : IDisposable
{
    [Inject] protected INotifierService NotifierService { get; set; } = default!;
    [Inject] protected ICookiesService CookiesService { get; set; } = default!;
    [Inject] protected IVKService VKService { get; set; } = default!;

    private bool isNotify;

    private AuthorizeModel _model = new();
    public AuthorizeModel Model
    {
        get => _model;
        set
        {
            _model = value;
            NotifierService.Notify(this);
        }
    }

    protected override async void OnInitialized()
    {
        isNotify = NotifierService.Subscribe(this, OnNotify);
        await Authorize();
    }

    private async Task OnNotify()
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        NotifierService.Unsubscribe(this);
    }

    private async Task Authorize()
    {
        string token = CookiesService.AccessToken;

        if (string.IsNullOrEmpty(token))
        {
            Model = new()
            {
                IsAuthorized = false,
                NameGroup = $"Tokens isn't exists in cookies."
            };

            return;
        }

        var result = await VKService.GetGroupInformation(token);

        if (!result.Succeeded)
        {
            Model = new()
            {
                IsAuthorized = false,
                NameGroup = result.Messages[0]
            };

            return;
        }

        if (result.Succeeded)
        {
            Model = new()
            {
                IsAuthorized = true,
                NameGroup = result.Data[0].Name,
                ImageUrl = result.Data[0].Photo100,
                MembersCount = result.Data[0].MembersCount
            };
        }
    }
}