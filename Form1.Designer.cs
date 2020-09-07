namespace 바람연
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton국내 = new System.Windows.Forms.RadioButton();
            this.radioButton부여 = new System.Windows.Forms.RadioButton();
            this.radioButton12지신 = new System.Windows.Forms.RadioButton();
            this.radioButton산적 = new System.Windows.Forms.RadioButton();
            this.radioButton민중 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(689, 194);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(527, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "도감 시작";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(303, 290);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton민중);
            this.groupBox1.Controls.Add(this.radioButton산적);
            this.groupBox1.Controls.Add(this.radioButton12지신);
            this.groupBox1.Controls.Add(this.radioButton부여);
            this.groupBox1.Controls.Add(this.radioButton국내);
            this.groupBox1.Location = new System.Drawing.Point(30, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 152);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "도감 설정";
            // 
            // radioButton국내
            // 
            this.radioButton국내.AutoSize = true;
            this.radioButton국내.Location = new System.Drawing.Point(7, 21);
            this.radioButton국내.Name = "radioButton국내";
            this.radioButton국내.Size = new System.Drawing.Size(47, 16);
            this.radioButton국내.TabIndex = 0;
            this.radioButton국내.TabStop = true;
            this.radioButton국내.Text = "국내";
            this.radioButton국내.UseVisualStyleBackColor = true;
            // 
            // radioButton부여
            // 
            this.radioButton부여.AutoSize = true;
            this.radioButton부여.Location = new System.Drawing.Point(60, 20);
            this.radioButton부여.Name = "radioButton부여";
            this.radioButton부여.Size = new System.Drawing.Size(47, 16);
            this.radioButton부여.TabIndex = 1;
            this.radioButton부여.TabStop = true;
            this.radioButton부여.Text = "부여";
            this.radioButton부여.UseVisualStyleBackColor = true;
            // 
            // radioButton12지신
            // 
            this.radioButton12지신.AutoSize = true;
            this.radioButton12지신.Location = new System.Drawing.Point(113, 20);
            this.radioButton12지신.Name = "radioButton12지신";
            this.radioButton12지신.Size = new System.Drawing.Size(59, 16);
            this.radioButton12지신.TabIndex = 1;
            this.radioButton12지신.TabStop = true;
            this.radioButton12지신.Text = "12지신";
            this.radioButton12지신.UseVisualStyleBackColor = true;
            // 
            // radioButton산적
            // 
            this.radioButton산적.AutoSize = true;
            this.radioButton산적.Location = new System.Drawing.Point(178, 21);
            this.radioButton산적.Name = "radioButton산적";
            this.radioButton산적.Size = new System.Drawing.Size(47, 16);
            this.radioButton산적.TabIndex = 1;
            this.radioButton산적.TabStop = true;
            this.radioButton산적.Text = "산적";
            this.radioButton산적.UseVisualStyleBackColor = true;
            // 
            // radioButton민중
            // 
            this.radioButton민중.AutoSize = true;
            this.radioButton민중.Location = new System.Drawing.Point(231, 21);
            this.radioButton민중.Name = "radioButton민중";
            this.radioButton민중.Size = new System.Drawing.Size(71, 16);
            this.radioButton민중.TabIndex = 1;
            this.radioButton민중.TabStop = true;
            this.radioButton민중.Text = "민중왕릉";
            this.radioButton민중.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton민중;
        private System.Windows.Forms.RadioButton radioButton산적;
        private System.Windows.Forms.RadioButton radioButton12지신;
        private System.Windows.Forms.RadioButton radioButton부여;
        private System.Windows.Forms.RadioButton radioButton국내;
    }
}

