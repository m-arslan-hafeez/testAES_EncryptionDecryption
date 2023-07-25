using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace testAES_EncryptionDecryption
{
    public partial class frmMain : Form
    {
        string unencryptedFilePath;
        string encryptedFilePath;

        public frmMain()
        {
            InitializeComponent();
        }

        private byte[] EncryptFile(string filePath, string password)
        {
            byte[] encryptedData;
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = new byte[32];
                byte[] iv = new byte[16];

                using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(password, iv))
                {
                    key = keyDerivationFunction.GetBytes(32);
                    iv = keyDerivationFunction.GetBytes(16);
                }

                aesAlg.Key = key;
                aesAlg.IV = iv;

                byte[] dataToEncrypt = File.ReadAllBytes(filePath);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                        }
                    }
                    encryptedData = msEncrypt.ToArray();
                }
            }

            return encryptedData;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
                    string password = txtPasswordEncrypt.Text;

                    try
                    {
                        byte[] encryptedData = EncryptFile(unencryptedFilePath, password);
                        string encryptedFilePath = unencryptedFilePath + ".zcryptex";

                        File.WriteAllBytes(encryptedFilePath, encryptedData);

                        MessageBox.Show("File encrypted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error encrypting the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            txtPasswordEncrypt.Clear();
            selectedFile1.Clear();
        }

        private byte[] DecryptFile(string encryptedFilePath, string password)
        {
            byte[] decryptedData;
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = new byte[32];
                byte[] iv = new byte[16];

                using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(password, iv))
                {
                    key = keyDerivationFunction.GetBytes(32);
                    iv = keyDerivationFunction.GetBytes(16);
                }

                aesAlg.Key = key;
                aesAlg.IV = iv;

                byte[] dataToDecrypt = File.ReadAllBytes(encryptedFilePath);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                        {
                            csDecrypt.Write(dataToDecrypt, 0, dataToDecrypt.Length);
                        }
                    }
                    decryptedData = msDecrypt.ToArray();
                }
            }
            return decryptedData;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
                    string password = txtPasswordDecrypt.Text;
                    try
                    {
                        byte[] decryptedData = DecryptFile(encryptedFilePath, password);
                        string originalFilePath = encryptedFilePath.Substring(0, encryptedFilePath.LastIndexOf(".zcryptex"));

                        File.WriteAllBytes(originalFilePath, decryptedData);

                        MessageBox.Show("File decrypted and saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error decrypting the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            txtPasswordDecrypt.Clear();
            selectedFile2.Clear();
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    unencryptedFilePath = openFileDialog.FileName;
                    selectedFile1.Text = unencryptedFilePath;
                }
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Encrypted Files|*.zcryptex";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    encryptedFilePath = openFileDialog.FileName;
                    selectedFile2.Text = encryptedFilePath;

                }
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}