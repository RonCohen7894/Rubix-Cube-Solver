namespace Rubix_Cube_Solver
{
    partial class popup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(popup));
            this.info = new System.Windows.Forms.TextBox();
            this.B_finish = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // info
            // 
            this.info.BackColor = System.Drawing.Color.MediumTurquoise;
            this.info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.info.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info.ForeColor = System.Drawing.Color.White;
            this.info.Location = new System.Drawing.Point(0, 0);
            this.info.Multiline = true;
            this.info.Name = "info";
            this.info.ReadOnly = true;
            this.info.Size = new System.Drawing.Size(412, 469);
            this.info.TabIndex = 1;
            this.info.Text = resources.GetString("info.Text");
            // 
            // B_finish
            // 
            this.B_finish.BackColor = System.Drawing.Color.DarkTurquoise;
            this.B_finish.FlatAppearance.BorderColor = System.Drawing.Color.Turquoise;
            this.B_finish.FlatAppearance.BorderSize = 3;
            this.B_finish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_finish.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_finish.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.B_finish.Location = new System.Drawing.Point(12, 397);
            this.B_finish.Name = "B_finish";
            this.B_finish.Size = new System.Drawing.Size(388, 60);
            this.B_finish.TabIndex = 1;
            this.B_finish.Text = "I\'m Done!";
            this.B_finish.UseVisualStyleBackColor = false;
            this.B_finish.Click += new System.EventHandler(this.B_finish_Click);
            // 
            // popup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 469);
            this.Controls.Add(this.B_finish);
            this.Controls.Add(this.info);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "popup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rubix Cube Solver ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox info;
        private System.Windows.Forms.Button B_finish;
    }
}