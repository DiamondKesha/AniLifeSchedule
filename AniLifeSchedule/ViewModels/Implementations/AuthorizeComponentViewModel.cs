using AniLifeSchedule.Models;
using AniLifeSchedule.Services;

namespace AniLifeSchedule.ViewModels.Implementations
{
    public class AuthorizeComponentViewModel : IAuthorizeComponentViewModel
    {
        private readonly INotifierService _notifierService;
        private readonly ICookiesService _cookiesService;
        private readonly IVKService _vkService;

        private AuthorizeModel _model = new();
        public AuthorizeModel Model
        {
            get => _model;
            set
            {
                _model = value;
                _notifierService.Notify(this);
            }
        }

        public AuthorizeComponentViewModel(INotifierService notifierService,
                                           ICookiesService cookiesService,
                                           IVKService vkService)
        {
            _notifierService = notifierService;
            _cookiesService = cookiesService;
            _vkService = vkService;

            _ = Authorize();
        }

        private async Task Authorize()
        {
            string token = _cookiesService.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                Model = new()
                {
                    IsAuthorized = false,
                    NameGroup = $"Tokens isn't exists in cookies."
                };

                return;
            }

            var result = await _vkService.GetGroupInformation(token);

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
}
