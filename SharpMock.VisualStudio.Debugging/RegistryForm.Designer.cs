namespace SharpMock.VisualStudio.Debugging
{
    partial class RegistryForm
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
            this.replaceableReferenceTypes = new System.Windows.Forms.ListBox();
            this.registeredReferences = new System.Windows.Forms.DataGridView();
            this.debugDisplay = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.registeredReferences)).BeginInit();
            this.SuspendLayout();
            // 
            // replaceableReferenceTypes
            // 
            this.replaceableReferenceTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceableReferenceTypes.FormattingEnabled = true;
            this.replaceableReferenceTypes.Location = new System.Drawing.Point(13, 13);
            this.replaceableReferenceTypes.Name = "replaceableReferenceTypes";
            this.replaceableReferenceTypes.Size = new System.Drawing.Size(246, 381);
            this.replaceableReferenceTypes.TabIndex = 0;
            // 
            // registeredReferences
            // 
            this.registeredReferences.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.registeredReferences.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.registeredReferences.Location = new System.Drawing.Point(265, 12);
            this.registeredReferences.Name = "registeredReferences";
            this.registeredReferences.Size = new System.Drawing.Size(602, 382);
            this.registeredReferences.TabIndex = 1;
            // 
            // debugDisplay
            // 
            this.debugDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.debugDisplay.FormattingEnabled = true;
            this.debugDisplay.Location = new System.Drawing.Point(12, 403);
            this.debugDisplay.Name = "debugDisplay";
            this.debugDisplay.Size = new System.Drawing.Size(855, 82);
            this.debugDisplay.TabIndex = 2;
            // 
            // RegistryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 497);
            this.Controls.Add(this.debugDisplay);
            this.Controls.Add(this.registeredReferences);
            this.Controls.Add(this.replaceableReferenceTypes);
            this.Name = "RegistryForm";
            this.Text = "RegistryForm";
            ((System.ComponentModel.ISupportInitialize)(this.registeredReferences)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox replaceableReferenceTypes;
        private System.Windows.Forms.DataGridView registeredReferences;
        private System.Windows.Forms.ListBox debugDisplay;
    }
}