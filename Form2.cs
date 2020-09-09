using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 빡자사
{
    public partial class Form2 : Form
    {

        public delegate void FormSendDataHandler(int no);

        public event FormSendDataHandler FormSendEvent;



        Form frm1;
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(Form1 _form)
        {
            InitializeComponent();
            frm1 = _form;

        }

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
        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            매크로종료.Size = this.Size;
        }

        private void 매크로종료_Click(object sender, EventArgs e)
        {
            매크로종료.Enabled = false;
            if (매크로종료.Text.ToString().Equals("일시 정지"))
            {
                매크로종료.Text = "다시 실행";
                int no = -1;

                this.Location = new System.Drawing.Point(0, 800);
                this.Size = new System.Drawing.Size(500, 100);

                this.FormSendEvent(no);
            }
            else
            {
                매크로종료.Text = "일시 정지";

                int no = 1;

                this.Location = new System.Drawing.Point(0, 800);
                this.Size = new System.Drawing.Size(500, 100);


                this.FormSendEvent(no);
            }

            Delay(500);
            매크로종료.Enabled = true;
        }

    }
}
