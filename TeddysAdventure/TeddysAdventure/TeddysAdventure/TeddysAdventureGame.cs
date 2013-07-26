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
using TeddysAdventureLibrary;

namespace TeddysAdventure
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TeddysAdventureGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Teddy teddy;
        private Screen screen;

        public TeddysAdventureGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 746;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            screen = new Screen(this, new Vector2(0, 0), Content.Load<Texture2D>(System.IO.Path.Combine(@"Screens", "basementLevelStyleSheet")));
            screen.DeathSprite = Content.Load<Texture2D>(System.IO.Path.Combine(@"Screens", "deathScreen"));
            screen.Surfaces.Add(new Rectangle(0, 650, 150, 200));
            screen.Surfaces.Add(new Rectangle(150, 675, 71, 71));
            screen.Surfaces.Add(new Rectangle(221, 700, 60, 46));
            screen.Surfaces.Add(new Rectangle(280, 725, 60, 46));
            screen.Surfaces.Add(new Rectangle(419, 650, 150, 150));
            screen.Surfaces.Add(new Rectangle(361, 531, 210, 35));
            screen.Surfaces.Add(new Rectangle(657, 422, 210, 35)); 
            screen.Surfaces.Add(new Rectangle(953, 313, 210, 35));
            screen.Surfaces.Add(new Rectangle(1378, 534, 210, 35));
            screen.Surfaces.Add(new Rectangle(1465, 499, 35, 35));
            screen.Surfaces.Add(new Rectangle(1674, 425, 210, 35));
            screen.Surfaces.Add(new Rectangle(1970, 316, 210, 35));
            screen.Surfaces.Add(new Rectangle(2504, 512, 210, 35));
            screen.Surfaces.Add(new Rectangle(2800, 403, 210, 35));
            screen.Surfaces.Add(new Rectangle(3096, 294, 210, 35));
            this.Components.Add(screen);

            teddy = new Teddy(this, Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRun")), new Vector2(20, 575), new Vector2(50, 75));

            this.Components.Add(teddy);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();            
            // TODO: Add your update logic here

            teddy.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            screen.Draw(gameTime);
            teddy.Draw(gameTime);
            base.Draw(gameTime);
        }

    }
}
