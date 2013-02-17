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

namespace VideoReceiver
{
    public partial class Form1 : Form
    {
        Socket sendingSocket;
        IPAddress sendToAddress;
        IPEndPoint sendingEndPoint;        
        
        Thread buffering;
        Thread displaying;   
       
        Thread bufferingDB;
        Thread displayingDB;       

        NetworkDataAccess liveData, recordedData;

        public Form1()
        {
            InitializeComponent();            

            liveData = new NetworkDataAccess(true, "Video", "Siz");
            liveData.setListenerSettings(AppSettingsController.GetAppSetting("receiveFromAddress", "127.0.0.1"), AppSettingsController.GetAppSetting("ListenPort", 11000));

            recordedData = new NetworkDataAccess(false, "DB_Video", "DB_Siz");
            recordedData.setListenerSettings(AppSettingsController.GetAppSetting("receiveFromAddressDB", "127.0.0.1"), AppSettingsController.GetAppSetting("ListenPortDB", 11001));

            displaying = new Thread(this.Display);
            buffering = new Thread(this.FillBuffer);            

            this.resetProgress(1,1);

            displayingDB = new Thread(this.DisplayDB);
            bufferingDB = new Thread(this.FillBufferDB);            

            this.resetProgressDB(1, 1);

            CheckForIllegalCrossThreadCalls = false;


            SetDoubleBuffered(pbFrame);
            SetDoubleBuffered(pbFrameDB);          
            
            
        }       

        public void updateProgress() 
        {
            if (progBuffer.Value >= progBuffer.Maximum)
                //this.resetProgress(Math.Abs(progBuffer.Value-progDisplay.Value),1);
                this.resetProgress(1, 1);
            else
                progBuffer.PerformStep();
        }

        public void updateProgressDisplay()
        {
            progDisplay.PerformStep();
        }

        public void updateProgressDB()
        {
            if (progBufferDB.Value >= progBufferDB.Maximum)
                //this.resetProgress(Math.Abs(progBuffer.Value-progDisplay.Value),1);
                this.resetProgress(1, 1);
            else
                progBufferDB.PerformStep();
        }

        public void updateProgressDisplayDB()
        {
            progDisplayDB.PerformStep();
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

        private void resetProgressDB(int buff, int dis)
        {
            progBufferDB.Minimum = 1;
            progBufferDB.Maximum = 2000;
            progBufferDB.Value = buff;
            progBufferDB.Step = 1;

            progDisplayDB.Minimum = 1;
            progDisplayDB.Maximum = 2000;
            progDisplayDB.Value = dis;
            progDisplayDB.Step = 1;
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
                liveData.clearData();                
                this.resetProgress(1, 1);                
                pbFrame.Image = null;
                liveData.deleteAllFiles();
                
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
            liveData.Display();
        }

        

        private void FillBuffer()
        {
            liveData.FillBuffer();            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            try
            {                
                buffering.Abort();                
                displaying.Abort();                 
                liveData.CloseListener();
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
            string ip = AppSettingsController.GetAppSetting("sendToAddress", "127.0.0.1");
            int port = AppSettingsController.GetAppSetting("sendToPort", 12000);
            Console.WriteLine(dayPicker.Value.Date);
            Console.WriteLine(hourCombo.SelectedItem);
            Console.WriteLine(mincombo.SelectedItem);
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendToAddress = IPAddress.Parse(ip);
            UTF8Encoding encoding = new UTF8Encoding();
            sendingEndPoint = new IPEndPoint(sendToAddress, port);
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

            bufferingDB.Abort();
            displayingDB.Abort();
            recordedData.clearData();            
            this.resetProgressDB(1, 1);            
            pbFrameDB.Image = null;
            recordedData.deleteAllFiles();
            displayingDB = new Thread(this.DisplayDB);
            bufferingDB = new Thread(this.FillBufferDB);
            displayingDB.Start();
            bufferingDB.Start();
        }

        private void stopbutton_Click(object sender, EventArgs e) 
        {
            byte[] send_buffer;
            string ip = AppSettingsController.GetAppSetting("sendToAddress", "127.0.0.1");
            int port = AppSettingsController.GetAppSetting("sendToPort", 12000);
            Console.WriteLine(dayPicker.Value.Date);
            Console.WriteLine(hourCombo.SelectedItem);
            Console.WriteLine(mincombo.SelectedItem);
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendToAddress = IPAddress.Parse(ip);
            UTF8Encoding encoding = new UTF8Encoding();
            sendingEndPoint = new IPEndPoint(sendToAddress, port);
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

            recordedData.clearData();            
            this.resetProgressDB(1, 1);
            bufferingDB.Abort();
            displayingDB.Abort();            
            pbFrameDB.Image = null;
        }

        private void DisplayDB()
        {
            recordedData.Display();            
        }



        private void FillBufferDB()
        {
            recordedData.FillBuffer();       

        }
        
    }
}
