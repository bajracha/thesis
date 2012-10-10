using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace VideoSender
{
    public partial class Form1 : Form
    {
        
        Image image;
        MemoryStream imgDataStream;
        Socket sendingSocket;
        IPAddress sendToAddress;
        IPEndPoint sendingEndPoint;
        byte[] sendBuffer;
        

        public Form1()
        {
            InitializeComponent();
            InitializeDevicesList();            
            imgDataStream = null;
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);            
            sendToAddress = IPAddress.Parse("127.0.0.1");
            //sendToAddress = IPAddress.Parse("192.168.0.101");
            sendingEndPoint = new IPEndPoint(sendToAddress, 11000);
            //sendingSocket.Connect(sendingEndPoint);
        }

        private void InitializeDevicesList()
        {
            foreach (CaptureDevice device in CaptureDevice.GetDevices())
            {
                cboDevices.Items.Add(device);
            }

            if (cboDevices.Items.Count > 0)
            {
                cboDevices.SelectedIndex = 0;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            int index = cboDevices.SelectedIndex;
            if (index != -1)
            {
                /*try
                {
                    ((CaptureDevice)cboDevices.SelectedItem).Detach();
                }
                catch
                {

                }*/
                try
                {
                    ((CaptureDevice)cboDevices.SelectedItem).Attach(pbImage);
                    
                    Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                    {
                        image = ((CaptureDevice)cboDevices.SelectedItem).Capture();
                        pbFrame.Image = image;
                        System.Threading.Thread.Sleep(33);
                        if (image != null)
                        {
                            imgDataStream = new MemoryStream();

                            image.Save(imgDataStream, ImageFormat.Jpeg);
                            sendBuffer = imgDataStream.ToArray();
                            try
                            {
                                //sendingSocket.Send(sendBuffer);
                                sendingSocket.SendTo(sendBuffer, sendingEndPoint);
                            }
                            catch
                            {
                            }
                            imgDataStream.Close();
                        }
                        
                    });
                }                    
                
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ((CaptureDevice)cboDevices.SelectedItem).Detach();
            //sendingSocket.Disconnect(true);
            sendingSocket.Close();
            
        }
    }
}
