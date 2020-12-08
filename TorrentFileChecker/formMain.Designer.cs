
namespace TorrentFileChecker
{
    partial class formMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbxTorrent = new System.Windows.Forms.TextBox();
            this.btnTorrent = new System.Windows.Forms.Button();
            this.btnFile = new System.Windows.Forms.Button();
            this.tbxFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxMD5 = new System.Windows.Forms.TextBox();
            this.tbxSHA1 = new System.Windows.Forms.TextBox();
            this.tbxSHA256 = new System.Windows.Forms.TextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.pbMain = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "种子文件：";
            // 
            // tbxTorrent
            // 
            this.tbxTorrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxTorrent.BackColor = System.Drawing.SystemColors.Window;
            this.tbxTorrent.Location = new System.Drawing.Point(78, 12);
            this.tbxTorrent.Name = "tbxTorrent";
            this.tbxTorrent.ReadOnly = true;
            this.tbxTorrent.Size = new System.Drawing.Size(329, 23);
            this.tbxTorrent.TabIndex = 1;
            // 
            // btnTorrent
            // 
            this.btnTorrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTorrent.Location = new System.Drawing.Point(413, 11);
            this.btnTorrent.Name = "btnTorrent";
            this.btnTorrent.Size = new System.Drawing.Size(68, 25);
            this.btnTorrent.TabIndex = 2;
            this.btnTorrent.Text = "浏览";
            this.btnTorrent.UseVisualStyleBackColor = true;
            this.btnTorrent.Click += new System.EventHandler(this.btnTorrent_Click);
            // 
            // btnFile
            // 
            this.btnFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFile.Location = new System.Drawing.Point(413, 40);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(68, 25);
            this.btnFile.TabIndex = 5;
            this.btnFile.Text = "浏览";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // tbxFile
            // 
            this.tbxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxFile.BackColor = System.Drawing.SystemColors.Window;
            this.tbxFile.Location = new System.Drawing.Point(78, 41);
            this.tbxFile.Name = "tbxFile";
            this.tbxFile.ReadOnly = true;
            this.tbxFile.Size = new System.Drawing.Size(329, 23);
            this.tbxFile.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "下载文件：";
            // 
            // tbxMD5
            // 
            this.tbxMD5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxMD5.BackColor = System.Drawing.SystemColors.Window;
            this.tbxMD5.Location = new System.Drawing.Point(78, 70);
            this.tbxMD5.Name = "tbxMD5";
            this.tbxMD5.ReadOnly = true;
            this.tbxMD5.Size = new System.Drawing.Size(402, 23);
            this.tbxMD5.TabIndex = 9;
            // 
            // tbxSHA1
            // 
            this.tbxSHA1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxSHA1.BackColor = System.Drawing.SystemColors.Window;
            this.tbxSHA1.Location = new System.Drawing.Point(78, 99);
            this.tbxSHA1.Name = "tbxSHA1";
            this.tbxSHA1.ReadOnly = true;
            this.tbxSHA1.Size = new System.Drawing.Size(402, 23);
            this.tbxSHA1.TabIndex = 10;
            // 
            // tbxSHA256
            // 
            this.tbxSHA256.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxSHA256.BackColor = System.Drawing.SystemColors.Window;
            this.tbxSHA256.Location = new System.Drawing.Point(78, 128);
            this.tbxSHA256.Name = "tbxSHA256";
            this.tbxSHA256.ReadOnly = true;
            this.tbxSHA256.Size = new System.Drawing.Size(402, 23);
            this.tbxSHA256.TabIndex = 11;
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheck.Location = new System.Drawing.Point(413, 156);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(68, 25);
            this.btnCheck.TabIndex = 16;
            this.btnCheck.Text = "校验";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // pbMain
            // 
            this.pbMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMain.Location = new System.Drawing.Point(12, 157);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(395, 23);
            this.pbMain.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 19;
            this.label4.Text = "MD5：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "SHA1：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 17);
            this.label6.TabIndex = 21;
            this.label6.Text = "SHA256：";
            // 
            // formMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(492, 191);
            this.Controls.Add(this.pbMain);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.tbxSHA256);
            this.Controls.Add(this.tbxSHA1);
            this.Controls.Add(this.tbxMD5);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.tbxFile);
            this.Controls.Add(this.btnTorrent);
            this.Controls.Add(this.tbxTorrent);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(2048, 229);
            this.MinimumSize = new System.Drawing.Size(500, 229);
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "种子文件校验工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxTorrent;
        private System.Windows.Forms.Button btnTorrent;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.TextBox tbxFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxMD5;
        private System.Windows.Forms.TextBox tbxSHA1;
        private System.Windows.Forms.TextBox tbxSHA256;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Panel pbMain;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}