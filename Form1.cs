using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace 얍
{
    public partial class Form1 : Form
    {
        #region 전역 변수
        yapHeader header;

        private Thread 제자리_Thread;
        private Thread 림보_Thread;
        IntPtr gamePtr;
        제자리스킬설정폼 form_제자리스킬설정;
        림보설정 form_림보설정;
        Color hpColor = new Color();
        Color mpColor = new Color();
        Color spColor = new Color();

        yapHeader.POINT hpPt = new yapHeader.POINT();
        yapHeader.POINT mpPt = new yapHeader.POINT();
        yapHeader.POINT spPt = new yapHeader.POINT();

        delegate Bitmap getBmpDelegate(IntPtr temp);
        public static string iniPath;
        #endregion

        #region 코드에서 변경해야 할 수치들


        public static string[] specialCharacter = new string[] { "기타", "티치엘" };
        enum HOTKEY : int { F1, F1CTRL, F7, F2, HOME, END, F5, PAGEDOWN, PAGEUP };

        internal static class Constants
        {
            public const int stand_buffNumber = 12;
            public const int stand_skillNumber = 3;

        }

        class 버프구조체
        {
            public DateTime buffTime = DateTime.Now;
            public string buffKey = "None";
            public int buffCool = int.MaxValue;
            public int buffDelay = int.MaxValue;
            public bool buffItems = false;
            public bool buffRightClick = false;

            public 버프구조체(DateTime buffTime, string buffKey, int buffCool, int buffDelay, bool buffItems, bool buffRightClick)
            {
                this.buffTime = buffTime;
                this.buffKey = buffKey;
                this.buffCool = buffCool;
                this.buffDelay = buffDelay;
                this.buffItems = buffItems;
                this.buffRightClick = buffRightClick;
            }
        }





        #endregion

        public Form1()
        {
            InitializeComponent();
            init();
            yapHeader.RegisterHotKey(this.Handle, (int)HOTKEY.PAGEDOWN, 0, Keys.PageDown.GetHashCode());
            yapHeader.RegisterHotKey(this.Handle, (int)HOTKEY.PAGEUP, 0, Keys.PageUp.GetHashCode());


        }


        private void init()
        {
            iniPath = System.Windows.Forms.Application.StartupPath + "\\Setting.ini";

            header = new yapHeader();
            header.setDelay("14");
            MessageBox.Show("마우스를 움직이지 마세요. 감도 체크 합니다.\n제어판에서 마우스 가속도(정확도 향상)을 꺼주세요.");
            header.mouseTest();
            MessageBox.Show("테스트 결과는 X 좌표 비율 : " + header.getMouseXratio() + ", Y 좌표 비율 : " + header.getMouseYratio() + "\n(마우스 비율 1은 제어판에서 마우스 속도 6칸 기준입니다.)");
            form_제자리스킬설정 = new 제자리스킬설정폼();
            form_림보설정 = new 림보설정();

            {
                hpPt.X = 138;
                hpPt.Y = 48;
                mpPt.X = 138;
                mpPt.Y = 64;
                spPt.X = 138;
                spPt.Y = 79;
            }           //hp, mp, sp 위치 설정


            loadProfileSettings();
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case (int)HOTKEY.PAGEDOWN:
                        {
                            try
                            {
                                제자리_Thread.Abort();
                                제자리_Thread = null;
                            }
                            catch { }

                            try
                            {
                                림보_Thread.Abort();
                                림보_Thread = null;
                            }
                            catch { }

                            break;
                        }


                    case (int)HOTKEY.PAGEUP:
                        {
                            IntPtr tempPtr = header.getHwndName("Talesweaver");
                            
                            yapHeader.POINT tempPt = new yapHeader.POINT();
                            yapHeader.GetCursorPos(out tempPt);


                            if (tempPtr == IntPtr.Zero)
                            {
                                MessageBox.Show("게임 실행 X");
                                return;
                            }

                            yapHeader.RECT clientRect = default(yapHeader.RECT);
                            yapHeader.GetWindowRect((int)tempPtr, ref clientRect);


                            Debug.WriteLine("현재 마우스는 " + (tempPt.X - clientRect.left) + "," + (tempPt.Y - clientRect.top));

                            break;
                        }

                }

            }
        }       //핫키
        private void loadProfileSettings()
        {

            StringBuilder sb = new StringBuilder();
            {
                yapHeader.GetPrivateProfileString("제자리스킬설정", "최소딜레이", null, sb, 1024, iniPath);
                form_제자리스킬설정.numericUpDown최소딜레이.Value = decimal.Parse(sb.ToString());
                yapHeader.GetPrivateProfileString("제자리스킬설정", "최대딜레이", null, sb, 1024, iniPath);
                form_제자리스킬설정.numericUpDown최대딜레이.Value = decimal.Parse(sb.ToString());

                for (int i = 0; i < Constants.stand_skillNumber; i++)
                {
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "스킬키" + (i + 1), null, sb, 1024, iniPath);
                    ((form_제자리스킬설정.Controls.Find("buttonSkill" + (i + 1), true)[0])).Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "스킬딜레이" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_제자리스킬설정.Controls.Find("numericUpDownSkillDelay" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "스킬사용여부" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxSkillUse" + (i + 1), true)[0])).Checked = true;
                    }
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "스킬우측클릭" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxSkillRightClick" + (i + 1), true)[0])).Checked = true;
                    }
                }

                /*
                 * 
                 *                             ↑ 스킬 설정        
                 *                             
                 *                             ↓ 버프 설정
                 * 
                 */

                for (int i = 0; i < Constants.stand_buffNumber; i++)
                {
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "버프키" + (i + 1), null, sb, 1024, iniPath);
                    ((form_제자리스킬설정.Controls.Find("buttonBuff" + (i + 1), true)[0])).Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "버프재사용시간" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_제자리스킬설정.Controls.Find("numericUpDownBuffCool" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("제자리스킬설정", "버프딜레이" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_제자리스킬설정.Controls.Find("numericUpDownBuffDelay" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());

                    yapHeader.GetPrivateProfileString("제자리스킬설정", "버프아이템" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxBuff" + (i + 1), true)[0])).Checked = true;
                    }

                    yapHeader.GetPrivateProfileString("제자리스킬설정", "버프사용여부" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxUse" + (i + 1), true)[0])).Checked = true;
                    }

                    yapHeader.GetPrivateProfileString("제자리스킬설정", "버프우측클릭" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxBuffRightClick" + (i + 1), true)[0])).Checked = true;
                    }
                }



            }           // 제자리 스킬 설정 파일 불러오기
            {
                yapHeader.GetPrivateProfileString("림보설정", "최소딜레이", null, sb, 1024, iniPath);
                form_림보설정.numericUpDown최소딜레이.Value = decimal.Parse(sb.ToString());
                yapHeader.GetPrivateProfileString("림보설정", "최대딜레이", null, sb, 1024, iniPath);
                form_림보설정.numericUpDown최대딜레이.Value = decimal.Parse(sb.ToString());

                for (int i = 0; i < Constants.stand_skillNumber; i++)
                {
                    yapHeader.GetPrivateProfileString("림보설정", "스킬키" + (i + 1), null, sb, 1024, iniPath);
                    ((form_림보설정.Controls.Find("buttonSkill" + (i + 1), true)[0])).Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("림보설정", "스킬딜레이" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownSkillDelay" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("림보설정", "스킬반복횟수" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownSkillRepeat" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());




                    yapHeader.GetPrivateProfileString("림보설정", "스킬사용여부" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_림보설정.Controls.Find("checkBoxSkillUse" + (i + 1), true)[0])).Checked = true;
                    }
                    yapHeader.GetPrivateProfileString("림보설정", "스킬우측클릭" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_림보설정.Controls.Find("checkBoxSkillRightClick" + (i + 1), true)[0])).Checked = true;
                    }
                }


                /*
                 * 
                 *                             ↑ 스킬 설정        
                 *                             
                 *                             ↓ 버프 설정
                 * 
                 */

                for (int i = 0; i < Constants.stand_buffNumber; i++)
                {
                    yapHeader.GetPrivateProfileString("림보설정", "버프키" + (i + 1), null, sb, 1024, iniPath);
                    ((form_림보설정.Controls.Find("buttonBuff" + (i + 1), true)[0])).Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("림보설정", "버프재사용시간" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownBuffCool" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("림보설정", "버프딜레이" + (i + 1), null, sb, 1024, iniPath);
                    ((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownBuffDelay" + (i + 1), true)[0])).Value = decimal.Parse(sb.ToString());

                    yapHeader.GetPrivateProfileString("림보설정", "버프아이템" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_림보설정.Controls.Find("checkBoxBuff" + (i + 1), true)[0])).Checked = true;
                    }

                    yapHeader.GetPrivateProfileString("림보설정", "버프사용여부" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_림보설정.Controls.Find("checkBoxUse" + (i + 1), true)[0])).Checked = true;
                    }

                    yapHeader.GetPrivateProfileString("림보설정", "버프우측클릭" + (i + 1), null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        ((CheckBox)(form_림보설정.Controls.Find("checkBoxBuffRightClick" + (i + 1), true)[0])).Checked = true;
                    }
                }



                /*
                 * 
                 * 
                 *                          그 외 설정 ↓
                 * 
                 * 
                 * 
                 */

                {
                    yapHeader.GetPrivateProfileString("림보설정", "림보캐릭터", null, sb, 1024, iniPath);
                    form_림보설정.buttonCharacterTeleport.Text = sb.ToString();
                    ((RadioButton)(form_림보설정.Controls.Find("radioButtonCharacter" + sb.ToString(), true)[0])).Checked = true;


                    yapHeader.GetPrivateProfileString("림보설정", "티치엘텔포", null, sb, 1024, iniPath);
                    form_림보설정.buttonCharacterTeleport.Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("림보설정", "티치엘텔포딜레이", null, sb, 1024, iniPath);
                    form_림보설정.numericUpDownCharacterTeleportDelay.Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("림보설정", "움직임딜레이", null, sb, 1024, iniPath);
                    form_림보설정.numericUpDown움직임딜레이.Value = decimal.Parse(sb.ToString());

                }               // 특수 캐릭 설정


                {
                    yapHeader.GetPrivateProfileString("물약설정", "HP키", null, sb, 1024, iniPath);
                    this.buttonHP.Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("물약설정", "MP키", null, sb, 1024, iniPath);
                    this.buttonMP.Text = sb.ToString();
                    yapHeader.GetPrivateProfileString("물약설정", "SP키", null, sb, 1024, iniPath);
                    this.buttonSP.Text = sb.ToString();


                    yapHeader.GetPrivateProfileString("물약설정", "HP딜레이", null, sb, 1024, iniPath);
                    this.numericUpDownHPDelay.Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("물약설정", "MP딜레이", null, sb, 1024, iniPath);
                    this.numericUpDownMPDelay.Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("물약설정", "SP딜레이", null, sb, 1024, iniPath);
                    this.numericUpDownSPDelay.Value = decimal.Parse(sb.ToString());
                    yapHeader.GetPrivateProfileString("물약설정", "HP우측클릭", null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        checkBoxHPRightClick.Checked = true;
                    }
                    yapHeader.GetPrivateProfileString("물약설정", "MP우측클릭", null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        checkBoxMPRightClick.Checked = true;
                    }
                    yapHeader.GetPrivateProfileString("물약설정", "SP우측클릭", null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        checkBoxSPRightClick.Checked = true;
                    }

                    yapHeader.GetPrivateProfileString("물약설정", "HP사용", null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        checkBoxHPUse.Checked = true;
                    }
                    yapHeader.GetPrivateProfileString("물약설정", "MP사용", null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        checkBoxMPUse.Checked = true;
                    }
                    yapHeader.GetPrivateProfileString("물약설정", "SP사용", null, sb, 1024, iniPath);
                    if (sb.ToString().Equals("true"))
                    {
                        checkBoxSPUse.Checked = true;
                    }







                }           // 물약 설정





            }           // 림보 스킬 설정 파일 불러오기


        }

       private string 림보좌표검색(Bitmap tempBmp)
        {
            
            int digX = 5;
            int digY = 10;
            string tempString = "";



            yapHeader.POINT[] points = new yapHeader.POINT[6];
            for(int i = 0; i < points.Length; i++)
            {
                points[i] = new yapHeader.POINT();
                points[i].Y = 513;
            }

            {
                points[0].X = 660;
                points[1].X = 666;
                points[2].X = 672;

                points[3].X = 684;
                points[4].X = 690;
                points[5].X = 696;
            }

            Bitmap[] digits = new Bitmap[6];
            for(int i = 3; i < digits.Length; i++)
            {
                yapHeader.RECT tempRect = new yapHeader.RECT();
                tempRect.left = points[i].X;
                tempRect.top = points[i].Y;
                tempRect.right = points[i].X + digX;
                tempRect.bottom = points[i].Y + digY;


                digits[i] = header.cropImage(tempBmp, tempRect);
                tempString = tempString + 이미지to좌표(digits[i]);
            }
            Debug.WriteLine("좌표 Y는 :" + tempString);
            return tempString;

        }

        private string 이미지to좌표(Bitmap img)
        {



            Color numBytes1 = img.GetPixel(1, 1);
            Color numBytes2 = img.GetPixel(0, 9);
            Color numBytes3 = img.GetPixel(1, 3);
            Color numBytes4 = img.GetPixel(0, 0);
            Color numBytes5 = img.GetPixel(4, 3);
            Color numBytes6 = img.GetPixel(1, 4);
            Color numBytes7 = img.GetPixel(2, 5);
            Color numBytes8 = img.GetPixel(2, 4);

            if ((numBytes1.R > 200) && (numBytes1.G > 200) && (numBytes1.B > 200) )
            {
                Color subNumBytes = img.GetPixel(2, 1);
                if ((subNumBytes.R > 200) && (subNumBytes.G > 200) && (subNumBytes.B > 200))
                {
                    Color subNumBytes2 = img.GetPixel(2, 2);
                    if ((subNumBytes2.R > 200) && (subNumBytes2.G > 200) && (subNumBytes2.B > 200) )
                    {
                        Color subNumBytes3 = img.GetPixel(2, 3);
                        if ((subNumBytes3.R > 200) && (subNumBytes3.G > 200) && (subNumBytes3.B > 200))
                        {
                            return "1";
                        }
                    }
                }
            }           // 1 판단
            else if ((numBytes2.R > 200) && (numBytes2.G > 200) && (numBytes2.B > 200) )
            {
                Color subNumBytes = img.GetPixel(4, 9);
                if ((subNumBytes.R > 200) && (subNumBytes.G > 200) && (subNumBytes.B > 200))
                {
                    Color subNumBytes2 = img.GetPixel(1, 6);
                    if ((subNumBytes2.R > 200) && (subNumBytes2.G > 200) && (subNumBytes2.B > 200))
                    {
                        return "2";
                    }
                }
            }
            else if ((numBytes3.R > 200) && (numBytes3.G > 200) && (numBytes3.B > 200) )
            {
                Color subNumBytes = img.GetPixel(1, 7);
                if ((subNumBytes.R > 200) && (subNumBytes.G > 200) && (subNumBytes.B > 200))
                {
                    Color subNumBytes2 = img.GetPixel(3, 7);
                    if ((subNumBytes2.R > 200) && (subNumBytes2.G > 200) && (subNumBytes2.B > 200))
                    {
                        return "4";
                    }
                }
            }
            else if ((numBytes4.R > 200) && (numBytes4.G > 200) && (numBytes4.B > 200))
            {
                Color subNumBytes = img.GetPixel(4, 0);
                if ((subNumBytes.R > 200) && (subNumBytes.G > 200) && (subNumBytes.B > 200))
                {
                    Color subNumBytes2 = img.GetPixel(1, 4);
                    if ((subNumBytes2.R > 200) && (subNumBytes2.G > 200) && (subNumBytes2.B > 200))
                    {
                        Color subNumBytes3 = img.GetPixel(0, 6);
                        if (!((subNumBytes3.R > 200) && (subNumBytes3.G > 200) && (subNumBytes3.B > 200)))
                        {
                            return "5";
                        }
                    }           //5
                    else
                    {
                        Color subNumBytes3 = img.GetPixel(2, 6);
                        if ((subNumBytes3.R > 200) && (subNumBytes3.G > 200) && (subNumBytes3.B > 200))
                        {
                            return "7";
                        }
                    }
                }
            }
            else if (!((numBytes5.R > 200) && (numBytes5.G > 200) && (numBytes5.B > 200) ))
            {
                return "6";
            }
            else if ((numBytes6.R > 200) && (numBytes6.G > 200) && (numBytes6.B > 200) )
            {
                return "8";
            }
            else if ((numBytes7.R > 200) && (numBytes7.G > 200) && (numBytes7.B > 200) )
            {
                return "9";
            }
            else if ((numBytes8.R > 200) && (numBytes8.G > 200) && (numBytes8.B > 200) )
            {
                return "3";
            }
            return "0";


        }

        private int getNumbytes(int x,int y,int bmpWidth)
        {
            return (y * (bmpWidth * 3)) + (x * 3);
        }

        private Bitmap 똥컴용getBmp(IntPtr findwindow)
        {
            Bitmap temp = header.fullscreenCapture();
            yapHeader.RECT clientRect = default(yapHeader.RECT);
            yapHeader.GetWindowRect((int)gamePtr, ref clientRect);
            return header.cropImage(temp, clientRect);

        }

        private void 림보반복()
        {
            yapHeader.POINT tempPt = new yapHeader.POINT();
            yapHeader.RECT clientRect = default(yapHeader.RECT);
            yapHeader.GetWindowRect((int)gamePtr, ref clientRect);
            getBmpDelegate tempDelegate;
            if (checkBox똥컴.Checked)
            {
                tempDelegate = new getBmpDelegate(똥컴용getBmp);
            }
            else
            {
                tempDelegate = new getBmpDelegate(header.getBmp);
            }

            // 윈도우에 포커스를 줘서 최상위로 만든다
            yapHeader.SetForegroundWindow(gamePtr);

            yapHeader.POINT hpPt = new yapHeader.POINT();
            yapHeader.POINT mpPt = new yapHeader.POINT();
            yapHeader.POINT spPt = new yapHeader.POINT();
            hpPt.X = 138;
            hpPt.Y = 48;
            mpPt.X = 138;
            mpPt.Y = 64;
            spPt.X = 138;
            spPt.Y = 79;

            int randomMin = Convert.ToInt32(form_림보설정.numericUpDown최소딜레이.Value);
            int randomMax = Convert.ToInt32(form_림보설정.numericUpDown최대딜레이.Value);

            DateTime lastItemUseDelay = DateTime.Now;
            DateTime lastSkillUseDelay = DateTime.Now;

            List<버프구조체> buffs = new List<버프구조체>();              // 버프 목록


            List<int> waitPortions = new List<int>();
            List<int> waitSkills = new List<int>();
            List<int> waitItems = new List<int>();

            HashSet<string> waitCheck_hash = new HashSet<string>();

            bool 포션사용여부 = false;
            bool bool_hp포션 = false;
            bool bool_mp포션 = false;
            bool bool_sp포션 = false;

            bool bool_moveCheck = false;

            int lastSkillRepeatCount = 0;
            int startPosition;

            {
                if (checkBoxHPUse.Checked || checkBoxMPUse.Checked || checkBoxSPUse.Checked)
                {
                    포션사용여부 = true;
                }                           // 포션 사용 여부 확인

                if (포션사용여부)
                {
                    if ((checkBoxHPUse.Checked) && (!buttonHP.Text.Equals("None")))
                    {
                        bool_hp포션 = true;
                    }
                    if ((checkBoxMPUse.Checked) && (!buttonMP.Text.Equals("None")))
                    {
                        bool_mp포션 = true;
                    }
                    if ((checkBoxSPUse.Checked) && (!buttonSP.Text.Equals("None")))
                    {
                        bool_sp포션 = true;
                    }
                }
                
                buffs.Add(new 버프구조체(DateTime.Now, buttonHP.Text, -1000, Convert.ToInt32(numericUpDownHPDelay.Value), true, checkBoxHPRightClick.Checked));          //hp포션
                buffs.Add(new 버프구조체(DateTime.Now, buttonMP.Text, -1000, Convert.ToInt32(numericUpDownMPDelay.Value), true, checkBoxMPRightClick.Checked));
                buffs.Add(new 버프구조체(DateTime.Now, buttonSP.Text, -1000, Convert.ToInt32(numericUpDownSPDelay.Value), true, checkBoxSPRightClick.Checked));
            }           //포션 설정

            {
                for (int i = 0; i < Form1.Constants.stand_buffNumber; i++)
                {
                    if ((!((form_림보설정.Controls.Find("buttonBuff" + (i + 1), true)[0])).Text.Equals("None")) && (((CheckBox)(form_림보설정.Controls.Find("checkBoxUse" + (i + 1), true)[0])).Checked == true))
                    {
                        buffs.Add(new 버프구조체(
                            DateTime.Now,
                            form_림보설정.Controls.Find("buttonBuff" + (i + 1), true)[0].Text,
                            Convert.ToInt32(((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownBuffCool" + (i + 1), true)[0])).Value),
                            Convert.ToInt32(((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownBuffDelay" + (i + 1), true)[0])).Value),
                            ((CheckBox)(form_림보설정.Controls.Find("checkBoxBuff" + (i + 1), true)[0])).Checked,
                            ((CheckBox)(form_림보설정.Controls.Find("checkBoxBuffRightClick" + (i + 1), true)[0])).Checked
                            ));
                    }
                }

            }               // 버프 설정

            {
                for (int i = 0; i < Form1.Constants.stand_skillNumber; i++)
                {
                    if ((!((form_림보설정.Controls.Find("buttonSkill" + (i + 1), true)[0])).Text.Equals("None")) && (((CheckBox)(form_림보설정.Controls.Find("checkBoxSkillUse" + (i + 1), true)[0])).Checked == true))
                    {
                        buffs.Add(new 버프구조체(
                            new DateTime(1),
                            (form_림보설정.Controls.Find("buttonSkill" + (i + 1), true)[0]).Text,
                            Convert.ToInt32(((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownSkillRepeat" + (i + 1), true)[0])).Value),
                            Convert.ToInt32(((NumericUpDown)(form_림보설정.Controls.Find("numericUpDownSkillDelay" + (i + 1), true)[0])).Value),
                            false,
                            ((CheckBox)(form_림보설정.Controls.Find("checkBoxSkillRightClick" + (i + 1), true)[0])).Checked
                            ));
                    }
                }


            }           // 스킬 설정

            while (true)
            {
                Bitmap tempBmp = tempDelegate(gamePtr);
                if (tempBmp == null)
                {
                    continue;
                }
                startPosition = Int32.Parse(림보좌표검색(tempBmp));
                
                if (hpColor.IsEmpty)
                {
                    hpColor = tempBmp.GetPixel(hpPt.X, hpPt.Y);
                    mpColor = tempBmp.GetPixel(mpPt.X, mpPt.Y);
                    spColor = tempBmp.GetPixel(spPt.X, spPt.Y);
                }
                tempBmp.Dispose();
                break;
            }


            while (true)
            {
                if ((gamePtr == IntPtr.Zero) || (!yapHeader.IsWindow(gamePtr)))
                {
                    MessageBox.Show("에러 발생.");

                    return;
                }
                header.mouseMove(clientRect.left + 643, clientRect.top + 482);              // 마우스를 캐릭터 쪽으로

                if (포션사용여부)
                {
                    Bitmap tempBmp = tempDelegate(gamePtr);
                    if (tempBmp == null)
                    {
                        continue;
                    }

                    if (bool_sp포션)
                    {
                        Color tempColor = tempBmp.GetPixel(spPt.X, spPt.Y);
                        if (tempColor.ToArgb() != spColor.ToArgb())
                        {
                            if (waitCheck_hash.Add(buttonSP.Text))             // 대기 리스트에 존재하지 않을 때 실행
                            {
                                waitPortions.Add(2);
                            }
                        }
                    }
                    if (bool_mp포션)
                    {
                        Color tempColor = tempBmp.GetPixel(mpPt.X, mpPt.Y);
                        if (tempColor.ToArgb() != mpColor.ToArgb())
                        {
                            if (waitCheck_hash.Add(buttonMP.Text))             // 포션이 이미 리스트에 존재하지 않을 때 실행
                            {
                                waitPortions.Add(1);
                            }
                        }
                    }
                    if (bool_hp포션)
                    {
                        Color tempColor = tempBmp.GetPixel(hpPt.X, hpPt.Y);
                        if (tempColor.ToArgb() != hpColor.ToArgb())
                        {
                            if (waitCheck_hash.Add(buttonHP.Text))             // 포션이 이미 리스트에 존재하지 않을 때 실행
                            {
                                waitPortions.Insert(0, 0);
                            }


                        }
                    }


                    tempBmp.Dispose();
                }               //포션은 계속 추가될 여지를 줌

                if(waitCheck_hash.Count > 0)
                {
                    if(DateTime.Now >= lastItemUseDelay)
                    {
                        if(waitPortions.Count > 0)
                        {
                            if (buffs[waitPortions[0]].buffRightClick)
                            {
                                if (buffs[waitPortions[0]].buffKey.Length > 1)
                                {
                                    header.randomTimeSpecialKey(buffs[waitPortions[0]].buffKey, randomMin, randomMax);
                                }
                                else
                                {
                                    header.randomTimeKey(buffs[waitPortions[0]].buffKey, randomMin, randomMax);
                                }
                                buffs[waitPortions[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitPortions[0]].buffCool);

                                lastItemUseDelay = DateTime.Now.AddMilliseconds(buffs[waitPortions[0]].buffDelay);
                                header.randomTimeMouseRightClick(randomMin, randomMax);
                                waitCheck_hash.Remove(buffs[waitPortions[0]].buffKey);
                                waitPortions.RemoveAt(0);
                            }
                            else
                            {
                                buffs[waitPortions[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitPortions[0]].buffCool);
                                lastItemUseDelay = DateTime.Now.AddMilliseconds(buffs[waitPortions[0]].buffDelay);

                                if (buffs[waitPortions[0]].buffKey.Length > 1)
                                {
                                    header.randomTimeSpecialKey(buffs[waitPortions[0]].buffKey, randomMin, randomMax);
                                }
                                else
                                {
                                    header.randomTimeKey(buffs[waitPortions[0]].buffKey, randomMin, randomMax);
                                }
                                waitCheck_hash.Remove(buffs[waitPortions[0]].buffKey);
                                waitPortions.RemoveAt(0);
                            }
                        }
                        else if (waitItems.Count > 0)
                        {
                            if (buffs[waitItems[0]].buffRightClick)
                            {
                                if (buffs[waitItems[0]].buffKey.Length > 1)
                                {
                                    header.randomTimeSpecialKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                                }
                                else
                                {
                                    header.randomTimeKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                                }
                                buffs[waitItems[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffCool);

                                lastItemUseDelay = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffDelay);
                                header.randomTimeMouseRightClick(randomMin, randomMax);
                                waitCheck_hash.Remove(buffs[waitItems[0]].buffKey);
                                waitItems.RemoveAt(0);
                            }
                            else
                            {
                                buffs[waitItems[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffCool);
                                lastItemUseDelay = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffDelay);

                                if (buffs[waitItems[0]].buffKey.Length > 1)
                                {
                                    header.randomTimeSpecialKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                                }
                                else
                                {
                                    header.randomTimeKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                                }
                                waitCheck_hash.Remove(buffs[waitItems[0]].buffKey);
                                waitItems.RemoveAt(0);
                            }
                        }
                    }
                    
                    if(DateTime.Now >= lastSkillUseDelay)
                    {
                        if(waitSkills.Count > 0)
                        {
                            if(DateTime.Compare(buffs[waitSkills[0]].buffTime,new DateTime(1)) == 0)
                            {
                                if (buffs[waitSkills[0]].buffRightClick)
                                {
                                    if (buffs[waitSkills[0]].buffKey.Length > 1)
                                    {
                                        header.randomTimeSpecialKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }
                                    else
                                    {
                                        header.randomTimeKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }

                                    lastSkillUseDelay = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffDelay);
                                    header.randomTimeMouseRightClick(randomMin, randomMax);
                                    if (lastSkillRepeatCount < buffs[waitSkills[0]].buffCool -1 )
                                    {
                                        lastSkillRepeatCount++;
                                    }
                                    else
                                    {
                                        lastSkillRepeatCount = 0;
                                        waitCheck_hash.Remove(buffs[waitSkills[0]].buffKey);
                                        waitSkills.RemoveAt(0);
                                    }
                                }
                                else
                                {
                                    lastSkillUseDelay = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffDelay);

                                    if (buffs[waitSkills[0]].buffKey.Length > 1)
                                    {
                                        header.randomTimeSpecialKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }
                                    else
                                    {
                                        header.randomTimeKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }


                                    if (lastSkillRepeatCount < buffs[waitSkills[0]].buffCool -1)
                                    {
                                        lastSkillRepeatCount++;
                                        Debug.WriteLine("" + lastSkillRepeatCount);
                                    }
                                    else
                                    {
                                        lastSkillRepeatCount = 0;
                                        waitCheck_hash.Remove(buffs[waitSkills[0]].buffKey);
                                        waitSkills.RemoveAt(0);
                                    }
                                }
                            }               // 사냥 스킬    (사냥 스킬일 경우 buffCool이 Repeat횟수)
                            else
                            {
                                if (buffs[waitSkills[0]].buffRightClick)
                                {
                                    if (buffs[waitSkills[0]].buffKey.Length > 1)
                                    {
                                        header.randomTimeSpecialKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }
                                    else
                                    {
                                        header.randomTimeKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }
                                    buffs[waitSkills[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffCool);

                                    lastSkillUseDelay = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffDelay);
                                    header.randomTimeMouseRightClick(randomMin, randomMax);
                                    waitCheck_hash.Remove(buffs[waitSkills[0]].buffKey);
                                    waitItems.RemoveAt(0);
                                }
                                else
                                {
                                    buffs[waitSkills[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffCool);
                                    lastSkillUseDelay = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffDelay);

                                    if (buffs[waitSkills[0]].buffKey.Length > 1)
                                    {
                                        header.randomTimeSpecialKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }
                                    else
                                    {
                                        header.randomTimeKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                                    }
                                    waitCheck_hash.Remove(buffs[waitSkills[0]].buffKey);
                                    waitSkills.RemoveAt(0);
                                }
                            }               // 버프 스킬
                        }
                    }



                    continue;
                }
                else
                {
                    Debug.WriteLine("버프랑 스킬 검색");

                    for (int i = 3; i < buffs.Count; i++)
                    {
                        if (DateTime.Now >= buffs[i].buffTime)
                        {
                            if (waitCheck_hash.Add(buffs[i].buffKey))           // 대기 리스트에 존재하지 않을 때 실행
                            {
                                if (buffs[i].buffItems)
                                {
                                    waitItems.Add(i);
                                }
                                else
                                {
                                    waitSkills.Add(i);
                                }
                            }
                        }
                    }                                               // 버프 대기 설정

                    bool_moveCheck = !bool_moveCheck;
                    
                    while (true)
                    {
                        if (DateTime.Now >= lastSkillUseDelay)
                        {
                            break;
                        }
                    }           //          스킬 모션 끝날떄까지 대기

                    int lastMoveInt;
                    Bitmap moveBmp;
                    while (true)
                    {
                        moveBmp = tempDelegate(gamePtr);
                        if (moveBmp != null)
                        {
                            break;
                        }
                    }
                    lastMoveInt = Int32.Parse(림보좌표검색(moveBmp));
                    moveBmp.Dispose();
                    if (form_림보설정.radioButtonCharacter기타.Checked)
                    {
                        if (bool_moveCheck)
                        {
                            tempPt.X = clientRect.left + 1136;
                            tempPt.Y = clientRect.top + 725;
                        }
                        else
                        {
                            tempPt.X = clientRect.left + 177;
                            tempPt.Y = clientRect.top + 245;
                        }               //위인지 아래인지 설정 처음은 밑으로 감

                        header.mouseMove(tempPt);
                        for (int i = 0; i < 4; i++)
                        {

                            DateTime moveWaitTime = DateTime.Now.AddMilliseconds(Convert.ToInt32(form_림보설정.numericUpDown움직임딜레이.Value));
                            header.randomTimeSpecialKey("ALTLC", randomMin, randomMax);
                            while (true)
                            {
                                if (DateTime.Now >= moveWaitTime)
                                {
                                    break;
                                }
                            }

                            
                            while (true)
                            {
                                moveBmp = tempDelegate(gamePtr);
                                if (moveBmp != null)
                                {
                                    break;
                                }
                            }
                            int tempInt = Int32.Parse(림보좌표검색(moveBmp));

                            if(lastMoveInt == tempInt)
                            {
                                i--;
                            }
                            else
                            {
                                lastMoveInt = tempInt;
                            }

                            

                            moveBmp.Dispose();
                        }
                    }
                    else if(form_림보설정.radioButtonCharacter티치엘.Checked)
                    {
                        if (bool_moveCheck)
                        {
                            tempPt.X = clientRect.left + 945;
                            tempPt.Y = clientRect.top + 629;
                        }
                        else
                        {
                            tempPt.X = clientRect.left + 369;
                            tempPt.Y = clientRect.top + 341;
                        }               //위인지 아래인지 설정 처음은 밑으로 감
                        header.mouseMove(tempPt);


                        for (int i = 0; i < 6; i++)
                        {
                            if(form_림보설정.buttonCharacterTeleport.Text.Length > 1)
                            {
                                header.randomTimeSpecialKey(form_림보설정.buttonCharacterTeleport.Text,randomMin,randomMax);
                            }
                            else
                            {
                                header.randomTimeKey(form_림보설정.buttonCharacterTeleport.Text, randomMin, randomMax);
                            }
                            DateTime moveWaitTime = DateTime.Now.AddMilliseconds(Convert.ToInt32(form_림보설정.numericUpDownCharacterTeleportDelay.Value));
                            header.randomTimeSpecialKey("ALTRC", randomMin, randomMax);
                            while (true)
                            {
                                if (DateTime.Now >= moveWaitTime)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }

            }






        }

        private void 제자리반복()
        {
            yapHeader.POINT tempPt = new yapHeader.POINT();
            yapHeader.RECT clientRect = default(yapHeader.RECT);
            yapHeader.GetWindowRect((int)gamePtr, ref clientRect);

            yapHeader.ShowWindowAsync(gamePtr, 1);

            // 윈도우에 포커스를 줘서 최상위로 만든다
            yapHeader.SetForegroundWindow(gamePtr);

            yapHeader.POINT hpPt = new yapHeader.POINT();
            yapHeader.POINT mpPt = new yapHeader.POINT();
            yapHeader.POINT spPt = new yapHeader.POINT();
            hpPt.X = 138;
            hpPt.Y = 48;
            mpPt.X = 138;
            mpPt.Y = 64;
            spPt.X = 138;
            spPt.Y = 79;

            int randomMin = Convert.ToInt32(form_제자리스킬설정.numericUpDown최소딜레이.Value);
            int randomMax = Convert.ToInt32(form_제자리스킬설정.numericUpDown최대딜레이.Value);

            DateTime lastItemUseDelay = DateTime.Now;
            DateTime lastSkillUseDelay = DateTime.Now;

            List<버프구조체> buffs = new List<버프구조체>();              // 버프 목록

            List<int> waitSkills = new List<int>();
            List<int> waitItems = new List<int>();

            HashSet<string> waitCheck_hash = new HashSet<string>();


            bool 포션사용여부 = false;
            bool bool_hp포션 = false;
            bool bool_mp포션 = false;
            bool bool_sp포션 = false;
            getBmpDelegate tempDelegate;
            if (checkBox똥컴.Checked)
            {
                tempDelegate = new getBmpDelegate(똥컴용getBmp);
            }
            else
            {
                tempDelegate = new getBmpDelegate(header.getBmp);
            }


            {
                if (checkBoxHPUse.Checked || checkBoxMPUse.Checked || checkBoxSPUse.Checked)
                {
                    포션사용여부 = true;
                }                           // 포션 사용 여부 확인

                if (포션사용여부)
                {
                    if ((checkBoxHPUse.Checked) && (!buttonHP.Text.Equals("None")))
                    {
                        bool_hp포션 = true;
                    }
                    if ((checkBoxMPUse.Checked) && (!buttonMP.Text.Equals("None")))
                    {
                        bool_mp포션 = true;
                    }
                    if ((checkBoxSPUse.Checked) && (!buttonSP.Text.Equals("None")))
                    {
                        bool_sp포션 = true;
                    }
                }
                buffs.Add(new 버프구조체(DateTime.Now, buttonHP.Text, -1000, Convert.ToInt32(numericUpDownHPDelay.Value), true, checkBoxHPRightClick.Checked));          //hp포션
                buffs.Add(new 버프구조체(DateTime.Now, buttonMP.Text, -1000, Convert.ToInt32(numericUpDownMPDelay.Value), true, checkBoxMPRightClick.Checked));
                buffs.Add(new 버프구조체(DateTime.Now, buttonSP.Text, -1000, Convert.ToInt32(numericUpDownSPDelay.Value), true, checkBoxSPRightClick.Checked));
                
                
            }           //포션 설정



            {
                for (int i = 0; i < Form1.Constants.stand_buffNumber; i++)
                {
                    if ((!((form_제자리스킬설정.Controls.Find("buttonBuff" + (i + 1), true)[0])).Text.Equals("None")) && (((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxUse" + (i + 1), true)[0])).Checked == true))
                    {
                        buffs.Add(new 버프구조체(
                            DateTime.Now,
                            form_제자리스킬설정.Controls.Find("buttonBuff" + (i + 1), true)[0].Text,
                            Convert.ToInt32(((NumericUpDown)(form_제자리스킬설정.Controls.Find("numericUpDownBuffCool" + (i + 1), true)[0])).Value),
                            Convert.ToInt32(((NumericUpDown)(form_제자리스킬설정.Controls.Find("numericUpDownBuffDelay" + (i + 1), true)[0])).Value),
                            ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxBuff" + (i + 1), true)[0])).Checked,
                            ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxBuffRightClick" + (i + 1), true)[0])).Checked
                            ));
                    }
                }

            }               // 버프 설정


            {
                for (int i = 0; i < Form1.Constants.stand_skillNumber; i++)
                {
                    if ((!((form_제자리스킬설정.Controls.Find("buttonSkill" + (i + 1), true)[0])).Text.Equals("None")) && (((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxSkillUse" + (i + 1), true)[0])).Checked == true))
                    {
                        buffs.Add(new 버프구조체(
                            DateTime.Now,
                            form_제자리스킬설정.Controls.Find("buttonSkill" + (i + 1), true)[0].Text,
                            -1,
                            Convert.ToInt32(((NumericUpDown)(form_제자리스킬설정.Controls.Find("numericUpDownSkillDelay" + (i + 1), true)[0])).Value),
                            false,
                            ((CheckBox)(form_제자리스킬설정.Controls.Find("checkBoxSkillRightClick" + (i + 1), true)[0])).Checked
                            ));
                    }
                }


            }           // 스킬 설정



            while (true)
            {
                Bitmap tempBmp = tempDelegate(gamePtr);
                if (tempBmp == null)
                {
                    continue;
                }
                if (hpColor.IsEmpty)
                {
                    hpColor = tempBmp.GetPixel(hpPt.X, hpPt.Y);
                    mpColor = tempBmp.GetPixel(mpPt.X, mpPt.Y);
                    spColor = tempBmp.GetPixel(spPt.X, spPt.Y);
                }
                tempBmp.Dispose();
                break;
            }





            while (true)
            {

                if ((gamePtr == IntPtr.Zero) || (!yapHeader.IsWindow(gamePtr)))
                {
                    MessageBox.Show("에러 발생.");

                    return;
                }

                header.mouseMove(clientRect.left + 643, clientRect.top + 482);              // 마우스를 캐릭터 쪽으로



                {
                    if ((waitItems.Count > 0) && (DateTime.Now >= lastItemUseDelay))
                    {
                        if (buffs[waitItems[0]].buffRightClick)
                        {
                            if (buffs[waitItems[0]].buffKey.Length > 1)
                            {
                                header.randomTimeSpecialKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                            }
                            else
                            {
                                header.randomTimeKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                            }
                            buffs[waitItems[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffCool);

                            lastItemUseDelay = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffDelay);
                            header.randomTimeMouseRightClick(randomMin, randomMax);
                            waitCheck_hash.Remove(buffs[waitItems[0]].buffKey);
                            waitItems.RemoveAt(0);
                        }
                        else
                        {
                            buffs[waitItems[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffCool);
                            lastItemUseDelay = DateTime.Now.AddMilliseconds(buffs[waitItems[0]].buffDelay);

                            if (buffs[waitItems[0]].buffKey.Length > 1)
                            {
                                header.randomTimeSpecialKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                            }
                            else
                            {
                                header.randomTimeKey(buffs[waitItems[0]].buffKey, randomMin, randomMax);
                            }
                            waitCheck_hash.Remove(buffs[waitItems[0]].buffKey);
                            waitItems.RemoveAt(0);
                        }
                    }

                    if ((waitSkills.Count > 0) && (DateTime.Now >= lastSkillUseDelay))
                    {
                        if (buffs[waitSkills[0]].buffRightClick)
                        {
                            if (buffs[waitSkills[0]].buffKey.Length > 1)
                            {
                                header.randomTimeSpecialKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                            }
                            else
                            {
                                header.randomTimeKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                            }
                            buffs[waitSkills[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffCool);

                            lastSkillUseDelay = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffDelay);
                            header.randomTimeMouseRightClick(randomMin, randomMax);
                            waitCheck_hash.Remove(buffs[waitSkills[0]].buffKey);
                            waitItems.RemoveAt(0);
                        }
                        else
                        {
                            buffs[waitSkills[0]].buffTime = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffCool);
                            lastSkillUseDelay = DateTime.Now.AddMilliseconds(buffs[waitSkills[0]].buffDelay);

                            if (buffs[waitSkills[0]].buffKey.Length > 1)
                            {
                                header.randomTimeSpecialKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                            }
                            else
                            {
                                header.randomTimeKey(buffs[waitSkills[0]].buffKey, randomMin, randomMax);
                            }
                            waitCheck_hash.Remove(buffs[waitSkills[0]].buffKey);
                            waitSkills.RemoveAt(0);
                        }
                    }
                }           //스킬 , 아이템 사용



                if (포션사용여부)
                {
                    Bitmap tempBmp = tempDelegate(gamePtr);
                    if (tempBmp == null)
                    {
                        continue;
                    }

                    if (bool_sp포션)
                    {
                        Color tempColor = tempBmp.GetPixel(spPt.X, spPt.Y);
                        if (tempColor.ToArgb() != spColor.ToArgb())
                        {
                            if (waitCheck_hash.Add(buttonSP.Text))             // 대기 리스트에 존재하지 않을 때 실행
                            {
                                waitItems.Insert(0, 2);
                            }
                        }
                    }
                    if (bool_mp포션)
                    {
                        Color tempColor = tempBmp.GetPixel(mpPt.X, mpPt.Y);
                        if (tempColor.ToArgb() != mpColor.ToArgb())
                        {
                            if (waitCheck_hash.Add(buttonMP.Text))             // 포션이 이미 리스트에 존재하지 않을 때 실행
                            {
                                waitItems.Insert(0, 1);
                            }
                        }
                    }
                    if (bool_hp포션)
                    {
                        Color tempColor = tempBmp.GetPixel(hpPt.X, hpPt.Y);
                        if (tempColor.ToArgb() != hpColor.ToArgb())
                        {
                            if (waitCheck_hash.Add(buttonHP.Text))             // 포션이 이미 리스트에 존재하지 않을 때 실행
                            {
                                waitItems.Insert(0, 0);
                            }


                        }
                    }


                    tempBmp.Dispose();
                }




                for (int i = 3; i < buffs.Count; i++)
                {
                    if (DateTime.Now >= buffs[i].buffTime)
                    {
                        if (waitCheck_hash.Add(buffs[i].buffKey))           // 대기 리스트에 존재하지 않을 때 실행
                        {
                            if (buffs[i].buffItems)
                            {
                                waitItems.Add(i);
                            }
                            else
                            {
                                waitSkills.Add(i);
                            }
                        }
                    }
                }                                               // 버프 대기 설정








            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            header.setDelay("14");
            header.arduEnd();
            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process p in mProcess)
                p.Kill();
        }

        private void 제자리_Click(object sender, EventArgs e)
        {
            IntPtr tempPtr = header.getHwndName("Talesweaver");

            if (tempPtr == IntPtr.Zero)
            {
                MessageBox.Show("게임 실행 X");
                return;
            }

            gamePtr = tempPtr;

            제자리_Thread = new Thread(new ThreadStart(제자리반복));
            제자리_Thread.Start();
            groupBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr tempPtr = header.getHwndName("Talesweaver");
            int x = 250;
            gamePtr = tempPtr;
            yapHeader.POINT tempPt = new yapHeader.POINT();
            yapHeader.GetCursorPos(out tempPt);
            

            if (tempPtr == IntPtr.Zero)
            {
                MessageBox.Show("게임 실행 X");
                return;
            }

            yapHeader.RECT clientRect = default(yapHeader.RECT);
            yapHeader.GetWindowRect((int)tempPtr, ref clientRect);


        }

        private void 제자리설정버튼_Click(object sender, EventArgs e)
        {
            form_제자리스킬설정.ShowDialog();
        }

        private void 림보설정버튼_Click(object sender, EventArgs e)
        {
            form_림보설정.ShowDialog();
        }



        private void 물약설정저장_Click(object sender, EventArgs e)
        {
            yapHeader.WritePrivateProfileString("물약설정", "HP키", buttonHP.Text, iniPath);
            yapHeader.WritePrivateProfileString("물약설정", "MP키", buttonMP.Text + "", iniPath);
            yapHeader.WritePrivateProfileString("물약설정", "SP키", buttonSP.Text + "", iniPath);

            yapHeader.WritePrivateProfileString("물약설정", "HP딜레이", numericUpDownHPDelay.Value + "", iniPath);
            yapHeader.WritePrivateProfileString("물약설정", "MP딜레이", numericUpDownMPDelay.Value + "", iniPath);
            yapHeader.WritePrivateProfileString("물약설정", "SP딜레이", numericUpDownSPDelay.Value + "", iniPath);


            if (checkBoxHPRightClick.Checked)
            {
                yapHeader.WritePrivateProfileString("물약설정", "HP우측클릭", "true", iniPath);
            }
            else
            {
                yapHeader.WritePrivateProfileString("물약설정", "HP우측클릭", "false", iniPath);
            }

            if (checkBoxMPRightClick.Checked)
            {
                yapHeader.WritePrivateProfileString("물약설정", "MP우측클릭", "true", iniPath);
            }
            else
            {
                yapHeader.WritePrivateProfileString("물약설정", "MP우측클릭", "false", iniPath);
            }

            if (checkBoxSPRightClick.Checked)
            {
                yapHeader.WritePrivateProfileString("물약설정", "SP우측클릭", "true", iniPath);
            }
            else
            {
                yapHeader.WritePrivateProfileString("물약설정", "SP우측클릭", "false", iniPath);
            }

            if (checkBoxHPUse.Checked)
            {
                yapHeader.WritePrivateProfileString("물약설정", "HP사용", "true", iniPath);
            }
            else
            {
                yapHeader.WritePrivateProfileString("물약설정", "HP사용", "false", iniPath);
            }

            if (checkBoxMPUse.Checked)
            {
                yapHeader.WritePrivateProfileString("물약설정", "MP사용", "true", iniPath);
            }
            else
            {
                yapHeader.WritePrivateProfileString("물약설정", "MP사용", "false", iniPath);
            }

            if (checkBoxSPUse.Checked)
            {
                yapHeader.WritePrivateProfileString("물약설정", "SP사용", "true", iniPath);
            }
            else
            {
                yapHeader.WritePrivateProfileString("물약설정", "SP사용", "false", iniPath);
            }



        }

        private void buttonHP_Click(object sender, EventArgs e)
        {
            키입력받는폼 frm1 = new 키입력받는폼();
            frm1.TextSendEvent += new 키입력받는폼.Form2_EventHandler((s) =>
            {

                Button temp = sender as Button;
                temp.Text = s;


            });

            frm1.ShowDialog();
        }

        private void 림보시작버튼_Click(object sender, EventArgs e)
        {
            IntPtr tempPtr = header.getHwndName("Talesweaver");

            if (tempPtr == IntPtr.Zero)
            {
                MessageBox.Show("게임 실행 X");
                return;
            }

            gamePtr = tempPtr;

            림보_Thread = new Thread(new ThreadStart(림보반복));
            림보_Thread.Start();
            groupBox1.Focus();
        }
    }
}
