﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WFontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class FontImageSourceHandler : IImageSourceHandler, IIconElementHandler
	{
		float _minimumDpi = 300;

		public Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken))
		{
			if (!(imagesource is FontImageSource fontsource))
				return null;

			var device = CanvasDevice.GetSharedDevice();
			var dpi = Math.Max(_minimumDpi, Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi);

			var textFormat = new CanvasTextFormat
			{
				FontFamily = GetFontSource(fontsource),
				FontSize = (float)fontsource.Size,
				HorizontalAlignment = CanvasHorizontalAlignment.Left,
				VerticalAlignment = CanvasVerticalAlignment.Center,
				Options = CanvasDrawTextOptions.Default
			};

			using (var layout = new CanvasTextLayout(device, fontsource.Glyph, textFormat, (float)fontsource.Size, (float)fontsource.Size))
			{
				var canvasWidth = (float)layout.LayoutBounds.Width + 2;
				var canvasHeight = (float)layout.LayoutBounds.Height + 2;

				var imageSource = new CanvasImageSource(device, canvasWidth, canvasHeight, dpi);
				using (var ds = imageSource.CreateDrawingSession(Windows.UI.Colors.Transparent))
				{
					var iconcolor = (fontsource.Color != Color.Default ? fontsource.Color : Color.White).ToWindowsColor();

					// offset by 1 as we added a 1 inset
					ds.DrawTextLayout(layout, 1f, 1f, iconcolor);
				}

				return Task.FromResult((Windows.UI.Xaml.Media.ImageSource)imageSource);
			}
		}

		public Task<Microsoft.UI.Xaml.Controls.IconSource> LoadIconSourceAsync(ImageSource imagesource, CancellationToken cancellationToken = default(CancellationToken))
		{
			Microsoft.UI.Xaml.Controls.IconSource image = null;

			if (imagesource is FontImageSource fontImageSource)
			{
				image = new WFontIconSource
				{
					Glyph = fontImageSource.Glyph,
					FontSize = fontImageSource.Size,
					Foreground = fontImageSource.Color.ToBrush()
				};

				var uwpFontFamily = fontImageSource.FontFamily.ToFontFamily();

				if (!string.IsNullOrEmpty(uwpFontFamily.Source))
					((WFontIconSource)image).FontFamily = uwpFontFamily;
			}

			return Task.FromResult(image);
		}

		public Task<IconElement> LoadIconElementAsync(ImageSource imagesource, CancellationToken cancellationToken = default(CancellationToken))
		{
			IconElement image = null;

			if (imagesource is FontImageSource fontImageSource)
			{
				image = new FontIcon
				{
					Glyph = fontImageSource.Glyph,
					FontSize = fontImageSource.Size,
					Foreground = fontImageSource.Color.ToBrush()
				};

				var uwpFontFamily = fontImageSource.FontFamily.ToFontFamily();

				if (!string.IsNullOrEmpty(uwpFontFamily.Source))
					((FontIcon)image).FontFamily = uwpFontFamily;
			}

			return Task.FromResult(image);
		}

		string GetFontSource(FontImageSource fontImageSource)
		{
			if (fontImageSource == null)
				return string.Empty;

			var fontFamily = fontImageSource.FontFamily.ToFontFamily();

			string fontSource = fontFamily.Source;

			var allFamilies = fontFamily.Source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (allFamilies.Length > 1)
			{       
				// There's really no perfect solution to handle font families with fallbacks (comma-separated)	
				// So if the font family has fallbacks, only one is taken, because CanvasTextFormat	
				// only supports one font family
				string source = fontImageSource.FontFamily;

				foreach(var family in allFamilies)
				{
					if(family.Contains(source))
					{
						fontSource = family;
						break;
					}
				}
			}

			return fontSource;
		}
	}
}
