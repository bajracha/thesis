using System;
//using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace VideoReceiver
{
    public partial class Form1 : Form
    {
        const int listenPort = 11000;
        UdpClient listener;
        IPAddress receiveAddress;
        IPEndPoint groupEP;        
        byte[] receiveByteArray;
        List<byte[]> saveByteArray;
        List<int> saveFrameSize;
        MemoryStream imgStream;
        Bitmap bmpImage;
        int fileCount;

        public Form1()
        {
            InitializeComponent();
            listener = new UdpClient(listenPort);
            //receiveAddress = IPAddress.Parse("127.0.0.1");
            receiveAddress = IPAddress.Parse("192.168.0.101");
            groupEP = new IPEndPoint(receiveAddress, listenPort);            
            imgStream = null;
            bmpImage = null;
            saveByteArray = new List<byte[]>();
            saveFrameSize = new List<int>();
            fileCount = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                {
                    receiveByteArray = listener.Receive(ref groupEP);
                    //Console.WriteLine(receiveByteArray.Length.ToString());

                    //code to save data
                    saveByteArray.Add(receiveByteArray);
                    saveFrameSize.Add(receiveByteArray.Length);
                    //saveFrameSize.Add(BitConverter.GetBytes(receiveByteArray.Length));
                    //Console.WriteLine(saveFrameSize[0].ToString());
                    if (saveByteArray.Count >= 40)
                    {
                        fileCount++;
                        String vidFilename = Directory.GetCurrentDirectory()+"Video" + fileCount.ToString() + ".dat";
                        String sizeFilename = Directory.GetCurrentDirectory()+"Siz" + fileCount.ToString() + ".dat";
                        FileStream vidFileStream = new FileStream(vidFilename, FileMode.Create, FileAccess.Write);
                        StreamWriter sizeFileStream = new StreamWriter(sizeFilename);
                        
                        foreach( byte[] byteArrayElement in saveByteArray )                         
                            vidFileStream.Write(byteArrayElement, 0, byteArrayElement.Length);

                        
                        foreach (int sizeArrayElement in saveFrameSize)
                            sizeFileStream.WriteLine(sizeArrayElement);
                       
                           
                        saveByteArray.Clear();
                        saveFrameSize.Clear();
                        vidFileStream.Close();
                        sizeFileStream.Close();
                    }

                    //code to display live data
                    imgStream = new MemoryStream(receiveByteArray);
                    bmpImage = new Bitmap(imgStream);
                    imgStream.Close();
                    if (bmpImage != null)
                    {
                        pbFrame.Image = bmpImage;
                    }
                });
            }
            catch
            {
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Application.ApplicationExit += new EventHandler(delegate(object sender1, EventArgs e1)
            {
                //System.Console.WriteLine("socket closed");
                listener.Close();
            });
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            /*
            String vidFilename = "C:\\Users\\Mahesh\\Documents\\Vid1.dat";
            String sizeFilename = "C:\\Users\\Mahesh\\Documents\\Siz1.dat";
            FileStream vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
            FileStream sizeFileStream = new FileStream(sizeFilename, FileMode.Open, FileAccess.Read);
            StreamReader sizeReader = new StreamReader(sizeFilename);
            String s = sizeReader.ReadLine();
            int size = int.Parse(s);
            byte[] frameData;
            MemoryStream frameStream;
            Bitmap frame;

            try
            {
                Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                {
                    if (s != null)
                    {
                        frameData = new byte[size];
                        vidFileStream.Read(frameData, 0, size);
                        frameStream = new MemoryStream(frameData);
                        frame = new Bitmap(frameStream);
                        frameStream.Close();
                        pbPlayback.Image = frame;                       
                        s = sizeReader.ReadLine();
                        try
                        {
                            size = int.Parse(s);
                        }
                        catch { ;}
                    }

                });
            }
            catch { ;}
            finally
            {
                //sizeReader.Close();
            }
            */

            String vidFilename;
            String sizeFilename;
            FileStream vidFileStream;            
            StreamReader sizeReader;
            String s;
            int size;
            byte[] frameData;
            MemoryStream frameStream;
            Bitmap frame;
            int fileCount = 0;

            try
            {
                fileCount++;
                vidFilename = Directory.GetCurrentDirectory()+"Video" + fileCount.ToString() + ".dat";
                sizeFilename = Directory.GetCurrentDirectory()+"Siz" + fileCount.ToString() + ".dat";
                vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
                sizeReader = new StreamReader(sizeFilename);
                s = sizeReader.ReadLine();
                size = int.Parse(s);        

                Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                {                        

                    if (s != null)
                    {
                        frameData = new byte[size];
                        vidFileStream.Read(frameData, 0, size);
                        frameStream = new MemoryStream(frameData);
                        frame = new Bitmap(frameStream);
                        frameStream.Close();
                        pbPlayback.Image = frame;
                        s = sizeReader.ReadLine();
                        try
                        {
                            size = int.Parse(s);
                        }
                        catch { ;}
                    }
                    else
                    {
                        vidFileStream.Close();
                        sizeReader.Close();
                        fileCount++;
                        vidFilename = Directory.GetCurrentDirectory()+"Video" + fileCount.ToString() + ".dat";
                        sizeFilename = Directory.GetCurrentDirectory()+"Siz" + fileCount.ToString() + ".dat";
                        vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
                        sizeReader = new StreamReader(sizeFilename);
                        s = sizeReader.ReadLine();
                        try
                        {
                            size = int.Parse(s);
                        }
                        catch { ;}
                    }

                });
            }
            catch { ;}           

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        

        
    }
}
