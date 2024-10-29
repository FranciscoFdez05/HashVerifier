using System.Security.Cryptography;
using System.Text;

namespace HashVerifier
{
    public partial class Form1 : Form
    {
        private string? selectedHashAlgorithm;

        public Form1()
        {
            InitializeComponent();
            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            comboBox_hashAlogrithm.Items.AddRange(new string[] { "md5", "sha1", "sha224", "sha256", "sha384", "sha512" });
            comboBox_hashAlogrithm.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Open a file to be hashed";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openDialog.FileName;

                    Button source = (Button)sender;
                    switch (source.Name)
                    {
                        case "button1":
                            this.textBox1.Text = filePath;
                            break;
                        case "button2":
                            this.textBox2.Text = filePath;
                            break;
                        case "button3":
                            this.textBox3.Text = filePath;
                            break;
                    }
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Open a file to be hashed";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openDialog.FileName;
                    this.textBox1.Text = filePath; // Mostrar la ruta del archivo en textBox1
                }
            }
        }

        private void button_examinarFileHash_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Open a file to be hashed";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openDialog.FileName;
                    this.textBox4.Text = filePath;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Mostrar la ruta del archivo en textBox1
            this.textBox1.Text = ((TextBox)sender).Text;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Implementar si es necesario
        }

        private byte[] ComputeHash(string filePath, string hashAlgorithm)
        {
            using (var stream = File.OpenRead(filePath))
            {
                HashAlgorithm algorithm = hashAlgorithm switch
                {
                    "md5" => MD5.Create(),
                    "sha1" => SHA1.Create(),
                    "sha256" => SHA256.Create(),
                    "sha384" => SHA384.Create(),
                    "sha512" => SHA512.Create(),
                    _ => throw new InvalidOperationException("Unsupported hash algorithm")
                };

                return algorithm.ComputeHash(stream);
            }
        }

        private void comboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            selectedHashAlgorithm = comboBox_hashAlogrithm.SelectedItem?.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Implementar si es necesario
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Implementar si es necesario
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // al darle al botton que hashe el archivo de la ruta textBox1 en la ruta del textBox4
            if (string.IsNullOrEmpty(this.textBox1.Text) || string.IsNullOrEmpty(this.textBox4.Text))
            {
                MessageBox.Show("Por favor, selecciona las rutas de los archivos.");
                return;
            }

            if (selectedHashAlgorithm == null)
            {
                MessageBox.Show("Por favor, selecciona un algoritmo de hash.");
                return;
            }

            try
            {
                byte[] hashedBytes = this.ComputeHash(this.textBox1.Text, selectedHashAlgorithm);
                string hexHash = ConvertToHex(hashedBytes);
                File.WriteAllText(this.textBox4.Text, hexHash);
                MessageBox.Show("Archivo hasheado y guardado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al hashear el archivo: {ex.Message}");
            }
        }

        private string ConvertToHex(byte[] hashBytes)
        {
            StringBuilder hex = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Open a file to be hashed";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openDialog.FileName;
                    this.textBox2.Text = filePath;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Open a file to be hashed";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openDialog.FileName;
                    this.textBox3.Text = filePath;
                }
            }
        }

        private void button_Verify_Click(object sender, EventArgs e)
        {
            if (selectedHashAlgorithm == null)
            {
                MessageBox.Show("Por favor, selecciona un algoritmo de hash.");
                return;
            }

            try
            {
                byte[] sourceHashedBytes = this.ComputeHash(this.textBox2.Text, selectedHashAlgorithm);
                string sourceHexHash = ConvertToHex(sourceHashedBytes);
                string verificationHexHash = File.ReadAllText(this.textBox3.Text);

                if (sourceHexHash == verificationHexHash)
                {
                    MessageBox.Show("Los hashes coinciden.");
                }
                else
                {
                    MessageBox.Show("Error: los hashes no coinciden.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el hash: {ex.Message}");
            }
        }

        private bool AreArraysIdentical(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }

        private void button_examinarFileHash_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Open a file to be hashed";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openDialog.FileName;
                    this.textBox4.Text = filePath; // Mostrar la ruta del archivo en textBox4
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // Mostrar la ruta del archivo en textBox4
            this.textBox4.Text = ((TextBox)sender).Text;
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            this.textBox2.Clear();
            this.textBox3.Clear();
            this.textBox4.Clear();
        }
    }
}
