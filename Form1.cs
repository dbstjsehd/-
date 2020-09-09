using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Blob;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Windows.Forms;
using Telegram;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace 빡자사
{

    public partial class Form1 : Form
    {
        #region 외부 함수

        public class AES
        {
            //AES_256 암호화
            public String AESEncrypt256(String Input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = Convert.ToBase64String(xBuff);
                return Output;
            }


            //AES_256 복호화
            public String AESDecrypt256(String Input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = Encoding.UTF8.GetString(xBuff);
                return Output;
            }
        }
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern Int32 GetCursorPos(out POINT pt);

        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32")]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32")]
        public static extern int GetWindowRect(int hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);


        #endregion

        #region 구조체



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

        #endregion

        #region 상수



        const double accurate = 0.8;


        const String GameName = "Gersang";

        Byte[] 동의그림;
        Bitmap 로그인그림;
        Byte[] 사냥중그림;
        Bitmap 접속그림;
        Bitmap 거탐그림;
        Bitmap 닫기그림;
        Bitmap 취소그림;
        Byte[] 대기중그림;
        Byte[] 계속하기그림;
        Bitmap 비밀번호그림;
        Bitmap 영자채팅그림;

        Byte[] 의원그림;

        Bitmap 게임가드에러그림;
        Bitmap 좌판닫기그림;
        Bitmap 배고픔그림;
        Bitmap 삼색채그림;
        Bitmap 무게감지그림;

        OpenCvSharp.ImgHash.PHash model;
        #endregion

        #region 변수

        HtmlDocument HD;

        int delayRate = 14;

        double mouseXratio = 1.0;
        double mouseYratio = 1.0;

        bool login = false;

        bool otp = false;

        int monsterTotal = 0;

        SerialPort arduSerial;

        IntPtr gersangHwnd;

        Process currentProcess;

        public Thread 반자사_Thread;

        private Telegram.Bot.TelegramBotClient Bot;
        String telegram_chat_id;

        Form2 dlg;

        Bitmap[] monster;
        int monsterArraySize;
        bool[] monsterPosition = new bool[4];

        bool checkAlaram = false;


        RECT chatRange = new RECT();
        RECT weightRange = new RECT();
        RECT 계속하기범위 = new RECT();
        RECT 사냥중범위 = new RECT();
        RECT 대기중범위 = new RECT();
        RECT 의원범위 = new RECT();
        RECT 동의범위 = new RECT();
        RECT 게임가드에러범위 = new RECT();

        RECT[] f11Range = new RECT[2];
        RECT[] monsterLocationRange = new RECT[4];


        POINT[] positionStart = new POINT[4];
        POINT[][] skillPt = new POINT[4][];
        POINT[] mapMovePt = new POINT[4];



        DateTime lastImageTime = DateTime.Now;
        Bitmap lastImage;
        #endregion




        public void 반자사()
        {
            try
            {

                DateTime lastClick = DateTime.Now;
                DateTime runTime = DateTime.Now;
                int count = 0;






                bool detect = false;
                bool inbattle = false;


                POINT pt;

                while (true)
                {


                    if ((gersangHwnd == IntPtr.Zero) || (!IsWindow(gersangHwnd)))
                    {
                        this.Invoke(new ThreadStart(delegate ()
                        {
                            HD = webBrowser1.Document;
                            HD.InvokeScript("gameStart", new object[] { "1" });
                        }));





                        IntPtr tempHwnd = IntPtr.Zero;
                        bool succees = false;


                        for (int i = 0; i < 60; i++)
                        {
                            tempHwnd = FindWindow(null, GameName);

                            if (tempHwnd != IntPtr.Zero)
                            {
                                gersangHwnd = tempHwnd;

                                t_id.Enabled = false;
                                t_pass.Enabled = false;
                                t_code.Enabled = false;

                                succees = true;
                                break;


                            }


                            Delay(1000);
                        }


                        if (!succees)               // 텔레그램 문자 보내기
                        {
                            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 오류 발생. 거상을 확인해주세요.");
                            this.Invoke(new Action(delegate ()
                            {

                                dlg.매크로종료.PerformClick();
                            }));
                            return;
                        }

                        // 윈도우가 최소화 되어 있다면 활성화 시킨다
                        ShowWindowAsync(gersangHwnd, 1);

                        // 윈도우에 포커스를 줘서 최상위로 만든다
                        SetForegroundWindow(gersangHwnd);

                        // 윈도우를 0,0 으로 옮긴다.
                        SetWindowPos(gersangHwnd, 0, 1, 0, 0, 0, 0x4 | 0x1 | 0x0040);



                    }       // 거상이 종료되었을 경우
                    if (error_screen())
                    {
                        errorCheck();
                        SendMessage(gersangHwnd, 0x0112, 0xF060, 0);
                        gersangHwnd = IntPtr.Zero;
                        Delay(10000);
                        continue;
                    }


                    // 윈도우가 최소화 되어 있다면 활성화 시킨다
                    ShowWindowAsync(gersangHwnd, 1);

                    // 윈도우에 포커스를 줘서 최상위로 만든다
                    SetForegroundWindow(gersangHwnd);

                    // 윈도우를 0,0 으로 옮긴다.
                    SetWindowPos(gersangHwnd, 0, 1, 0, 0, 0, 0x4 | 0x1 | 0x0040);

                    RECT stRect = default(RECT);
                    GetWindowRect((int)FindWindow(null, GameName), ref stRect);

                    this.Invoke(new Action(delegate ()
                    {

                        dlg.Location = new System.Drawing.Point(0, 800);
                        dlg.Size = new System.Drawing.Size(500, 100);
                    }));

                    Bitmap firstBmp = getBmp();
                    if (firstBmp == null)
                    {
                        continue;
                    }
                    Bitmap 동의bmp = cropImage(firstBmp, 동의범위);
                    Bitmap 계속하기bmp = cropImage(firstBmp, 계속하기범위);
                    Bitmap 사냥중bmp = cropImage(firstBmp, 사냥중범위);
                    Bitmap 대기중bmp = cropImage(firstBmp, 대기중범위);
                    Bitmap 게임가드에러bmp = cropImage(firstBmp, 게임가드에러범위);


                    if (searchIMG(사냥중bmp, 사냥중그림) > accurate)
                    {
                        sendSpecialKey("F11");
                        if (!inbattle)
                        {
                            count++;
                            inbattle = true;
                        }
                        if ((count > 148) && (checkAlaram == false))
                        {
                            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 캐릭터가 전투횟수가 거의 꽉 찼어요! 확인해주세요.");
                        };
                        int foundMob = 0;
                        pt.X = (stRect.left + stRect.right) / 2;
                        pt.Y = (stRect.top + stRect.bottom) / 2;
                        mouseMove(pt);
                        sendKey("0s0=s");
                        if (!f11판별())
                        {
                            pt.X = stRect.left + 987;
                            pt.Y = stRect.top + 745;
                            mouseMove(pt);
                            mouseLeftClick();
                            pt.X = stRect.left + 515;
                            pt.Y = stRect.top + 450;
                            mouseMove(pt);
                            mouseLeftClick();
                            pt.X = stRect.left + 524;
                            pt.Y = stRect.top + 441;
                            mouseMove(pt);
                            mouseLeftClick();
                            pt.X = stRect.left + 524;
                            pt.Y = stRect.top + 441;
                            mouseMove(pt);
                            mouseLeftClick();
                            Delay(3000);
                            sendKey("     nnnnn");

                            동의bmp.Dispose();
                            계속하기bmp.Dispose();
                            사냥중bmp.Dispose();
                            대기중bmp.Dispose();
                            firstBmp.Dispose();
                            게임가드에러bmp.Dispose();


                        }       //에러나면 퇴각
                        if (!탭여부())
                        {
                            sendSpecialKey("TAB");
                        }
                        if (monsterTotal > 3 || monsterTotal < 2)
                        {
                            pt.X = stRect.left + 987;
                            pt.Y = stRect.top + 745;
                            mouseMove(pt);
                            mouseLeftClick();
                            pt.X = stRect.left + 515;
                            pt.Y = stRect.top + 450;
                            mouseMove(pt);
                            mouseLeftClick();

                            pt.X = stRect.left + 524;
                            pt.Y = stRect.top + 441;
                            mouseMove(pt);
                            mouseLeftClick();

                            pt.X = stRect.left + 524;
                            pt.Y = stRect.top + 441;
                            mouseMove(pt);
                            mouseLeftClick();
                            Delay(2000);
                        }           // 몬스터 부대 수가 이상하면 퇴각
                        else
                        {
                            Thread 몬스터위치_Thread = new Thread(new ThreadStart(몬스터위치));
                            몬스터위치_Thread.Start();
                            #region 안에 변수 초기화
                            DateTime 전투시작시간 = DateTime.Now;
                            int retreat2 = Convert.ToInt32(퇴각시간2부대.Value);
                            int retreat3 = Convert.ToInt32(퇴각시간3부대.Value);

                            String tempString = "";
                            POINT[] tempMapMovePt = new POINT[4];
                            for (int i = 0; i < 4; i++)
                            {
                                tempMapMovePt[i].X = stRect.left + mapMovePt[i].X;
                                tempMapMovePt[i].Y = stRect.top + mapMovePt[i].Y;
                            }
                            POINT[][] tempSkillpt = new POINT[4][];
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    tempSkillpt[i] = new POINT[3];
                                    for (int j = 0; j < 3; j++)
                                    {
                                        tempSkillpt[i][j].X = skillPt[i][j].X + stRect.left;
                                        tempSkillpt[i][j].Y = skillPt[i][j].Y + stRect.top;
                                    }
                                }
                            }           // 스킬 찍을 곳 정하기
                            #endregion

                            tempString = 시작스킬.Text.ToString();
                            사냥(tempString);
                            sendKey("-");
                            mouseRightClick();
                            sendSpecialKey("LCD");
                            sendKey("s");
                            sendSpecialKey("LCU");




                            int myPosition = 내위치();
                            if (myPosition == -1)
                            {
                                pt.X = stRect.left + 987;
                                pt.Y = stRect.top + 745;
                                mouseMove(pt);
                                mouseLeftClick();
                                pt.X = stRect.left + 515;
                                pt.Y = stRect.top + 450;
                                mouseMove(pt);
                                mouseLeftClick();
                                pt.X = stRect.left + 524;
                                pt.Y = stRect.top + 441;
                                mouseMove(pt);
                                mouseLeftClick();
                                pt.X = stRect.left + 524;
                                pt.Y = stRect.top + 441;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(3000);
                                sendKey("     nnnnn");



                                동의bmp.Dispose();
                                계속하기bmp.Dispose();
                                사냥중bmp.Dispose();
                                대기중bmp.Dispose();
                                firstBmp.Dispose();
                                게임가드에러bmp.Dispose();

                                continue;
                            }       //퇴각

                            String[] skills = new String[3];
                            skills[0] = 왼쪽스킬.Text.ToString();
                            skills[1] = 오른쪽스킬.Text.ToString();
                            skills[2] = 가운데스킬.Text.ToString();


                            {
                                mouseMove(tempMapMovePt[(myPosition + 2) % 4]);     //화면 가운데로 이동
                                Delay(1000);

                                pt.X = (stRect.left + stRect.right) / 2;
                                pt.Y = (stRect.top + stRect.bottom) / 2;
                                mouseMove(pt);
                                sendSpecialKey("LCD");
                                sendKey("z");
                                sendSpecialKey("LCU");
                                mouseLeftClick();

                                tempString = 맵가운데.Text.ToString();
                                사냥(tempString);

                            }       //화면 가운데 스킬

                            몬스터위치_Thread.Join();
                            몬스터위치_Thread = null;

                            int[] tempMonster = new int[3];
                            tempMonster[0] = (myPosition + 1) % 4;
                            tempMonster[1] = (myPosition + 3) % 4;
                            tempMonster[2] = (myPosition + 2) % 4;




                            POINT summonPt = new POINT();
                            POINT skillUsePt = new POINT();
                            for (int i = 0; i < 3; i++)
                            {
                                if (monsterPosition[tempMonster[i]])
                                {
                                    DateTime findTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, 5000));
                                    sendKey("-----");
                                    mouseMove(tempMapMovePt[tempMonster[i]]);
                                    Delay(10);
                                    if (i == 2)
                                    {
                                        Delay(900);
                                    }
                                    while (true)
                                    {
                                        if (DateTime.Now >= findTime)
                                        {
                                            pt.X = (stRect.left + stRect.right) / 2;
                                            pt.Y = (stRect.top + stRect.bottom) / 2;
                                            mouseMove(pt);          //화면 정중앙으로 마우스 이동
                                            break;
                                        }

                                        Bitmap tempBmp2 = getBmp();
                                        if (tempBmp2 == null)
                                        {
                                            continue;
                                        }
                                        // 윈도우가 최소화 되어 있다면 활성화 시킨다
                                        ShowWindowAsync(gersangHwnd, 1);

                                        // 윈도우에 포커스를 줘서 최상위로 만든다
                                        SetForegroundWindow(gersangHwnd);

                                        // 윈도우를 0,0 으로 옮긴다.
                                        SetWindowPos(gersangHwnd, 0, 1, 0, 0, 0, 0x4 | 0x1 | 0x0040);
                                        Bitmap monsterLocationBmp = null;
                                        RECT tempRectangle = new RECT();
                                        if (myPosition == 1)
                                        {
                                            if (tempMonster[i] == 2)
                                            {
                                                tempRectangle.left = monsterLocationRange[3].left;
                                                tempRectangle.right = monsterLocationRange[3].right;
                                                tempRectangle.top = monsterLocationRange[3].top;
                                                tempRectangle.bottom = monsterLocationRange[2].bottom;
                                            }
                                            else if (tempMonster[i] == 0)
                                            {
                                                tempRectangle.left = monsterLocationRange[3].left;
                                                tempRectangle.right = monsterLocationRange[3].right;
                                                tempRectangle.top = monsterLocationRange[0].top;
                                                tempRectangle.bottom = monsterLocationRange[0].bottom;
                                            }
                                            else
                                            {
                                                tempRectangle.left = monsterLocationRange[tempMonster[i]].left;
                                                tempRectangle.right = monsterLocationRange[tempMonster[i]].right;
                                                tempRectangle.top = monsterLocationRange[tempMonster[i]].top;
                                                tempRectangle.bottom = monsterLocationRange[tempMonster[i]].bottom;
                                            }
                                        }
                                        else if (myPosition == 2)
                                        {
                                            if (tempMonster[i] == 3)
                                            {
                                                tempRectangle.left = monsterLocationRange[3].left;
                                                tempRectangle.right = monsterLocationRange[3].right;
                                                tempRectangle.top = monsterLocationRange[0].top;
                                                tempRectangle.bottom = monsterLocationRange[2].bottom;

                                            }
                                            else if (tempMonster[i] == 1)
                                            {

                                                tempRectangle.left = monsterLocationRange[1].left;
                                                tempRectangle.right = monsterLocationRange[1].right;
                                                tempRectangle.top = monsterLocationRange[0].top;
                                                tempRectangle.bottom = monsterLocationRange[2].bottom;

                                            }
                                            else
                                            {
                                                tempRectangle.left = monsterLocationRange[tempMonster[i]].left;
                                                tempRectangle.right = monsterLocationRange[tempMonster[i]].right;
                                                tempRectangle.top = monsterLocationRange[tempMonster[i]].top;
                                                tempRectangle.bottom = monsterLocationRange[tempMonster[i]].bottom;
                                            }
                                        }
                                        else
                                        {
                                            tempRectangle.left = monsterLocationRange[tempMonster[i]].left;
                                            tempRectangle.right = monsterLocationRange[tempMonster[i]].right;
                                            tempRectangle.top = monsterLocationRange[tempMonster[i]].top;
                                            tempRectangle.bottom = monsterLocationRange[tempMonster[i]].bottom;
                                        }
                                        monsterLocationBmp = cropImage(tempBmp2, tempRectangle);

                                        tempBmp2.Dispose();
                                        bool 분홍색찾음 = false;
                                        if (분홍색찾기(monsterLocationBmp, myPosition, tempMonster[i], out skillUsePt, out summonPt))
                                        {
                                            pt.X = (stRect.left + stRect.right) / 2;
                                            pt.Y = (stRect.top + stRect.bottom) / 2;
                                            mouseMove(pt);
                                            분홍색찾음 = true;
                                        }
                                        monsterLocationBmp.Dispose();
                                        if (분홍색찾음)
                                        {
                                            while (true)
                                            {
                                                tempBmp2 = getBmp();
                                                if (tempBmp2 == null)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            monsterLocationBmp = cropImage(tempBmp2, tempRectangle);
                                            tempBmp2.Dispose();

                                            if (분홍색찾기(monsterLocationBmp, myPosition, tempMonster[i], out skillUsePt, out summonPt))
                                            {
                                                skillUsePt.X = skillUsePt.X + stRect.left + tempRectangle.left;
                                                skillUsePt.Y = skillUsePt.Y + stRect.top + tempRectangle.top;
                                                summonPt.X = summonPt.X + stRect.left + tempRectangle.left;
                                                summonPt.Y = summonPt.Y + stRect.top + tempRectangle.top;

                                                if (summonPt.X < stRect.left + 70)
                                                {
                                                    summonPt.X = stRect.left + 70;
                                                }
                                                else if (summonPt.X > stRect.left + 960)
                                                {
                                                    summonPt.X = stRect.left + 960;
                                                }

                                                if (summonPt.Y < stRect.top + 95)
                                                {
                                                    summonPt.Y = stRect.top + 95;
                                                }
                                                else if (summonPt.Y > stRect.top + 675)
                                                {
                                                    summonPt.Y = stRect.top + 675;
                                                }

                                                if(skillUsePt.X < stRect.left + 70)
                                                {
                                                    if (i != 2)
                                                    {
                                                        int positive = 1;
                                                        if(myPosition == 0)
                                                        {
                                                            positive = -1;
                                                        }
                                                        else if(myPosition == 2)
                                                        {
                                                            positive = 1;
                                                        }
                                                        else
                                                        {
                                                            if(tempMonster[i] == 2)
                                                            {
                                                                positive = -1;
                                                            }
                                                            else
                                                            {
                                                                positive = 1;
                                                            }
                                                        }

                                                        int temp = (skillUsePt.X - (stRect.left + 70)) * positive*-1;
                                                        temp = temp / 2;

                                                        skillUsePt.Y = skillUsePt.Y + temp;
                                                    }
                                                    skillUsePt.X = stRect.left + 70;
                                                }
                                                else if (skillUsePt.X > stRect.left +960)
                                                {
                                                    if (i != 2)
                                                    {
                                                        int positive = 1;
                                                        if (myPosition == 0)
                                                        {
                                                            positive = -1;
                                                        }
                                                        else if (myPosition == 2)
                                                        {
                                                            positive = 1;
                                                        }
                                                        else
                                                        {
                                                            if (tempMonster[i] == 2)
                                                            {
                                                                positive = -1;
                                                            }
                                                            else
                                                            {
                                                                positive = 1;
                                                            }
                                                        }

                                                        int temp = (skillUsePt.X - (stRect.left + 960)) * positive;
                                                        temp = temp / 2;

                                                        skillUsePt.Y = skillUsePt.Y + temp;
                                                    }
                                                    skillUsePt.X = stRect.left + 960;
                                                }

                                                if(skillUsePt.Y < stRect.top + 95)
                                                {
                                                    if (i != 2)
                                                    {
                                                        int positive = 1;

                                                        if(myPosition == 1)
                                                        {
                                                            positive = 1;
                                                        }
                                                        else if(myPosition == 3)
                                                        {
                                                            positive = -1;
                                                        }
                                                        else
                                                        {
                                                            if(tempMonster[i] == 1)
                                                            {
                                                                positive = -1;
                                                            }
                                                            else
                                                            {
                                                                positive = 1;
                                                            }
                                                        }


                                                        int temp = (skillUsePt.Y - (stRect.top + 95)) * positive *-1;
                                                        temp = temp * 2;

                                                        skillUsePt.X = skillUsePt.X + temp;
                                                    }
                                                    skillUsePt.Y = stRect.top + 95;
                                                }
                                                else if (skillUsePt.Y > stRect.top + 675)
                                                {
                                                    if (i != 2)
                                                    {
                                                        int positive = 1;

                                                        if (myPosition == 1)
                                                        {
                                                            positive = 1;
                                                        }
                                                        else if (myPosition == 3)
                                                        {
                                                            positive = -1;
                                                        }
                                                        else
                                                        {
                                                            if (tempMonster[i] == 1)
                                                            {
                                                                positive = -1;
                                                            }
                                                            else
                                                            {
                                                                positive = 1;
                                                            }
                                                        }


                                                        int temp = (skillUsePt.Y - (stRect.top + 675)) * positive ;
                                                        temp = temp * 2;

                                                        skillUsePt.X = skillUsePt.X + temp;
                                                    }
                                                    skillUsePt.Y = stRect.top + 675;
                                                }

                                                if (i == 2)
                                                {
                                                    if ((tempMonster[i] % 2 == 0))
                                                    {
                                                        skillUsePt.X = (stRect.left + stRect.right) / 2;
                                                        summonPt.X = (stRect.left + stRect.right) / 2;
                                                    }
                                                    else
                                                    {
                                                        skillUsePt.Y = stRect.top + 365;
                                                        summonPt.Y = stRect.top + 365;
                                                    }
                                                }

                                                if (뇌전주.Checked == false)
                                                {

                                                    skillUsePt.X = (((summonPt.X * foundMob) + (skillUsePt.X * (2 - foundMob)))) / 2;
                                                    skillUsePt.Y = (((summonPt.Y * foundMob) + (skillUsePt.Y * (2 - foundMob)))) / 2;
                                                    foundMob++;
                                                }

                                                mouseMove(summonPt);
                                                사냥(skills[i]);



                                                mouseMove(skillUsePt);
                                                monsterLocationBmp.Dispose();
                                                break;
                                            }


                                            monsterLocationBmp.Dispose();
                                        }

                                    }







                                    tempString = 사냥스킬.Text.ToString();
                                    사냥(tempString);



                                }
                            }   // 좌우 몹 잡기

                            pt.X = (stRect.left + stRect.right) / 2;
                            pt.Y = stRect.top + 365;
                            mouseMove(pt);
                            sendKey("-----");







                            DateTime 퇴각시간;
                            if (monsterTotal == 2)
                            {
                                퇴각시간 = 전투시작시간.Add(new TimeSpan(0, 0, 0, 0, Convert.ToInt32(퇴각시간2부대.Value)));
                            }
                            else
                            {
                                퇴각시간 = 전투시작시간.Add(new TimeSpan(0, 0, 0, 0, Convert.ToInt32(퇴각시간3부대.Value)));
                            }

                            while (true)
                            {
                                Bitmap asdfBmp = getBmp();
                                if (asdfBmp == null)
                                {
                                    if (DateTime.Now >= 퇴각시간)
                                    {
                                        pt.X = stRect.left + 987;
                                        pt.Y = stRect.top + 745;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        pt.X = stRect.left + 515;
                                        pt.Y = stRect.top + 450;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        pt.X = stRect.left + 524;
                                        pt.Y = stRect.top + 441;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        pt.X = stRect.left + 524;
                                        pt.Y = stRect.top + 441;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        Delay(3000);
                                        sendKey("     nnnnn");

                                        break;
                                    }           //퇴각
                                    continue;
                                }
                                // 윈도우가 최소화 되어 있다면 활성화 시킨다
                                ShowWindowAsync(gersangHwnd, 1);

                                // 윈도우에 포커스를 줘서 최상위로 만든다
                                SetForegroundWindow(gersangHwnd);

                                // 윈도우를 0,0 으로 옮긴다.
                                SetWindowPos(gersangHwnd, 0, 1, 0, 0, 0, 0x4 | 0x1 | 0x0040);
                                Bitmap retreatBmp = cropImage(asdfBmp, 사냥중범위);
                                asdfBmp.Dispose();


                                if (DateTime.Now >= 퇴각시간)
                                {
                                    retreatBmp.Dispose();
                                    {
                                        pt.X = stRect.left + 987;
                                        pt.Y = stRect.top + 745;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        pt.X = stRect.left + 515;
                                        pt.Y = stRect.top + 450;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        pt.X = stRect.left + 524;
                                        pt.Y = stRect.top + 441;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        pt.X = stRect.left + 524;
                                        pt.Y = stRect.top + 441;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                        Delay(3000);
                                        sendKey("     nnnnn");
                                    } // 퇴각
                                    break;
                                }
                                else if (searchIMG(retreatBmp, 사냥중그림) < accurate)
                                {
                                    sendKey("     nnnnn");
                                    retreatBmp.Dispose();
                                    break;
                                }
                                else
                                {
                                    사냥(마치고스킬.Text);
                                    retreatBmp.Dispose();
                                }
                            }


                            if ((count > 148) && (checkAlaram == false))
                            {
                                firstBmp.Dispose();
                                동의bmp.Dispose();
                                계속하기bmp.Dispose();
                                사냥중bmp.Dispose();
                                대기중bmp.Dispose();
                                게임가드에러bmp.Dispose();
                                while (true) ;
                            }


                        }                                             // 몬스터 사냥



                    }
                    else if (searchIMG(firstBmp, 거탐그림, out pt) > accurate)
                    {
                        if (!detect)
                        {
                            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 거탐 탐지!!! 빨리 풀어주세요!");       //거탐 탐지 여기가 중요
                        }
                        detect = true;
                        count = 0;


                        if (checkAlaram == true)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                pt.X = stRect.left + 426;
                                pt.Y = stRect.top + 426;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(100);
                                pt.X = stRect.left + 515;
                                pt.Y = stRect.top + 570;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(100);
                                pt.X = stRect.left + 515;
                                pt.Y = stRect.top + 570;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(100);
                                pt.X = stRect.left + 625;
                                pt.Y = stRect.top + 440;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(100);
                                pt.X = stRect.left + 625;
                                pt.Y = stRect.top + 440;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(100);
                            }


                            Bitmap asdfBmp = getBmp();
                            if (asdfBmp == null)
                            {
                                SendMessage(gersangHwnd, 0x0112, 0xF060, 0);

                                gersangHwnd = IntPtr.Zero;



                                Delay(10000);
                                동의bmp.Dispose();
                                계속하기bmp.Dispose();
                                사냥중bmp.Dispose();
                                대기중bmp.Dispose();
                                firstBmp.Dispose();
                                게임가드에러bmp.Dispose();

                                continue;
                            }
                            if (searchIMG(asdfBmp, 거탐그림, out pt) > accurate)
                            {
                                SendMessage(gersangHwnd, 0x0112, 0xF060, 0);

                                gersangHwnd = IntPtr.Zero;



                                Delay(10000);
                            }       // 못 풀었으면 튀기..
                            else
                            {
                                Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + "거탐 뚫렸어요!");       //거탐 탐지 여기가 중요
                            }

                            asdfBmp.Dispose();
                        }
                        else
                        {
                            Delay(80000);
                            SendMessage(gersangHwnd, 0x0112, 0xF060, 0);
                            gersangHwnd = IntPtr.Zero;
                            Delay(10000);
                        }

                    }
                    else if (searchIMG(계속하기bmp, 계속하기그림) > accurate)
                    {
                        pt.X = pt.X + stRect.left + (계속하기범위.left + 계속하기범위.right) / 2;
                        pt.Y = pt.Y + stRect.top + (계속하기범위.top + 계속하기범위.bottom) / 2;

                        mouseMove(pt);
                        mouseLeftClick();
                    }
                    else if (searchIMG(대기중bmp, 대기중그림) > accurate)
                    {
                        Bitmap chatBmp = cropImage(firstBmp, chatRange);
                        Bitmap weightBmp = cropImage(firstBmp, weightRange);

                        detect = false;
                        inbattle = false;
                        Bitmap 의원bmp = cropImage(firstBmp, 의원범위);


                        Color outPixel = firstBmp.GetPixel(128, 687);
                        Color inPixel = firstBmp.GetPixel(128, 697);





                        if (searchIMG(chatBmp, 영자채팅그림, out pt) > accurate)
                        {
                            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 운영자 출몰 주의! 빨리 들어와서 체크 바람");
                            pt.X = stRect.left + 450;
                            pt.Y = stRect.top + 760;
                            mouseMove(pt);
                            mouseLeftClick();
                            firstBmp.Dispose();
                            동의bmp.Dispose();
                            계속하기bmp.Dispose();
                            사냥중bmp.Dispose();
                            대기중bmp.Dispose();

                            chatBmp.Dispose();
                            weightBmp.Dispose();
                            의원bmp.Dispose();
                            게임가드에러bmp.Dispose();


                            this.Invoke(new Action(delegate ()
                            {
                                dlg.매크로종료.PerformClick();
                            }));
                            while (true) ;
                        }
                        if (searchIMG(weightBmp, 무게감지그림, out pt) > accurate)
                        {
                            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 무게가 꽉찼어요!");
                            firstBmp.Dispose();
                            동의bmp.Dispose();
                            계속하기bmp.Dispose();
                            사냥중bmp.Dispose();
                            대기중bmp.Dispose();

                            chatBmp.Dispose();
                            weightBmp.Dispose();
                            의원bmp.Dispose();
                            게임가드에러bmp.Dispose();


                            this.Invoke(new Action(delegate ()
                            {
                                dlg.매크로종료.PerformClick();
                            }));
                            while (true) ;
                        }
                        if (searchIMG(의원bmp, 의원그림) > accurate)
                        {
                            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 캐릭터가 죽었어요!");
                            firstBmp.Dispose();
                            동의bmp.Dispose();
                            계속하기bmp.Dispose();
                            사냥중bmp.Dispose();
                            대기중bmp.Dispose();

                            chatBmp.Dispose();
                            weightBmp.Dispose();
                            의원bmp.Dispose();
                            게임가드에러bmp.Dispose();

                            sendSpecialKey("ENTER");
                            this.Invoke(new Action(delegate ()
                            {
                                dlg.매크로종료.PerformClick();
                            }));
                            while (true) ;
                        }
                        if (searchIMG(firstBmp, 닫기그림, out pt) > accurate)
                        {

                            pt.X = pt.X + stRect.left + 닫기그림.Width / 2;
                            pt.Y = pt.Y + stRect.top + 닫기그림.Height / 2;


                            mouseMove(pt);
                            mouseLeftClick();

                        }           //닫기 버튼 클릭
                        if (searchIMG(firstBmp, 좌판닫기그림, out pt) > accurate)
                        {
                            pt.X = pt.X + stRect.left + 좌판닫기그림.Width / 2;
                            pt.Y = pt.Y + stRect.top + 좌판닫기그림.Height / 2;


                            mouseMove(pt);
                            mouseLeftClick();
                        }


                        if (searchIMG(firstBmp, 취소그림, out pt) > accurate)
                        {

                            pt.X = pt.X + stRect.left + 취소그림.Width / 2;
                            pt.Y = pt.Y + stRect.top + 취소그림.Height / 2;

                            mouseMove(pt);
                            mouseLeftClick();

                        }           //취소 버튼 클릭
                        else if (searchIMG(firstBmp, 배고픔그림, out pt) > accurate)
                        {
                            pt.X = pt.X + stRect.left + 배고픔그림.Width / 2;
                            pt.Y = pt.Y + stRect.top + 배고픔그림.Height / 2;


                            mouseMove(pt);
                            mouseLeftClick();
                        }           //확인 그림 클릭


                        if (outPixel.G > inPixel.G)
                        {
                            bool errorCheck = false;
                            sendKey("11111iiiii");
                            pt.X = pt.X + stRect.left + 배고픔그림.Width / 2;
                            pt.Y = pt.Y + stRect.top + 배고픔그림.Height / 2;


                            mouseMove(pt);
                            DateTime checkTime = DateTime.Now.Add(new TimeSpan(0, 1, 0));       // 1분 동안 체크
                            while (true)
                            {
                                if (DateTime.Now >= checkTime)
                                {
                                    Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 오류 감지(?) 껏다 킬게요.");
                                    SendMessage(gersangHwnd, 0x0112, 0xF060, 0);
                                    gersangHwnd = IntPtr.Zero;
                                    errorCheck = true;
                                    Delay(10000);
                                    break;
                                }
                                Bitmap 삼색채bmp = getBmp();
                                if (삼색채bmp == null)
                                {
                                    continue;
                                }
                                if (searchIMG(삼색채bmp, 좌판닫기그림, out pt) > accurate)
                                {
                                    if (searchIMG(삼색채bmp, 삼색채그림, out pt) > accurate)
                                    {
                                        pt.X = pt.X + stRect.left + 삼색채그림.Width / 2;
                                        pt.Y = pt.Y + stRect.top + 삼색채그림.Height / 2;
                                        mouseMove(pt);
                                        for (int i = 0; i < 7; i++)
                                        {
                                            mouseRightClick();
                                            Delay(200);
                                        }
                                        sendKey("     nnnnn");
                                        삼색채bmp.Dispose();
                                        break;
                                    }
                                    else
                                    {
                                        Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 삼색채 경단이 없어요.");
                                        this.Invoke(new Action(delegate ()
                                        {

                                            dlg.매크로종료.PerformClick();
                                        }));
                                        while (true) ;

                                    }
                                }
                                else
                                {
                                    sendKey("1i");
                                    Delay(10);
                                }
                                삼색채bmp.Dispose();

                            }

                            if (errorCheck == true)
                            {
                                chatBmp.Dispose();
                                weightBmp.Dispose();
                                의원bmp.Dispose();

                                동의bmp.Dispose();
                                계속하기bmp.Dispose();
                                사냥중bmp.Dispose();
                                대기중bmp.Dispose();
                                firstBmp.Dispose();
                                게임가드에러bmp.Dispose();

                                continue;
                            }
                        }                               //포만도


                        Bitmap enemyBmp = getBmp();
                        if (enemyBmp == null)
                        {
                            chatBmp.Dispose();
                            weightBmp.Dispose();
                            의원bmp.Dispose();

                            동의bmp.Dispose();
                            계속하기bmp.Dispose();
                            사냥중bmp.Dispose();
                            대기중bmp.Dispose();
                            firstBmp.Dispose();
                            게임가드에러bmp.Dispose();

                            continue;
                        }


                        for (int i = 1; i < monsterArraySize; i++)
                        {
                            if (searchIMG(enemyBmp, monster[i], out pt) > 0.6)
                            {
                                pt.X = pt.X + stRect.left + monster[i].Width / 2;
                                pt.Y = pt.Y + stRect.top + monster[i].Height / 2;
                                mouseMove(pt);

                                pt.X = pt.X + 3;
                                pt.Y = pt.Y + 3;
                                mouseMove(pt);

                                pt.X = pt.X - 3;
                                pt.Y = pt.Y - 3;
                                mouseMove(pt);


                                mouseRightClick();


                                pt.X = (stRect.left + stRect.right) / 2;
                                pt.Y = (stRect.top + stRect.bottom) / 2;
                                mouseMove(pt);
                                break;
                            }
                        }
                        enemyBmp.Dispose();
                        chatBmp.Dispose();
                        weightBmp.Dispose();
                        의원bmp.Dispose();
                    }
                    else if (searchIMG(동의bmp, 동의그림) > accurate)
                    {
                        pt.X = (stRect.left + stRect.right) / 2;
                        pt.Y = (stRect.top + stRect.bottom) / 2;
                        mouseMove(pt);
                        mouseLeftClick();
                        pt.X = 10;
                        pt.Y = 10;
                        mouseMove(pt);
                        sendSpecialKey("ENTER");
                        Delay(500);
                        while (true)
                        {
                            if ((gersangHwnd == IntPtr.Zero) || (!IsWindow(gersangHwnd)))
                            {
                                break;
                            }

                            Bitmap passwordBmp = getBmp();
                            if (passwordBmp == null)
                            {
                                continue;
                            }
                            // 윈도우가 최소화 되어 있다면 활성화 시킨다
                            ShowWindowAsync(gersangHwnd, 1);

                            // 윈도우에 포커스를 줘서 최상위로 만든다
                            SetForegroundWindow(gersangHwnd);

                            // 윈도우를 0,0 으로 옮긴다.
                            SetWindowPos(gersangHwnd, 0, 1, 0, 0, 0, 0x4 | 0x1 | 0x0040);
                            if (error_screen())
                            {
                                errorCheck();
                                SendMessage(gersangHwnd, 0x0112, 0xF060, 0);
                                gersangHwnd = IntPtr.Zero;
                                Delay(10000);
                            }

                            if (searchIMG(passwordBmp, 로그인그림, out pt) > accurate)
                            {
                                if (checkAlaram == false)
                                {
                                    Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 대기열을 뚫었습니다.");
                                    Delay(550000);
                                    sendSpecialKey("ESC");
                                    Delay(1000);
                                }
                                else
                                {
                                    passwordBmp.Dispose();
                                    Delay(1000);
                                    sendSpecialKey("ENTER");
                                    pt.X = pt.X + stRect.left;
                                    pt.Y = pt.Y + stRect.top;
                                    mouseMove(pt);
                                    mouseLeftClick();
                                    Delay(1000);
                                    break;
                                }
                            }           // 캐릭터 창
                            else if (searchIMG(passwordBmp, 비밀번호그림, out pt) > accurate)
                            {
                                pt.X = stRect.left + 463;
                                pt.Y = stRect.top + 525;
                                mouseMove(pt);
                                mouseLeftClick();
                                Delay(2000);
                                pt.X = 10;
                                pt.Y = 10;
                                mouseMove(pt);
                                Delay(10);

                                Bitmap tempBmp = getBmp();
                                if (tempBmp == null)
                                {
                                    sendSpecialKey("ESC");
                                    sendSpecialKey("ESC");
                                    continue;
                                }
                                Bitmap number;
                                string temp = 비밀번호2차.Text.ToString();

                                this.Invoke(new Action(delegate ()
                                {
                                    dlg.Location = new System.Drawing.Point(0, 800);
                                    dlg.Size = new System.Drawing.Size(500, 100);
                                }));


                                if (temp.Length != 4)
                                {


                                    Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 2차 비밀번호가 이상합니다.");
                                    this.Invoke(new Action(delegate ()
                                    {

                                        dlg.Location = new System.Drawing.Point(30, 30);
                                        dlg.Size = new System.Drawing.Size(stRect.right - stRect.left - 50, stRect.bottom - stRect.top - 50);
                                    }));

                                    동의bmp.Dispose();
                                    계속하기bmp.Dispose();
                                    사냥중bmp.Dispose();
                                    대기중bmp.Dispose();
                                    firstBmp.Dispose();
                                    tempBmp.Dispose();
                                    passwordBmp.Dispose();
                                    게임가드에러bmp.Dispose();

                                    while (true) ;
                                }               //2차 비밀번호가 4개가 아닐경우
                                for (int i = 0; i < 4; i++)
                                {
                                    number = new Bitmap(".\\img\\number\\" + temp[i] + ".png");
                                    if (searchIMG(tempBmp, number, out pt) > 0)
                                    {
                                        pt.X = pt.X + stRect.left + number.Width / 2;
                                        pt.Y = pt.Y + stRect.top + number.Height / 2;
                                        mouseMove(pt);
                                        mouseLeftClick();
                                    }
                                    number.Dispose();
                                }       // 4글자 클릭
                                number = new Bitmap(".\\img\\number\\ok.png");
                                if (searchIMG(tempBmp, number, out pt) > 0)
                                {
                                    pt.X = pt.X + stRect.left + number.Width / 2;
                                    pt.Y = pt.Y + stRect.top + number.Height / 2;
                                    mouseMove(pt);
                                    mouseLeftClick();
                                }               // ok 클릭
                                number.Dispose();
                                tempBmp.Dispose();
                                Delay(100);
                                sendSpecialKey("ESC");
                                sendSpecialKey("ESC");
                                Delay(800);

                                pt.X = stRect.left + 628;
                                pt.Y = stRect.top + 637;
                                mouseMove(pt);
                                mouseLeftClick();
                            }
                            else if (searchIMG(passwordBmp, 접속그림, out pt) > accurate)
                            {
                                sendSpecialKey("ENTER");
                            }
                            else if (searchIMG(passwordBmp, 동의그림) > accurate)
                            {
                                pt.X = (stRect.left + stRect.right) / 2;
                                pt.Y = (stRect.top + stRect.bottom) / 2;
                                mouseMove(pt);
                                mouseLeftClick();
                                pt.X = 10;
                                pt.Y = 10;
                                mouseMove(pt);
                                sendSpecialKey("ENTER");
                            }


                            passwordBmp.Dispose();
                        }

                    }               // 동의 창(게임 실행 하자마자)
                    else if (searchIMG(게임가드에러bmp, 게임가드에러그림, out pt) > accurate)
                    {
                        SendMessage(gersangHwnd, 0x0112, 0xF060, 0);

                        gersangHwnd = IntPtr.Zero;



                        Delay(10000);
                    }



                    동의bmp.Dispose();
                    계속하기bmp.Dispose();
                    사냥중bmp.Dispose();
                    대기중bmp.Dispose();
                    firstBmp.Dispose();
                    게임가드에러bmp.Dispose();
                    Delay(10);

                }
            }
            catch (ThreadInterruptedException)
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
                    반자사_Thread.Abort();
                    반자사_Thread = null;
                }
                catch { }

            }
            catch (ThreadAbortException)
            {
                Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 매크로 중단 또는 종료");
            }
            catch (Exception e)
            {

                Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 에러 발생. 다시 실행해주세요.");
                MessageBox.Show("" + e.StackTrace);
            }
        }


        public Form1()
        {
            InitializeComponent();

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Hide();

            init();

            webBrowser1.Navigate("http://www.gersang.co.kr/main.gs");

            ready();


        }

        private void 매크로시작_click(object sender, EventArgs e)
        {
            HD = webBrowser1.Document;

            if (t_id.Text.ToString().Equals(""))
            {
                MessageBox.Show("ID를 입력해주세요.");

                return;
            }
            if (t_pass.Text.ToString().Equals(""))
            {
                MessageBox.Show("비밀번호를 입력해주세요.");

                return;
            }
            if (comboBox1.Text.ToString().Equals(""))
            {
                MessageBox.Show("사냥할 몬스터를 선택해주세요.");

                return;
            }

            매크로시작.Enabled = false;

            if (!login)
            {
                HD.GetElementById("GSuserID").InnerText = t_id.Text;
                HD.GetElementById("GSuserPW").InnerText = t_pass.Text;
                HD.GetElementById("frmLogin").InvokeMember("Submit");

                Delay(1000);


                ready();

                if (webBrowser1.Url.ToString().Equals("https://www.gersang.co.kr/pub/logi/login/otp.gs"))
                {
                    HD.GetElementById("GSotpNo").InnerText = t_code.Text;
                    HD.GetElementById("otpform").InvokeMember("Submit");
                    otp = true;

                }
                else if (webBrowser1.Url.ToString().Equals("http://www.gersang.co.kr/main.gs?"))
                {
                    otp = false;
                }
                else if (webBrowser1.Url.ToString().Equals("https://www.gersang.co.kr/pub/logi/login/loginProc.gs"))
                {
                    Debug.WriteLine("로그인 에러");
                    MessageBox.Show("로그인 에러");
                    return;

                    // 로그인 에러
                }
                ready();
                Delay(3000);

            }               // 로그인이 안 되어 있을 경우


            bool succeess = false;

            if (gersangHwnd == IntPtr.Zero)
            {
                HD.InvokeScript("gameStart", new object[] { "1" });
                ready();


                IntPtr tempHwnd = IntPtr.Zero;

                for (int i = 0; i < 60; i++)
                {
                    tempHwnd = FindWindow(null, GameName);

                    if (tempHwnd != IntPtr.Zero)
                    {
                        gersangHwnd = tempHwnd;
                        login = true;

                        t_id.Enabled = false;
                        t_pass.Enabled = false;
                        t_code.Enabled = false;
                        비밀번호2차.Enabled = false;


                        succeess = true;
                        break;
                    }

                    Debug.WriteLine("" + i);
                    Delay(1000);
                }
            }       // 거상이 없을 경우

            if (!succeess)              // 텔레그램 문자 보내기
            {
                Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 오류 발생. 거상을 확인해주세요.");
                return;
            }


            반자사_Thread = new Thread(new ThreadStart(반자사));
            반자사_Thread.Start();



            dlg.Show();
            dlg.Location = new System.Drawing.Point(0, 800);
            dlg.Size = new System.Drawing.Size(500, 100);
        }




        #region 시간 바꿔야 되는 함수

        public bool error_screen()
        {
            if (lastImage == null)
            {
                lastImage = getBmp();
                return false;
            }
            if (DateTime.Now >= lastImageTime)
            {
                Bitmap tempBmp = getBmp();
                if (tempBmp == null)
                {
                    return false;
                }


                lastImageTime = DateTime.Now.Add(new TimeSpan(0, 0, 60));

                if (searchIMG(lastImage, tempBmp, out _) > 0.99999)
                {
                    Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정에 화면이 안 바뀌어요! 거상을 확인해주세요.");
                    lastImage.Dispose();
                    lastImage = null;
                    return true;
                }
                else
                {
                    lastImage.Dispose();
                    lastImage = tempBmp;
                }
            }
            return false;

        }

        public bool errorCheck()
        {
            POINT pt = new POINT();
            pt.X = 1000;
            pt.Y = 612;
            mouseMove(pt);
            sendSpecialKey("LBC");
            sendSpecialKey("LBC");
            sendSpecialKey("ENTER");
            sendSpecialKey("ENTER");

            IntPtr findwindow = FindWindow(null, "Gersang_x86");
            if (findwindow != IntPtr.Zero)
            {
                SendMessage(findwindow, 0x0112, 0xF060, 0);
                return true;
            }
            else
            {
                return false;
            }


        }

        void mouseRightClick()
        {
            sendSpecialKey("RBD");
            sendSpecialKey("RBU");

        }

        void mouseLeftClick()
        {
            sendSpecialKey("LBD");
            sendSpecialKey("LBU");
        }

        #endregion

        #region 완료된 함수



        public void threadInterrupt(int no)
        {
            try
            {
                if (no == -1)
                {
                    반자사_Thread.Abort();
                    반자사_Thread = null;

                    MessageBox.Show("종료되었습니다.");
                }
                else
                {
                    원격종료();
                    if (반자사_Thread == null)
                    {
                        반자사_Thread = new Thread(new ThreadStart(반자사));
                        반자사_Thread.Start();
                    }
                    else
                    {
                        반자사_Thread.Start();
                    }
                }
            }
            catch { }


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

            {
                System.IO.StreamReader file = new System.IO.StreamReader(@".\\사냥스킬설정.txt");
                시작스킬.Text = file.ReadLine();
                맵가운데.Text = file.ReadLine();
                왼쪽스킬.Text = file.ReadLine();
                가운데스킬.Text = file.ReadLine();
                오른쪽스킬.Text = file.ReadLine();
                사냥스킬.Text = file.ReadLine();
                마치고스킬.Text = file.ReadLine();
                퇴각시간2부대.Value = Convert.ToDecimal(file.ReadLine());
                퇴각시간3부대.Value = Convert.ToDecimal(file.ReadLine());
                t_id.Text = file.ReadLine();
                AES tempCrypt = new AES();
                String temp = "";
                temp = file.ReadLine();
                if (!temp.Equals(""))
                {
                    t_pass.Text = tempCrypt.AESDecrypt256(temp, "01053764603010360646030103533460");
                }

                temp = file.ReadLine();
                if (!temp.Equals(""))
                {
                    비밀번호2차.Text = tempCrypt.AESDecrypt256(temp, "01053764603010360646030103533460");
                }

                try
                {
                    temp = file.ReadLine();
                    전체딜레이.Value = Convert.ToDecimal(temp);
                    delayRate = Convert.ToInt32(전체딜레이.Value);
                    sendSetDelay(temp);
                    좌우소환거리.Value = Convert.ToDecimal(file.ReadLine());
                    정면소환3시.Value = Convert.ToDecimal(file.ReadLine());
                    정면소환6시.Value = Convert.ToDecimal(file.ReadLine());
                    좌우스킬거리.Value = Convert.ToDecimal(file.ReadLine());
                    정면스킬3시.Value = Convert.ToDecimal(file.ReadLine());
                    정면스킬6시.Value = Convert.ToDecimal(file.ReadLine());
                    뇌전주.Checked = file.ReadLine().Equals("1") ? true :  false;
                }
                catch
                {
                    MessageBox.Show("설정이 제대로 안되어 있어서 딜레이를 14로 합니다. 설정을 하고 저장버튼을 눌러주세요.");
                    sendSetDelay("" + delayRate);
                }

                this.Text = "딜레이 : " + delayRate + ". 호갱버전!";

            }               //사냥스킬 정보랑 계정 읽기

            {
                System.IO.StreamReader file = new System.IO.StreamReader(@".\\텔레그램설정.txt");
                String temp = file.ReadLine();
                telegram_chat_id = file.ReadLine();

                Bot = new Telegram.Bot.TelegramBotClient(temp);
                Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 매크로 프로그램이 켜졌습니다.");
            }


            {
                동의그림 = File.ReadAllBytes(".\\img\\byte\\동의.bin");
                로그인그림 = getFindBmp("로그인.png");
                사냥중그림 = File.ReadAllBytes(".\\img\\byte\\사냥중.bin");
                접속그림 = getFindBmp("접속.png");
                거탐그림 = getFindBmp("거탐.png");
                닫기그림 = getFindBmp("닫기.png");
                취소그림 = getFindBmp("취소.png");
                대기중그림 = File.ReadAllBytes(".\\img\\byte\\대기중.bin");
                계속하기그림 = File.ReadAllBytes(".\\img\\byte\\계속하기.bin");
                비밀번호그림 = getFindBmp("비밀번호.png");
                영자채팅그림 = getFindBmp("영자채팅.png");

                의원그림 = File.ReadAllBytes(".\\img\\byte\\의원.bin");

                게임가드에러그림 = getFindBmp("게임가드에러.png");
                좌판닫기그림 = getFindBmp("좌판닫기.png");
                배고픔그림 = getFindBmp("배고픔.png");
                삼색채그림 = getFindBmp("삼색채.png");
                무게감지그림 = getFindBmp("무게감지.png");

            }
            {

                {
                    chatRange.left = 255;
                    chatRange.top = 680;
                    chatRange.right = 310;
                    chatRange.bottom = 750;
                }           //채팅 범위

                {
                    weightRange.left = 220;
                    weightRange.top = 55;
                    weightRange.right = 315;
                    weightRange.bottom = 215;
                }           //무게 범위

                {


                }

                {
                    계속하기범위.left = 461;
                    계속하기범위.top = 227;
                    계속하기범위.right = 570;
                    계속하기범위.bottom = 254;
                }

                {
                    사냥중범위.left = 979;
                    사냥중범위.top = 711;
                    사냥중범위.right = 999;
                    사냥중범위.bottom = 733;
                }


                {
                    대기중범위.left = 895;
                    대기중범위.top = 695;
                    대기중범위.right = 907;
                    대기중범위.bottom = 720;
                }

                {
                    의원범위.left = 503;
                    의원범위.top = 146;
                    의원범위.right = 526;
                    의원범위.bottom = 158;
                }

                {
                    동의범위.left = 471;
                    동의범위.top = 544;
                    동의범위.right = 494;
                    동의범위.bottom = 557;
                }

                {
                    게임가드에러범위.left = 596;
                    게임가드에러범위.top = 425;
                    게임가드에러범위.right = 663;
                    게임가드에러범위.bottom = 454;
                }


                {
                    for (int i = 0; i < f11Range.Length; i++)
                    {
                        f11Range[i] = new RECT();
                    }

                    {
                        f11Range[0].left = 1;
                        f11Range[0].top = 1;
                        f11Range[0].right = 1;
                        f11Range[0].bottom = 1;
                    }


                }


            }
            {

                positionStart[0].X = 117;
                positionStart[0].Y = 710;

                positionStart[1].X = 171;
                positionStart[1].Y = 739;

                positionStart[2].X = 114;
                positionStart[2].Y = 734;

                positionStart[3].X = 61;
                positionStart[3].Y = 737;
            }
            {


                {
                    for (int i = 0; i < 4; i++)
                    {
                        skillPt[i] = new POINT[3];
                    }

                    skillPt[0][0].X = 136;          //[x][y]  x: (x*3) 시 방향 , y : 0 좌측, 1 우측 , 2 중앙
                    skillPt[0][0].Y = 565;
                    skillPt[0][1].X = 954;
                    skillPt[0][1].Y = 565;
                    skillPt[0][2].X = 518;
                    skillPt[0][2].Y = 636;


                    skillPt[1][0].X = 115;
                    skillPt[1][0].Y = 603;
                    skillPt[1][1].X = 115;
                    skillPt[1][1].Y = 136;
                    skillPt[1][2].X = 81;
                    skillPt[1][2].Y = 395;


                    skillPt[2][0].X = 138;
                    skillPt[2][0].Y = 220;
                    skillPt[2][1].X = 927;
                    skillPt[2][1].Y = 220;
                    skillPt[2][2].X = 515;
                    skillPt[2][2].Y = 68;


                    skillPt[3][0].X = 860;
                    skillPt[3][0].Y = 160;
                    skillPt[3][1].X = 860;
                    skillPt[3][1].Y = 620;
                    skillPt[3][2].X = 933;
                    skillPt[3][2].Y = 395;

                }           // 스킬 위치

                {
                    for (int i = 0; i < monsterLocationRange.Length; i++)
                    {
                        monsterLocationRange[i] = new RECT();
                    }

                    {
                        monsterLocationRange[0].left = 240;
                        monsterLocationRange[0].top = 95;
                        monsterLocationRange[0].right = 960;
                        monsterLocationRange[0].bottom = 240;

                        monsterLocationRange[1].left = 715;
                        monsterLocationRange[1].top = 160;
                        monsterLocationRange[1].right = 960;
                        monsterLocationRange[1].bottom = 520;

                        monsterLocationRange[2].left = 360;
                        monsterLocationRange[2].top = 420;
                        monsterLocationRange[2].right = 960;
                        monsterLocationRange[2].bottom = 675;

                        monsterLocationRange[3].left = 145;
                        monsterLocationRange[3].top = 160;
                        monsterLocationRange[3].right = 340;
                        monsterLocationRange[3].bottom = 520;



                        //monsterLocationRange[0].left = 240;
                        //monsterLocationRange[0].top = 80;
                        //monsterLocationRange[0].right = 885;
                        //monsterLocationRange[0].bottom = 125;

                        //monsterLocationRange[1].left = 895;
                        //monsterLocationRange[1].top = 165;
                        //monsterLocationRange[1].right = 990;
                        //monsterLocationRange[1].bottom = 580;

                        //monsterLocationRange[2].left = 175;
                        //monsterLocationRange[2].top = 590;
                        //monsterLocationRange[2].right = 900;
                        //monsterLocationRange[2].bottom = 645;

                        //monsterLocationRange[3].left = 50;
                        //monsterLocationRange[3].top = 175;
                        //monsterLocationRange[3].right = 145;
                        //monsterLocationRange[3].bottom = 620;
                    }
                }           // 몬스터 범위 초기화
                {
                    mapMovePt[0].X = 500;
                    mapMovePt[0].Y = 30;
                    mapMovePt[1].X = 1017;
                    mapMovePt[1].Y = 356;
                    mapMovePt[2].X = 506;
                    mapMovePt[2].Y = 764;
                    mapMovePt[3].X = 5;
                    mapMovePt[3].Y = 335;

                }
            }

            dlg = new Form2();
            dlg.FormSendEvent += new Form2.FormSendDataHandler(threadInterrupt);

            {
                DirectoryInfo info = new DirectoryInfo(".\\img\\monster");

                if (info.Exists)
                {
                    DirectoryInfo[] Cinfo = info.GetDirectories();

                    foreach (DirectoryInfo t in Cinfo)
                    {
                        comboBox1.Items.Add(t.Name);
                    }
                }
            }


            model = OpenCvSharp.ImgHash.PHash.Create();
        }               // 프로그램 시작될 때 초기화


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

        void ready()
        {
            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            for (int j = 0; j != -1; j++)
            {
                if (this.webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                {
                    System.Threading.Thread.Sleep(1);
                    break;
                }
            }
        }                   // 웹브라우저 기다리기

        public void mouseTest()
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


            Debug.WriteLine(mouseXratio + "," + mouseYratio);


        }       // 마우스 감도 설정


        public Bitmap getFindBmp(String path)
        {
            Bitmap findBmp;
            findBmp = new Bitmap(".\\img\\" + path);
            return findBmp;

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
            try
            {


                if (findwindow != IntPtr.Zero)
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

            }
            catch (Exception e)
            {
                

            }


            return null;
        }


        public double searchIMG(Bitmap screen_img, Byte[] find_img)
        {
            try
            {
                Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat(screen_img);
                MatOfByte hash = new MatOfByte();
                model.Compute(img, hash);
                byte[] hashArray = hash.ToArray();



                return pHashsimilarity(hashArray, find_img);
            }
            catch
            {
                return 0.0;
            }
        }

        public double searchIMG(Bitmap screen_img, Bitmap find_img, out POINT pt)
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
                    Debug.WriteLine("찾은 이미지의 유사도 : " + maxval);

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

        public void sendKey(string s)
        {
            arduSerial.Write(s);
            Delay(s.Length * delayRate * 2);
        }

        private double pHashsimilarity(Byte[] a, Byte[] b)
        {


            System.Collections.BitArray aBit = new System.Collections.BitArray(a);
            System.Collections.BitArray bBit = new System.Collections.BitArray(b);

            System.Collections.BitArray temp = aBit.Xor(bBit);

            int sum = 0;

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i])
                {
                    sum++;
                }
            }


            return (1.0 - (double)sum / (double)temp.Length);



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

            Delay(10);

            return;

        }

        public void sendSetDelay(string s)
        {
            arduSerial.Write("$" + s + "+");
            Delay(2);
        }

        public void sendSpecialKey(string s)
        {
            Debug.WriteLine("키 " + s);
            arduSerial.Write("@" + s + "+");
            Delay(delayRate * 2);
        }       //ESC -> ESC ; 

        public void send예약시전(string s, int iterator)
        {
            arduSerial.Write("#" + s + "," + iterator + ".+");
            Delay(4 * iterator * delayRate);
        }
        public bool CheckingSpecialText(string txt)
        {
            string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
            return rex.IsMatch(txt);
        }   // 특수문자 체크





        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {


            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process p in mProcess)
                p.Kill();
        }
        #endregion



        private void 비밀번호2차_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }





        public bool 분홍색찾기(Bitmap img, int myPosition, int enemyPosition, out POINT skillUsePt, out POINT summonPt)
        {
            skillUsePt.X = 0;
            skillUsePt.Y = 0;
            summonPt.X = 0;
            summonPt.Y = 0;
            POINT tempPt;
            tempPt.X = 0;
            tempPt.Y = 0;
            bool xReverse = false;

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


            int xIncrease = -1;
            int yIncrease = -1;

            POINT startPt = new POINT();
            POINT endPt = new POINT();



            {
                if (myPosition == 0)
                {
                    if (enemyPosition == 1)
                    {
                        xReverse = true;
                        startPt.X = 0;
                        startPt.Y = 0;

                        summonPt.X = -2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = -1* Convert.ToInt32(좌우소환거리.Value);
                        tempPt.X = -2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = -1 * Convert.ToInt32(좌우스킬거리.Value);

                    }

                    else if (enemyPosition == 3)
                    {
                        xReverse = true;
                        startPt.X = bmpWidth;
                        startPt.Y = 0;

                        summonPt.X = 2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = -1* Convert.ToInt32(좌우소환거리.Value);

                        tempPt.X = 2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = -1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else
                    {
                        startPt.X = 0;
                        startPt.Y = 0;

                        summonPt.Y = -1 * Convert.ToInt32(정면소환6시.Value);
                        tempPt.Y = -1 * Convert.ToInt32(정면스킬6시.Value);
                    }

                }
                else if (myPosition == 1)
                {
                    xReverse = true;
                    if (enemyPosition == 2)
                    {
                        startPt.X = bmpWidth;
                        startPt.Y = 0;

                        summonPt.X = 2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = -1* Convert.ToInt32(좌우소환거리.Value);

                        tempPt.X = 2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = -1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else if (enemyPosition == 0)
                    {
                        startPt.X = bmpWidth;
                        startPt.Y = bmpHeight;


                        skillUsePt.Y = monster[0].Height;

                        summonPt.X = 2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = 1* Convert.ToInt32(좌우소환거리.Value);

                        tempPt.X = 2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = 1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else
                    {
                        startPt.X = bmpWidth;
                        startPt.Y = 0;

                        summonPt.X = Convert.ToInt32(정면소환3시.Value);
                        tempPt.X =  Convert.ToInt32(정면스킬3시.Value);

                    }
                }
                else if (myPosition == 2)
                {
                    if (enemyPosition == 3)
                    {
                        xReverse = true;
                        startPt.X = bmpWidth;
                        startPt.Y = bmpHeight;


                        skillUsePt.Y = monster[0].Height;

                        summonPt.X = 2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = 1* Convert.ToInt32(좌우소환거리.Value);

                        tempPt.X = 2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = 1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else if (enemyPosition == 1)
                    {
                        xReverse = true;
                        startPt.X = 0;
                        startPt.Y = bmpHeight;


                        skillUsePt.Y = monster[0].Height;

                        summonPt.X = -2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = 1* Convert.ToInt32(좌우소환거리.Value);

                        tempPt.X = -2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = 1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else
                    {
                        startPt.X = 0;
                        startPt.Y = bmpHeight;


                        skillUsePt.Y = monster[0].Height;
                        summonPt.Y = 1 * Convert.ToInt32(정면소환6시.Value);
                        tempPt.Y = 1 * Convert.ToInt32(정면스킬6시.Value);
                    }
                }
                else
                {
                    xReverse = true;
                    if (enemyPosition == 0)
                    {
                        startPt.X = 0;
                        startPt.Y = bmpHeight;


                        skillUsePt.Y = monster[0].Height;

                        summonPt.X = -2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = 1* Convert.ToInt32(좌우소환거리.Value);
                        tempPt.X = -2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = 1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else if (enemyPosition == 2)
                    {
                        startPt.X = 0;
                        startPt.Y = 0;

                        summonPt.X = -2* Convert.ToInt32(좌우소환거리.Value);
                        summonPt.Y = -1* Convert.ToInt32(좌우소환거리.Value);

                        tempPt.X = -2 * Convert.ToInt32(좌우스킬거리.Value);
                        tempPt.Y = -1 * Convert.ToInt32(좌우스킬거리.Value);
                    }
                    else
                    {
                        startPt.X = 0;
                        startPt.Y = 0;

                        summonPt.X = -1 * Convert.ToInt32(정면소환3시.Value);
                        tempPt.X = -1 *Convert.ToInt32(정면스킬3시.Value);
                    }
                }


                if (startPt.X == 0)
                {
                    endPt.X = bmpWidth;
                    xIncrease = 1;


                }
                else
                {
                    endPt.X = 0;
                    xIncrease = -1;

                }

                if (startPt.Y == 0)
                {
                    endPt.Y = bmpHeight;
                    yIncrease = 1;

                }
                else
                {
                    endPt.Y = 0;
                    yIncrease = -1;

                }
            }   //초기화..

            int numBytes = 0;



            if (xReverse == false)
            {
                for (int y = startPt.Y + yIncrease; y * yIncrease < endPt.Y; y += yIncrease)
                {
                    for (int x = startPt.X + xIncrease; x * xIncrease < endPt.X; x += xIncrease)
                    {
                        numBytes = (y * (bmpWidth * 3)) + (x * 3);
                        if ((rgbValues[numBytes] == 26) && (rgbValues[numBytes + 2] == 158))        //BGR로 처리되는구나..
                        {
                            if ((rgbValues[numBytes + 1] == 64))
                            {
                                skillUsePt.Y += (numBytes / (bmpWidth * 3));
                                skillUsePt.X += ((numBytes % (bmpWidth * 3)) / 3);

                                summonPt.X = summonPt.X + skillUsePt.X;
                                summonPt.Y = summonPt.Y + skillUsePt.Y;

                                skillUsePt.X += tempPt.X;
                                skillUsePt.Y += tempPt.Y;

                                return true;
                            }
                        }

                    }
                }
            }
            else
            {
                for (int x = startPt.X + xIncrease; x * xIncrease < endPt.X; x += xIncrease)
                {
                    for (int y = startPt.Y + yIncrease; y * yIncrease < endPt.Y; y += yIncrease)
                    {
                        numBytes = (y * (bmpWidth * 3)) + (x * 3);
                        if ((rgbValues[numBytes] == 26) && (rgbValues[numBytes + 2] == 158))        //BGR로 처리되는구나..
                        {
                            if ((rgbValues[numBytes + 1] == 64))
                            {
                                skillUsePt.Y += (numBytes / (bmpWidth * 3));
                                skillUsePt.X += ((numBytes % (bmpWidth * 3)) / 3);

                                summonPt.X = summonPt.X + skillUsePt.X;
                                summonPt.Y = summonPt.Y + skillUsePt.Y;

                                skillUsePt.X += tempPt.X;
                                skillUsePt.Y += tempPt.Y;
                                return true;
                            }
                        }

                    }
                }
            }







            return false;
        }
        private void 로그인버튼_Click(object sender, EventArgs e)
        {
            HD = webBrowser1.Document;

            if (t_id.Text.ToString().Equals(""))
            {
                MessageBox.Show("ID를 입력해주세요.");

                return;
            }
            if (t_pass.Text.ToString().Equals(""))
            {
                MessageBox.Show("비밀번호를 입력해주세요.");

                return;
            }
            if (comboBox1.Text.ToString().Equals(""))
            {
                MessageBox.Show("사냥할 몬스터를 선택해주세요.");

                return;
            }


            HD.GetElementById("GSuserID").InnerText = t_id.Text;
            HD.GetElementById("GSuserPW").InnerText = t_pass.Text;
            HD.GetElementById("frmLogin").InvokeMember("Submit");

            Delay(1000);


            ready();

            if (webBrowser1.Url.ToString().Equals("https://www.gersang.co.kr/pub/logi/login/otp.gs"))
            {
                HD.GetElementById("GSotpNo").InnerText = t_code.Text;
                HD.GetElementById("otpform").InvokeMember("Submit");
                otp = true;

            }       //otp 입력
            else if (webBrowser1.Url.ToString().Equals("http://www.gersang.co.kr/main.gs?"))
            {
                otp = false;
            }
            else if (webBrowser1.Url.ToString().Equals("https://www.gersang.co.kr/pub/logi/login/loginProc.gs"))
            {
                Debug.WriteLine("로그인 에러");
                MessageBox.Show("로그인 에러");
                return;

                // 로그인 에러
            }
            ready();

            로그인버튼.Enabled = false;
            login = true;
        }

        private void 거상찾기_Click(object sender, EventArgs e)
        {
            IntPtr tempPtr;
            tempPtr = FindWindow(null, GameName);

            if (tempPtr == IntPtr.Zero)
            {
                MessageBox.Show("못 찾았습니다.");
                return;
            }

            gersangHwnd = tempPtr;
        }



        public void 원격종료()
        {
            IntPtr wow64backup = IntPtr.Zero;
            if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
            {
                NativeMethods.Wow64DisableWow64FsRedirection(ref wow64backup);
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
                NativeMethods.Wow64RevertWow64FsRedirection(wow64backup);
            }
        }




        private void 매크로시작_Click_1(object sender, EventArgs e)
        {
            if (!login)
            {
                MessageBox.Show("로그인을 해주세요.");
                return;
            }

            if (gersangHwnd == IntPtr.Zero)
            {
                HD.InvokeScript("gameStart", new object[] { "1" });
                ready();


                IntPtr tempHwnd = IntPtr.Zero;

                bool succeess = false;

                for (int i = 0; i < 60; i++)
                {
                    tempHwnd = FindWindow(null, GameName);

                    if (tempHwnd != IntPtr.Zero)
                    {
                        gersangHwnd = tempHwnd;
                        login = true;

                        t_id.Enabled = false;
                        t_pass.Enabled = false;
                        t_code.Enabled = false;
                        비밀번호2차.Enabled = false;


                        succeess = true;
                        break;
                    }

                    Debug.WriteLine("" + i);
                    Delay(1000);
                }

                if (!succeess)              // 텔레그램 문자 보내기
                {
                    Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 오류 발생. 거상을 확인해주세요.");
                    return;
                }
            }

            반자사_Thread = new Thread(new ThreadStart(반자사));
            반자사_Thread.Start();



            dlg.Show();
            dlg.Location = new System.Drawing.Point(0, 800);
            dlg.Size = new System.Drawing.Size(500, 100);

            매크로시작.Enabled = false;
        }






        public void 몬스터위치()
        {

            for (int i = 0; i < 4; i++)
            {
                monsterPosition[i] = true;
            }

            if (monsterTotal == 3)
            {
                return;
            }
            Delay(300);

            DateTime lastTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, 4000));

            Bitmap bmp;
            while (true)
            {
                bmp = getBmp();
                if (bmp != null)
                {
                    break;
                }
                if (lastTime >= DateTime.Now)
                {
                    return;
                }
            }

            for (int k = 0; k < 4; k++)
            {
                int count = 0;
                if (k == 2)
                {
                    continue;
                }
                for (int i = 0; i < 22; i += 2)
                {
                    for (int j = 0; j < 14; j += 2)
                    {
                        if (bmp.GetPixel(positionStart[k].X + i, positionStart[k].Y + j).GetHue() > 0)
                        {
                            count++;
                        }
                    }
                }
                if (count < 3)
                {
                    monsterPosition[k] = false;
                    bmp.Dispose();
                    return;
                }
            }

            monsterPosition[2] = false;
            bmp.Dispose();
            return;
        }
        public bool 탭여부()
        {
            Bitmap kkkk = getBmp();
            if (kkkk == null)
            {
                return true;
            }
            int count = 0;

            Color[] pixel = new Color[2];
            pixel[0] = kkkk.GetPixel(55, 743);
            pixel[1] = kkkk.GetPixel(203, 743);
            kkkk.Dispose();
            for (int i = 0; i < 2; i++)
            {
                if (pixel[i].GetHue() == 0)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return false;   //안 눌러져 있음
            }
            else
            {
                return true; // 눌러져있음
            }



        }

        public int 내위치()
        {
            Bitmap tempBmp = getBmp();
            if (tempBmp == null)
            {
                return -1;
            }

            Color topPixel = tempBmp.GetPixel(516, 60);
            Color bottomPixel = tempBmp.GetPixel(516, 666);
            Color leftPixel = tempBmp.GetPixel(7, 352);
            Color rightPixel = tempBmp.GetPixel(1010, 352);

            tempBmp.Dispose();

            if (topPixel.GetHue() != 0)         //6시
            {
                return 2;
            }
            else if (leftPixel.GetHue() != 0)       // 3시
            {
                return 1;
            }
            else if (rightPixel.GetHue() != 0)  //9시
            {

                return 3;
            }
            else                    //12시
            {
                return 0;
            }
        }

        public void 사냥(String skill)
        {
            for (int i = 0; i < skill.Length; i++)
            {
                if (('0' <= skill[i]) && (skill[i] <= '9'))
                {
                    sendKey("" + skill[i]);
                }
                else
                {
                    int temp = i;
                    int count = 1;
                    while (true)
                    {
                        temp++;
                        if (temp > skill.Length - 1)
                        {
                            break;
                        }
                        if (skill[i] != skill[temp])
                        {
                            break;
                        }
                        count++;
                    }

                    send예약시전("" + skill[i], count);
                    i = temp - 1;


                }
            }
            sendKey("=-`");
        }

        public bool f11판별()
        {
            Bitmap temp = getBmp();
            if (temp == null)
            {
                return false;
            }
            int count = 0;

            Color[][] pixel = new Color[3][];

            pixel[0] = new Color[10];
            pixel[1] = new Color[10];
            pixel[2] = new Color[10];

            pixel[0][0] = temp.GetPixel(38, 272);
            pixel[0][1] = temp.GetPixel(70, 262);
            pixel[0][2] = temp.GetPixel(55, 267);
            pixel[0][3] = temp.GetPixel(14, 271);
            pixel[0][4] = temp.GetPixel(32, 262);
            pixel[0][5] = temp.GetPixel(16, 264);
            pixel[0][6] = temp.GetPixel(94, 267);
            pixel[0][7] = temp.GetPixel(120, 271);
            pixel[0][8] = temp.GetPixel(20, 262);
            pixel[0][9] = temp.GetPixel(22, 262);

            pixel[1][0] = temp.GetPixel(38, 308);
            pixel[1][1] = temp.GetPixel(70, 298);
            pixel[1][2] = temp.GetPixel(55, 303);
            pixel[1][3] = temp.GetPixel(14, 307);
            pixel[1][4] = temp.GetPixel(32, 307);
            pixel[1][5] = temp.GetPixel(16, 300);
            pixel[1][6] = temp.GetPixel(94, 303);
            pixel[1][7] = temp.GetPixel(120, 307);
            pixel[1][8] = temp.GetPixel(20, 298);
            pixel[1][9] = temp.GetPixel(22, 298);

            pixel[2][0] = temp.GetPixel(38, 290);
            pixel[2][1] = temp.GetPixel(70, 280);
            pixel[2][2] = temp.GetPixel(55, 285);
            pixel[2][3] = temp.GetPixel(14, 289);
            pixel[2][4] = temp.GetPixel(32, 289);
            pixel[2][5] = temp.GetPixel(16, 282);
            pixel[2][6] = temp.GetPixel(94, 285);
            pixel[2][7] = temp.GetPixel(120, 289);
            pixel[2][8] = temp.GetPixel(20, 280);
            pixel[2][9] = temp.GetPixel(22, 280);

            Color bgc = temp.GetPixel(53, 303);
            
            for (int i = 0; i < 3; i++)
            {
                if ( (pixel[i][0].GetHue() > 1) && (pixel[i][0].GetBrightness() == pixel[i][1].GetBrightness()) && (pixel[i][0].GetBrightness() == pixel[i][2].GetBrightness()) && (pixel[i][0].GetBrightness() != pixel[i][3].GetBrightness()) && (pixel[i][0].GetBrightness() != pixel[i][4].GetBrightness()) && (pixel[i][0].GetBrightness() != bgc.GetBrightness()) && (pixel[i][0].GetBrightness() == pixel[i][5].GetBrightness()) && (pixel[i][0].GetBrightness() == pixel[i][6].GetBrightness()) && (pixel[i][0].GetBrightness() == pixel[i][7].GetBrightness()) && (pixel[i][0].GetBrightness() == pixel[i][8].GetBrightness()) && (pixel[i][0].GetBrightness() == pixel[i][9].GetBrightness()))
                {
                    count++;
                }
            }

            monsterTotal = count + 1;

            temp.Dispose();
            return true;

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

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (monster != null)
            {
                for (int i = 0; i < monster.Length; i++)
                {
                    monster[i].Dispose();
                    monster[i] = null;
                }
                monster = null;
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }


            string[] arr = Directory.GetFiles(".\\img\\monster\\" + comboBox1.Text, "*.png");
            monsterArraySize = arr.Length;
            Array.Sort(arr, StringComparer.Ordinal);

            monster = new Bitmap[monsterArraySize];

            for (int i = 0; i < monsterArraySize; i++)
            {
                monster[i] = new Bitmap(arr[i]);
            }


        }

        private void 시작스킬_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || char.IsLetter(e.KeyChar)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }


        private void 스킬저장_Click(object sender, EventArgs e)
        {

            AES tempCrypt = new AES();
            String passWord = tempCrypt.AESEncrypt256(t_pass.Text, "01053764603010360646030103533460");
            String passWord2 = tempCrypt.AESEncrypt256(비밀번호2차.Text, "01053764603010360646030103533460");
            String 뇌전주확인 = 뇌전주.Checked ? "1" : "0";

            String saveData = 시작스킬.Text + "\r\n" + 맵가운데.Text + "\r\n" + 왼쪽스킬.Text + "\r\n" + 가운데스킬.Text + "\r\n" + 오른쪽스킬.Text + "\r\n" + 사냥스킬.Text + "\r\n" + 마치고스킬.Text + "\r\n" + 퇴각시간2부대.Value.ToString() + "\r\n" + 퇴각시간3부대.Value.ToString() + "\r\n" + t_id.Text + "\r\n" + passWord + "\r\n" + passWord2 + "\r\n" + 전체딜레이.Value.ToString() + "\r\n" + 좌우소환거리.Value.ToString() +"\r\n" + 정면소환3시.Value.ToString() + "\r\n" + 정면소환6시.Value.ToString() + "\r\n" + 좌우스킬거리.Value.ToString() + "\r\n" + 정면스킬3시.Value.ToString() + "\r\n" + 정면스킬6시.Value.ToString() + "\r\n"+ 뇌전주확인+"\r\n";

            System.IO.File.WriteAllText(@".\사냥스킬설정.txt", saveData, Encoding.Default);
            sendSetDelay(전체딜레이.Value.ToString());
            delayRate = Convert.ToInt32(전체딜레이.Value);
            this.Text = "딜레이 : " + delayRate + ". 호갱버전!";
        }

        private void Check알림_CheckedChanged(object sender, EventArgs e)
        {
            checkAlaram = check알림.Checked;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat(getBmp());
            MatOfByte hash = new MatOfByte();
            model.Compute(img, hash);
            byte[] hashArray = hash.ToArray();

        }





        //public void 옛버전()
        //{
        //    if (!detect)
        //    {
        //        Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + " 계정 거탐 탐지!!! 빨리 풀어주세요!");       //거탐 탐지 여기가 중요
        //    }
        //    detect = true;
        //    count = 0;


        //    if (checkAlaram == true)
        //    {
        //        POINT temp = new POINT();
        //        temp.X = 80;
        //        temp.Y = 228;

        //        for (int asdf = 0; asdf < 2; asdf++)
        //        {
        //            for (int j = temp.Y; j < 645; j += 43)
        //            {
        //                for (int i = temp.X; i < 630; i += 60)
        //                {
        //                    pt.X = stRect.left + i;
        //                    pt.Y = stRect.top + j;
        //                    mouseMove(pt);
        //                    {
        //                        arduSerial.Write("@LBD+");
        //                        arduSerial.Write("@LBU+");
        //                        Delay(5);
        //                    }

        //                }
        //            }
        //        }

        //        Bitmap asdfBmp = getBmp();
        //        if (asdfBmp == null)
        //        {
        //            SendMessage(gersangHwnd, 0x0112, 0xF060, 0);

        //            gersangHwnd = IntPtr.Zero;



        //            Delay(10000);
        //            동의bmp.Dispose();
        //            계속하기bmp.Dispose();
        //            사냥중bmp.Dispose();
        //            대기중bmp.Dispose();
        //            firstBmp.Dispose();
        //            게임가드에러bmp.Dispose();

        //            continue;
        //        }
        //        Bitmap confirmBmp = cropImage(asdfBmp, 대기중범위);
        //        asdfBmp.Dispose();
        //        if (searchIMG(confirmBmp, 대기중그림, out pt) < accurate)
        //        {
        //            SendMessage(gersangHwnd, 0x0112, 0xF060, 0);

        //            gersangHwnd = IntPtr.Zero;



        //            Delay(10000);
        //        }       // 못 풀었으면 튀기..
        //        else
        //        {
        //            Bot.SendTextMessageAsync(telegram_chat_id, t_id.Text.ToString() + "거탐 뚫렸어요!");       //거탐 탐지 여기가 중요
        //        }

        //        confirmBmp.Dispose();
        //    }
        //    else
        //    {
        //        Delay(80000);
        //        SendMessage(gersangHwnd, 0x0112, 0xF060, 0);
        //        gersangHwnd = IntPtr.Zero;
        //        Delay(10000);
        //    }
        //}

    }
}
