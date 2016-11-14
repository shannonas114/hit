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

namespace WindowsGame2{
	/// <summary>
	/// This is the main type for your game
	/// </summary> 
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics; 
		SpriteBatch spriteBatch;
		float a; 
		float b;
		
        	private Model yureiModel;//幽霊
		private Matrix[] yureiTransform; 
		private Matrix yureiWorld;
    
    		private BoundingSphere sphere1;//円(大）  
		private BoundingSphere sphere2;//円(中)    
		private BoundingSphere sphere3;//円(小)
		
        	private Ray ray1; //中心   
		private Vector2 rayPosition = new Vector2(640.0f, 350.0f);
		
        	private Texture2D flame;　　　　　　　　//フレーム  
	     	private Vector2 flamePosition;          //フレームの場所    
		private Vector2 flameCenterPosition;    //フレームの中心座標
  
		private Vector3 cameraTargetPosition;//カメラ    
		private bool cameraSize = false;
		private Vector3 cameraPosition; 
		private Vector3 yureiPosition;
		
        	private Matrix view; 
		private Matrix projection;
 
 		int zoom = 0;//ズーム切り替え用
		
     		DynamicVertexBuffer vertexBuffer = null;
        	KeyboardState keyboardState;
		
        	public Game1() 
		{
			graphics = new GraphicsDeviceManager(this);         
			Content.RootDirectory = "Content";
			
            		//画面サイズ  
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
			yureiModel = Content.Load<Model>("yuurei");//幽霊   
			yureiTransform = new Matrix[yureiModel.Bones.Count];  
			yureiModel.CopyAbsoluteBoneTransformsTo(yureiTransform);   
			yureiPosition = new Vector3(0.0f, 0.0f, 0.0f);    
			yureiWorld = Matrix.CreateTranslation(yureiPosition);
			
           	 	flame = Content.Load<Texture2D>("flame");//フレーム  
			flameCenterPosition = new Vector2(flame.Width / 2, flame.Height / 2);   //フレームの中心座標    
			flamePosition = new Vector2(758.0f, 420.0f);//画面の中心      
		}
		#endregion
        	#region カメラ 
		private void InitializeCamera() 
		{
			float fieldOfView; 
			float aspectRatio;   
			float nearPlaneDistance;    
			float farPlaneDistance;
            		cameraPosition = new Vector3(0.0f, 0.0f, 800.0f);
			
            		cameraTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
            		fieldOfView = MathHelper.ToRadians(45.0f);   
			aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;  
			nearPlaneDistance = 1.0f;   
			farPlaneDistance = 2000.0f;
            		projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);      
		}  
		#endregion
        	/// <summary>
		/// LoadContent will be called once per game and is the place to load  
		/// all of your content.        
		/// </summary>    
		protected override void LoadContent()     
		{
			// Create a new SpriteBatch, which can be used to draw textures. 
			spriteBatch = new SpriteBatch(GraphicsDevice);
			// TODO: use this.Content to load your game content here
			
            		#region 当たり判定     
			sphere1 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 110.0f);//円(大)     
			sphere2 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 75.0f);//円(中)    
			sphere3 = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 25.0f);//円(小) 
			#endregion 
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
						rayPosition.X += -5.0f;  
					}
			}
			if (flamePosition.X <= 1500.0f) 
			{
				if (keyboardState.IsKeyDown(Keys.D)) 
				{
					flamePosition.X += 5.0f;  
					rayPosition.X += 5.0f;
				}
			}
			if (flamePosition.Y >= -10.0f) 
			{
				if (keyboardState.IsKeyDown(Keys.W))
				{
					flamePosition.Y += -5.0f;
					rayPosition.Y += -5.0f;
				}
			}
			if (flamePosition.Y <= 830.0f)
			{
				if (keyboardState.IsKeyDown(Keys.S))
				{
					flamePosition.Y += 5.0f; 
					rayPosition.Y += 5.0f;
				}
	 	         }
			 #endregion
           	 	#region カメラワーク(仮)   
			if (keyboardState.IsKeyDown(Keys.Left))
			{
				cameraPosition.X -= 5.0f; 
				cameraTargetPosition.X -= 5.0f;
			}
			if (keyboardState.IsKeyDown(Keys.Right))  
			{
				cameraPosition.X += 5.0f;
				cameraTargetPosition.X += 5.0f;    
			}
			if (keyboardState.IsKeyDown(Keys.Up))
			{
				cameraPosition.Y += 5.0f;   
				cameraTargetPosition.Y += 5.0f;   
			}
			if (keyboardState.IsKeyDown(Keys.Down))  
			{
				cameraPosition.Y -= 5.0f; 
				cameraTargetPosition.Y -= 5.0f;  
			}
			if (keyboardState.IsKeyDown(Keys.J))
			{
				a += 5.0f;
			}
			if (keyboardState.IsKeyDown(Keys.L))  
          		{
				a -= 5.0f; 
			}
			if (keyboardState.IsKeyDown(Keys.I))
			{
				b += 5.0f;  
			}
			if (keyboardState.IsKeyDown(Keys.K)) 
			{
				b -= 5.0f;
			}
			if (keyboardState.IsKeyDown(Keys.U)) 
			{
				cameraPosition.Z -= 5.0f;  
			}
			if (keyboardState.IsKeyDown(Keys.O))   
			{
				cameraPosition.Z += 5.0f;  
			}
			#endregion
            		#region ズーム
			if (zoom==0)  
			{
				if (keyboardState.IsKeyDown(Keys.LeftShift))
				{
					cameraPosition.Z -= 400.0f; 
					zoom = 1;
					
                    			cameraSize = true;
				}
			}
			if (zoom==1) 
			{
				if (keyboardState.IsKeyUp(Keys.LeftShift)) 
				{
					zoom = 2;
				}
			}
			if (zoom == 2)  
			{
				if (keyboardState.IsKeyDown(Keys.LeftShift))
				{
					cameraPosition.Z += 400.0f; 
					zoom = 3;
                    			
					cameraSize = false;
				}
			}
			if (zoom == 3)
			{
				if (keyboardState.IsKeyUp(Keys.LeftShift))   
				{
					zoom = 0;
				}
			}
            		#endregion
			
            		#region レイ座標     
			Viewport viewport = GraphicsDevice.Viewport; 
			Vector3 screenPosition = new Vector3(rayPosition, 1.0f); 
			Vector3 worldPoint = viewport.Unproject(screenPosition, projection, view, Matrix.Identity);
            
	    		ray1 = new Ray(new Vector3(0.0f,0.0f,800.0f), Vector3.Normalize(worldPoint - cameraPosition));    
			#endregion
            
	    		view = Matrix.CreateLookAt(cameraPosition, cameraTargetPosition, Vector3.Up);//カメラ
            		base.Update(gameTime); 
		}
        	/// <summary>   
		/// This is called when the game should draw itself.   
		/// </summary> 
		/// <param name="gameTime">Provides a snapshot of timing values.</param> 
		protected override void Draw(GameTime gameTime)     
		{
			GraphicsDevice.Clear(Color.Violet);    
			if(ray1.Intersects(sphere3)!=null) 
			{
				GraphicsDevice.Clear(Color.Green); 
			}
			else if (ray1.Intersects(sphere2) != null)    
			{
				GraphicsDevice.Clear(Color.Yellow);   
			} 
			else if(ray1.Intersects(sphere1)!=null)  
			{
				GraphicsDevice.Clear(Color.Red);   
			}
			
            		// TODO: Add your drawing code here  
			//DrawBoxModel(boxModel, boxTransform, boxWorld);     
			DrawYureiModel(yureiModel, yureiTransform, yureiWorld);
            
	    		spriteBatch.Begin();
 
 			if (!cameraSize)
				spriteBatch.Draw(flame, flamePosition, null, Color.White, 0.0f, flamePosition, new Vector2(0.25f, 0.25f), SpriteEffects.None, 0.0f);
            
	    		if (cameraSize)  
				spriteBatch.Draw(flame, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, new Vector2(2.15f, 1.8f), SpriteEffects.None, 0.0f);
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
					//effect.PreferPerPixelLighting = true;
					effect.EnableDefaultLighting();
 
 					effect.View = view;  
					effect.Projection = projection;  
					effect.World = transforms[mesh.ParentBone.Index] * world  
					* Matrix.CreateScale(10.0f)//お化けのサイズ：10.0f    
					* Matrix.CreateRotationX(MathHelper.ToRadians(b)) 
					* Matrix.CreateRotationY(MathHelper.ToRadians(a))   
					* Matrix.CreateRotationZ(MathHelper.ToRadians(0.0f));
                    
		    			effect.DirectionalLight0.Enabled = true;    
					effect.DirectionalLight1.Enabled = true;    
					effect.DirectionalLight2.Enabled = true;  
				}
				mesh.Draw();    
			}
		}
    }
}
