namespace AutoClicker
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.startButton = new System.Windows.Forms.Button();
            this.clicksBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.leftRadio = new System.Windows.Forms.RadioButton();
            this.rightRadio = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 61);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(199, 28);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.toolTip1.SetToolTip(this.startButton, "Once clicked program will commence\r\nclicking after 5 seconds. Hit CTRL + ALT + S " +
        "\r\nto stop the clicker! or click the start button\r\nagain.");
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // clicksBox
            // 
            this.clicksBox.Location = new System.Drawing.Point(106, 12);
            this.clicksBox.Name = "clicksBox";
            this.clicksBox.Size = new System.Drawing.Size(105, 20);
            this.clicksBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.clicksBox, "Number of clicks to perform. If number is\r\nnegative then the mouse button will be" +
        "\r\nheld down for the inputed number of\r\nseconds. Can NOT be 0.");
            this.clicksBox.TextChanged += new System.EventHandler(this.clicksBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Number Of Clicks";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // leftRadio
            // 
            this.leftRadio.AutoSize = true;
            this.leftRadio.Checked = true;
            this.leftRadio.Location = new System.Drawing.Point(16, 38);
            this.leftRadio.Name = "leftRadio";
            this.leftRadio.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.leftRadio.Size = new System.Drawing.Size(69, 17);
            this.leftRadio.TabIndex = 5;
            this.leftRadio.TabStop = true;
            this.leftRadio.Text = "Left Click";
            this.leftRadio.UseVisualStyleBackColor = true;
            // 
            // rightRadio
            // 
            this.rightRadio.AutoSize = true;
            this.rightRadio.Location = new System.Drawing.Point(106, 38);
            this.rightRadio.Name = "rightRadio";
            this.rightRadio.Size = new System.Drawing.Size(76, 17);
            this.rightRadio.TabIndex = 6;
            this.rightRadio.Text = "Right Click";
            this.rightRadio.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F);
            this.button1.Location = new System.Drawing.Point(180, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(31, 17);
            this.button1.TabIndex = 7;
            this.button1.Text = "V1.1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 100);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rightRadio);
            this.Controls.Add(this.leftRadio);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clicksBox);
            this.Controls.Add(this.startButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Auto-Clicker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.TextBox clicksBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton leftRadio;
        private System.Windows.Forms.RadioButton rightRadio;
        private System.Windows.Forms.Button button1;
    }
}

