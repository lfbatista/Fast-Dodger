using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace com.ipg.fastdoger
{
    class Explosao
    {
        private const int DURACAO_EXPLOSAO = 300; // milisegundos

        private const int IMAGENS_LINHA = 5;
        private const int IMAGENS_COLUNA = 5;

        private const int TOTAL_IMAGENS = IMAGENS_LINHA * IMAGENS_COLUNA;
        private const int DURACAO_CADA_IMAGEM = DURACAO_EXPLOSAO / TOTAL_IMAGENS;

        internal static Texture2D imagem;

        private Vector2 posicao;
        private int posImagem = 0;
        private double tinicio;
        private float escala;
        private GameTime gameTime;

        public Explosao(Vector2 posicao, float escala, GameTime gameTime)
        {
            this.posicao = new Vector2(600, 400);
            this.escala = escala;
            this.tinicio = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public void Update(GameTime gameTime)
        {
            double tactual = gameTime.TotalGameTime.TotalMilliseconds;
            double tempoDecorrido = tactual - tinicio;

            posImagem = (int)tempoDecorrido / DURACAO_CADA_IMAGEM;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Desapareceu()) return;

            int l = posImagem/ IMAGENS_COLUNA;
            int c = posImagem % IMAGENS_COLUNA;

            int alturaCadaImagem = imagem.Height / IMAGENS_LINHA;
            int larguraCadaImagem = imagem.Width / IMAGENS_COLUNA;

            Rectangle areaDesenhar = new Rectangle(c + (larguraCadaImagem), (l * alturaCadaImagem), larguraCadaImagem, alturaCadaImagem);
            // Vector2 posicaoRelativaEcran = posicao - Fundo.posicaoCamera;
            Vector2 posicaoRelativaEcran = posicao;

            spriteBatch.Draw(imagem, posicaoRelativaEcran, areaDesenhar, Color.White, 0.0f, Vector2.Zero, escala, SpriteEffects.None, 1.0f);
        }

        public bool Desapareceu()
        {
            return (posImagem >= TOTAL_IMAGENS);
        }
    }
}