namespace HashVerifier
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
            button_ExaminarFileToHash = new Button();
            textBox1 = new TextBox();
            button_hashear = new Button();
            button_SelectFile = new Button();
            button_SelectFileHash = new Button();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            button_Verify = new Button();
            comboBox_hashAlogrithm = new ComboBox();
            label2 = new Label();
            button_examinarFileHash = new Button();
            textBox4 = new TextBox();
            button_clear = new Button();
            label3 = new Label();
            label1 = new Label();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // button_ExaminarFileToHash
            // 
            button_ExaminarFileToHash.Location = new Point(23, 54);
            button_ExaminarFileToHash.Margin = new Padding(3, 2, 3, 2);
            button_ExaminarFileToHash.Name = "button_ExaminarFileToHash";
            button_ExaminarFileToHash.Size = new Size(109, 20);
            button_ExaminarFileToHash.TabIndex = 0;
            button_ExaminarFileToHash.Text = "Fichero a hashear";
            button_ExaminarFileToHash.UseVisualStyleBackColor = true;
            button_ExaminarFileToHash.Click += button1_Click_1;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(138, 54);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(402, 23);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button_hashear
            // 
            button_hashear.Location = new Point(138, 138);
            button_hashear.Margin = new Padding(3, 2, 3, 2);
            button_hashear.Name = "button_hashear";
            button_hashear.Size = new Size(402, 39);
            button_hashear.TabIndex = 2;
            button_hashear.Text = "Hashear";
            button_hashear.UseVisualStyleBackColor = true;
            button_hashear.Click += button2_Click;
            // 
            // button_SelectFile
            // 
            button_SelectFile.Location = new Point(23, 226);
            button_SelectFile.Margin = new Padding(3, 2, 3, 2);
            button_SelectFile.Name = "button_SelectFile";
            button_SelectFile.Size = new Size(109, 20);
            button_SelectFile.TabIndex = 4;
            button_SelectFile.Text = "Fichero fuente";
            button_SelectFile.UseVisualStyleBackColor = true;
            button_SelectFile.Click += button3_Click;
            // 
            // button_SelectFileHash
            // 
            button_SelectFileHash.Location = new Point(23, 272);
            button_SelectFileHash.Margin = new Padding(3, 2, 3, 2);
            button_SelectFileHash.Name = "button_SelectFileHash";
            button_SelectFileHash.Size = new Size(109, 22);
            button_SelectFileHash.TabIndex = 5;
            button_SelectFileHash.Text = "Fichero hash";
            button_SelectFileHash.UseVisualStyleBackColor = true;
            button_SelectFileHash.Click += button4_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(138, 223);
            textBox2.Margin = new Padding(3, 2, 3, 2);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(402, 23);
            textBox2.TabIndex = 6;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(138, 271);
            textBox3.Margin = new Padding(3, 2, 3, 2);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(402, 23);
            textBox3.TabIndex = 7;
            // 
            // button_Verify
            // 
            button_Verify.Location = new Point(138, 305);
            button_Verify.Margin = new Padding(3, 2, 3, 2);
            button_Verify.Name = "button_Verify";
            button_Verify.Size = new Size(402, 39);
            button_Verify.TabIndex = 8;
            button_Verify.Text = "Verificar";
            button_Verify.UseVisualStyleBackColor = true;
            button_Verify.Click += button_Verify_Click;
            // 
            // comboBox_hashAlogrithm
            // 
            comboBox_hashAlogrithm.FormattingEnabled = true;
            comboBox_hashAlogrithm.Location = new Point(561, 54);
            comboBox_hashAlogrithm.Margin = new Padding(3, 2, 3, 2);
            comboBox_hashAlogrithm.Name = "comboBox_hashAlogrithm";
            comboBox_hashAlogrithm.Size = new Size(89, 23);
            comboBox_hashAlogrithm.TabIndex = 9;
            comboBox_hashAlogrithm.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(551, 30);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
            label2.TabIndex = 10;
            label2.Text = "algoridmo del hash";
            label2.Click += label2_Click;
            // 
            // button_examinarFileHash
            // 
            button_examinarFileHash.Location = new Point(23, 105);
            button_examinarFileHash.Margin = new Padding(3, 2, 3, 2);
            button_examinarFileHash.Name = "button_examinarFileHash";
            button_examinarFileHash.Size = new Size(109, 20);
            button_examinarFileHash.TabIndex = 11;
            button_examinarFileHash.Text = "Fichero hasheado";
            button_examinarFileHash.UseVisualStyleBackColor = true;
            button_examinarFileHash.Click += button_examinarFileHash_Click_1;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(138, 104);
            textBox4.Margin = new Padding(3, 2, 3, 2);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(402, 23);
            textBox4.TabIndex = 12;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // button_clear
            // 
            button_clear.Location = new Point(561, 304);
            button_clear.Name = "button_clear";
            button_clear.Size = new Size(89, 39);
            button_clear.TabIndex = 14;
            button_clear.Text = "Limpiar";
            button_clear.UseVisualStyleBackColor = true;
            button_clear.Click += button_clear_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(170, 87);
            label3.Name = "label3";
            label3.Size = new Size(262, 15);
            label3.TabIndex = 13;
            label3.Text = "Introduce donde quieres guardar el archivo hash";
            label3.Click += label3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(170, 30);
            label1.Name = "label1";
            label1.Size = new Size(278, 15);
            label1.TabIndex = 3;
            label1.Text = "Introduce el archivo a hashear, para obtener el hash";
            label1.Click += label1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(138, 254);
            label4.Name = "label4";
            label4.Size = new Size(201, 15);
            label4.TabIndex = 15;
            label4.Text = "Introduce un .txt con el hash original";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(138, 206);
            label5.Name = "label5";
            label5.Size = new Size(183, 15);
            label5.TabIndex = 16;
            label5.Text = "Introduce el archivo a comprobar";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(685, 355);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(button_clear);
            Controls.Add(label3);
            Controls.Add(textBox4);
            Controls.Add(button_examinarFileHash);
            Controls.Add(label2);
            Controls.Add(comboBox_hashAlogrithm);
            Controls.Add(button_Verify);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(button_SelectFileHash);
            Controls.Add(button_SelectFile);
            Controls.Add(label1);
            Controls.Add(button_hashear);
            Controls.Add(textBox1);
            Controls.Add(button_ExaminarFileToHash);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "HashVerifier";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_ExaminarFileToHash;
        private TextBox textBox1;
        private Button button_hashear;
        private Button button_SelectFile;
        private Button button_SelectFileHash;
        private TextBox textBox2;
        private TextBox textBox3;
        private Button button_Verify;
        private ComboBox comboBox_hashAlogrithm;
        private Label label2;
        private Button button_examinarFileHash;
        private TextBox textBox4;
        private Button button_clear;
        private Label label3;
        private Label label1;
        private Label label4;
        private Label label5;
    }
}
