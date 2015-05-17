namespace Willowsoft.WillowLib.WinForm
{
    partial class ErrorDisplayForm
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
            this.errorList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // errorList
            // 
            this.errorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.errorList.FormattingEnabled = true;
            this.errorList.Location = new System.Drawing.Point(12, 12);
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(537, 212);
            this.errorList.TabIndex = 0;
            // 
            // ErrorDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 238);
            this.Controls.Add(this.errorList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDisplayForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Validation Errors";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox errorList;
    }
}