using AniLifeSchedule.Data;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Services;
using AniLifeSchedule.Services.Implementations;
using AniLifeSchedule.ViewModels;
using AniLifeSchedule.ViewModels.Implementations;
using Blazored.Toast;
using Polly;
using Polly.Timeout;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

// Thirtparty Services
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddBlazoredToast();

// View Models
builder.Services.AddTransient<IScheduleViewModel, ScheduleViewModel>();
builder.Services.AddTransient<IAuthorizeComponentViewModel, AuthorizeComponentViewModel>();
builder.Services.AddTransient<ICreatePostsViewModel, CreatePostsViewModel>();

// Services
builder.Services.AddScoped<INotifierService, NotifierService>();
builder.Services.AddScoped<ICookiesService, CookiesService>();
builder.Services.AddTransient<IImageCreatorService, ImageCreatorService>();
builder.Services.AddTransient<IAnilistService, AnilistService>();
builder.Services.AddTransient<IShikimoriService, ShikimoriService>();
builder.Services.AddTransient<IVKService, VKService>();

// Data
builder.Services.AddSingleton<ScheduleTransferData>();

// Configurations
builder.Services.Configure<ScheduleConfiguration>(builder.Configuration.GetSection(nameof(ScheduleConfiguration)));
builder.Services.Configure<ApiUrls>(builder.Configuration.GetSection(nameof(ApiUrls)));
builder.Services.Configure<VKConfiguration>(builder.Configuration.GetSection(nameof(VKConfiguration)));

#region Polly

var retryPolicy = Policy
    .HandleResult<HttpResponseMessage>( r => !r.IsSuccessStatusCode)
    .Or<TimeoutRejectedException>()
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15)
    });

//var swallowExceptions = Policy.Handle<Exception>().FallbackAsync(ct => { return Task.CompletedTask; });

var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(5), (ctx, t, task, e) =>
{
    Console.WriteLine(e.Message);
    return Task.CompletedTask;
});

//var policyWrap = Policy.WrapAsync(swallowExceptions, timeoutPolicy);
var policies = timeoutPolicy.WrapAsync(retryPolicy);

builder.Services.AddHttpClient("Anilist", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection($"{nameof(ApiUrls)}:{nameof(ApiUrls.Anilist)}").Value);
}).AddPolicyHandler(policies);

builder.Services.AddHttpClient("Shikimori", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection($"{nameof(ApiUrls)}:{nameof(ApiUrls.Shikimori)}").Value);
}).AddPolicyHandler(policies);

builder.Services.AddHttpClient("VK", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection($"{nameof(ApiUrls)}:{nameof(ApiUrls.VK)}").Value);
}).AddPolicyHandler(policies);

builder.Services.AddHttpClient("Base").AddPolicyHandler(policies);

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsAllowAll",
        builder =>
        {
            builder.AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .WithMethods("GET, PATCH, DELETE, PUT, POST, OPTIONS");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("CorsAllowAll");

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
