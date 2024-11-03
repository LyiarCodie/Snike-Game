using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Snike
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D whitepixel;

        Rectangle snakeHead;
        int directionX;
        int directionY;

        Rectangle fruit;
        Rectangle topBorder;
        Rectangle levelZone;

        Random random;

        List<Rectangle> snakeBody;

        SpriteFont gameFont;
        int score = 0;

        bool gameOver = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            graphics.PreferredBackBufferWidth = 608;
            graphics.PreferredBackBufferHeight = 640;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Window.Title = "My Snake Game";

            snakeHead = new Rectangle(608 / 2, 640 / 2, 16, 16);
            topBorder = new Rectangle(0, 32, 608, 1);
            fruit = new Rectangle(96, 96, 16, 16);
            levelZone = new Rectangle(0, 32, 608, 608);
            random = new Random();

            snakeBody = new List<Rectangle>();
            snakeBody.Add(snakeHead);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            whitepixel = Content.Load<Texture2D>("whitepixel");
            gameFont = Content.Load<SpriteFont>("gameFont");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        float accumulator = 0f;
        float moveInterval = 0.4f / 4f;
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (gameOver != true)
            {
                accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                {
                    directionY = 0;
                    directionX = -1;
                }
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                {
                    directionY = 0;
                    directionX = 1;
                }
                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                {
                    directionY = -1;
                    directionX = 0;

                }
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                {
                    directionY = 1;
                    directionX = 0;
                }

                if (accumulator >= moveInterval)
                {
                    accumulator -= moveInterval;

                    // move snake taild
                    if (snakeBody.Count > 1)
                    {
                        for (int i = snakeBody.Count - 1; i > 0; i--)
                        {

                            var currentSnakePiece = snakeBody[i];
                            currentSnakePiece.X = snakeBody[i - 1].X;
                            currentSnakePiece.Y = snakeBody[i - 1].Y;
                            snakeBody[i] = currentSnakePiece;
                        }
                    }

                    // move snake head
                    var snakeHead = snakeBody[0];
                    snakeHead.X = snakeHead.X + directionX * 16;
                    snakeHead.Y = snakeHead.Y + directionY * 16;
                    snakeBody[0] = snakeHead;

                    // gameover when colliding with the level margins
                    if (snakeHead.Left <= levelZone.Left || snakeHead.Top <= levelZone.Top || 
                        snakeHead.Right >= levelZone.Right || snakeHead.Bottom >= levelZone.Bottom)
                    {
                        gameOver = true;
                    }

                    // gameover when colliding with itself
                    for (int i = 1; i < snakeBody.Count; i++)
                    {
                        if (snakeHead.Intersects(snakeBody[i]))
                        {
                            gameOver = true;
                            break;
                        }
                    }

                    // eat fruit
                    if (snakeBody[0].Intersects(fruit))
                    {
                        PutFruitInARandomPosition();

                        // grow the snake
                        snakeBody.Add(snakeHead);

                        score += 1;
                    }
                }
            }

            // reset the game
            if (gameOver == true && keyboardState.IsKeyDown(Keys.Enter))
            {
                snakeBody.Clear();
                snakeBody.Add(snakeHead);
                directionX = 0;
                directionY = 0;
                gameOver = false;
                PutFruitInARandomPosition();
                score = 0;
            }

            base.Update(gameTime);
        }

        private void PutFruitInARandomPosition()
        {
            // position the fruit to a pseudo-random location
            int minX = levelZone.Left / 16;
            int maxX = levelZone.Right / 16;
            int minY = levelZone.Top / 16;
            int maxY = levelZone.Bottom / 16;

            fruit.X = random.Next(minX, maxX) * 16;
            fruit.Y = random.Next(minY, maxY) * 16;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(whitepixel, fruit, Color.Red);

            for (int i = 0; i < snakeBody.Count; i++)
            {
                spriteBatch.Draw(whitepixel, snakeBody[i], Color.White);
            }

            spriteBatch.Draw(whitepixel, topBorder, Color.White);

            spriteBatch.DrawString(gameFont, string.Format("Score: {0}", score), new Vector2(16f, 8f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
