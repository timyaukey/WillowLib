namespace Willowsoft.WillowLib.CodeGen
{
    partial class CodeViewer
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
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.cboEntity = new System.Windows.Forms.ComboBox();
            this.btnGenerateClass = new System.Windows.Forms.Button();
            this.lblEntity = new System.Windows.Forms.Label();
            this.btnGenerateTable = new System.Windows.Forms.Button();
            this.btnGenerateProc = new System.Windows.Forms.Button();
            this.btnGenerateConversion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(12, 12);
            this.txtOutput.MaxLength = 20000000;
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(701, 495);
            this.txtOutput.TabIndex = 0;
            // 
            // cboEntity
            // 
            this.cboEntity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEntity.FormattingEnabled = true;
            this.cboEntity.Location = new System.Drawing.Point(403, 544);
            this.cboEntity.Name = "cboEntity";
            this.cboEntity.Size = new System.Drawing.Size(162, 21);
            this.cboEntity.TabIndex = 3;
            // 
            // btnGenerateClass
            // 
            this.btnGenerateClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateClass.Location = new System.Drawing.Point(571, 542);
            this.btnGenerateClass.Name = "btnGenerateClass";
            this.btnGenerateClass.Size = new System.Drawing.Size(142, 23);
            this.btnGenerateClass.TabIndex = 4;
            this.btnGenerateClass.Text = "Generate Partial Class";
            this.btnGenerateClass.UseVisualStyleBackColor = true;
            this.btnGenerateClass.Click += new System.EventHandler(this.btnGenerateClass_Click);
            // 
            // lblEntity
            // 
            this.lblEntity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(361, 547);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(36, 13);
            this.lblEntity.TabIndex = 2;
            this.lblEntity.Text = "Entity:";
            // 
            // btnGenerateTable
            // 
            this.btnGenerateTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerateTable.Location = new System.Drawing.Point(12, 513);
            this.btnGenerateTable.Name = "btnGenerateTable";
            this.btnGenerateTable.Size = new System.Drawing.Size(161, 23);
            this.btnGenerateTable.TabIndex = 1;
            this.btnGenerateTable.Text = "Generate Create Table Script";
            this.btnGenerateTable.UseVisualStyleBackColor = true;
            this.btnGenerateTable.Click += new System.EventHandler(this.btnGenerateTable_Click);
            // 
            // btnGenerateProc
            // 
            this.btnGenerateProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerateProc.Location = new System.Drawing.Point(179, 542);
            this.btnGenerateProc.Name = "btnGenerateProc";
            this.btnGenerateProc.Size = new System.Drawing.Size(134, 23);
            this.btnGenerateProc.TabIndex = 5;
            this.btnGenerateProc.Text = "Generate Proc Script";
            this.btnGenerateProc.UseVisualStyleBackColor = true;
            this.btnGenerateProc.Click += new System.EventHandler(this.btnGenerate_Proc);
            // 
            // btnGenerateConversion
            // 
            this.btnGenerateConversion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerateConversion.Location = new System.Drawing.Point(12, 542);
            this.btnGenerateConversion.Name = "btnGenerateConversion";
            this.btnGenerateConversion.Size = new System.Drawing.Size(161, 23);
            this.btnGenerateConversion.TabIndex = 6;
            this.btnGenerateConversion.Text = "Generate Conversion Script";
            this.btnGenerateConversion.UseVisualStyleBackColor = true;
            this.btnGenerateConversion.Click += new System.EventHandler(this.btnGenerateConversion_Click);
            // 
            // CodeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 577);
            this.Controls.Add(this.btnGenerateConversion);
            this.Controls.Add(this.btnGenerateProc);
            this.Controls.Add(this.btnGenerateTable);
            this.Controls.Add(this.lblEntity);
            this.Controls.Add(this.btnGenerateClass);
            this.Controls.Add(this.cboEntity);
            this.Controls.Add(this.txtOutput);
            this.Name = "CodeViewer";
            this.Text = "C# and SQL Entity Code Generator";
            this.Load += new System.EventHandler(this.CodeViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ComboBox cboEntity;
        private System.Windows.Forms.Button btnGenerateClass;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.Button btnGenerateTable;
        private System.Windows.Forms.Button btnGenerateProc;
        private System.Windows.Forms.Button btnGenerateConversion;
    }
}

