using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Bollens data
        Texture2D _ballImg;
        Vector2 _ballPos;
        Vector2 _ballDirection = new Vector2(1, 1);
        Vector2 _ballSpeed = new Vector2(11, 11);
        Rectangle _ballBB; // BoundingBox

        // Pad-data
        Texture2D _padImg;
        Vector2 _padPos1;
        Vector2 _padPos2;
        Vector2 _padSpeed = new Vector2(0, 6);
        Rectangle _padBB1; // BoundingBox
        Rectangle _padBB2;

        // Poäng
        SpriteFont _pointFont;
        int _player1Points = 0;
        int _player2Points = 0;
        Vector2 _player1PointPos = new Vector2(200, 20);
        Vector2 _player2PointPos = new Vector2(600, 20);

        int SCREEN_WIDTH = 1000;
        int SCREEN_HEIGHT = 1000;

        KeyboardState ks;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = SCREEN_WIDTH,
                PreferredBackBufferHeight = SCREEN_HEIGHT,
            };
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _ballImg = Texture2D.FromFile(GraphicsDevice, "bilder/boll.png");
            _ballPos = new Vector2(SCREEN_WIDTH / 2 - _ballImg.Width / 2, SCREEN_HEIGHT / 2 - _ballImg.Height / 2);

            _padImg = Texture2D.FromFile(GraphicsDevice, "bilder/pad.png");
            _padPos1 = new Vector2(10, SCREEN_HEIGHT / 2 - _padImg.Height / 2);
            _padPos2 = new Vector2(SCREEN_WIDTH - 10 - _padImg.Width, SCREEN_HEIGHT / 2 - _padImg.Height / 2);

            _padBB1 = new Rectangle((int)_padPos1.X, (int)_padPos1.Y, _padImg.Width, _padImg.Height);
            _padBB2 = new Rectangle((int)_padPos2.X, (int)_padPos2.Y, _padImg.Width, _padImg.Height);

            _pointFont = Content.Load<SpriteFont>("points");
        }

        protected override void Update(GameTime gameTime)
        {

            ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Escape))
                Exit();

            // Spelare 1 rörelser
            if (ks.IsKeyDown(Keys.W))
            {
                _padPos1.Y -= _padSpeed.Y;
            }
            if (ks.IsKeyDown(Keys.S))
            {
                _padPos1.Y += _padSpeed.Y;
            }

            // Spelare 2 rörelser (AI) - smooth following without invalid 'Sleep' call
            // Use Lerp to make movement smidigare istället för attempting to call a non-existent Sleep on Vector2
            _padPos2.Y = MathHelper.Lerp(_padPos2.Y, _ballPos.Y, 0.08f);

            // Begränsa paddlar till skärmen
            _padPos1.Y = Math.Clamp(_padPos1.Y, 0, SCREEN_HEIGHT - _padImg.Height);
            _padPos2.Y = Math.Clamp(_padPos2.Y, 0, SCREEN_HEIGHT - _padImg.Height);

            // Uppdatera Pad-rektanglar
            _padBB1 = new Rectangle((int)_padPos1.X, (int)_padPos1.Y, _padImg.Width, _padImg.Height);
            _padBB2 = new Rectangle((int)_padPos2.X, (int)_padPos2.Y, _padImg.Width, _padImg.Height);

            // Uppdatera bollens position
            _ballPos += _ballDirection * _ballSpeed;
            _ballBB = new Rectangle((int)_ballPos.X, (int)_ballPos.Y, _ballImg.Width, _ballImg.Height);

            // Bollens kollision med väggar
            if (_ballPos.Y <= 0 || _ballPos.Y + _ballImg.Height >= SCREEN_HEIGHT)
            {
                _ballDirection.Y = -_ballDirection.Y;
            }

            // Kollision med paddlar
            if (_padBB1.Intersects(_ballBB) || _padBB2.Intersects(_ballBB))
            {
                _ballDirection.X = -_ballDirection.X;
                _ballSpeed = new Vector2(11, 11);
            }

            // Bollens kollision med vänster och höger sida
            if (_ballPos.X < 0)
            {
                _player2Points++;
                ResetBall();
            }
            else if (_ballPos.X + _ballImg.Width > SCREEN_WIDTH)
            {
                _player1Points++;
                ResetBall();
            }

            base.Update(gameTime);
        }

        private void ResetBall()
        {
            _ballPos = new Vector2(SCREEN_WIDTH / 2 - _ballImg.Width / 2, SCREEN_HEIGHT / 2 - _ballImg.Height / 2);
            _ballDirection.X = -_ballDirection.X;
            _ballSpeed = new Vector2(5, 5);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Rita boll och paddlar
            _spriteBatch.Draw(_ballImg, _ballPos, Color.White);
            _spriteBatch.Draw(_padImg, _padPos1, Color.White);
            _spriteBatch.Draw(_padImg, _padPos2, Color.White);

            // Rita poäng
            _spriteBatch.DrawString(_pointFont, $"Player 1: {_player1Points}", _player1PointPos, Color.Red);
            _spriteBatch.DrawString(_pointFont, $"Player 2: {_player2Points}", _player2PointPos, Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
