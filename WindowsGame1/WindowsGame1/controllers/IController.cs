using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    interface IController
    {
        void Initialise();
        void processKeyboardInput(KeyboardState currentKeyboardState);
        void update(GameTime withGameTime);
        void Draw(SpriteBatch spritebatch);
    }
}
