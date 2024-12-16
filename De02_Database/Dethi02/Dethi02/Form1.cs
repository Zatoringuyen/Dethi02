using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dethi02.Model;

namespace Dethi02
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool isAdding = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<LoaiSP> listLoaiSP = context.LoaiSP.ToList();
                List<Sanpham> listSP = context.Sanpham.ToList();
                FillCombobox(listLoaiSP);
                BindGrid(listSP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillCombobox(List<LoaiSP> listLoaiSP)
        {
            this.cmbLoaiSP.DataSource = listLoaiSP;
            this.cmbLoaiSP.DisplayMember = "TenLoai";
            this.cmbLoaiSP.ValueMember = "MaLoai";

        }

        private void BindGrid(List<Sanpham> listSP)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listSP)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.MaSP;
                dataGridView1.Rows[index].Cells[1].Value = item.TenSP;
                var LoaiSP = cmbLoaiSP.Items.Cast<LoaiSP>()
            .FirstOrDefault(f => f.MaLoai == item.MaLoai);
                if (LoaiSP != null)
                {
                    dataGridView1.Rows[index].Cells[3].Value = LoaiSP.TenLoai;
                }
                dataGridView1.Rows[index].Cells[2].Value = item.Ngaynhap;
            }
        }

        private void btn_Them_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                    string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                    !DateTime.TryParse(dtNgayNhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new Model1())
                {
                    if (context.Sanpham.Any(s => s.MaSP == txtMaSP.Text))
                    {
                        MessageBox.Show("Mã sản phẩm đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var newSanpham = new Sanpham
                    {
                        MaSP = txtMaSP.Text,
                        TenSP = txtTenSP.Text,
                        Ngaynhap = ngayNhap,
                        MaLoai = cmbLoaiSP.SelectedValue.ToString()
                    };
                    context.Sanpham.Add(newSanpham);
                    context.SaveChanges();
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BindGrid(context.Sanpham.ToList());
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        private void HandleException(Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0) // Đảm bảo không click vào tiêu đề
                {
                    txtMaSP.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    txtTenSP.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value?.ToString();
                    dtNgayNhap.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value?.ToString();
                    cmbLoaiSP.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadFacultyComboBox()
        {
            try
            {
                using (var context = new Model1())
                {
                    var faculties = context.LoaiSP.ToList();
                    cmbLoaiSP.DataSource = faculties;
                    cmbLoaiSP.DisplayMember = "TenLoai";
                    cmbLoaiSP.ValueMember = "MaLoai";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khoa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_Luu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                    string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                    !DateTime.TryParse(dtNgayNhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new Model1())
                {
                    string MaSP = txtMaSP.Text;

                    // Kiểm tra sản phẩm có tồn tại trong cơ sở dữ liệu không
                    var existingProduct = context.Sanpham.FirstOrDefault(s => s.MaSP == MaSP);

                    if (existingProduct != null)
                    {
                        // Nếu sản phẩm đã tồn tại, cập nhật thông tin
                        existingProduct.TenSP = txtTenSP.Text;
                        existingProduct.Ngaynhap = ngayNhap;
                        existingProduct.MaLoai = cmbLoaiSP.SelectedValue.ToString();
                        MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Nếu sản phẩm chưa tồn tại, thêm mới
                        var newProduct = new Sanpham
                        {
                            MaSP = MaSP,
                            TenSP = txtTenSP.Text,
                            Ngaynhap = ngayNhap,
                            MaLoai = cmbLoaiSP.SelectedValue.ToString()
                        };
                        context.Sanpham.Add(newProduct);
                        MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Lưu thay đổi
                    context.SaveChanges();

                    // Làm mới danh sách hiển thị
                    BindGrid(context.Sanpham.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Sua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                    string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                    !DateTime.TryParse(dtNgayNhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new Model1())
                {
                    // Tìm sản phẩm cần sửa theo mã sản phẩm
                    string MaSP = txtMaSP.Text;
                    var productToUpdate = context.Sanpham.FirstOrDefault(s => s.MaSP == MaSP);

                    if (productToUpdate != null)
                    {
                        // Cập nhật thông tin sản phẩm
                        productToUpdate.TenSP = txtTenSP.Text;
                        productToUpdate.Ngaynhap = ngayNhap;
                        productToUpdate.MaLoai = cmbLoaiSP.SelectedValue.ToString();

                        // Lưu thay đổi
                        context.SaveChanges();

                        // Hiển thị lại danh sách
                        BindGrid(context.Sanpham.ToList());

                        MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu chưa chọn sản phẩm
                if (string.IsNullOrWhiteSpace(txtMaSP.Text))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?",
                                                    "Xác nhận xóa",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    using (var context = new Model1())
                    {
                        // Tìm sản phẩm cần xóa
                        string MaSP = txtMaSP.Text;
                        var productToDelete = context.Sanpham.FirstOrDefault(s => s.MaSP == MaSP);

                        if (productToDelete != null)
                        {
                            // Xóa sản phẩm khỏi cơ sở dữ liệu
                            context.Sanpham.Remove(productToDelete);
                            context.SaveChanges();

                            // Hiển thị lại danh sách sản phẩm
                            BindGrid(context.Sanpham.ToList());

                            MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sản phẩm cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_KhongLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu có dòng được chọn trong DataGridView
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Lấy dữ liệu của dòng được chọn
                    var selectedRow = dataGridView1.SelectedRows[0];
                    txtMaSP.Text = selectedRow.Cells[0].Value?.ToString();
                    txtTenSP.Text = selectedRow.Cells[1].Value?.ToString();
                    dtNgayNhap.Text = selectedRow.Cells[2].Value?.ToString();
                    cmbLoaiSP.Text = selectedRow.Cells[3].Value?.ToString();
                }
                else
                {
                    // Nếu không có dòng nào được chọn, xóa toàn bộ ô nhập liệu
                    txtMaSP.Clear();
                    txtTenSP.Clear();
                    dtNgayNhap.Value = DateTime.Now; // Đặt lại ngày hiện tại
                    cmbLoaiSP.SelectedIndex = -1; // Bỏ chọn combobox
                }

                // Hiển thị thông báo
                MessageBox.Show("Dữ liệu đã được khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện khôi phục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình?",
                                       "Xác nhận Thoát",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            // Nếu người dùng chọn "Yes", thoát ứng dụng
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btn_Tim_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy từ khóa tìm kiếm và chuẩn hóa
                var keyword = txtTimKiem.Text.Trim().ToLower();

                using (var context = new Model1()) // Thay "Model1" bằng DbContext thực tế
                {
                    // Kiểm tra từ khóa có rỗng không
                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        MessageBox.Show("Vui lòng nhập từ khóa để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Tìm kiếm sản phẩm theo Mã sản phẩm hoặc Tên sản phẩm
                    var filteredProducts = context.Sanpham
                        .Where(p => p.MaSP.ToLower().Contains(keyword) || p.TenSP.ToLower().Contains(keyword))
                        .ToList();

                    // Kiểm tra kết quả
                    if (!filteredProducts.Any())
                    {
                        dataGridView1.Rows.Clear(); // Xóa toàn bộ dữ liệu trên DataGridView
                        MessageBox.Show("Không tìm thấy sản phẩm nào phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Hiển thị kết quả lên DataGridView
                    BindGrid(filteredProducts);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            btn_Tim_Click(sender, e);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) // Khi nhấn Enter
            {
                btn_Tim_Click(null, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Tim.PerformClick(); // Gọi hàm tìm kiếm khi nhấn Enter
            }
        }
    }    
}
        
    

