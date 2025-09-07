using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
            // sha224 fuera: no está soportado de serie en .NET
            comboBox_hashAlogrithm.Items.AddRange(new string[] { "md5", "sha1", "sha256", "sha384", "sha512" });
            comboBox_hashAlogrithm.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void comboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            selectedHashAlgorithm = comboBox_hashAlogrithm.SelectedItem?.ToString();
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

        private string ConvertToHex(byte[] hashBytes)
        {
            StringBuilder hex = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        // Botón "Source file"
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Select source file";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openDialog.FileName;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = ((TextBox)sender).Text;
        }

        // Botón "Hash" (button_hashear -> Click += button2_Click)
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Select file to hash (Source file)");
                return;
            }

            if (string.IsNullOrWhiteSpace(selectedHashAlgorithm))
            {
                MessageBox.Show("Select a hash algorithm");
                return;
            }

            try
            {
                byte[] hashedBytes = ComputeHash(textBox1.Text, selectedHashAlgorithm!);
                string hexHash = ConvertToHex(hashedBytes);

                // Mostrar en textBox_SourceHash
                textBox_SourceHash.Text = hexHash;

                // Copiar al portapapeles
                Clipboard.SetText(hexHash);

                MessageBox.Show("Hash generated and copied to clipboard");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error hashing the file: {ex.Message}");
            }
        }

        // Botón "Check file"
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new())
            {
                openDialog.Filter = "All files|*.*";
                openDialog.Title = "Select file to verify";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = openDialog.FileName;
                }
            }
        }

        // Botón "Copy" de la sección inferior: pega desde portapapeles al textBox_checkhash
        private void button4_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                textBox_checkhash.Text = Clipboard.GetText();
                MessageBox.Show("Hash pasted from clipboard into the verification field");
            }
            else
            {
                MessageBox.Show("No text in clipboard to paste as hash");
            }
        }

        // Botón "Verify"
        private void button_Verify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedHashAlgorithm))
            {
                MessageBox.Show("Select a hash algorithm");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Select the file to verify (Check file)");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox_checkhash.Text))
            {
                MessageBox.Show("Enter or paste the reference hash");
                return;
            }

            try
            {
                byte[] sourceHashedBytes = ComputeHash(textBox2.Text, selectedHashAlgorithm!);
                string sourceHexHash = ConvertToHex(sourceHashedBytes);

                string pasted = textBox_checkhash.Text.Trim();
                pasted = Regex.Replace(pasted, @"[^0-9a-fA-F]", "");

                bool match = string.Equals(sourceHexHash, pasted, StringComparison.OrdinalIgnoreCase);

                MessageBox.Show(match ? "Hashes match" : "Error: hashes do not match");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error verifying hash: {ex.Message}");
            }
        }

        // Botón "Copy" de la sección superior: copia textBox_SourceHash al portapapeles
        private void button_examinarFileHash_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox_SourceHash.Text))
            {
                Clipboard.SetText(textBox_SourceHash.Text);
                MessageBox.Show("Hash copied to clipboard");
            }
            else
            {
                MessageBox.Show("No hash to copy");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox_SourceHash.Text = ((TextBox)sender).Text;
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

        private void button_clear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox_checkhash.Clear();
            textBox_SourceHash.Clear();
        }
    }
}
