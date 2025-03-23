using AniLifeSchedule.Common.HttpRequestHandlers;
using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Services.Anilist;
using AniLifeSchedule.Services.ImageCreator;
using AniLifeSchedule.Services.Shikimori;
using AniLifeSchedule.Services.VK;
using Blazored.LocalStorage;
using MudBlazor.Services;
using System.Text;

namespace AniLifeSchedule.Common;

public static class DepedencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<ScheduleConfiguration>(configuration.GetSection(nameof(ScheduleConfiguration)));
        services.Configure<ApiUrlsConfiguration>(configuration.GetSection(nameof(ApiUrlsConfiguration)));
        services.Configure<SiteUrlsConfiguration>(configuration.GetSection(nameof(SiteUrlsConfiguration)));
        services.Configure<VKConfiguration>(configuration.GetSection(nameof(VKConfiguration)));

        services.AddScoped<HttpRequestHandler>();
        services.AddScoped<VKHttpRequestHandler>();

        services.AddHttpClient("Base").AddHttpMessageHandler<HttpRequestHandler>();

        services.AddHttpClient("Anilist", client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection($"{nameof(ApiUrlsConfiguration)}:{nameof(ApiUrlsConfiguration.Anilist)}").Value!);
        }).AddHttpMessageHandler<HttpRequestHandler>();

        services.AddHttpClient("Shikimori", client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection($"{nameof(ApiUrlsConfiguration)}:{nameof(ApiUrlsConfiguration.Shikimori)}").Value!);
        }).AddHttpMessageHandler<HttpRequestHandler>();

        services.AddHttpClient("VK", client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection($"{nameof(ApiUrlsConfiguration)}:{nameof(ApiUrlsConfiguration.VK)}").Value!);
        }).AddHttpMessageHandler<VKHttpRequestHandler>();

        services.AddHttpClient("VKOuath", client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection($"{nameof(VKConfiguration)}:{nameof(VKConfiguration.AuthorizeUrl)}").Value!);
        }).AddHttpMessageHandler<HttpRequestHandler>();

        services.AddScoped<IAnilistService, AnilistService>();
        services.AddScoped<IShikimoriService, ShikimoriService>();
        services.AddScoped<IVKService, VKService>();
        services.AddScoped<IVKAuthService, VKAuthService>();
        services.AddScoped<IImageCreatorService, ImageCreatorService>();

        services.AddMudServices();
        services.AddBlazoredLocalStorage();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return services;
    }
}
