
namespace RPG_App
{
	partial class RPG_Form
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
			this.OutputTxt = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// OutputTxt
			// 
			this.OutputTxt.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OutputTxt.Location = new System.Drawing.Point(12, 12);
			this.OutputTxt.Multiline = true;
			this.OutputTxt.Name = "OutputTxt";
			this.OutputTxt.ReadOnly = true;
			this.OutputTxt.Size = new System.Drawing.Size(510, 500);
			this.OutputTxt.TabIndex = 2;
			// 
			// RPG_Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(594, 553);
			this.Controls.Add(this.OutputTxt);
			this.KeyPreview = true;
			this.Name = "RPG_Form";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.RPG_Form_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox OutputTxt;
	}
}

