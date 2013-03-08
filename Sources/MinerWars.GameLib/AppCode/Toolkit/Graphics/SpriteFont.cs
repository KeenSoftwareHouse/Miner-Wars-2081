/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX.IO;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Represents a font texture.
    /// </summary>
    public sealed class SpriteFont : 
    {
        private readonly float globalBaseOffsetY;
        private readonly Dictionary<char, int> characterMap;
        private readonly Dictionary<int, float> kerningMap;
        private readonly SpriteFontData.Glyph[] glyphs;
        private Texture2D[] textures;

        // Lookup table indicates which way to move along each axis per SpriteEffects enum value.
        private static readonly Vector2[] axisDirectionTable = new[]
                                                                   {
                                                                       new Vector2(-1, -1),
                                                                       new Vector2(1, -1),
                                                                       new Vector2(-1, 1),
                                                                       new Vector2(1, 1),
                                                                   };

        // Lookup table indicates which axes are mirrored for each SpriteEffects enum value.
        private static readonly Vector2[] axisIsMirroredTable = new[]
                                                                    {
                                                                        new Vector2(0, 0),
                                                                        new Vector2(1, 0),
                                                                        new Vector2(0, 1),
                                                                        new Vector2(1, 1),
                                                                    };

        public static SpriteFont New(GraphicsDevice device, SpriteFontData spriteFontData)
        {
            return new SpriteFont(device, spriteFontData);
        }


        /// <summary>
        /// Loads an <see cref="EffectData"/> from the specified stream.
        /// </summary>
        /// <param name="device">The graphics device</param>
        /// <param name="stream">The stream.</param>
        /// <param name="bitmapDataLoader">A delegate to load bitmap data that are not stored in the buffer.</param>
        /// <returns>An <see cref="EffectData"/>. Null if the stream is not a serialized <see cref="EffectData"/>.</returns>
        /// <remarks>
        /// </remarks>
        public static SpriteFont Load(GraphicsDevice device, Stream stream, SpriteFontBitmapDataLoaderDelegate bitmapDataLoader = null)
        {
            var spriteFontData = SpriteFontData.Load(stream, bitmapDataLoader);
            if (spriteFontData == null)
                return null;
            return New(device, spriteFontData);
        }

        /// <summary>
        /// Loads an <see cref="EffectData"/> from the specified buffer.
        /// </summary>
        /// <param name="device">The graphics device</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bitmapDataLoader">A delegate to load bitmap data that are not stored in the buffer.</param>
        /// <returns>An <see cref="EffectData"/> </returns>
        public static SpriteFont Load(GraphicsDevice device, byte[] buffer, SpriteFontBitmapDataLoaderDelegate bitmapDataLoader = null)
        {
            return Load(device, new MemoryStream(buffer), bitmapDataLoader);
        }

        /// <summary>
        /// Loads an <see cref="EffectData"/> from the specified file.
        /// </summary>
        /// <param name="device">The graphics device</param>
        /// <param name="fileName">The filename.</param>
        /// <returns>An <see cref="EffectData"/> </returns>
        public static SpriteFont Load(GraphicsDevice device, string fileName)
        {
            var fileDirectory = Path.GetDirectoryName(fileName);
            using (var stream = new NativeFileStream(fileName, NativeFileMode.Open, NativeFileAccess.Read))
                return Load(device, stream, bitmapName => Texture2D.Load(device, Path.Combine(fileDirectory, bitmapName)));
        }
       
        internal SpriteFont(GraphicsDevice device, SpriteFontData spriteFontData)
            : base(device)
        {
            // Read the glyph data.
            globalBaseOffsetY = spriteFontData.BaseOffset;
            glyphs = spriteFontData.Glyphs;
            characterMap = new Dictionary<char, int>(glyphs.Length * 2);

            // Prebuild the character map
            var characterList = new List<char>(glyphs.Length);
            for (int i = 0; i < glyphs.Length; i++)
            {
                var charItem = (char)glyphs[i].Character;
                characterMap.Add(charItem, i);
                characterList.Add(charItem);
            }

            // Prepare kernings if they are available.
            var kernings = spriteFontData.Kernings;
            if (kernings != null)
            {
                kerningMap = new Dictionary<int, float>(spriteFontData.Kernings.Length);
                for (int i = 0; i < kernings.Length; i++)
                {
                    int key = (kernings[i].First << 16) | kernings[i].Second;
                    kerningMap.Add(key, kernings[i].Offset);
                }
            }

            Characters = new ReadOnlyCollection<char>(characterList);

            // Read font properties.
            LineSpacing = spriteFontData.LineSpacing;

            DefaultCharacter = (char)spriteFontData.DefaultCharacter;

            // Read the texture data.
            textures = new Texture2D[spriteFontData.Bitmaps.Length];
            for(int i = 0; i < textures.Length; i++)
            {
                var bitmap = spriteFontData.Bitmaps[i];
                if (bitmap.Data is SpriteFontData.BitmapData)
                {
                    var image = (SpriteFontData.BitmapData) bitmap.Data;
                    textures[i] = ToDispose(Texture2D.New(device, image.Width, image.Height, image.PixelFormat, image.Data));
                }
                else if (bitmap.Data is Texture2D)
                {
                    textures[i] = (Texture2D) bitmap.Data;
                }
                else
                {
                    throw new NotSupportedException(string.Format("SpriteFontData.Bitmap of type [{0}] is not supported. Only SpriteFontData.BitmapData or Texture2D", bitmap == null ? "null" : bitmap.GetType().Name));
                }
            }
        }

        internal void InternalDraw(ref StringProxy text, SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, ref Vector2 scale, SpriteEffects spriteEffects, float depth)
        {
            var baseOffset = origin;
            //baseOffset.Y += globalBaseOffsetY;

            // If the text is mirrored, offset the start position accordingly.
            if (spriteEffects != SpriteEffects.None)
            {
                baseOffset -= MeasureString(ref text)*axisIsMirroredTable[(int) spriteEffects & 3];
            }

            var localScale = scale;


            // Draw each character in turn.
            ForEachGlyph(ref text, (ref SpriteFontData.Glyph glyph, float x, float y) =>
                                       {
                                           var offset = new Vector2(x, y + glyph.Offset.Y);
                                           Vector2.Modulate(ref offset, ref axisDirectionTable[(int) spriteEffects & 3], out offset);
                                           Vector2.Add(ref offset, ref baseOffset, out offset);


                                           if (spriteEffects != SpriteEffects.None)
                                           {
                                               // For mirrored characters, specify bottom and/or right instead of top left.
                                               var glyphRect = new Vector2(glyph.Subrect.Right - glyph.Subrect.Left, glyph.Subrect.Top - glyph.Subrect.Bottom);
                                               Vector2.Modulate(ref glyphRect, ref axisIsMirroredTable[(int) spriteEffects & 3], out offset);
                                           }
                                           var destination = new DrawingRectangleF(position.X, position.Y, localScale.X, localScale.Y);
                                           DrawingRectangle? sourceRectangle = glyph.Subrect;
                                           spriteBatch.DrawSprite(textures[glyph.BitmapIndex], ref destination, true, ref sourceRectangle, color, rotation, ref offset, spriteEffects, depth);
                                       });
        }

        /// <summary>Returns the width and height of a string as a Vector2.</summary>
        /// <param name="text">The string to measure.</param>
        public Vector2 MeasureString(string text)
        {
            var proxyText = new StringProxy(text);
            return MeasureString(ref proxyText);
        }

        /// <summary>Returns the width and height of a string as a Vector2.</summary>
        /// <param name="text">The string to measure.</param>
        public Vector2 MeasureString(StringBuilder text)
        {
            var proxyText = new StringProxy(text);
            return MeasureString(ref proxyText);
        }

        private Vector2 MeasureString(ref StringProxy text)
        {
            var result = Vector2.Zero;
            ForEachGlyph(ref text, (ref SpriteFontData.Glyph glyph, float x, float y) =>
            {
                float w = x + (glyph.Subrect.Right - glyph.Subrect.Left);
                float h = y + Math.Max((glyph.Subrect.Bottom - glyph.Subrect.Top) + glyph.Offset.Y, LineSpacing);
                if (w > result.X) result.X = w;
                if (h > result.Y) result.Y = h;
            });

            return result;
        }

        /// <summary>Gets a collection of all the characters that are included in the font.</summary>
        public ReadOnlyCollection<char> Characters { get; private set; }

        /// <summary>Gets or sets the default character for the font.</summary>
        public char? DefaultCharacter { get; set; }

        /// <summary>Gets or sets the vertical distance (in pixels) between the base lines of two consecutive lines of text. Line spacing includes the blank space between lines as well as the height of the characters.</summary>
        public float LineSpacing { get; set; }

        /// <summary>Gets or sets the spacing of the font characters.</summary>
        public float Spacing { get; set; }

        private delegate void GlyphAction(ref SpriteFontData.Glyph glyph, float x, float y);

        private unsafe void ForEachGlyph(ref StringProxy text, GlyphAction action)
        {
            float x = 0;
            float y = 0;
            // TODO: Not sure how to handle globalBaseOffsetY from AngelCode BMFont

            fixed (void* pGlyph = glyphs)
            {
                var key = 0;
                for (int i =  0; i < text.Length; i++)
                {
                    char character = text[i];
                    key |= character;

                    switch (character)
                    {
                        case '\r':
                            // Skip carriage returns.
                            continue;

                        case '\n':
                            // New line.
                            x = 0;
                            y += LineSpacing;
                            break;

                        default:
                            // Output this character.
                            int glyphIndex;
                            if (!characterMap.TryGetValue(character, out glyphIndex))
                                throw new ArgumentException(string.Format("Character '{0}' is not available in the SpriteFont character map", character), "text");

                            var glyph = (SpriteFontData.Glyph*) pGlyph + glyphIndex;


                            x += glyph->Offset.X;

                            if (x < 0)
                                x = 0;

                            // Offset the kerning
                            float kerningOffset;
                            if (kerningMap != null && kerningMap.TryGetValue(key, out kerningOffset))
                                x += kerningOffset;

                            if (!char.IsWhiteSpace(character))
                            {
                                action(ref *glyph, x, y);
                            }

                            x += glyph->XAdvance;
                            break;
                    }

                    // Shift the kerning key
                    key  =  (key << 16);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StringProxy
        {
            private string textString;
            private StringBuilder textBuilder;
            public readonly int Length;
            public StringProxy(string text)
            {
                this.textString = text;
                this.textBuilder = null;
                this.Length = text.Length;
            }

            public StringProxy(StringBuilder text)
            {
                this.textBuilder = text;
                this.textString = null;
                this.Length = text.Length;
            }

            public char this[int index]
            {
                get
                {
                    if (this.textString != null)
                    {
                        return this.textString[index];
                    }
                    return this.textBuilder[index];
                }
            }
        }
    }


}*/