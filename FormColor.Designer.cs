namespace Smartvisor.Benchmark
{
    partial class FormColor
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
            this.pictureBoxHistogramme = new System.Windows.Forms.PictureBox();
            this.simpleButton = new DevExpress.XtraEditors.SimpleButton();
            this.textEditColor = new DevExpress.XtraEditors.TextEdit();
            this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
            this.textEdithsv = new DevExpress.XtraEditors.TextEdit();
            this.textEditRGB = new DevExpress.XtraEditors.TextEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
            this.pictureBoxCLAHE = new System.Windows.Forms.PictureBox();
            this.pictureBoxDEHAZE = new System.Windows.Forms.PictureBox();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.carTraceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMatchedColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYoloWorld1ColorDetection = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYoloWorld2ColorDetection = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCurrentColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colImageId = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistogramme)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdithsv.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditRGB.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCLAHE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDEHAZE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.carTraceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
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
            // pictureBoxHistogramme
            // 
            this.pictureBoxHistogramme.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxHistogramme.Location = new System.Drawing.Point(348, 9);
            this.pictureBoxHistogramme.Name = "pictureBoxHistogramme";
            this.pictureBoxHistogramme.Size = new System.Drawing.Size(224, 10);
            this.pictureBoxHistogramme.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxHistogramme.TabIndex = 0;
            this.pictureBoxHistogramme.TabStop = false;
            // 
            // simpleButton
            // 
            this.simpleButton.Location = new System.Drawing.Point(589, 50);
            this.simpleButton.Name = "simpleButton";
            this.simpleButton.Size = new System.Drawing.Size(121, 23);
            this.simpleButton.TabIndex = 2;
            this.simpleButton.Text = "select image";
            this.simpleButton.Click += new System.EventHandler(this.simpleButton_Click);
            // 
            // textEditColor
            // 
            this.textEditColor.Location = new System.Drawing.Point(654, 25);
            this.textEditColor.Name = "textEditColor";
            this.textEditColor.Size = new System.Drawing.Size(136, 20);
            this.textEditColor.TabIndex = 3;
            // 
            // chartControl1
            // 
            this.chartControl1.Location = new System.Drawing.Point(17, -4);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl1.Size = new System.Drawing.Size(300, 154);
            this.chartControl1.TabIndex = 4;
            // 
            // textEdithsv
            // 
            this.textEdithsv.Location = new System.Drawing.Point(372, 25);
            this.textEdithsv.Name = "textEdithsv";
            this.textEdithsv.Size = new System.Drawing.Size(135, 20);
            this.textEdithsv.TabIndex = 6;
            // 
            // textEditRGB
            // 
            this.textEditRGB.Location = new System.Drawing.Point(527, 25);
            this.textEditRGB.Name = "textEditRGB";
            this.textEditRGB.Size = new System.Drawing.Size(121, 20);
            this.textEditRGB.TabIndex = 7;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(643, -4);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(147, 23);
            this.simpleButton1.TabIndex = 8;
            this.simpleButton1.Text = "Add seen color";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(17, 341);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(159, 23);
            this.simpleButton2.TabIndex = 9;
            this.simpleButton2.Text = "CLAHE";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.Location = new System.Drawing.Point(262, 341);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(167, 23);
            this.simpleButton3.TabIndex = 10;
            this.simpleButton3.Text = "DEHAZE";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // simpleButton4
            // 
            this.simpleButton4.Location = new System.Drawing.Point(715, 50);
            this.simpleButton4.Margin = new System.Windows.Forms.Padding(2);
            this.simpleButton4.Name = "simpleButton4";
            this.simpleButton4.Size = new System.Drawing.Size(75, 23);
            this.simpleButton4.TabIndex = 11;
            this.simpleButton4.Text = "Test";
            this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
            // 
            // pictureBoxCLAHE
            // 
            this.pictureBoxCLAHE.Location = new System.Drawing.Point(17, 155);
            this.pictureBoxCLAHE.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxCLAHE.Name = "pictureBoxCLAHE";
            this.pictureBoxCLAHE.Size = new System.Drawing.Size(207, 181);
            this.pictureBoxCLAHE.TabIndex = 12;
            this.pictureBoxCLAHE.TabStop = false;
            // 
            // pictureBoxDEHAZE
            // 
            this.pictureBoxDEHAZE.Location = new System.Drawing.Point(240, 155);
            this.pictureBoxDEHAZE.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxDEHAZE.Name = "pictureBoxDEHAZE";
            this.pictureBoxDEHAZE.Size = new System.Drawing.Size(205, 181);
            this.pictureBoxDEHAZE.TabIndex = 13;
            this.pictureBoxDEHAZE.TabStop = false;
            // 
            // gridControl1
            // 
            this.gridControl1.DataSource = this.carTraceBindingSource;
            this.gridControl1.Location = new System.Drawing.Point(450, 78);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(340, 200);
            this.gridControl1.TabIndex = 14;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // carTraceBindingSource
            // 
            this.carTraceBindingSource.DataSource = typeof(Smartvisor.Benchmark.CarTrace);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMatchedColor,
            this.colYoloWorld1ColorDetection,
            this.colYoloWorld2ColorDetection,
            this.colCurrentColor,
            this.colId,
            this.colImageId});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // colMatchedColor
            // 
            this.colMatchedColor.FieldName = "MatchedColor";
            this.colMatchedColor.Name = "colMatchedColor";
            this.colMatchedColor.Visible = true;
            this.colMatchedColor.VisibleIndex = 0;
            // 
            // colYoloWorld1ColorDetection
            // 
            this.colYoloWorld1ColorDetection.FieldName = "YoloWorld1ColorDetection";
            this.colYoloWorld1ColorDetection.Name = "colYoloWorld1ColorDetection";
            this.colYoloWorld1ColorDetection.Visible = true;
            this.colYoloWorld1ColorDetection.VisibleIndex = 1;
            // 
            // colYoloWorld2ColorDetection
            // 
            this.colYoloWorld2ColorDetection.FieldName = "YoloWorld2ColorDetection";
            this.colYoloWorld2ColorDetection.Name = "colYoloWorld2ColorDetection";
            this.colYoloWorld2ColorDetection.Visible = true;
            this.colYoloWorld2ColorDetection.VisibleIndex = 2;
            // 
            // colCurrentColor
            // 
            this.colCurrentColor.FieldName = "CurrentColor";
            this.colCurrentColor.Name = "colCurrentColor";
            this.colCurrentColor.Visible = true;
            this.colCurrentColor.VisibleIndex = 3;
            // 
            // colId
            // 
            this.colId.FieldName = "Id";
            this.colId.Name = "colId";
            this.colId.Visible = true;
            this.colId.VisibleIndex = 4;
            // 
            // colImageId
            // 
            this.colImageId.FieldName = "ImageId";
            this.colImageId.Name = "colImageId";
            this.colImageId.Visible = true;
            this.colImageId.VisibleIndex = 5;
            // 
            // FormColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 632);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.pictureBoxDEHAZE);
            this.Controls.Add(this.pictureBoxCLAHE);
            this.Controls.Add(this.simpleButton4);
            this.Controls.Add(this.simpleButton3);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.textEditRGB);
            this.Controls.Add(this.textEdithsv);
            this.Controls.Add(this.chartControl1);
            this.Controls.Add(this.textEditColor);
            this.Controls.Add(this.simpleButton);
            this.Controls.Add(this.pictureBoxHistogramme);
            this.Name = "FormColor";
            this.Text = "FormVisualize";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistogramme)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdithsv.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditRGB.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCLAHE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDEHAZE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.carTraceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.PictureBox pictureBoxHistogramme;
        private DevExpress.XtraEditors.SimpleButton simpleButton;
        private DevExpress.XtraEditors.TextEdit textEditColor;
        private DevExpress.XtraCharts.ChartControl chartControl1;
        private DevExpress.XtraEditors.TextEdit textEdithsv;
        private DevExpress.XtraEditors.TextEdit textEditRGB;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.SimpleButton simpleButton4;
        private System.Windows.Forms.PictureBox pictureBoxCLAHE;
        private System.Windows.Forms.PictureBox pictureBoxDEHAZE;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private System.Windows.Forms.BindingSource carTraceBindingSource;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colMatchedColor;
        private DevExpress.XtraGrid.Columns.GridColumn colYoloWorld1ColorDetection;
        private DevExpress.XtraGrid.Columns.GridColumn colYoloWorld2ColorDetection;
        private DevExpress.XtraGrid.Columns.GridColumn colCurrentColor;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colImageId;
    }
}