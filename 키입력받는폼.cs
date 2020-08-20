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
    public partial class 키입력받는폼 : Form
    {
        public delegate void Form2_EventHandler(string data); // string 을 반환값으로 같은 대리자를 선언
        public event Form2_EventHandler TextSendEvent;

        public 키입력받는폼()
        {
            InitializeComponent();
        }

        private void 키입력받는폼_KeyDown(object sender, KeyEventArgs e)
        {
            if( ((e.KeyValue >= 0x30) && (e.KeyValue <= 0x5A)) || ((e.KeyValue >= 0x70) && (e.KeyValue <= 0x7B)))
            {
                if ((e.KeyValue >= 30) && (e.KeyValue <= 0x39))
                {
                    TextSendEvent("" + (e.KeyValue - 0x30)); // 스트링값을 메인폼에게 보내줌
                }
                else
                {
                    TextSendEvent("" + e.KeyData); // 스트링값을 메인폼에게 보내줌
                }
                this.Close();
            }
        }
    }
}
