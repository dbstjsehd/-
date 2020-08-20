using System.Windows.Forms;

namespace 얍
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
            this.제자리버튼 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.물약설정저장 = new System.Windows.Forms.Button();
            this.numericUpDownSPDelay = new System.Windows.Forms.NumericUpDown();
            this.checkBoxSPUse = new System.Windows.Forms.CheckBox();
            this.checkBoxSPRightClick = new System.Windows.Forms.CheckBox();
            this.label26 = new System.Windows.Forms.Label();
            this.buttonSP = new System.Windows.Forms.Button();
            this.numericUpDownMPDelay = new System.Windows.Forms.NumericUpDown();
            this.checkBoxMPUse = new System.Windows.Forms.CheckBox();
            this.checkBoxMPRightClick = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.buttonMP = new System.Windows.Forms.Button();
            this.numericUpDownHPDelay = new System.Windows.Forms.NumericUpDown();
            this.checkBoxHPUse = new System.Windows.Forms.CheckBox();
            this.checkBoxHPRightClick = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.buttonHP = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.제자리설정버튼 = new System.Windows.Forms.Button();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.림보설정버튼 = new System.Windows.Forms.Button();
            this.림보시작버튼 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBox똥컴 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSPDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMPDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHPDelay)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // 제자리버튼
            // 
            this.제자리버튼.Location = new System.Drawing.Point(6, 20);
            this.제자리버튼.Name = "제자리버튼";
            this.제자리버튼.Size = new System.Drawing.Size(75, 23);
            this.제자리버튼.TabIndex = 0;
            this.제자리버튼.Text = "시작";
            this.제자리버튼.UseVisualStyleBackColor = true;
            this.제자리버튼.Click += new System.EventHandler(this.제자리_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.물약설정저장);
            this.groupBox1.Controls.Add(this.numericUpDownSPDelay);
            this.groupBox1.Controls.Add(this.checkBoxSPUse);
            this.groupBox1.Controls.Add(this.checkBoxSPRightClick);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.buttonSP);
            this.groupBox1.Controls.Add(this.numericUpDownMPDelay);
            this.groupBox1.Controls.Add(this.checkBoxMPUse);
            this.groupBox1.Controls.Add(this.checkBoxMPRightClick);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.buttonMP);
            this.groupBox1.Controls.Add(this.numericUpDownHPDelay);
            this.groupBox1.Controls.Add(this.checkBoxHPUse);
            this.groupBox1.Controls.Add(this.checkBoxHPRightClick);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.buttonHP);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(202, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 162);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "물약 설정";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 8.25F);
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(112, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 11);
            this.label2.TabIndex = 93;
            this.label2.Text = "모든 종료 키는 PageDown 키 입니다.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(14, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 22);
            this.label1.TabIndex = 92;
            this.label1.Text = "포션은 사용이 체크되어 있을시 게이지가\n 절반이하로 떨어지면 사용합니다.";
            // 
            // 물약설정저장
            // 
            this.물약설정저장.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.물약설정저장.Location = new System.Drawing.Point(236, 133);
            this.물약설정저장.Name = "물약설정저장";
            this.물약설정저장.Size = new System.Drawing.Size(75, 23);
            this.물약설정저장.TabIndex = 91;
            this.물약설정저장.Text = "설정 저장";
            this.물약설정저장.UseVisualStyleBackColor = true;
            this.물약설정저장.Click += new System.EventHandler(this.물약설정저장_Click);
            // 
            // numericUpDownSPDelay
            // 
            this.numericUpDownSPDelay.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.numericUpDownSPDelay.Location = new System.Drawing.Point(175, 99);
            this.numericUpDownSPDelay.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownSPDelay.Name = "numericUpDownSPDelay";
            this.numericUpDownSPDelay.Size = new System.Drawing.Size(53, 21);
            this.numericUpDownSPDelay.TabIndex = 86;
            this.numericUpDownSPDelay.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // checkBoxSPUse
            // 
            this.checkBoxSPUse.AutoSize = true;
            this.checkBoxSPUse.Location = new System.Drawing.Point(276, 104);
            this.checkBoxSPUse.Name = "checkBoxSPUse";
            this.checkBoxSPUse.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSPUse.TabIndex = 87;
            this.checkBoxSPUse.UseVisualStyleBackColor = true;
            // 
            // checkBoxSPRightClick
            // 
            this.checkBoxSPRightClick.AutoSize = true;
            this.checkBoxSPRightClick.Location = new System.Drawing.Point(122, 104);
            this.checkBoxSPRightClick.Name = "checkBoxSPRightClick";
            this.checkBoxSPRightClick.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSPRightClick.TabIndex = 88;
            this.checkBoxSPRightClick.UseVisualStyleBackColor = true;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label26.Location = new System.Drawing.Point(5, 104);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(23, 12);
            this.label26.TabIndex = 89;
            this.label26.Text = "SP";
            // 
            // buttonSP
            // 
            this.buttonSP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.buttonSP.Location = new System.Drawing.Point(38, 99);
            this.buttonSP.Name = "buttonSP";
            this.buttonSP.Size = new System.Drawing.Size(53, 23);
            this.buttonSP.TabIndex = 90;
            this.buttonSP.Text = "None";
            this.buttonSP.UseVisualStyleBackColor = true;
            this.buttonSP.Click += new System.EventHandler(this.buttonHP_Click);
            // 
            // numericUpDownMPDelay
            // 
            this.numericUpDownMPDelay.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.numericUpDownMPDelay.Location = new System.Drawing.Point(175, 70);
            this.numericUpDownMPDelay.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMPDelay.Name = "numericUpDownMPDelay";
            this.numericUpDownMPDelay.Size = new System.Drawing.Size(53, 21);
            this.numericUpDownMPDelay.TabIndex = 81;
            this.numericUpDownMPDelay.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // checkBoxMPUse
            // 
            this.checkBoxMPUse.AutoSize = true;
            this.checkBoxMPUse.Location = new System.Drawing.Point(276, 75);
            this.checkBoxMPUse.Name = "checkBoxMPUse";
            this.checkBoxMPUse.Size = new System.Drawing.Size(15, 14);
            this.checkBoxMPUse.TabIndex = 82;
            this.checkBoxMPUse.UseVisualStyleBackColor = true;
            // 
            // checkBoxMPRightClick
            // 
            this.checkBoxMPRightClick.AutoSize = true;
            this.checkBoxMPRightClick.Location = new System.Drawing.Point(122, 75);
            this.checkBoxMPRightClick.Name = "checkBoxMPRightClick";
            this.checkBoxMPRightClick.Size = new System.Drawing.Size(15, 14);
            this.checkBoxMPRightClick.TabIndex = 83;
            this.checkBoxMPRightClick.UseVisualStyleBackColor = true;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(4, 75);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(26, 12);
            this.label25.TabIndex = 84;
            this.label25.Text = "MP";
            // 
            // buttonMP
            // 
            this.buttonMP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.buttonMP.Location = new System.Drawing.Point(38, 70);
            this.buttonMP.Name = "buttonMP";
            this.buttonMP.Size = new System.Drawing.Size(53, 23);
            this.buttonMP.TabIndex = 85;
            this.buttonMP.Text = "None";
            this.buttonMP.UseVisualStyleBackColor = true;
            this.buttonMP.Click += new System.EventHandler(this.buttonHP_Click);
            // 
            // numericUpDownHPDelay
            // 
            this.numericUpDownHPDelay.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.numericUpDownHPDelay.Location = new System.Drawing.Point(175, 41);
            this.numericUpDownHPDelay.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownHPDelay.Name = "numericUpDownHPDelay";
            this.numericUpDownHPDelay.Size = new System.Drawing.Size(53, 21);
            this.numericUpDownHPDelay.TabIndex = 80;
            this.numericUpDownHPDelay.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // checkBoxHPUse
            // 
            this.checkBoxHPUse.AutoSize = true;
            this.checkBoxHPUse.Location = new System.Drawing.Point(276, 46);
            this.checkBoxHPUse.Name = "checkBoxHPUse";
            this.checkBoxHPUse.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHPUse.TabIndex = 78;
            this.checkBoxHPUse.UseVisualStyleBackColor = true;
            // 
            // checkBoxHPRightClick
            // 
            this.checkBoxHPRightClick.AutoSize = true;
            this.checkBoxHPRightClick.Location = new System.Drawing.Point(122, 46);
            this.checkBoxHPRightClick.Name = "checkBoxHPRightClick";
            this.checkBoxHPRightClick.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHPRightClick.TabIndex = 77;
            this.checkBoxHPRightClick.UseVisualStyleBackColor = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label24.Location = new System.Drawing.Point(6, 46);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(23, 12);
            this.label24.TabIndex = 76;
            this.label24.Text = "HP";
            // 
            // buttonHP
            // 
            this.buttonHP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.buttonHP.Location = new System.Drawing.Point(38, 41);
            this.buttonHP.Name = "buttonHP";
            this.buttonHP.Size = new System.Drawing.Size(53, 23);
            this.buttonHP.TabIndex = 75;
            this.buttonHP.Text = "None";
            this.buttonHP.UseVisualStyleBackColor = true;
            this.buttonHP.Click += new System.EventHandler(this.buttonHP_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(268, 25);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(43, 12);
            this.label23.TabIndex = 74;
            this.label23.Text = "사용 ?";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label22.Location = new System.Drawing.Point(169, 25);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(93, 12);
            this.label22.TabIndex = 72;
            this.label22.Text = "사용 후 딜레이";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label21.Location = new System.Drawing.Point(101, 25);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(62, 12);
            this.label21.TabIndex = 79;
            this.label21.Text = "우측 클릭";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(42, 25);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(49, 12);
            this.label20.TabIndex = 71;
            this.label20.Text = "물약 키";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(624, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.제자리설정버튼);
            this.groupBox2.Controls.Add(this.제자리버튼);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(184, 48);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "제자리 동꼽 대신";
            // 
            // 제자리설정버튼
            // 
            this.제자리설정버튼.Location = new System.Drawing.Point(87, 20);
            this.제자리설정버튼.Name = "제자리설정버튼";
            this.제자리설정버튼.Size = new System.Drawing.Size(75, 23);
            this.제자리설정버튼.TabIndex = 1;
            this.제자리설정버튼.Text = "설정";
            this.제자리설정버튼.UseVisualStyleBackColor = true;
            this.제자리설정버튼.Click += new System.EventHandler(this.제자리설정버튼_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.림보설정버튼);
            this.groupBox3.Controls.Add(this.림보시작버튼);
            this.groupBox3.Location = new System.Drawing.Point(12, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 48);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "림보 자동";
            // 
            // 림보설정버튼
            // 
            this.림보설정버튼.Location = new System.Drawing.Point(87, 20);
            this.림보설정버튼.Name = "림보설정버튼";
            this.림보설정버튼.Size = new System.Drawing.Size(75, 23);
            this.림보설정버튼.TabIndex = 1;
            this.림보설정버튼.Text = "설정";
            this.림보설정버튼.UseVisualStyleBackColor = true;
            this.림보설정버튼.Click += new System.EventHandler(this.림보설정버튼_Click);
            // 
            // 림보시작버튼
            // 
            this.림보시작버튼.Location = new System.Drawing.Point(6, 20);
            this.림보시작버튼.Name = "림보시작버튼";
            this.림보시작버튼.Size = new System.Drawing.Size(75, 23);
            this.림보시작버튼.TabIndex = 0;
            this.림보시작버튼.Text = "시작";
            this.림보시작버튼.UseVisualStyleBackColor = true;
            this.림보시작버튼.Click += new System.EventHandler(this.림보시작버튼_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "label3";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(-23, -46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(264, 265);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 50);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // checkBox똥컴
            // 
            this.checkBox똥컴.AutoSize = true;
            this.checkBox똥컴.Location = new System.Drawing.Point(131, 63);
            this.checkBox똥컴.Name = "checkBox똥컴";
            this.checkBox똥컴.Size = new System.Drawing.Size(60, 16);
            this.checkBox똥컴.TabIndex = 8;
            this.checkBox똥컴.Text = "똥컴용";
            this.checkBox똥컴.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBox똥컴);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSPDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMPDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHPDelay)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button 제자리버튼;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button 제자리설정버튼;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private GroupBox groupBox3;
        private Button 림보설정버튼;
        private Button 림보시작버튼;
        internal NumericUpDown numericUpDownSPDelay;
        internal CheckBox checkBoxSPUse;
        internal CheckBox checkBoxSPRightClick;
        private Label label26;
        internal Button buttonSP;
        internal NumericUpDown numericUpDownMPDelay;
        internal CheckBox checkBoxMPUse;
        internal CheckBox checkBoxMPRightClick;
        private Label label25;
        internal Button buttonMP;
        internal NumericUpDown numericUpDownHPDelay;
        internal CheckBox checkBoxHPUse;
        internal CheckBox checkBoxHPRightClick;
        private Label label24;
        internal Button buttonHP;
        private Label label23;
        private Label label22;
        private Label label21;
        private Label label20;
        private Button 물약설정저장;
        private Label label1;
        private Label label2;
        private Label label3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private CheckBox checkBox똥컴;
    }
}

