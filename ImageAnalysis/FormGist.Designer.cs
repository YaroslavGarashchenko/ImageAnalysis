namespace ImageAnalysis
{
    partial class FormGist
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartGistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonSwitch = new System.Windows.Forms.Button();
            this.chartIntegral = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonProperty = new System.Windows.Forms.Button();
            this.propertyGridUniverse = new System.Windows.Forms.PropertyGrid();
            this.buttonTest = new System.Windows.Forms.Button();
            this.richTextBoxResultVerify = new System.Windows.Forms.RichTextBox();
            this.comboBoxAlfa = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartGistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartIntegral)).BeginInit();
            this.SuspendLayout();
            // 
            // chartGistogram
            // 
            chartArea1.Name = "ChartArea1";
            this.chartGistogram.ChartAreas.Add(chartArea1);
            this.chartGistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartGistogram.Location = new System.Drawing.Point(0, 0);
            this.chartGistogram.Name = "chartGistogram";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.chartGistogram.Series.Add(series1);
            this.chartGistogram.Size = new System.Drawing.Size(784, 461);
            this.chartGistogram.TabIndex = 7;
            this.chartGistogram.DoubleClick += new System.EventHandler(this.ChartGistogram_DoubleClick);
            // 
            // buttonSwitch
            // 
            this.buttonSwitch.Location = new System.Drawing.Point(11, 0);
            this.buttonSwitch.Name = "buttonSwitch";
            this.buttonSwitch.Size = new System.Drawing.Size(234, 25);
            this.buttonSwitch.TabIndex = 6;
            this.buttonSwitch.Text = "Интегральная функция распределения";
            this.buttonSwitch.UseVisualStyleBackColor = true;
            this.buttonSwitch.Click += new System.EventHandler(this.ButtonSwitch_Click);
            // 
            // chartIntegral
            // 
            chartArea2.Name = "ChartArea1";
            this.chartIntegral.ChartAreas.Add(chartArea2);
            this.chartIntegral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartIntegral.Location = new System.Drawing.Point(0, 0);
            this.chartIntegral.Name = "chartIntegral";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Series1";
            this.chartIntegral.Series.Add(series2);
            this.chartIntegral.Size = new System.Drawing.Size(784, 461);
            this.chartIntegral.TabIndex = 8;
            // 
            // buttonProperty
            // 
            this.buttonProperty.Location = new System.Drawing.Point(593, -1);
            this.buttonProperty.Name = "buttonProperty";
            this.buttonProperty.Size = new System.Drawing.Size(178, 23);
            this.buttonProperty.TabIndex = 5;
            this.buttonProperty.Text = "Настройки";
            this.buttonProperty.UseVisualStyleBackColor = true;
            this.buttonProperty.Click += new System.EventHandler(this.ButtonProperty_Click);
            // 
            // propertyGridUniverse
            // 
            this.propertyGridUniverse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridUniverse.Location = new System.Drawing.Point(581, 21);
            this.propertyGridUniverse.Name = "propertyGridUniverse";
            this.propertyGridUniverse.SelectedObject = this.chartGistogram;
            this.propertyGridUniverse.Size = new System.Drawing.Size(203, 440);
            this.propertyGridUniverse.TabIndex = 4;
            this.propertyGridUniverse.ToolbarVisible = false;
            this.propertyGridUniverse.Visible = false;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(267, 0);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(152, 25);
            this.buttonTest.TabIndex = 2;
            this.buttonTest.Text = "Проверка гипотезы";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.ButtonTest_Click);
            // 
            // richTextBoxResultVerify
            // 
            this.richTextBoxResultVerify.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxResultVerify.Location = new System.Drawing.Point(581, 21);
            this.richTextBoxResultVerify.Name = "richTextBoxResultVerify";
            this.richTextBoxResultVerify.ReadOnly = true;
            this.richTextBoxResultVerify.Size = new System.Drawing.Size(203, 440);
            this.richTextBoxResultVerify.TabIndex = 0;
            this.richTextBoxResultVerify.Text = "";
            this.richTextBoxResultVerify.Visible = false;
            this.richTextBoxResultVerify.DoubleClick += new System.EventHandler(this.RichTextBoxResultVerify_DoubleClick);
            // 
            // comboBoxAlfa
            // 
            this.comboBoxAlfa.FormattingEnabled = true;
            this.comboBoxAlfa.Items.AddRange(new object[] {
            "0,20",
            "0,15",
            "0,10",
            "0,05",
            "0,01",
            "0,005",
            "0,001"});
            this.comboBoxAlfa.Location = new System.Drawing.Point(447, 1);
            this.comboBoxAlfa.Name = "comboBoxAlfa";
            this.comboBoxAlfa.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAlfa.TabIndex = 1;
            this.comboBoxAlfa.Text = "0,05";
            this.comboBoxAlfa.UseWaitCursor = true;
            // 
            // FormGist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.richTextBoxResultVerify);
            this.Controls.Add(this.comboBoxAlfa);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.propertyGridUniverse);
            this.Controls.Add(this.buttonProperty);
            this.Controls.Add(this.buttonSwitch);
            this.Controls.Add(this.chartGistogram);
            this.Controls.Add(this.chartIntegral);
            this.HelpButton = true;
            this.Name = "FormGist";
            this.ShowInTaskbar = false;
            this.Text = "Гистограмма распределения: ";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.chartGistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartIntegral)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        /// <summary>
        /// Гистограмма распределения
        /// </summary>
        public System.Windows.Forms.DataVisualization.Charting.Chart chartGistogram;
        private System.Windows.Forms.Button buttonSwitch;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartIntegral;
        private System.Windows.Forms.Button buttonProperty;
        private System.Windows.Forms.PropertyGrid propertyGridUniverse;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.RichTextBox richTextBoxResultVerify;
        private System.Windows.Forms.ComboBox comboBoxAlfa;
    }
}