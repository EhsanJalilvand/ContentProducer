namespace BrowserForm
{
    partial class SmartCrawlerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            browser = new CefSharp.WinForms.ChromiumWebBrowser();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // browser
            // 
            browser.ActivateBrowserOnCreation = false;
            browser.Dock = DockStyle.Fill;
            browser.Location = new Point(0, 0);
            browser.Name = "browser";
            browser.Size = new Size(800, 450);
            browser.TabIndex = 0;
            browser.LoadingStateChanged += browser_LoadingStateChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(browser);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 450);
            panel1.TabIndex = 1;
            // 
            // SmartCrawlerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            KeyPreview = true;
            Name = "SmartCrawlerForm";
            Text = "SmartCrawler";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private CefSharp.WinForms.ChromiumWebBrowser browser;
        private Panel panel1;
    }
}