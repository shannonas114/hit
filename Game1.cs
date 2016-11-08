# hit
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

namespace WindowsGame2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Model boxModel;
        private Matrix[] boxTransform;
        private Matrix boxWorld;

        private BoundingSphere sphere1;
        private BoundingSphere sphere2;
        private BoundingSphere sphere3;

        private BoundingBox boundBox;
        private BoundingBox boundBox2;
        private BoundingBox boundBox3;

        private Texture2D flame;
        private Vector2 flamePosition;

        private Vector3 cameraTargetPosition;

        private Vector3 boxPosition;
        private Vector3 cameraPosition;

        private Matrix view;
        private Matrix projection;

        DynamicVertexBuffer vertexBuffer = null;

        KeyboardState keyboardState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
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

            InitializeModel();

            InitializeCamera();

            base.Initialize();
        }

        private void InitializeModel()
        {
            boxModel = Content.Load<Model>("sikaku");

            boxTransform = new Matrix[boxModel.Bones.Count];

            boxModel.CopyAbsoluteBoneTransformsTo(boxTransform);

            boxPosition = new Vector3(0.0f, 0.0f, 0.0f);

            boxWorld = Matrix.CreateTranslation(boxPosition);

            flame = Content.Load<Texture2D>("flame");

            flamePosition = new Vector2(490.0f, 260.0f);//画面の中心
        }

        private void InitializeCamera()
        {
            float fieldOfView;
            float aspectRatio;
            float nearPlaneDistance;
            float farPlaneDistance;

            cameraPosition = new Vector3(0.0f, 0.0f, 800.0f);

            cameraTargetPosition = Vector3.Zero;

            fieldOfView = MathHelper.ToRadians(45.0f);
            aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            nearPlaneDistance = 1.0f;
            farPlaneDistance = 2000.0f;

            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
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
            sphere1 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 50.0f);
            sphere2 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 250.0f);
            sphere3 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 10.0f);

            boundBox = new BoundingBox(new Vector3(0.0f, 0.0f, 0.0f),new Vector3(0.0f,0.0f,0.0f));
            boundBox2 = new BoundingBox(new Vector3(-500.0f, 0.0f, 0.0f), new Vector3(-500.0f, 0.0f, 0.0f));
            boundBox3 = new BoundingBox(new Vector3(500.0f, 0.0f, 0.0f), new Vector3(500.0f, 0.0f, 0.0f));
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed||Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            keyboardState = Keyboard.GetState();

            #region フレームと円の移動

            if (keyboardState.IsKeyDown(Keys.A))
            {
                flamePosition.X += -3.0f;
                sphere1.Center.X += -3.0f;
                sphere2.Center.X += -3.0f;
                sphere3.Center.X += -3.0f;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                flamePosition.X += 3.0f;
                sphere1.Center.X += 3.0f;
                sphere2.Center.X += 3.0f;
                sphere3.Center.X += 3.0f;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                flamePosition.Y += -3.0f;
                sphere1.Center.X += 3.0f;
                sphere2.Center.X += 3.0f;
                sphere3.Center.X += 3.0f;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                flamePosition.Y += 3.0f;
                sphere1.Center.X += -3.0f;
                sphere2.Center.X += -3.0f;
                sphere3.Center.X += -3.0f;
            }
            #endregion

            #region ズーム
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                cameraPosition.Z = 400.0f;
            }
            if (keyboardState.IsKeyUp(Keys.LeftShift))
            {
                cameraPosition.Z = 800.0f;
            }
            #endregion

            view = Matrix.CreateLookAt(cameraPosition, cameraTargetPosition, Vector3.Up);

            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);

            boxWorld = Matrix.CreateTranslation(position);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (sphere3.Intersects(boundBox))
            {
                GraphicsDevice.Clear(Color.Pink);
            }
            else if(sphere2.Intersects(boundBox))
            {
                GraphicsDevice.Clear(Color.Green);
            }
            else if (sphere1.Intersects(boundBox))
            {
                GraphicsDevice.Clear(Color.Yellow);
            }

            if (sphere3.Intersects(boundBox2))
            {
                GraphicsDevice.Clear(Color.Pink);
            }
            else if (sphere2.Intersects(boundBox2))
            {
                GraphicsDevice.Clear(Color.Green);
            }
            else if (sphere1.Intersects(boundBox2))
            {
                GraphicsDevice.Clear(Color.Yellow);
            }

            if (sphere3.Intersects(boundBox3))
            {
                GraphicsDevice.Clear(Color.Pink);
            }
            else if (sphere2.Intersects(boundBox3))
            {
                GraphicsDevice.Clear(Color.Green);
            }
            else if (sphere1.Intersects(boundBox3))
            {
                GraphicsDevice.Clear(Color.Yellow);
            }


            // TODO: Add your drawing code here
            DrawModel(boxModel, boxTransform, boxWorld);

            spriteBatch.Begin();

            spriteBatch.Draw(flame,flamePosition,null,Color.White,0.0f,Vector2.Zero,new Vector2(0.5f,0.5f),SpriteEffects.None,0.0f);

            spriteBatch.End();

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix[] transforms, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.PreferPerPixelLighting = true;

                    effect.EnableDefaultLighting();

                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = transforms[mesh.ParentBone.Index] * world;

                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight1.Enabled = true;
                    effect.DirectionalLight2.Enabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
