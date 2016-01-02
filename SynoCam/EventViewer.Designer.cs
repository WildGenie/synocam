using System;

namespace SynoCam
{
    partial class EventViewer
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
            this.eventView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.snapshotBox = new System.Windows.Forms.PictureBox();
            this.streamVideoButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.snapshotBox)).BeginInit();
            this.SuspendLayout();
            // 
            // eventView
            // 
            this.eventView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.eventView.FullRowSelect = true;
            this.eventView.GridLines = true;
            this.eventView.Location = new System.Drawing.Point(12, 12);
            this.eventView.MultiSelect = false;
            this.eventView.Name = "eventView";
            this.eventView.Size = new System.Drawing.Size(402, 300);
            this.eventView.TabIndex = 0;
            this.eventView.UseCompatibleStateImageBehavior = false;
            this.eventView.SelectedIndexChanged += new System.EventHandler(this.EventViewSelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Start";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Camera name";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Reason";
            this.columnHeader3.Width = 100;
            // 
            // snapshotBox
            // 
            this.snapshotBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.snapshotBox.Location = new System.Drawing.Point(420, 12);
            this.snapshotBox.Name = "snapshotBox";
            this.snapshotBox.Size = new System.Drawing.Size(229, 192);
            this.snapshotBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.snapshotBox.TabIndex = 1;
            this.snapshotBox.TabStop = false;
            // 
            // streamVideoButton
            // 
            this.streamVideoButton.Location = new System.Drawing.Point(420, 210);
            this.streamVideoButton.Name = "streamVideoButton";
            this.streamVideoButton.Size = new System.Drawing.Size(87, 23);
            this.streamVideoButton.TabIndex = 2;
            this.streamVideoButton.Text = "Play video";
            this.streamVideoButton.UseVisualStyleBackColor = true;
            this.streamVideoButton.Visible = false;
            this.streamVideoButton.Click += new System.EventHandler(this.StreamVideoButtonClick);
            // 
            // EventViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 324);
            this.Controls.Add(this.streamVideoButton);
            this.Controls.Add(this.snapshotBox);
            this.Controls.Add(this.eventView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EventViewer";
            this.Text = "Event Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.snapshotBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView eventView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.PictureBox snapshotBox;
        private System.Windows.Forms.Button streamVideoButton;

    }
}