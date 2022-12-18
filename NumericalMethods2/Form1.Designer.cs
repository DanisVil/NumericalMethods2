
namespace NumericalMethods2
{
    partial class Form1
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
            this.nbox = new System.Windows.Forms.TextBox();
            this.resbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // nbox
            // 
            this.nbox.Location = new System.Drawing.Point(370, 140);
            this.nbox.Name = "nbox";
            this.nbox.Size = new System.Drawing.Size(277, 23);
            this.nbox.TabIndex = 0;
            // 
            // resbox
            // 
            this.resbox.Location = new System.Drawing.Point(352, 282);
            this.resbox.Name = "resbox";
            this.resbox.Size = new System.Drawing.Size(313, 23);
            this.resbox.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 627);
            this.Controls.Add(this.resbox);
            this.Controls.Add(this.nbox);
            this.Name = "Form1";
            this.Text = "Ну графики или че-то такое";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nbox;
        private System.Windows.Forms.TextBox resbox;
    }
}

