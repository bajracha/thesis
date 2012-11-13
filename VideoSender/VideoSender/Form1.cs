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

using MySql.Data.MySqlClient;
using System.Configuration;

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
        Thread receiver;


        //For database
        string ConnectionString = "SERVER=localhost;DATABASE=thesis;UID=root;PASSWORD=;";
        MySqlConnection connection;
        MySqlDataAdapter adapter;
        DataTable DTItems;

        Thread txfromDB; 
        Boolean sendFromDB = true;
        String sendToIP = "";
        DateTime sendToTime;

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
            
            receiver = new Thread(this.receiveData);
            
            receiver.Start();
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
                DateTime currenttime = System.DateTime.Now;
                if (saveByteArray.Count==0) 
                    currenttime=System.DateTime.Now;
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

                    vidFileStream.Close();

                    foreach (int sizeArrayElement in saveFrameSize)
                        sizeFileStream.WriteLine(sizeArrayElement);

                    
                    sizeFileStream.Close();
                    //Save to database
                    connection = new MySqlConnection(ConnectionString);
                    connection.Open();
                    try
                    {
                        /*string query = "select * from video_detail";
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        adapter = new MySqlDataAdapter(query, connection);
                        DataSet DS = new DataSet();


                        //get query results in dataset
                        adapter.Fill(DS);
                        MySqlDataReader dataReader = cmd.ExecuteReader();

                        //Read the data and store them in the list
                        while (dataReader.Read())
                        {
                            Console.WriteLine(dataReader["file_name"]);
                        }*/
                        byte[] content=System.IO.File.ReadAllBytes(vidFilename);
                        Console.WriteLine(content.Length);

                        MySqlCommand command = new MySqlCommand("", connection);
                        command.CommandText = "insert into video_detail(file_name, file_content, size, file_order, timestamp, sender_ip) values (@name, @content, @size, @order, @time, @ip);";
                        command.Parameters.AddWithValue("@name", vidFilename);
                        command.Parameters.AddWithValue("@content", content);
                        command.Parameters.AddWithValue("@size", 1024);
                        command.Parameters.AddWithValue("@order", 2);
                        command.Parameters.AddWithValue("@time", currenttime);
                        command.Parameters.AddWithValue("@ip", "127.0.0.1");
                        
                        command.ExecuteNonQuery();

                        MySqlCommand idcmd = new MySqlCommand("SELECT last_insert_id()", connection);
                        MySqlDataReader dataReader = idcmd.ExecuteReader(CommandBehavior.SequentialAccess);
                        dataReader.Read();
                        int videoID = dataReader.GetInt16(0);
                        Console.WriteLine(videoID);
                        dataReader.Close();
                        

                        MySqlCommand sizecommand = new MySqlCommand("", connection);
                        sizecommand.CommandText = "insert into video_size_detail(file_name, file_content, size, timestamp, sender_ip) values (@name, @content, @order, @time, @ip);";
                        sizecommand.Parameters.AddWithValue("@name", sizeFilename);
                        sizecommand.Parameters.AddWithValue("@content", System.IO.File.ReadAllBytes(sizeFilename));
                        sizecommand.Parameters.AddWithValue("@size", 1024);
                        sizecommand.Parameters.AddWithValue("@order", 2);
                        sizecommand.Parameters.AddWithValue("@time", currenttime);
                        sizecommand.Parameters.AddWithValue("@ip", "127.0.0.1");

                        sizecommand.ExecuteNonQuery();

                        MySqlCommand sizeidcmd = new MySqlCommand("SELECT last_insert_id()", connection);
                        MySqlDataReader sizedataReader = sizeidcmd.ExecuteReader(CommandBehavior.SequentialAccess);
                        sizedataReader.Read();
                        int sizeID = sizedataReader.GetInt16(0);
                        Console.WriteLine(sizeID);
                        sizedataReader.Close();

                        MySqlCommand timedivcommand = new MySqlCommand("", connection);
                        timedivcommand.CommandText = "insert into video_time_division(timestamp, video_id, size_id) values (@time, @videoid, @sizeid);";
                        timedivcommand.Parameters.AddWithValue("@time", currenttime);
                        timedivcommand.Parameters.AddWithValue("@videoid", videoID);
                        timedivcommand.Parameters.AddWithValue("@sizeid", sizeID);
                        timedivcommand.ExecuteNonQuery();


                        connection.Close();
                        

                        //adapter = new MySqlDataAdapter(query, connection);
                        

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error in database" + ex.Message);
                    }

                    saveByteArray.Clear();
                    saveFrameSize.Clear();
                    //filesBuffered++;
                    filesBuffered = filesBuffered > 5 ? 5 : filesBuffered+1;
                    
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Buffering: "+ex.Message);
            }
        }

        private void readandsendFromDB()
        {
            
            for (int oldFileCount = 0; oldFileCount <= 10; oldFileCount++)
            {
                String oldvidFilename = "dbVideo" + oldFileCount.ToString() + ".dat";
                String oldsizeFilename = "dbSiz" + oldFileCount.ToString() + ".dat";

                if (File.Exists(oldsizeFilename))
                {
                    File.Delete(oldvidFilename);
                    File.Delete(oldsizeFilename);
                }
            }

                //Read from database
                MySqlConnection readconnection = new MySqlConnection(ConnectionString);
                readconnection.Open();
                string query = "select video_id,size_id from video_time_division where timestamp between @time and @time + interval 10 minute";
                MySqlCommand cmd = new MySqlCommand(query, readconnection);
                //DateTime time=new DateTime(2012,11,9,11,20,0);
                cmd.Parameters.AddWithValue("@time",sendToTime);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    int sizeID=dataReader.GetInt16(1);
                    int videoID=dataReader.GetInt16(0);
                    
                MySqlConnection sizeconnection = new MySqlConnection(ConnectionString);
                sizeconnection.Open();

                query = "select * from video_size_detail where file_id=@id";
                MySqlCommand sizecmd = new MySqlCommand(query, sizeconnection);
                sizecmd.Parameters.AddWithValue("@id", sizeID);
                
                //get query results in dataset
                MySqlDataReader sizedataReader = sizecmd.ExecuteReader();

                //Read the data and store them in the list
                //            byte[] file = new byte[5];
                FileStream fs;
                BinaryWriter bw;
                int bufferSize = 100;
                byte[] outbyte = new byte[bufferSize];
                long startIndex = 0;
                long retval = 0;
                int fileID;
                while (sizedataReader.Read())
                {
                    fileID = (int)sizedataReader["file_id"];
                    String fileName = (String)sizedataReader["file_name"];
                    Console.WriteLine(fileName);
                    startIndex = 0;
                    fs = new FileStream("db" + fileName, FileMode.OpenOrCreate, FileAccess.Write);
                    bw = new BinaryWriter(fs);
                    retval = sizedataReader.GetBytes(2, startIndex, outbyte, 0, bufferSize);
                    while (retval == bufferSize)
                    {
                        bw.Write(outbyte);
                        bw.Flush();

                        // Reposition the start index to the end of the last buffer and fill the buffer.
                        startIndex += bufferSize;
                        retval = sizedataReader.GetBytes(2, startIndex, outbyte, 0, bufferSize);
                    }
                    bw.Write(outbyte, 0, (int)retval);
                    bw.Flush();
                    bw.Close();
                    fs.Close();
                    //Read the data and store them in the list
                    MySqlConnection connection1 = new MySqlConnection(ConnectionString);
                    connection1.Open();
                    query = "select * from video_detail where file_id=@id";

                    MySqlCommand cmd1 = new MySqlCommand(query, connection1);
                    cmd1.Parameters.AddWithValue("@id", videoID);
                    MySqlDataReader dataReader1 = cmd1.ExecuteReader(CommandBehavior.SequentialAccess);

                    //get query results in dataset
                    
                    while (dataReader1.Read())
                    {
                        Console.WriteLine(dataReader1["file_name"]);
                        StreamReader sizeReader = new StreamReader("db" + fileName);
                        String s = sizeReader.ReadLine();
                        int size = int.Parse(s);
                        try
                        {
                            startIndex = 0;
                            while (s != null)
                            {
                                //Console.WriteLine(size);
                                System.Threading.Thread.Sleep(100);
                                byte[] frameData = new byte[size];
                                //vidFileStream.Read(frameData, 0, size);
                                dataReader1.GetBytes(2, startIndex, frameData, 0, size);
                                startIndex += size;
                                /*MemoryStream frameStream = new MemoryStream(frameData);
                                Bitmap frame = new Bitmap(frameStream);
                                frameStream.Close();
                                pbImage.Image = frame;
                                pbImage.Refresh();*/
                                if (sendFromDB)
                                    sendingSocket.SendTo(frameData, sendingEndPoint);
                                else
                                    return;
                                s = sizeReader.ReadLine();
                                try
                                {
                                    size = int.Parse(s);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Sending error in readFromDB" + ex.Message); ;
                                    ;
                                }
                            }
                        }
                        finally{

                        sizeReader.Close();
                        }
                    }

                    
                    dataReader1.Close();
                    connection1.Close();
                    
                }
                sizeconnection.Close();
                sizedataReader.Close();

                


                }

                dataReader.Close();
                readconnection.Close();
                
            
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
                        Console.WriteLine("Sending from buffer");

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

        private void receiveData() {
            Console.WriteLine("Listening...");
            byte[] receiveByteArray;
            UdpClient listener;
            IPEndPoint groupEP;
            const int listenPort = 12000;
            IPAddress receiveAddress;
            listener = new UdpClient(listenPort);
            receiveAddress = IPAddress.Parse("127.0.0.1");
            groupEP = new IPEndPoint(receiveAddress, listenPort);
                    
            String sender="";
            String date="";
            String hour="";
            String min="";
            String status = "";
            while (true)
            {
                try
                {
                    receiveByteArray = listener.Receive(ref groupEP);
                    UTF8Encoding enc = new UTF8Encoding();
                    String data = enc.GetString(receiveByteArray);
                    if (data.Contains("sender"))
                    {
                        int index = data.IndexOf(">");
                        sender = data.Substring(index + 1, data.Length - index - 1);
                        for (int i = 0; i < 4; i++)
                        {
                            receiveByteArray = listener.Receive(ref groupEP);
                            data = enc.GetString(receiveByteArray);
                            index = data.IndexOf(">");
                            Console.WriteLine(data);

                            if (data.Contains("date")) date = data.Substring(index + 1, data.Length - index - 1);
                            else if (data.Contains("hour")) hour = data.Substring(index + 1, data.Length - index - 1);
                            else if (data.Contains("min")) min = data.Substring(index + 1, data.Length - index - 1);
                            else if (data.Contains("status")) status = data.Substring(index + 1, data.Length - index - 1);
                            Console.WriteLine("status" + status);

                        }
                     if (sender != "" && date != "" && hour != "" && min != "" && status.Equals("send"))
                     {
                         int year, month, day;
                         month = Convert.ToInt16(date.Substring(0, date.IndexOf("/")));
                         date = date.Substring(date.IndexOf("/") + 1, date.Length - date.IndexOf("/") - 1);
                         Console.WriteLine(date);
                         day = Convert.ToInt16(date.Substring(0, date.IndexOf("/")));
                         date = date.Substring(date.IndexOf("/") + 1, date.Length - date.IndexOf("/") - 1);
                         Console.WriteLine(date);
                         year = Convert.ToInt16(date.Substring(0, date.IndexOf(" ")));
                         DateTime dateTime = new DateTime(year, month, day, Convert.ToInt16(hour), Convert.ToInt16(min), 0);
                         Console.WriteLine(dateTime);
                         //sendingSocket.EndSend();
                         Console.WriteLine(transmission.ThreadState.ToString());
                         if (transmission.ThreadState.ToString().Equals("Running"))
                         {
                             Console.WriteLine("Running");
                             transmission.Abort();
                         }
                         sendFromDB = true;
                         sendToTime = dateTime;
                         sendToIP = sender;
                         txfromDB = new Thread(this.readandsendFromDB);             
                         txfromDB.Start();
                         //readandsendFromDB(sender, dateTime);
                         //transmission.Resume();
                         sender = "";
                         date = "";
                         hour = "";
                         min = "";
                     }
                     else if (status.Equals("sendlive"))
                     {
                         Console.WriteLine(transmission.ThreadState.ToString());
                         if (transmission.ThreadState.ToString().Equals("Aborted") || transmission.ThreadState.ToString().Equals("Stopped"))
                         {
                             transmission = new Thread(this.sendFromBuffer);
                             transmission.Start();
                         }
                     }
                     else if (status.Equals("stop"))
                     {
                         if (transmission.ThreadState.ToString().Equals("Running"))
                         {
                             Console.WriteLine("Running");
                             transmission.Abort();
                         }
                         txfromDB.Abort();
                         sendFromDB = false;
                     }
                        
                    }
                    
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
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
