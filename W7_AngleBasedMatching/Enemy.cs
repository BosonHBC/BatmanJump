using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace W7_AngleBasedMatching
{
    class Enemy
    {
        public Enemy(int x, int y, float s) 
        {
            posX = x;
            posY = y;
            speed = s;
            box.X = posX;
            box.Y = posY;
            box.Width = 60;
            box.Height = 94;
        }

        public void MoveEnemy(int dir)
        {
            if (dir == 1) // left right
            {
                if (movingR)
                {
                    speed = Math.Abs(speed);
                }
                else
                {
                    speed = -1 * Math.Abs(speed);
                }

                if (posX > 0 && posX + 60 < 640)
                {
                    posX += (int)speed;
                }
                if(posX <= 0)
                {
                    movingR = true;
                    posX = 2;
                }
                if (posX >= 580)
                {
                    movingR = false;
                    posX = 578;
                }              
            }

            box.X = posX;
        }

        
        public int posX;
        public int posY;
        public Rect box;
        public bool dead;

        float speed;
        bool movingR = true;
    }
}
