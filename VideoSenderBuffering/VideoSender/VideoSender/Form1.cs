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
using System.Threading;

namespace VideoSender
{
    public partial class Form1 : Form
    {
        
        Image image;
        MemoryStream imgDataStream;
        Socket sendingSocket;
        IPAddress sendToAddress;
        IPEndPoint sendingEndPoint;
        byte[] sendByteArray;
        CircularBuffer frameBuffer;
        Thread transmission;
        Thread buffering;

        public Form1()
        {
            InitializeComponent();
            InitializeDevicesList();            
            imgDataStream = null;
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);            
            sendToAddress = IPAddress.Parse("127.0.0.1");
            //sendToAddress = IPAddress.Parse("131.216.16.125");
            sendingEndPoint = new IPEndPoint(sendToAddress, 11000);

            frameBuffer = new CircularBuffer();
            transmission = new Thread(this.SendFrame);
            buffering = new Thread(this.FillBuffer);
            buffering.SetApartmentState(ApartmentState.STA); //this is reqd as this thread gets data from clipboard.
            CheckForIllegalCrossThreadCalls = false;
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

        private void resetProgress()
        {
            progBuffer.Minimum = 1;
            progBuffer.Maximum = 4000;
            progBuffer.Value = 1;
            progBuffer.Step = 1;

            progSend.Minimum = 1;
            progSend.Maximum = 4000;
            progSend.Value = 1;
            progSend.Step = 1;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            int index = cboDevices.SelectedIndex;
            if (index != -1)
            {
                try
                {
                    ((CaptureDevice)cboDevices.SelectedItem).Attach(pbImage);

                    if (transmission.ThreadState.ToString().Equals("Unstarted"))
                        transmission.Start();
                    if (buffering.ThreadState.ToString().Equals("Unstarted"))
                        buffering.Start();

                    //Console.WriteLine(buffering.GetApartmentState().ToString());
                    
                    /*Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                    {
                        if (false) { }
                        /*image = ((CaptureDevice)cboDevices.SelectedItem).Capture();
                        
                        //System.Threading.Thread.Sleep(33);
                        if (image != null)
                        {
                            pbFrame.Image = image;

                            imgDataStream = new MemoryStream();
                            image.Save(imgDataStream, ImageFormat.Jpeg);
                            sendByteArray = imgDataStream.ToArray();
                            //Console.WriteLine("Buffering...");
                            frameBuffer.enqueue(ref sendByteArray);
                            if (progBuffer.Value >= progBuffer.Maximum)
                                this.resetProgress();
                            else
                                progBuffer.PerformStep();
                            
                            imgDataStream.Close();
                        }

                        
                    });*/
                }                    
                
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void SendFrame()
        {
            while (true)
            {
                
                if (frameBuffer.size() >= 40)
                {
                    try
                    {
                        if (sendingSocket != null)
                        {
                            //Console.WriteLine("sending...");
                            sendingSocket.SendTo(frameBuffer.read(), sendingEndPoint);
                            progSend.PerformStep();
                        }
                        Thread.Sleep(30);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Sending problem... " + ex.Message);
                    }
                }
            }
        }

        private void FillBuffer()
        {
            
            while(true)
            {
                try
                {
                            
                
                    image = ((CaptureDevice)cboDevices.SelectedItem).Capture();
                    //buffering.SetApartmentState(ApartmentState.MTA);

                    if (image != null)
                    {
                        imgDataStream = new MemoryStream();
                        image.Save(imgDataStream, ImageFormat.Jpeg);
                        sendByteArray = imgDataStream.ToArray();
                        Console.WriteLine(sendByteArray.Length);
                        //Console.WriteLine("Buffering...");
                        frameBuffer.enqueue(ref sendByteArray);
                        if (progBuffer.Value >= progBuffer.Maximum)
                            this.resetProgress();
                        else
                            progBuffer.PerformStep();

                        imgDataStream.Close();
                    }
                    //else
                    //    Console.WriteLine("Image is empty...");
                
                }
                catch (Exception ex) { Console.WriteLine("buffering problem.. " + ex.Message); }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ((CaptureDevice)cboDevices.SelectedItem).Detach();            
            sendingSocket.Close();
            sendingSocket.Dispose();
            transmission.Abort();
            buffering.Abort();
            //Application.Exit();
            
        }
    }
}
