using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 얍
{
    public partial class 림보설정 : Form
    {
        public 림보설정()
        {
            InitializeComponent();
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

        private void button_림보설정저장_Click(object sender, EventArgs e)
        {
            string tempString = "";

            groupBox1.Focus();


            {
                yapHeader.WritePrivateProfileString("림보설정", "최소딜레이", numericUpDown최소딜레이.Value + "", Form1.iniPath);      // 최소 딜레이 저장
                yapHeader.WritePrivateProfileString("림보설정", "최대딜레이", numericUpDown최대딜레이.Value + "", Form1.iniPath);      // 최대 딜레이 저장
            }                                                                               // 림보 공통 설정

            for (int i = 0; i < Form1.Constants.stand_skillNumber; i++)
            {
                yapHeader.WritePrivateProfileString("림보설정", "스킬키" + (i + 1), ((this.Controls.Find("buttonSkill" + (i + 1), true)[0])).Text + "", Form1.iniPath);      // 스킬키 저장
                yapHeader.WritePrivateProfileString("림보설정", "스킬딜레이" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownSkillDelay" + (i + 1), true)[0])).Value + "", Form1.iniPath);      // 스킬 딜레이 저장
                yapHeader.WritePrivateProfileString("림보설정", "스킬반복횟수" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownSkillRepeat" + (i + 1), true)[0])).Value + "", Form1.iniPath);      // 스킬 딜레이 저장

                if (((CheckBox)(this.Controls.Find("checkBoxSkillUse" + (i + 1), true)[0])).Checked)
                {
                    tempString = "true";
                }
                else
                {
                    tempString = "false";
                }

                yapHeader.WritePrivateProfileString("림보설정", "스킬사용여부" + (i + 1), tempString, Form1.iniPath);

                if (((CheckBox)(this.Controls.Find("checkBoxSkillRightClick" + (i + 1), true)[0])).Checked)
                {
                    tempString = "true";
                }
                else
                {
                    tempString = "false";
                }

                yapHeader.WritePrivateProfileString("림보설정", "스킬우측클릭" + (i + 1), tempString, Form1.iniPath);

            }                 //스킬 저장                                                              

            for (int i = 0; i < Form1.Constants.stand_buffNumber; i++)
            {
                yapHeader.WritePrivateProfileString("림보설정", "버프키" + (i + 1), ((this.Controls.Find("buttonBuff" + (i + 1), true)[0])).Text + "", Form1.iniPath);      // 버프키 저장
                yapHeader.WritePrivateProfileString("림보설정", "버프재사용시간" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownBuffCool" + (i + 1), true)[0])).Value + "", Form1.iniPath);      // 버프 재사용시간 저장
                yapHeader.WritePrivateProfileString("림보설정", "버프딜레이" + (i + 1), ((NumericUpDown)(this.Controls.Find("numericUpDownBuffDelay" + (i + 1), true)[0])).Value + "", Form1.iniPath);      // 버프 딜레이 저장

                {
                    if (((CheckBox)(this.Controls.Find("checkBoxBuff" + (i + 1), true)[0])).Checked)
                    {
                        tempString = "true";
                    }
                    else
                    {
                        tempString = "false";
                    }

                    yapHeader.WritePrivateProfileString("림보설정", "버프아이템" + (i + 1), tempString, Form1.iniPath);
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

                    yapHeader.WritePrivateProfileString("림보설정", "버프사용여부" + (i + 1), tempString, Form1.iniPath);
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

                    yapHeader.WritePrivateProfileString("림보설정", "버프우측클릭" + (i + 1), tempString, Form1.iniPath);
                }                       //버프 우측 클릭

            }                   //버프 저장

            {
                

                

                for(int i = 0; i< Form1.specialCharacter.Length; i++)
                {
                    if (((RadioButton)(this.Controls.Find("radioButtonCharacter" + Form1.specialCharacter[i], true)[0])).Checked)
                    {
                        yapHeader.WritePrivateProfileString("림보설정", "림보캐릭터", Form1.specialCharacter[i], Form1.iniPath);
                        break;
                    }
                }

                yapHeader.WritePrivateProfileString("림보설정", "티치엘텔포", buttonCharacterTeleport.Text, Form1.iniPath);      // 티치엘 텔포 키 설정
                yapHeader.WritePrivateProfileString("림보설정", "티치엘텔포딜레이", numericUpDownCharacterTeleportDelay.Value + "", Form1.iniPath);      // 티치엘 텔포 딜레이
                yapHeader.WritePrivateProfileString("림보설정", "움직임딜레이", numericUpDown움직임딜레이.Value + "", Form1.iniPath);      // 움직임 딜레이



            }                                                                               // 특수캐릭터 저장
        }

        private void button_림보스킬설정폼나가기_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void radioButtonCharacter기타_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    label텔레포트.Hide();
                    label움직임딜레이.Hide();
                    buttonCharacterTeleport.Hide();
                    label텔레포트딜레이.Hide();
                    numericUpDown움직임딜레이.Hide();
                    numericUpDownCharacterTeleportDelay.Hide();

                    if (rb.Equals(this.radioButtonCharacter기타))
                    {
                        label움직임딜레이.Show();
                        numericUpDown움직임딜레이.Show();
                    }
                    else if (rb.Equals(this.radioButtonCharacter티치엘))
                    {
                        label텔레포트.Show();
                        buttonCharacterTeleport.Show();
                        label텔레포트딜레이.Show();
                        numericUpDownCharacterTeleportDelay.Show();
                    }
                    else
                    {
                        MessageBox.Show("error");
                    }
                }
            }
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
    }
}
