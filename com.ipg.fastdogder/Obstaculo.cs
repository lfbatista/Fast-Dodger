using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace com.ipg.fastdoger
{
    class Obstaculo
    {
        public Vector2 Posicao;
        public Vector2 origemDaExplosao;
        public bool Visivel = true;
        public int x = 1;
        public int y = 1;


        public Obstaculo()
        {
        }

        internal Explosao Explode(GameTime gameTime)
        {
            return new Explosao(Posicao, 3, gameTime);
        }

       
    }
}

