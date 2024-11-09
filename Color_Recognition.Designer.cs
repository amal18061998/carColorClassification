namespace Smartvisor.Benchmark
{
    partial class Color_Recognition
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
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.pictureBoxDehaze = new System.Windows.Forms.PictureBox();
            this.simpleButton = new DevExpress.XtraEditors.SimpleButton();
            this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
            this.pictureBoxEquialization = new System.Windows.Forms.PictureBox();
            this.pictureBoxCLAHE = new System.Windows.Forms.PictureBox();
            this.textEditDEHAZE = new DevExpress.XtraEditors.TextEdit();
            this.textEditEqui = new DevExpress.XtraEditors.TextEdit();
            this.textEditRGB = new DevExpress.XtraEditors.TextEdit();
            this.textEditCLAHE = new DevExpress.XtraEditors.TextEdit();
            this.carTraceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDehaze)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEquialization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCLAHE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditDEHAZE.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditEqui.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditRGB.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCLAHE.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.carTraceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // pictureBoxDehaze
            // 
            this.pictureBoxDehaze.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxDehaze.Location = new System.Drawing.Point(44, 57);
            this.pictureBoxDehaze.Name = "pictureBoxDehaze";
            this.pictureBoxDehaze.Size = new System.Drawing.Size(331, 241);
            this.pictureBoxDehaze.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDehaze.TabIndex = 0;
            this.pictureBoxDehaze.TabStop = false;
            // 
            // simpleButton
            // 
            this.simpleButton.Location = new System.Drawing.Point(264, 28);
            this.simpleButton.Name = "simpleButton";
            this.simpleButton.Size = new System.Drawing.Size(111, 23);
            this.simpleButton.TabIndex = 2;
            this.simpleButton.Text = "select image";
            this.simpleButton.Click += new System.EventHandler(this.simpleButton_Click);
            // 
            // chartControl1
            // 
            this.chartControl1.Location = new System.Drawing.Point(275, 331);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl1.Size = new System.Drawing.Size(341, 238);
            this.chartControl1.TabIndex = 4;
            // 
            // pictureBoxEquialization
            // 
            this.pictureBoxEquialization.Location = new System.Drawing.Point(460, 57);
            this.pictureBoxEquialization.Name = "pictureBoxEquialization";
            this.pictureBoxEquialization.Size = new System.Drawing.Size(329, 234);
            this.pictureBoxEquialization.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxEquialization.TabIndex = 8;
            this.pictureBoxEquialization.TabStop = false;
            this.pictureBoxEquialization.Click += new System.EventHandler(this.pictureBoxEquialization_Click);
            // 
            // pictureBoxCLAHE
            // 
            this.pictureBoxCLAHE.Location = new System.Drawing.Point(639, 349);
            this.pictureBoxCLAHE.Name = "pictureBoxCLAHE";
            this.pictureBoxCLAHE.Size = new System.Drawing.Size(150, 199);
            this.pictureBoxCLAHE.TabIndex = 9;
            this.pictureBoxCLAHE.TabStop = false;
            // 
            // textEditDEHAZE
            // 
            this.textEditDEHAZE.Location = new System.Drawing.Point(190, 304);
            this.textEditDEHAZE.Name = "textEditDEHAZE";
            this.textEditDEHAZE.Size = new System.Drawing.Size(185, 20);
            this.textEditDEHAZE.TabIndex = 10;
            // 
            // textEditEqui
            // 
            this.textEditEqui.Location = new System.Drawing.Point(689, 297);
            this.textEditEqui.Name = "textEditEqui";
            this.textEditEqui.Size = new System.Drawing.Size(100, 20);
            this.textEditEqui.TabIndex = 12;
            // 
            // textEditRGB
            // 
            this.textEditRGB.Location = new System.Drawing.Point(275, 586);
            this.textEditRGB.Name = "textEditRGB";
            this.textEditRGB.Size = new System.Drawing.Size(150, 20);
            this.textEditRGB.TabIndex = 7;
            // 
            // textEditCLAHE
            // 
            this.textEditCLAHE.Location = new System.Drawing.Point(516, 586);
            this.textEditCLAHE.Name = "textEditCLAHE";
            this.textEditCLAHE.Size = new System.Drawing.Size(100, 20);
            this.textEditCLAHE.TabIndex = 11;
            // 
            // carTraceBindingSource
            // 
            this.carTraceBindingSource.DataSource = typeof(Smartvisor.Benchmark.CarTrace);
            // 
            // Color_Recognition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 632);
            this.Controls.Add(this.textEditEqui);
            this.Controls.Add(this.textEditCLAHE);
            this.Controls.Add(this.textEditDEHAZE);
            this.Controls.Add(this.pictureBoxCLAHE);
            this.Controls.Add(this.pictureBoxEquialization);
            this.Controls.Add(this.textEditRGB);
            this.Controls.Add(this.chartControl1);
            this.Controls.Add(this.simpleButton);
            this.Controls.Add(this.pictureBoxDehaze);
            this.Name = "Color_Recognition";
            this.Text = "FormVisualize";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDehaze)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEquialization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCLAHE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditDEHAZE.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditEqui.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditRGB.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCLAHE.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.carTraceBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.PictureBox pictureBoxDehaze;
        private DevExpress.XtraEditors.SimpleButton simpleButton;
        private DevExpress.XtraCharts.ChartControl chartControl1;
        private System.Windows.Forms.BindingSource carTraceBindingSource;
        private System.Windows.Forms.PictureBox pictureBoxEquialization;
        private System.Windows.Forms.PictureBox pictureBoxCLAHE;
        private DevExpress.XtraEditors.TextEdit textEditDEHAZE;
        private DevExpress.XtraEditors.TextEdit textEditEqui;
        private DevExpress.XtraEditors.TextEdit textEditRGB;
        private DevExpress.XtraEditors.TextEdit textEditCLAHE;
    }
}