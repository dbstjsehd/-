using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Management;
using System.Drawing.Imaging;
using Hook;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.IO;

namespace 웨어하우스
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator System.Drawing.Point(POINT point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }
    }

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

  

    enum HOTKEY : int { F1, F1CTRL, F7, F2, HOME, END, F5,PAGEDOWN }

    public partial class Form1 : Form
    {
        #region 외부 함수 추가
        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern Int32 GetCursorPos(out POINT pt);

        [DllImport("user32")]
        public static extern int GetWindowRect(int hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        #endregion

        DateTime lastWheel = DateTime.Now;
        Rectangle captureRectangle;
        #region 설정값
        const double accurate = 0.8;
        double mouseXratio = 1.0;
        double mouseYratio = 1.0;
        int suddenResolution = 0;

        String GameName = "SuddenAttack";
        Process currentProcess;

        Thread 웨어하우스_Thread;
        Thread 프렙스_Thread;
        Thread 인아웃_Thread;
        Thread 연패_Thread;
        Thread 연승_Thread;
        Thread 오토대기_Thread = null;
        Thread 형광오토_Thread = null;
        Thread 스킨_Thread = null;
        bool 형광오토run = false;
        POINT lastPoint = new POINT();



        SerialPort arduSerial;

        bool mouseDown = false;
        bool keyboardReady = true;
        int x = 960, y = 540;
        
        int windowLeft = 0, windowTop = 0;

        string fullpath = "";

        #endregion

        public class ScreenStateLogger
        {
            private byte[] _previousScreen;
            private bool _run, _init;

            Factory1 factory;
            Adapter1 adapter;
            SharpDX.Direct3D11.Device device;
            Output output;
            Output1 output1;
            Texture2DDescription textureDesc;
            Texture2D screenTexture;
            public int Size { get; private set; }
            public ScreenStateLogger()
            {

            }

            public void Start()
            {
                _run = true;
                factory = new Factory1();
                //Get first adapter
                adapter = factory.GetAdapter1(0);
                //Get device from adapter
                device = new SharpDX.Direct3D11.Device(adapter);
                //Get front buffer of the adapter
                output = adapter.GetOutput(0);
                output1 = output.QueryInterface<Output1>();

                // Width/Height of desktop to capture
                int width = output.Description.DesktopBounds.Right;
                int height = output.Description.DesktopBounds.Bottom;

                // Create Staging texture CPU-accessible
                textureDesc = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = width,
                    Height = height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };
                screenTexture = new Texture2D(device, textureDesc);



                Task.Factory.StartNew(() =>
                {
                    // Duplicate the output
                    using (var duplicatedOutput = output1.DuplicateOutput(device))
                    {
                        while (_run)
                        {
                            try
                            {
                                SharpDX.DXGI.Resource screenResource;
                                OutputDuplicateFrameInformation duplicateFrameInformation;

                                // Try to get duplicated frame within given time is ms
                                duplicatedOutput.AcquireNextFrame(5, out duplicateFrameInformation, out screenResource);

                                // copy resource into memory that can be accessed by the CPU
                                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                                    device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);

                                // Get the desktop capture texture
                                var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                                // Create Drawing.Bitmap
                                using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                                {
                                    var boundsRect = new Rectangle(0, 0, width, height);

                                    // Copy pixels from screen capture Texture to GDI bitmap
                                    var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    var sourcePtr = mapSource.DataPointer;
                                    var destPtr = mapDest.Scan0;
                                    for (int y = 0; y < height; y++)
                                    {
                                        // Copy a single line 
                                        Utilities.CopyMemory(destPtr, sourcePtr, width * 4);

                                        // Advance pointers
                                        sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                                        destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                                    }

                                    destPtr = mapDest.Scan0;
                                    int bytes = Math.Abs(mapDest.Stride) * height;
                                    byte[] rgbValues = new byte[bytes];

                                    System.Runtime.InteropServices.Marshal.Copy(destPtr, rgbValues, 0, bytes);

                                    
                                    // Release source and dest locks
                                    bitmap.UnlockBits(mapDest);
                                    device.ImmediateContext.UnmapSubresource(screenTexture, 0);

                                    ScreenRefreshed?.Invoke(this, rgbValues);
                                    _init = true;
                                }
                                screenResource.Dispose();
                                duplicatedOutput.ReleaseFrame();
                            }
                            catch (SharpDXException e)
                            {

                            }
                        }
                    }
                });
                while (!_init) ;
            }

            public void Stop()
            {
                _run = false;

                
            }

            public void Dispose()
            {
                if (factory != null)
                {
                    factory.Dispose();
                    factory = null;
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                    adapter = null;
                }
                if (device != null)
                {
                    device.Dispose();
                    device = null;
                }
                if (output != null)
                {
                    output.Dispose();
                    output = null;
                }
                if (output1 != null)
                {
                    output1.Dispose();
                    output1 = null;
                }
                if (screenTexture != null)
                {
                    screenTexture.Dispose();
                    screenTexture = null;
                }
                GC.Collect();
            }

            public EventHandler<byte[]> ScreenRefreshed;
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {

                    case (int)HOTKEY.F1CTRL:
                    case (int)HOTKEY.F1:
                        {
                            Debug.WriteLine("F1");
                            try
                            {
                                if (오토대기_Thread == null)
                                {
                                    오토대기_Thread = new Thread(new ThreadStart(오토대기));
                                    오토대기_Thread.Start();
                                    Debug.WriteLine("" + 오토대기_Thread.ThreadState);
                                }
                                else
                                {
                                    if (오토대기_Thread.ThreadState != System.Threading.ThreadState.Running)
                                    {
                                        오토대기_Thread.Abort();
                                        오토대기_Thread = new Thread(new ThreadStart(오토대기));
                                        오토대기_Thread.Start();
                                    }
                                }
                            }
                            catch
                            {

                            }


                        }
                        break;

                    case (int)HOTKEY.F2:
                        {
                            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
                            foreach (System.Diagnostics.Process p in mProcess)
                            {
                                if (p.Id != currentProcess.Id)
                                {
                                    p.Kill();
                                }
                            }
                            try
                            {
                                웨어하우스_Thread.Abort();
                                웨어하우스_Thread = null;
                            }
                            catch { }

                            try
                            {
                                인아웃_Thread.Abort();
                                인아웃_Thread = null;
                            }
                            catch { }

                            try
                            {
                                연승_Thread.Abort();
                                연승_Thread = null;
                            }
                            catch { }
                            try
                            {
                                연패_Thread.Abort();
                                연패_Thread = null;
                            }
                            catch { }

                        }
                        break;

                    case (int)HOTKEY.F5:
                        {
                            Bitmap img = getFullScreen();

                            int bmpWidth = img.Width;
                            int bmpHeight = img.Height;

                            Rectangle rect = new Rectangle(0, 0, bmpWidth, bmpHeight);
                            System.Drawing.Imaging.BitmapData bmpData =
                                img.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                PixelFormat.Format24bppRgb);

                            // Get the address of the first line.
                            IntPtr ptr = bmpData.Scan0;

                            // Declare an array to hold the bytes of the bitmap.
                            int bytes = Math.Abs(bmpData.Stride) * bmpHeight;
                            byte[] rgbValues = new byte[bytes];

                            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);


                            img.UnlockBits(bmpData);

                            img.Dispose();
                            lastPoint.X = -1;

                            Stack<POINT> foundPoints = new Stack<POINT>();
                            for (int y = 0; y * y < bmpHeight; y += 1)
                            {
                                for (int x = 0; x < bmpWidth; x += 1)
                                {
                                    int numBytes = (y * (bmpWidth * 3)) + (x * 3);
                                    double h = 0, s = 0, v = 0;
                                    rgb_to_hsv(rgbValues[numBytes + 2], rgbValues[numBytes + 1], rgbValues[numBytes], out h, out s, out v);

                                    if ((h >= 290) && (s >= 60) && (v >= 15))
                                    {
                                        POINT temp = new POINT();
                                        temp.Y = y;
                                        temp.X = x;
                                        foundPoints.Push(temp);
                                        x += 19;
                                    }
                                }
                            }

                            int min = int.MaxValue;

                            while (foundPoints.Count > 0)
                            {
                                POINT temp = foundPoints.Pop();
                                int calc = Math.Abs(this.x - (temp.X + 2)) + Math.Abs(this.y - temp.Y - 2);
                                if (calc < min)
                                {
                                    min = calc;
                                    lastPoint = temp;
                                }
                            }

                            this.x = 400;
                            this.y = 300;
                            if (lastPoint.X != -1)
                            {

                                lastPoint.X = (int)((((lastPoint.X + 2) - this.x) * mouseXratio) * 3);
                                lastPoint.Y = (int)((lastPoint.Y + 2 - this.y) * mouseYratio * 3);




                                arduSerial.Write("!" + lastPoint.X + "," + lastPoint.Y + ".+");

                            }
                            MessageBox.Show("" + lastPoint.X + "," + lastPoint.Y);
                        }

                        break;
                    case (int)HOTKEY.F7:
                        {
                            Delay(10);
                            if (프렙스_Thread == null)
                            {
                                프렙스_Thread = new Thread(new ThreadStart(프렙스1));
                                프렙스_Thread.Start();
                            }
                            else if (프렙스_Thread.IsAlive)
                            {
                                프렙스_Thread.Abort();
                                프렙스_Thread = null;
                            }
                            else
                            {
                                프렙스_Thread.Start();
                            }
                        }

                        break;
                    
                    case (int)HOTKEY.HOME:
                        {
                            try
                            {
                                형광오토run = true;
                                if (형광오토_Thread == null)
                                {
                                    형광오토_Thread = new Thread(new ThreadStart(형광오토));
                                    형광오토_Thread.Start();
                                }
                                else
                                {
                                    if (형광오토_Thread.ThreadState != System.Threading.ThreadState.Running)
                                    {
                                        형광오토_Thread.Abort();
                                        형광오토_Thread = new Thread(new ThreadStart(형광오토));
                                        형광오토_Thread.Start();
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }

                        break;
                    case (int)HOTKEY.END:
                        {
                            형광오토run = false;
                            try
                            {
                                if (형광오토_Thread == null)
                                {
                                }
                                else
                                {
                                    Delay(100);
                                    형광오토_Thread.Abort();
                                    형광오토_Thread = null;
                                }
                            }
                            catch
                            {

                            }
                        }

                        break;

                    case (int)HOTKEY.PAGEDOWN:
                        {
                            스나모드.Checked = !스나모드.Checked;
                            if (형광오토_Thread != null)
                            {
                                if (형광오토_Thread.ThreadState == System.Threading.ThreadState.Running)
                                {
                                    형광오토run = false;
                                    Delay(100);
                                    형광오토_Thread.Abort();
                                    형광오토run = true;
                                    형광오토_Thread = null;
                                    형광오토_Thread = new Thread(new ThreadStart(형광오토));
                                    형광오토_Thread.Start();
                                }
                            }
                            break;
                        }

                }

            }
        }       //핫키

        public void 형광오토()
        {
            POINT sniperPoint = new POINT();
            bool 스나모드 = this.스나모드.Checked;

            IntPtr findwindow = FindWindow(null, GameName);
            if (findwindow == IntPtr.Zero)
            {
                MessageBox.Show("서든어택이 안 켜져있어요.");
                return;
            }
            int xOffset = Decimal.ToInt32(x좌표수정.Value), yOffset = Decimal.ToInt32(y좌표수정.Value) * -1;
            captureRectangle = Screen.AllScreens[0].Bounds;
            RECT findRect = new RECT();
            RECT firstFindRect = new RECT();
            RECT stRect = default(RECT);
            double xRate = 1.0, yRate = 1.0;
            GetWindowRect((int)findwindow, ref stRect);

            windowLeft = stRect.left;
            windowTop = stRect.top;
            int leftBorder = 0;
            int topBorder = 0;

            findRect.left = stRect.left + 30;
            findRect.right = stRect.right - 30;



            if ((stRect.right - stRect.left) == captureRectangle.Width)
            {
                Debug.WriteLine("전체");
                this.x = captureRectangle.Width / 2;
                this.y = captureRectangle.Height / 2;
                if(captureRectangle.Width == 800)
                {
                    Debug.WriteLine("800x600");
                    findRect.top = 264;
                    findRect.bottom = 331;
                    sniperPoint.X = 400;
                    sniperPoint.Y = 299;
                    xRate = 800.0 / 1280.0;
                    yRate = 600.0 / 1024.0;

                }
                else if(captureRectangle.Width == 1024)
                {
                    Debug.WriteLine("1024x768");
                    findRect.top = 312;
                    findRect.bottom = 415;
                    sniperPoint.X = 512;
                    sniperPoint.Y = 383;
                    xRate = 800.0 / 1024.0;
                    yRate = 600.0 / 768.0;

                }
                else if(captureRectangle.Width == 1152)
                {
                    Debug.WriteLine("1152x864");
                    findRect.top = 395;
                    findRect.bottom = 453;

                    sniperPoint.X = 576;
                    sniperPoint.Y = 431;
                    xRate = 1152.0 / 1280.0;
                    yRate = 864.0 / 1024.0;
                }
                else
                {
                    Debug.WriteLine("1280x1024");
                    findRect.top = 458;
                    findRect.bottom = 533;
                    sniperPoint.X = 641;
                    sniperPoint.Y = 513;
                }

                

            }
            else
            {
                Debug.WriteLine("창모드O");

                this.x = 960;
                this.y = 540;
                sniperPoint.X = 960;
                sniperPoint.Y = 551;



                Debug.WriteLine("크기 : " + captureRectangle.Width);

                if ((799<(stRect.right-stRect.left)) &&((stRect.right - stRect.left) < 1000))
                {
                    Debug.WriteLine("800x600");
                    leftBorder = ((stRect.right - stRect.left) - 800)/2;
                    
                    MessageBox.Show("800 창모드 지원 X");
                    return;

                }
                else if ((1010 < (stRect.right - stRect.left)) && ((stRect.right - stRect.left) < 1130))
                {
                    Debug.WriteLine("1024x768");
                    MessageBox.Show("1024 창모드 지원 X");
                    return;

                }
                else if ((1140 < (stRect.right - stRect.left)) && ((stRect.right - stRect.left) < 1260))
                {
                    Debug.WriteLine("1152x864");
                    MessageBox.Show("1152 창모드 지원 X");
                    return;
                }
                else
                {
                    findRect.top = stRect.top + 485;
                    findRect.bottom = stRect.top + 600;
                    Debug.WriteLine("1280 창모드");
                }



            }


            if (스나모드)
            {
                Debug.WriteLine("스나모드");
                if (captureRectangle.Width == 800)
                {
                    Debug.WriteLine("800x600");
                    findRect.top = 200;
                    findRect.bottom = 350;
                    sniperPoint.X = 400;
                    sniperPoint.Y = 299;
                    xRate = 800 / 1280;
                    yRate = 600 / 1024;

                }
            }
            else
            {
                Debug.WriteLine("라플모드");

            }

            firstFindRect.left = this.x - (int)((xRate * 30.0) - xOffset);
            firstFindRect.right = this.x + (int)((xRate * 30.0) - xOffset);
            firstFindRect.top = this.y - (int)((yRate * 50.0) - yOffset);
            firstFindRect.bottom = this.y + (int)((yRate * 50.0) - yOffset);
            


            if (스나모드)
            {
                Debug.WriteLine("스나모드");

            }
            else
            {
                Debug.WriteLine("라플모드");
            }

            suddenResolution = stRect.right - stRect.left;
            Debug.WriteLine("" + suddenResolution);

            Debug.WriteLine("윈도우 창 좌표 : " + windowLeft + "," + windowTop);

            Delay(10);
            DateTime lastTime = DateTime.Now;

            

            var screenStateLogger = new ScreenStateLogger();
            EventHandler<byte[]> handler = screenStateLogger.ScreenRefreshed += (sender, data) =>
            {
                //new Frame 
                byte[] rgbValues = data;
                bool sniperMode = false;


                if (스나모드)
                {
                    mouseDown = false;
                    int tempPosition = ((captureRectangle.Width * sniperPoint.Y) + sniperPoint.X) * 4;

                    if ((rgbValues[tempPosition + 2] == 255) && (rgbValues[tempPosition + 1] == 0) && (rgbValues[tempPosition] == 0))
                    {
                        sniperMode = true;

                    }
                }


                if (mouseDown || sniperMode)
                {


                    
                    {
                        int position;
                        int min = int.MaxValue;
                        Stack<POINT> foundPoints = new Stack<POINT>();
                        bool successFound = false;
                        lastPoint = new POINT();
                        lastPoint.X = -1;

                        for (int y = firstFindRect.top; y < firstFindRect.bottom; y++)
                        {
                            for (int x = firstFindRect.left; x < firstFindRect.right; x++)
                            {
                                position = ((captureRectangle.Width * y) + x) * 4;
                                
                                double h = 0, s = 0, v = 0;
                                rgb_to_hsv(rgbValues[position + 2], rgbValues[position + 1], rgbValues[position], out h, out s, out v);

                                if ((h >= 290) && (s >= 60) && (v >= 15))
                                {
                                    POINT temp = new POINT();
                                    temp.Y = y;
                                    temp.X = x;
                                    foundPoints.Push(temp);

                                    successFound = true;
                                }
                            }
                        }

                        if (successFound)
                        {
                            goto 찾았을경우;
                        }

                        for (int y = findRect.top; y < firstFindRect.top; y++)
                        {


                            for (int x = findRect.left; x < findRect.right; x++)
                            {
                                position = ((captureRectangle.Width * y) + x) * 4;
                                double h =0, s = 0, v = 0;
                                rgb_to_hsv(rgbValues[position + 2], rgbValues[position + 1], rgbValues[position],out h,out s,out v);

                                if((h>=290) &&(s>= 60) &&(v >= 15))
                                {
                                    POINT temp = new POINT();
                                    temp.Y = y;
                                    temp.X = x;
                                    foundPoints.Push(temp);
                                    x += 19;
                                    
                                }

                            }
                        }

                        for (int y = firstFindRect.top; y < findRect.bottom; y++)
                        {


                            for (int x = findRect.left; x < firstFindRect.left; x++)
                            {
                                position = ((captureRectangle.Width * y) + x) * 4;
                                double h = 0, s = 0, v = 0;
                                rgb_to_hsv(rgbValues[position + 2], rgbValues[position + 1], rgbValues[position], out h, out s, out v);

                                if ((h >= 290) && (s >= 60) && (v >= 15))
                                {
                                    POINT temp = new POINT();
                                    temp.Y = y;
                                    temp.X = x;
                                    foundPoints.Push(temp);
                                    x += 19;
                                }
                            }
                        }
                        for (int y = firstFindRect.top; y < findRect.bottom; y++)
                        {


                            for (int x = firstFindRect.right; x < findRect.right; x++)
                            {
                                position = ((captureRectangle.Width * y) + x) * 4;
                                double h = 0, s = 0, v = 0;
                                rgb_to_hsv(rgbValues[position + 2], rgbValues[position + 1], rgbValues[position], out h, out s, out v);

                                if ((h >= 290) && (s >= 60) && (v >= 15))
                                {
                                    POINT temp = new POINT();
                                    temp.Y = y;
                                    temp.X = x;
                                    foundPoints.Push(temp);
                                    x += 19;
                                }
                            }
                        }
                        for (int y = firstFindRect.bottom; y < findRect.bottom; y++)
                        {


                            for (int x = firstFindRect.left; x < firstFindRect.right; x++)
                            {
                                position = ((captureRectangle.Width * y) + x) * 4;
                                double h = 0, s = 0, v = 0;
                                rgb_to_hsv(rgbValues[position + 2], rgbValues[position + 1], rgbValues[position], out h, out s, out v);

                                if ((h >= 290) && (s >= 60) && (v >=15))
                                {
                                    POINT temp = new POINT();
                                    temp.Y = y;
                                    temp.X = x;
                                    foundPoints.Push(temp);
                                    x += 19;
                                }
                            }
                        }






                    찾았을경우:
                        while (foundPoints.Count > 0)
                        {
                            POINT temp = foundPoints.Pop();
                            int calc = Math.Abs(this.x - (temp.X + xOffset))+ Math.Abs(this.y -temp.Y - yOffset);
                            if (calc < min)
                            {
                                min = calc;
                                lastPoint = temp;
                            }
                        }


                        if (lastPoint.X != -1)
                        {


                            lastPoint.X = (int)((((lastPoint.X + xOffset) - this.x) * mouseXratio) );
                            lastPoint.Y = (int)((lastPoint.Y + yOffset - this.y) * mouseYratio );





                            arduSerial.Write("!" + lastPoint.X + "," + lastPoint.Y + ".+");

                        }

                    }
                }
            };

            screenStateLogger.Start();

            try
            {
                while (형광오토run)
                {

                }
                screenStateLogger.Stop();
                Delay(20);
                screenStateLogger.Dispose();
                screenStateLogger.ScreenRefreshed -= handler;
                screenStateLogger.ScreenRefreshed = null;
                handler = null;
            }
            catch(ThreadInterruptedException)
            {
                screenStateLogger.Stop();
                Delay(20);
                screenStateLogger.Dispose();
                screenStateLogger.ScreenRefreshed -= handler;
                screenStateLogger.ScreenRefreshed = null;
                handler = null;
            }



        }


        public void rgb_to_hsv(double r, double g, double b,out double h, out double s, out double v)
        {

            // R, G, B values are divided by 255 
            // to change the range from 0..255 to 0..1 
            r = r / 255.0;
            g = g / 255.0;
            b = b / 255.0;

            // h, s, v = hue, saturation, value 
            double cmax = Math.Max(r, Math.Max(g, b)); // maximum of r, g, b 
            double cmin = Math.Min(r, Math.Min(g, b)); // minimum of r, g, b 
            double diff = cmax - cmin; // diff of cmax and cmin. 
            h = -1;
            s = -1;

            // if cmax and cmax are equal then h = 0 
            if (cmax == cmin)
                h = 0;

            // if cmax equal r then compute h 
            else if (cmax == r)
                h = (60 * ((g - b) / diff) + 360) % 315;

            // if cmax equal g then compute h 
            else if (cmax == g)
                h = (60 * ((b - r) / diff) + 120) % 315;

            // if cmax equal b then compute h 
            else if (cmax == b)
                h = (60 * ((r - g) / diff) + 240) % 315;

            if (cmax == 0)
                s = 0;
            else
                s = (diff / cmax) * 100;

            // compute v 
            v = cmax * 100;

            // compute v 

        }


        public void 프렙스1()
        {
            while (true)
            {
                sendSpeicialKey("F9");
                Thread.Sleep(Decimal.ToInt32(numericUpDown1.Value));
            }
        }

        public Form1()
        {
            InitializeComponent();
            RegisterHotKey(this.Handle, (int)HOTKEY.F1CTRL, 2, Keys.F1.GetHashCode());
            RegisterHotKey(this.Handle, (int)HOTKEY.F1, 0, Keys.F1.GetHashCode());
            RegisterHotKey(this.Handle, (int)HOTKEY.F2, 0, Keys.F2.GetHashCode());
            //RegisterHotKey(this.Handle, (int)HOTKEY.F7, 0, Keys.F7.GetHashCode());
            //RegisterHotKey(this.Handle, (int)HOTKEY.F5, 0, Keys.F5.GetHashCode());
            RegisterHotKey(this.Handle, (int)HOTKEY.HOME, 0, Keys.Home.GetHashCode());
            RegisterHotKey(this.Handle, (int)HOTKEY.END, 0, Keys.End.GetHashCode());
            //RegisterHotKey(this.Handle, (int)HOTKEY.PAGEDOWN, 0, Keys.PageDown.GetHashCode());
            captureRectangle = Screen.AllScreens[0].Bounds;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region 선언부
            POINT pt = new POINT();
            pt.X = 0;
            pt.Y = 0;

            #endregion

            init();
            arduSerial.Write("$0+");
            MouseHook.MouseDown += MouseHook_MouseDown;
            MouseHook.MouseUp += MouseHook_MouseUp;
            MouseHook.MouseScroll += MouseHook_Scroll;
            KeyboardHook.KeyUp += KeyboardHook_KeyUp;
            MouseHook.HookStart();
            KeyboardHook.HookStart();



            fullpath = System.IO.Directory.GetCurrentDirectory()+"\\";

            
            
            
        }
        private bool KeyboardHook_KeyUp(int vkcode)
        {
            if(vkcode == 49)
            {
                if (radio_사용.Checked)
                {
                    if (keyboardReady)
                    {
                        if(오토대기_Thread != null)
                        {
                            if(오토대기_Thread.ThreadState == System.Threading.ThreadState.Running)
                            {
                                return true;
                            }
                        }
                        if (스나모드.Checked)
                        {
                            keyboardReady = false;
                            sendKey("r");
                            Delay(50);
                            sendKey("=");
                            
                            Delay(50);
                            
                            keyboardReady = true;
                        }





                    }
                }
            }

            

            return true;
        }

        private bool MouseHook_MouseDown(MouseEventType type, int x, int y)
        {
            if (type == MouseEventType.RIGHT)
            {
                mouseDown = true;
            }
            else if(type == MouseEventType.WHEEL)
            {
                if(lastWheel > DateTime.Now)
                {

                    return true;
                }
                lastWheel = DateTime.Now.AddMilliseconds(100);

                if (프렙스_Thread == null)
                {
                    프렙스_Thread = new Thread(new ThreadStart(프렙스1));
                    프렙스_Thread.Start();
                }
                else if (프렙스_Thread.IsAlive)
                {
                    프렙스_Thread.Abort();
                    프렙스_Thread = null;
                }
                else
                {
                    프렙스_Thread.Start();
                }
            }
            return true;
        }


        private bool MouseHook_MouseUp(MouseEventType type, int x, int y)
        {
            if (type == MouseEventType.RIGHT)
            {
                mouseDown = false;
            }
            
            return true;
        }

        private bool MouseHook_Scroll(MouseScrollType type)
        {
            Debug.WriteLine("" + type);

            return true;
        }


        private string GetSerialPort()
        {

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                {
                    var portnames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                    var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                    foreach (string s in portList)
                    {
                        if (s.Contains("Leonardo"))
                        {

                            string[] temp = s.Split(' ');
                            return temp[0];
                        }
                    }
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show(e.Message);
            }

            return "error";
        }

        private void init()
        {
            arduSerial = new SerialPort();


            string portName = GetSerialPort();
            if (portName.Equals("error"))
            {
                MessageBox.Show("아두이노가 안 꽂혀있는 것 같습니다.");
                return;
            }


            arduSerial.PortName = portName;
            arduSerial.BaudRate = 9600;
            arduSerial.Open();
            MessageBox.Show("마우스를 움직이지 마세요. 감도 체크 합니다.\n제어판에서 마우스 가속도(정확도 향상)을 꺼주세요.");
            mouseTest();
            MessageBox.Show("테스트 결과는 X 좌표 비율 : " + mouseXratio + ", Y 좌표 비율 : " + mouseYratio + "\n(마우스 비율 1은 제어판에서 마우스 속도 6칸 기준입니다.)");
            currentProcess = Process.GetCurrentProcess();


            스킨_Thread = new Thread(new ThreadStart(스킨));
            스킨_Thread.Start();
        }


        public double searchIMG(Bitmap screen_img, Bitmap find_img, out POINT pt)
        {
            //스크린 이미지 선언
            using (Mat ScreenMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(screen_img))
            //찾을 이미지 선언
            using (Mat FindMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(find_img))
            //스크린 이미지에서 FindMat 이미지를 찾아라
            using (Mat res = ScreenMat.MatchTemplate(FindMat, TemplateMatchModes.CCoeffNormed))
            {
                //찾은 이미지의 유사도를 담을 더블형 최대 최소 값을 선언합니다.
                double minval, maxval = 0;
                //찾은 이미지의 위치를 담을 포인트형을 선업합니다.
                OpenCvSharp.Point minloc, maxloc;
                //찾은 이미지의 유사도 및 위치 값을 받습니다. 
                Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                Debug.WriteLine("찾은 이미지의 유사도 : " + maxval);


                pt.X = maxloc.X;
                pt.Y = maxloc.Y;

                return maxval;
            }
        }

        public Bitmap getFindBmp(String path, double fx, double fy)
        {
            Bitmap findBmp;
            Bitmap resizeBmp;

            findBmp = new Bitmap(".\\img\\" + path);


            if (findBmp != null)
            {
                System.Drawing.Size resize = new System.Drawing.Size((int)(findBmp.Width * fx), (int)(findBmp.Height * fy));
                resizeBmp = new Bitmap(findBmp, resize);
            }
            else
            {
                findBmp.Dispose();
                MessageBox.Show("에러");
                resizeBmp = null;
            }

            return resizeBmp;
        }

        public Bitmap getBmp()
        {
            IntPtr findwindow = FindWindow(null, GameName);

            if (findwindow != IntPtr.Zero)
            {
                try
                {
                    Graphics Graphicsdata = Graphics.FromHwnd(findwindow);

                    //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                    Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                    //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                    Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                    //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                        IntPtr hdc = g.GetHdc();
                        PrintWindow(findwindow, hdc, 0x2);
                        g.ReleaseHdc(hdc);
                    }
                    Graphicsdata.Dispose();

                    Thread STAThread = new Thread(

    delegate ()

    {

        System.Windows.Forms.Clipboard.Clear();

    });

                    STAThread.SetApartmentState(ApartmentState.STA);
                    STAThread.Start();


                    STAThread.Join();


                    return bmp;
                }
                catch
                {
                    Thread STAThread = new Thread(

                        delegate ()

                        {

                            System.Windows.Forms.Clipboard.Clear();

                        });

                    STAThread.SetApartmentState(ApartmentState.STA);
                    STAThread.Start();


                    STAThread.Join();
                    return null;
                }
            }


            MessageBox.Show("에러 발생 서든어택 실행 여부를 확인해주세요.");

            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process p in mProcess)
                p.Kill();

            return null;
        }

        public void 오토대기()
        {
            Debug.WriteLine("시작");
            if (형광오토_Thread != null)
            {
                if (형광오토_Thread.ThreadState == System.Threading.ThreadState.Running)
                {
                    Debug.WriteLine("겹침");
                    return;
                }
            }
            Color color1 = new Color();
            bool doneBool = false;
            bool fireBool = false;
            Bitmap temp;

            POINT pt = new POINT();
            var screenStateLogger = new ScreenStateLogger();
            EventHandler<byte[]> handler = screenStateLogger.ScreenRefreshed += (sender, data) =>
            {
                if (doneBool)
                {
                    byte[] tempColor = data;
                    int position = ((captureRectangle.Width * (pt.Y)) + (pt.X + 1)) * 4;
                    int r = tempColor[position + 2];
                    int g = tempColor[position + 1];
                    int b = tempColor[position];
                    if (!((color1.R == r) && (color1.G == g) && (color1.B == b)))
                    {
                        doneBool = false;
                        sendSpeicialKey("LBD");
                        screenStateLogger.Stop();
                        Delay(50);
                        sendSpeicialKey("LBU");
                        if (radio_사용.Checked && 스나모드.Checked)
                        {
                            sendKey("3");
                            Delay(50);
                            sendKey("1");
                            Delay(50);
                            sendKey("r");
                            Delay(50);
                            sendKey("=");
                        }
                        screenStateLogger.Dispose();
                        Delay(30);
                        fireBool = true;
                    }
                }
            };
            try
            {
                screenStateLogger.ScreenRefreshed += handler;
                screenStateLogger.Start();

                captureRectangle = Screen.AllScreens[0].Bounds;
                temp = getFullScreen();

                if(temp == null)
                {
                    fireBool = true;
                    screenStateLogger.Stop();
                    Delay(20);
                    screenStateLogger.Dispose();
                    screenStateLogger.ScreenRefreshed -= handler;
                    screenStateLogger.ScreenRefreshed = null;
                    handler = null;

                    return;
                }

                GetCursorPos(out pt);
                if(captureRectangle.Width == 800)
                {
                    pt.X = 401;
                    pt.Y = 300;
                }
                else if(captureRectangle.Width == 1024)
                {
                    pt.X = 513;
                    pt.Y = 384;
                }
                else if(captureRectangle.Width == 1280)
                {
                    pt.X = 640;
                    pt.Y = 513;
                }
                
                color1 = temp.GetPixel(pt.X + 1, pt.Y);

                temp.Dispose();
                doneBool = true;
                while (!fireBool)
                {
                    Delay(5);
                }
                screenStateLogger.ScreenRefreshed -= handler;
                screenStateLogger.ScreenRefreshed = null;
                handler = null;
            }
            catch (ThreadInterruptedException)
            {
                fireBool = true;
                screenStateLogger.Stop();
                Delay(100);
                screenStateLogger.Dispose();
                screenStateLogger.ScreenRefreshed -= handler;
                screenStateLogger.ScreenRefreshed = null;
                handler = null;
            }
            catch(Exception)
            {
                fireBool = true;
                screenStateLogger.Stop();
                Delay(100);
                screenStateLogger.Dispose();
                screenStateLogger.ScreenRefreshed -= handler;
                screenStateLogger.ScreenRefreshed = null;
                handler = null;
            }

        }



        public Bitmap getFullScreen()
        {
            try

            {

                //Creating a new Bitmap object



                //Bitmap captureBitmap = new Bitmap(int width, int height, PixelFormat);

                //Creating a Rectangle object which will  

                //capture our Current Screen

                

                Bitmap captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height, PixelFormat.Format24bppRgb);


                //Creating a New Graphics Object

                Graphics captureGraphics = Graphics.FromImage(captureBitmap);



                //Copying Image from The Screen

                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size, CopyPixelOperation.SourceCopy);
                captureGraphics.Dispose();


                //Saving the Image File (I am here Saving it in My E drive).
                


                return captureBitmap;



                //Displaying the Successfull Result





            }

            catch (Exception ex)

            {
                Thread STAThread = new Thread(

delegate ()

{

    System.Windows.Forms.Clipboard.Clear();

});

                STAThread.SetApartmentState(ApartmentState.STA);
                STAThread.Start();


                STAThread.Join();
                return null;

            }
        }

        public void 인아웃()
        {
            POINT pt;
            double fx = 1, fy = 1;


            RECT stRect = default(RECT);
            GetWindowRect((int)FindWindow(null, GameName), ref stRect);

            Bitmap findBmp2;
            DateTime lastTime = DateTime.Now.AddSeconds(7);
            if (radioButton1.Checked)
            {
                fx = 640.0 / 1280.0;
                fy = 480.0 / 1024.0;

                MessageBox.Show("640 은 안되더라고요! 이딴 해상도는 쓰지마세욧! 귀찮아요!");
                return;
            }
            else if (radioButton2.Checked)
            {
                fx = 800.0 / 1280.0;
                fy = 600.0 / 1024.0;
            }
            else if (radioButton3.Checked)
            {
                fx = 1024.0 / 1280.0;
                fy = 768.0 / 1024.0;
            }
            else if (radioButton4.Checked)
            {
                fx = 1152.0 / 1280.0;
                fy = 864.0 / 1024.0;
            }
            else
            {
                fx = 1.0;
                fy = 1.0;
            }

            if (radioButton2.Checked)
            {
                findBmp2 = getFindBmp("800리스폰.png", 1.0, 1.0);
            }
            else if (radioButton3.Checked)
            {
                findBmp2 = getFindBmp("1024리스폰.png", 1.0, 1.0);
            }
            else if (radioButton4.Checked)
            {
                findBmp2 = getFindBmp("1152리스폰.png", 1.0, 1.0);
            }
            else
            {
                findBmp2 = getFindBmp("1280리스폰.png", 1.0, 1.0);
            }

            Bitmap findBmp1 = getFindBmp("게임시작.png", fx, fy);

            while (true)
            {
                Bitmap bmp = getBmp();





                if (searchIMG(bmp, findBmp1, out pt) > accurate)
                {
                    pt.X = pt.X + stRect.left + findBmp1.Width / 2;
                    pt.Y = pt.Y + stRect.top + findBmp1.Height / 2;

                    mouseMove(pt);
                    sendSpeicialKey("LBC");
                    sendSpeicialKey("ENTER");
                    Delay(500);

                    pt.X = pt.X + 100;
                    mouseMove(pt);
                }
                else if (인아웃_check.Checked)
                {
                    if (searchIMG(bmp, findBmp2, out pt) > accurate)
                    {
                        sendSpeicialKey("ESC");
                        Delay(10);
                        sendSpeicialKey("ENTER");
                        Delay(10);
                        sendSpeicialKey("ENTER");
                        Delay(500);
                    }
                }
                else
                {
                    sendKey("1");
                    if (lastTime < DateTime.Now)
                    {
                        sendKey(뒤돌기.Text);
                        lastTime = DateTime.Now.AddSeconds(7);
                    }
                }
                bmp.Dispose();

            }
        }
        public void 웨어하우스()
        {
            POINT pt;
            double fx = 1, fy = 1;


            RECT stRect = default(RECT);
            GetWindowRect((int)FindWindow(null, GameName), ref stRect);

            Debug.WriteLine("아아아  " + stRect.top + "," + stRect.left);

            if (radioButton1.Checked)
            {
                fx = 640.0 / 1280.0;
                fy = 480.0 / 1024.0;

                MessageBox.Show("640 은 안되더라고요! 이딴 해상도는 쓰지마세욧! 귀찮아요!");
                return;
            }
            else if (radioButton2.Checked)
            {
                fx = 800.0 / 1280.0;
                fy = 600.0 / 1024.0;
            }
            else if (radioButton3.Checked)
            {
                fx = 1024.0 / 1280.0;
                fy = 768.0 / 1024.0;
            }
            else if (radioButton4.Checked)
            {
                fx = 1152.0 / 1280.0;
                fy = 864.0 / 1024.0;
            }
            else
            {
                fx = 1.0;
                fy = 1.0;
            }
            while (true)
            {
                Bitmap bmp = getBmp();
                Bitmap findBmp1 = getFindBmp("바로입장1.png", fx, fy);
                Bitmap findBmp2 = getFindBmp("준비완료.png", fx, fy);
                Bitmap findBmp3 = getFindBmp("게임시작.png", fx, fy);
                Bitmap findBmp4 = getFindBmp("로딩.png", fx, fy);
                Bitmap findBmp5 = getFindBmp("확인.png", fx, fy);
                Bitmap findBmp6 = getFindBmp("나가기.png", fx, fy);
                Bitmap findBmp7 = getFindBmp("바로입장2.png", fx, fy);

                if (searchIMG(bmp, findBmp5, out pt) > accurate)
                {
                    pt.X = pt.X + stRect.left + 5;
                    pt.Y = pt.Y + stRect.top + 5;
                    mouseMove(pt);
                    sendSpeicialKey("LBC");
                    Delay(10);
                }
                else if (searchIMG(bmp, findBmp6, out pt) > accurate)   //나가기
                {
                    sendSpeicialKey("ENTER");
                    sendSpeicialKey("ESC");
                    Delay(1000);
                }
                else if (searchIMG(bmp, findBmp1, out pt) > accurate)           // 바로입장 1
                {
                    sendSpeicialKey("ENTER");
                    pt.X = pt.X + stRect.left + findBmp1.Width / 2;
                    pt.Y = pt.Y + stRect.top + findBmp1.Height / 2;

                    mouseMove(pt);
                    sendSpeicialKey("LBC");
                    Delay(1000);

                    string[] s = new string[2];
                    s[0] = "모든맵.PNG";
                    s[1] = "모든맵2.PNG";

                    Bitmap quickBmp = getBmp();
                    foreach (string a in s)
                    {
                        Bitmap tempBmp = getFindBmp(a, fx, fy);
                        Double temp = searchIMG(quickBmp, tempBmp, out pt);
                        tempBmp.Dispose();
                        if (temp > accurate)
                        {
                            pt.X = pt.X + stRect.left + 5;
                            pt.Y = pt.Y + stRect.top + 5;
                            mouseMove(pt);
                            sendSpeicialKey("LBC");

                            tempBmp.Dispose();

                            Delay(1000);
                            break;
                        }
                    }
                    quickBmp.Dispose();

                    quickBmp = getBmp();

                    if (searchIMG(quickBmp, getFindBmp("웨어하우스.PNG", fx, fy), out pt) > 0.5)
                    {
                        pt.X = pt.X + stRect.left;
                        pt.Y = pt.Y + stRect.top;
                        mouseMove(pt);
                        sendSpeicialKey("LBC");
                        Delay(1000);
                    }
                    else if (searchIMG(quickBmp, getFindBmp("웨어하우스2.PNG", fx, fy), out pt) > 0.5)
                    {
                        pt.X = pt.X + stRect.left;
                        pt.Y = pt.Y + stRect.top;
                        mouseMove(pt);
                        sendSpeicialKey("LBC");
                        Delay(1000);
                    }
                    else
                    {
                        Bitmap tempBmp = cropImage(quickBmp, new Rectangle(0, 0, (int)(900.0 * fx), (int)(938.0 * fy)));
                        POINT upPt = new POINT(), downPt = new POINT();

                        string tempString = "";
                        string tempString2 = "";
                        if (fx == 800.0 / 1280.0)
                        {
                            tempString = "800위.PNG";
                            tempString2 = "800아래.PNG";
                        }
                        else if (fx == 1024.0 / 1280.0)
                        {
                            tempString = "1024위.PNG";
                            tempString2 = "1024아래.PNG";
                        }
                        else if (fx == 1152.0 / 1280.0)
                        {
                            tempString = "1152위.PNG";
                            tempString2 = "1152아래.PNG";
                        }
                        else
                        {
                            tempString = "위.PNG";
                            tempString2 = "아래.PNG";
                        }

                        if (searchIMG(tempBmp, getFindBmp(tempString, 1.0, 1.0), out upPt) < accurate)
                        {
                            
                        }
                        if (searchIMG(tempBmp, getFindBmp(tempString2, 1.0, 1.0), out downPt) < accurate)
                        {

                        }

                        upPt.X = upPt.X + stRect.left;
                        upPt.Y = upPt.Y + stRect.top;
                        downPt.X = downPt.X + stRect.left;
                        downPt.Y = downPt.Y + stRect.top;

                        tempBmp.Dispose();

                        mouseMove(upPt);
                        for (int i = 0; i < 120; i++)
                        {
                            sendSpeicialKey("LBC");
                        }

                        Delay(10);
                        mouseMove(downPt);
                        Bitmap ware = getBmp();
                        Bitmap teamDeath = getFindBmp("팀데스매치1.PNG", fx, fy);
                        if (searchIMG(ware, teamDeath, out pt) > accurate)
                        {
                            ware.Dispose();
                            pt.X = pt.X + stRect.left;
                            pt.Y = pt.Y + stRect.top;
                            mouseMove(pt);
                            sendSpeicialKey("LBC");
                            Delay(10);
                        }
                        else
                        {
                            ware.Dispose();
                            mouseMove(downPt);
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    sendSpeicialKey("LBC");
                                    Delay(10);
                                }

                                ware = getBmp();
                                if (searchIMG(ware, teamDeath, out pt) > accurate)
                                {
                                    ware.Dispose();
                                    Delay(1000);
                                    pt.X = pt.X + stRect.left;
                                    pt.Y = pt.Y + stRect.top;
                                    mouseMove(pt);
                                    sendSpeicialKey("LBC");
                                    Delay(10);


                                    break;
                                }
                                ware.Dispose();
                            }

                        }
                        mouseMove(downPt);
                        for (int i = 0; i < 8; i++)
                        {
                            sendSpeicialKey("LBC");
                            Delay(10);
                        }

                        ware = getBmp();
                        if (searchIMG(ware, getFindBmp("웨어하우스.PNG", fx, fy), out pt) > 0.5)
                        {
                            ware.Dispose();
                            Delay(1000);
                            pt.X = pt.X + stRect.left;
                            pt.Y = pt.Y + stRect.top;
                            mouseMove(pt);
                            sendSpeicialKey("LBC");
                            Delay(10);
                        }
                        else
                        {
                            ware.Dispose();
                            mouseMove(upPt);
                            for (int i = 0; i < 8; i++)
                            {
                                sendSpeicialKey("LBC");
                                Delay(10);
                            }
                            ware = getBmp();
                            if (searchIMG(ware, teamDeath, out pt) > accurate)
                            {
                                Delay(1000);
                                pt.X = pt.X + stRect.left;
                                pt.Y = pt.Y + stRect.top;
                                mouseMove(pt);
                                sendSpeicialKey("LBC");
                                Delay(20);
                            }
                            else
                            {
                                sendSpeicialKey("ESC");
                                Delay(500);
                                sendSpeicialKey("ENTER");
                                Delay(500);
                                sendSpeicialKey("ENTER");
                            }
                            ware.Dispose();
                            teamDeath.Dispose();

                            mouseMove(downPt);
                            for (int i = 0; i < 8; i++)
                            {
                                sendSpeicialKey("LBC");
                                Delay(20);
                            }

                            ware = getBmp();
                            if (searchIMG(ware, getFindBmp("웨어하우스.PNG", fx, fy), out pt) > 0.5)
                            {
                                Delay(1000);
                                ware.Dispose();
                                pt.X = pt.X + stRect.left;
                                pt.Y = pt.Y + stRect.top;
                                mouseMove(pt);
                                sendSpeicialKey("LBC");
                                Delay(20);
                            }
                            else
                            {
                                ware.Dispose();
                            }
                        }

                    }
                    quickBmp.Dispose();
                    sendSpeicialKey("ENTER");
                    Delay(2000);

                }
                else if (searchIMG(bmp, findBmp7, out pt) > accurate)           // 바로입장 1
                {
                    sendSpeicialKey("ENTER");
                    pt.X = stRect.left + 300;
                    pt.Y = stRect.top + 160;

                    mouseMove(pt);
                    Delay(500);
                    sendSpeicialKey("LBC");
                    Delay(1000);

                    string[] s = new string[2];
                    s[0] = "모든맵.PNG";
                    s[1] = "모든맵2.PNG";

                    Bitmap quickBmp = getBmp();
                    foreach (string a in s)
                    {
                        Bitmap tempBmp = getFindBmp(a, fx, fy);
                        Double temp = searchIMG(quickBmp, tempBmp, out pt);
                        tempBmp.Dispose();
                        if (temp > accurate)
                        {
                            pt.X = pt.X + stRect.left + 5;
                            pt.Y = pt.Y + stRect.top + 5;
                            mouseMove(pt);
                            sendSpeicialKey("LBC");

                            tempBmp.Dispose();

                            Delay(1000);
                            break;
                        }
                    }
                    quickBmp.Dispose();

                    quickBmp = getBmp();

                    if (searchIMG(quickBmp, getFindBmp("웨어하우스.PNG", fx, fy), out pt) > 0.5)
                    {
                        pt.X = pt.X + stRect.left;
                        pt.Y = pt.Y + stRect.top;
                        mouseMove(pt);
                        sendSpeicialKey("LBC");
                        Delay(1000);
                    }
                    else if (searchIMG(quickBmp, getFindBmp("웨어하우스2.PNG", fx, fy), out pt) > 0.5)
                    {
                        pt.X = pt.X + stRect.left;
                        pt.Y = pt.Y + stRect.top;
                        mouseMove(pt);
                        sendSpeicialKey("LBC");
                        Delay(1000);
                    }
                    else
                    {
                        Bitmap tempBmp = cropImage(quickBmp, new Rectangle(0, 0, (int)(900.0 * fx), (int)(938.0 * fy)));
                        POINT upPt = new POINT(), downPt = new POINT();

                        string tempString = "";
                        string tempString2 = "";
                        if (fx == 800.0 / 1280.0)
                        {
                            tempString = "800위.PNG";
                            tempString2 = "800아래.PNG";
                        }
                        else if (fx == 1024.0 / 1280.0)
                        {
                            tempString = "1024위.PNG";
                            tempString2 = "1024아래.PNG";
                        }
                        else if (fx == 1152.0 / 1280.0)
                        {
                            tempString = "1152위.PNG";
                            tempString2 = "1152아래.PNG";
                        }
                        else
                        {
                            tempString = "위.PNG";
                            tempString2 = "아래.PNG";
                        }

                        if (searchIMG(tempBmp, getFindBmp(tempString, 1.0, 1.0), out upPt) < accurate)
                        {
                            
                        }
                        if (searchIMG(tempBmp, getFindBmp(tempString2, 1.0, 1.0), out downPt) < accurate)
                        {

                        }

                        upPt.X = upPt.X + stRect.left;
                        upPt.Y = upPt.Y + stRect.top;
                        downPt.X = downPt.X + stRect.left;
                        downPt.Y = downPt.Y + stRect.top;

                        tempBmp.Dispose();

                        mouseMove(upPt);
                        for (int i = 0; i < 120; i++)
                        {
                            sendSpeicialKey("LBC");
                        }

                        Delay(10);
                        mouseMove(downPt);
                        Bitmap ware = getBmp();
                        Bitmap teamDeath = getFindBmp("팀데스매치1.PNG", fx, fy);
                        if (searchIMG(ware, teamDeath, out pt) > accurate)
                        {
                            ware.Dispose();
                            pt.X = pt.X + stRect.left;
                            pt.Y = pt.Y + stRect.top;
                            mouseMove(pt);
                            sendSpeicialKey("LBC");
                            Delay(10);
                        }
                        else
                        {
                            ware.Dispose();
                            mouseMove(downPt);
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    sendSpeicialKey("LBC");
                                    Delay(10);
                                }

                                ware = getBmp();
                                if (searchIMG(ware, teamDeath, out pt) > accurate)
                                {
                                    ware.Dispose();
                                    Delay(1000);
                                    pt.X = pt.X + stRect.left;
                                    pt.Y = pt.Y + stRect.top;
                                    mouseMove(pt);
                                    sendSpeicialKey("LBC");
                                    Delay(10);


                                    break;
                                }
                                ware.Dispose();
                            }

                        }
                        mouseMove(downPt);
                        for (int i = 0; i < 8; i++)
                        {
                            sendSpeicialKey("LBC");
                            Delay(10);
                        }

                        ware = getBmp();
                        if (searchIMG(ware, getFindBmp("웨어하우스.PNG", fx, fy), out pt) > 0.5)
                        {
                            ware.Dispose();
                            Delay(1000);
                            pt.X = pt.X + stRect.left;
                            pt.Y = pt.Y + stRect.top;
                            mouseMove(pt);
                            sendSpeicialKey("LBC");
                            Delay(10);
                        }
                        else
                        {
                            ware.Dispose();
                            mouseMove(upPt);
                            for (int i = 0; i < 8; i++)
                            {
                                sendSpeicialKey("LBC");
                                Delay(10);
                            }
                            ware = getBmp();
                            if (searchIMG(ware, teamDeath, out pt) > accurate)
                            {
                                Delay(1000);
                                pt.X = pt.X + stRect.left;
                                pt.Y = pt.Y + stRect.top;
                                mouseMove(pt);
                                sendSpeicialKey("LBC");
                                Delay(20);
                            }
                            else
                            {
                                sendSpeicialKey("ESC");
                                Delay(500);
                                sendSpeicialKey("ENTER");
                                Delay(500);
                                sendSpeicialKey("ENTER");
                            }
                            ware.Dispose();
                            teamDeath.Dispose();

                            mouseMove(downPt);
                            for (int i = 0; i < 8; i++)
                            {
                                sendSpeicialKey("LBC");
                                Delay(20);
                            }

                            ware = getBmp();
                            if (searchIMG(ware, getFindBmp("웨어하우스.PNG", fx, fy), out pt) > 0.5)
                            {
                                Delay(1000);
                                ware.Dispose();
                                pt.X = pt.X + stRect.left;
                                pt.Y = pt.Y + stRect.top;
                                mouseMove(pt);
                                sendSpeicialKey("LBC");
                                Delay(20);
                            }
                            else
                            {
                                ware.Dispose();
                            }
                        }

                    }
                    quickBmp.Dispose();
                    sendSpeicialKey("ENTER");
                    Delay(2000);

                }
                else if (searchIMG(bmp, findBmp4, out pt) > accurate) ;         // 로딩중일때 아무 것도 안함..
                else if (searchIMG(bmp, findBmp2, out pt) > accurate)          // 준비완료
                {
                    sendSpeicialKey("ESC");
                    Delay(1000);
                }
                else if (searchIMG(bmp, findBmp3, out pt) > accurate)      // 게임시작
                {
                    pt.X = pt.X + stRect.left + findBmp3.Width / 2;
                    pt.Y = pt.Y + stRect.top + findBmp3.Height / 2;

                    mouseMove(pt);
                    sendSpeicialKey("LBC");
                    Delay(1000);
                    sendSpeicialKey("ESC");
                    sendSpeicialKey("ESC");
                    Delay(1000);

                    pt.X = pt.X + 100;
                    mouseMove(pt);
                }
                else
                {
                    string kill = "";
                    Bitmap killBmp;
                    double tempDouble;
                    string[] s = new string[4];
                    s[0] = "100kill.PNG";
                    s[1] = "120kill.PNG";
                    s[2] = "140kill.PNG";
                    s[3] = "160kill.PNG";

                    foreach (string a in s)
                    {
                        killBmp = getFindBmp(a, fx, fy);
                        tempDouble = searchIMG(bmp, killBmp, out pt);
                        killBmp.Dispose();
                        if (tempDouble > accurate)
                        {
                            kill = a;
                            break;
                        }
                    }

                    if (!kill.Equals(""))
                    {
                        DateTime lastTime = DateTime.Now.Add(new TimeSpan(0, 0, 1));

                        int i = 0;

                        int count = 0;

                        sendKey("4");
                        sendSpeicialKey("DARD");
                        Delay(1500);
                        sendSpeicialKey("LBD");
                        Delay(100);
                        sendSpeicialKey("LBU");

                        while (true)
                        {
                            Bitmap gameBmp = getBmp();
                            if (gameBmp == null)
                            {
                                continue;
                            }
                            Color pixel1 = gameBmp.GetPixel(613, 498);
                            Color pixel2 = gameBmp.GetPixel(338, 445);


                            if (즉리.Checked)
                            {
                                if (count == 100)
                                {

                                    if (!((pixel2.R == 0) && (pixel2.G == 255) && (pixel2.B == 0)))
                                    {
                                        count = 0;
                                        Debug.WriteLine("죽음");
                                        sendKey("4");
                                        sendSpeicialKey("DARD");
                                        Delay(1200);
                                        sendSpeicialKey("LBD");
                                        Delay(100);
                                        sendSpeicialKey("LBU");
                                    }

                                }
                                else if ((pixel2.R == 0) && (pixel2.G == 255) && (pixel2.B == 0))
                                {
                                    sendKey("e");
                                    count = 100;
                                }
                            }

                            else if ((i++) % 2 == 0)
                            {
                                sendKey("4");
                                sendSpeicialKey("DARD");
                                Delay(1000);
                            }
                            else
                            {
                                sendSpeicialKey("LBD");
                                Delay(100);
                                sendSpeicialKey("LBU");
                            }

                            if (!((pixel1.R == 255) && (pixel1.G == 255) && (pixel1.B == 255)))
                            {
                                killBmp = getFindBmp(kill, fx, fy);
                                if (searchIMG(gameBmp, killBmp, out pt) < accurate)
                                {
                                    Debug.WriteLine("끝");
                                    killBmp.Dispose();
                                    gameBmp.Dispose();
                                    break;
                                }
                                killBmp.Dispose();
                            }
                            gameBmp.Dispose();
                        }
                    }
                    else
                    {
                        sendSpeicialKey("ENTER");
                        Delay(1000);
                        sendSpeicialKey("ENTER");
                        Delay(500);
                        sendSpeicialKey("ESC");
                        Delay(500);
                        sendSpeicialKey("ENTER");
                        Delay(500);
                        sendSpeicialKey("ENTER");
                        Delay(1000);
                    }
                }


                bmp.Dispose();
                findBmp1.Dispose();
                findBmp2.Dispose();
                findBmp3.Dispose();
                findBmp4.Dispose();
                findBmp5.Dispose();
                findBmp6.Dispose();
                findBmp7.Dispose();
            }
        }


        private void 웨어하우스_시작_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = FindWindow(null, GameName);

            if (!hWnd.Equals(IntPtr.Zero))
            {
                // 윈도우가 최소화 되어 있다면 활성화 시킨다
                ShowWindowAsync(hWnd, 1);

                // 윈도우에 포커스를 줘서 최상위로 만든다
                SetForegroundWindow(hWnd);
            }

            웨어하우스_Thread = new Thread(new ThreadStart(웨어하우스));
            웨어하우스_Thread.Start();

            Form2 dlg = new Form2();
            dlg.Owner = this;
            dlg.ShowDialog();


        }
        public Bitmap cropImage(Bitmap origin, Rectangle cropRect)
        {
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);



            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(origin, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            return target;
        }

        public void sendKey(string s)
        {
            arduSerial.Write(s);
            Delay(s.Length);
        }

        public void mouseTest()
        {
            POINT thisPt = new POINT();
            POINT afterPt = new POINT();

            GetCursorPos(out thisPt);

            arduSerial.Write("!127,127.+");
            Delay(10);

            GetCursorPos(out afterPt);

            thisPt.X = afterPt.X - thisPt.X;
            thisPt.Y = afterPt.Y - thisPt.Y;



            mouseXratio = 127.0 / thisPt.X;
            mouseYratio = 127.0 / thisPt.Y;


            Debug.WriteLine(mouseXratio + "," + mouseYratio);


        }

        public void mouseMove(POINT pt)
        {
            if ((pt.X == 0) && (pt.Y == 0))
            {
                Debug.WriteLine("error일 확률 존재");
                return;
            }

            POINT thisPt = new POINT();
            GetCursorPos(out thisPt);

            pt.X = (int)((pt.X - thisPt.X) * mouseXratio);
            pt.Y = (int)((pt.Y - thisPt.Y) * mouseYratio);

            Debug.WriteLine(pt.X + "," + pt.Y);

            arduSerial.Write("!" + pt.X + "," + pt.Y + ".+");

            Delay(2);

            return;

        }
        public void mouseMove2(POINT pt)
        {
            if (pt.X == -1)
            {
                Debug.WriteLine("error일 확률 존재");
                return;
            }


            pt.X = (int)(((pt.X - this.x) * mouseXratio)*5);
            pt.Y = (int)((pt.Y - this.y) * mouseYratio*3);
            

            

            arduSerial.Write("!" + pt.X + "," + pt.Y + ".+");

            Delay(10);

            return;

        }

        public void sendSpeicialKey(string s)
        {
            Debug.WriteLine("키 " + s);
            arduSerial.Write("@" + s + "+");
            Delay(5);
        }       //ESC -> ESC ; 

        private static DateTime Delay(int ms)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime AfterWards = ThisMoment.Add(duration);
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;

            }
            return DateTime.Now;
        }       // thread sleep 대체
        public bool CheckingSpecialText(string txt)
        {
            string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
            return rex.IsMatch(txt);
        }   // 특수문자 체크

        private void Button1_Click(object sender, EventArgs e)
        {
            IntPtr findwindow = FindWindow(null, GameName);
            if (findwindow != IntPtr.Zero)
            {

                Graphics Graphicsdata = Graphics.FromHwnd(findwindow);

                //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(findwindow, hdc, 0x2);
                    g.ReleaseHdc(hdc);
                }

                
            }
            else
            {
                MessageBox.Show("못찾");
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process p in mProcess)
                p.Kill();
        }               // 종료시 이름 같은거 다 종료,, 이름을 unique 하게 지어야 함!

        private void 인아웃_시작_Click(object sender, EventArgs e)
        {
            if (CheckingSpecialText(뒤돌기.Text))
            {
                MessageBox.Show("뒤돌기 키는 특수 문자가 안됩니다.");
                return;
            }

            IntPtr hWnd = FindWindow(null, GameName);

            if (!hWnd.Equals(IntPtr.Zero))
            {
                // 윈도우가 최소화 되어 있다면 활성화 시킨다
                ShowWindowAsync(hWnd, 1);

                // 윈도우에 포커스를 줘서 최상위로 만든다
                SetForegroundWindow(hWnd);
            }

            인아웃_Thread = new Thread(new ThreadStart(인아웃));
            인아웃_Thread.Start();

            Form2 dlg = new Form2();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        public void 연승()
        {
            POINT pt;
            double fx = 1, fy = 1;


            RECT stRect = default(RECT);
            GetWindowRect((int)FindWindow(null, GameName), ref stRect);

            if (radioButton1.Checked)
            {
                fx = 640.0 / 1280.0;
                fy = 480.0 / 1024.0;

                MessageBox.Show("640 은 안되더라고요! 이딴 해상도는 쓰지마세욧! 귀찮아요!");
                return;
            }
            else if (radioButton2.Checked)
            {
                fx = 800.0 / 1280.0;
                fy = 600.0 / 1024.0;
            }
            else if (radioButton3.Checked)
            {
                fx = 1024.0 / 1280.0;
                fy = 768.0 / 1024.0;
            }
            else if (radioButton4.Checked)
            {
                fx = 1152.0 / 1280.0;
                fy = 864.0 / 1024.0;
            }
            else
            {
                fx = 1.0;
                fy = 1.0;
            }

            Bitmap 게임시작그림 = getFindBmp("게임시작.png", fx, fy);

            while (true)
            {
                Bitmap tempBmp = getBmp();

                if (searchIMG(tempBmp, 게임시작그림, out pt) > accurate)
                {
                    pt.X = stRect.left + pt.X + 게임시작그림.Width / 2;
                    pt.Y = stRect.top + pt.Y + 게임시작그림.Height / 2;
                    mouseMove(pt);
                    sendSpeicialKey("LBC");
                    Delay(500);
                    pt.X = (stRect.left + stRect.right) / 2;
                    pt.Y = (stRect.top + stRect.bottom) / 2;
                    mouseMove(pt);
                }
                else
                {
                    pt.X = (stRect.left + stRect.right) / 2;
                    pt.Y = (stRect.top + stRect.bottom) / 2;
                    mouseMove(pt);

                    sendSpeicialKey("ENTER");
                }
            }

        }

        public void 연패()
        {
            POINT pt;
            double fx = 1, fy = 1;


            RECT stRect = default(RECT);
            GetWindowRect((int)FindWindow(null, GameName), ref stRect);

            if (radioButton1.Checked)
            {
                fx = 640.0 / 1280.0;
                fy = 480.0 / 1024.0;

                MessageBox.Show("640 은 안되더라고요! 이딴 해상도는 쓰지마세욧! 귀찮아요!");
                return;
            }
            else if (radioButton2.Checked)
            {
                fx = 800.0 / 1280.0;
                fy = 600.0 / 1024.0;
            }
            else if (radioButton3.Checked)
            {
                fx = 1024.0 / 1280.0;
                fy = 768.0 / 1024.0;
            }
            else if (radioButton4.Checked)
            {
                fx = 1152.0 / 1280.0;
                fy = 864.0 / 1024.0;
            }
            else
            {
                fx = 1.0;
                fy = 1.0;
            }

            Bitmap 연승전그림 = getFindBmp("연승전게임인.png", fx, fy);
            Bitmap 준비완료그림 = getFindBmp("준비완료.png", fx, fy);

            while (true)
            {
                Bitmap tempBmp = getBmp();
                if (searchIMG(tempBmp, 연승전그림, out pt) > accurate)
                {
                    while (true)
                    {
                        Bitmap tempBmp2 = getBmp();
                        if (searchIMG(tempBmp2, 연승전그림, out pt) < accurate)
                        {
                            sendSpeicialKey("ESC");
                            Delay(10);
                            sendSpeicialKey("ENTER");
                            Delay(10);
                            sendSpeicialKey("ENTER");
                            Delay(500);
                            tempBmp2.Dispose();
                            break;
                        }
                        tempBmp2.Dispose();
                    }
                }
                else if (searchIMG(tempBmp, 준비완료그림, out pt) > accurate)
                {
                    pt.X = pt.X + stRect.left + 준비완료그림.Width / 2;
                    pt.Y = pt.Y + stRect.top + 준비완료그림.Height / 2;

                    mouseMove(pt);
                    sendSpeicialKey("LBC");
                    Delay(500);
                }
                tempBmp.Dispose();
            }
        }


        private void 스킨()
        {
            
            bool exeBool = false;
            bool doneBool = false;

            string nexonPath = @"C:\Nexon\SuddenAttack\game\fx\effect_sprite\dtx\";
                        

            while (true)
            {
                IntPtr findwindow = FindWindow("__GH_Sudden_Attack__", GameName);

                if(findwindow.Equals(IntPtr.Zero))          // 꺼져있을 경우
                {
                    if (exeBool)                    // 켜져 있었음
                    {
                        파일복구();
                    }
                    else                          // 꺼져 있었음
                    {
                        if (doneBool == false)
                        {
                            doneBool = false;
                            FileInfo fi = new FileInfo(nexonPath + "len00.dtx");
                            if (fi.Exists)                          //파일이 있을경우
                            {

                            }
                            else
                            {
                                파일복구();
                            }

                        }

                    }


                    exeBool = false;
                }
                else
                {                                           // 켜져 있을 경우
                    if (exeBool)                // 켜져 있었음
                    {
                        if(doneBool == false)
                        {
                            doneBool = true;
                            FileInfo fi = new FileInfo(nexonPath + "len00.dtx");
                            if (fi.Exists)          //파일이 있을 경우 
                            {
                                파일삭제();

                            }
                            else                          // 파일이 없을 경우
                            {

                            }

                        }
                    }
                    else                          // 꺼져 있었음
                    {
                        Delay(60000);               
                        파일삭제();
                    }

                    exeBool = true;
                }
            }


        }


        private void 파일복구()
        {
            string nexonPath = @"C:\Nexon\SuddenAttack\game\fx\effect_sprite\dtx\";
            string[] fileName = { "len00.dtx", "len001.dtx", "smoke00.dtx", "smoke01.dtx", "smoke02.dtx", "smoke03.dtx", "smoke04.dtx", "smoke05.dtx" };

            string nexonScopePath = @"C:\Nexon\SuddenAttack\game\sa_interface\hud\scope\";
            
            string[] scopeName = { "scope.dtx", "scope_bd.dtx", "scope_block.dtx", "scope_blued.dtx", "scope_matchlock.dtx" };

            for(int i = 0; i < fileName.Length; i++)
            {
                FileInfo fi = new FileInfo(fullpath+@"game\fx\effect_sprite\dtx\" + fileName[i]);

                if (fi.Exists)
                {
                    fi.CopyTo(nexonPath + fileName[i], true);
                }
            }

            for(int i = 0; i < scopeName.Length; i++)
            {
                FileInfo fi = new FileInfo(fullpath + @"raw_scope\" + scopeName[i]);
                if (fi.Exists)
                {
                    fi.CopyTo(nexonScopePath + scopeName[i], true);
                }
            }
            

        }

        private void 파일삭제()
        {
            string nexonPath = @"C:\Nexon\SuddenAttack\game\fx\effect_sprite\dtx\";
            string[] fileName = { "len00.dtx", "len001.dtx", "smoke00.dtx", "smoke01.dtx", "smoke02.dtx", "smoke03.dtx", "smoke04.dtx", "smoke05.dtx" };


            string nexonScopePath = @"C:\Nexon\SuddenAttack\game\sa_interface\hud\scope\";

            string[] scopeName = { "scope.dtx", "scope_bd.dtx", "scope_block.dtx", "scope_blued.dtx", "scope_matchlock.dtx" };


            for(int i = 0; i< fileName.Length; i++)
            {
                FileInfo fi = new FileInfo(nexonPath + fileName[i]);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }

            for (int i = 0; i < scopeName.Length; i++)
            {
                FileInfo fi = new FileInfo(fullpath + @"modified_scope\" + scopeName[i]);
                if (fi.Exists)
                {
                    fi.CopyTo(nexonScopePath + scopeName[i], true);
                }
            }

        }

        private void 연패시작_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = FindWindow(null, GameName);

            if (!hWnd.Equals(IntPtr.Zero))
            {
                // 윈도우가 최소화 되어 있다면 활성화 시킨다
                ShowWindowAsync(hWnd, 1);

                // 윈도우에 포커스를 줘서 최상위로 만든다
                SetForegroundWindow(hWnd);
            }

            연패_Thread = new Thread(new ThreadStart(연패));
            연패_Thread.Start();

            Form2 dlg = new Form2();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void 연승시작_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = FindWindow(null, GameName);

            if (!hWnd.Equals(IntPtr.Zero))
            {
                // 윈도우가 최소화 되어 있다면 활성화 시킨다
                ShowWindowAsync(hWnd, 1);

                // 윈도우에 포커스를 줘서 최상위로 만든다
                SetForegroundWindow(hWnd);
            }

            연승_Thread = new Thread(new ThreadStart(연승));
            연승_Thread.Start();

            Form2 dlg = new Form2();
            dlg.Owner = this;
            dlg.ShowDialog();
        }
    }
}
