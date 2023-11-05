using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System.Diagnostics;

namespace Aurora.Disktop.Graphics
{





    public class GraphicContext : IDisposable
    {

        public struct ContextState
        {
            public ContextState(SpriteSortMode sortMode, BlendState blending, Effect effect, Boolean beginCalled)
            {
                this.Effect = effect;
                this.BeginCalled = beginCalled;
                this.SortMode = sortMode;
                this.Blending = blending;

            }
            public readonly BlendState Blending;
            public readonly Effect Effect;
            public readonly SpriteSortMode SortMode;
            public readonly Boolean BeginCalled;
        }

        public GraphicsDevice GraphicsDevice { get; private set; }
        private SpriteBatch SpriteBatch { get; set; }

        private Boolean _beginCalled { get; set; }

        private BasicEffect _basicEffect { get; set; }

        private BlendState currentBlending { get; set; } = null;

        private Effect currentEffect { get; set; } = null;

        private SpriteSortMode currentSortMode { get; set; } = SpriteSortMode.Deferred;

        private Stack<ContextState> _stack = new Stack<ContextState>();

        private Texture2D pixel;


        public GraphicContext(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            this.SpriteBatch = new SpriteBatch(graphicsDevice);
            this.currentSortMode = SpriteSortMode.Deferred;
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            //
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }


        public virtual void Dispose()
        {
            if (this.GraphicsDevice != null)
            {
                this.GraphicsDevice = null;
                this.SpriteBatch.Dispose();
                this.SpriteBatch = null;
                this._basicEffect.Dispose();
                this._basicEffect = null;
                this.currentBlending = null;
            }
        }


        public TargetRenderer TargetRender(TargetTexture texture)
        {
            return new TargetRenderer(this, texture);
        }



        public TargetTexture CreateRenderTarget(Int32 width, Int32 height)
        {
            return TargetTexture.Create(this.GraphicsDevice, width, height);
        }


        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, Effect effect = null)
        {
            if (!this._beginCalled)
            {
                this.currentEffect = effect;
                this.currentBlending = blendState;
                this.currentSortMode = sortMode;
                this.SpriteBatch.Begin(sortMode, blendState, effect: effect);
                this._beginCalled = true;
            }
        }


        public ContextState? SetState(SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if (sortMode != currentSortMode)
            {
                var state = new ContextState(currentSortMode, currentBlending, currentEffect, this._beginCalled);
                if (this._beginCalled) this.End();
                this.currentSortMode = sortMode;
                this.Begin(sortMode: currentSortMode, blendState: currentBlending, effect: currentEffect);
                return state;
            }
            return null;
        }


        public ContextState? SetState(Effect effect = null)
        {
            if (effect != currentEffect)
            {
                var state = new ContextState(currentSortMode, currentBlending, currentEffect, this._beginCalled);
                if (this._beginCalled) this.End();
                this.currentEffect = effect;
                this.Begin(sortMode: currentSortMode, blendState: currentBlending, effect: effect);
                return state;
            }
            return null;
        }

        public ContextState? SetState(BlendState blendState = null)
        {
            if (blendState != currentBlending)
            {
                var state = new ContextState(currentSortMode, currentBlending, currentEffect, this._beginCalled);
                if (this._beginCalled) this.End();
                this.currentBlending = blendState;
                this.Begin(sortMode: currentSortMode, blendState: currentBlending, effect: currentEffect);
                return state;
            }
            return null;
        }

        public ContextState? SetState(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, Effect effect = null)
        {
            if (blendState != currentBlending || effect != currentEffect || sortMode != currentSortMode)
            {
                var state = new ContextState(currentSortMode, currentBlending, currentEffect, this._beginCalled);
                if (this._beginCalled) this.End();
                this.currentSortMode = sortMode;
                this.currentBlending = blendState;
                this.currentEffect = effect;
                this.Begin(sortMode: currentSortMode, blendState: blendState, effect: effect);
                return state;
            }
            return null;
        }



        public ContextState EndState()
        {
            var result = new ContextState(this.currentSortMode, this.currentBlending, this.currentEffect, this._beginCalled);
            if (this._beginCalled) this.End();
            return result;
        }


 



