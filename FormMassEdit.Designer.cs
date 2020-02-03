namespace Yapped
{
    partial class FormMassEdit
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
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fieldSelectionBox = new System.Windows.Forms.ComboBox();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.multiplyCheckbox = new System.Windows.Forms.CheckBox();
            this.startIdBox = new System.Windows.Forms.TextBox();
            this.endIdBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // acceptButton
            // 
            this.acceptButton.Location = new System.Drawing.Point(12, 64);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(75, 23);
            this.acceptButton.TabIndex = 0;
            this.acceptButton.Text = "Accept";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(93, 65);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // fieldSelectionBox
            // 
            this.fieldSelectionBox.AccessibleDescription = "Field to Edit";
            this.fieldSelectionBox.FormattingEnabled = true;
            this.fieldSelectionBox.Location = new System.Drawing.Point(12, 37);
            this.fieldSelectionBox.Name = "fieldSelectionBox";
            this.fieldSelectionBox.Size = new System.Drawing.Size(121, 21);
            this.fieldSelectionBox.TabIndex = 2;
            this.fieldSelectionBox.SelectedIndexChanged += new System.EventHandler(this.fieldSelectionBox_SelectedIndexChanged);
            // 
            // valueBox
            // 
            this.valueBox.AccessibleDescription = "Value to Edit With";
            this.valueBox.Location = new System.Drawing.Point(139, 37);
            this.valueBox.Name = "valueBox";
            this.valueBox.Size = new System.Drawing.Size(121, 20);
            this.valueBox.TabIndex = 4;
            this.valueBox.TextChanged += new System.EventHandler(this.valueBox_TextChanged);
            // 
            // multiplyCheckbox
            // 
            this.multiplyCheckbox.AutoSize = true;
            this.multiplyCheckbox.Location = new System.Drawing.Point(266, 37);
            this.multiplyCheckbox.Name = "multiplyCheckbox";
            this.multiplyCheckbox.Size = new System.Drawing.Size(61, 17);
            this.multiplyCheckbox.TabIndex = 5;
            this.multiplyCheckbox.Text = "Multiply";
            this.multiplyCheckbox.UseVisualStyleBackColor = true;
            this.multiplyCheckbox.CheckedChanged += new System.EventHandler(this.multiplyCheckbox_CheckedChanged);
            // 
            // startIdBox
            // 
            this.startIdBox.AccessibleDescription = "Starting Row ID";
            this.startIdBox.Location = new System.Drawing.Point(12, 11);
            this.startIdBox.Name = "startIdBox";
            this.startIdBox.Size = new System.Drawing.Size(121, 20);
            this.startIdBox.TabIndex = 6;
            // 
            // endIdBox
            // 
            this.endIdBox.AccessibleDescription = "Ending Row ID";
            this.endIdBox.Location = new System.Drawing.Point(139, 11);
            this.endIdBox.Name = "endIdBox";
            this.endIdBox.Size = new System.Drawing.Size(121, 20);
            this.endIdBox.TabIndex = 7;
            // 
            // FormMassEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 99);
            this.Controls.Add(this.endIdBox);
            this.Controls.Add(this.startIdBox);
            this.Controls.Add(this.multiplyCheckbox);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.fieldSelectionBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Name = "FormMassEdit";
            this.Text = "Mass Edit";
            this.Load += new System.EventHandler(this.FormMassEdit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox fieldSelectionBox;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.CheckBox multiplyCheckbox;
        private System.Windows.Forms.TextBox startIdBox;
        private System.Windows.Forms.TextBox endIdBox;
    }
}