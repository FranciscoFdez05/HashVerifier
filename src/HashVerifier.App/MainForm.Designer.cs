namespace HashVerifier.App
{
    partial class MainForm
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
            if (disposing)
            {
                components?.Dispose();
                _cts?.Dispose();
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
            label_algorithm = new Label();
            comboBox_algorithm = new ComboBox();
            label_language = new Label();
            comboBox_language = new ComboBox();
            groupBox_generate = new GroupBox();
            label_fileToHash = new Label();
            textBox_fileToHash = new TextBox();
            button_browseHash = new Button();
            label_generatedHash = new Label();
            textBox_generatedHash = new TextBox();
            button_copy = new Button();
            button_calculate = new Button();
            groupBox_verify = new GroupBox();
            label_fileToVerify = new Label();
            textBox_fileToVerify = new TextBox();
            button_browseVerify = new Button();
            label_expectedHash = new Label();
            textBox_expectedHash = new TextBox();
            button_paste = new Button();
            button_verify = new Button();
            label_result = new Label();
            progressBar = new ProgressBar();
            button_cancel = new Button();
            button_clear = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            groupBox_generate.SuspendLayout();
            groupBox_verify.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // label_algorithm
            //
            label_algorithm.AutoSize = true;
            label_algorithm.Location = new Point(12, 16);
            label_algorithm.Name = "label_algorithm";
            label_algorithm.TabIndex = 0;
            label_algorithm.Text = "Hash algorithm";
            //
            // comboBox_algorithm
            //
            comboBox_algorithm.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_algorithm.Location = new Point(130, 12);
            comboBox_algorithm.Name = "comboBox_algorithm";
            comboBox_algorithm.Size = new Size(130, 23);
            comboBox_algorithm.TabIndex = 1;
            comboBox_algorithm.SelectedIndexChanged += ComboBox_algorithm_SelectedIndexChanged;
            //
            // label_language
            //
            label_language.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_language.Location = new Point(480, 16);
            label_language.Name = "label_language";
            label_language.Size = new Size(140, 15);
            label_language.TabIndex = 2;
            label_language.Text = "Language";
            label_language.TextAlign = ContentAlignment.TopRight;
            //
            // comboBox_language
            //
            comboBox_language.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox_language.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_language.Location = new Point(628, 12);
            comboBox_language.Name = "comboBox_language";
            comboBox_language.Size = new Size(120, 23);
            comboBox_language.TabIndex = 3;
            comboBox_language.SelectedIndexChanged += ComboBox_language_SelectedIndexChanged;
            //
            // groupBox_generate
            //
            groupBox_generate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox_generate.Controls.Add(label_fileToHash);
            groupBox_generate.Controls.Add(textBox_fileToHash);
            groupBox_generate.Controls.Add(button_browseHash);
            groupBox_generate.Controls.Add(label_generatedHash);
            groupBox_generate.Controls.Add(textBox_generatedHash);
            groupBox_generate.Controls.Add(button_copy);
            groupBox_generate.Controls.Add(button_calculate);
            groupBox_generate.Location = new Point(12, 46);
            groupBox_generate.Name = "groupBox_generate";
            groupBox_generate.Size = new Size(736, 166);
            groupBox_generate.TabIndex = 4;
            groupBox_generate.TabStop = false;
            groupBox_generate.Text = "Generate hash";
            //
            // label_fileToHash
            //
            label_fileToHash.AutoSize = true;
            label_fileToHash.Location = new Point(12, 22);
            label_fileToHash.Name = "label_fileToHash";
            label_fileToHash.TabIndex = 0;
            label_fileToHash.Text = "File to hash";
            //
            // textBox_fileToHash
            //
            textBox_fileToHash.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_fileToHash.Location = new Point(12, 42);
            textBox_fileToHash.Name = "textBox_fileToHash";
            textBox_fileToHash.Size = new Size(600, 23);
            textBox_fileToHash.TabIndex = 1;
            //
            // button_browseHash
            //
            button_browseHash.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_browseHash.Location = new Point(622, 41);
            button_browseHash.Name = "button_browseHash";
            button_browseHash.Size = new Size(100, 25);
            button_browseHash.TabIndex = 2;
            button_browseHash.Text = "Browse…";
            button_browseHash.UseVisualStyleBackColor = true;
            button_browseHash.Click += Button_browseHash_Click;
            //
            // label_generatedHash
            //
            label_generatedHash.AutoSize = true;
            label_generatedHash.Location = new Point(12, 74);
            label_generatedHash.Name = "label_generatedHash";
            label_generatedHash.TabIndex = 3;
            label_generatedHash.Text = "Generated hash";
            //
            // textBox_generatedHash
            //
            textBox_generatedHash.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_generatedHash.Font = new Font("Consolas", 9.75F);
            textBox_generatedHash.Location = new Point(12, 94);
            textBox_generatedHash.Multiline = true;
            textBox_generatedHash.Name = "textBox_generatedHash";
            textBox_generatedHash.ReadOnly = true;
            textBox_generatedHash.Size = new Size(600, 23);
            textBox_generatedHash.TabIndex = 4;
            textBox_generatedHash.TextChanged += TextBox_generatedHash_LayoutChanged;
            textBox_generatedHash.Resize += TextBox_generatedHash_LayoutChanged;
            //
            // button_copy
            //
            button_copy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_copy.Location = new Point(622, 93);
            button_copy.Name = "button_copy";
            button_copy.Size = new Size(100, 25);
            button_copy.TabIndex = 5;
            button_copy.Text = "Copy";
            button_copy.UseVisualStyleBackColor = true;
            button_copy.Click += Button_copy_Click;
            //
            // button_calculate
            //
            button_calculate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button_calculate.Location = new Point(12, 126);
            button_calculate.Name = "button_calculate";
            button_calculate.Size = new Size(710, 30);
            button_calculate.TabIndex = 6;
            button_calculate.Text = "Calculate";
            button_calculate.UseVisualStyleBackColor = true;
            button_calculate.Click += Button_calculate_Click;
            //
            // groupBox_verify
            //
            groupBox_verify.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox_verify.Controls.Add(label_fileToVerify);
            groupBox_verify.Controls.Add(textBox_fileToVerify);
            groupBox_verify.Controls.Add(button_browseVerify);
            groupBox_verify.Controls.Add(label_expectedHash);
            groupBox_verify.Controls.Add(textBox_expectedHash);
            groupBox_verify.Controls.Add(button_paste);
            groupBox_verify.Controls.Add(button_verify);
            groupBox_verify.Controls.Add(label_result);
            groupBox_verify.Location = new Point(12, 222);
            groupBox_verify.Name = "groupBox_verify";
            groupBox_verify.Size = new Size(736, 196);
            groupBox_verify.TabIndex = 5;
            groupBox_verify.TabStop = false;
            groupBox_verify.Text = "Verify file";
            //
            // label_fileToVerify
            //
            label_fileToVerify.AutoSize = true;
            label_fileToVerify.Location = new Point(12, 22);
            label_fileToVerify.Name = "label_fileToVerify";
            label_fileToVerify.TabIndex = 0;
            label_fileToVerify.Text = "File to verify";
            //
            // textBox_fileToVerify
            //
            textBox_fileToVerify.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_fileToVerify.Location = new Point(12, 42);
            textBox_fileToVerify.Name = "textBox_fileToVerify";
            textBox_fileToVerify.Size = new Size(600, 23);
            textBox_fileToVerify.TabIndex = 1;
            //
            // button_browseVerify
            //
            button_browseVerify.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_browseVerify.Location = new Point(622, 41);
            button_browseVerify.Name = "button_browseVerify";
            button_browseVerify.Size = new Size(100, 25);
            button_browseVerify.TabIndex = 2;
            button_browseVerify.Text = "Browse…";
            button_browseVerify.UseVisualStyleBackColor = true;
            button_browseVerify.Click += Button_browseVerify_Click;
            //
            // label_expectedHash
            //
            label_expectedHash.AutoSize = true;
            label_expectedHash.Location = new Point(12, 74);
            label_expectedHash.Name = "label_expectedHash";
            label_expectedHash.TabIndex = 3;
            label_expectedHash.Text = "Expected hash";
            //
            // textBox_expectedHash
            //
            textBox_expectedHash.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_expectedHash.Font = new Font("Consolas", 9.75F);
            textBox_expectedHash.Location = new Point(12, 94);
            textBox_expectedHash.Name = "textBox_expectedHash";
            textBox_expectedHash.Size = new Size(600, 23);
            textBox_expectedHash.TabIndex = 4;
            textBox_expectedHash.TextChanged += TextBox_expectedHash_TextChanged;
            //
            // button_paste
            //
            button_paste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_paste.Location = new Point(622, 93);
            button_paste.Name = "button_paste";
            button_paste.Size = new Size(100, 25);
            button_paste.TabIndex = 5;
            button_paste.Text = "Paste";
            button_paste.UseVisualStyleBackColor = true;
            button_paste.Click += Button_paste_Click;
            //
            // button_verify
            //
            button_verify.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button_verify.Location = new Point(12, 126);
            button_verify.Name = "button_verify";
            button_verify.Size = new Size(710, 30);
            button_verify.TabIndex = 6;
            button_verify.Text = "Verify";
            button_verify.UseVisualStyleBackColor = true;
            button_verify.Click += Button_verify_Click;
            //
            // label_result
            //
            label_result.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label_result.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            label_result.Location = new Point(12, 162);
            label_result.Name = "label_result";
            label_result.Size = new Size(710, 26);
            label_result.TabIndex = 7;
            label_result.TextAlign = ContentAlignment.MiddleCenter;
            //
            // progressBar
            //
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(12, 430);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(524, 27);
            progressBar.TabIndex = 6;
            //
            // button_cancel
            //
            button_cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_cancel.Enabled = false;
            button_cancel.Location = new Point(542, 430);
            button_cancel.Name = "button_cancel";
            button_cancel.Size = new Size(100, 27);
            button_cancel.TabIndex = 7;
            button_cancel.Text = "Cancel";
            button_cancel.UseVisualStyleBackColor = true;
            button_cancel.Click += Button_cancel_Click;
            //
            // button_clear
            //
            button_clear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_clear.Location = new Point(648, 430);
            button_clear.Name = "button_clear";
            button_clear.Size = new Size(100, 27);
            button_clear.TabIndex = 8;
            button_clear.Text = "Clear";
            button_clear.UseVisualStyleBackColor = true;
            button_clear.Click += Button_clear_Click;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 468);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(760, 22);
            statusStrip.TabIndex = 9;
            //
            // statusLabel
            //
            statusLabel.Name = "statusLabel";
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 490);
            Controls.Add(label_algorithm);
            Controls.Add(comboBox_algorithm);
            Controls.Add(label_language);
            Controls.Add(comboBox_language);
            Controls.Add(groupBox_generate);
            Controls.Add(groupBox_verify);
            Controls.Add(progressBar);
            Controls.Add(button_cancel);
            Controls.Add(button_clear);
            Controls.Add(statusStrip);
            MinimumSize = new Size(620, 529);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HashVerifier";
            FormClosing += MainForm_FormClosing;
            groupBox_generate.ResumeLayout(false);
            groupBox_generate.PerformLayout();
            groupBox_verify.ResumeLayout(false);
            groupBox_verify.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_algorithm;
        private ComboBox comboBox_algorithm;
        private Label label_language;
        private ComboBox comboBox_language;
        private GroupBox groupBox_generate;
        private Label label_fileToHash;
        private TextBox textBox_fileToHash;
        private Button button_browseHash;
        private Label label_generatedHash;
        private TextBox textBox_generatedHash;
        private Button button_copy;
        private Button button_calculate;
        private GroupBox groupBox_verify;
        private Label label_fileToVerify;
        private TextBox textBox_fileToVerify;
        private Button button_browseVerify;
        private Label label_expectedHash;
        private TextBox textBox_expectedHash;
        private Button button_paste;
        private Button button_verify;
        private Label label_result;
        private ProgressBar progressBar;
        private Button button_cancel;
        private Button button_clear;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
    }
}
