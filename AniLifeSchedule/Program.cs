using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Services;
using AniLifeSchedule.Services.Implementations;
using AniLifeSchedule.ViewModels;
using AniLifeSchedule.ViewModels.Implementations;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<IScheduleViewModel, ScheduleViewModel>();
builder.Services.AddTransient<IImageCreatorService, ImageCreatorService>();
builder.Services.AddTransient<IAnilistService, AnilistService>();
builder.Services.AddTransient<IShikimoriService, ShikimoriService>();

// Configurations
builder.Services.Configure<ScheduleImage>(builder.Configuration.GetSection(nameof(ScheduleImage)));
builder.Services.Configure<ApiUrls>(builder.Configuration.GetSection(nameof(ApiUrls)));

#region Polly

var retryPolicy = Policy
    .HandleResult<HttpResponseMessage>( r => !r.IsSuccessStatusCode)
    .Or<TimeoutRejectedException>()
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    });

var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(3), (ctx, t, task, e) =>
{
    Console.WriteLine(e.Message);
    return Task.CompletedTask;
});

var policies = retryPolicy.WrapAsync(timeoutPolicy);


builder.Services.AddHttpClient("Anilist", client =>
{
    client.BaseAddress = new Uri("https://graphql.anilist.co/");
}).AddPolicyHandler(policies);

builder.Services.AddHttpClient("Shikimori", client =>
{
    client.BaseAddress = new Uri("https://shikimori.one/api/");
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
