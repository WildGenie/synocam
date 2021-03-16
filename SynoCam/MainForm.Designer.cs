using System.Windows.Forms;

namespace SynoCam
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.twoSecondsRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thirtySecondsRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneMinuteRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.twoMinutesRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fourMinutesRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelControl = new System.Windows.Forms.Panel();
            this.noFocusCueButton1 = new SynoCam.NoFocusCueButton();
            this.buttonClose = new SynoCam.NoFocusCueButton();
            this.buttonMinimize = new SynoCam.NoFocusCueButton();
            this.labelLoading = new System.Windows.Forms.Label();
            this.contextMenu.SuspendLayout();
            this.panelControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshRateToolStripMenuItem,
            this.viewEventsToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(140, 48);
            // 
            // refreshRateToolStripMenuItem
            // 
            this.refreshRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.twoSecondsRefreshToolStripMenuItem,
            this.thirtySecondsRefreshToolStripMenuItem,
            this.oneMinuteRefreshToolStripMenuItem,
            this.twoMinutesRefreshToolStripMenuItem,
            this.fourMinutesRefreshToolStripMenuItem});
            this.refreshRateToolStripMenuItem.Name = "refreshRateToolStripMenuItem";
            this.refreshRateToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.refreshRateToolStripMenuItem.Text = "Refresh Rate";
            // 
            // twoSecondsRefreshToolStripMenuItem
            // 
            this.twoSecondsRefreshToolStripMenuItem.Name = "twoSecondsRefreshToolStripMenuItem";
            this.twoSecondsRefreshToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.twoSecondsRefreshToolStripMenuItem.Text = "2 seconds";
            this.twoSecondsRefreshToolStripMenuItem.Click += new System.EventHandler(this.TwoSecondsToolStripMenuItemClick);
            // 
            // thirtySecondsRefreshToolStripMenuItem
            // 
            this.thirtySecondsRefreshToolStripMenuItem.Name = "thirtySecondsRefreshToolStripMenuItem";
            this.thirtySecondsRefreshToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.thirtySecondsRefreshToolStripMenuItem.Text = "30 seconds";
            this.thirtySecondsRefreshToolStripMenuItem.Click += new System.EventHandler(this.ThirtySecondsToolStripMenuItemClick);
            // 
            // oneMinuteRefreshToolStripMenuItem
            // 
            this.oneMinuteRefreshToolStripMenuItem.Name = "oneMinuteRefreshToolStripMenuItem";
            this.oneMinuteRefreshToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.oneMinuteRefreshToolStripMenuItem.Text = "1 minute";
            this.oneMinuteRefreshToolStripMenuItem.Click += new System.EventHandler(this.OneMinuteToolStripMenuItemClick);
            // 
            // twoMinutesRefreshToolStripMenuItem
            // 
            this.twoMinutesRefreshToolStripMenuItem.Name = "twoMinutesRefreshToolStripMenuItem";
            this.twoMinutesRefreshToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.twoMinutesRefreshToolStripMenuItem.Text = "2 minutes";
            this.twoMinutesRefreshToolStripMenuItem.Click += new System.EventHandler(this.TwoMinutesToolStripMenuItemClick);
            // 
            // fourMinutesRefreshToolStripMenuItem
            // 
            this.fourMinutesRefreshToolStripMenuItem.Name = "fourMinutesRefreshToolStripMenuItem";
            this.fourMinutesRefreshToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.fourMinutesRefreshToolStripMenuItem.Text = "4 minutes";
            this.fourMinutesRefreshToolStripMenuItem.Click += new System.EventHandler(this.FourMinutesToolStripMenuItemClick);
            // 
            // viewEventsToolStripMenuItem
            // 
            this.viewEventsToolStripMenuItem.Name = "viewEventsToolStripMenuItem";
            this.viewEventsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.viewEventsToolStripMenuItem.Text = "View Events";
            this.viewEventsToolStripMenuItem.Click += new System.EventHandler(this.viewEventsToolStripMenuItem_Click);
            // 
            // panelControl
            // 
            this.panelControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panelControl.Controls.Add(this.noFocusCueButton1);
            this.panelControl.Controls.Add(this.buttonClose);
            this.panelControl.Controls.Add(this.buttonMinimize);
            this.panelControl.Location = new System.Drawing.Point(673, -2);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(14, 423);
            this.panelControl.TabIndex = 3;
            // 
            // noFocusCueButton1
            // 
            this.noFocusCueButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.noFocusCueButton1.BackColor = System.Drawing.Color.Black;
            this.noFocusCueButton1.FlatAppearance.BorderSize = 0;
            this.noFocusCueButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.noFocusCueButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.noFocusCueButton1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noFocusCueButton1.ForeColor = System.Drawing.Color.White;
            this.noFocusCueButton1.Location = new System.Drawing.Point(0, 48);
            this.noFocusCueButton1.Name = "noFocusCueButton1";
            this.noFocusCueButton1.Size = new System.Drawing.Size(14, 25);
            this.noFocusCueButton1.TabIndex = 3;
            this.noFocusCueButton1.TabStop = false;
            this.noFocusCueButton1.Text = "R";
            this.noFocusCueButton1.UseVisualStyleBackColor = false;
            this.noFocusCueButton1.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.BackColor = System.Drawing.Color.Black;
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Location = new System.Drawing.Point(0, 2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(14, 25);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.TabStop = false;
            this.buttonClose.Text = "x";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // buttonMinimize
            // 
            this.buttonMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMinimize.BackColor = System.Drawing.Color.Black;
            this.buttonMinimize.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonMinimize.FlatAppearance.BorderSize = 0;
            this.buttonMinimize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.buttonMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMinimize.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMinimize.ForeColor = System.Drawing.Color.White;
            this.buttonMinimize.Location = new System.Drawing.Point(0, 25);
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.Size = new System.Drawing.Size(14, 23);
            this.buttonMinimize.TabIndex = 2;
            this.buttonMinimize.TabStop = false;
            this.buttonMinimize.Text = "-";
            this.buttonMinimize.UseVisualStyleBackColor = false;
            this.buttonMinimize.Click += new System.EventHandler(this.MinimizeButtonClick);
            // 
            // labelLoading
            // 
            this.labelLoading.ForeColor = System.Drawing.Color.White;
            this.labelLoading.Location = new System.Drawing.Point(297, 186);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(111, 15);
            this.labelLoading.TabIndex = 4;
            this.labelLoading.Text = "Loading, please wait..";
            this.labelLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(687, 420);
            this.ContextMenuStrip = this.contextMenu;
            this.ControlBox = false;
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.panelControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thuis";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DoubleClick += new System.EventHandler(this.MainFormDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainFormMouseDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenu.ResumeLayout(false);
            this.panelControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private SynoCam.NoFocusCueButton buttonClose;
        private SynoCam.NoFocusCueButton buttonMinimize;
        private Panel panelControl;
        private Label labelLoading;
        private SynoCam.NoFocusCueButton noFocusCueButton1;
        private ToolStripMenuItem refreshRateToolStripMenuItem;
        private ToolStripMenuItem twoSecondsRefreshToolStripMenuItem;
        private ToolStripMenuItem thirtySecondsRefreshToolStripMenuItem;
        private ToolStripMenuItem oneMinuteRefreshToolStripMenuItem;
        private ToolStripMenuItem twoMinutesRefreshToolStripMenuItem;
        private ToolStripMenuItem fourMinutesRefreshToolStripMenuItem;
        private ToolStripMenuItem viewEventsToolStripMenuItem;
    }
}

