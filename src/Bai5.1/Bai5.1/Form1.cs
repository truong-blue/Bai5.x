using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Bai5._1
{
    public partial class frmLogin : Form
    {
        private string connectionString = "Server=localhost;Database=QuanLyHeThongDB;Uid=root;Pwd=;Port=3306;";

        public frmLogin()
        {
            InitializeComponent();
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.AcceptButton = btnLogin; 
            this.CancelButton = btnExit;   

            txtPassword.UseSystemPasswordChar = true;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
        private bool ValidateInputs()
        {
            bool isValid = true;
            errorProvider.Clear(); 

            if (string.IsNullOrWhiteSpace(txtUsername.Text.Trim()))
            {
                errorProvider.SetError(txtUsername, "Vui lòng nhập Tên đăng nhập!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text.Trim()))
            {
                errorProvider.SetError(txtPassword, "Vui lòng nhập Mật khẩu!");
                isValid = false;
            }

            return isValid;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                // 1. Dùng MySqlConnection kết nối MySQL XAMPP
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @User AND Password = @Pass AND IsActive = 1";

                    // 2. Dùng MySqlCommand
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@User", username);
                        cmd.Parameters.AddWithValue("@Pass", password);

                        // 3. Ép kiểu Convert.ToInt32 để tránh lỗi văng Cast Exception từ MySQL
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show($"Đăng nhập thành công! Chào mừng {username}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL MySQL: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}