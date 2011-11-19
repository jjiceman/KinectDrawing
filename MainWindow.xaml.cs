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
using Microsoft.Research.Kinect.Nui;
using Microsoft.Samples.Kinect.WpfViewers;


namespace KinectTest
{
    public partial class MainWindow : Window
    {

        private Runtime myRuntime;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myRuntime = Runtime.Kinects[0];
            myRuntime.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);
            myRuntime.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            myRuntime.SkeletonFrameReady += new EventHandler <SkeletonFrameReadyEventArgs>(Depth_Ready);
        }

        void Depth_Ready(object sender, SkeletonFrameReadyEventArgs e)
        {
            preview.Children.Clear();
            SkeletonData data = e.SkeletonFrame.Skeletons[0];

            double x = data.Joints[JointID.HandRight].Position.X;
            double y = data.Joints[JointID.HandRight].Position.Y;
            double z = data.Joints[JointID.HandRight].Position.Z;

            preview.Children.Add(getBodySegment(data.Joints, Brushes.Green, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
            preview.Children.Add(getBodySegment(data.Joints, Brushes.Green, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
            preview.Children.Add(getBodySegment(data.Joints, Brushes.Green, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
            preview.Children.Add(getBodySegment(data.Joints, Brushes.Green, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
            preview.Children.Add(getBodySegment(data.Joints, Brushes.Green, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

            /*
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            double dispX = width * x + width / 2.0;
            double dispY = height * y + height / 2.0;

            textBlock1.Text = "X: " + x + "\nY: " + y + "\nZ: " + z + "\nDX: " + dispX + "\nDY: " + dispY;

            Canvas.SetLeft(pointer, dispX);
            Canvas.SetTop(pointer, dispY);
            */
        }



        // MS WORKER METHODS

        private Point getDisplayPosition(Joint joint)
        {
            // Worker method provided by Microsoft, converts a Joint to a Point used on the canvas
            float depthX, depthY;
            myRuntime.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = Math.Max(0, Math.Min(depthX * 320, 320));  //convert to 320, 240 space
            depthY = Math.Max(0, Math.Min(depthY * 240, 240));  //convert to 320, 240 space

            return new Point((int)(depthX), (int)(depthY));
        }

        Polyline getBodySegment(JointsCollection joints, Brush brush, params JointID[] ids)
        {
            // Combines given points into a line segment
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i)
            {
                points.Add(getDisplayPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                myRuntime.NuiCamera.ElevationAngle = (int)e.NewValue;
            }
            catch (Exception ex)
            {

            }
        }


    }
}
