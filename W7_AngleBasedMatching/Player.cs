using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace W7_AngleBasedMatching
{
    class Player
    {
        public Player(int x, int y, int size)
        {
            initX = x;
            initY = y;

            posX = x;
            posY = y;

            box.X = x;
            box.Y = y;
            box.Height = size;
            box.Width = size;


        }

        public int posX;
        public int posY;
        public Rect box;
        public bool faceLeft = true;

        public bool isGrounded = true;
        public bool isFlying = false;
        public bool isFloating = false;
        public bool isJumping = false;
        public bool isTopped = false;

        public float vY = 0;
        public float vX = 0;
        public float v0 = 0;

        int initX;
        int initY;
    }
}
