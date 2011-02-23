namespace TwitterTester
{
    partial class Form1
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
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.btnGetRequestToken = new System.Windows.Forms.Button();
            this.btnGetAccessToken = new System.Windows.Forms.Button();
            this.txtRequestToken = new System.Windows.Forms.TextBox();
            this.txtAccessToken = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtAccessTokenSecret = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(416, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(727, 617);
            this.webBrowser.TabIndex = 0;
            // 
            // btnGetRequestToken
            // 
            this.btnGetRequestToken.Location = new System.Drawing.Point(35, 26);
            this.btnGetRequestToken.Name = "btnGetRequestToken";
            this.btnGetRequestToken.Size = new System.Drawing.Size(126, 44);
            this.btnGetRequestToken.TabIndex = 1;
            this.btnGetRequestToken.Text = "Get Request Token";
            this.btnGetRequestToken.UseVisualStyleBackColor = true;
            this.btnGetRequestToken.Click += new System.EventHandler(this.btnGetRequestToken_Click);
            // 
            // btnGetAccessToken
            // 
            this.btnGetAccessToken.Location = new System.Drawing.Point(51, 218);
            this.btnGetAccessToken.Name = "btnGetAccessToken";
            this.btnGetAccessToken.Size = new System.Drawing.Size(126, 44);
            this.btnGetAccessToken.TabIndex = 2;
            this.btnGetAccessToken.Text = "Get Access Token";
            this.btnGetAccessToken.UseVisualStyleBackColor = true;
            this.btnGetAccessToken.Click += new System.EventHandler(this.btnGetAccessToken_Click);
            // 
            // txtRequestToken
            // 
            this.txtRequestToken.Location = new System.Drawing.Point(35, 112);
            this.txtRequestToken.Name = "txtRequestToken";
            this.txtRequestToken.Size = new System.Drawing.Size(357, 20);
            this.txtRequestToken.TabIndex = 3;
            // 
            // txtAccessToken
            // 
            this.txtAccessToken.Location = new System.Drawing.Point(35, 298);
            this.txtAccessToken.Name = "txtAccessToken";
            this.txtAccessToken.Size = new System.Drawing.Size(357, 20);
            this.txtAccessToken.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Request Token:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Access Token:";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(35, 371);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(357, 237);
            this.txtLog.TabIndex = 7;
            // 
            // txtAccessTokenSecret
            // 
            this.txtAccessTokenSecret.Location = new System.Drawing.Point(35, 326);
            this.txtAccessTokenSecret.Name = "txtAccessTokenSecret";
            this.txtAccessTokenSecret.Size = new System.Drawing.Size(357, 20);
            this.txtAccessTokenSecret.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1144, 620);
            this.Controls.Add(this.txtAccessTokenSecret);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAccessToken);
            this.Controls.Add(this.txtRequestToken);
            this.Controls.Add(this.btnGetAccessToken);
            this.Controls.Add(this.btnGetRequestToken);
            this.Controls.Add(this.webBrowser);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button btnGetRequestToken;
        private System.Windows.Forms.Button btnGetAccessToken;
        private System.Windows.Forms.TextBox txtRequestToken;
        private System.Windows.Forms.TextBox txtAccessToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtAccessTokenSecret;
    }
}

