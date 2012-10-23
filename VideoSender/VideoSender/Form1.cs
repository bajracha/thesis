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
        List<byte[]> saveByteArray = new List<byte[]>();
        List<int> saveFrameSize = new List<int>();
        int fileCount = 0;
        System.Threading.Thread t; 

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
                bool sendFlag = true;
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
                                saveToBuffer(sendBuffer);
                                //sendingSocket.SendTo(sendBuffer, sendingEndPoint);

                                if (sendFlag) {
                                    t= new System.Threading.Thread(sendFromBuffer);
                                    t.Start();
                                } 
                                sendFlag = false;
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

        private void saveToBuffer(byte[] sendBuffer)
        {
            saveByteArray.Add(sendBuffer);
            saveFrameSize.Add(sendBuffer.Length);
            if (saveByteArray.Count >= 40)
            {
                fileCount++;
                if (fileCount > 10)
                    fileCount = 1;
                
                String vidFilename = Directory.GetCurrentDirectory() + "Video" + fileCount.ToString() + ".dat";
                String sizeFilename = Directory.GetCurrentDirectory() + "Siz" + fileCount.ToString() + ".dat";
                //Console.WriteLine("Save"+vidFilename);
                if (File.Exists(sizeFilename))
                {
                    //File.Delete(Directory.GetCurrentDirectory() + "Video" + "1"+ ".dat");
                    //File.Delete(Directory.GetCurrentDirectory() + "Siz" + "1" + ".dat");
                    //File.Delete(vidFilename);
                    //File.Delete(sizeFilename);
                }

                FileStream vidFileStream = new FileStream(vidFilename, FileMode.Create, FileAccess.Write);
                StreamWriter sizeFileStream = new StreamWriter(sizeFilename);

                foreach (byte[] byteArrayElement in saveByteArray)
                    vidFileStream.Write(byteArrayElement, 0, byteArrayElement.Length);


                foreach (int sizeArrayElement in saveFrameSize)
                    sizeFileStream.WriteLine(sizeArrayElement);


                saveByteArray.Clear();
                saveFrameSize.Clear();
                vidFileStream.Close();
                sizeFileStream.Close();
            }
        }

        private void sendFromBuffer() {
            String vidFilename;
            String sizeFilename;
            FileStream vidFileStream;
            StreamReader sizeReader;
            String s;
            int size;
            byte[] frameData;
            MemoryStream frameStream;
            Bitmap frame;
            int fileCount = 1;
            
            while (true)
            {
                try
                {
                vidFilename = Directory.GetCurrentDirectory() + "Video" + fileCount.ToString() + ".dat";
                sizeFilename = Directory.GetCurrentDirectory() + "Siz" + fileCount.ToString() + ".dat";
                vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
                sizeReader = new StreamReader(sizeFilename);
                s = sizeReader.ReadLine();
                size = int.Parse(s);
                //Console.WriteLine("Read" + vidFilename);
                int oldFileCount = fileCount > 5 ? fileCount - 5 : (10+fileCount-5);
                String oldvidFilename = Directory.GetCurrentDirectory() + "Video" + oldFileCount.ToString() + ".dat";
                String oldsizeFilename = Directory.GetCurrentDirectory() + "Siz" + oldFileCount.ToString() + ".dat";

                if (File.Exists(oldsizeFilename))
                {
                    File.Delete(oldvidFilename);
                    File.Delete(oldsizeFilename);
                    //File.Delete(vidFilename);
                    //File.Delete(sizeFilename);
                } 
                fileCount++;
                if (fileCount > 10)
                    fileCount = 1;
                   
                    //Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                    //{

                        while (s != null)
                        {
                            System.Threading.Thread.Sleep(33);
                            frameData = new byte[size];
                            vidFileStream.Read(frameData, 0, size);
                            //frameStream = new MemoryStream(frameData);
                            //frame = new Bitmap(frameStream);
                            //frameStream.Close();
                            //pbPlayback.Image = frame;
                            //Console.WriteLine(frameData.Length);
                            sendingSocket.SendTo(frameData, sendingEndPoint);
                            s = sizeReader.ReadLine();
                            try
                            {
                                size = int.Parse(s);
                            }
                            catch { ;}
                        }
                            /*try
                            {

                                vidFileStream.Close();
                                sizeReader.Close();
                                fileCount++;
                                vidFilename = Directory.GetCurrentDirectory() + "Video" + fileCount.ToString() + ".dat";
                                sizeFilename = Directory.GetCurrentDirectory() + "Siz" + fileCount.ToString() + ".dat";
                                vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
                                sizeReader = new StreamReader(sizeFilename);
                                s = sizeReader.ReadLine();

                                size = int.Parse(s);
                            }
                            catch { ;}*/
                        

                    //});
                        sizeReader.Close();
                        vidFileStream.Close();
                }

                catch {
                    System.Threading.Thread.Sleep(200);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ((CaptureDevice)cboDevices.SelectedItem).Detach();
            //sendingSocket.Disconnect(true);
            t.Abort();
            sendingSocket.Close();
            
        }
    }
}
