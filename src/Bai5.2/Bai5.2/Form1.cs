using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Bai5._2
{
    public partial class frmStudentRegister : Form
    {
        private string connectionString = "Server=localhost;Database=QuanLyHeThongDB;Uid=root;Pwd=;Port=3306;";

        public frmStudentRegister()
        {
            InitializeComponent();
            this.Load += frmStudentRegister_Load;
        }

        private void frmStudentRegister_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            dtpBirthDate.Format = DateTimePickerFormat.Custom;
            dtpBirthDate.CustomFormat = "dd/MM/yyyy";
            mtxtPhone.Mask = "(000) 000-0000";
            NapDanhSachKhoaHoc();
        }
        private void NapDanhSachKhoaHoc()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT CourseId, CourseName FROM Courses";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboCourse.DataSource = dt;
                    cboCourse.DisplayMember = "CourseName"; 
                    cboCourse.ValueMember = "CourseId";     
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh sách khóa học từ MySQL: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateInputs()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(txtFullName.Text.Trim()))
            {
                errorProvider.SetError(txtFullName, "Vui lòng nhập Họ và tên!");
                isValid = false;
            }

            string rawPhone = mtxtPhone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            if (rawPhone.Length < 10)
            {
                errorProvider.SetError(mtxtPhone, "Vui lòng nhập đủ 10 số điện thoại!");
                isValid = false;
            }

            if (cboCourse.SelectedValue == null)
            {
                errorProvider.SetError(cboCourse, "Vui lòng chọn Khóa học!");
                isValid = false;
            }

            return isValid;
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string fullName = txtFullName.Text.Trim();
            string phone = mtxtPhone.Text;
            string gender = rdoMale.Checked ? "Nam" : "Nữ";
            DateTime birthDate = dtpBirthDate.Value;
            int courseId = Convert.ToInt32(cboCourse.SelectedValue);
            string courseName = cboCourse.Text;
            bool isMember = chkIsMember.Checked;
            string infoSummary = $"--- THÔNG TIN ĐĂNG KÝ HỌC VIÊN ---\n\n" +
                                 $"• Họ và tên: {fullName}\n" +
                                 $"• Điện thoại: {phone}\n" +
                                 $"• Giới tính: {gender}\n" +
                                 $"• Ngày sinh: {birthDate:dd/MM/yyyy}\n" +
                                 $"• Khóa học: {courseName}\n" +
                                 $"• Ưu đãi: {(isMember ? "Học viên thân thiết (Giảm 10%)" : "Không")}";

            MessageBox.Show(infoSummary, "Xác nhận đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Information);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Students (FullName, Phone, Gender, BirthDate, CourseId, IsMember) 
                                     VALUES (@Name, @Phone, @Gender, @BirthDate, @CourseId, @IsMember)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", fullName);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.Parameters.AddWithValue("@IsMember", isMember ? 1 : 0);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Đã lưu thông tin học viên vào CSDL MySQL thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetForm();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu MySQL: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtFullName.Clear();
            mtxtPhone.Clear();
            rdoMale.Checked = true;
            dtpBirthDate.Value = DateTime.Now;
            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;
            chkIsMember.Checked = false;
            errorProvider.Clear();
            txtFullName.Focus();
        }
        private void label2_Click(object sender, EventArgs e) { }
        private void txtFullName_TextChanged(object sender, EventArgs e) { }
        private void rdoMale_CheckedChanged(object sender, EventArgs e) { }
        private void rdoFemale_CheckedChanged(object sender, EventArgs e) { }
        private void mtxtPhone_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { }
        private void dtpBirthDate_ValueChanged(object sender, EventArgs e) { }
        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e) { }
        private void chkIsMember_CheckedChanged(object sender, EventArgs e) { }
    }
}