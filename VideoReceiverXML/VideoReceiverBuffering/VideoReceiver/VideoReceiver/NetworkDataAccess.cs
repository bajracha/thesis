using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace VideoReceiver
{
    class NetworkDataAccess
    {

        int listenPort;
        UdpClient listener;
        IPAddress receiveAddress;
        IPEndPoint groupEP;
        byte[] receiveByteArray;
        List<byte[]> saveByteArray;
        List<int> saveFrameSize;
        Boolean isLive;
        String videoFilePrefix;
        String sizeFilePrefix;

        int fileCount = 0;
        int filesBuffered = 0;

        public NetworkDataAccess(Boolean isLive, String videoFilePrefix, String sizeFilePrefix)
        {
            saveByteArray = new List<byte[]>();
            saveFrameSize = new List<int>();
            this.isLive = isLive;
            this.videoFilePrefix = videoFilePrefix;
            this.sizeFilePrefix = sizeFilePrefix;
        }

        public void setListenerSettings(string ip, int port)
        {
            listenPort = port;
            listener = new UdpClient(listenPort);
            receiveAddress = IPAddress.Parse(ip);            
            groupEP = new IPEndPoint(receiveAddress, listenPort);
            receiveByteArray = null;
        }

        public void clearData()
        {
            try
            {
                saveByteArray.Clear();
                saveFrameSize.Clear();
                fileCount = 0;
                filesBuffered = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void deleteAllFiles()
        {
            int max = AppSettingsController.GetAppSetting("BufferFilesLimit", 20);
            for (int oldFileCount = 1; oldFileCount <= max; oldFileCount++)
            {
                String oldvidFilename = videoFilePrefix + oldFileCount.ToString() + ".dat";
                String oldsizeFilename = sizeFilePrefix + oldFileCount.ToString() + ".dat";

                if (File.Exists(oldsizeFilename))
                {
                    File.Delete(oldvidFilename);
                    File.Delete(oldsizeFilename);
                }

            }
        }

        public void FillBuffer()
        {
            int max = AppSettingsController.GetAppSetting("BufferFilesLimit", 20);
            int maxFrames = AppSettingsController.GetAppSetting("FramesInFileLimit", 40);
            Console.WriteLine("filling...");

            while (true)
            {

                try
                {
                    receiveByteArray = listener.Receive(ref groupEP);
                    saveByteArray.Add(receiveByteArray);
                    saveFrameSize.Add(receiveByteArray.Length);

                    if (saveByteArray.Count >= maxFrames)
                    {
                        fileCount++;

                        if (fileCount > max)
                            fileCount = 1;

                        String vidFilename = videoFilePrefix + fileCount.ToString() + ".dat";
                        String sizeFilename = sizeFilePrefix + fileCount.ToString() + ".dat";
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
                        finally
                        {
                            saveByteArray.Clear();
                            saveFrameSize.Clear();
                            filesBuffered = filesBuffered > 5 ? 5 : filesBuffered + 1;
                            vidFileStream.Close();
                            sizeFileStream.Close();

                        }


                    }

                    if(isLive)
                        Program.f.updateProgress();
                    else
                        Program.f.updateProgressDB();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Buffer " + ex.Message);
                }
            }
        }

        public void Display()
        {
            String vidFilename;
            String sizeFilename;
            String s;
            int size;
            byte[] frameData;
            MemoryStream frameStream;
            Bitmap frame;
            int fileCount = 1;
            int maxForDisplay = AppSettingsController.GetAppSetting("BufferLimitForDisplay", 3);
            int max = AppSettingsController.GetAppSetting("BufferFilesLimit", 20);
            int displayDelay = AppSettingsController.GetAppSetting("DisplayDelay", 300);
            int waitForFileDelay = AppSettingsController.GetAppSetting("WaitForFileDelay", 300);

            Console.WriteLine("Display...");


            while (true)
            {

                try
                {
                    if (filesBuffered > maxForDisplay)
                    {
                        vidFilename = videoFilePrefix + fileCount.ToString() + ".dat";
                        sizeFilename = sizeFilePrefix + fileCount.ToString() + ".dat";
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
                            if (fileCount > max)
                                fileCount = 1;

                            while (s != null)
                            {
                                frameData = new byte[size];
                                vidFileStream.Read(frameData, 0, size);
                                frameStream = new MemoryStream(frameData);
                                frame = new Bitmap(frameStream);
                                frameStream.Close();
                                if (isLive)
                                {
                                    Program.f.pbFrame.Image = frame;
                                    Program.f.pbFrame.Refresh();
                                    Thread.Sleep(displayDelay);
                                    Program.f.updateProgressDisplay();
                                }
                                else
                                {
                                    Program.f.pbFrameDB.Image = frame;
                                    Program.f.pbFrameDB.Refresh();
                                    Thread.Sleep(displayDelay);
                                    Program.f.updateProgressDisplayDB();
                                }
                                
                                s = sizeReader.ReadLine();
                                try
                                {
                                    size = int.Parse(s);
                                }
                                catch { ;}
                            }
                            vidFileStream.Close();
                            sizeReader.Close();

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
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            vidFileStream.Close();
                            sizeReader.Close();
                        }

                    }
                }
                catch (FileNotFoundException) { Thread.Sleep(waitForFileDelay); }
                catch (Exception ex)
                {
                    Console.WriteLine("Display " + ex.Message);
                }



            }


        }

        public void CloseListener()
        {
            listener.Close();
        }
    }
}
