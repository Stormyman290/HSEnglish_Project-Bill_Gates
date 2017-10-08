using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Beta
{
    class Projectile : Sprite
    {
        //http://www.xnadevelopment.com/tutorials/thewizard/theWizard.shtml
        public static Projectile aBullet { get; private set; }
        Vector2 _position;
        public bool Visible = false;
        Vector2 _speed;
        Vector2 _direction;
        Texture2D daTexture;
        int fighterOwnerenumber;
        int MAX_DISTANCE;
        int pain;
        
        public Projectile(Texture2D texture, Vector2 position, SpriteBatch spritebatch, int fighterOwner,int damage)
            : base(texture, position, spritebatch)
        {
            pain = damage;
            daTexture = texture;
            _position = position;
            fighterOwnerenumber = fighterOwner;
            aBullet = this;
        }

        public new void Update(GameTime theGameTime)
        {
            Position += _direction * _speed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
            if (Vector2.Distance(_position, Position) > MAX_DISTANCE || Stage.Level.HasRoomForRectangle(Bounds)==false)
            {
                Visible = false;
            }

            if (Visible == true)
            {
                setDirection(_direction);
                setSpeed(_speed);
                checkForPlayers();
                base.Update(theGameTime);
            }
        }
        public override void Draw()
        {
            if (Visible == true)
            {
                base.Draw();
            }
        }
        public void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager, "ur mum");
        }
        public void Fire(Vector2 theStartPosition, Vector2 theSpeed, Vector2 theDirection, int theMax)
        {
            MAX_DISTANCE = theMax;
            Position = theStartPosition;
            _position = theStartPosition;
            _speed = theSpeed;
            _direction = theDirection;
            Visible = true;
        }
        public int getFighterOwner()
        {
            return fighterOwnerenumber;
        }
        public void setVisibleFalse()
        {
            Visible = false;
        }
        ///<projectile vs player>
        ///
        ///<projectile vs player>

        public void checkForPlayers()
        {
            for (int x = 0; x <= Stage.Level.players.Length-1; x++)
            {
                if (Stage.Level.players[x].Bounds.Intersects(Bounds) && CompareTags(Stage.Level.players[x].getFighterTag(), fighterOwnerenumber) == false) 
                {
                    if (Stage.Level.players[x].Bounds == Stage.Level.players[3].Bounds)
                    {
                        Stage.Level.players[0].Philo += Stage.Level.players[0].Money;
                        Stage.Level.players[0].Money =0;
                    }
                    else if (Stage.Level.players[x].Bounds == Stage.Level.players[2].Bounds&&Stage.Level.players[0].Philo ==1000000)
                    {
                        Stage.Level.players[0].Rate += 100*(double)pain;
                        pain++;
                    }
                    else
                    {
                        Stage.Level.players[0].Rate += (double)pain;
                        pain++;
                    }
                    Stage.Level.players[x].setCharacterState(CharacterState.hit);
                    this.setVisibleFalse(); 
                }
            }
        }
        private bool CompareTags(int playerTag, int projectileTag)
        {
            bool comparer = false;
            if (playerTag == projectileTag) { comparer = true; }
            else if (!(playerTag == projectileTag)) { comparer = false; }
            return comparer;
        }
    }
}
