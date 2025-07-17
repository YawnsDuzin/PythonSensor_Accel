namespace Itlog
{
    partial class ItlogMain
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
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.txt_d_ref = new System.Windows.Forms.TextBox();
            this.label92 = new System.Windows.Forms.Label();
            this.label95 = new System.Windows.Forms.Label();
            this.label94 = new System.Windows.Forms.Label();
            this.label97 = new System.Windows.Forms.Label();
            this.label99 = new System.Windows.Forms.Label();
            this.lblAccelRMS_G = new System.Windows.Forms.Label();
            this.lblVelocityRMS_MMS = new System.Windows.Forms.Label();
            this.lblDBV = new System.Windows.Forms.Label();
            this.lblDBV_lv = new System.Windows.Forms.Label();
            this.btnSensorSet = new System.Windows.Forms.Button();
            this.btnSensorSet_br = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblRMS = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 41);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(200, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.Location = new System.Drawing.Point(12, 387);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(817, 229);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(218, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "포트 검색";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(299, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "열기";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.FormattingEnabled = true;
            this.cmbBaudRate.Items.AddRange(new object[] {
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400"});
            this.cmbBaudRate.Location = new System.Drawing.Point(12, 14);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(200, 20);
            this.cmbBaudRate.TabIndex = 0;
            // 
            // txt_d_ref
            // 
            this.txt_d_ref.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_d_ref.ForeColor = System.Drawing.Color.Red;
            this.txt_d_ref.Location = new System.Drawing.Point(613, 58);
            this.txt_d_ref.Name = "txt_d_ref";
            this.txt_d_ref.Size = new System.Drawing.Size(110, 20);
            this.txt_d_ref.TabIndex = 14;
            this.txt_d_ref.Text = "0.000001";
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Font = new System.Drawing.Font("굴림", 8.25F);
            this.label92.ForeColor = System.Drawing.Color.Black;
            this.label92.Location = new System.Drawing.Point(569, 61);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(38, 11);
            this.label92.TabIndex = 15;
            this.label92.Text = "기준값";
            // 
            // label95
            // 
            this.label95.BackColor = System.Drawing.Color.White;
            this.label95.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label95.ForeColor = System.Drawing.Color.Black;
            this.label95.Location = new System.Drawing.Point(21, 81);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(68, 21);
            this.label95.TabIndex = 22;
            this.label95.Text = "가속도";
            this.label95.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label94
            // 
            this.label94.BackColor = System.Drawing.Color.White;
            this.label94.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label94.ForeColor = System.Drawing.Color.Black;
            this.label94.Location = new System.Drawing.Point(21, 102);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(68, 21);
            this.label94.TabIndex = 22;
            this.label94.Text = "진동속도";
            this.label94.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label97
            // 
            this.label97.BackColor = System.Drawing.Color.White;
            this.label97.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label97.ForeColor = System.Drawing.Color.Black;
            this.label97.Location = new System.Drawing.Point(454, 58);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(68, 21);
            this.label97.TabIndex = 22;
            this.label97.Text = "dB(V)";
            this.label97.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label99
            // 
            this.label99.BackColor = System.Drawing.Color.White;
            this.label99.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label99.ForeColor = System.Drawing.Color.Black;
            this.label99.Location = new System.Drawing.Point(21, 147);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(68, 21);
            this.label99.TabIndex = 22;
            this.label99.Text = "진동등급";
            this.label99.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAccelRMS_G
            // 
            this.lblAccelRMS_G.BackColor = System.Drawing.Color.Black;
            this.lblAccelRMS_G.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAccelRMS_G.ForeColor = System.Drawing.Color.Transparent;
            this.lblAccelRMS_G.Location = new System.Drawing.Point(95, 81);
            this.lblAccelRMS_G.Name = "lblAccelRMS_G";
            this.lblAccelRMS_G.Size = new System.Drawing.Size(225, 21);
            this.lblAccelRMS_G.TabIndex = 22;
            this.lblAccelRMS_G.Text = " ";
            this.lblAccelRMS_G.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVelocityRMS_MMS
            // 
            this.lblVelocityRMS_MMS.BackColor = System.Drawing.Color.Black;
            this.lblVelocityRMS_MMS.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVelocityRMS_MMS.ForeColor = System.Drawing.Color.Transparent;
            this.lblVelocityRMS_MMS.Location = new System.Drawing.Point(95, 103);
            this.lblVelocityRMS_MMS.Name = "lblVelocityRMS_MMS";
            this.lblVelocityRMS_MMS.Size = new System.Drawing.Size(225, 21);
            this.lblVelocityRMS_MMS.TabIndex = 22;
            this.lblVelocityRMS_MMS.Text = " ";
            this.lblVelocityRMS_MMS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDBV
            // 
            this.lblDBV.BackColor = System.Drawing.Color.Black;
            this.lblDBV.Font = new System.Drawing.Font("굴림", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDBV.ForeColor = System.Drawing.Color.Red;
            this.lblDBV.Location = new System.Drawing.Point(455, 81);
            this.lblDBV.Name = "lblDBV";
            this.lblDBV.Size = new System.Drawing.Size(432, 114);
            this.lblDBV.TabIndex = 22;
            this.lblDBV.Text = "0000000";
            this.lblDBV.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDBV_lv
            // 
            this.lblDBV_lv.BackColor = System.Drawing.Color.Black;
            this.lblDBV_lv.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDBV_lv.ForeColor = System.Drawing.Color.Transparent;
            this.lblDBV_lv.Location = new System.Drawing.Point(95, 147);
            this.lblDBV_lv.Name = "lblDBV_lv";
            this.lblDBV_lv.Size = new System.Drawing.Size(225, 21);
            this.lblDBV_lv.TabIndex = 22;
            this.lblDBV_lv.Text = " ";
            this.lblDBV_lv.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSensorSet
            // 
            this.btnSensorSet.Location = new System.Drawing.Point(690, 17);
            this.btnSensorSet.Name = "btnSensorSet";
            this.btnSensorSet.Size = new System.Drawing.Size(104, 23);
            this.btnSensorSet.TabIndex = 23;
            this.btnSensorSet.Text = "센서 설정";
            this.btnSensorSet.UseVisualStyleBackColor = true;
            this.btnSensorSet.Click += new System.EventHandler(this.btnSensorSet_Click);
            // 
            // btnSensorSet_br
            // 
            this.btnSensorSet_br.Location = new System.Drawing.Point(580, 17);
            this.btnSensorSet_br.Name = "btnSensorSet_br";
            this.btnSensorSet_br.Size = new System.Drawing.Size(104, 23);
            this.btnSensorSet_br.TabIndex = 23;
            this.btnSensorSet_br.Text = "BaudRate 설정";
            this.btnSensorSet_br.UseVisualStyleBackColor = true;
            this.btnSensorSet_br.Click += new System.EventHandler(this.btnSensorSet_br_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(454, 218);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 21);
            this.label1.TabIndex = 22;
            this.label1.Text = "RMS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRMS
            // 
            this.lblRMS.BackColor = System.Drawing.Color.Black;
            this.lblRMS.Font = new System.Drawing.Font("굴림", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRMS.ForeColor = System.Drawing.Color.Red;
            this.lblRMS.Location = new System.Drawing.Point(455, 241);
            this.lblRMS.Name = "lblRMS";
            this.lblRMS.Size = new System.Drawing.Size(649, 82);
            this.lblRMS.TabIndex = 22;
            this.lblRMS.Text = "0000000";
            this.lblRMS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ItlogMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 626);
            this.Controls.Add(this.btnSensorSet_br);
            this.Controls.Add(this.btnSensorSet);
            this.Controls.Add(this.lblDBV_lv);
            this.Controls.Add(this.label99);
            this.Controls.Add(this.lblRMS);
            this.Controls.Add(this.lblDBV);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVelocityRMS_MMS);
            this.Controls.Add(this.label97);
            this.Controls.Add(this.lblAccelRMS_G);
            this.Controls.Add(this.label94);
            this.Controls.Add(this.label95);
            this.Controls.Add(this.label92);
            this.Controls.Add(this.txt_d_ref);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cmbBaudRate);
            this.Controls.Add(this.comboBox1);
            this.Name = "ItlogMain";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItlogMain_FormClosing);
            this.Load += new System.EventHandler(this.ItlogMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.TextBox txt_d_ref;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.Label label95;
        private System.Windows.Forms.Label label94;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.Label label99;
        private System.Windows.Forms.Label lblAccelRMS_G;
        private System.Windows.Forms.Label lblVelocityRMS_MMS;
        private System.Windows.Forms.Label lblDBV;
        private System.Windows.Forms.Label lblDBV_lv;
        private System.Windows.Forms.Button btnSensorSet;
        private System.Windows.Forms.Button btnSensorSet_br;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRMS;
    }
}

