#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 MDrawing.cs is part of SFXLastPosition.

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

#endregion License

#region



#endregion

namespace ElUtilitySuite.Vendor.SFX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    using SharpDX.Direct3D9;
    using EloBuddy;
    internal class MDrawing
    {
        private static readonly Dictionary<int, Font> Fonts = new Dictionary<int, Font>();
        private static readonly HashSet<Line> Lines = new HashSet<Line>();
        private static Sprite _sprite;

        static MDrawing()
        {
            try
            {
                Drawing.OnPreReset += OnDrawingPreReset;
                Drawing.OnPostReset += OnDrawingPostReset;
                //Entry.OnUnload += OnUnload;
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        private static void OnUnload(object sender, Entry.UnloadEventArgs unloadEventArgs)
        {
            try
            {
                if (_sprite != null && !_sprite.IsDisposed)
                {
                    _sprite.Dispose();
                }

                foreach (var font in Fonts.Where(font => font.Value != null && !font.Value.IsDisposed))
                {
                    font.Value.Dispose();
                }

                foreach (var line in Lines.Where(line => line != null && !line.IsDisposed))
                {
                    line.Dispose();
                }

                Drawing.OnPreReset -= OnDrawingPreReset;
                Drawing.OnPostReset -= OnDrawingPostReset;
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }
        private static void OnDrawingPostReset(EventArgs args)
        {
            try
            { 
                if (_sprite != null && !_sprite.IsDisposed)
                {
                    _sprite.OnResetDevice();
                }

                foreach (var font in Fonts.Where(font => font.Value != null && !font.Value.IsDisposed))
                {
                    font.Value.OnResetDevice();
                }

                foreach (var line in Lines.Where(line => line != null && !line.IsDisposed))
                {
                    line.OnResetDevice();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        private static void OnDrawingPreReset(EventArgs args)
        {
            try
            {
                if (_sprite != null && !_sprite.IsDisposed)
                {
                    _sprite.OnLostDevice();
                }

                foreach (var font in Fonts.Where(font => font.Value != null && !font.Value.IsDisposed))
                {
                    font.Value.OnLostDevice();
                }

                foreach (var line in Lines.Where(line => line != null && !line.IsDisposed))
                {
                    line.OnLostDevice();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        public static Font GetFont(int fontSize)
        {
            Font font = null;
            try
            {
                if (!Fonts.TryGetValue(fontSize, out font))
                {
                    font = new Font(
                        Drawing.Direct3DDevice,
                        new FontDescription
                        {
                            FaceName = "Tahoma",
                            Height = fontSize,
                            OutputPrecision = FontPrecision.Default,
                            Quality = FontQuality.Default
                        });
                    Fonts[fontSize] = font;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
            return font;
        }

        public static Line GetLine(int width = -1)
        {
            Line line = null;
            try
            {
                line = new Line(Drawing.Direct3DDevice);
                if (width >= 0)
                {
                    line.Width = width;
                }
                Lines.Add(line);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
            return line;
        }

        public static Sprite GetSprite()
        {
            try
            {
                _sprite = new Sprite(Drawing.Direct3DDevice);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
            return _sprite;
        }
    }
}