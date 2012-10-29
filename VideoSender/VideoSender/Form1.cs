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
        byte[] sendBuffer;
        List<byte[]> saveByteArray;
        List<int> saveFrameSize;
        int fileCount = 0;
        int filesBuffered = 0;
        Thread buffering;
        Thread transmission;

        public Form1()
        {
            InitializeComponent();
            InitializeDevicesList();            
            imgDataStream = null;
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);            
            sendToAddress = IPAddress.Parse("127.0.0.1");
            //sendToAddress = IPAddress.Parse("192.168.0.101");
            sendingEndPoint = new IPEndPoint(sendToAddress, 11000);
            transmission = new Thread(this.sendFromBuffer);
            buffering = new Thread(this.FillBuffer);
            buffering.SetApartmentState(ApartmentState.STA); //this is reqd as this thread gets data from clipboard
            CheckForIllegalCrossThreadCalls = false;

            saveByteArray = new List<byte[]>();
            saveFrameSize = new List<int>();

            for (int oldFileCount = 0; oldFileCount <= 10; oldFileCount++)
            {
                String oldvidFilename = "Video" + oldFileCount.ToString() + ".dat";
                String oldsizeFilename = "Siz" + oldFileCount.ToString() + ".dat";

                if (File.Exists(oldsizeFilename))
                {
                    File.Delete(oldvidFilename);
                    File.Delete(oldsizeFilename);
                }
            }
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
                try
                {
                    ((CaptureDevice)cboDevices.SelectedItem).Attach(pbImage);

                    if (transmission.ThreadState.ToString().Equals("Unstarted"))
                        transmission.Start();
                    if (buffering.ThreadState.ToString().Equals("Unstarted"))
                        buffering.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        

        private void FillBuffer()
        {

            while (true)
            {
                try
                {


                    image = ((CaptureDevice)cboDevices.SelectedItem).Capture();
                    System.Threading.Thread.Sleep(33);
                    if (image != null)
                    {
                        imgDataStream = new MemoryStream();
                        image.Save(imgDataStream, ImageFormat.Jpeg);
                        sendBuffer = imgDataStream.ToArray();
                        try
                        {
                           saveToBuffer(sendBuffer);
                        }
                        catch {}
                        imgDataStream.Close();
                    }
                }
                catch {}
            }
        }

        private void saveToBuffer(byte[] sendBuffer)
        {
            try
            {
                saveByteArray.Add(sendBuffer);
                saveFrameSize.Add(sendBuffer.Length);
                if (saveByteArray.Count >= 40)
                {
                    fileCount++;
                    if (fileCount > 10)
                        fileCount = 1;

                    String vidFilename = "Video" + fileCount.ToString() + ".dat";
                    String sizeFilename = "Siz" + fileCount.ToString() + ".dat";
                    //Console.WriteLine("Save"+vidFilename);


                    FileStream vidFileStream = new FileStream(vidFilename, FileMode.Create, FileAccess.Write);
                    StreamWriter sizeFileStream = new StreamWriter(sizeFilename);

                    foreach (byte[] byteArrayElement in saveByteArray)
                        vidFileStream.Write(byteArrayElement, 0, byteArrayElement.Length);


                    foreach (int sizeArrayElement in saveFrameSize)
                        sizeFileStream.WriteLine(sizeArrayElement);


                    saveByteArray.Clear();
                    saveFrameSize.Clear();
                    //filesBuffered++;
                    filesBuffered = filesBuffered > 5 ? 5 : filesBuffered+1;
                    vidFileStream.Close();
                    sizeFileStream.Close();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Buffering: "+ex.Message);
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
            int fileCount = 1;

            while (true)
            {
                try
                {
                    if (filesBuffered > 1)
                    {
                        vidFilename = "Video" + fileCount.ToString() + ".dat";
                        sizeFilename = "Siz" + fileCount.ToString() + ".dat";
                        vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
                        sizeReader = new StreamReader(sizeFilename);
                        s = sizeReader.ReadLine();
                        size = int.Parse(s);

                        /*int oldFileCount = fileCount > 5 ? fileCount - 5 : (10 + fileCount - 5);
                        String oldvidFilename = "Video" + oldFileCount.ToString() + ".dat";
                        String oldsizeFilename = "Siz" + oldFileCount.ToString() + ".dat";

                        if (File.Exists(oldsizeFilename))
                        {
                            File.Delete(oldvidFilename);
                            File.Delete(oldsizeFilename);
                        }*/
                        fileCount++;
                        if (fileCount > 10)
                            fileCount = 1;


                        while (s != null)
                        {
                            System.Threading.Thread.Sleep(10);
                            frameData = new byte[size];
                            vidFileStream.Read(frameData, 0, size);
                            sendingSocket.SendTo(frameData, sendingEndPoint);
                            s = sizeReader.ReadLine();
                            try
                            {
                                size = int.Parse(s);
                            }
                            catch { ;}
                        }

                        sizeReader.Close();
                        vidFileStream.Close();
                        try
                        {
                            File.Delete(vidFilename);
                            File.Delete(sizeFilename);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("File Delete Error: " + ex.Message);
                        }
                    }
                }

                catch (FileNotFoundException)
                {
                    System.Threading.Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Transmission: " + ex.Message);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ((CaptureDevice)cboDevices.SelectedItem).Detach();
            sendingSocket.Close();
            sendingSocket.Dispose();
            transmission.Abort();
            buffering.Abort();
            
        }

        /*private void btnStart_Click(object sender, EventArgs e)
        {
            
            
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
            
        }*/
    }
}
