namespace 빡자사
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.매크로종료 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // 매크로종료
            // 
            this.매크로종료.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.매크로종료.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.매크로종료.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.매크로종료.Location = new System.Drawing.Point(13, 13);
            this.매크로종료.Name = "매크로종료";
            this.매크로종료.Size = new System.Drawing.Size(75, 23);
            this.매크로종료.TabIndex = 0;
            this.매크로종료.Text = "일시 정지";
            this.매크로종료.UseVisualStyleBackColor = false;
            this.매크로종료.SizeChanged += new System.EventHandler(this.Form2_SizeChanged);
            this.매크로종료.Click += new System.EventHandler(this.매크로종료_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(128, 40);
            this.Controls.Add(this.매크로종료);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form2";
            this.Text = "Form2";
            this.TopMost = true;
            this.SizeChanged += new System.EventHandler(this.Form2_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button 매크로종료;
    }
}