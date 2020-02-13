﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XFColorPickerControl
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
	{
        private SKPoint _lastTouchPoint = new SKPoint();

		public MainPage()
        {
            InitializeComponent();
        }

		private void SkCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			var skImageInfo = e.Info;
			var skSurface = e.Surface;
			var skCanvas = skSurface.Canvas;

			var skCanvasWidth = skImageInfo.Width;
			var skCanvasHeight = skImageInfo.Height;

			skCanvas.Clear(SKColors.White);

			// Draw colorful gradient spectrum
			using (var paint = new SKPaint())
			{
				paint.IsAntialias = true;

				// Initiate the darkened primary color list
				// picked from Google search "online color picker"
				var colors = new SKColor[]
				{
                    new SKColor(255, 0, 0),
                    new SKColor(255, 255, 0),
                    new SKColor(0, 255, 0),
                    new SKColor(0, 255, 255),
                    new SKColor(0, 0, 255),
                    new SKColor(255, 0, 255),
                    new SKColor(255, 0, 0),
				};

				// create the gradient shader 
				using (var shader = SKShader.CreateLinearGradient(
					new SKPoint(0, 0),
					new SKPoint(skCanvasWidth, 0),
					colors,
					null,
					SKShaderTileMode.Clamp))
				{
					paint.Shader = shader;
					skCanvas.DrawPaint(paint);
				}
			}

			// Draw darker gradient spectrum
			using (var paint = new SKPaint())
			{
				paint.IsAntialias = true;

				// Initiate the darkened primary color list
				var colors = new SKColor[]
				{
					SKColors.Transparent,
					SKColors.Black
				};

				// create the gradient shader 
				using (var shader = SKShader.CreateLinearGradient(
					new SKPoint(0, 0),
					new SKPoint(0, skCanvasHeight),
					colors,
					null,
					SKShaderTileMode.Clamp))
				{
					paint.Shader = shader;
					skCanvas.DrawPaint(paint);
				}
			}

			// retrieve the color of the current Touch point
            SKColor touchPointColor;

			// Inefficient : causes memory overload errors
			//using (var skImage = skSurface.Snapshot())
			//{
			//	using (var skData = skImage.Encode(SKEncodedImageFormat.Webp, 100))
			//	{
			//                 if (skData != null)
			//                 {
			//		    using (SKBitmap bitmap = SKBitmap.Decode(skData))
			//		    {
			//			    touchPointColor = bitmap.GetPixel((int)_lastTouchPoint.X, (int)_lastTouchPoint.Y);
			//                     }
			//                 }
			//	}
			//}


			// this is more efficent
			// https://forums.xamarin.com/discussion/92899/read-a-pixel-info-from-a-canvas
			// create the 1x1 bitmap (auto allocates the pixel buffer)
			SKBitmap bitmap = new SKBitmap(skImageInfo);

            // get the pixel buffer for the bitmap
            IntPtr dstpixels = bitmap.GetPixels();

			// read the surface into the bitmap
            skSurface.ReadPixels(skImageInfo, dstpixels, skImageInfo.RowBytes, (int)_lastTouchPoint.X, (int)_lastTouchPoint.Y);

			// access the color
            touchPointColor = bitmap.GetPixel(0, 0);



			// painting the Touch point
			using (SKPaint paintTouchPoint = new SKPaint())
			{
				paintTouchPoint.Style = SKPaintStyle.Fill;
				paintTouchPoint.Color = SKColors.White;
				paintTouchPoint.IsAntialias = true;

				skCanvas.DrawCircle(
					_lastTouchPoint.X,
					_lastTouchPoint.Y,
					30, paintTouchPoint);

				paintTouchPoint.Color = touchPointColor;

				skCanvas.DrawCircle(
					_lastTouchPoint.X,
					_lastTouchPoint.Y,
					20, paintTouchPoint);
			}

			// Set selected color
            var colorInXamarinForms = touchPointColor.ToFormsColor();
			
			// set page background color
            this.BackgroundColor = colorInXamarinForms;
        }

		private void SkCanvasView_OnTouch(object sender, SKTouchEventArgs e)
        {
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed)
            {
                _lastTouchPoint = e.Location;
                e.Handled = true;
            }

            _lastTouchPoint = e.Location;

			// update the Canvas as you wish
			SkCanvasView.InvalidateSurface();
		}
	}
}