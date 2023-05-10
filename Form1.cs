using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using AForge.Video.DirectShow;

namespace arCam
{
    public partial class Form1 : Form
    {        
        InitializeCompositeCameras archaicDevices;

        public Form1()
        {
            InitializeComponent();           
            cmbDevices.Items.Clear();
            archaicDevices = new InitializeCompositeCameras(pbViewer);
            cmbDevices.Items.AddRange(archaicDevices.cameras.ToArray());
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            archaicDevices.ConfigureCameraObject(cmbDevices.SelectedIndex);
            archaicDevices.StartStopArchaicCameraFeed(0);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            archaicDevices.StartStopArchaicCameraFeed(0);
            pbViewer.Image = null;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cmbDevices.Items.Clear();
            archaicDevices = new InitializeCompositeCameras();
            cmbDevices.Items.AddRange(archaicDevices.cameras.ToArray());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            archaicDevices.StartStopArchaicCameraFeed(1);
        }

        // camera.SetCameraProperty(CameraControlProperty.Exposure, (int)nudExposure.Value, CameraControlFlags.Manual);

    }
}
