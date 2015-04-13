namespace Shlomi.mapz._2
{
    partial class Capture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Capture));
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_capture_area_name = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_strat_job = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_set_target_folder = new System.Windows.Forms.Button();
            this.txt_folder_path = new System.Windows.Forms.TextBox();
            this.panel_satuts_capturing = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbl_Doing = new System.Windows.Forms.Label();
            this.lbl_Tot = new System.Windows.Forms.Label();
            this.chk_add_scale = new System.Windows.Forms.CheckBox();
            this.chk_add_coords = new System.Windows.Forms.CheckBox();
            this.btn_cancel_all = new System.Windows.Forms.Button();
            this.chk_open_results = new System.Windows.Forms.CheckBox();
            this.panel_satuts_capturing.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.Location = new System.Drawing.Point(16, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Capturing Area: ";
            // 
            // lbl_capture_area_name
            // 
            this.lbl_capture_area_name.AutoSize = true;
            this.lbl_capture_area_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lbl_capture_area_name.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lbl_capture_area_name.Location = new System.Drawing.Point(168, 101);
            this.lbl_capture_area_name.Name = "lbl_capture_area_name";
            this.lbl_capture_area_name.Size = new System.Drawing.Size(77, 16);
            this.lbl_capture_area_name.TabIndex = 2;
            this.lbl_capture_area_name.Text = "Area Name";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 122);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(887, 17);
            this.progressBar1.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.button1.ForeColor = System.Drawing.Color.OrangeRed;
            this.button1.Location = new System.Drawing.Point(377, 145);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 41);
            this.button1.TabIndex = 8;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.cancel_job_Click);
            // 
            // btn_strat_job
            // 
            this.btn_strat_job.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btn_strat_job.ForeColor = System.Drawing.Color.Green;
            this.btn_strat_job.Location = new System.Drawing.Point(199, 145);
            this.btn_strat_job.Name = "btn_strat_job";
            this.btn_strat_job.Size = new System.Drawing.Size(160, 41);
            this.btn_strat_job.TabIndex = 9;
            this.btn_strat_job.Text = "Start";
            this.btn_strat_job.UseVisualStyleBackColor = true;
            this.btn_strat_job.Click += new System.EventHandler(this.btn_strat_job_Click);
            // 
            // btn_set_target_folder
            // 
            this.btn_set_target_folder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btn_set_target_folder.Location = new System.Drawing.Point(16, 11);
            this.btn_set_target_folder.Name = "btn_set_target_folder";
            this.btn_set_target_folder.Size = new System.Drawing.Size(133, 28);
            this.btn_set_target_folder.TabIndex = 10;
            this.btn_set_target_folder.Text = "Set Target Folder";
            this.btn_set_target_folder.UseVisualStyleBackColor = true;
            this.btn_set_target_folder.Click += new System.EventHandler(this.btn_set_target_folder_Click);
            // 
            // txt_folder_path
            // 
            this.txt_folder_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txt_folder_path.Location = new System.Drawing.Point(155, 12);
            this.txt_folder_path.MaximumSize = new System.Drawing.Size(1000, 29);
            this.txt_folder_path.Name = "txt_folder_path";
            this.txt_folder_path.ReadOnly = true;
            this.txt_folder_path.Size = new System.Drawing.Size(744, 26);
            this.txt_folder_path.TabIndex = 11;
            // 
            // panel_satuts_capturing
            // 
            this.panel_satuts_capturing.Controls.Add(this.label4);
            this.panel_satuts_capturing.Controls.Add(this.label5);
            this.panel_satuts_capturing.Controls.Add(this.lbl_Doing);
            this.panel_satuts_capturing.Controls.Add(this.lbl_Tot);
            this.panel_satuts_capturing.Location = new System.Drawing.Point(289, 94);
            this.panel_satuts_capturing.Name = "panel_satuts_capturing";
            this.panel_satuts_capturing.Size = new System.Drawing.Size(614, 27);
            this.panel_satuts_capturing.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label4.Location = new System.Drawing.Point(21, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Capturing Tile: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label5.Location = new System.Drawing.Point(180, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "of";
            // 
            // lbl_Doing
            // 
            this.lbl_Doing.AutoSize = true;
            this.lbl_Doing.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lbl_Doing.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Doing.Location = new System.Drawing.Point(124, 7);
            this.lbl_Doing.Name = "lbl_Doing";
            this.lbl_Doing.Size = new System.Drawing.Size(15, 16);
            this.lbl_Doing.TabIndex = 8;
            this.lbl_Doing.Text = "1";
            // 
            // lbl_Tot
            // 
            this.lbl_Tot.AutoSize = true;
            this.lbl_Tot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lbl_Tot.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Tot.Location = new System.Drawing.Point(205, 7);
            this.lbl_Tot.Name = "lbl_Tot";
            this.lbl_Tot.Size = new System.Drawing.Size(15, 16);
            this.lbl_Tot.TabIndex = 10;
            this.lbl_Tot.Text = "1";
            // 
            // chk_add_scale
            // 
            this.chk_add_scale.AutoSize = true;
            this.chk_add_scale.Checked = true;
            this.chk_add_scale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_add_scale.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.chk_add_scale.Location = new System.Drawing.Point(155, 44);
            this.chk_add_scale.Name = "chk_add_scale";
            this.chk_add_scale.Size = new System.Drawing.Size(90, 20);
            this.chk_add_scale.TabIndex = 14;
            this.chk_add_scale.Text = "Add Scale";
            this.chk_add_scale.UseVisualStyleBackColor = true;
            // 
            // chk_add_coords
            // 
            this.chk_add_coords.AutoSize = true;
            this.chk_add_coords.Checked = true;
            this.chk_add_coords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_add_coords.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.chk_add_coords.Location = new System.Drawing.Point(260, 44);
            this.chk_add_coords.Name = "chk_add_coords";
            this.chk_add_coords.Size = new System.Drawing.Size(99, 20);
            this.chk_add_coords.TabIndex = 15;
            this.chk_add_coords.Text = "Add Coords";
            this.chk_add_coords.UseVisualStyleBackColor = true;
            // 
            // btn_cancel_all
            // 
            this.btn_cancel_all.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btn_cancel_all.ForeColor = System.Drawing.Color.OrangeRed;
            this.btn_cancel_all.Location = new System.Drawing.Point(561, 145);
            this.btn_cancel_all.Name = "btn_cancel_all";
            this.btn_cancel_all.Size = new System.Drawing.Size(167, 41);
            this.btn_cancel_all.TabIndex = 16;
            this.btn_cancel_all.Text = "Cancel All";
            this.btn_cancel_all.UseVisualStyleBackColor = true;
            this.btn_cancel_all.Click += new System.EventHandler(this.btn_cancel_all_Click);
            // 
            // chk_open_results
            // 
            this.chk_open_results.AutoSize = true;
            this.chk_open_results.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.chk_open_results.Location = new System.Drawing.Point(377, 44);
            this.chk_open_results.Name = "chk_open_results";
            this.chk_open_results.Size = new System.Drawing.Size(191, 20);
            this.chk_open_results.TabIndex = 17;
            this.chk_open_results.Text = "Open Images when finished";
            this.chk_open_results.UseVisualStyleBackColor = true;
            // 
            // Capture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 199);
            this.Controls.Add(this.chk_open_results);
            this.Controls.Add(this.btn_cancel_all);
            this.Controls.Add(this.chk_add_coords);
            this.Controls.Add(this.chk_add_scale);
            this.Controls.Add(this.panel_satuts_capturing);
            this.Controls.Add(this.txt_folder_path);
            this.Controls.Add(this.btn_set_target_folder);
            this.Controls.Add(this.btn_strat_job);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lbl_capture_area_name);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Capture";
            this.Text = "Mapz Capture Procedure";
            this.panel_satuts_capturing.ResumeLayout(false);
            this.panel_satuts_capturing.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_capture_area_name;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_strat_job;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btn_set_target_folder;
        private System.Windows.Forms.TextBox txt_folder_path;
        private System.Windows.Forms.Panel panel_satuts_capturing;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_Doing;
        private System.Windows.Forms.Label lbl_Tot;
        private System.Windows.Forms.CheckBox chk_add_scale;
        private System.Windows.Forms.CheckBox chk_add_coords;
        private System.Windows.Forms.Button btn_cancel_all;
        private System.Windows.Forms.CheckBox chk_open_results;
    }
}