        public void RestoreState(ContextState state)
        {
            if (this._beginCalled) this.End();
            this.currentSortMode = state.SortMode;
            this.currentBlending = state.Blending;
            this.currentEffect = state.Effect;
            if (state.BeginCalled)
            {
                this.Begin(sortMode: currentSortMode, blendState: currentBlending, effect: currentEffect);
            }
        }



        public BlendState End()
        {
            if (this._beginCalled)
            {
                this.SpriteBatch.End();
                this._beginCalled = false;
            }
            return this.currentBlending;
        }








        public Boolean PushState(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, Effect effect = null)
        {
            if (blendState != currentBlending || effect != currentEffect || sortMode != currentSortMode)
            {
                this._stack.Push(new ContextState(currentSortMode, currentBlending, currentEffect, this._beginCalled));
                if (this._beginCalled) this.End();
                this.currentSortMode = sortMode;
                this.currentBlending = blendState;
                this.currentEffect = effect;
                this.Begin(sortMode: currentSortMode, blendState: blendState, effect: effect);
                return true;
            }
            return false;
        }


        public void PopState()
        {
            if (this._stack.Count == 0)
            {
                return;
            }
            var state = this._stack.Pop();
            this.End();
            this.currentSortMode = state.SortMode;
            this.currentBlending = state.Blending;
            this.currentEffect = state.Effect;
            if (state.BeginCalled)
            {
                this.Begin(sortMode: currentSortMode, blendState: currentBlending, effect: currentEffect);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void Clear(Color color)
        {
            this.GraphicsDevice.Clear(color);
        }


        public void DrawSprite(SpriteObject sprite, Vector2 position, Int32 index, Color color)
        {
            var tex = sprite.Texture.Tex();
            if (tex != null)
            {
                var state = this.SetState(sprite.Texture.BlendState);
                var sourceRectangle = sprite.GetFrameRectangle(index);
                this.SpriteBatch.Draw(tex, position + sprite.Texture.Offset, sourceRectangle, color);
                if (state != null) this.RestoreState(state.Value);
            }
        }

        public void DrawSprite(SpriteObject sprite, Rectangle destinationRectangle, Int32 index, Color color)
        {
            var tex = sprite.Texture.Tex();
            if (tex != null)
            {
                var state = this.SetState(sprite.Texture.BlendState);
                var sourceRectangle = sprite.GetFrameRectangle(index);
                this.SpriteBatch.Draw(tex, destinationRectangle.Add(sprite.Texture.Offset), sourceRectangle, color);
                if (state != null) this.RestoreState(state.Value);
            }
        }


        public void Draw(ITexture context, Vector2 position, Color color)
        {
            var tex = context.Tex();
            if (tex != null)
            {
                var state = this.SetState(context.BlendState);
                this.SpriteBatch.Draw(tex, position + context.Offset, color);
                if (state != null) this.RestoreState(state.Value);
            }
        }

        public void Draw(ITexture context, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            var tex = context.Tex();
            if (tex != null)
            {
                var state = this.SetState(context.BlendState);
                this.SpriteBatch.Draw(tex, position + context.Offset, sourceRectangle, color);
                if (state != null) this.RestoreState(state.Value);
            }
        }
        public void Draw(ITexture context, Rectangle destinationRectangle, Color color)
        {
            var tex = context.Tex();
            if (tex != null)
            {
                var state = this.SetState(context.BlendState);
                var rectangle = new Rectangle(destinationRectangle.X + (int)context.Offset.X, destinationRectangle.Y + (int)context.Offset.Y, destinationRectangle.Width, destinationRectangle.Height);
                this.SpriteBatch.Draw(tex, rectangle, color);
                if (state != null) this.RestoreState(state.Value);
            }
        }
        public void Draw(ITexture context, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            var tex = context.Tex();
            if (tex != null)
            {
                var state = this.SetState(context.BlendState);
                var rectangle = new Rectangle(destinationRectangle.X + (int)context.Offset.X, destinationRectangle.Y + (int)context.Offset.Y, destinationRectangle.Width, destinationRectangle.Height);
                this.SpriteBatch.Draw(tex, rectangle, sourceRectangle, color);
                if (state != null) this.RestoreState(state.Value);
            }
        }
        public void Draw(ITexture context, Rectangle destinationRectangle, Color color, float rotation, Vector2 origin)
        {
            var tex = context.Tex();
            if (tex != null)
            {
                var state = this.SetState(context.BlendState);
                var rectangle = new Rectangle(destinationRectangle.X + (int)context.Offset.X, destinationRectangle.Y + (int)context.Offset.Y, destinationRectangle.Width, destinationRectangle.Height);
                this.SpriteBatch.Draw(tex, rectangle, tex.Bounds, color, rotation, origin, SpriteEffects.None, 0);
                if (state != null) this.RestoreState(state.Value);
            }
        }
        public void Draw(ITexture context, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin)
        {
            var tex = context.Tex();
            if (tex != null)
            {
                var state = this.SetState(context.BlendState);
                var rectangle = new Rectangle(destinationRectangle.X + (int)context.Offset.X, destinationRectangle.Y + (int)context.Offset.Y, destinationRectangle.Width, destinationRectangle.Height);
                this.SpriteBatch.Draw(tex, rectangle, sourceRectangle, color, rotation, origin, SpriteEffects.None, 0);
                if (state != null) this.RestoreState(state.Value);
            }
        }





        /// <summary>
        /// 渲染地砖
        /// </summary>
        /// <param name="Destination"></param>
        /// <param name="Flags"></param>
        public void DrawTitle(ITexture texure, Rectangle Destination, Color color)
        {
            var tex = texure.Tex();
            var state = this.SetState(texure.BlendState);
            Point Local = new Point(Destination.Left, Destination.Top);
            while (true)
            {
                int mHeight = Math.Min(texure.Height, Destination.Bottom - Local.Y);
                while (true)
                {
                    //获取当前最大渲染宽度
                    int mwidth = Math.Min(texure.Width, Destination.Right - Local.X);
                    {
                        //获取剪裁大小
                        Rectangle rSourceRect = new Rectangle(0, 0, mwidth, mHeight);
                        //定位渲染位置
                        this.SpriteBatch.Draw(tex, new Vector2(Local.X, Local.Y), rSourceRect, color);
                    }
                    //原，目标大小为 mwidth 和 mheight
                    Local.X += mwidth;
                    if (Local.X >= Destination.Right) break;
                }
                //
                Local.Y += mHeight;
                Local.X = Destination.Left;
                if (Local.Y >= Destination.Bottom) break;
            }
            if (state != null) this.RestoreState(state.Value);
        }




        /// <summary>
        /// 渲染地砖
        /// </summary>
        /// <param name="Destination"></param>
        /// <param name="Flags"></param>
        public void DrawTitle(ITexture texure, Rectangle Destination, Rectangle srcRect, Color color)
        {
            var tex = texure.Tex();
            var state = this.SetState(texure.BlendState);
            Point Local = new Point(Destination.Left, Destination.Top);
            while (true)
            {
                int mHeight = Math.Min(srcRect.Height, Destination.Bottom - Local.Y);
                while (true)
                {
                    //获取当前最大渲染宽度
                    int mwidth = Math.Min(srcRect.Width, Destination.Right - Local.X);
                    {
                        //获取剪裁大小
                        Rectangle rSourceRect = new Rectangle(srcRect.Left, srcRect.Top, mwidth, mHeight);
                        //定位渲染位置
                        this.SpriteBatch.Draw(tex, new Rectangle(Local.X, Local.Y, rSourceRect.Width, rSourceRect.Height), rSourceRect, color);
                    }
                    //原，目标大小为 mwidth 和 mheight
                    Local.X += mwidth;
                    if (Local.X >= Destination.Right) break;
                }
                //
                Local.Y += mHeight;
                Local.X = Destination.Left;
                if (Local.Y >= Destination.Bottom) break;
            }
            if (state != null) this.RestoreState(state.Value);
        }






        public void DrawNineSlice(ITexture texure, Rectangle dest)
        {
            // 计算边框的大小
            Point borderSize = new Point(texure.Width / 3, texure.Height / 3);
            var tex = texure.Tex();
            var state = this.SetState(texure.BlendState);
            // 绘制九宫格的四个角
            this.SpriteBatch.Draw(tex, new Rectangle(dest.X, dest.Y, borderSize.X, borderSize.Y), new Rectangle(0, 0, borderSize.X, borderSize.Y), Color.White);
            this.SpriteBatch.Draw(tex, new Rectangle(dest.X + dest.Width - borderSize.X, dest.Y, borderSize.X, borderSize.Y), new Rectangle(tex.Width - borderSize.X, 0, borderSize.X, borderSize.Y), Color.White);
            this.SpriteBatch.Draw(tex, new Rectangle(dest.X, dest.Y + dest.Height - borderSize.Y, borderSize.X, borderSize.Y), new Rectangle(0, tex.Height - borderSize.Y, borderSize.X, borderSize.Y), Color.White);
            this.SpriteBatch.Draw(tex, new Rectangle(dest.X + dest.Width - borderSize.X, dest.Y + dest.Height - borderSize.Y, borderSize.X, borderSize.Y), new Rectangle(tex.Width - borderSize.X, tex.Height - borderSize.Y, borderSize.X, borderSize.Y), Color.White);

            // 绘制九宫格的四个边，使用 Tiling 方式填充
            for (int x = dest.X + borderSize.X; x < dest.X + dest.Width - borderSize.X; x += borderSize.X)
            {
                int width = Math.Min(borderSize.X, dest.X + dest.Width - borderSize.X - x);
                this.SpriteBatch.Draw(tex, new Rectangle(x, dest.Y, width, borderSize.Y), new Rectangle(borderSize.X, 0, width, borderSize.Y), Color.White);
                this.SpriteBatch.Draw(tex, new Rectangle(x, dest.Y + dest.Height - borderSize.Y, width, borderSize.Y), new Rectangle(borderSize.X, tex.Height - borderSize.Y, width, borderSize.Y), Color.White);
            }

            for (int y = dest.Y + borderSize.Y; y < dest.Y + dest.Height - borderSize.Y; y += borderSize.Y)
            {
                int height = Math.Min(borderSize.Y, dest.Y + dest.Height - borderSize.Y - y);
                this.SpriteBatch.Draw(tex, new Rectangle(dest.X, y, borderSize.X, height), new Rectangle(0, borderSize.Y, borderSize.X, height), Color.White);
                this.SpriteBatch.Draw(tex, new Rectangle(dest.Right - borderSize.X, y, borderSize.X, height), new Rectangle(tex.Width - borderSize.X, borderSize.Y, borderSize.X, height), Color.White);
            }

            // 绘制九宫格的中间部分，使用 Tiling 方式填充
            for (int x = dest.X + borderSize.X; x < dest.X + dest.Width - borderSize.X; x += borderSize.X)
            {
                int width = Math.Min(borderSize.X, dest.X + dest.Width - borderSize.X - x);
                for (int y = dest.Y + borderSize.Y; y < dest.Y + dest.Height - borderSize.Y; y += borderSize.Y)
                {
                    int height = Math.Min(borderSize.Y, dest.Y + dest.Height - borderSize.Y - y);
                    this.SpriteBatch.Draw(tex, new Rectangle(x, y, width, height), new Rectangle(borderSize.X, borderSize.Y, width, height), Color.White);
                }
            }
            if (state != null) this.RestoreState(state.Value);
        }



        public float DrawString(DynamicSpriteFont font, string _string_, Vector2 pos, Color color)
        {
            var state = this.SetState(blendState: null);
            var val = font.DrawString(this.SpriteBatch, _string_, pos, color);
            if (state != null) this.RestoreState(state.Value);
            return val;
        }


        public float DrawString(DynamicSpriteFont font, string _string_, Vector2 pos, Color color, Vector2 scale)
        {
            var state = this.SetState(blendState: null);
            var val = font.DrawString(this.SpriteBatch, _string_, pos, color, scale);
            if (state != null) this.RestoreState(state.Value);
            return val;
        }

        public Vector2 MeasureString(DynamicSpriteFont font, string text)
        {
            return font.MeasureString(text);
        }

        public Rectangle GetTextBounds(DynamicSpriteFont font, Vector2 position, string text)
        {
            return font.GetTextBounds(position, text);
        }



        public void DrawMarkRect(Rectangle rect, Double Angle, Color dwColor)
        {
            VertexPositionColor vResult;
            VertexPositionColor[] vbData = new VertexPositionColor[15];
            //  规定中心点
            var nTriangles = 0;
            var Cx = rect.Left + (float)rect.Width / 2;
            var Cy = rect.Top + (float)rect.Height / 2;
            var w2 = (float)rect.Width / 2;
            var h2 = (float)rect.Height / 2;

            // taiangle 1
            vbData[00] = new VertexPositionColor(new Vector3(rect.Left, rect.Top, 0), dwColor);
            vbData[01] = new VertexPositionColor(new Vector3(Cx, rect.Top, 0), dwColor);
            vbData[02] = new VertexPositionColor(new Vector3(Cx, Cy, 0), dwColor);
            // taiangle 2                                           
            vbData[03] = new VertexPositionColor(new Vector3(rect.Left, rect.Bottom, 0), dwColor);
            vbData[04] = new VertexPositionColor(new Vector3(rect.Left, rect.Top, 0), dwColor);
            vbData[05] = new VertexPositionColor(new Vector3(Cx, Cy, 0), dwColor);
            // taiangle 3                                           
            vbData[06] = new VertexPositionColor(new Vector3(rect.Right, rect.Bottom, 0), dwColor);
            vbData[07] = new VertexPositionColor(new Vector3(rect.Left, rect.Bottom, 0), dwColor);
            vbData[08] = new VertexPositionColor(new Vector3(Cx, Cy, 0), dwColor);
            // taiangle 4                                           
            vbData[09] = new VertexPositionColor(new Vector3(rect.Right, rect.Top, 0), dwColor);
            vbData[10] = new VertexPositionColor(new Vector3(rect.Right, rect.Bottom, 0), dwColor);
            vbData[11] = new VertexPositionColor(new Vector3(Cx, Cy, 0), dwColor);
            // taiangle 5                                           
            vbData[12] = new VertexPositionColor(new Vector3(Cx, rect.Top, 0), dwColor);
            vbData[13] = new VertexPositionColor(new Vector3(rect.Right, rect.Top, 0), dwColor);
            vbData[14] = new VertexPositionColor(new Vector3(Cx, Cy, 0), dwColor);

            //Rem  计算此时秒针的向量
            Angle = Angle * Math.PI / 180;
            var a2 = Math.Atan(w2 / h2);
            if (Angle < a2)
            {
                nTriangles = 1;
                vResult.Position.Y = -h2;
                vResult.Position.X = -h2 * (Single)Math.Tan(Angle);
            }
            else if (Angle < Math.PI - a2)
            {
                Angle = Angle - Math.PI / 2;
                nTriangles = 2;
                vResult.Position.Y = w2 * (Single)Math.Tan(Angle);
                vResult.Position.X = -w2;
            }
            else if (Angle < Math.PI + a2)
            {
                nTriangles = 3;
                vResult.Position.Y = h2;
                vResult.Position.X = h2 * (Single)Math.Tan(Angle);
            }
            else if (Angle < 2 * Math.PI - a2)
            {
                Angle = Angle - Math.PI / 2;
                nTriangles = 4;
                vResult.Position.Y = -w2 * (Single)Math.Tan(Angle);
                vResult.Position.X = w2;
            }
            else
            {
                nTriangles = 5;
                vResult.Position.Y = -h2;
                vResult.Position.X = -h2 * (Single)Math.Tan(Angle);
            }
            vbData[nTriangles * 3 - 3].Position.X = Cx + vResult.Position.X;
            vbData[nTriangles * 3 - 3].Position.Y = Cy + vResult.Position.Y;
            this._basicEffect.World = Matrix.CreateOrthographicOffCenter(0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height, 0, 0, 1);
            this._basicEffect.CurrentTechnique.Passes[0].Apply();
            this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vbData, 0, nTriangles);
        }




        #region DrawLine

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coord of the first point</param>
        /// <param name="y1">The Y coord of the first point</param>
        /// <param name="x2">The X coord of the second point</param>
        /// <param name="y2">The Y coord of the second point</param>
        /// <param name="color">The color to use</param>
        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, 1.0f);
        }


        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coord of the first point</param>
        /// <param name="y1">The Y coord of the first point</param>
        /// <param name="x2">The X coord of the second point</param>
        /// <param name="y2">The Y coord of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness)
        {
            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }


        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        public void DrawLine(Vector2 point1, Vector2 point2, Color color)
        {
            DrawLine(point1, point2, color, 1.0f);
        }


        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        public void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);
            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(point1, distance, angle, color, thickness);
        }


        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point in radians</param>
        /// <param name="color">The color to use</param>
        public void DrawLine(Vector2 point, float length, float angle, Color color)
        {
            DrawLine(point, length, angle, color, 1.0f);
        }



        public void DrawLine(Vector2 point, float length, float angle, Color color, float thickness)
        {
            this.SpriteBatch.Draw(pixel, point, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        #endregion


        #region FillRectangle
        public void FillRectangle(Rectangle rect, Color color)
        {
            this.SpriteBatch.Draw(pixel, rect, color);
        }

        public void FillRectangle(Rectangle rect, Color color, float angle)
        {
            this.SpriteBatch.Draw(pixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void FillRectangle(Vector2 location, Vector2 size, Color color)
        {
            this.FillRectangle(location, size, color, 0.0f);
        }

        public void FillRectangle(Vector2 location, Vector2 size, Color color, float angle)
        {
            this.SpriteBatch.Draw(pixel, location, null, color, angle, Vector2.Zero, size, SpriteEffects.None, 0);
        }

        #endregion



        #region DrawRectangle

        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        public void DrawRectangle(Rectangle rect, Color color)
        {
            DrawRectangle(rect, color, 1.0f);
        }


        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the lines</param>
        public void DrawRectangle(Rectangle rect, Color color, float thickness)
        {

            // TODO: Handle rotations
            // TODO: Figure out the pattern for the offsets required and then handle it in the line instead of here

            DrawLine(new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness); // top
            DrawLine(new Vector2(rect.X + 1f, rect.Y), new Vector2(rect.X + 1f, rect.Bottom + thickness), color, thickness); // left
            DrawLine(new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness); // bottom
            DrawLine(new Vector2(rect.Right + 1f, rect.Y), new Vector2(rect.Right + 1f, rect.Bottom + thickness), color, thickness); // right
        }


        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        public void DrawRectangle(Vector2 location, Vector2 size, Color color)
        {
            DrawRectangle(new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, 1.0f);
        }


        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the line</param>
        public void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness)
        {
            DrawRectangle(new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, thickness);
        }

        #endregion






        /// <summary>
        /// 用颜色填充矩形
        /// </summary>
        /// <param name="rect">位置与大小</param>
        /// <param name="nColor">颜色</param>
        public void FillRectangle2(Rectangle rect, Color dwColor)
        {
            VertexPositionColor[] vbData = new VertexPositionColor[4];
            vbData[0] = new VertexPositionColor(new Vector3(rect.Left, rect.Top, 0), dwColor);
            vbData[1] = new VertexPositionColor(new Vector3(rect.Right, rect.Top, 0), dwColor);
            vbData[2] = new VertexPositionColor(new Vector3(rect.Left, rect.Bottom, 0), dwColor);
            vbData[3] = new VertexPositionColor(new Vector3(rect.Right, rect.Bottom, 0), dwColor);
            this._basicEffect.CurrentTechnique.Passes[0].Apply();
            this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vbData, 0, 2);
        }



        /// <summary>
        /// 渲染矩形线条
        /// </summary>
        /// <param name="rect">位置与大小</param>
        /// <param name="nColor">颜色</param>
        public void StrokeRectangle2(Rectangle rect, Color dwColor)
        {
            VertexPositionColor[] vbData = new VertexPositionColor[8];
            vbData[0] = new VertexPositionColor(new Vector3(rect.Left, rect.Top, 0), dwColor);
            vbData[1] = new VertexPositionColor(new Vector3(rect.Right, rect.Top, 0), dwColor);
            vbData[2] = new VertexPositionColor(new Vector3(rect.Right, rect.Top, 0), dwColor);
            vbData[3] = new VertexPositionColor(new Vector3(rect.Right, rect.Bottom, 0), dwColor);
            vbData[4] = new VertexPositionColor(new Vector3(rect.Right, rect.Bottom, 0), dwColor);
            vbData[5] = new VertexPositionColor(new Vector3(rect.Left, rect.Bottom, 0), dwColor);
            vbData[6] = new VertexPositionColor(new Vector3(rect.Left, rect.Bottom, 0), dwColor);
            vbData[7] = new VertexPositionColor(new Vector3(rect.Left, rect.Top, 0), dwColor);
            this._basicEffect.CurrentTechnique.Passes[0].Apply();
            this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vbData, 0, 4);
        }

    }
}
