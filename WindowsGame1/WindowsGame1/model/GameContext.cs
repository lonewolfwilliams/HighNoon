using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Diagnostics;
using WindowsGame1.model;
using WindowsGame1.view;
using XNATweener;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameContext : Microsoft.Xna.Framework.Game
    {
        RenderTarget2D scalingBuffer;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int scaleFactor;
        public static int width;
        public static int height;

        Texture2D mainBackground;
        AnimationFactory factory;

        Effect tvShader;
        Song organDonor;

        List<IController> controllers;
        int controllerIndex = 0;
        IController CurrentController
        {
            get
            {
                return controllers[controllerIndex];
            }
        }

        //super simple DI...should really go in a model locator style pattern but I'm too lazy
        public static StateMachine gameStateMachine;
        public static MessageBus messageBus;
        public static ScoreModel scoreModel;

        public GameContext()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // TODO: Add your initialization logic here
            
        }

        void gameStateMachine_StateChanged(object sender, StateEventArgs args)
        {
            if (++controllerIndex >= controllers.Count)
            {
                controllerIndex = 0;
            }

            CurrentController.Initialise();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            scaleFactor = 4;

            //create a buffer to draw sprites into, this will be scaled up to the backbuffer to make the display pixelated as fuck.
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            width = pp.BackBufferWidth;
            height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            scalingBuffer = new RenderTarget2D(GraphicsDevice, width/scaleFactor, height/scaleFactor, false,
                                               format, pp.DepthStencilFormat, pp.MultiSampleCount,
                                               RenderTargetUsage.DiscardContents);
            
            //background view component
            mainBackground = Content.Load<Texture2D>("wildwestbackground");

            //set up our two gunslinger view components
            factory = new AnimationFactory(Content);
            messageBus = new MessageBus();
            scoreModel = new ScoreModel();
            gameStateMachine = new StateMachine();
            gameStateMachine.StateChanged += gameStateMachine_StateChanged;

            controllers = new List<IController>();
            controllers.Add( CreateTitleController() );
            controllers.Add( CreateLobbyController() );
            controllers.Add( CreateGameController() );
            controllers.Add( CreateGameOverController() );

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
            tvShader = Content.Load<Effect>("SimpleTV");
            organDonor = Content.Load<Song>("Organ_Donor_8bit_by_triple_sSs");

            MediaPlayer.Play(organDonor);
            MediaPlayer.IsRepeating = true;
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

            CurrentController.processKeyboardInput(Keyboard.GetState());
            CurrentController.update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(scalingBuffer);

            // draw sprites to half-res buffer
            GraphicsDevice.Clear(Color.DarkOrange);
            spriteBatch.Begin();
                spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
                CurrentController.Draw(spriteBatch);
            spriteBatch.End();

            //return rendertarget to back buffer
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                float totalMS = (float)gameTime.TotalGameTime.TotalMilliseconds;
                tvShader.Parameters["gameTimeInMS"].SetValue(totalMS);
                tvShader.Parameters["videoSize"].SetValue(new Vector2(width, height));
                tvShader.Parameters["textureSize"].SetValue(new Vector2(width, height));
                tvShader.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(scalingBuffer, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
            
            //GraphicsDevice
            base.Draw(gameTime);
        }

        #region helpers

        IController CreateGameController()
        {
            GunslingerViewComponent leftPlayerComponent;
            GunslingerViewComponent rightPlayerComponent;

            PlayerModel leftPlayerModel;
            PlayerModel rightPlayerModel;

            leftPlayerComponent = new GunslingerViewComponent();
            leftPlayerComponent.Initialize(factory, true);
            leftPlayerComponent.X = (width / scaleFactor / 3) * 1;
            leftPlayerComponent.Y = (height / scaleFactor / 2) + 16;

            rightPlayerComponent = new GunslingerViewComponent();
            rightPlayerComponent.Initialize(factory, false);
            rightPlayerComponent.X = (width / scaleFactor / 3) * 2;
            rightPlayerComponent.Y = (height / scaleFactor / 2) + 16;

            //now our two models
            leftPlayerModel = new PlayerModel();
            rightPlayerModel = new PlayerModel();

            SpriteFont font = Content.Load<SpriteFont>("pixelFont12");
            GameController controller = new GameController(font);
            leftPlayerModel.playerNumber = controller.addModel(leftPlayerModel);
            rightPlayerModel.playerNumber = controller.addModel(rightPlayerModel);

            controller.addViewComponent(leftPlayerComponent);
            leftPlayerComponent.playerBinding = leftPlayerModel.playerNumber;

            controller.addViewComponent(rightPlayerComponent);
            rightPlayerComponent.playerBinding = rightPlayerModel.playerNumber;

            return controller;
        }

        IController CreateLobbyController()
        {
            GunslingerViewComponent leftPlayerComponent;
            ButtonViewComponent leftButtonComponent;
            GunslingerViewComponent rightPlayerComponent;
            ButtonViewComponent rightButtonComponent;

            leftPlayerComponent = new GunslingerViewComponent();
            leftPlayerComponent.Initialize(factory, true);
            leftPlayerComponent.X = (width / scaleFactor / 3) * 1;
            leftPlayerComponent.Y = -16;

            leftButtonComponent = new ButtonViewComponent();
            leftButtonComponent.Initialise(factory, true);
            leftButtonComponent.X = (width / scaleFactor / 3) * 1;
            leftButtonComponent.Y = (height / scaleFactor / 2) + 44;

            rightPlayerComponent = new GunslingerViewComponent();
            rightPlayerComponent.Initialize(factory, false);
            rightPlayerComponent.X = (width / scaleFactor / 3) * 2;
            rightPlayerComponent.Y = -16;

            rightButtonComponent = new ButtonViewComponent();
            rightButtonComponent.Initialise(factory, false);
            rightButtonComponent.X = (width / scaleFactor / 3) * 2;
            rightButtonComponent.Y = (height / scaleFactor / 2) + 44;

            LobbyController controller = new LobbyController(   leftPlayerComponent,
                                                                rightPlayerComponent,
                                                                leftButtonComponent,
                                                                rightButtonComponent);

            return controller;
        }

        IController CreateTitleController()
        {
            SpriteFont font = Content.Load<SpriteFont>("pixelFont12");

            TitleController controller = new TitleController(font);
            return controller;
        }

        IController CreateGameOverController()
        {
            SpriteFont font = Content.Load<SpriteFont>("pixelFont12");

            GameOverController controller = new GameOverController(font);
            return controller;
        }

        #endregion

    }
}
