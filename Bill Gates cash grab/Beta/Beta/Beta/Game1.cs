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

namespace Beta
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public struct Var
        {
            public static Rectangle MenueScreen =
            new Rectangle(0, 0, 1200, 800);
            public static Rectangle WholeWindowRectangle =
            new Rectangle(0, 0, 800, 800);

            public static Rectangle GameRec = new Rectangle(
                0, 0, WholeWindowRectangle.Width, WholeWindowRectangle.Height - 300);

            public enum CurrentWindow { SplashScreen, Menue, Game }
            public static CurrentWindow Currentwindow = CurrentWindow.SplashScreen;

            public struct Input
            {
                public static KeyboardState Prev_Key, New_Key;
                public static MouseState Prev_Mouse, new_Mose;
            }
            public struct CharacterSize
            {
                public const int Width = 100;
                public const int Height = 100;
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ContentManager content;
        Texture2D codes, ibm, microsoft, boxSprite, gatesSprite, billMelindaSprite, hardPlatforms, backGroundTex, hazardTexture, prey;
        Random rnd = new Random();
        Stage gameStage;
        Dictionary<String, Rectangle> seasonPlatforms;
        Hazard[] hazards = new Hazard[4];
        GameCharacter[] players = new GameCharacter[4];
        Background background;
        Season[] seasons = new Season[4];
        Season currentSeason;
        KeyboardState kb;
        KeyboardState oldKB;
        Song[] songs = new Song[2];
        SpriteFont pauseFont;
        SpriteFont winFont;

        int repeat = 1997;
        bool isGamePaused;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = new ContentManager(Services);
            graphics.IsFullScreen = false;
            //graphics.PreferredBackBufferHeight = 800;
            //graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = Var.MenueScreen.Width;
            graphics.PreferredBackBufferHeight = Var.MenueScreen.Height;

            graphics.PreferredBackBufferWidth = Var.WholeWindowRectangle.Width;
            graphics.PreferredBackBufferHeight = Var.WholeWindowRectangle.Height;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            isGamePaused = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            loadSongs();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hazardTexture = this.Content.Load<Texture2D>("ElementalObstructions");
            createSeasons(hazardTexture);
            for (int k = 3; k <= 9; k++)
            {
                Other.Functions.Load_Textures(
                   ref Menu.Splash.Texture,
                   Content.Load<Texture2D>("Menue/SplashArt_" + k.ToString()));
            }
            LoadOtherAnimations();
            
            Menu.Splash.ShowSplashScreen();


            backGroundTex = this.Content.Load<Texture2D>("4ElementsSeasonsBackgroundFinal copy");
            background = new Background(currentSeason, 25, 25, backGroundTex, spriteBatch);

            pauseFont = this.Content.Load<SpriteFont>("SpriteFont1");
            winFont = this.Content.Load<SpriteFont>("SpriteFont2");
            boxSprite = this.Content.Load<Texture2D>("Characters/Testbox/Square");
            gatesSprite = this.Content.Load<Texture2D>("Bill-Gates-Microsoft-52");
            billMelindaSprite = this.Content.Load<Texture2D>("billMelinda");
            microsoft = this.Content.Load<Texture2D>("microsoft_logo_1");
            ibm = this.Content.Load<Texture2D>("IBM");
            codes = this.Content.Load<Texture2D>("codes");
            //(texture , Position , spritebatch , fighter number, content manager, projectile sprite)
            players[0] = new GameCharacter(gatesSprite, new Vector2(100, 19*32), spriteBatch, 1, 10, 5, this.Content,codes);
            players[1] = new GameCharacter(ibm, new Vector2(32 * 19, 16 * 32), spriteBatch, 2, 0, 0, this.Content, boxSprite);
            players[2] = new GameCharacter(microsoft, new Vector2(32 * 19, 13 * 32), spriteBatch, 3, 0, 0, this.Content, boxSprite);
            players[3] = new GameCharacter(billMelindaSprite, new Vector2(32 * 19, 19 * 32), spriteBatch, 4, 0, 0, this.Content, boxSprite);

            hardPlatforms = this.Content.Load<Texture2D>("Platforms");

            initializeSeasonPlatforms(hardPlatforms);

            gameStage = new Stage(players, background, seasonPlatforms, seasons, hardPlatforms, spriteBatch);

            if (!Menu.Splash.isSplasingNow())
            {
                MediaPlayer.Play(songs[0]);
            }
            // TODO: use this.Content to load your game content here
        }
       
        private static void LoadOtherAnimations()
        {
            //splash screen loop 
            Animation.Animator_Controller.LoadAnimator(
                Menu.Splash.Texture,
                0, 0,
                Var.WholeWindowRectangle.Width,
                Var.WholeWindowRectangle.Height,
                Var.CurrentWindow.SplashScreen,
                85, Animation.Animator_Controller.AnimationType.Normal,
                null,
                Player.Player_Manager.PlayerState.NULL,
                Animation.Animator_Controller.OtherAnimations_enum.SplashScreen);
        }

        private void loadSongs()
        {
            Random tempRand = new Random();

            songs[0] = this.Content.Load<Song>("GatesLetter");
        }

        private void createSeasons(Texture2D hazardTexture)
        {
            createSeasonHazards(hazardTexture);

            seasonPlatforms = new Dictionary<String, Rectangle>();

            seasons[0] = new Season("Spring", 6, 384, hazards[0]);
            seasons[1] = new Season("Summer", 4, 128, hazards[1]);
            seasons[2] = new Season("Fall", 4, 0, hazards[2]);
            seasons[3] = new Season("Winter", 4, 256, hazards[3]);
            currentSeason = seasons[rnd.Next(2,2)];
        }

        private void createSeasonHazards(Texture2D hazardTexture)
        {
            for (int i = 0; i < 4; i++)
            {
                hazards[i] = new Hazard(hazardTexture, new Rectangle(32 * i, 0, 32, 32));
            }
        }

        private void initializeSeasonPlatforms(Texture2D hardPlatforms)
        {
            String[] names = new String[4];
            names[0] = "Spring";
            names[1] = "Summer";
            names[2] = "Fall";
            names[3] = "Winter";
            for (int i = 0; i < 4; i++)
            {
                seasonPlatforms.Add(names[i], new Rectangle(i * 32, 0, 32, 32));
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape) == true)
                this.Exit();

            if (kb.IsKeyDown(Keys.Tab) && !oldKB.IsKeyDown(Keys.Tab))
                isGamePaused = !isGamePaused;
            if (isGamePaused == true || Stage.Level.charactersDead >= 3)
            {

            }
            else if (isGamePaused == false)
            {
                Animation.Animator_Controller.UpdateAll(gameTime);

                Var.Input.Prev_Key = Var.Input.New_Key;
                Var.Input.New_Key = Keyboard.GetState();

                Var.Input.Prev_Mouse = Var.Input.new_Mose;
                Var.Input.new_Mose = Mouse.GetState();

                Animation.Animator_Controller.UpdateAll(gameTime);
                Menu.Splash.Update(Var.Input.New_Key, graphics, songs);

                //makes the walking animation for cyberwulf draw
                //Player.Player_Manager.Update(Var.Input.Prev_Key, Var.Input.New_Key);
                gameStage.update(gameTime);
            }

            if (songs[0].Duration.TotalMilliseconds - 30603 <= repeat && !Menu.Splash.isSplasingNow())
            {
                MediaPlayer.Play(songs[0]);
                repeat = 0;
            }
            repeat++;
            // TODO: Add your update logic here
            oldKB = kb;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            gameStage.draw(spriteBatch, gameTime);
            Animation.Animator_Controller.DrawAll(spriteBatch);
            if (Stage.Level.charactersDead < 3)
                if (isGamePaused == true)
                    spriteBatch.DrawString(pauseFont, "Paused", new Vector2(16, 4), Color.White);
            if (Stage.Level.players[0].Philo == 1000000000)
                spriteBatch.DrawString(winFont, "    THE\nAMERICAN DREAM\n   BABY", new Vector2(15, 180), Color.Gray);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
