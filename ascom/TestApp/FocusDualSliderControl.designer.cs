namespace TestApp
{
    partial class FocusDualSliderControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FocusDualSliderControl));
            this.label1 = new System.Windows.Forms.Label();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRightFine = new System.Windows.Forms.Button();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            this.buttonLeftFine = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonRightCoarse = new System.Windows.Forms.Button();
            this.numericUpDownFine = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownCoarse = new System.Windows.Forms.NumericUpDown();
            this.buttonLeftCoarse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonSetup = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.leftPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoarse)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // leftPanel
            // 
            resources.ApplyResources(this.leftPanel, "leftPanel");
            this.leftPanel.Controls.Add(this.tableLayoutPanel1);
            this.leftPanel.Name = "leftPanel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonRightFine, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxReverse, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonLeftFine, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonRightCoarse, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownFine, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownCoarse, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonLeftCoarse, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonRightFine
            // 
            resources.ApplyResources(this.buttonRightFine, "buttonRightFine");
            this.buttonRightFine.Name = "buttonRightFine";
            this.buttonRightFine.UseVisualStyleBackColor = true;
            this.buttonRightFine.Click += new System.EventHandler(this.buttonRightFine_Click);
            // 
            // checkBoxReverse
            // 
            resources.ApplyResources(this.checkBoxReverse, "checkBoxReverse");
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            // 
            // buttonLeftFine
            // 
            resources.ApplyResources(this.buttonLeftFine, "buttonLeftFine");
            this.buttonLeftFine.Name = "buttonLeftFine";
            this.buttonLeftFine.UseVisualStyleBackColor = true;
            this.buttonLeftFine.Click += new System.EventHandler(this.buttonLeftFine_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // buttonRightCoarse
            // 
            resources.ApplyResources(this.buttonRightCoarse, "buttonRightCoarse");
            this.buttonRightCoarse.Name = "buttonRightCoarse";
            this.buttonRightCoarse.UseVisualStyleBackColor = true;
            this.buttonRightCoarse.Click += new System.EventHandler(this.buttonRightCoarse_Click);
            // 
            // numericUpDownFine
            // 
            resources.ApplyResources(this.numericUpDownFine, "numericUpDownFine");
            this.numericUpDownFine.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownFine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFine.Name = "numericUpDownFine";
            this.numericUpDownFine.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownCoarse
            // 
            resources.ApplyResources(this.numericUpDownCoarse, "numericUpDownCoarse");
            this.numericUpDownCoarse.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownCoarse.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownCoarse.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownCoarse.Name = "numericUpDownCoarse";
            this.numericUpDownCoarse.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // buttonLeftCoarse
            // 
            resources.ApplyResources(this.buttonLeftCoarse, "buttonLeftCoarse");
            this.buttonLeftCoarse.Name = "buttonLeftCoarse";
            this.buttonLeftCoarse.UseVisualStyleBackColor = true;
            this.buttonLeftCoarse.Click += new System.EventHandler(this.buttonLeftCoarse_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.buttonSetup);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.textBoxValue);
            this.panel1.Name = "panel1";
            // 
            // buttonSetup
            // 
            resources.ApplyResources(this.buttonSetup, "buttonSetup");
            this.buttonSetup.Name = "buttonSetup";
            this.buttonSetup.UseVisualStyleBackColor = true;
            this.buttonSetup.Click += new System.EventHandler(this.buttonSetup_Click);
            // 
            // buttonStop
            // 
            resources.ApplyResources(this.buttonStop, "buttonStop");
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxValue
            // 
            resources.ApplyResources(this.textBoxValue, "textBoxValue");
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.ReadOnly = true;
            // 
            // FocusDualSliderControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.leftPanel);
            this.Name = "FocusDualSliderControl";
            this.leftPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoarse)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonRightFine;
        private System.Windows.Forms.Button buttonLeftFine;
        private System.Windows.Forms.NumericUpDown numericUpDownFine;
        private System.Windows.Forms.Button buttonRightCoarse;
        private System.Windows.Forms.Button buttonLeftCoarse;
        private System.Windows.Forms.NumericUpDown numericUpDownCoarse;
        private System.Windows.Forms.CheckBox checkBoxReverse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonSetup;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
