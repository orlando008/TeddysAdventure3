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
        private StartMenu startMenu;

        private Boolean _started = false;
        private Boolean _levelLoaded = false;

        public TeddysAdventureGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1250;
            graphics.PreferredBackBufferHeight = 750;
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

            LoadStartMenu();

            // TODO: use this.Content to load your game content here
            //screen = new Screen(this, "basement1");
  
            //this.Components.Add(screen);

            //teddy = new Teddy(this, Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRun")), new Vector2(20, 575), new Vector2(50, 75));

            //this.Components.Add(teddy);
        }

        private void LoadStartMenu()
        {
            // Calling just in case to clear everything
            this.Components.Clear();

            //spriteBatch = new SpriteBatch(GraphicsDevice);

            startMenu = new StartMenu(this, new Vector2(0, 0), Content.Load<Texture2D>(System.IO.Path.Combine(@"Screens", "startMenuTest")));//, Content.Load<Texture2D>(System.IO.Path.Combine(@"Screens", "startMenuIcon")), new Vector2(200, 1000));
            this.Components.Add(startMenu);
        }

        // should probably changed to take a level name, possibly
        private void LoadLevel()
        {
            this.Components.Clear();
            

            _levelLoaded = false;

            screen = new Screen(this, "basement1");

            this.Components.Add(screen);

            teddy = new Teddy(this, Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRun")), new Vector2(20, 200), new Vector2(50, 75));

            this.Components.Add(teddy);

            _levelLoaded = true;
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

            if (!_started)
            {
                startMenu.Update(gameTime);
                _started = startMenu.Started;
            }
            else
            {
                if (_levelLoaded)
                {
                    if (screen.GoBackToStartScreen)
                    {
                        _started = false;
                        _levelLoaded = false;
                        startMenu.Started = false;
                        this.Content.Unload();
                        this.LoadContent();
                    }
                    else
                    {
                        screen.Update(gameTime);
                        teddy.Update(gameTime);
                    }

                }
                else
                    LoadLevel();
            }            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            if (!_started)
                startMenu.Draw(gameTime);
            else
            {
                if (_levelLoaded)
                {
                    screen.Draw(gameTime);
                    teddy.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }

    }
}
