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

using Microsoft.Kinect;
using System.Timers;
using System.Media;

namespace W7_AngleBasedMatching
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;

        private Skeleton[] skeletons = null;
        private JointType[] bones = { 
                                      // torso 
                                      JointType.Head, JointType.ShoulderCenter,
                                      JointType.ShoulderCenter, JointType.ShoulderLeft,
                                      JointType.ShoulderCenter, JointType.ShoulderRight,
                                      JointType.ShoulderCenter, JointType.Spine, 
                                      JointType.Spine, JointType.HipCenter,
                                      JointType.HipCenter, JointType.HipLeft, 
                                      JointType.HipCenter, JointType.HipRight,
                                      // left arm 
                                      JointType.ShoulderLeft, JointType.ElbowLeft,
                                      JointType.ElbowLeft, JointType.WristLeft,
                                      JointType.WristLeft, JointType.HandLeft,
                                      // right arm 
                                      JointType.ShoulderRight, JointType.ElbowRight,
                                      JointType.ElbowRight, JointType.WristRight,
                                      JointType.WristRight, JointType.HandRight,
                                      // left leg
                                      JointType.HipLeft, JointType.KneeLeft,
                                      JointType.KneeLeft, JointType.AnkleLeft,
                                      JointType.AnkleLeft, JointType.FootLeft,
                                      // right leg
                                      JointType.HipRight, JointType.KneeRight,
                                      JointType.KneeRight, JointType.AnkleRight,
                                      JointType.AnkleRight, JointType.FootRight,
                                    };



        int screenWid = 640;
        int screenHei = 960;

        private Pose floatPose = new Pose();

        int currentFrame = 0;

        // flying
        double hipCenter_Y = 0;
        double totalY = 0;
        double averageY = 0;
        bool flying = false;

        // move horizontal
        float bodyAngle = 270;
        float horiSpeed = 0;

        //
        // game part
        //

        MediaPlayer loop;
        SoundPlayer broken;
        SoundPlayer energy_up;
        SoundPlayer fail_save_people;
        SoundPlayer gameover;
        SoundPlayer jump02;
        SoundPlayer save_people;
        SoundPlayer shooting;
        SoundPlayer win;

        bool gameOver = false;
        bool restart = false;

        bool gameStart = false;
        // physic
        float g = 3f;
        float t = 0;
        float currT = 0;
        bool cut = false;

        bool check = false;
        // object
        Player player;
        Bullet bullet = new Bullet();
        bool shoot = false;
        bool isShoot = false;

        Rect[] help = new Rect[7];
        int[] helpArrX = { 107, 532, 325, 225, 453, 282, 481 };
        int[] helpArrY = { 125, -1611, -2565, -3704, -4473, -5648, -7873 };
        bool[] helped = { false, false, false, false, false, false, false};

        // Move direction of the bullet
        Vector moveDir;
        System.Timers.Timer aTimer = new System.Timers.Timer();

        Ground[] stand = new Ground[68]; // total ?
        int[] standArrX = {/*0-29*/ 0, 223, 430, 238, 32, 238, 46, 209, 347, 77, 319, 32, 238, 453, 20, 328, 40, 36, 338, 360, 161, 77, 116, 352, 139, 206, 244, 134, 142, 426, /* next is disappear*//*30-55*/    408, 426, 12, 130, 116, 321, 340, 18, 437, 178, 417, 244, 418, 145, 101, 387, 352, 50, 436, 46, 232, 48, 38, 377, 385, 70,  /*next is moving hori 56-61*/ 40, 60, 42, 65, 36, 287,  /* next is  vertical moving 62-67*/ 153, 249, 135, 239, 255, 46 };
        int[] standArrY = { 940, 864, 763, 673, 587, 499, 416, 304, 117, 71, -13, -249, -602, -1039, -1292, -2138, -2309, -2694, -3097, -3439, -3621, -4245, -4522, -4656, -5254, -5566, -5948, -7033, -7264, -7790, /* next is disappear*/ 392, 244, 207, -137, -438, -825, -1174, -1449, -1528, -1942, -2364, -2482, -2864, -2955, -3261, -4389, -5130, -5730, -5845, -6066, -6179, -6399, -6892, -7131, -7331, -7452,/*next is moving hori*/-2728, -3819, -5373, -6291, -7599, -7705,/* next is  vertical moving*/ -1576, -3922, -4819, -6566, -7866, -7971 };

        //Ground[] disStand = new Ground[2]; // total 25
        //int[] disStandArrX = {407, 408};
        //int[] disStandArrY = {391, 244};

        Enemy[] enemy = new Enemy[8];
        int[] enemyArrX = { 433, 573, 43, 450, 425, 332, 211, 203 };
        int[] enemyArrY = { -107, -1132, 2402, -3533, -4751, -6042, -7126, -7538};

        // ui
        BitmapImage ui1;
        BitmapImage ui2;
        float batEnergy = 0;
        Rect energyR;
        
        int score = 0;



        // drawing
        private DrawingGroup drawingGroup; // Drawing group for skeleton rendering output
        private BitmapImage[] playerImg = new BitmapImage[5];
        private BitmapImage bg;
        private BitmapImage bulletImg;
        private BitmapImage jokerImg;

        private BitmapImage sStand;
        private BitmapImage dStand;
        private BitmapImage mStand;

        private BitmapImage helpImg;

        private Rect bgRect;
        float mapSpeed = 0.5f;
        float getSpeed = 0;
        bool isGot = false;
        private DrawingImage drawingImg; // Drawing image that we will display

        DrawingGroup skeletonGroup;
        DrawingImage skeImg;

        public MainWindow()
        {
            InitializeComponent();
        }

        void Restart() 
        {
              currentFrame = 0;

              overText.Visibility = Visibility.Hidden;

            // flying
              hipCenter_Y = 0;
              totalY = 0;
              averageY = 0;
              flying = false;

            // move horizontal
              bodyAngle = 270;
              horiSpeed = 0;

            //
            // game part
            //

              
              restart = false;
              gameStart = false;
            // physic
              g = 3f;
              t = 0;
              currT = 0;
              cut = false;

              check = false;
            // object
             shoot = false;
              isShoot = false;

            batEnergy = 0;

            mapSpeed = 0.5f;
            getSpeed = 0;
            isGot = false;

            energyR = new Rect(6, -200, 43, 238);

            bgRect = new Rect(0, -8142, 640, 9120);

            for (int i = 0; i < helped.Length; i++)
            {
                helped[i] = false;
            }

            // object
            player = new Player(40, 700, 75);
            // bullet
            bullet = new Bullet();
            // ground
            for (int i = 0; i < standArrX.Length; i++)
            {
                if (i == 0)
                {
                    stand[i] = new Ground(0, 940, 640, 300);
                }
                else
                    stand[i] = new Ground(standArrX[i], standArrY[i], 170, 60);


            }
            // enemy
            for (int i = 0; i < enemy.Length; i++)
            {
                
                enemy[i] = new Enemy(enemyArrX[i], enemyArrY[i], 2);
            }
            // help
            for (int i = 0; i < help.Length; i++)
            {

                help[i] = new Rect(helpArrX[i], helpArrY[i], 80, 80);
            }

            gameOver = false;
            score = 0;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No Kinects detected", "Depth Sensor Basics");
                Application.Current.Shutdown();
            }
            else
            {
                sensor = KinectSensor.KinectSensors[0];
                if (sensor == null)
                {
                    MessageBox.Show("Kinect is not ready to use", "Depth Sensor Basics");
                    Application.Current.Shutdown();
                }
            }

            // skeleton stream 
            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];

            // Create the drawing group we'll use for drawing
            drawingGroup = new DrawingGroup();
            skeletonGroup = new DrawingGroup();
            // Create an image source that we can use in our image control
            drawingImg = new DrawingImage(drawingGroup);
            skeImg = new DrawingImage(skeletonGroup);
            // Display the drawing using our image control
            gameImg.Source = drawingImg;
            skeletonImg.Source = skeImg;
            // prevent drawing outside of our render area
            drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, screenWid, screenHei));
            skeletonGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 386, 530));
            
            bg = LoadImages("Image/bg.jpg");
            bulletImg = LoadImages("Image/bullet.png");
            jokerImg = LoadImages("Image/joker.png");
            sStand = LoadImages("Image/stand.png");
            dStand = LoadImages("Image/dissappear.png");
            mStand = LoadImages("Image/move.png");

            helpImg = LoadImages("Image/help.png");

            playerImg[0] = LoadImages("Image/down_right.png"); // down right
            playerImg[1] = LoadImages("Image/down_left.png"); // down left
            playerImg[2] = LoadImages("Image/jump_right.png"); // jumpright
            playerImg[3] = LoadImages("Image/jump_left.png"); // jump left
            playerImg[4] = LoadImages("Image/float.png"); // float

            ui1 = LoadImages("Image/ui.png");
            ui2 = LoadImages("Image/ui2.png");

            energyR = new Rect(6, -200, 43, 238);

            bgRect = new Rect(0, -8142, 640, 9120);

            // bgm
            //loop.Open(new Uri("ding.wav",UriKind.Relative));
            broken = new SoundPlayer("bgm/broken.wav");
            energy_up = new SoundPlayer("bgm/energy_up.wav");
            fail_save_people = new SoundPlayer("bgm/fail_save_people.wav");
            gameover = new SoundPlayer("bgm/gameover.wav");
            jump02 = new SoundPlayer("bgm/jump02.wav");
            save_people = new SoundPlayer("bgm/save_people.wav");
            shooting = new SoundPlayer("bgm/shooting.wav");
            win = new SoundPlayer("bgm/win.wav");


            // object
            player = new Player(40, 700, 75);
            // bullet
            bullet = new Bullet();
            // ground
            for (int i = 0; i < standArrX.Length; i++)
            {
                if (i == 0)
                {
                    stand[i] = new Ground(0, 940, 640, 300);
                }
                else
                    stand[i] = new Ground(standArrX[i], standArrY[i], 170, 60);

                
            }
            // enemy
            for (int i = 0; i < enemy.Length; i++)
            {
                Random rd = new Random();
                enemy[i] = new Enemy(enemyArrX[i], enemyArrY[i], rd.Next(6, 9));
            }
            // help
            for (int i = 0; i < help.Length; i++)
            {
             
                help[i] = new Rect(helpArrX[i], helpArrY[i], 80, 80);
            }

            //// ground will disappear
            //for (int i = 0; i < disStandArrX.Length; i++)
            //{
            //    disStand[i] = new Ground(disStandArrX[i], disStandArrY[i], 170, 60);

            //}
            // bullet timer internal
            aTimer.Elapsed += new ElapsedEventHandler(shootInternal);

            aTimer.Interval = 1000;    // 1秒 = 1000毫秒

            aTimer.Start();

            FloatPose();
            
            // start the kinect
            sensor.Start();

            //loop.Play();
        }

        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            using (DrawingContext dc = this.drawingGroup.Open()) // clear the drawing
            {
                // draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, screenWid, screenHei));

                //statusTxt.Text = "No Skeleton Detected";

                dc.DrawImage(bg, bgRect);

                if (!gameOver)
                {
                    // draw player
                    if (player.vY > 0 && !player.isFloating && horiSpeed >= 0)
                    {
                        dc.DrawImage(playerImg[2], player.box);
                    }
                    else if (player.vY > 0 && !player.isFloating && horiSpeed <= 0)
                    {
                        dc.DrawImage(playerImg[3], player.box);
                    }
                    else if (player.vY <= 0 && !player.isFloating && horiSpeed >= 0)
                    {
                        dc.DrawImage(playerImg[0], player.box);
                    }
                    else if (player.vY <= 0 && !player.isFloating && horiSpeed <= 0)
                    {
                        dc.DrawImage(playerImg[1], player.box);
                    }
                    if (player.isFloating)
                    {
                        dc.DrawImage(playerImg[4], player.box);
                    }
                }
                // drawing ui
                
                dc.DrawImage(ui2, energyR);
                dc.DrawImage(ui1,new Rect(0,0,179,250));

                if (batEnergy <= 100) 
                {
                    batEnergy += 0.5f;
                    energyR.Y += 1;
                }
                else if (batEnergy > 100) 
                {
                    batEnergy = 100;
                    energyR.Y = 0;
                }

                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    {
                        frame.CopySkeletonDataTo(skeletons);

                        // Add your code below 

                        // Find the closest skeleton 
                        Skeleton skeleton = GetPrimarySkeleton(skeletons);

                        if (skeleton == null) return;
                        //statusTxt.Text = "Skeleton Detected";

                        using (DrawingContext dc2 = this.skeletonGroup.Open())
                        {
                            dc2.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, 386, 530));
                            DrawSkeleton(skeleton, dc2, Brushes.GreenYellow, new Pen(Brushes.DarkGreen, 6));
                        }
                        //
                        // my code
                        //
                        if (!gameOver)
                        {
                            bodyAngle = GetAngle(skeleton, JointType.HipCenter, JointType.ShoulderCenter);
                            if (player.posX + 80 <= 640 && player.posX >= 0)
                                horiSpeed = (bodyAngle - 270) / 5;
                            else if (player.posX + 80 > 640)
                            {
                                horiSpeed = 0;
                                player.posX = 560;
                            }
                            else if (player.posX < 0)
                            {
                                horiSpeed = 0;
                                player.posX = 0;
                            }
                            //Console.WriteLine((int)horiSpeed);

                            // detect Flying
                            hipCenter_Y = getHipCenterY(skeleton);
                            if (currentFrame < 30)
                                totalY += hipCenter_Y = getHipCenterY(skeleton);
                            else
                            {
                                averageY = totalY / 30;

                                //hCenterY_Text.Text = "Y: " + averageY + "current Frame: " + currentFrame + "horiSpeed: " + horiSpeed;
                            }
                        }
                        // game loop start, 30fps

                            Point handLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandLeft].Position);
                            Point handRight = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandRight].Position);

                            if (gameOver && HitTest(handLeft, handRight, 50)) 
                            {
                                Restart();
                            }

                        if (!gameOver)
                        {
                            //
                            // stand part
                            //
                            // stand collision
                            for (int i = 0; i < stand.Length; i++)
                            {
                                if (Check_Collision_V(player.box, stand[i].boundingBox) == 1)
                                {
                                    player.isGrounded = true;
                                    if (i >= 30 && i <= 55)
                                    {
                                        stand[i].boundingBox.X = 700;
                                        broken.Play();
                                    }
                                    break;
                                }
                                else
                                {
                                    player.isGrounded = false;
                                }
                            }
                            // stand moving

                            for (int i = 56; i < standArrX.Length; i++)
                            {
                                if (i <= 61)
                                {
                                    stand[i].MovingGround(1, 5, 0);
                                }
                                if (i >= 62)
                                {
                                    stand[i].MovingGround(2, 8, 150);
                                }

                            }


                            // draw stand
                            for (int i = 0; i < standArrX.Length; i++)
                            {
                                if (i <= 29)
                                {
                                    dc.DrawImage(sStand, stand[i].boundingBox);
                                }

                                if (i >= 30 && i <= 55)
                                {
                                    dc.DrawImage(dStand, stand[i].boundingBox);
                                }

                                if (i >= 56)
                                {
                                    dc.DrawImage(mStand, stand[i].boundingBox);
                                }

                            }
                            // gravity
                            if (!player.isGrounded)
                            {
                                if (player.isFloating)
                                {
                                    g = 0;
                                }
                                else
                                    g = 3f;

                                if (!cut)
                                {
                                    cut = true;
                                    currT = currentFrame;
                                }

                                t = (currentFrame - currT) / 3;

                            }
                            else
                            {
                                cut = false;
                                currT = 0;
                                t = 0;
                                g = 0;

                                player.vY = 0;
                                player.isJumping = false;
                                player.isFloating = false;
                                player.isFlying = false;
                                //test = false;
                                check = false;

                            }

                            // moving left and right
                            //switch (angle)
                            //{
                            //        case 15
                            //    default:
                            //        break;
                            //}
                            player.vX = 3 * horiSpeed;

                            Point head = SkeletonPointToScreenPoint(skeleton.Joints[JointType.Head].Position);

                            // jumping
                            if (!player.isJumping && handRight.Y < head.Y - 20) // match pose 
                            {
                                gameStart = true;
                                player.isJumping = true;
                                player.v0 = -20;

                                if (!check)
                                {
                                    jump02.Play();
                                    check = true;
                                    player.posY -= 3;
                                }
                            }

                            // flying
                            if (!player.isFlying && hipCenter_Y <= averageY - 50 && batEnergy >= 50)//currentFrame > 100) // match pose 
                            {
                                gameStart = true;
                                player.isFlying = true;
                                batEnergy -= 50;
                                energyR.Y -= 100;
                                player.v0 = -35;
                                if (!check)
                                {
                                    check = true;
                                    player.posY -= 3;
                                }
                            }
                            // floating
                            if (!player.isFloating && player.vY > 0 && isMatched(skeleton, floatPose) && batEnergy >= 30) // match pose 
                            {
                                player.isFloating = true;
                                batEnergy -= 30;
                                energyR.Y -= 60;
                                player.v0 = 4;
                                if (!check)
                                {
                                    check = true;
                                    player.posY -= 3;
                                }
                            }

                            //
                            // shooting
                            //

                            if (Shoot(skeleton))
                            {
                                ShootBullet(skeleton, dc);
                            }
                            if (bullet.isInit)  // bullet move
                            {
                                bullet.MoveBullet(moveDir);

                                if (bullet.getPosision().X < 0 || bullet.getPosision().Y < 0 || bullet.getPosision().X > screenWid || bullet.getPosision().Y > screenHei)
                                {
                                    bullet.isDestroy = true;
                                }

                            }
                            // draw bullet
                            if (bullet.isInit)  // bullet move
                            {
                                dc.DrawImage(bulletImg, new Rect(bullet.getPosision(), new Size(52, 25)));
                            }


                            // caculate speed
                            if (player.isJumping || player.isFlying || player.isFloating)
                            {
                                player.vY = player.v0 + g * t;

                            }
                            else
                            {
                                if (player.vY <= 70)
                                    player.vY += g * t;
                            }

                            //player.vY = -4;

                            // if top, player don't move, map move
                            //if (player.isTopped)
                            //{
                            //    //mapSpeed = -1 * player.vY;
                            //    if (!isGot)
                            //    {
                            //        isGot = true;
                            //        getSpeed = player.vY - 10;
                            //    }
                            //    player.vY = 0;

                            //    mapSpeed = (/*player.v0*/ -1 * getSpeed - g * t);


                            //    if (mapSpeed <= 0)
                            //    {

                            //        player.posY += 2;
                            //        player.vY = 2;
                            //    }

                            //    if (mapSpeed > 0)
                            //    {
                            //        bgRect.Y += mapSpeed;
                            //        // stand, enemy, energy, people move together
                            //        for (int i = 0; i < standArrX.Length - 1; i++)
                            //        {
                            //            stand[i].boundingBox.Y += mapSpeed;
                            //        }

                            //        score += 100;
                            //    }
                            //}

                            // move map
                            if (mapSpeed > 0 && gameStart)
                            {
                                bgRect.Y += mapSpeed;
                                // stand, enemy, energy, people move together
                                for (int i = 0; i < standArrX.Length; i++)
                                {
                                    stand[i].boundingBox.Y += mapSpeed;
                                    if (i >= 62)
                                    {
                                        stand[i].initY += (int)mapSpeed;
                                    }
                                }
                                // enemy
                                for (int i = 0; i < enemyArrX.Length; i++)
                                {
                                    enemy[i].box.Y += (int)mapSpeed;
                                }
                                // help
                                for (int i = 0; i < help.Length; i++)
                                {
                                    help[i].Y += mapSpeed;
                                }


                                score += (int)mapSpeed;
                            }

                            if (bgRect.Y >= -9000 && bgRect.Y <= -8000)
                                mapSpeed = 1.5f;
                            else if (bgRect.Y >= -8000 && bgRect.Y <= -6000)
                                mapSpeed = 3f;
                            else if (bgRect.Y >= -6000 && bgRect.Y <= -4000)
                                mapSpeed = 4.5f;
                            else if (bgRect.Y >= -4000 && bgRect.Y <= -2000)
                                mapSpeed = 5.5f;
                            else if (bgRect.Y >= -2000 && bgRect.Y < -10)
                                mapSpeed = 6f;
                            else if (bgRect.Y >= -10)
                                mapSpeed = 0;

                            // top check
                            if (player.posY <= 1)
                            {
                                if (bgRect.Y >= -10)
                                {
                                    win.Play();
                                    gameOver = true;
                                    overText.Text = "You win, Your Score: " + score;
                                    overText.Visibility = Visibility.Visible;
                                }

                                player.isTopped = true;
                                player.vY = 0;
                                player.posY = 2;

                            }
                            else
                            {
                                player.isTopped = false;
                            }

                            //
                            // emeny
                            //

                            // drawing
                            for (int i = 0; i < enemy.Length; i++)
                            {
                                if (!enemy[i].dead)
                                {
                                    dc.DrawImage(jokerImg, enemy[i].box);
                                }
                            }

                            // moving
                            for (int i = 0; i < enemy.Length; i++)
                            {
                                if (!enemy[i].dead)
                                {
                                    enemy[i].MoveEnemy(1);
                                }
                            }
                            // collision
                            for (int i = 0; i < enemy.Length; i++)
                            {
                                if (!enemy[i].dead && enemy[i].box.IntersectsWith(player.box))
                                {
                                    gameOver = true;
                                }
                                if (!enemy[i].dead && enemy[i].box.IntersectsWith(new Rect(bullet.getPosision(), new Size(30, 11))))
                                {
                                    enemy[i].dead = true;
                                    score += 200;
                                }
                            }

                            //
                            // help
                            //
                            for (int i = 0; i < help.Length; i++)
                            {
                                dc.DrawImage(helpImg, help[i]);


                                if (help[i].IntersectsWith(player.box))
                                {
                                    help[i].X = 800;
                                    score += 300;
                                    // play sound
                                    save_people.Play();
                                }
                                if (help[i].Y >= 960 &&!helped[i])
                                {
                                    helped[i] = true;
                                    score -= 300;

                                    // play sound
                                    fail_save_people.Play();
                                }
                            }
                            // caculate position
                            //Console.WriteLine(player.vY + "g: " + g + "isFloating: " + player.isFloating);
                            player.posY += (int)(player.vY);// * t + 0.5 * g * t * t);
                            player.posX += (int)player.vX;

                            // update box
                            player.box.X = player.posX;
                            player.box.Y = player.posY;

                            if (player.posY >= 955)
                            {
                                gameOver = true;
                            }

                            if (gameOver) 
                            {
                                overText.Text = "Game Over\nClose Hand";
                                overText.Visibility = Visibility.Visible;
                                gameover.Play();
                            }

                            currentFrame++;
                        }
                    }
                    else
                    {
                        totalY = 0;
                        averageY = 0;
                        currentFrame = 0;
                    }
                }
            }
            //info.Text = "grounded: " + player.isGrounded +
            //    " \ngameStart: " + gameStart +
            //    " \nfloat: " + player.isFloating +
            //    " \nshoot: " + shoot +
            //    " \nmapspeed: " + mapSpeed +
            //    " \nplayer: " + player.vY
            //    ;
            info.Text = "Score: " + score +
                " \nGameOver: " + gameOver +
                " \nFloating: " + player.isFloating +
                " \nFlying: " + player.isFlying
                ;
        }

        private Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                //Find the closest skeleton       
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null) skeleton = skeletons[i];
                        else if (skeleton.Position.Z > skeletons[i].Position.Z)
                            skeleton = skeletons[i];
                    }
                }
            }

            return skeleton;
        }

        private void DrawSkeleton(Skeleton skeleton, DrawingContext dc, Brush jointBrush, Pen bonePen)
        {
            for (int i = 0; i < bones.Length; i += 2)
                DrawBone(skeleton, dc, bones[i], bones[i + 1], bonePen);

            // Render joints
            foreach (Joint j in skeleton.Joints)
            {
                if (j.TrackingState == JointTrackingState.NotTracked) continue;

                dc.DrawEllipse(jointBrush, null, SkeletonPointToScreenPoint(j.Position), 5, 5);
            }
        }

        private void DrawBone(Skeleton skeleton, DrawingContext dc, JointType jointType0, JointType jointType1, Pen bonePen)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked) return;

            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred) return;

            //dc.DrawLine(new Pen(Brushes.Red, 5),
            dc.DrawLine(bonePen,
                SkeletonPointToScreenPoint(joint0.Position),
                SkeletonPointToScreenPoint(joint1.Position));
        }

        private Point SkeletonPointToScreenPoint(SkeletonPoint sp)
        {
            ColorImagePoint pt = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(
                sp, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(pt.X, pt.Y);
        }

        private float GetAngle(Skeleton s, JointType js, JointType je)
        {
            Point sp = SkeletonPointToScreenPoint(s.Joints[js].Position);
            Point ep = SkeletonPointToScreenPoint(s.Joints[je].Position);

            float angle = (float)(
                Math.Atan2(ep.Y - sp.Y, ep.X - sp.X) * 180 / Math.PI);

            angle = (angle + 360) % 360;

            return angle;
        }

        //private void PoseMatching(Skeleton skeleton)
        //{

        //}


        //private void JumpPose() // initialized in Window_Loaded
        //{
        //    // initialize the targetPose below 
        //    jumpPose.Title = "Jump";

        //    PoseAngle[] angles = new PoseAngle[4];

        //    angles[0] = new PoseAngle(JointType.ShoulderRight,
        //                 JointType.ElbowRight, 315, 30);
        //    angles[1] = new PoseAngle(JointType.ElbowRight,
        //                 JointType.WristRight, 270, 20);


        //    angles[2] = new PoseAngle(JointType.ShoulderLeft,
        //                 JointType.ElbowLeft, 135, 20);
        //    angles[3] = new PoseAngle(JointType.ElbowLeft,
        //                 JointType.WristLeft, 270, 20);
        //    jumpPose.Angles = angles;
        //}

        private void FloatPose() // initialized in Window_Loaded
        {
            // initialize the targetPose below 
            floatPose.Title = "Float";

            PoseAngle[] angles = new PoseAngle[4];

            angles[0] = new PoseAngle(JointType.ShoulderRight,
                         JointType.ElbowRight, 315, 20);
            angles[1] = new PoseAngle(JointType.ElbowRight,
                         JointType.WristRight, 315, 20);


            angles[2] = new PoseAngle(JointType.ShoulderLeft,
                         JointType.ElbowLeft, 225, 20);
            angles[3] = new PoseAngle(JointType.ElbowLeft,
                         JointType.WristLeft, 225, 20);
            floatPose.Angles = angles;
        }

        double getHipCenterY(Skeleton sk)
        {
            double centerY = 0;
            Point hipCenter = SkeletonPointToScreenPoint(sk.Joints[JointType.HipCenter].Position);
            centerY = hipCenter.Y;

            return centerY;
        }

        private bool Shoot(Skeleton skeleton) // shoot pose
        {
            Point hipRight = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ShoulderRight].Position);
            Point hRight = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandRight].Position);

            if (HitTest(hipRight, hRight, 50))
            {
                shoot = true;
                return true;
            }
            else
            {
                shoot = false;
                return false;
            }
        }

        private void ShootBullet(Skeleton skeleton, DrawingContext dc)
        {
            Point pt1 = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandLeft].Position);
            Point pt2 = SkeletonPointToScreenPoint(skeleton.Joints[JointType.WristLeft].Position);
            Point rightHand = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandRight].Position);
            Point elbowLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ElbowLeft].Position);


            if (shoot && !isShoot)
            {
                isShoot = true;
                shoot = false;

                shooting.Play();

                moveDir = new Vector(pt1.X - pt2.X, pt1.Y - pt2.Y);
                moveDir.Normalize();

                bullet = new Bullet(new Point(player.box.X + 32.5, player.box.Y));

                //shootEffect.Play();

            }
            else if (bullet.isInit && bullet.isDestroy)
            {
                bullet = new Bullet();
            }

            if (bullet.isInit)
            {
                bullet.DrawBullet(dc, bulletImg);
            }
        }

        private void shootInternal(object source, ElapsedEventArgs e)
        {
            isShoot = false;
        }

        Boolean isMatched(Skeleton skeleton, Pose pose)
        {
            for (int i = 0; i < pose.Angles.Length; i++)
            {

                if (AngularDifference(GetAngle(skeleton, pose.Angles[i].StartJoint, pose.Angles[i].EndJoint), pose.Angles[i].Angle) > pose.Angles[i].Threshold)
                    return false;

                Console.WriteLine(GetAngle(skeleton, pose.Angles[i].StartJoint, pose.Angles[i].EndJoint));
            }
            return true;
        }

        float AngularDifference(float a1, float a2)
        {
            float abs_diff = Math.Abs(a1 - a2);
            return Math.Min(abs_diff, 360 - abs_diff);
        }

        private bool HitTest(Point pt1, Point pt2, double radious)
        {
            if ((pt1.Y - pt2.Y) * (pt1.Y - pt2.Y) + (pt2.X - pt1.X) * (pt2.X - pt1.X) < radious * radious)
                return true;
            else
                return false;
        }

        // other function

        private BitmapImage LoadImages(String url)
        {
            BitmapImage img;
            img = new BitmapImage(new Uri(url, UriKind.Relative));

            return img;
        }

        private int Check_Collision_V(Rect A, Rect B)
        {

            //The sides of the rectangles
            double leftA, leftB;
            double rightA, rightB;
            double topA, topB;
            double bottomA, bottomB;


            //Calculate the sides of rect A
            leftA = A.X;
            rightA = A.X + A.Width;
            topA = A.Y;
            bottomA = A.Y + A.Height;

            //Calculate the sides of rect B
            leftB = B.X;
            rightB = B.X + B.Width;
            topB = B.Y;
            bottomB = B.Y + B.Height;
            if (player.vY >= 0)
            {
                //If any of the sides from A are outside of B
                if (bottomA < bottomB && bottomA > topB && rightA > leftB + A.Width / 2 && leftA < rightB - A.Width / 2)
                {
                    player.posY = (int)(topB - 73);
                    return 1;
                }
            }
            return 2;
        }
    }
}
