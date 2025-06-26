namespace CreatorChannelsXrmToolbox
{
    partial class About
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
            this.LblNameApp = new System.Windows.Forms.Label();
            this.LblDescription = new System.Windows.Forms.Label();
            this.LblAuthor = new System.Windows.Forms.Label();
            this.LblYear = new System.Windows.Forms.Label();
            this.LblVersion = new System.Windows.Forms.Label();
            this.ImgLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImgLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // LblNameApp
            // 
            this.LblNameApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblNameApp.Location = new System.Drawing.Point(1, 133);
            this.LblNameApp.Name = "LblNameApp";
            this.LblNameApp.Size = new System.Drawing.Size(451, 29);
            this.LblNameApp.TabIndex = 0;
            this.LblNameApp.Text = "Generate custom channel solution";
            this.LblNameApp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblDescription
            // 
            this.LblDescription.Location = new System.Drawing.Point(12, 186);
            this.LblDescription.Name = "LblDescription";
            this.LblDescription.Size = new System.Drawing.Size(432, 37);
            this.LblDescription.TabIndex = 1;
            this.LblDescription.Text = "This component allows you to generate a Dynamics 365 solution to establish a cust" +
    "om channel in Customer Insights - Journeys.";
            this.LblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblAuthor
            // 
            this.LblAuthor.AutoSize = true;
            this.LblAuthor.Location = new System.Drawing.Point(190, 238);
            this.LblAuthor.Name = "LblAuthor";
            this.LblAuthor.Size = new System.Drawing.Size(66, 16);
            this.LblAuthor.TabIndex = 2;
            this.LblAuthor.Text = "Eric Vega";
            // 
            // LblYear
            // 
            this.LblYear.AutoSize = true;
            this.LblYear.Location = new System.Drawing.Point(201, 264);
            this.LblYear.Name = "LblYear";
            this.LblYear.Size = new System.Drawing.Size(42, 16);
            this.LblYear.TabIndex = 3;
            this.LblYear.Text = "[year]";
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblVersion.Location = new System.Drawing.Point(190, 164);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(73, 16);
            this.LblVersion.TabIndex = 6;
            this.LblVersion.Text = "Version 1.0";
            // 
            // ImgLogo
            // 
            this.ImgLogo.Image = global::CreatorChannelsXrmToolbox.Properties.Resources.logo;
            this.ImgLogo.Location = new System.Drawing.Point(171, 2);
            this.ImgLogo.Name = "ImgLogo";
            this.ImgLogo.Size = new System.Drawing.Size(128, 128);
            this.ImgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImgLogo.TabIndex = 4;
            this.ImgLogo.TabStop = false;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 294);
            this.Controls.Add(this.LblVersion);
            this.Controls.Add(this.ImgLogo);
            this.Controls.Add(this.LblYear);
            this.Controls.Add(this.LblAuthor);
            this.Controls.Add(this.LblDescription);
            this.Controls.Add(this.LblNameApp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImgLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblNameApp;
        private System.Windows.Forms.Label LblDescription;
        private System.Windows.Forms.Label LblAuthor;
        private System.Windows.Forms.Label LblYear;
        private System.Windows.Forms.PictureBox ImgLogo;
        private System.Windows.Forms.Label LblVersion;
    }
}