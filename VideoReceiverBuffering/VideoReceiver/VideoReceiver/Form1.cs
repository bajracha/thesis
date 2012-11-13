﻿using System;
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
using System.Threading;
using System.Management;
using System.Reflection;
//using System.Reflection.Emit;

namespace VideoReceiver
{
    public partial class Form1 : Form
    {
        Socket sendingSocket;
        IPAddress sendToAddress;
        IPEndPoint sendingEndPoint;

        const int listenPort = 11000;
        UdpClient listener;
        IPAddress receiveAddress;
        IPEndPoint groupEP;
        byte[] receiveByteArray;
        List<byte[]> saveByteArray;
        List<int> saveFrameSize;
        //MemoryStream imgStream;
        //Bitmap bmpImage;
        Thread buffering;
        Thread displaying;
        //CircularBuffer frameBuffer;

        int fileCount = 0;
        int filesBuffered = 0;
        //Boolean[] isBuffered;        

        public Form1()
        {
            InitializeComponent();
            listener = new UdpClient(listenPort);
            receiveAddress = IPAddress.Parse("127.0.0.1");
            //receiveAddress = IPAddress.Parse("131.216.16.125");
            groupEP = new IPEndPoint(receiveAddress, listenPort);
            receiveByteArray = null;
            //imgStream = null;
            //bmpImage = null;
            //frameBuffer = new CircularBuffer();

            displaying = new Thread(this.Display);
            buffering = new Thread(this.FillBuffer);
            saveByteArray = new List<byte[]>();
            saveFrameSize = new List<int>();

            this.resetProgress(1,1);

            CheckForIllegalCrossThreadCalls = false;


            SetDoubleBuffered(pbFrame);

            //isBuffered = new Boolean[20];
            
            
        }

        private void deleteAllFiles() {
            for (int oldFileCount = 1; oldFileCount <= 20; oldFileCount++)
            {
                String oldvidFilename = "Video" + oldFileCount.ToString() + ".dat";
                String oldsizeFilename = "Siz" + oldFileCount.ToString() + ".dat";

                if (File.Exists(oldsizeFilename))
                {
                    File.Delete(oldvidFilename);
                    File.Delete(oldsizeFilename);
                }

                //isBuffered[oldFileCount - 1] = false;
            }
        }

