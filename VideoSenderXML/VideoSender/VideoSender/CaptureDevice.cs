﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace VideoSender
{
    /// <summary>
    /// This class represents a device that is capable of capturing audio and video
    /// </summary>
    public class CaptureDevice
    {
        private static int MAX_DEVICES = 10;

        private ushort deviceNumber;
        private string name;
        private string description;
        private IntPtr deviceHandle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceNumber">the number</param>
        /// <param name="name">the name</param>
        /// <param name="description">the description</param>
        private CaptureDevice(ushort deviceNumber, string name, string description)
        {
            this.deviceNumber = deviceNumber;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Setter and Getter for the Device Number
        /// </summary>
        public ushort DeviceNumber
        {
            get { return deviceNumber; }
            set { deviceNumber = value; }
        }

        /// <summary>
        /// Setter and Getter for the Device name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Setter and Getter for the Device description
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Attaches the preview stream to the given control
        /// </summary>
        /// <param name="control">the control</param>
        public void Attach(System.Windows.Forms.Control control)
        {
            deviceHandle = CameraCapture.Avicap32.capCreateCaptureWindow("", CameraCapture.Constants.WS_VISIBLE | CameraCapture.Constants.WS_CHILD, 0, 0, control.Width, control.Height, control.Handle, 0);
            int val=CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_DRIVER_CONNECT, (IntPtr)deviceNumber, (IntPtr)0).ToInt32();
            //Console.WriteLine(val);
            //if ( val> 0)
            while (CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_DRIVER_CONNECT, (IntPtr)deviceNumber, (IntPtr)0).ToInt32() <= 0)
            {
                CameraCapture.User32.DestroyWindow(deviceHandle);
                System.Threading.Thread.Sleep(1000);
            }
            {
                CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_SET_SCALE, (IntPtr)(-1), (IntPtr)0);
                CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_SET_PREVIEWRATE, (IntPtr)0x34, (IntPtr)0);
                CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_SET_PREVIEW, (IntPtr)(-1), (IntPtr)0);
                CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_DLG_VIDEOFORMAT, (IntPtr)(-1), (IntPtr)0);
                CameraCapture.User32.SetWindowPos(deviceHandle, new IntPtr(0), 0, 0, control.Width, control.Height, 6);


            }
            //else {
              //  Console.WriteLine(deviceNumber);
              //  Console.WriteLine("Error");
               // CameraCapture.User32.DestroyWindow(deviceHandle);
            //}
        }

        /// <summary>
        /// Detaches from the control
        /// </summary>
        public void Detach()
        {
            if (deviceHandle.ToInt32() != 0)
            {
                CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_DRIVER_DISCONNECT, (IntPtr)deviceNumber, (IntPtr)0);
                CameraCapture.User32.DestroyWindow(deviceHandle);
            }
            deviceHandle = new IntPtr(0);

        }

        /// <summary>
        /// Returns a captured image
        /// </summary>
        /// <returns>an image, null if capture failed</returns>
        public Image Capture()
        {
            if (deviceHandle.ToInt32() != 0)
            {
                CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_EDIT_COPY, (IntPtr)0, (IntPtr)0);
                IDataObject ido = Clipboard.GetDataObject();
                if (ido.GetDataPresent(DataFormats.Bitmap))
                {
                    return ((Bitmap)ido.GetData(DataFormats.Bitmap));
                }
            }

            return null;
        }

        public Image GrabFrame(String path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("cannot delete file: " + ex.Message);
                }
            }
            CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_GRAB_FRAME_NOSTOP, (IntPtr)0, (IntPtr)0);
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(path);
            CameraCapture.User32.SendMessage(deviceHandle, CameraCapture.Constants.WM_CAP_SAVEDIB, (IntPtr)0, hBmp);            
            return Image.FromFile(path);
        }

        /// <summary>
        /// Returns an array with available capture devices
        /// </summary>
        /// <returns>the device names</returns>
        public static List<CaptureDevice> GetDevices()
        {
            List<CaptureDevice> devices = new List<CaptureDevice>();

            for (ushort i = 0; i < MAX_DEVICES; ++i)
            {
                int capacity = 200;
                StringBuilder name = new StringBuilder(capacity);
                StringBuilder description = new StringBuilder(capacity);

                if (CameraCapture.Avicap32.capGetDriverDescription(i, name, capacity, description, capacity).ToInt32() > 0)
                {
                    devices.Add(new CaptureDevice(i, name.ToString(), description.ToString()));
                }
            }

            return devices;
        }
    }
}