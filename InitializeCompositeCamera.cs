using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace arCam
{
    class InitializeCompositeCameras
    {
        public  VideoCaptureDevice archaicCamera { get; private set; }
        public FilterInfoCollection compositeDevices { get; private set; }
        public List<string> cameras { get; private set; }
        public Bitmap imageUpdate { get; private set; }
        public PictureBox pictureBox { get; set; }

        /// <summary>
        /// Initialize Video Capture Object, then get list of cameras for video capture
        /// </summary>

        public InitializeCompositeCameras(PictureBox pb)
        {
            archaicCamera = new VideoCaptureDevice();
            this.pictureBox = pb;
            GetCompositeDevices();
        }

        public InitializeCompositeCameras()
        {
            GetCompositeDevices();
        }

        /// <summary>
        /// This should identify all composite video capture devices on the system
        /// </summary>
        /// <returns></returns>
        public List<string> GetCompositeDevices()
        {
            compositeDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            cameras = new List<string>();

            foreach (FilterInfo device in compositeDevices)
            {
                cameras.Add(device.Name);
            }

            return cameras;
        }

        /// <summary>
        /// Lets separate feed start in case settings need to be applied
        /// </summary>
        public void StartStopArchaicCameraFeed(int flag)
        {
            try
            {
                if (!archaicCamera.IsRunning && flag != 1)
                    archaicCamera.Start();
                else
                {
                    archaicCamera.Stop();
                    pictureBox.Dispose();
                    compositeDevices.Clear();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Device not found, ensure the device is plugged in and working", "Device not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// We're going to configure the cameras event here
        /// </summary>
        /// <param name="cameraIndex"></param>
        public void ConfigureCameraObject(int cameraIndex)
        {
            try
            {
                // Retrieve moniker string for device identification
                archaicCamera = new VideoCaptureDevice(compositeDevices[cameraIndex].MonikerString);

                // New Evenet
                archaicCamera.NewFrame += new AForge.Video.NewFrameEventHandler(ArchaicCamera_NewFrame);

                // Get and set default composite properties below this line
                // ...
                // ...
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Could not start device, capture source not identified","Device not found",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// We're going to re-render the clone to a proportionate size to fit in the picturebox.
        /// Some devices appear to have a predefined resolution scale that cannot be changed.
        /// The device in question has default scale of 640x480. We will resize the const to
        /// fit 320x240 for improved clarity.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ArchaicCamera_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            // This will create an exact copy (clone) of the video frame from the camera
            // The second line will divide the resolution in half giving us 320x240
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            Bitmap resizedFrame = new Bitmap(frame, new Size(frame.Width / 2, frame.Height / 2));

            // Set resolution dpi, if supported for 320x240 (18.18dpi)
            resizedFrame.SetResolution(18.18f, 18.18f);

            // Lets draw the graphics now, again improve quality if supported
            using (Graphics graphics = Graphics.FromImage(resizedFrame))
            {
                graphics.InterpolationMode = InterpolationMode.Bicubic;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Assign attribute to cloned image
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    graphics.DrawImage(resizedFrame, new Rectangle(new Point(0, 0), new Size(320, 240)), 0, 0, resizedFrame.Width, resizedFrame.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            // Return updated clone
            pictureBox.Image = resizedFrame;
        }

        List<CameraControlProperty> DeviceCaptureProperties;
        CameraControlFlags flags;
        public void GetCaptureDeviceProperties()
        {
            DeviceCaptureProperties = new List<CameraControlProperty>
            {
                (CameraControlProperty.Roll),
                (CameraControlProperty.Pan),
                (CameraControlProperty.Exposure),
                (CameraControlProperty.Focus),
                (CameraControlProperty.Iris),
                (CameraControlProperty.Zoom),
                (CameraControlProperty.Tilt)
            };
        }

        // Debug Only
        /*
        public void misc()
        {
            //CameraControlFlags flag;
            //int min, max, step, def;

            foreach(CameraControlProperty p in DeviceCaptureProperties)
            {
                camera.GetCameraPropertyRange(p, out min, out max, out step, out def, out flag);
                foreach(Control c in this.Controls)
                {
                    if(c != null && c is NumericUpDown && c.Tag.ToString().Contains(p.ToString().ToLower()))
                    {
                        (c as NumericUpDown).Minimum = min;
                        (c as NumericUpDown).Maximum = max;
                        (c as NumericUpDown).Increment = step;
                        // (c as NumericUpDown).Value = def;
                    }
                }
                camera.SetCameraProperty(p, def, CameraControlFlags.Auto);
                txbProperties.Text += $"{p}: Min: {min} Max: {max} Step: {step} Def: {def} Flag: {flag}{Environment.NewLine}";           
        }
        */
    }
}
