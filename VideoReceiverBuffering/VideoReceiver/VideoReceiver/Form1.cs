using System;
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
        const int listenPort = 11000;
        UdpClient listener;
        IPAddress receiveAddress;
        IPEndPoint groupEP;
        byte[] receiveByteArray;
        MemoryStream imgStream;
        Bitmap bmpImage;
        Thread buffering;
        Thread displaying;
        CircularBuffer frameBuffer;
        //DynamicMethod assemblyMethod;
        //ILGenerator assemblyGen;
        //System.Threading.Timer delay;
        //System.Threading.TimerCallback delayCallBack;

        int d; //for storing difference betn head and tail of buffer

        DateTime dt1, dt2;
        TimeSpan ts;

        //ApplicationContext a;

        public Form1()
        {
            InitializeComponent();
            listener = new UdpClient(listenPort);
            receiveAddress = IPAddress.Parse("127.0.0.1");
            //receiveAddress = IPAddress.Parse("131.216.16.125");
            groupEP = new IPEndPoint(receiveAddress, listenPort);
            receiveByteArray = null;
            imgStream = null;
            bmpImage = null;
            frameBuffer = new CircularBuffer();

            displaying = new Thread(this.Display);
            buffering = new Thread(this.FillBuffer);
            //delayCallBack = new TimerCallback(GenerateDelay);

            //assemblyMethod = new DynamicMethod("NoOperation", null, Type.EmptyTypes);
            //assemblyGen = assemblyMethod.GetILGenerator();
            this.resetProgress();

            CheckForIllegalCrossThreadCalls = false;


            SetDoubleBuffered(pbFrame);
            

            //Console.WriteLine(this.CPUSpeed());            
            /*
            DateTime dt1 = DateTime.Now;            
            for (int i = 0; i < 1000000; i++) { }
            DateTime dt2 = DateTime.Now;
            TimeSpan ts = dt2 - dt1;
            Console.WriteLine(ts.Milliseconds);
            */
        }

        private void resetProgress()
        {
            progBuffer.Minimum = 1;
            progBuffer.Maximum = 2000;
            progBuffer.Value = 1;
            progBuffer.Step = 1;

            progDisplay.Minimum = 1;
            progDisplay.Maximum = 2000;
            progDisplay.Value = 1;
            progDisplay.Step = 1;
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        /*private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                {
                    receiveByteArray = listener.Receive(ref groupEP);
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
            
        }*/

        private void btnStart_Click(object sender, EventArgs e)
        {
            //start = true;
            int d = 0;
            try
            {
                 
                if (buffering.ThreadState.ToString().Equals("Unstarted"))
                    buffering.Start();
                if (displaying.ThreadState.ToString().Equals("Unstarted"))
                    displaying.Start();
                /*
                //while (true)
                //{
                
                //Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                //{

                    d = frameBuffer.diff();
                    //Console.WriteLine(d);
                    if (d < 10)
                    {
                        //Console.WriteLine("Buffering..");
                        displaying.Suspend();
                    }
                    else
                    {
                        if (displaying.ThreadState.ToString().Equals("Suspended"))
                            displaying.Resume();
                    }

                //});
               // }*/
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Display()
        {
            //int count = 100;

            Console.WriteLine("Display...");
            while (true)
            {
                //Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                //{
                if (frameBuffer.size() >= 100)
                {
                    try
                    {
                        //if (count > 0)
                        //{
                        imgStream = new MemoryStream(frameBuffer.read());
                        bmpImage = new Bitmap(imgStream);
                        imgStream.Close();
                        if (bmpImage != null)
                        {
                            pbFrame.Image = bmpImage;
                            pbFrame.Refresh();
                            progDisplay.PerformStep();                            
                        }

                        //}
                        //count--;

                        dt1 = DateTime.Now;
                        //delay = new System.Threading.Timer(delayCallBack, null, 0, 5000);
                        Thread.Sleep(33);
                        dt2 = DateTime.Now;
                        ts = dt2 - dt1;
                        //Console.WriteLine(ts.Milliseconds);
                        //delay.Dispose();
                            


                        //Thread.Sleep(15);
                        /*dt1 = DateTime.Now;
                        for (int i = 0; i < 300000; i++)
                        {
                            assemblyGen.Emit(OpCodes.Nop);
                        }
                        dt2 = DateTime.Now;
                        ts = dt2 - dt1;*/
                        //Console.WriteLine(this.CPUSpeed());
                        //Console.WriteLine(ts.Milliseconds);


                            
                    }
                    catch(Exception ex) 
                    {
                        Console.WriteLine("Display " + ex.Message);
                        //Console.Clear();
                    }
                }
            }
           // });            

        }

        /*private void GenerateDelay(Object o)
        {
            ;
        }*/

        private void FillBuffer()
        {
            Console.WriteLine("filling...");            
            while (true)
            {
                // Application.Idle += new EventHandler(delegate(object sender1, EventArgs e1)
                // {
                try
                {
                    receiveByteArray = listener.Receive(ref groupEP);
                    Console.WriteLine("receive: " + receiveByteArray.Length);
                    frameBuffer.enqueue(ref receiveByteArray);

                    d = frameBuffer.diff();
                    if (d < 10)
                    {
                        displaying.Suspend();
                    }
                    else
                    {
                        if (displaying.ThreadState.ToString().Equals("Suspended"))
                            displaying.Resume();
                    }
                    
                    if (progBuffer.Value >= progBuffer.Maximum)
                        this.resetProgress();
                    else
                        progBuffer.PerformStep();                   
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Buffer " + ex.Message);                    
                }
            }

            //});
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            try
            {
                //start = false;
                buffering.Abort();
                //while (buffering.IsAlive) { }
                if (displaying.ThreadState.ToString().Equals("Suspended"))
                    displaying.Resume();
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
            
            /*Application.ApplicationExit += new EventHandler(delegate(object sender1, EventArgs e1)
            {
                //System.Console.WriteLine("socket closed");
                
                try
                {
                    buffering.Abort();
                    displaying.Abort();
                    listener.Close();                    
                }
                catch { }
            });*/
        }

        private uint CPUSpeed()
        {
            ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
            uint sp = (uint)(Mo["CurrentClockSpeed"]);
            Mo.Dispose();
            return sp;
        }

        
    }
}
