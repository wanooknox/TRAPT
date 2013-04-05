using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace TRAPT
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Cursor : Mover//Microsoft.Xna.Framework.GameComponent
    {
        //Tracking
        Vector2 position;
        public bool cameraMode = false;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //Drawing
        Texture2D cursorImg;

        public Cursor(Game game)
            : base(game)
        {
            //TraptMain.layers[2].Add(this);
            //game.Components.Add(this);
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.DrawOrder = 9000;
            //load the cursor image
            //this.cursorImg = Game.Content.Load<Texture2D>("cursor");
            this.ChangeMouseMode("menu");

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //update the cursor position
            MouseState ms = Mouse.GetState();
            this.position.X = ms.X;
            this.position.Y = ms.Y;

            if (TraptMain.useGamePad)
            {
                gps = GamePad.GetState(PlayerIndex.One);

                this.position.X = gps.ThumbSticks.Right.X * (Game.GraphicsDevice.Viewport.Width/4) + (Game.GraphicsDevice.Viewport.Width / 2);
                this.position.Y = -gps.ThumbSticks.Right.Y * (Game.GraphicsDevice.Viewport.Height/4) + (Game.GraphicsDevice.Viewport.Height / 2);

                gpsold = gps;
            }

            if (this.cameraMode) //modify position to account for camera rendering
            {
                this.position = Vector2.Transform(this.position, Matrix.Invert(TraptMain.camera.GetViewMatrix()));
            }

            base.Update(gameTime);
        }

        public Vector2 GetMouseInWorld()
        {
            if (TraptMain.useGamePad)
            {
                return this.position;
                //    Vector2.Transform(
                //new Vector2(this.position.X, this.position.Y),
                //Matrix.Invert(TraptMain.camera.GetViewMatrix()
                //    ));
            }

            MouseState ms = Mouse.GetState();
            return Vector2.Transform(
                new Vector2(ms.X, ms.Y), 
                Matrix.Invert(TraptMain.camera.GetViewMatrix()
                    ));
        }

        public void ChangeMouseMode(string mode)
        {
            if (mode.Equals("menu"))
            {
                this.cursorImg = Game.Content.Load<Texture2D>("cursor");
                this.destination.Width = this.cursorImg.Width;
                this.destination.Height = this.cursorImg.Height;
                this.cameraMode = false;
            }
            else // play mode
            {
                this.cursorImg = Game.Content.Load<Texture2D>("crosshairs");
                this.destination.Width = this.cursorImg.Width;
                this.destination.Height = this.cursorImg.Height;
                this.cameraMode = true;
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.cameraMode)
            {
                this.destination.X = (int)Math.Round(this.position.X - this.destination.Width / 2);
                this.destination.Y = (int)Math.Round(this.position.Y - this.destination.Height / 2);
            }
            else
            {
                this.destination.X = (int)Math.Round(this.position.X);
                this.destination.Y = (int)Math.Round(this.position.Y);
            }

            //draw the cursor
            //spriteBatch.Draw(this.cursorImg, this.position, Color.White);
            spriteBatch.Draw(this.cursorImg, this.Destination, Color.White);
        }
    }
}
