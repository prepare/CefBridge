﻿namespace CefBridgeTest
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
            this.button7 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.cmdReloadIgnoreCache = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cefWebBrowser1 = new LayoutFarm.CefBridge.CefWebBrowserControl();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(12, 55);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(132, 37);
            this.button7.TabIndex = 8;
            this.button7.Text = "Html5 Test";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 151);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 37);
            this.button1.TabIndex = 9;
            this.button1.Text = "ExecJs";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 194);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 37);
            this.button2.TabIndex = 10;
            this.button2.Text = "PostData";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 237);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(132, 37);
            this.button3.TabIndex = 11;
            this.button3.Text = "GetText";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 280);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(132, 37);
            this.button4.TabIndex = 12;
            this.button4.Text = "GetSource";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(12, 488);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(132, 37);
            this.button8.TabIndex = 15;
            this.button8.Text = "DevTools";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(12, 12);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(132, 37);
            this.button9.TabIndex = 16;
            this.button9.Text = "NewBwWindow";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(150, 23);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(56, 37);
            this.button10.TabIndex = 17;
            this.button10.Text = "Back";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(212, 23);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(56, 37);
            this.button11.TabIndex = 18;
            this.button11.Text = "Forward";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(339, 23);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(59, 37);
            this.button12.TabIndex = 19;
            this.button12.Text = "Reload";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(274, 23);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(59, 37);
            this.button13.TabIndex = 20;
            this.button13.Text = "Stop";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // cmdReloadIgnoreCache
            // 
            this.cmdReloadIgnoreCache.Location = new System.Drawing.Point(404, 23);
            this.cmdReloadIgnoreCache.Name = "cmdReloadIgnoreCache";
            this.cmdReloadIgnoreCache.Size = new System.Drawing.Size(119, 37);
            this.cmdReloadIgnoreCache.TabIndex = 21;
            this.cmdReloadIgnoreCache.Text = "ReloadIgnoreCache";
            this.cmdReloadIgnoreCache.UseVisualStyleBackColor = true;
            this.cmdReloadIgnoreCache.Click += new System.EventHandler(this.cmdReloadIgnoreCache_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(12, 98);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(132, 37);
            this.button14.TabIndex = 22;
            this.button14.Text = "YouTube";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(150, 66);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cefWebBrowser1);
            this.splitContainer1.Size = new System.Drawing.Size(922, 668);
            this.splitContainer1.SplitterDistance = 61;
            this.splitContainer1.TabIndex = 23;
            // 
            // cefWebBrowser1
            // 
            this.cefWebBrowser1.BackColor = System.Drawing.Color.White;
            this.cefWebBrowser1.CefBrowserListener = null;
            this.cefWebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cefWebBrowser1.Location = new System.Drawing.Point(0, 0);
            this.cefWebBrowser1.Name = "cefWebBrowser1";
            this.cefWebBrowser1.Size = new System.Drawing.Size(857, 668);
            this.cefWebBrowser1.TabIndex = 7;
            this.cefWebBrowser1.Text = "cefWebBrowser1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 762);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.cmdReloadIgnoreCache);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button7);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button cmdReloadIgnoreCache;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private LayoutFarm.CefBridge.CefWebBrowserControl cefWebBrowser1;
    }
}

