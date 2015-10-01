using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using com.ipg.fastdoger;

namespace com.ipg.fastdogder
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1  : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D carro;
        private Texture2D fundo;
        private Texture2D estrada;
        private Texture2D obstaculo;

        private KeyboardState posicaoAnterior;

        private Vector2 posicaoDoCarro = new Vector2(465, 525);
        private int posicaoXX = 160; // Espaço em que se move
        private int velocidade;
        private double obstaculoAparece;
        private int vidas;
        private int ultrapassagens;
        private int aceleracao;
        private double temporizador = 10;

        private int[] estradaYY = new int[2];
        private List<Obstaculo> listDeObstaculos = new List<Obstaculo>();
        private List<Explosao> explosoes = new List<Explosao>();

        private Random random = new Random();

        private SpriteFont fonte;

        SoundEffectInstance somJogo;
        SoundEffect somUltrapassagem;
        SoundEffect somAcidente;

        private enum State
        {
            MenuInicial,
            Jogo,
            Acidente,
            GameOver,
            Vitoria
        }

        private State estadoCurrente = State.MenuInicial;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(111111);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1280;
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
            // TouchPanel.EnableMouseTouchPoint = true;
            TouchPanel.EnabledGestures = GestureType.Tap;

            base.Initialize();


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            carro = Content.Load<Texture2D>("Imagens/Car");
            fundo = Content.Load<Texture2D>("Imagens/Background");
            estrada = Content.Load<Texture2D>("Imagens/Road");
            obstaculo = Content.Load<Texture2D>("Imagens/Hazard");
            Explosao.imagem = Content.Load<Texture2D>("Imagens/explosoes");

            fonte = Content.Load<SpriteFont>("MyFont");

            somJogo = Content.Load<SoundEffect>("fun").CreateInstance();
            somUltrapassagem = Content.Load<SoundEffect>("dodge");
            somAcidente = Content.Load<SoundEffect>("crash");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void InicioDoJogo()
        {
            estradaYY[0] = 0;
            estradaYY[1] = -1 * estrada.Height;

            ultrapassagens = 0;
            vidas = 3;
            velocidade = 3;
            obstaculoAparece = 1.5;
            aceleracao = 5;

            listDeObstaculos.Clear();

            estadoCurrente = State.Jogo;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState teclado = Keyboard.GetState();

            // Para sair do jogo
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || teclado.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }

            bool jogadorFezClique = false;

            if (teclado.IsKeyUp(Keys.Space))
            {
                //  jogadorFezClique = true;
            }

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesto = TouchPanel.ReadGesture();

                if (gesto.GestureType == GestureType.Tap)
                {
                    jogadorFezClique = true;

                    break;
                }
            }

            switch (estadoCurrente)
            {
                case State.MenuInicial:
                case State.Vitoria:
                case State.GameOver:
                    {
                        Temporizador(gameTime);

                        if (jogadorFezClique)
                        {
                            InicioDoJogo();
                        }
                        break;
                    }

                case State.Jogo:
                    {
                        // Mudança de vias ao tocar no ecrã
                        if (jogadorFezClique)
                        {
                            posicaoDoCarro.X += posicaoXX;
                            posicaoXX *= -1;
                            somUltrapassagem.Play();
                        }

                        AvancarDaEstrada();

                        foreach (Obstaculo umObstaculo in listDeObstaculos)
                        {
                            if (Colisoes(umObstaculo, gameTime) == true)
                            {

                                break;
                            }

                            MoveObstaculo(umObstaculo);
                        }

                        somJogo.Play();

                        AtualizaObstaculos(gameTime);
                        break;
                    }

                case State.Acidente:
                    {
                        if (jogadorFezClique)
                        {

                            listDeObstaculos.Clear();

                            estadoCurrente = State.Jogo;

                        }

                        break;
                    }
            }

            for (int i = explosoes.Count - 1; i >= 0; i--)
            {
                explosoes[i].Update(gameTime);

                if (explosoes[i].Desapareceu())
                {
                    explosoes.RemoveAt(i);
                }
            }

            //posicaoAnterior = posicaoCurrente;

            base.Update(gameTime);
        }

        private void AvancarDaEstrada()
        {
            // Fazer avançar a estrada
            for (int blocoDeEstrada = 0; blocoDeEstrada < estradaYY.Length; blocoDeEstrada++)
            {
                if (estradaYY[blocoDeEstrada] >= this.Window.ClientBounds.Height)
                {
                    int ultimoBlocoDeEstrada = blocoDeEstrada;
                    for (int nBlocosDeEstrada = 0; nBlocosDeEstrada < estradaYY.Length; nBlocosDeEstrada++)
                    {
                        if (estradaYY[nBlocosDeEstrada] < estradaYY[ultimoBlocoDeEstrada])
                        {
                            ultimoBlocoDeEstrada = nBlocosDeEstrada;
                        }
                    }
                    estradaYY[blocoDeEstrada] = estradaYY[ultimoBlocoDeEstrada] - estrada.Height;
                }
            }

            for (int blocoDeEstrada = 0; blocoDeEstrada < estradaYY.Length; blocoDeEstrada++)
            {
                estradaYY[blocoDeEstrada] += velocidade;
            }
        }

        private void MoveObstaculo(Obstaculo oObstaculo)
        {
            oObstaculo.Posicao.Y += velocidade;
            if (oObstaculo.Posicao.Y > graphics.GraphicsDevice.Viewport.Height && oObstaculo.Visivel == true)
            {
                oObstaculo.Visivel = false;
                ultrapassagens += 1;

                if (ultrapassagens >= 30)
                {
                    estadoCurrente = State.Vitoria;
                    temporizador = 10;
                }

                aceleracao -= 1;
                if (aceleracao < 0)
                {
                    aceleracao = 5;
                    velocidade += 1;
                }
            }
        }

        private void AtualizaObstaculos(GameTime theGameTime)
        {
            obstaculoAparece -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (obstaculoAparece < 0)
            {
                int aParteInferior = 24 - (velocidade * 2);
                int aParteSuperior = 30 - (velocidade * 2);

                if (velocidade > 10)
                {
                    aParteInferior = 6;
                    aParteSuperior = 8;
                }

                obstaculoAparece = (double)random.Next(aParteInferior, aParteSuperior) / 10;
                AdicionaObstaculos();
            }
        }

        private void AdicionaObstaculos()
        {
            int posicaoNaEstrada = random.Next(1, 3);
            int umaPosicao = 275;
            if (posicaoNaEstrada == 2)
            {
                umaPosicao = 440;
            }

            bool adicionaNovoObstaculo = true;
            foreach (Obstaculo umObstaculo in listDeObstaculos)
            {
                if (umObstaculo.Visivel == false)
                {
                    adicionaNovoObstaculo = false;
                    umObstaculo.Visivel = true;
                    umObstaculo.Posicao = new Vector2(umaPosicao, -obstaculo.Height);
                    break;
                }
            }

            if (adicionaNovoObstaculo == true)
            {
                // Adicionar novo obstáculo
                Obstaculo umObstaculo = new Obstaculo();
                umObstaculo.Posicao = new Vector2(umaPosicao, -obstaculo.Height);

                listDeObstaculos.Add(umObstaculo);
            }
        }

        // Colisão
        private bool Colisoes(Obstaculo oObstaculo, GameTime gameTime)
        {

            BoundingBox areaDaCaixa = new BoundingBox(new Vector3(oObstaculo.Posicao.X, oObstaculo.Posicao.Y, 0), new Vector3(oObstaculo.Posicao.X + (obstaculo.Width * 0.4f), oObstaculo.Posicao.Y + ((obstaculo.Height - 50) * .4f), 0));
            BoundingBox areaDaCarro = new BoundingBox(new Vector3(posicaoDoCarro.X, posicaoDoCarro.Y, 0), new Vector3(posicaoDoCarro.X + (carro.Width * .2f), posicaoDoCarro.Y + (carro.Height * .2f), 0));

            if (areaDaCaixa.Intersects(areaDaCarro) == true)
            {
                somAcidente.Play();
                explosoes.Add(oObstaculo.Explode(gameTime));
                estadoCurrente = State.Acidente;
                vidas -= 1;
                if (vidas < 0)
                {
                    estadoCurrente = State.GameOver;
                    temporizador = 10;
                }
                return true;
            }
            return false;
        }

        private void Temporizador(GameTime theGameTime)
        {
            temporizador -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (temporizador < 0)
            {
                this.Exit();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(fundo, new Rectangle(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);

            switch (estadoCurrente)
            {
                case State.MenuInicial:
                    {
                        // Texto do menu inicial
                        DesenhaTextoAoCentro("Faz o maior numero de ultrapassegens que conseguires!", 200);
                        DesenhaTextoAoCentro("Toca para iniciar", 260);
                        DesenhaTextoAoCentro("ou sai em " + ((int)temporizador).ToString() + " segundos", 400);

                        break;
                    }

                default:
                    {
                        DesenhaEstrada();
                        DesenhaObstaculos();

                        foreach (Explosao e in explosoes)
                        {
                            e.Draw(spriteBatch, gameTime);
                        }


                        spriteBatch.Draw(carro, posicaoDoCarro, new Rectangle(0, 0, carro.Width, carro.Height), Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);

                        spriteBatch.DrawString(fonte, "", new Vector2(5, 425), Color.Brown, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        for (int i = 0; i < vidas; i++)
                        {
                            spriteBatch.Draw(carro, new Vector2(10 + (25 * i), 650), new Rectangle(0, 0, carro.Width, carro.Height), Color.White, 0, new Vector2(0, 0), 0.05f, SpriteEffects.None, 0);
                        }

                        spriteBatch.DrawString(fonte, ultrapassagens.ToString() + " ultrapassagens", new Vector2(15, 25), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        if (estadoCurrente == State.Acidente)
                        {
                            //DesenhaAreaDeTexto();
                            DesenhaTextoAoCentro("Pumba!", 200);
                            DesenhaTextoAoCentro("Toca para continuar", 260);
                        }
                        else if (estadoCurrente == State.GameOver)
                        {
                            DesenhaAreaDeTexto();

                            DesenhaTextoAoCentro("GAME OVER", 200);
                            DesenhaTextoAoCentro("Toca para jogar de novo", 260);
                            DesenhaTextoAoCentro("ou sai em " + ((int)temporizador).ToString() + " segundos", 400);

                        }
                        else if (estadoCurrente == State.Vitoria)
                        {
                            DesenhaAreaDeTexto();

                            DesenhaTextoAoCentro("Venceste!", 200);
                            DesenhaTextoAoCentro("Toca para jogar de novo", 260);
                            DesenhaTextoAoCentro("ou sai em " + ((int)temporizador).ToString() + " segundos", 400);
                        }

                        break;
                    }
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DesenhaEstrada()
        {
            for (int i = 0; i < estradaYY.Length; i++)
            {
                if (estradaYY[i] > estrada.Height * -1 && estradaYY[i] <= this.Window.ClientBounds.Height)
                {
                    spriteBatch.Draw(estrada, new Rectangle((int)((this.Window.ClientBounds.Width - estrada.Width) / 2), estradaYY[i], estrada.Width, estrada.Height), Color.White);
                }
            }
        }

        private void DesenhaObstaculos()
        {
            foreach (Obstaculo umObstaculo in listDeObstaculos)
            {
                if (umObstaculo.Visivel == true)
                {
                    spriteBatch.Draw(obstaculo, umObstaculo.Posicao, new Rectangle(0, 0, obstaculo.Width, obstaculo.Height), Color.White, 0, new Vector2(-420, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }

        private void DesenhaAreaDeTexto()
        {
            int posicaoX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (450 / 2));
            spriteBatch.Draw(fundo, new Rectangle(posicaoX, 75, 450, 400), Color.White);
        }

        private void DesenhaTextoAoCentro(string texto, int posicaoY)
        {
            Vector2 aSize = fonte.MeasureString(texto);
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (aSize.X / 2));

            spriteBatch.DrawString(fonte, texto, new Vector2(aPositionX, posicaoY), Color.Beige, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(fonte, texto, new Vector2(aPositionX + 1, posicaoY + 1), Color.Brown, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
    }
}
