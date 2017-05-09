using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace W7_AngleBasedMatching
{
    class Ground
    {        
        int iX;
        int iY;
        int iWid;
        int iHei;

        int initX;
        public int initY;
        bool movingR = true;
        bool movingT = true;
        public Rect boundingBox;

        public Ground() { }

        public Ground(int iposX, int iPosY, int iWidth, int iHeight)
        {
            iX = iposX;
            iY = iPosY;
            iWid = iWidth;
            iHei = iHeight;

            initX = iposX;
            initY = iPosY;

            boundingBox.X = iX;
            boundingBox.Y = iY;
            boundingBox.Height = iHei;
            boundingBox.Width = iWid;


        }


        public void MovingGround(int dir, int speed, int change)
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

                if (iX > 0 && iX + 170 < 640)
                {
                    iX += (int)speed;
                }
                if (iX <= 0)
                {
                    movingR = true;
                    iX = 2;
                }
                if (iX >= 470)
                {
                    movingR = false;
                    iX = 468;
                }

                boundingBox.X = iX;
                
            }
            if (dir == 2)
            {
                if (movingT)
                {
                    speed = -1* Math.Abs(speed);
                }
                else
                {
                    speed = Math.Abs(speed);
                }

                if (iY > 0 && iY + 34 < 960)
                {
                    iY += (int)speed;
                }

                if (iY +34 >= initY+change)
                {
                    movingT = true;
                    iY = initY + change - 34 -2;
                }
                if (iY <= initY-change)
                {
                    movingT = false;
                    iY = initY - change +2;
                }
                boundingBox.Y = iY;
            }
        }



    }
}
