using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 얍
{
    public partial class 제자리스킬설정폼 : Form
    {


        public 제자리스킬설정폼()
        {
            InitializeComponent();


            
        }

        private void numericUpDown최소딜레이_ValueChanged(object sender, EventArgs e)
        {
            int minDelay = Convert.ToInt32(numericUpDown최소딜레이.Value), maxDelay = Convert.ToInt32(numericUpDown최대딜레이.Value);

            if (minDelay > maxDelay)
            {
                numericUpDown최대딜레이.Value = new decimal(minDelay);
            }
        }

        private void numericUpDown최대딜레이_ValueChanged(object sender, EventArgs e)
        {
            int minDelay = Convert.ToInt32(numericUpDown최소딜레이.Value), maxDelay = Convert.ToInt32(numericUpDown최대딜레이.Value);

            if (minDelay > maxDelay)
            {
                numericUpDown최소딜레이.Value = new decimal(maxDelay);
            }
        }

        private void button_제자리스킬설정폼나가기_Click(object sender, EventArgs e)
        {
            
            groupBox1.Focus();
            this.Hide();
        }


        private void button_스킬설정(object sender, EventArgs e)
        {

            키입력받는폼 frm1 = new 키입력받는폼();
            frm1.TextSendEvent += new 키입력받는폼.Form2_EventHandler((s) => {

                Button temp = sender as Button;
                temp.Text = s;
            
            
            });

            frm1.ShowDialog();
        }

        private void button_제자리스킬설정저장_Click(object sender, EventArgs e)
        {
            
            string tempString = "";

            groupBox1.Focus();



            yapHeader.WritePrivateProfileString("제자리스킬설정", "최소딜레이", numericUpDown최소딜레이.Value +"", Form1.iniPath);      // 최소 딜레이 저장
            yapHeader.WritePrivateProfileString("제자리스킬설정", "최대딜레이", numericUpDown최대딜레이.Value + "", Form1.iniPath);      // 최대 딜레이 저장

            for(int i = 0; i < Form1.Constants.stand_skillNumber; i++)
            {
                yapHeader.WritePrivateProfileString("제자리스킬설정", "스킬키" + (i + 1), ((this.Controls.Find("buttonSkill" + (i + 1), true)[0])).Text + "", Form1.iniPath);      // 스킬키 저장
                yapHeader.WritePrivateProfileString("제자리스킬설정", "스킬딜레이" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownSkillDelay" + (i + 1), true)[0])).Value + "", Form1.iniPath);      // 스킬 딜레이 저장

                if (((CheckBox)(this.Controls.Find("checkBoxSkillUse" + (i + 1), true)[0])).Checked)
                {
                    tempString = "true";
                }
                else
                {
                    tempString = "false";
                }

                yapHeader.WritePrivateProfileString("제자리스킬설정", "스킬사용여부" + (i + 1), tempString, Form1.iniPath);

                if (((CheckBox)(this.Controls.Find("checkBoxSkillRightClick" + (i + 1), true)[0])).Checked)
                {
                    tempString = "true";
                }
                else
                {
                    tempString = "false";
                }

                yapHeader.WritePrivateProfileString("제자리스킬설정", "스킬우측클릭" + (i + 1), tempString, Form1.iniPath);

            }

            for (int i = 0; i < Form1.Constants.stand_buffNumber; i++)
            {
                yapHeader.WritePrivateProfileString("제자리스킬설정", "버프키" + (i + 1), ((this.Controls.Find("buttonBuff" + (i + 1), true)[0])).Text + "", Form1.iniPath);      // 버프키 저장
                yapHeader.WritePrivateProfileString("제자리스킬설정", "버프재사용시간" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownBuffCool" + (i + 1), true)[0])).Value + "", Form1.iniPath);      // 버프 재사용시간 저장
                yapHeader.WritePrivateProfileString("제자리스킬설정", "버프딜레이" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownBuffDelay" + (i + 1), true)[0])).Value +"", Form1.iniPath);      // 버프 딜레이 저장

                {
                    if (((CheckBox)(this.Controls.Find("checkBoxBuff" + (i + 1), true)[0])).Checked)
                    {
                        tempString = "true";
                    }
                    else
                    {
                        tempString = "false";
                    }

                    yapHeader.WritePrivateProfileString("제자리스킬설정", "버프아이템" + (i + 1), tempString, Form1.iniPath);
                }                       //버프 아이템 체크 저장
                {
                    if (((CheckBox)(this.Controls.Find("checkBoxUse" + (i + 1), true)[0])).Checked)
                    {
                        tempString = "true";
                    }
                    else
                    {
                        tempString = "false";
                    }

                    yapHeader.WritePrivateProfileString("제자리스킬설정", "버프사용여부" + (i + 1), tempString, Form1.iniPath);
                }                       //버프 사용 여부 저장

                {
                    if (((CheckBox)(this.Controls.Find("checkBoxBuffRightClick" + (i + 1), true)[0])).Checked)
                    {
                        tempString = "true";
                    }
                    else
                    {
                        tempString = "false";
                    }

                    yapHeader.WritePrivateProfileString("제자리스킬설정", "버프우측클릭" + (i + 1), tempString, Form1.iniPath);
                }                       //버프 우측 클릭

            }                   //버프 스킬 저장
        }
    }
}
