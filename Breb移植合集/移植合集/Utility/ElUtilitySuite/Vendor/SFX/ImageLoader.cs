#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 ImageLoader.cs is part of SFXLastPosition.

 SFXLastPosition is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 SFXLastPosition is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with SFXLastPosition. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

namespace ElUtilitySuite.Vendor.SFX
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;

    using PortAIO.Properties;

    using LeagueSharp;
    using EloBuddy;
    internal class ImageLoader
    {
        #region Static Fields

        /// <summary>
        ///     The base directory
        /// </summary>
        public static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        ///     The Cache directory
        /// </summary>
        public static string CacheDir = Path.Combine(BaseDir, "ElUtilitySuite" + " - Cache");

        #endregion

        #region Public Methods and Operators

        public static Bitmap Load(string uniqueId, string name)
        {
            try
            {
                uniqueId = uniqueId.ToUpper();
                var cachePath = GetCachePath(uniqueId, name);
                if (File.Exists(cachePath))
                {
                    return new Bitmap(cachePath);
                }
                var bitmap = Resources.ResourceManager.GetObject(name) as Bitmap;
                if (bitmap != null)
                {
                    switch (uniqueId)
                    {
                        case "LP":
                            bitmap = CreateLastPositionImage(bitmap);
                            break;
                    }
                    if (bitmap != null)
                    {
                        bitmap.Save(cachePath);
                    }
                }
                return bitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
            return null;
        }

        #endregion

        #region Methods

        private static Bitmap CreateLastPositionImage(Bitmap source)
        {
            try
            {
                var img = new Bitmap(source.Width, source.Width);
                var cropRect = new Rectangle(0, 0, source.Width, source.Width);

                using (var sourceImage = source)
                {
                    using (var croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                    {
                        using (var tb = new TextureBrush(croppedImage))
                        {
                            using (var g = Graphics.FromImage(img))
                            {
                                g.FillEllipse(tb, 0, 0, source.Width, source.Width);
                                g.DrawEllipse(
                                    new Pen(Color.FromArgb(86, 86, 86), 6) { Alignment = PenAlignment.Inset },
                                    0,
                                    0,
                                    source.Width,
                                    source.Width);
                            }
                        }
                    }
                }
                return img.Resize(24, 24).Grayscale();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
            return null;
        }

        private static string GetCachePath(string uniqueId, string name)
        {
            try
            {
                if (!Directory.Exists(CacheDir))
                {
                    Directory.CreateDirectory(CacheDir);
                }
                string path = Path.Combine(CacheDir, string.Format("{0}", Game.Version.Substring(0, 19)));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, uniqueId);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return Path.Combine(path, string.Format("{0}.png", name));
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
            return null;
        }

        #endregion
    }
}