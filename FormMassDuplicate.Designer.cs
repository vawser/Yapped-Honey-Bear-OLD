namespace Yapped
{
    partial class FormMassDuplicate
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
            this.startIDInput = new System.Windows.Forms.TextBox();
            this.endIDInput = new System.Windows.Forms.TextBox();
            this.startRowIDLabel = new System.Windows.Forms.Label();
            this.endRowIDLabel = new System.Windows.Forms.Label();
            this.duplicateRowsBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.stepValueTextbox = new System.Windows.Forms.TextBox();
            this.stepValueLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startIDInput
            // 
            this.startIDInput.Location = new System.Drawing.Point(12, 28);
            this.startIDInput.Name = "startIDInput";
            this.startIDInput.Size = new System.Drawing.Size(68, 20);
            this.startIDInput.TabIndex = 0;
            // 
            // endIDInput
            // 
            this.endIDInput.Location = new System.Drawing.Point(12, 70);
            this.endIDInput.Name = "endIDInput";
            this.endIDInput.Size = new System.Drawing.Size(68, 20);
            this.endIDInput.TabIndex = 1;
            // 
            // startRowIDLabel
            // 
            this.startRowIDLabel.AutoSize = true;
            this.startRowIDLabel.Location = new System.Drawing.Point(12, 12);
            this.startRowIDLabel.Name = "startRowIDLabel";
            this.startRowIDLabel.Size = new System.Drawing.Size(68, 13);
            this.startRowIDLabel.TabIndex = 2;
            this.startRowIDLabel.Text = "Start Row ID";
            // 
            // endRowIDLabel
            // 
            this.endRowIDLabel.AutoSize = true;
            this.endRowIDLabel.Location = new System.Drawing.Point(12, 54);
            this.endRowIDLabel.Name = "endRowIDLabel";
            this.endRowIDLabel.Size = new System.Drawing.Size(65, 13);
            this.endRowIDLabel.TabIndex = 3;
            this.endRowIDLabel.Text = "End Row ID";
            // 
            // duplicateRowsBtn
            // 
            this.duplicateRowsBtn.Location = new System.Drawing.Point(86, 28);
            this.duplicateRowsBtn.Name = "duplicateRowsBtn";
            this.duplicateRowsBtn.Size = new System.Drawing.Size(75, 23);
            this.duplicateRowsBtn.TabIndex = 4;
            this.duplicateRowsBtn.Text = "Duplicate";
            this.duplicateRowsBtn.UseVisualStyleBackColor = true;
            this.duplicateRowsBtn.Click += new System.EventHandler(this.duplicateRowsBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(86, 70);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 5;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // stepValueTextbox
            // 
            this.stepValueTextbox.Location = new System.Drawing.Point(12, 114);
            this.stepValueTextbox.Name = "stepValueTextbox";
            this.stepValueTextbox.Size = new System.Drawing.Size(70, 20);
            this.stepValueTextbox.TabIndex = 6;
            // 
            // stepValueLabel
            // 
            this.stepValueLabel.AutoSize = true;
            this.stepValueLabel.Location = new System.Drawing.Point(18, 98);
            this.stepValueLabel.Name = "stepValueLabel";
            this.stepValueLabel.Size = new System.Drawing.Size(59, 13);
            this.stepValueLabel.TabIndex = 7;
            this.stepValueLabel.Text = "Step Value";
            // 
            // FormMassDuplicate
            // 
            this.AcceptButton = this.duplicateRowsBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(178, 144);
            this.Controls.Add(this.stepValueLabel);
            this.Controls.Add(this.stepValueTextbox);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.duplicateRowsBtn);
            this.Controls.Add(this.endRowIDLabel);
            this.Controls.Add(this.startRowIDLabel);
            this.Controls.Add(this.endIDInput);
            this.Controls.Add(this.startIDInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMassDuplicate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mass Duplicate";
            this.Load += new System.EventHandler(this.FormMassDuplicate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox startIDInput;
        private System.Windows.Forms.TextBox endIDInput;
        private System.Windows.Forms.Label startRowIDLabel;
        private System.Windows.Forms.Label endRowIDLabel;
        private System.Windows.Forms.Button duplicateRowsBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.TextBox stepValueTextbox;
        private System.Windows.Forms.Label stepValueLabel;
    }
}