        private void resetProgress(int buff, int dis)
        {
            progBuffer.Minimum = 1;
            progBuffer.Maximum = 2000;
            progBuffer.Value = buff;
            progBuffer.Step = 1;

            progDisplay.Minimum = 1;
            progDisplay.Maximum = 2000;
            progDisplay.Value = dis;
            progDisplay.Step = 1;
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

       
            
        

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            try
            {
                Console.WriteLine("Status_>"+displaying.IsAlive);
                if (!displaying.IsAlive)
                {
                    byte[] send_buffer;
                    Console.WriteLine(dayPicker.Value.Date);
                    Console.WriteLine(hourCombo.SelectedItem);
                    Console.WriteLine(mincombo.SelectedItem);
                    sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sendToAddress = IPAddress.Parse("127.0.0.1");
                    UTF8Encoding encoding = new UTF8Encoding();
                    sendingEndPoint = new IPEndPoint(sendToAddress, 12000);
                    send_buffer = encoding.GetBytes("sender->" + sendToAddress.ToString());
                    sendingSocket.SendTo(send_buffer, sendingEndPoint);
                    send_buffer = encoding.GetBytes("date->" + dayPicker.Value.Date.ToString());
                    sendingSocket.SendTo(send_buffer, sendingEndPoint);
                    send_buffer = encoding.GetBytes("hour->" + hourCombo.SelectedItem.ToString());
                    sendingSocket.SendTo(send_buffer, sendingEndPoint);
                    send_buffer = encoding.GetBytes("min->" + mincombo.SelectedItem.ToString());
                    sendingSocket.SendTo(send_buffer, sendingEndPoint);
                    send_buffer = encoding.GetBytes("status->sendlive");
                    sendingSocket.SendTo(send_buffer, sendingEndPoint);

                    buffering.Abort();
                    displaying.Abort();
                    saveByteArray.Clear();
                    saveFrameSize.Clear();
                    this.resetProgress(1, 1);
                    fileCount = 0;
                    filesBuffered = 0;
                    pbFrame.Image = null;
                    deleteAllFiles();
                    displaying = new Thread(this.Display);
                    buffering = new Thread(this.FillBuffer);
                    displaying.Start();
                    buffering.Start();
                }
                if (buffering.ThreadState.ToString().Equals("Unstarted"))
                    buffering.Start();                
                if (displaying.ThreadState.ToString().Equals("Unstarted"))
                    displaying.Start();           
               
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Display()
        {
            String vidFilename;
            String sizeFilename;
            String s;
            int size;
            byte[] frameData;
            MemoryStream frameStream;
            Bitmap frame;
            int fileCount = 1;

               

            Console.WriteLine("Display...");
            

            while (true)
            {

                try
                {
                    if (filesBuffered > 3)
                    {
                        
                        //if (isBuffered[fileCount - 1])
                        //{
                        
                            vidFilename = "Video" + fileCount.ToString() + ".dat";
                            sizeFilename = "Siz" + fileCount.ToString() + ".dat";
                            FileStream vidFileStream = new FileStream(vidFilename, FileMode.Open, FileAccess.Read);
                            StreamReader sizeReader = new StreamReader(sizeFilename);
                            try
                            {
                            s = sizeReader.ReadLine();
                            size = int.Parse(s);

                            /*int oldFileCount = fileCount > 10 ? fileCount - 10 : (20 + fileCount - 10);
                            String oldvidFilename = "Video" + oldFileCount.ToString() + ".dat";
                            String oldsizeFilename = "Siz" + oldFileCount.ToString() + ".dat";

                            if (File.Exists(oldsizeFilename))
                            {
                                File.Delete(oldvidFilename);
                                File.Delete(oldsizeFilename);
                            }*/

                            fileCount++;
                            if (fileCount > 20)
                                fileCount = 1;

                            while (s != null)
                            {
                                frameData = new byte[size];
                                vidFileStream.Read(frameData, 0, size);
                                frameStream = new MemoryStream(frameData);
                                frame = new Bitmap(frameStream);
                                frameStream.Close();
                                pbFrame.Image = frame;
                                pbFrame.Refresh();
                                Thread.Sleep(150);
                                progDisplay.PerformStep();
                                s = sizeReader.ReadLine();
                                try
                                {
                                    size = int.Parse(s);
                                }
                                catch { ;}
                            }
                            vidFileStream.Close();
                            sizeReader.Close();
                            //lock (isBuffered)
                            //{
                            //    isBuffered[fileCount - 1] = false;
                            //}
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
                        catch(Exception ex){
                        }
                        finally{
                            vidFileStream.Close();
                            sizeReader.Close(); 
                        }

                    }
                }
                catch (FileNotFoundException) { Thread.Sleep(200); }
                catch (Exception ex)
                {                    
                    Console.WriteLine("Display " + ex.Message);                    
                }


               
            }
                       

        }

        

        private void FillBuffer()
        {
            Console.WriteLine("filling...");
            
            while (true)
            {
                
                try
                {
                    receiveByteArray = listener.Receive(ref groupEP);
                    saveByteArray.Add(receiveByteArray);
                    saveFrameSize.Add(receiveByteArray.Length);
                    //Console.WriteLine(saveByteArray.Count);
                    if (saveByteArray.Count >= 40)
                    {
                        fileCount++;
                        //if (!isBuffered[fileCount - 1])
                        //{
                            if (fileCount > 20)
                                fileCount = 1;

                            String vidFilename = "Video" + fileCount.ToString() + ".dat";
                            String sizeFilename = "Siz" + fileCount.ToString() + ".dat";
                            FileStream vidFileStream = new FileStream(vidFilename, FileMode.Create, FileAccess.Write);
                            StreamWriter sizeFileStream = new StreamWriter(sizeFilename);
                            try
                            {

                                foreach (byte[] byteArrayElement in saveByteArray)
                                    vidFileStream.Write(byteArrayElement, 0, byteArrayElement.Length);


                                foreach (int sizeArrayElement in saveFrameSize)
                                    sizeFileStream.WriteLine(sizeArrayElement);
                            }
                            catch (Exception ex)
                            {
                            }
                            finally {
                                saveByteArray.Clear();
                                saveFrameSize.Clear();
                                filesBuffered = filesBuffered > 5 ? 5 : filesBuffered + 1;
                                vidFileStream.Close();
                                sizeFileStream.Close();

                            }
                            
                            //lock (isBuffered)
                            //{
                             //   isBuffered[fileCount - 1] = true;
                            //}

                            


                        //}
                    }
                    if (progBuffer.Value >= progBuffer.Maximum)
                        //this.resetProgress(Math.Abs(progBuffer.Value-progDisplay.Value),1);
                        this.resetProgress(1,1);
                    else
                        progBuffer.PerformStep();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Buffer " + ex.Message);                    
                }
            }

            
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            try
            {
                //start = false;
                buffering.Abort();                
                displaying.Abort();
                //while (displaying.IsAlive) { }
                listener.Close();               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Application.Exit();
            }
            
            
        }

        private uint CPUSpeed()
        {
            ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
            uint sp = (uint)(Mo["CurrentClockSpeed"]);
            Mo.Dispose();
            return sp;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void datetimebutton_Click(object sender, EventArgs e)
        {
            byte[] send_buffer; 
            Console.WriteLine(dayPicker.Value.Date);
            Console.WriteLine(hourCombo.SelectedItem);
            Console.WriteLine(mincombo.SelectedItem);
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendToAddress = IPAddress.Parse("127.0.0.1");
            UTF8Encoding encoding = new UTF8Encoding();
            sendingEndPoint = new IPEndPoint(sendToAddress, 12000);
            send_buffer = encoding.GetBytes("sender->"+sendToAddress.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("date->" + dayPicker.Value.Date.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("hour->" + hourCombo.SelectedItem.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("min->" + mincombo.SelectedItem.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("status->send");
            sendingSocket.SendTo(send_buffer, sendingEndPoint);

            buffering.Abort();
            displaying.Abort();
            saveByteArray.Clear();
            saveFrameSize.Clear();
            this.resetProgress(1, 1);
            fileCount = 0;
            filesBuffered = 0;
            pbFrame.Image = null;
            deleteAllFiles();
            displaying = new Thread(this.Display);
            buffering = new Thread(this.FillBuffer);
            displaying.Start();
            buffering.Start();
        }

        private void stopbutton_Click(object sender, EventArgs e)
        {
            byte[] send_buffer;
            Console.WriteLine(dayPicker.Value.Date);
            Console.WriteLine(hourCombo.SelectedItem);
            Console.WriteLine(mincombo.SelectedItem);
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendToAddress = IPAddress.Parse("127.0.0.1");
            UTF8Encoding encoding = new UTF8Encoding();
            sendingEndPoint = new IPEndPoint(sendToAddress, 12000);
            send_buffer = encoding.GetBytes("sender->" + sendToAddress.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("date->" + dayPicker.Value.Date.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("hour->" + hourCombo.SelectedItem.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("min->" + mincombo.SelectedItem.ToString());
            sendingSocket.SendTo(send_buffer, sendingEndPoint);
            send_buffer = encoding.GetBytes("status->stop");
            sendingSocket.SendTo(send_buffer, sendingEndPoint);

            saveByteArray.Clear();
            saveFrameSize.Clear();
            this.resetProgress(1, 1);
            buffering.Abort();
            displaying.Abort();
            //deleteAllFiles();
            fileCount = 0;
            filesBuffered = 0;
            pbFrame.Image = null;
        }

        
    }
}
