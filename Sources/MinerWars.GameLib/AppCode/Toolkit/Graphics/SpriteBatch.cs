using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpDX.Direct3D9;
using MinerWars.AppCode.Game.Utils.VertexFormats;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Renders a group of sprites.
    /// </summary>
    public class SpriteBatch //: GraphicsResource 
    {
        private const int MaxBatchSize = 2048;
        private const int MinBatchSize = 128;
        private const int InitialQueueSize = 64;
        private const int VerticesPerSprite = 4;
        private const int IndicesPerSprite = 6;
        private const int MaxVertexCount = MaxBatchSize * VerticesPerSprite;
        private const int MaxIndexCount = MaxBatchSize * IndicesPerSprite;

        private static readonly Vector2[] CornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.UnitY, Vector2.One };
        private static readonly short[] indices;
        private static Vector2 vector2Zero = Vector2.Zero;
        private static DrawingRectangle? nullRectangle;
        
        private readonly BackToFrontComparer backToFrontComparer = new BackToFrontComparer();
        private readonly EffectHandle effectMatrixTransform;
        private readonly EffectHandle effectTexture;
        private readonly EffectHandle effectCubeTexture;
        private readonly FrontToBackComparer frontToBackComparer = new FrontToBackComparer();
        private IndexBuffer indexBuffer; //short
        private Effect spriteEffect;
        private readonly EffectHandle spriteTechnique;
        private readonly EffectHandle spriteTechniqueCube0;
        private readonly EffectHandle spriteTechniqueCube1;
        private readonly EffectHandle spriteTechniqueCube2;
        private readonly EffectHandle spriteTechniqueCube3;
        private readonly EffectHandle spriteTechniqueCube4;
        private readonly EffectHandle spriteTechniqueCube5;
        private readonly TextureComparer textureComparer = new TextureComparer();
        private ResourceContext VBResourceContext;
        private readonly Dictionary<Int64, TextureInfo> textureInfos = new Dictionary<Int64, TextureInfo>(128);

        private BlendState blendState;
        private SamplerState samplerState;
        private RasterizerState rasterizerState;
        private DepthStencilState depthStencilState;
 
        private Effect customEffect;
        private EffectHandle customEffectMatrixTransform;
        private EffectHandle customEffectSampler;
        private EffectHandle customEffectTexture;

        private bool isBeginCalled;
        
        private int[] sortIndices;
        private SpriteInfo[] sortedSprites;
        private SpriteInfo[] spriteQueue;
        private int spriteQueueCount;
        private SpriteSortMode spriteSortMode;
        private TextureInfo[] spriteTextures;

        private Matrix transformMatrix;

        private Device GraphicsDevice;

        static SpriteBatch()
        {
            indices = new short[MaxIndexCount];
            int k = 0;
            for (int i = 0; i < MaxIndexCount; k += VerticesPerSprite)
            {
                indices[i++] = (short)(k + 0);
                indices[i++] = (short)(k + 1);
                indices[i++] = (short)(k + 2);
                indices[i++] = (short)(k + 1);
                indices[i++] = (short)(k + 3);
                indices[i++] = (short)(k + 2);
            }            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public SpriteBatch(Device graphicsDevice, string debugName) //: base(graphicsDevice, debugName)
        {
            //graphicsDevice.DefaultEffectPool.RegisterBytecode(effectBytecode);

            GraphicsDevice = graphicsDevice;

            spriteQueue = new SpriteInfo[MaxBatchSize];
            spriteTextures = new TextureInfo[MaxBatchSize];

            string curdir = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory("Content\\Effects2\\Toolkit");
            spriteEffect = Effect.FromFile(graphicsDevice, "SpriteEffect.fx", ShaderFlags.None);
            System.IO.Directory.SetCurrentDirectory(curdir);
            //spriteEffect.Technique = spriteEffect.GetTechnique(0];
            EffectHandle technique = spriteEffect.GetTechnique(0);

            effectMatrixTransform = spriteEffect.GetParameter(null, "MatrixTransform");
            effectTexture = spriteEffect.GetParameter(null, "Texture");
            effectCubeTexture = spriteEffect.GetParameter(null, "SpriteTextureCube");

            spriteTechnique = spriteEffect.GetTechnique("SpriteBatch");
            spriteTechniqueCube0 = spriteEffect.GetTechnique("SpriteBatchCube0");
            spriteTechniqueCube1 = spriteEffect.GetTechnique("SpriteBatchCube1");
            spriteTechniqueCube2 = spriteEffect.GetTechnique("SpriteBatchCube2");
            spriteTechniqueCube3 = spriteEffect.GetTechnique("SpriteBatchCube3");
            spriteTechniqueCube4 = spriteEffect.GetTechnique("SpriteBatchCube4");
            spriteTechniqueCube5 = spriteEffect.GetTechnique("SpriteBatchCube5");

            // Creates the vertex buffer (shared by within a device context).
            //resourceContext = GraphicsDevice.GetOrCreateSharedData(SharedDataType.PerContext, "SpriteBatch.VertexBuffer", () => new ResourceContext(GraphicsDevice));
            VBResourceContext = new ResourceContext(graphicsDevice, debugName);

            // Creates the index buffer (shared within a Direct3D11 Device)
            //indexBuffer =  GraphicsDevice.GetOrCreateSharedData(SharedDataType.PerDevice, "SpriteBatch.IndexBuffer", () => Buffer.Index.New(GraphicsDevice, indices));
            indexBuffer = new IndexBuffer(graphicsDevice, indices.Length * 2, Usage.WriteOnly, Pool.Default, true);
            indexBuffer.DebugName = "SpriteBatchIB(" + debugName + ")";
            indexBuffer.Lock(0, 0, LockFlags.None).WriteRange(indices);
            indexBuffer.Unlock();
        }

        /// <summary>
        /// Begins a sprite batch operation using deferred sort and default state objects (BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise).
        /// </summary>
        public void Begin(SpriteSortMode spritemode = SpriteSortMode.Deferred, Effect effect = null)
        {
            Begin(spritemode, null, null, null, null, effect, Matrix.Identity);
        }

        /// <summary>
        /// Begins a sprite batch rendering using the specified sorting mode and blend state. Other states are sets to default (DepthStencilState.None, SamplerState.LinearClamp, RasterizerState.CullCounterClockwise). If you pass a null blend state, the default is BlendState.AlphaBlend.
        /// </summary>
        /// <param name="sortMode">Sprite drawing order.</param>
        /// <param name="blendState">Blending options.</param>
        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            Begin(sortMode, blendState, null, null, null, null, Matrix.Identity);
        }

        /// <summary>
        /// Begins a sprite batch rendering using the specified sorting mode and blend state, sampler, depth stencil and rasterizer state objects. Passing null for any of the state objects selects the default default state objects (BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise).
        /// </summary>
        /// <param name="sortMode">Sprite drawing order.</param>
        /// <param name="blendState">Blending options.</param>
        /// <param name="samplerState">Texture sampling options.</param>
        /// <param name="depthStencilState">Depth and stencil options.</param>
        /// <param name="rasterizerState">Rasterization options.</param>
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Matrix.Identity);
        }

        /// <summary>
        /// Begins a sprite batch rendering using the specified sorting mode and blend state, sampler, depth stencil and rasterizer state objects, plus a custom effect. Passing null for any of the state objects selects the default default state objects (BlendState.AlphaBlend, DepthStencilState.None, RasterizerState.CullCounterClockwise, SamplerState.LinearClamp). Passing a null effect selects the default SpriteBatch Class shader.
        /// </summary>
        /// <param name="sortMode">Sprite drawing order.</param>
        /// <param name="blendState">Blending options.</param>
        /// <param name="samplerState">Texture sampling options.</param>
        /// <param name="depthStencilState">Depth and stencil options.</param>
        /// <param name="rasterizerState">Rasterization options.</param>
        /// <param name="effect">Effect state options.</param>
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
        }

        /// <summary>
        /// Begins a sprite batch rendering using the specified sorting mode and blend state, sampler, depth stencil, rasterizer state objects, plus a custom effect and a 2D transformation matrix. Passing null for any of the state objects selects the default default state objects (BlendState.AlphaBlend, DepthStencilState.None, RasterizerState.CullCounterClockwise, SamplerState.LinearClamp). Passing a null effect selects the default SpriteBatch Class shader. 
        /// </summary>
        /// <param name="sortMode">Sprite drawing order.</param>
        /// <param name="blendState">Blending options.</param>
        /// <param name="samplerState">Texture sampling options.</param>
        /// <param name="depthStencilState">Depth and stencil options.</param>
        /// <param name="rasterizerState">Rasterization options.</param>
        /// <param name="effect">Effect state options.</param>
        /// <param name="transformMatrix">Transformation matrix for scale, rotate, translate options.</param>
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            if (isBeginCalled)
            {
                throw new InvalidOperationException("End must be called before begin");
            }

            this.blendState = blendState;
            this.samplerState = samplerState;
            this.depthStencilState = depthStencilState;
            this.rasterizerState = rasterizerState;

            this.spriteSortMode = sortMode;
            this.customEffect = effect;
            this.transformMatrix = transformMatrix;

            // If custom effect is not null, get all its potential default parameters
            if (customEffect != null)
            {
                customEffectMatrixTransform = customEffect.GetParameter(null, "MatrixTransform");
                customEffectTexture = customEffect.GetParameter(null, "Texture");
                customEffectSampler = customEffect.GetParameter(null, "TextureSampler");
            }

            // Immediate mode, then prepare for rendering here instead of End()
            if (sortMode == SpriteSortMode.Immediate)
            {
                if (VBResourceContext.IsInImmediateMode)
                {
                    throw new InvalidOperationException("Only one SpriteBatch at a time can use SpriteSortMode.Immediate");
                }

                PrepareForRendering();

                VBResourceContext.IsInImmediateMode = true;
            }

            // Sets to true isBeginCalled
            isBeginCalled = true;
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, destination rectangle, and color. 
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="destinationRectangle">A rectangle that specifies (in screen coordinates) the destination for drawing the sprite.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <remarks>
        /// Before making any calls to Draw, you must call Begin. Once all calls to Draw are complete, call End. 
        /// </remarks>
        public void Draw(Texture texture, DrawingRectangle destinationRectangle, Color color)
        {
            var destination = new DrawingRectangleF(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawSprite(texture, null, ref destination, false, ref nullRectangle, color, 0f, ref vector2Zero, SpriteEffects.None, 0f);
        }

        public void Draw(CubeTexture texture, CubeMapFace face, DrawingRectangle destinationRectangle, Color color)
        {
            var destination = new DrawingRectangleF(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawSprite(texture, face, ref destination, false, ref nullRectangle, color, 0f, ref vector2Zero, SpriteEffects.None, 0f);
        }

        
        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position and color. 
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        public void Draw(Texture texture, Vector2 position, Color color)
        {
            var destination = new DrawingRectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(texture, null, ref destination, true, ref nullRectangle, color, 0f, ref vector2Zero, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, destination rectangle, source rectangle, color, rotation, origin, effects and layer. 
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="destinationRectangle">A rectangle that specifies (in screen coordinates) the destination for drawing the sprite. If this rectangle is not the same size as the source rectangle, the sprite will be scaled to fit.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture. </param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="effects">Effects to apply.</param>
        /// <param name="layerDepth">The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.</param>
        public void Draw(Texture texture, DrawingRectangle destinationRectangle, DrawingRectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            var destination = new DrawingRectangleF(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawSprite(texture, null, ref destination, false, ref sourceRectangle, color, rotation, ref origin, effects, layerDepth);
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position, source rectangle, and color. 
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture. </param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        public void Draw(Texture texture, Vector2 position, DrawingRectangle? sourceRectangle, Color color)
        {
            var destination = new DrawingRectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(texture, null, ref destination, true, ref sourceRectangle, color, 0f, ref vector2Zero, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position, source rectangle, color, rotation, origin, scale, effects, and layer. 
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture. </param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        /// <param name="layerDepth">The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.</param>
        public void Draw(Texture texture, Vector2 position, DrawingRectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            var destination = new DrawingRectangleF(position.X, position.Y, scale, scale);
            DrawSprite(texture, null, ref destination, true, ref sourceRectangle, color, rotation, ref origin, effects, layerDepth);
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position, source rectangle, color, rotation, origin, scale, effects, and layer. 
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture. </param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        /// <param name="layerDepth">The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.</param>
        public void Draw(Texture texture, Vector2 position, DrawingRectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            var destination = new DrawingRectangleF(position.X, position.Y, scale.X, scale.Y);
            DrawSprite(texture, null, ref destination, true, ref sourceRectangle, color, rotation, ref origin, effects, layerDepth);
        }
             

        /// <summary>
        /// Flushes the sprite batch and restores the device state to how it was before Begin was called. 
        /// </summary>
        public void End()
        {
            if (!isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before End");
            }

            if (spriteSortMode == SpriteSortMode.Immediate)
            {
                VBResourceContext.IsInImmediateMode = false;
            }
            else if (spriteQueueCount > 0)
            {
                  // Draw the queued sprites now.
                if (VBResourceContext.IsInImmediateMode)
                {
                    throw new InvalidOperationException("Cannot end one SpriteBatch while another is using SpriteSortMode.Immediate");
                }

                // If not immediate, then setup and render all sprites
                PrepareForRendering();
                FlushBatch();
            }

            // Clear the custom effect so that it won't be used next Begin/End
            if (customEffect != null)
            {
                customEffectMatrixTransform = null;
                customEffectTexture = null;
                customEffectSampler = null;
                customEffect = null;
            }

            // Clear stored texture infos
            textureInfos.Clear();

            // We are with begin pair
            isBeginCalled = false;
        }

        private void FlushBatch()
        {
            SpriteInfo[] spriteQueueForBatch;

            // If Deferred, then sprites are displayed in the same order they arrived
            if (spriteSortMode == SpriteSortMode.Deferred)
            {
                spriteQueueForBatch = spriteQueue;
            }
            else
            {
                // Else Sort all sprites according to their sprite order mode.
                SortSprites();
                spriteQueueForBatch = sortedSprites;
            }

            // Iterate on all sprites and group batch per texture.
            int offset = 0;
            var previousTexture = default(TextureInfo);
            for (int i = 0; i < spriteQueueCount; i++)
            {
                TextureInfo texture;

                if (spriteSortMode == SpriteSortMode.Deferred)
                {
                    texture = spriteTextures[i];
                }
                else
                {
                    // Copy ordered sprites to the queue to batch
                    int index = sortIndices[i];
                    spriteQueueForBatch[i] = spriteQueue[index];

                    // Get the texture indirectly
                    texture = spriteTextures[index];
                }

                if (texture.Texture != previousTexture.Texture)
                {
                    if (i > offset)
                    {
                        DrawBatchPerTexture(ref previousTexture, spriteQueueForBatch, offset, i - offset);
                    }

                    offset = i;
                    previousTexture = texture;
                }
            }

            // Draw the last batch
            DrawBatchPerTexture(ref previousTexture, spriteQueueForBatch, offset, spriteQueueCount - offset);

            // Reset the queue.
            Array.Clear(spriteTextures, 0, spriteQueueCount);
            spriteQueueCount = 0;

            // When sorting is disabled, we persist mSortedSprites data from one batch to the next, to avoid
            // uneccessary work in GrowSortedSprites. But we never reuse these when sorting, because re-sorting
            // previously sorted items gives unstable ordering if some sprites have identical sort keys.
            if (spriteSortMode != SpriteSortMode.Deferred)
            {
                Array.Clear(sortedSprites, 0, sortedSprites.Length);
            }
        }

        private void SortSprites()
        {
            IComparer<int> comparer;

            switch (spriteSortMode)
            {
                case SpriteSortMode.Texture:
                    textureComparer.SpriteTextures = spriteTextures;
                    comparer = textureComparer;
                    break;

                case SpriteSortMode.BackToFront:
                    backToFrontComparer.SpriteQueue = spriteQueue;
                    comparer = backToFrontComparer;
                    break;

                case SpriteSortMode.FrontToBack:
                    frontToBackComparer.SpriteQueue = spriteQueue;
                    comparer = frontToBackComparer;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if ((sortIndices == null) || (sortIndices.Length < spriteQueueCount))
            {
                sortIndices = new int[spriteQueueCount];
                sortedSprites = new SpriteInfo[spriteQueueCount];
            }

            // Reset all indices to the original order
            for (int i = 0; i < spriteQueueCount; i++)
            {
                sortIndices[i] = i;
            }

            Array.Sort(sortIndices, 0, spriteQueueCount, comparer);
        }

        internal unsafe void DrawSprite(BaseTexture texture, CubeMapFace? face, ref DrawingRectangleF destination, bool scaleDestination, ref DrawingRectangle? sourceRectangle, Color color, float rotation, ref Vector2 origin, SpriteEffects effects, float depth)
        {
            // Check that texture is not null
            if (texture == null || texture.NativePointer == IntPtr.Zero)
            {
                throw new ArgumentNullException("texture");
            }

            // Make sure that Begin was called
            if (!isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before draw");
            }

            // Resize the buffer of SpriteInfo
            if (spriteQueueCount >= spriteQueue.Length)
            {
                Array.Resize(ref spriteQueue, spriteQueue.Length*2);
            }

            // Gets the resource information from the view (width, height).
            // Cache the result in order to avoid this request if the texture is reused 
            // inside a same Begin/End block.
            TextureInfo textureInfo;
            if (!textureInfos.TryGetValue(texture.NativePointer.ToInt64(), out textureInfo))
            {
                textureInfo.Texture = texture;
                textureInfo.Face = face;

                SurfaceDescription description2D;
                if (face.HasValue)
                {
                    Surface cubeSurface = ((CubeTexture)texture).GetCubeMapSurface(face.Value, 0);
                    description2D = cubeSurface.Description;
                    cubeSurface.Dispose();
                }
                else
                {
                    description2D = ((Texture)texture).GetLevelDescription(0);
                }

                textureInfo.Width = description2D.Width;
                textureInfo.Height = description2D.Height;

                textureInfos.Add(texture.NativePointer.ToInt64(), textureInfo);
            }

            // Put values in next SpriteInfo
            fixed (SpriteInfo* spriteInfo = &(spriteQueue[spriteQueueCount]))
            {
                float width;
                float height;

                // If the source rectangle has a value, then use it.
                if (sourceRectangle.HasValue)
                {
                    DrawingRectangle rectangle = sourceRectangle.Value;
                    spriteInfo->Source.X = rectangle.X;
                    spriteInfo->Source.Y = rectangle.Y;
                    width = rectangle.Width;
                    height = rectangle.Height;
                }
                else
                {
                    // Else, use directly the size of the texture
                    spriteInfo->Source.X = 0.0f;
                    spriteInfo->Source.Y = 0.0f;
                    width = textureInfo.Width;
                    height = textureInfo.Height;
                }

                // Sets the width and height
                spriteInfo->Source.Width = width;
                spriteInfo->Source.Height = height;

                // Scale the destination box
                if (scaleDestination)
                {
                    destination.Width *= width;
                    destination.Height *= height;
                }

                // Sets the destination
                spriteInfo->Destination = destination;

                // Copy all other values.
                spriteInfo->Origin.X = origin.X;
                spriteInfo->Origin.Y = origin.Y;
                spriteInfo->Rotation = rotation;
                spriteInfo->Depth = depth;
                spriteInfo->SpriteEffects = effects;
                spriteInfo->Color = color;
            }

            // If we are in immediate mode, render the sprite directly
            if (spriteSortMode == SpriteSortMode.Immediate)
            {
                DrawBatchPerTexture(ref textureInfo, spriteQueue, 0, 1);
            }
            else
            {
                if (spriteTextures.Length < spriteQueue.Length)
                {
                    Array.Resize(ref spriteTextures, spriteQueue.Length);
                }
                spriteTextures[spriteQueueCount] = textureInfo;
                spriteQueueCount++;
            }
        }

        private void DrawBatchPerTexture(ref TextureInfo texture, SpriteInfo[] sprites, int offset, int count)
        {
            if (customEffect != null)
            {
                var currentTechnique = customEffect.Technique;

                int passCount = customEffect.GetTechniqueDescription(currentTechnique).Passes;
                for (int i = 0; i < passCount; i++)
                {
                    // Sets the texture on the custom effect if the parameter exist
                    if (customEffectTexture != null)
                    {
                        customEffect.SetTexture(customEffectTexture, texture.Texture);
                    }


                    customEffect.Begin();
                    // Apply the current pass
                    customEffect.BeginPass(i);

                    // Draw the batch of sprites
                    DrawBatchPerTextureAndPass(ref texture, sprites, offset, count);

                    customEffect.EndPass();
                    customEffect.End();
                }
            }
            else
            {

                if (texture.Face.HasValue)
                {
                    spriteEffect.SetTexture(effectCubeTexture, texture.Texture);

                    switch (texture.Face.Value)
                    {
                        case CubeMapFace.PositiveX:
                            spriteEffect.Technique = spriteTechniqueCube0;
                            break;
                        case CubeMapFace.NegativeX:
                            spriteEffect.Technique = spriteTechniqueCube1;
                            break;
                        case CubeMapFace.PositiveY:
                            spriteEffect.Technique = spriteTechniqueCube2;
                            break;
                        case CubeMapFace.NegativeY:
                            spriteEffect.Technique = spriteTechniqueCube3;
                            break;
                        case CubeMapFace.PositiveZ:
                            spriteEffect.Technique = spriteTechniqueCube4;
                            break;
                        case CubeMapFace.NegativeZ:
                            spriteEffect.Technique = spriteTechniqueCube5;
                            break;
                    }
                }
                else
                {
                    spriteEffect.SetTexture(effectTexture, texture.Texture);
                    spriteEffect.Technique = spriteTechnique;
                }
            }

            spriteEffect.Begin();

            spriteEffect.BeginPass(0);

            DrawBatchPerTextureAndPass(ref texture, sprites, offset, count);

            spriteEffect.EndPass();

            spriteEffect.End();
        }

        private unsafe void DrawBatchPerTextureAndPass(ref TextureInfo texture, SpriteInfo[] sprites, int offset, int count)
        {
            float deltaX = 1f/(texture.Width);
            float deltaY = 1f/(texture.Height);
            while (count > 0)
            {
                // How many sprites do we want to draw?
                int batchSize = count;

                // How many sprites does the D3D vertex buffer have room for?
                int remainingSpace = MaxBatchSize - VBResourceContext.VertexBufferPosition;
                if (batchSize > remainingSpace)
                {
                    if (remainingSpace < MinBatchSize)
                    {
                        VBResourceContext.VertexBufferPosition = 0;
                        batchSize = (count < MaxBatchSize) ? count : MaxBatchSize;
                    }
                    else
                    {
                        batchSize = remainingSpace;
                    }
                }

                // Sets the data directly to the buffer in memory
                int offsetInBytes = VBResourceContext.VertexBufferPosition * VerticesPerSprite * MyVertexFormatPositionTextureColor.Stride;

                var noOverwrite = VBResourceContext.VertexBufferPosition == 0 ? LockFlags.Discard : LockFlags.NoOverwrite;

                var ptr = VBResourceContext.VertexBuffer.LockToPointer(offsetInBytes, batchSize * VerticesPerSprite * MyVertexFormatPositionTextureColor.Stride, noOverwrite);
                   
                var vertexPtr = (MyVertexFormatPositionTextureColor*)ptr;
                    
                for (int i = 0; i < batchSize; i++)
                {
                    UpdateVertexFromSpriteInfo(ref sprites[offset + i], ref vertexPtr, deltaX, deltaY);
                } 

                VBResourceContext.VertexBuffer.Unlock();

                // Draw from the specified index
                int startIndex = VBResourceContext.VertexBufferPosition * IndicesPerSprite;
                int indexCount = batchSize * IndicesPerSprite;

                GraphicsDevice.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, MaxVertexCount, startIndex, indexCount / 3);

                // Update position, offset and remaining count
                VBResourceContext.VertexBufferPosition += batchSize;
                offset += batchSize;
                count -= batchSize;
            }
        }

        private unsafe void UpdateVertexFromSpriteInfo(ref SpriteInfo spriteInfo, ref MyVertexFormatPositionTextureColor* vertex, float deltaX, float deltaY)
        {
            var rotation = spriteInfo.Rotation != 0f ? new Vector2((float) Math.Cos(spriteInfo.Rotation), (float) Math.Sin(spriteInfo.Rotation)) : Vector2.UnitX;

            // Origin scale down to the size of the source texture 
            var origin = spriteInfo.Origin;
            origin.X /= spriteInfo.Source.Width == 0f ? float.Epsilon : spriteInfo.Source.Width;
            origin.Y /= spriteInfo.Source.Height == 0f ? float.Epsilon : spriteInfo.Source.Height;

            for (int j = 0; j < 4; j++)
            {
                // Gets the corner and take into account the Flip mode.
                var corner = CornerOffsets[j];
                // Calculate position on destination
                var position = new Vector2((corner.X - origin.X) * spriteInfo.Destination.Width, (corner.Y - origin.Y) * spriteInfo.Destination.Height);

                // Apply rotation and destination offset
                vertex->Position.X = spriteInfo.Destination.X + (position.X * rotation.X) - (position.Y * rotation.Y);
                vertex->Position.Y = spriteInfo.Destination.Y + (position.X * rotation.Y) + (position.Y * rotation.X);
                vertex->Position.Z = spriteInfo.Depth;
                vertex->Color = SharpDXHelper.ToXNA(spriteInfo.Color.ToVector4());

                corner = CornerOffsets[j ^ (int)spriteInfo.SpriteEffects];
                vertex->TexCoord.X = (spriteInfo.Source.X + corner.X * spriteInfo.Source.Width) * deltaX;
                vertex->TexCoord.Y = (spriteInfo.Source.Y + corner.Y * spriteInfo.Source.Height) * deltaY;

                vertex++;
            }
        }

        private void UpdateVertexFromSpriteInfo2(ref SpriteInfo spriteInfo, ref MyVertexFormatPositionTextureColor vertex, float deltaX, float deltaY)
        {
            var rotation = spriteInfo.Rotation != 0f ? new Vector2((float)Math.Cos(spriteInfo.Rotation), (float)Math.Sin(spriteInfo.Rotation)) : Vector2.UnitX;

            // Origin scale down to the size of the source texture 
            var origin = spriteInfo.Origin;
            origin.X /= spriteInfo.Source.Width == 0f ? float.Epsilon : spriteInfo.Source.Width;
            origin.Y /= spriteInfo.Source.Height == 0f ? float.Epsilon : spriteInfo.Source.Height;

            for (int j = 0; j < 4; j++)
            {
                // Gets the corner and take into account the Flip mode.
                var corner = CornerOffsets[j];
                // Calculate position on destination
                var position = new Vector2((corner.X - origin.X) * spriteInfo.Destination.Width, (corner.Y - origin.Y) * spriteInfo.Destination.Height);

                // Apply rotation and destination offset
                vertex.Position.X = spriteInfo.Destination.X + (position.X * rotation.X) - (position.Y * rotation.Y);
                vertex.Position.Y = spriteInfo.Destination.Y + (position.X * rotation.Y) + (position.Y * rotation.X);
                vertex.Position.Z = spriteInfo.Depth;
                vertex.Color = SharpDXHelper.ToXNA(spriteInfo.Color.ToVector4());

                corner = CornerOffsets[j ^ (int)spriteInfo.SpriteEffects];
                vertex.TexCoord.X = (spriteInfo.Source.X + corner.X * spriteInfo.Source.Width) * deltaX;
                vertex.TexCoord.Y = (spriteInfo.Source.Y + corner.Y * spriteInfo.Source.Height) * deltaY;
            }
        }

        private void PrepareForRendering()
        {
            // Setup states (Blend, DepthStencil, Rasterizer)
            if (blendState != null)
                blendState.Apply();

            if (rasterizerState != null)
                rasterizerState.Apply();

            if (depthStencilState != null)
                depthStencilState.Apply();

            if (samplerState != null)
                samplerState.Apply();

            // Build ortho-projection matrix
            ViewportF viewport = GraphicsDevice.Viewport;
            float xRatio = (viewport.Width > 0) ? (1f/(viewport.Width)) : 0f;
            float yRatio = (viewport.Height > 0) ? (-1f/(viewport.Height)) : 0f;
            var matrix = new Matrix { M11 = xRatio * 2f, M22 = yRatio * 2f, M33 = 1f, M44 = 1f, M41 = -1f, M42 = 1f };

            Matrix finalMatrix;
            Matrix.Multiply(ref transformMatrix, ref matrix, out finalMatrix);

           
            // Use LinearClamp for sampler state
            //var localSamplerState = samplerState ?? GraphicsDevice.SamplerStates.LinearClamp;

            // Setup effect states and parameters: SamplerState and MatrixTransform
            // Sets the sampler state
            if (customEffect != null)
            {
                if (customEffect.Technique == null)
                    throw new InvalidOperationException("CurrentTechnique is not set on custom effect");

                //if (customEffectSampler != null)
                  //  customEffectSampler.SetResource(localSamplerState);

                if (customEffectMatrixTransform != null)
                    customEffect.SetValue(customEffectMatrixTransform, finalMatrix);
            }
            else
            {
                //effectSampler.SetResource(localSamplerState);
                spriteEffect.SetValue(effectMatrixTransform, finalMatrix);
            }

            // Set VertexInputLayout
            GraphicsDevice.VertexDeclaration = MyVertexFormatPositionTextureColor.VertexDeclaration;

            // VertexBuffer
            GraphicsDevice.SetStreamSource(0, VBResourceContext.VertexBuffer, 0, MyVertexFormatPositionTextureColor.Stride);

            // Index buffer
            GraphicsDevice.Indices = indexBuffer;

            // If this is a deferred D3D context, reset position so the first Map call will use D3D11_MAP_WRITE_DISCARD.
           /* if (GraphicsDevice.IsDeferred)
            {
                VBResourceContext.VertexBufferPosition = 0;
            } */
        }


        public void Dispose()
        {
            indexBuffer.Dispose();
            indexBuffer = null;

            spriteEffect.Dispose();
            spriteEffect = null;

            VBResourceContext.Dispose();
            VBResourceContext = null;

            //base.Dispose(disposeManagedResources);
        }

        #region Nested type: BackToFrontComparer

        private class BackToFrontComparer : IComparer<int>
        {
            public SpriteInfo[] SpriteQueue;

            #region IComparer<int> Members

            public int Compare(int left, int right)
            {
                return SpriteQueue[right].Depth.CompareTo(SpriteQueue[left].Depth);
            }

            #endregion
        }

        #endregion

        #region Nested type: FrontToBackComparer

        private class FrontToBackComparer : IComparer<int>
        {
            public SpriteInfo[] SpriteQueue;

            #region IComparer<int> Members

            public int Compare(int left, int right)
            {
                return SpriteQueue[left].Depth.CompareTo(SpriteQueue[right].Depth);
            }

            #endregion
        }

        #endregion

        #region Nested type: TextureComparer

        private class TextureComparer : IComparer<int>
        {
            public TextureInfo[] SpriteTextures;

            #region IComparer<int> Members

            public int Compare(int left, int right)
            {
                return SpriteTextures[left].Texture.NativePointer.ToInt64().CompareTo(SpriteTextures[right].Texture.NativePointer.ToInt64());
            }

            #endregion
        }

        #endregion

        #region Nested type: SpriteInfo

        [StructLayout(LayoutKind.Sequential)]
        private struct SpriteInfo
        {
            public DrawingRectangleF Source;
            public DrawingRectangleF Destination;
            public Vector2 Origin;
            public float Rotation;
            public float Depth;
            public SpriteEffects SpriteEffects;
            public Color Color;
        }

        #endregion


        /// <summary>
        /// Use a ResourceContext per GraphicsDevice (DeviceContext)
        /// </summary>
        private class ResourceContext : Component
        {
            public readonly VertexBuffer VertexBuffer;

            public int VertexBufferPosition;

            public bool IsInImmediateMode;

            public ResourceContext(Device device, string debugName)
            {
                VertexBuffer = new VertexBuffer(device, MyVertexFormatPositionTextureColor.Stride * MaxVertexCount, Usage.Dynamic | Usage.WriteOnly, VertexFormat.None, Pool.Default);
                VertexBuffer.DebugName = "SpriteBatchVB(" + debugName + ")";
            }

            protected override void Dispose(bool disposeManagedResources)
            {
                VertexBuffer.Dispose();

                base.Dispose(disposeManagedResources);
            }
        }

        /// <summary>
        /// Internal structure used to store texture information.
        /// </summary>
        private struct TextureInfo
        {
            public BaseTexture Texture;
            public CubeMapFace? Face;

            public int Width;

            public int Height;
        }
    }
}