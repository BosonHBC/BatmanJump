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
using System.Media;

namespace W7_PositionBasedMatching
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        private float threshold = 30;
        private Point leftTarget = new Point(320 - 150, 240);
        private Point rightTarget = new Point(320 + 150, 240);

        private FormattedText leftText = new FormattedText(
    "L", System.Globalization.CultureInfo.GetCultureInfo("en-us"),
    FlowDirection.LeftToRight, new Typeface("Verdana"), 32,
    Brushes.Black);

        private bool isHit = false;

        private SoundPlayer soundPlayer = new SoundPlayer("ding.wav");

        private KinectSensor sensor;

        private Skeleton[] skeletons = null;
        private JointType[] bones = { 
                                      // torso 
                                      JointType.Head, JointType.ShoulderCenter, // each pair defines one bone 
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

        private DrawingGroup drawingGroup; // Drawing group for skeleton rendering output
        private DrawingImage drawingImg; // Drawing image that we will display

        public MainWindow()
        {
            InitializeComponent();
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
            // Create an image source that we can use in our image control
            drawingImg = new DrawingImage(drawingGroup);
            // Display the drawing using our image control
            skeletonImg.Source = drawingImg;
            // prevent drawing outside of our render area
            drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 640, 480));

            // start the kinect
            sensor.Start();
        }

        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (DrawingContext dc = this.drawingGroup.Open()) // clear the drawing
            {
                // draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, 640, 480));

                RenderTargetPositions(dc);

                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    {
                        frame.CopySkeletonDataTo(skeletons);

                        // Add your code below 

                        // Find the closest skeleton 
                        Skeleton skeleton = GetPrimarySkeleton(skeletons);

                        if (skeleton == null) return;

                        PositionBasedMatching(skeleton);
                        DrawSkeleton(skeleton, dc, Brushes.GreenYellow, new Pen(Brushes.DarkGreen, 6));

                    }
                }
            }
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

        private void RenderTargetPositions(DrawingContext dc)
        {
            if (isHit)
            {
                dc.DrawEllipse(Brushes.Red, null, leftTarget, threshold, threshold);
            }
            else 
            {
                dc.DrawEllipse(Brushes.Green, null, leftTarget, threshold, threshold);
            }

            
            dc.DrawText(leftText, leftTarget);
            dc.DrawEllipse(Brushes.Red, null, rightTarget, threshold, threshold);
        }

        private Boolean HitTest(Point pt1, Point pt2, float threshold)
        {
            if ((pt1.X - pt2.X) * (pt1.X - pt2.X) +
                (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y) < threshold * threshold)
                return true;
            else
                return false;
        }

        bool isSoundPlay = false;

        private void PositionBasedMatching(Skeleton skeleton)
        {
            // add your code below 
            if (HitTest(
                SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandLeft].Position),
                leftTarget,
                threshold
                ))
            {
                isHit = true;
                if (!isSoundPlay)
                {
                    isSoundPlay = true;
                    soundPlayer.Play();
                }

                //Random

                Random rd = new Random();

                leftTarget.X = rd.Next(50,320);
                leftTarget.Y = rd.Next(50, 430);
            }
            else 
            {
                isHit = false;
                isSoundPlay = false;
            }
        }


    }
}
