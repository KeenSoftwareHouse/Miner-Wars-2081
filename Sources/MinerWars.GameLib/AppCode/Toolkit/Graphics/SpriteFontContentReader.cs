using System.IO;
//using SharpDX.Toolkit.Content;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Internal class to load a SpriteFont.
    /// </summary>
    internal class SpriteFontContentReader// : GraphicsResourceContentReaderBase<SpriteFont>
    {                         /*
        protected override SpriteFont ReadContent(IContentManager readerManager, GraphicsDevice device, string assetName, Stream stream)
        {
            SpriteFont spriteFont = null;
            var assetPath = Path.GetDirectoryName(assetName);

            // Load the sprite font data
            var spriteFontData = SpriteFontData.Load(stream, name => readerManager.Load<Texture2D>(Path.Combine(assetPath ?? string.Empty, name)));

            // If sprite font was fine, then instantiate SpriteFont graphics object.
            if (spriteFontData != null)
                spriteFont = SpriteFont.New(device, spriteFontData);

            return spriteFont;
        }                       */
    }
}