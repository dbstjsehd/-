using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 얍
{
    class yapHeader
    {
        #region 윈도우 함수
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern Int32 GetCursorPos(out POINT pt);


        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool IsWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int GetWindowRect(int hwnd, ref RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);


        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        
       


        

        #endregion


        #region 구조체
        internal struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator System.Drawing.Point(POINT point)
            {
                return new System.Drawing.Point(point.X, point.Y);
            }
        }

        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion



        #region 전역변수
        private SerialPort arduSerial;

        private int delayRate = 0;
        private double mouseXratio = 1.0;
        private double mouseYratio = 1.0;

        Random rand;

        #endregion


        #region 생성자

        public yapHeader()
        {
            rand = new Random();
            this.arduSerial = new SerialPort();
            if (!arduinoSetting())
            {
                MessageBox.Show("생성자 호출 오류");
            }
        }

        #endregion

        #region getter setter
        internal int getDelayRate()
        {
            return delayRate;
        }
        internal double getMouseXratio()
        {
            return mouseXratio;
        }
        internal double getMouseYratio()
        {
            return mouseYratio;
        }



        #endregion

        #region private함수
        private bool arduinoSetting()
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
                            arduSerial.PortName = temp[0];
                            arduSerial.BaudRate = 9600;
                            arduSerial.Open();
                            return true;
                        }
                    }
                }
            }
            catch (ManagementException e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }                                   //아두이노 초반 세팅


        
        #endregion



        internal void arduEnd()
        {
            if(arduSerial != null)
            {
                if (arduSerial.IsOpen)
                {
                    arduSerial.Close();
                }
            }
        }

        internal void randomRangeMouse(POINT pt1,POINT pt2)
        {
            if((pt1.X == 0) && (pt1.Y == 0))
            {
                Debug.WriteLine("error일 확률 존재");
                return;
            }

            int tempInt;

            if (pt1.X > pt2.X)
            {
                tempInt = pt1.X;
                pt1.X = pt2.X;
                pt2.X = tempInt;
            }

            if(pt1.Y > pt2.Y)
            {
                tempInt = pt1.Y;
                pt1.Y = pt2.Y;
                pt2.Y = tempInt;
            }

            POINT thisPt = new POINT();
            GetCursorPos(out thisPt);

            pt1.X = rand.Next(pt1.X, pt2.X);
            pt1.Y = rand.Next(pt1.Y, pt2.Y);

            pt1.X = (int)((pt1.X - thisPt.X) * mouseXratio);
            pt1.Y = (int)((pt1.Y - thisPt.Y) * mouseYratio);

            

            arduSerial.Write("!" + pt1.X + "," + pt1.Y + ".+");

            Delay(10);

            return;

        }

        internal void mouseMove(POINT pt)
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

            arduSerial.Write("!" + pt.X + "," + pt.Y + ".+");

            Delay(10);

            return;

        }
        internal void mouseMove(int x,int y)
        {
            if ((x == 0) && (y == 0))
            {
                Debug.WriteLine("error일 확률 존재");
                return;
            }

            POINT thisPt = new POINT();
            GetCursorPos(out thisPt);

            x = (int)((x - thisPt.X) * mouseXratio);
            y = (int)((y - thisPt.Y) * mouseYratio);

            

            arduSerial.Write("!" + x + "," + y + ".+");

            Delay(10);

            return;

        }

        internal IntPtr getHwndName(string gameName)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.MainWindowTitle.IndexOf(gameName, StringComparison.InvariantCulture) > -1)
                {
                    return process.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
        }

        internal Bitmap getBmp(IntPtr findwindow)
        {
            while (true)
            {
                try
                {
                    System.Drawing.Rectangle rect = System.Drawing.Rectangle.Empty;

                    Graphics Graphicsdata = Graphics.FromHwnd(findwindow);

                    //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                    rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                    //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                    Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                    //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                        IntPtr hdc = g.GetHdc();
                        try
                        {
                            PrintWindow(findwindow, hdc, 0x2);
                        }
                        finally
                        {
                            g.ReleaseHdc(hdc);
                        }
                    }
                    Graphicsdata.Dispose();





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
                }
            }

        }


        internal void randomTimeSpecialKey(string s,int min, int max)
        {
            int tempDelay = rand.Next(min,max);
            arduSerial.Write("$" + tempDelay + "+");            // 아두이노 딜레이 설정 $
            arduSerial.Write("@" + s + "+");                    // 스페셜 키 보내기 @
            Delay(tempDelay * 2 + 1);                           // 대기

            Debug.WriteLine(s + " 특수키 , 딜레이:" + tempDelay);
        }


        internal void randomTimeMouseLeftClick(int min, int max)
        {
            arduSerial.Write("@LBD+");
            Delay(rand.Next(min, max));
            arduSerial.Write("@LBU+");
            Delay(rand.Next(min, max));
        }

        internal void randomTimeMouseRightClick(int min, int max)
        {
            arduSerial.Write("@RBD+");
            Delay(rand.Next(min, max));
            arduSerial.Write("@RBU+");
            Delay(rand.Next(min, max));
        }

        

        internal void randomTimeKey(string s,int min, int max)
        {
            
            int tempDelay = 14;
            for(int i = 0; i < s.Length; i++)
            {
                tempDelay = rand.Next(min, max);
                arduSerial.Write("$" + tempDelay + "+");            // 아두이노 딜레이 설정 $
                arduSerial.Write(""+s[i]);                          // 키 보내기
                Delay(tempDelay * 2 + 1);                           // 대기


                Debug.WriteLine(s + "키 , 딜레이:"+tempDelay);
            }
        }

        internal Bitmap getFindBmp(String path)
        {
            Bitmap findBmp;
            findBmp = new Bitmap(".\\img\\" + path);
            return findBmp;

        }
        internal Bitmap getFindBmp(String path, double fx, double fy)
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
        internal double searchIMG(Bitmap screen_img, Bitmap find_img, out POINT pt)
        {
            try
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
                    

                    //이미지를 찾았을 경우 클릭이벤트를 발생!!
                    pt.X = maxloc.X;
                    pt.Y = maxloc.Y;

                    return maxval;
                }
            }
            catch
            {
                pt.X = 1;
                pt.Y = 1;
                return 0.0;
            }
        }
        internal void setDelay(string s)
        {
            try
            {
                delayRate = Int32.Parse(s);
            }
            catch
            {
                delayRate = 14;
            }
            arduSerial.Write("$" + delayRate + "+");
            Delay(1);
        }

        internal void mouseTest()
        {
            POINT thisPt = new POINT();
            POINT afterPt = new POINT();

            GetCursorPos(out thisPt);

            arduSerial.Write("!127,127.+");
            Delay(2);

            GetCursorPos(out afterPt);

            thisPt.X = afterPt.X - thisPt.X;
            thisPt.Y = afterPt.Y - thisPt.Y;



            mouseXratio = 127.0 / thisPt.X;
            mouseYratio = 127.0 / thisPt.Y;


            


        }       // 마우스 감도 설정

        internal Bitmap cropImage(Bitmap src, RECT rect)
        {
            Rectangle cropRect = new Rectangle(rect.left, rect.top, (rect.right - rect.left), (rect.bottom - rect.top));
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;


        }
        internal void sendKey(string s)
        {
            arduSerial.Write(s);
            Delay(s.Length * delayRate * 2);
        }

        public void sendSpecialKey(string s)
        {
            Debug.WriteLine("키 " + s);
            arduSerial.Write("@" + s + "+");
            Delay(delayRate * 2);
        }       // 특수 키 보내기.... ESC -> ESC ; 

        internal void 원격종료()
        {
            IntPtr wow64backup = IntPtr.Zero;
            if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
            {
                Wow64DisableWow64FsRedirection(ref wow64backup);
            }

            System.Diagnostics.ProcessStartInfo cmd = new System.Diagnostics.ProcessStartInfo();
            Process process = new Process();

            cmd.FileName = @"cmd";
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;

            cmd.RedirectStandardOutput = true;
            cmd.RedirectStandardInput = true;
            cmd.RedirectStandardError = true;

            process.EnableRaisingEvents = false;
            process.StartInfo = cmd;
            process.Start();

            process.StandardInput.Write("for /f \"tokens=4 delims= \" %G in ('tasklist /FI \"IMAGENAME eq tasklist.exe\" /NH') do SET RDP_SESSION=%G" + Environment.NewLine);
            process.StandardInput.Write("tscon %RDP_SESSION% /dest:console" + Environment.NewLine);

            process.StandardInput.Close();

            process.WaitForExit();
            process.Close();

            if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
            {
                Wow64RevertWow64FsRedirection(wow64backup);
            }
        }

        internal static DateTime Delay(int ms)
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


        internal Bitmap fullscreenCapture()
        {

            while (true)
            {
                try

                {

                    //Creating a new Bitmap object

                    Bitmap captureBitmap = new Bitmap(1024, 768, PixelFormat.Format32bppArgb);


                    //Bitmap captureBitmap = new Bitmap(int width, int height, PixelFormat);

                    //Creating a Rectangle object which will  

                    //capture our Current Screen

                    Rectangle captureRectangle = Screen.AllScreens[0].Bounds;



                    //Creating a New Graphics Object

                    Graphics captureGraphics = Graphics.FromImage(captureBitmap);



                    //Copying Image from The Screen

                    captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);



                    //Saving the Image File (I am here Saving it in My E drive).





                    //Displaying the Successfull Result
                    captureGraphics.Dispose();
                    return captureBitmap;

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

                }
            }
        }
    }
}
