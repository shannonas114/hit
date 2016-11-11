# hit
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
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
		private int boxPositionCount = 0;
		private Matrix transform;
		private Vector3 scale; 
		private Model yureiModel;    
		private Matrix[] yureiTransform;   
		private Matrix yureiWorld;   
		private Vector3 rotation;      
		private float boxsize = 100.0f;    
		private BoundingBox boundBox;      
		private BoundingSphere sphere1;    
		private BoundingSphere sphere2;    
		private Ray ray1; //中心      
		private Texture2D flame;     
		private Vector2 flamePosition;     //フレームの場所     
		private Vector2 flameCenterPosition;    //フレームの中心座標   
		private Vector3 cameraTargetPosition;     
		private Vector3 boxPosition;     
		private Vector3 cameraPosition;     
		private Vector3 yureiPosition;    
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
		/// related content.  Calling base.Initialize will enumerate through any components  
		/// and initialize them as well.     
		/// </summary>  
		
		protected override void Initialize()     
		{            
			// TODO: Add your initialization logic here 
			InitializeModel();     
			InitializeCamera();   
			
			base.Initialize();  
			}
		#region モデルの読み込み      
		private void InitializeModel() 
		{
		//boxModel = Content.Load<Model>("sikaku");    
		//boxTransform = new Matrix[boxModel.Bones.Count];   
		//boxModel.CopyAbsoluteBoneTransformsTo(boxTransform);  
		//boxPosition = new Vector3(0.0f, 0.0f, 0.0f);     
		//boxWorld = Matrix.CreateTranslation(boxPosition);     
		yureiModel = Content.Load<Model>("yurei FBXban");    
		yureiTransform = new Matrix[yureiModel.Bones.Count];   
		yureiModel.CopyAbsoluteBoneTransformsTo(yureiTransform);    
		yureiPosition = new Vector3(0.0f, 0.0f, 0.0f);    
		yureiWorld = Matrix.CreateTranslation(yureiPosition);   
		flame = Content.Load<Texture2D>("flame");    
		flameCenterPosition = new Vector2(flame.Width / 2, flame.Height / 2);   //フレームの中心座標  
		flamePosition = new Vector2(490.0f, 260.0f);//画面の中心 
		}
		#endregion    
		#region カメラ 
		private void InitializeCamera()    
		{        
		float fieldOfView; 
		float aspectRatio;  
		float nearPlaneDistance;
		float farPlaneDistance;   
		cameraPosition = new Vector3(0.0f, 0.0f, 400.0f);     
		cameraTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);  
		fieldOfView = MathHelper.ToRadians(45.0f);  
		aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;       
		nearPlaneDistance = 1.0f;   
		farPlaneDistance = 2000.0f;   
		projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);      
		} 
		#endregion   
		/// <summary>   
		/// LoadContent will be called once per game and is the place to load        /// all of your content.        
		/// </summary>
		protected override void LoadContent()    
		{
		// Create a new SpriteBatch, which can be used to draw textures.          
		spriteBatch = new SpriteBatch(GraphicsDevice);    
		// TODO: use this.Content to load your game content here     
		//boundBox = new BoundingBox();         
		//boundBox.Max = new Vector3(100.0f,250.0f,100.0f)+boxPosition;   
		//boundBox.Min = new Vector3(-100.0f,0.0f,-100.0f)+boxPosition;    
		sphere1 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 200.0f);   
		sphere2 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 50.0f);   
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
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))              
		this.Exit();           
		// TODO: Add your update logic here   
		keyboardState = Keyboard.GetState();        
		#region フレームと円の移動        
		if (flamePosition.X >= -15.0f) 
		{            
		if (keyboardState.IsKeyDown(Keys.A))         
		{             
		flamePosition.X += -5.0f;   
		} 
		}
		if (flamePosition.X <= 1000.0f)    
		{
		if (keyboardState.IsKeyDown(Keys.D))
		{
		flamePosition.X += 5.0f;    
		}
		}
		if (flamePosition.Y >= -15.0f)
		{
		if (keyboardState.IsKeyDown(Keys.W))  
		{
		flamePosition.Y += -5.0f;
		}  
		}
		if (flamePosition.Y <= 530.0f) 
		{
		if (keyboardState.IsKeyDown(Keys.S)) 
		{    
		flamePosition.Y += 5.0f;  
		}     
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
		//#region ボックスの移動     
		//if (boxPositionCount == 0 && boxPosition.X >= 400.0f)  
		//{
		//    boxPositionCount = 1;  
		//}
		//else if (boxPositionCount == 1 && boxPosition.X <= -400.0f)    
		//{      
		//    boxPositionCount = 0;  
		//}        
		//if (boxPosition.X <= 400.0f && boxPositionCount == 0)   
		//{ 
		//    boxPosition.X += 1.0f;   
		//    boundBox.Max += new Vector3(1.0f, 0.0f, 0.0f);  
		//    boundBox.Min += new Vector3(1.0f, 0.0f, 0.0f);  
		//}     
		//else if (boxPosition.X >= -400.0f && boxPositionCount == 1)    
		//{         
		//  boxPosition.X -= 1.0f;    
		//    boundBox.Max -= new Vector3(1.0f, 0.0f, 0.0f);  
		//    boundBox.Min -= new Vector3(1.0f, 0.0f, 0.0f);   
		//}    
		//#endregion    
		view = Matrix.CreateLookAt(cameraPosition, cameraTargetPosition, Vector3.Up);   
		boxWorld = Matrix.CreateTranslation(boxPosition);   
		Viewport viewport = GraphicsDevice.Viewport;   
		Vector3 screenPosition = new Vector3(flamePosition, 1.0f);  
		Vector3 worldPoint = viewport.Unproject(screenPosition, projection, view, Matrix.Identity);     
		ray1 = new Ray(cameraPosition, Vector3.Normalize(worldPoint - cameraPosition));    
		transform *= Matrix.CreateRotationY(180.0f);   
		transform *= Matrix.CreateScale(10.0f);  
		transform *= Matrix.CreateTranslation(Vector3.Zero);  
		base.Update(gameTime);       
		}  
		/// <summary>   
		/// This is called when the game should draw itself.  
		/// </summary>   
		/// <param name="gameTime">Provides a snapshot of timing values.</param>     
		protected override void Draw(GameTime gameTime)      
		{    
		GraphicsDevice.Clear(Color.CornflowerBlue);  
		if(ray1.Intersects(sphere1)!=null)    
		{   
		GraphicsDevice.Clear(Color.Yellow);    
		}    
		if (ray1.Intersects(sphere2) != null)    
		{
		GraphicsDevice.Clear(Color.Green);  
		}   
		// TODO: Add your drawing code here  
		//DrawBoxModel(boxModel, boxTransform, boxWorld);     
		DrawYureiModel(yureiModel, yureiTransform, yureiWorld);   
		spriteBatch.Begin();  
		spriteBatch.Draw(flame, flamePosition, null, Color.White, 0.0f, flameCenterPosition, new Vector2(0.25f, 0.25f), SpriteEffects.None, 0.0f); 
		spriteBatch.End(); 
		GraphicsDevice.SetVertexBuffer(vertexBuffer);   
		base.Draw(gameTime);     
		} 
		private void DrawYureiModel(Model model, Matrix[] transforms, Matrix world)
		{
		foreach (ModelMesh mesh in model.Meshes)   
		{ 
		foreach (BasicEffect effect in mesh.Effects)   
		{
		effect.PreferPerPixelLighting = true;      
		effect.EnableDefaultLighting();   
		effect.View = view; 
		effect.Projection = projection;      
		effect.World = transforms[mesh.ParentBone.Index] * world            
		* Matrix.CreateScale(20.0f)
		* Matrix.CreateRotationX(MathHelper.ToRadians(0.0f))    
		* Matrix.CreateRotationY(MathHelper.ToRadians(0.0f))   
		* Matrix.CreateRotationZ(MathHelper.ToRadians(0.0f));    
		effect.DirectionalLight0.Enabled = true; 
		effect.DirectionalLight1.Enabled = true;  
		effect.DirectionalLight2.Enabled = true; 
		}           
		mesh.Draw();    
		}
		}    
		
