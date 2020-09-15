using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 바람연
{
    public partial class Form1 : Form
    {
        #region 윈도우 함수
        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, int hWnd2, string lpsz1, string lpsz2);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        #endregion


        private IntPtr gamePtr;
        private IntPtr handlePtr;
        private IntPtr handlePtr2;


        #region 구조체

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
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


        #endregion


        private int gameWidth =964, gameHeight = 574;

        enum HOTKEY : int { F1, F1CTRL, F7, F2, HOME, END, F5, PAGEDOWN, PAGEUP };


        private Thread 도감작_Thread = null;



        Bitmap 완성bmp;

        Bitmap 완성됨bmp;
        Bitmap 진행중bmp;
        Bitmap 수리bmp;
        Bitmap 도감장소bmp;
        Bitmap 하위목록bmp;
        Bitmap 대장간bmp;



        public Form1()
        {
            InitializeComponent();
            RegisterHotKey(this.Handle, (int)HOTKEY.F2, 0, Keys.F2.GetHashCode());
            {
                완성bmp = new Bitmap(".\\img\\완성하기.png");
                완성됨bmp = new Bitmap(".\\img\\완성됨.png");
                진행중bmp = new Bitmap(".\\img\\진행중.png");
                수리bmp = new Bitmap(".\\img\\수리.png");
                도감장소bmp = new Bitmap(".\\search\\캡처.png");
                하위목록bmp = new Bitmap(".\\img\\sub.png");
                대장간bmp = new Bitmap(".\\img\\대장간.png");
            }
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case (int)HOTKEY.F2:
                        {

                            this.Invoke(new ThreadStart(delegate ()
                            {
                                this.Text = "대기 중";
                            }));

                            try
                            {
                                도감작_Thread.Abort();
                                도감작_Thread = null;
                            }
                            catch { }


                            break;


                            
                        }


                  

                }

            }
        }       //핫키

        private bool 움직일수있는공간찾기(Bitmap bmp,ref POINT pt)
        {
            Color color;

            int checkX = 460;

            int startY = 280, endY = 515;

            for(int y = startY; y <= endY; y++)
            {
                color = bmp.GetPixel(checkX, y);

                if(color.GetBrightness() > 0.13)
                {
                    pt.X = checkX;
                    pt.Y = y;
                    return true;
                }
            }




            return false;
        }


        private void 자동이동대기()
        {


            POINT tempPt = new POINT();
            DateTime lastTime = DateTime.Now.AddSeconds(10);
            Color color;
            Bitmap 즉시이동Bmp = new Bitmap(".\\img\\즉시이동.png");
            

            while(DateTime.Now <= lastTime)
            {
                Bitmap temp = getBmp(handlePtr);
                Debug.Write("이동중");
                bool success = true , success2 = true;

                if(searchIMG(temp,즉시이동Bmp,ref tempPt) < 0.8)
                {
                    success = false;
                }
                else
                {
                    Debug.Write("첫번째 이동 못찾음");
                }
                

                for(int y = 302; y <= 307; y++)
                {
                    color = temp.GetPixel(429, y);
                    if (!((color.GetSaturation() <= 0.24) && (color.GetBrightness() >= 0.90)))
                    {
                        Debug.WriteLine("두번째 이동 못찾음");
                        success2 = false;
                        break;
                    }

                }

                temp.Dispose();


                if (success || success2)
                {
                    lastTime = DateTime.Now.AddSeconds(10);
                }
            }

            Debug.WriteLine("");
            Debug.WriteLine("이동끝");
        }

        private void 도감작()
        {
            Debug.WriteLine("도감작 시작");
            
            POINT tempPt = new POINT();
            RECT 완성RECT = new RECT();
            {
                완성RECT.left = 740;
                완성RECT.top = 174;
                완성RECT.right = 836;
                완성RECT.bottom = 518;
            }
            RECT 수리RECT = new RECT();
            {
                수리RECT.left = 654;
                수리RECT.top = 24;
                수리RECT.right = 817;
                수리RECT.bottom = 142;
            }

            bool foundSuccess = false;

            this.Invoke(new ThreadStart(delegate ()
            {
                this.Text = "실행 중";
            }));

            while (true)
            {
                foundSuccess = false;
                gamePtr = FindWindow(null, "NoxPlayer");
                if (gamePtr != null)
                {
                    handlePtr = FindWindowEx(gamePtr, 0, "Qt5QWindowIcon", "ScreenBoardClassWindow");
                    if (handlePtr != null)
                    {
                        handlePtr2 = FindWindowEx(handlePtr, 0, "subWin", "sub");
                        if (handlePtr2 != null)
                        {
                            SetWindowPos(gamePtr, -2, 0, 0, gameWidth, gameHeight, 0x0002);
                        }
                        else
                        {
                            this.Invoke(new ThreadStart(delegate ()
                            {
                                this.Text = "대기중";
                            }));
                        
                            return;
                        }

                    }
                    else
                    {
                        this.Invoke(new ThreadStart(delegate ()
                        {
                            this.Text = "대기중";
                        }));
                       
                        return;
                    }
                }
                else
                {
                    this.Invoke(new ThreadStart(delegate ()
                    {
                        this.Text = "대기중";
                    }));
                    return;
                }

                Bitmap bmp = getBmp(handlePtr);
                if (도감100체크(bmp))
                {
                    mouseLeftClick(handlePtr, 145, 195);
                    Delay(4000);
                    tempPt.X = 782;
                    tempPt.Y = 212;

                    {
                        Bitmap bmp2 = getBmp(handlePtr);
                        Bitmap bmp_완성 = cropImage(bmp2, 완성RECT);
                        if (searchIMG(bmp_완성, 완성bmp, ref tempPt) > 0.8)
                        {
                            tempPt.X = tempPt.X + 완성RECT.left;
                            tempPt.Y = tempPt.Y + 완성RECT.top;
                        }
                        else
                        {
                            tempPt.X = 782;
                            tempPt.Y = 212;
                        }
                        bmp_완성.Dispose();
                        bmp2.Dispose();
                    }

                    for(int i = 0; i < 5; i++)
                    {
                        mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);
                        Delay(3000);
                        Debug.Write("도감 완료 클릭");
                    }

                    Debug.WriteLine("얍얍");
                    for (int i = 0; i < 30; i++)
                    {

                        Bitmap bmp2 = getBmp(handlePtr);
                        Bitmap bmp_완성 = cropImage(bmp2, 완성RECT);

                        if(searchIMG(bmp_완성,진행중bmp,ref tempPt) > 0.8)
                        {
                            foundSuccess = true;
                            Debug.WriteLine("도감에서 찾음");
                            bmp2.Dispose();
                            bmp_완성.Dispose();

                            tempPt.X = tempPt.X + 완성RECT.left +94;
                            tempPt.Y = tempPt.Y + 완성RECT.top;

                            mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);          //이동버튼 누르기
                            Delay(1000);
                            mouseLeftClick(handlePtr, 420, 300);          //걸어가기 버튼 누르기
                            Delay(1000);
                            자동이동대기();
                            mouseLeftClick(handlePtr, 918, 318);          //오토 누르기
                            Delay(500);
                            PostMessage(handlePtr, 0x0201, new IntPtr(0x0001), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                            Delay(500);
                            PostMessage(handlePtr, 0x0202, new IntPtr(0), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                            Delay(500);
                            mouseLeftClick(handlePtr, 20, 300);          //도감 누르기
                            Delay(500);
                            break;

                        }
                        else if (미완료도감(bmp2,ref tempPt))
                        {                       
                            foundSuccess = true;
                            bmp2.Dispose();
                            bmp_완성.Dispose();

                            Debug.WriteLine("최상위 도감에서 찾음");

                            tempPt.X = 393;
                            tempPt.Y = tempPt.Y - 36;

                            mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);          //버튼 펼치기
                            Delay(100);

                            for(int j = 0; j < 15; j++)
                            {
                                Bitmap tempBmp = getBmp(handlePtr);
                                움직일수있는공간찾기(tempBmp, ref tempPt);
                                mouseYSlowDrag(handlePtr, tempPt.X, tempPt.Y, tempPt.Y + 163);
                                tempBmp.Dispose();
                            }

                            for(int j = 0; j < 25; j++)
                            {
                                Bitmap tempBmp = getBmp(handlePtr);
                                움직일수있는공간찾기(tempBmp, ref tempPt);
                                mouseYSlowDrag(handlePtr, tempPt.X, tempPt.Y, tempPt.Y - 163);
                                tempBmp.Dispose();

                                tempBmp = getBmp(handlePtr);
                                Bitmap tempBmp2 = cropImage(tempBmp, 완성RECT);

                                if (searchIMG(tempBmp2, 진행중bmp, ref tempPt) > 0.8)
                                {
                                    Debug.WriteLine("도감에서 찾음");
                                    tempBmp.Dispose();
                                    tempBmp2.Dispose();


                                    tempPt.X = tempPt.X + 완성RECT.left + 94;
                                    tempPt.Y = tempPt.Y + 완성RECT.top;

                                    mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);          //이동버튼 누르기
                                    Delay(1000);
                                    mouseLeftClick(handlePtr, 420, 300);          //걸어가기 버튼 누르기
                                    Delay(1000);

                                    자동이동대기();


                                    mouseLeftClick(handlePtr, 918, 318);          //오토 누르기
                                    Delay(500);
                                    PostMessage(handlePtr, 0x0201, new IntPtr(0x0001), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                                    Delay(500);
                                    PostMessage(handlePtr, 0x0202, new IntPtr(0), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                                    Delay(500);
                                    mouseLeftClick(handlePtr, 20, 300);          //도감 누르기
                                    Delay(500);

                                    break;
                                }

                                tempBmp.Dispose();
                                tempBmp2.Dispose();
                            }

                            break;

                        }
                        else if(움직일수있는공간찾기(bmp2,ref tempPt))
                        {
                            Debug.WriteLine("움직일 수 있는공간 찾음");
                        }



                        bmp2.Dispose();
                        bmp_완성.Dispose();

                        mouseYSlowDrag(handlePtr, tempPt.X, tempPt.Y, tempPt.Y - 163);

                    }

                }
                Bitmap repairBmp = cropImage(bmp, 수리RECT);

                if (searchIMG(repairBmp,수리bmp,ref tempPt) > 0.8)
                {
                    
                    mouseLeftClick(handlePtr, tempPt.X + 수리RECT.left, tempPt.Y + 수리RECT.top);          // 수리 버튼 클릭
                    Delay(1000);
                    자동이동대기();
                    Delay(10000);
                    mouseLeftClick(handlePtr, 461, 366);            // 매크로 버튼 클릭
                    Delay(1000);
                    mouseLeftClick(handlePtr, 774, 369);            // 모두 고쳐줘 클릭;
                    Delay(1000);
                    도감복귀();
                    자동이동대기();
                    mouseLeftClick(handlePtr, 918, 318);          //오토 누르기
                    Delay(500);
                    PostMessage(handlePtr, 0x0201, new IntPtr(0x0001), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                    Delay(500);
                    PostMessage(handlePtr, 0x0202, new IntPtr(0), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                    Delay(500);
                    mouseLeftClick(handlePtr, 20, 300);          //도감 누르기
                    Delay(500);
                }
                else if(searchIMG(bmp,대장간bmp,ref tempPt) > 0.8)
                {
                    도감복귀();
                    자동이동대기();
                    mouseLeftClick(handlePtr, 918, 318);          //오토 누르기
                    Delay(500);
                    PostMessage(handlePtr, 0x0201, new IntPtr(0x0001), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                    Delay(500);
                    PostMessage(handlePtr, 0x0202, new IntPtr(0), new IntPtr((426 << 16) | (152 & 0xFFFF)));
                    Delay(500);
                    mouseLeftClick(handlePtr, 20, 300);          //도감 누르기
                    Delay(500);
                }

                repairBmp.Dispose();
                bmp.Dispose();
                

            }
        }

        private void 도감복귀()
        {
            POINT tempPt = new POINT();

            RECT 완성RECT = new RECT();
            {
                완성RECT.left = 740;
                완성RECT.top = 174;
                완성RECT.right = 836;
                완성RECT.bottom = 518;
            }

            mouseLeftClick(handlePtr, 799, 39);            // 메뉴 클릭;
            Delay(1000);
            mouseLeftClick(handlePtr, 727, 356);            // 도감 클릭;
            Delay(5000);
            if (searchIMG(tempBmp, 완성bmp, ref tempPt) > 0.8)
            {
                for (int i = 0; i < 5; i++)
                {
                    mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);
                    Delay(3000);
                    Debug.Write("도감 완료 클릭");
                }
            }

            tempBmp.Dispose();


            mouseLeftClick(handlePtr, 300, 100);            // 사냥 클릭;
            Delay(5000);
            {
                tempPt.Y = 155;
                if (radioButton국내.Checked)
                {
                    tempPt.X = 97;
                }
                else if (radioButton부여.Checked)
                {
                    tempPt.X = 278;
                }
                else if (radioButton12지신.Checked)
                {
                    tempPt.X = 435;
                }
                else if (radioButton산적.Checked)
                {
                    tempPt.X = 602;
                }
                else if (radioButton민중.Checked)
                {
                    tempPt.X = 771;
                }
            }                                               // 좌표 설정
            mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);
            Delay(5000);

            for(int i = 0; i < 25; i++)
            {
                Bitmap tempBmp = getBmp(handlePtr);
                if(searchIMG(tempBmp,도감장소bmp,ref tempPt) > 0.9)
                {
                    
                    Delay(1000);
                    Bitmap tempBmp2 = getBmp(handlePtr);
                    if(searchIMG(tempBmp2,도감장소bmp,ref tempPt) > 0.9)
                    {
                        RECT 하위RECT = new RECT();
                        {
                            하위RECT.left = 370;
                            하위RECT.top = tempPt.Y;
                            하위RECT.right = 450;
                            하위RECT.bottom = tempPt.Y + 80;
                        }
                        Bitmap tempBmp3 = cropImage(tempBmp2, 하위RECT);
                        if(searchIMG(tempBmp3,하위목록bmp,ref tempPt) > 0.8)
                        {
                            
                            mouseLeftClick(handlePtr, 393, tempPt.Y+하위RECT.top);
                            Delay(2000);


                            for(int j = 0; j < 15; j++)
                            {
                                Bitmap tempBmp4 = getBmp(handlePtr);
                                Bitmap tempBmp완성 = cropImage(tempBmp4, 완성RECT);
                                if(searchIMG(tempBmp완성,진행중bmp,ref tempPt) > 0.8)
                                {
                                    tempBmp완성.Dispose();
                                    tempBmp4.Dispose();


                                    tempPt.X = tempPt.X + 완성RECT.left + 94;
                                    tempPt.Y = tempPt.Y + 완성RECT.top;

                                    mouseLeftClick(handlePtr, tempPt.X, tempPt.Y);          //이동버튼 누르기
                                    Delay(1000);
                                    mouseLeftClick(handlePtr, 420, 300);          //걸어가기 버튼 누르기
                                    Delay(500);



                                    break;
                                }

                                움직일수있는공간찾기(tempBmp4, ref tempPt);
                                mouseYSlowDrag(handlePtr, tempPt.X, tempPt.Y, tempPt.Y - 163);
                                tempBmp완성.Dispose();
                                tempBmp4.Dispose();
                                

                            }

                        }
                        tempBmp3.Dispose();

                    }
                    tempBmp2.Dispose();
                    tempBmp.Dispose();

                    break;
                }
                tempBmp.Dispose();
                mouseYSlowDrag(handlePtr, 760, 340, 340 - 163);

            }

            mouseLeftClick(handlePtr, 934, 14);
            Delay(100);
            mouseLeftClick(handlePtr, 934, 14);
            Delay(100);
            mouseLeftClick(handlePtr, 934, 14);
            Delay(100);

        }

        private Bitmap cropImage(Bitmap src, RECT rect)
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

        private bool 미완료도감(Bitmap bmp, ref POINT pt)
        {
            int checkX = 690;
            int checkR1 = 40, checkG1 = 40, checkB1 = 40 ,checkR2 = 60, checkG2 = 50, checkB2 = 50;

            
            int startY = 280, endY = 515;


            Color color;
            for(int y = startY; y <= endY; y++)
            {
                color = bmp.GetPixel(checkX, y);

                if ((color.R >= checkR1) && (color.G >= checkG1) && (color.B >= checkB1) && (color.R <= checkR2) && (color.G <= checkG2) && (color.B <= checkB2))
                {
                    pt.X = checkX;
                    pt.Y = y;


                    
                    return true;
                }





            }




            return false;
        }

        private bool 도감100체크(Bitmap bmp)
        {
            Color temp;


            temp = bmp.GetPixel(58, 36);

            if(!((temp.R == 180) && (temp.G == 43) && (temp.B == 0)))
            {
                return false;
            }
            

            for(int x = 47; x <= 48; x++)
            {
                for(int y = 190; y <= 200; y++)
                {
                    temp = bmp.GetPixel(x, y);
                    if( !((temp.R>=210) && (temp.G >=210) && (temp.B >= 210)))
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public double searchIMG(Bitmap screen_img, Bitmap find_img, ref POINT pt)
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
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return 0.0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("시작");
            POINT tempPt = new POINT();
            gamePtr = FindWindow(null, "NoxPlayer");
            if(gamePtr != null)
            {
                handlePtr = FindWindowEx(gamePtr, 0, "Qt5QWindowIcon", "ScreenBoardClassWindow");
                if(handlePtr != null)
                {
                    handlePtr2 = FindWindowEx(handlePtr, 0, "subWin", "sub");
                    if (handlePtr2 != null)
                    {
                        Bitmap temp = getBmp(handlePtr);
                        temp.Save("asd.png");
                        
                    }
                    else
                    {
                        MessageBox.Show("error");
                    }

                }
                else
                {
                    MessageBox.Show("error");
                }
            }
            else
            {
                MessageBox.Show("error");
            }
        }


        private void mouseLeftClick(IntPtr handle, int x, int y)
        {
            PostMessage(handle, 0x0201, new IntPtr(0x0001), new IntPtr((y << 16) | (x & 0xFFFF)));
            PostMessage(handle, 0x0202, new IntPtr(0), new IntPtr((y << 16) | (x & 0xFFFF)));
            return;
        }

        private void mouseYSlowDrag(IntPtr handle, int x, int y1, int y2)
        {
            int addNum = (y1 > y2) ? -1 : 1;
            PostMessage(handle, 0x0201, new IntPtr(0x0001), new IntPtr((y1 << 16) | (x & 0xFFFF)));
            Delay(100);
            
            for (; y2 != y1; y1 += addNum)
            {
                PostMessage(handle, 0x0200, new IntPtr(0x0001), new IntPtr((y1 << 16) | (x & 0xFFFF)));
                Delay(5);
            
            }
            Delay(100);


            PostMessage(handle, 0x0202, new IntPtr(0), new IntPtr((y2 << 16) | ((x-1) & 0xFFFF)));
            PostMessage(handle, 0x0202, new IntPtr(0), new IntPtr((y2 << 16) | ((x - 1) & 0xFFFF)));
        }

        private void mouseYDrag(IntPtr handle, int x,int y1, int y2)
        {
            int addNum = (y1 > y2) ? -1 : 1;
            PostMessage(handle, 0x0201, new IntPtr(0x0001), new IntPtr((y1 << 16) | (x & 0xFFFF)));

            Debug.WriteLine("" + y1);
            for (; y2!=y1 ;y1+=addNum)
            {
                PostMessage(handle, 0x0200, new IntPtr(0x0001), new IntPtr((y1 << 16) | (x & 0xFFFF)));
                Delay(10);
                Debug.WriteLine("" + y1);
            }

            PostMessage(handle, 0x0202, new IntPtr(0), new IntPtr((y2 << 16) | (x & 0xFFFF)));
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (도감작_Thread == null)
            {
                도감작_Thread = new Thread(new ThreadStart(도감작));
                도감작_Thread.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process p in mProcess)
                p.Kill();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("시작");
            POINT tempPt = new POINT();
            gamePtr = FindWindow(null, "NoxPlayer");
            if (gamePtr != null)
            {
                handlePtr = FindWindowEx(gamePtr, 0, "Qt5QWindowIcon", "ScreenBoardClassWindow");
                if (handlePtr != null)
                {
                    handlePtr2 = FindWindowEx(handlePtr, 0, "subWin", "sub");
                    if (handlePtr2 != null)
                    {
                        도감복귀();

                    }
                    else
                    {
                        MessageBox.Show("error");
                    }

                }
                else
                {
                    MessageBox.Show("error");
                }
            }
            else
            {
                MessageBox.Show("error");
            }

            
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
    }
}
