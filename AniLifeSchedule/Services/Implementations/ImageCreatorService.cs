using AniLifeSchedule.Enums;
using AniLifeSchedule.Models;
using AniLifeSchedule.Models.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SkiaSharp;
using Topten.RichTextKit;

namespace AniLifeSchedule.Services.Implementations
{
    public class ImageCreatorService : IImageCreatorService
    {
        #region Styles 

        /// <summary>
        /// Uses in top screen text (like "Расписание на 12.12.2022", "Schedule to 12.12.2022")
        /// </summary>
        private readonly Style _titleStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 80.56f,
            TextColor = SKColors.White,
            LineHeight = 0.65f
        };

        /// <summary>
        /// Uses in bottom screen text information
        /// </summary>
        private readonly Style _infoStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 41.67f,
            TextColor = SKColors.White,
            LineHeight = 0.8f
        };

        /// <summary>
        /// Uses in anime title text
        /// </summary>
        private readonly Style _animeTitleStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 34.72f,
            TextColor = SKColors.White,
            LetterSpacing = 1,
            LineHeight = 0.8f
        };

        /// <summary>
        /// Uses in status
        /// </summary>
        private readonly Style _animeReleaseStatusStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 38.89f,
            TextColor = SKColor.Parse("#f44336")
        };

        /// <summary>
        /// Uses in status
        /// </summary>
        private readonly Style _animeOngoingStatusStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 38.89f,
            TextColor = SKColor.Parse("#62ec2c")
        };

        /// <summary>
        /// Uses in status
        /// </summary>
        private readonly Style _animePremierStatusStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 38.89f,
            TextColor = SKColor.Parse("f110af")
        };

        /// <summary>
        /// Uses in status
        /// </summary>
        private readonly Style _animePreviewStatusStyle = new Style()
        {
            FontFamily = "Russo One",
            FontSize = 38.89f,
            TextColor = SKColor.Parse("f110af")
        };

        #endregion

        private int heightOutputImage;
        private int widthOutputImage;

        private readonly NavigationManager _hostEnv;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ScheduleImage _scheduleImageConfiguration;

        private HttpClient httpClient;

        public ImageCreatorService(IHttpClientFactory httpClientFactory, IOptions<ScheduleImage> scheduleImageConfiguration, NavigationManager hostEnv)
        {
            _hostEnv = hostEnv;
            _httpClientFactory = httpClientFactory;
            _scheduleImageConfiguration = scheduleImageConfiguration.Value;

            httpClient = _httpClientFactory.CreateClient("Base");
        }

        public async Task<string> Create(List<ScheduleModel> scheduleModel)
        {
            if (scheduleModel.Count == 0) return string.Empty;

            using SKBitmap bitmap = CreateBitmap(scheduleModel.Count);
            using SKCanvas canvas = new (bitmap);
            using SKPaint paint = new();

            await DrawMainInformation(canvas, paint, scheduleModel[0].ReleaseDate.ToShortDateString());
            await DrawScheduleData(canvas, paint, scheduleModel);

            using SKImage outputImage = SKImage.FromBitmap(bitmap);
            using SKData bitmapData = outputImage.Encode(SKEncodedImageFormat.Jpeg, 80);

            if (_scheduleImageConfiguration.IsSave)
            {
                using var fileStream = File.OpenWrite($"{_scheduleImageConfiguration.PathToSave}\\Schedule to {scheduleModel[0].ReleaseDate.ToShortDateString()}.jpg");
                bitmapData.SaveTo(fileStream);
            }

            return Convert.ToBase64String(bitmapData.ToArray());
        }

        /// <summary>
        /// Creates new bitmap
        /// </summary>
        /// <param name="dataCount">Count of anime airing items</param>
        /// <returns>SKBitmap object</returns>
        private SKBitmap CreateBitmap(int dataCount)
        {
            heightOutputImage = (640 * (int)Math.Ceiling((decimal)dataCount / 5)) + 320;
            widthOutputImage = 2300;

            return new SKBitmap(widthOutputImage, heightOutputImage);
        }

        /// <summary>
        /// Draws main informations (like background color, titles, placeholder by) on canvas
        /// </summary>
        /// <param name="canvas">Canvas object which will be drawing</param>
        /// <param name="paint">Paint object</param>
        /// <param name="shortAirTime">AirTime</param>
        private async Task DrawMainInformation(SKCanvas canvas, SKPaint paint, string shortAirTime)
        {
            SKRect backgroundRect = new(0, 0, widthOutputImage, heightOutputImage);
            paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(backgroundRect.MidX, backgroundRect.Top),
                    new SKPoint(backgroundRect.MidX, backgroundRect.Bottom),
                    new SKColor[] { SKColor.Parse("#232526") },
                    new float[] { 0 },
                    SKShaderTileMode.Repeat);
            canvas.DrawRect(backgroundRect, paint);
            paint.Reset();

            using SKImage copyrightImage = await GetImageFromUrl($"{_hostEnv.BaseUri}img/AniLifeLink.png");
            ResizeAndDrawImage(canvas, copyrightImage, paint, 290, 90, new SKPoint(1967, 27));

            DrawTextBlock(canvas, $"РАСПИСАНИЕ НА {shortAirTime}", _titleStyle, TextAlignment.Left, new SKPoint(40, 42.5f), widthOutputImage, 1);
            DrawTextBlock(canvas, @"Время указано по московскому часовому поясу выхода в Японии.
Некоторые аниме - сериалы, фильмы, OVA, ONA и т.п (в основном китайские и корейские)
могут отсутствовать.",
                         _infoStyle, TextAlignment.Center, new SKPoint(0, heightOutputImage - 150), widthOutputImage, 3);
        }

        /// <summary>
        /// Draws schedule data
        /// </summary>
        /// <param name="canvas">Canvas object which will be drawing</param>
        /// <param name="paint">Paint object</param>
        /// <param name="data">Data model with type ScheduleModel</param>
        /// <returns></returns>
        private async Task DrawScheduleData(SKCanvas canvas, SKPaint paint, List<ScheduleModel> data)
        {
            int tempIndex = 0;
            float horizontalCoverPosition = 0,
                  verticalCoverPosition = 150;

            string airTimeAndEpisode;
            SKRect statusRectPoint;
            SKPoint statusTextPoint;

            for (int y = 0; y < Math.Ceiling((decimal)data.Count / 5) + 1; y++)
            {
                for (int i = tempIndex; i < tempIndex + 5; i++)
                {
                    if (i >= data.Count) break;

                    SKImage coverImage = await GetImageFromUrl((data[i].ImageUrl));
                    ResizeAndDrawImage(canvas, coverImage, paint, 460, 640, new SKPoint(horizontalCoverPosition, verticalCoverPosition));

                    paint.Reset();
                    paint.Color = new SKColor(35, 37, 38, 140);

                    statusRectPoint = new SKRect(horizontalCoverPosition + 20, verticalCoverPosition + 10, horizontalCoverPosition, verticalCoverPosition + 64);
                    statusTextPoint = new(horizontalCoverPosition + 29, verticalCoverPosition + 15);

                    canvas.DrawRect(new SKRect(horizontalCoverPosition, verticalCoverPosition + 440, horizontalCoverPosition + 460, verticalCoverPosition + 640), paint);

                    DrawTextBlock(canvas,
                        !string.IsNullOrEmpty(data[i].TitleRussian) ? data[i].TitleRussian : data[i].TitleRomaji,
                        _animeTitleStyle,
                        TextAlignment.Center, new SKPoint(horizontalCoverPosition, verticalCoverPosition + 450), 460, 4);

                    if (data[i].Status == Status.NOT_YET_RELEASED)
                    {
                        statusRectPoint.Right += 229;
                        
                        canvas.DrawRect(statusRectPoint, paint);
                        DrawTextBlock(canvas, "ОНГОИНГ", _animeOngoingStatusStyle, TextAlignment.Left, statusTextPoint, 220);
                    }
                    else if (data[i].Status == Status.FINISHED)
                    {
                        statusRectPoint.Right += 173;

                        canvas.DrawRect(statusRectPoint, paint);
                        DrawTextBlock(canvas, "РЕЛИЗ", _animeReleaseStatusStyle, TextAlignment.Left, statusTextPoint, 160);
                    }
                    else if (data[i].Status == Status.PREVIEW)
                    {
                        statusRectPoint.Right += 286;

                        canvas.DrawRect(statusRectPoint, paint);
                        DrawTextBlock(canvas, "ПРЕДПОКАЗ", _animePreviewStatusStyle, TextAlignment.Left, statusTextPoint, 250);
                    }

                    airTimeAndEpisode = (data[i].Format == Format.MOVIE ? "ФИЛЬМ" : $"{data[i].CurrentEpisode} / " + (data[i].Episodes.ToString() != string.Empty ? data[i].Episodes.ToString() : "?")) + $" - {data[i].ReleaseDate.ToShortTimeString()}";
                    DrawTextBlock(canvas, airTimeAndEpisode, _animeTitleStyle, TextAlignment.Center, new SKPoint(horizontalCoverPosition, verticalCoverPosition + 600), 460);

                    horizontalCoverPosition += 460;
                }
                horizontalCoverPosition = 0;
                verticalCoverPosition += 640;
                tempIndex += 5;
            }
        }

        /// <summary>
        /// Resizing image and draw it on canvas
        /// </summary>
        /// <param name="canvas">Canvas object which will be drawing image</param>
        /// <param name="image">Image object</param>
        /// <param name="paint">Paint object</param>
        /// <param name="resizedWidth">New image width</param>
        /// <param name="resizedHeight">New image height</param>
        /// <param name="position">Image position</param>
        private void ResizeAndDrawImage(SKCanvas canvas, SKImage image, SKPaint paint, int resizedWidth, int resizedHeight, SKPoint position)
        {
            if (image == null) return;

            paint.Reset();
            paint.IsAntialias = true;
            paint.FilterQuality = SKFilterQuality.High;

            using var surface = SKSurface.Create(new SKImageInfo
            {
                Width = resizedWidth,
                Height = resizedHeight,
                ColorType = SKImageInfo.PlatformColorType,
                AlphaType = SKAlphaType.Premul
            });

            surface.Canvas.DrawImage(image, new SKRectI(0, 0, resizedWidth, resizedHeight), paint);
            surface.Canvas.Flush();

            using var newImage = surface.Snapshot();
            canvas.DrawImage(newImage, position, null);

        }

        /// <summary>
        /// Draws texts on canvas
        /// </summary>
        /// <param name="canvas">Canvas object which will be drawing text</param>
        /// <param name="text">Text which will be drawing</param>
        /// <param name="textStyle">Text style</param>
        /// <param name="textAlignment">Text aligment</param>
        /// <param name="point">Text position</param>
        /// <param name="maxWidth">Max width</param>
        /// <param name="maxLines">Max lines</param>
        private void DrawTextBlock(SKCanvas canvas, string text, Style textStyle, TextAlignment textAlignment, SKPoint point, float maxWidth, int maxLines = 1)
        {
            TextBlock textBlock = new() { Alignment = textAlignment, MaxWidth = maxWidth, MaxLines = maxLines };
            
            textBlock.AddText(text, textStyle);
            textBlock.Paint(canvas, point);
        }

        /// <summary>
        /// Getting image from url
        /// </summary>
        /// <param name="url">Url from will be getting image</param>
        /// <returns>SKImage object</returns>
        private async Task<SKImage> GetImageFromUrl(string url)
        {
            var data = await httpClient.GetStreamAsync(url);

            if (data != null) return SKImage.FromEncodedData(data);

            return SKImage.FromEncodedData(await httpClient.GetStreamAsync($"img/placeholder.png"));
        }
    }
}
