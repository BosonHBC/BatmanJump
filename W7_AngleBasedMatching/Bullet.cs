using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace W7_AngleBasedMatching
{
    class Bullet
    {
        public Point startPoint;
        public bool isInit = false;
        public bool isDestroy = false;
        public static int number = 0;

        private double CposX;
        private double CposY;
        private double SposX;
        private double SposY;
        private double moveSpeed = 20;


        public Point getPosision()
        {
            Point curPoint = new Point(CposX, CposY);
            return curPoint;
        }

        public Bullet()
        {
            isInit = false;
        }

        public Bullet(Point sPoint)
        {
            startPoint = sPoint;

            SposX = sPoint.X;
            SposY = sPoint.Y;

            CposX = SposX;
            CposY = SposY;
            isInit = true;

            number++;
        }

        public void MoveBullet(Vector dir)
        {
            CposX += moveSpeed * dir.X;
            CposY += moveSpeed * dir.Y;
        }

        public void DrawBullet(DrawingContext dc, BitmapImage img)
        {
            //dc.DrawEllipse(Brushes.Red, null, new Point(CposX,CposY), 3, 3);
            dc.DrawImage(img, new Rect(CposX, CposY, 52, 25));
        }

        public int returnNumber()
        {
            return number;
        }
    }
}
