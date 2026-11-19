using System.Drawing;
using System.Windows.Forms;

namespace Bai_6_4_GarageManagement
{
    partial class  FormMain
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabXe;
        private TabPage tabPhuTung;
        private TabPage tabLapPhieu;
        private TabPage tabHoaDon;

        // Tab 1: Tiếp nhận Xe & Chủ xe
        private TextBox txtTraCuuBienSo, txtBienSo, txtTenKH, txtDienThoai, txtDiaChi, txtHieuXe;
        private NumericUpDown numNamSX;
        private Button btnTraCuuXe, btnTiepNhanXe, btnSuaXe, btnXoaXe, btnMoiXe;
        private DataGridView dgvXe;

        // Tab 2: Quản lý Kho Phụ tùng
        private TextBox txtMaPT, txtTenPT, txtDonGiaPT;
        private NumericUpDown numTonKhoPT;
        private Button btnThemPT, btnSuaPT, btnXoaPT, btnMoiPT;
        private DataGridView dgvPhuTung;

        // Tab 3: Lập Phiếu Sửa chữa (SqlTransaction)
        private ComboBox cboPhieuBienSo, cboPhieuTrangThai, cboChonPhuTung;
        private TextBox txtPhieuChuXe, txtPhieuHieuXe, txtNoiDungSua, txtTienCong;
        private NumericUpDown numSoLuongThay;
        private Button btnThemVaoPhieu, btnXoaKhoiPhieu, btnXacNhanPhieu, btnLamMoiPhieu;
        private DataGridView dgvGioPhuTung;
        private Label lblTongTienTamTinh;

        // Tab 4: Thanh toán & Xuất Hóa đơn
        private ComboBox cboLocTrangThaiPhieu;
        private DataGridView dgvPhieuSuaChua, dgvChiTietHoaDon;
        private RichTextBox rtbHoaDon;
        private Button btnThanhToan, btnLamMoiHoaDon;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Text = "BÀI 6.4 - HỆ THỐNG QUẢN LÝ GARAGE VÀ BẢO DƯỠNG XE Ô TÔ (3-TIER)";
            this.Size = new Size(1360, 790);
            this.MinimumSize = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            tabMain = new TabControl { Dock = DockStyle.Fill };
            tabXe = new TabPage("1. Tiếp nhận Xe && Chủ xe");
            tabPhuTung = new TabPage("2. Quản lý Kho Phụ tùng");
            tabLapPhieu = new TabPage("3. Lập Phiếu Sửa chữa && Thay thế Phụ tùng");
            tabHoaDon = new TabPage("4. Thanh toán && Xuất Hóa đơn Dịch vụ");

            InitTabXe();
            InitTabPhuTung();
            InitTabLapPhieu();
            InitTabHoaDon();

            tabMain.TabPages.AddRange(new[] { tabXe, tabPhuTung, tabLapPhieu, tabHoaDon });
            this.Controls.Add(tabMain);
        }

        private void InitTabXe()
        {
            tabXe.Padding = new Padding(10);

            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu nhanh Biển số xe (SqlParameter)",
                Dock = DockStyle.Top,
                Height = 68
            };

            grpTraCuu.Controls.Add(new Label
            {
                Text = "Nhập Biển số xe:",
                Location = new Point(18, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            });
            txtTraCuuBienSo = new TextBox { Location = new Point(165, 27), Width = 250 };
            btnTraCuuXe = new Button
            {
                Text = "Tra cứu Xe",
                Location = new Point(435, 25),
                Size = new Size(140, 31),
                BackColor = Color.LightSkyBlue
            };
            grpTraCuu.Controls.AddRange(new Control[] { txtTraCuuBienSo, btnTraCuuXe });

            var tblXe = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 8, 0, 0)
            };
            tblXe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tblXe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));

            var grpThongTinXe = new GroupBox
            {
                Text = "Thông tin Xe && Chủ xe",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 6, 0),
                Padding = new Padding(12)
            };

            var tblFormXe = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8
            };
            tblFormXe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblFormXe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 5; i++) tblFormXe.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tblFormXe.RowStyles.Add(new RowStyle(SizeType.Absolute, 75F));
            tblFormXe.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
            tblFormXe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            txtBienSo = new TextBox { Dock = DockStyle.Fill };
            txtHieuXe = new TextBox { Dock = DockStyle.Fill };
            numNamSX = new NumericUpDown { Width = 150, Minimum = 1980, Maximum = 2030, Value = 2022 };
            txtTenKH = new TextBox { Dock = DockStyle.Fill };
            txtDienThoai = new TextBox { Dock = DockStyle.Fill };
            txtDiaChi = new TextBox { Dock = DockStyle.Fill, Multiline = true };

            tblFormXe.Controls.Add(new Label { Text = "Biển số xe:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblFormXe.Controls.Add(txtBienSo, 1, 0);
            tblFormXe.Controls.Add(new Label { Text = "Hiệu xe:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblFormXe.Controls.Add(txtHieuXe, 1, 1);
            tblFormXe.Controls.Add(new Label { Text = "Năm sản xuất:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            tblFormXe.Controls.Add(numNamSX, 1, 2);
            tblFormXe.Controls.Add(new Label { Text = "Tên chủ xe:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            tblFormXe.Controls.Add(txtTenKH, 1, 3);
            tblFormXe.Controls.Add(new Label { Text = "Số điện thoại:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
            tblFormXe.Controls.Add(txtDienThoai, 1, 4);
            tblFormXe.Controls.Add(new Label { Text = "Địa chỉ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, 5);
            tblFormXe.Controls.Add(txtDiaChi, 1, 5);

            var tblBtnXe = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = new Padding(0, 8, 0, 0) };
            tblBtnXe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblBtnXe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblBtnXe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tblBtnXe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            btnTiepNhanXe = new Button { Text = "Tiếp nhận Xe mới", Dock = DockStyle.Fill, BackColor = Color.LightGreen, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnSuaXe = new Button { Text = "Cập nhật Thông tin", Dock = DockStyle.Fill };
            btnXoaXe = new Button { Text = "Xóa Xe", Dock = DockStyle.Fill };
            btnMoiXe = new Button { Text = "Làm mới Form", Dock = DockStyle.Fill };

            tblBtnXe.Controls.Add(btnTiepNhanXe, 0, 0);
            tblBtnXe.Controls.Add(btnSuaXe, 1, 0);
            tblBtnXe.Controls.Add(btnXoaXe, 0, 1);
            tblBtnXe.Controls.Add(btnMoiXe, 1, 1);

            tblFormXe.Controls.Add(tblBtnXe, 0, 6);
            tblFormXe.SetColumnSpan(tblBtnXe, 2);
            grpThongTinXe.Controls.Add(tblFormXe);

            var grpDanhSachXe = new GroupBox
            {
                Text = "Danh sách Xe đang quản lý tại Garage",
                Dock = DockStyle.Fill,
                Margin = new Padding(6, 0, 0, 0)
            };

            dgvXe = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };
            grpDanhSachXe.Controls.Add(dgvXe);

            tblXe.Controls.Add(grpThongTinXe, 0, 0);
            tblXe.Controls.Add(grpDanhSachXe, 1, 0);

            tabXe.Controls.Add(tblXe);
            tabXe.Controls.Add(grpTraCuu);
        }

        private void InitTabPhuTung()
        {
            tabPhuTung.Padding = new Padding(10);

            var tblPT = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            tblPT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
            tblPT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));

            var grpNhapPT = new GroupBox
            {
                Text = "Thông tin Phụ tùng / Vật tư",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 6, 0),
                Padding = new Padding(12)
            };

            var tblFormPT = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6
            };
            tblFormPT.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblFormPT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 4; i++) tblFormPT.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tblFormPT.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
            tblFormPT.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            txtMaPT = new TextBox { Width = 120, ReadOnly = true };
            txtTenPT = new TextBox { Dock = DockStyle.Fill };
            txtDonGiaPT = new TextBox { Dock = DockStyle.Fill };
            numTonKhoPT = new NumericUpDown { Width = 150, Minimum = 0, Maximum = 100000, Value = 10 };

            tblFormPT.Controls.Add(new Label { Text = "Mã phụ tùng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblFormPT.Controls.Add(txtMaPT, 1, 0);
            tblFormPT.Controls.Add(new Label { Text = "Tên phụ tùng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblFormPT.Controls.Add(txtTenPT, 1, 1);
            tblFormPT.Controls.Add(new Label { Text = "Đơn giá (VNĐ):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            tblFormPT.Controls.Add(txtDonGiaPT, 1, 2);
            tblFormPT.Controls.Add(new Label { Text = "Số lượng tồn:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            tblFormPT.Controls.Add(numTonKhoPT, 1, 3);

            var tblBtnPT = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = new Padding(0, 8, 0, 0) };
            tblBtnPT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblBtnPT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblBtnPT.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tblBtnPT.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            btnThemPT = new Button { Text = "Thêm Phụ tùng", Dock = DockStyle.Fill, BackColor = Color.LightGreen, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnSuaPT = new Button { Text = "Cập nhật Giá && Tồn", Dock = DockStyle.Fill };
            btnXoaPT = new Button { Text = "Xóa Phụ tùng", Dock = DockStyle.Fill };
            btnMoiPT = new Button { Text = "Làm mới Form", Dock = DockStyle.Fill };

            tblBtnPT.Controls.Add(btnThemPT, 0, 0);
            tblBtnPT.Controls.Add(btnSuaPT, 1, 0);
            tblBtnPT.Controls.Add(btnXoaPT, 0, 1);
            tblBtnPT.Controls.Add(btnMoiPT, 1, 1);

            tblFormPT.Controls.Add(tblBtnPT, 0, 4);
            tblFormPT.SetColumnSpan(tblBtnPT, 2);
            grpNhapPT.Controls.Add(tblFormPT);

            var grpDanhSachPT = new GroupBox
            {
                Text = "Danh mục Phụ tùng && Số lượng Tồn kho",
                Dock = DockStyle.Fill,
                Margin = new Padding(6, 0, 0, 0)
            };

            dgvPhuTung = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };
            grpDanhSachPT.Controls.Add(dgvPhuTung);

            tblPT.Controls.Add(grpNhapPT, 0, 0);
            tblPT.Controls.Add(grpDanhSachPT, 1, 0);
            tabPhuTung.Controls.Add(tblPT);
        }

        private void InitTabLapPhieu()
        {
            tabLapPhieu.Padding = new Padding(10);

            // --- KHUNG 1: THÔNG TIN XE & NỘI DUNG SỬA ---
            var grpThongTinPhieu = new GroupBox
            {
                Text = "1. Thông tin Xe tiếp nhận && Nội dung sửa chữa",
                Dock = DockStyle.Top,
                Height = 145,
                Padding = new Padding(10)
            };

            var tblTopPhieu = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 2
            };
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblTopPhieu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));

            tblTopPhieu.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tblTopPhieu.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            cboPhieuBienSo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            txtPhieuChuXe = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };
            txtPhieuHieuXe = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };
            cboPhieuTrangThai = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPhieuTrangThai.Items.AddRange(new object[] { "Đang sửa", "Đã hoàn thành" });
            cboPhieuTrangThai.SelectedIndex = 1;
            txtNoiDungSua = new TextBox { Dock = DockStyle.Fill, Multiline = true };

            tblTopPhieu.Controls.Add(new Label { Text = "Chọn Biển số xe:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) }, 0, 0);
            tblTopPhieu.Controls.Add(cboPhieuBienSo, 1, 0);
            tblTopPhieu.Controls.Add(new Label { Text = "Chủ xe:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 2, 0);
            tblTopPhieu.Controls.Add(txtPhieuChuXe, 3, 0);
            tblTopPhieu.Controls.Add(new Label { Text = "Hiệu xe:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 4, 0);
            tblTopPhieu.Controls.Add(txtPhieuHieuXe, 5, 0);
            tblTopPhieu.Controls.Add(new Label { Text = "Trạng thái phiếu:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 6, 0);
            tblTopPhieu.Controls.Add(cboPhieuTrangThai, 7, 0);

            tblTopPhieu.Controls.Add(new Label { Text = "Nội dung sửa chữa:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, 1);
            tblTopPhieu.Controls.Add(txtNoiDungSua, 1, 1);
            tblTopPhieu.SetColumnSpan(txtNoiDungSua, 7);

            grpThongTinPhieu.Controls.Add(tblTopPhieu);

            // --- KHUNG 2: CHỌN PHỤ TÙNG THAY THẾ & TIỀN CÔNG ---
            var grpChonPT = new GroupBox
            {
                Text = "2. Thêm Phụ tùng thay thế && Tiền công vào phiếu",
                Dock = DockStyle.Top,
                Height = 76
            };

            grpChonPT.Controls.Add(new Label { Text = "Chọn Phụ tùng:", Location = new Point(15, 34), AutoSize = true });
            cboChonPhuTung = new ComboBox { Location = new Point(135, 30), Width = 390, DropDownStyle = ComboBoxStyle.DropDownList };

            grpChonPT.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(545, 34), AutoSize = true });
            numSoLuongThay = new NumericUpDown { Location = new Point(625, 31), Width = 75, Minimum = 1, Maximum = 1000, Value = 1 };

            grpChonPT.Controls.Add(new Label { Text = "Tiền công (VNĐ):", Location = new Point(720, 34), AutoSize = true });
            txtTienCong = new TextBox { Location = new Point(850, 31), Width = 140, Text = "100000" };

            btnThemVaoPhieu = new Button { Text = "+ Thêm vào Phiếu", Location = new Point(1010, 28), Size = new Size(145, 32), BackColor = Color.LightSkyBlue };
            btnXoaKhoiPhieu = new Button { Text = "Xóa dòng chọn", Location = new Point(1165, 28), Size = new Size(115, 32) };

            grpChonPT.Controls.AddRange(new Control[]
            {
                cboChonPhuTung, numSoLuongThay, txtTienCong, btnThemVaoPhieu, btnXoaKhoiPhieu
            });

            // --- KHUNG 3: THANH XÁC NHẬN DƯỚI ĐÁY ---
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 62, Padding = new Padding(0, 8, 0, 4) };
            var tblBottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340F));

            lblTongTienTamTinh = new Label
            {
                Text = "Tổng chi phí dự kiến: 0 VNĐ",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            btnLamMoiPhieu = new Button { Text = "Làm mới Phiếu", Dock = DockStyle.Fill, Margin = new Padding(4, 2, 4, 2) };
            btnXacNhanPhieu = new Button
            {
                Text = "Xác nhận Hoàn tất && Xuất Kho (SqlTransaction)",
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 2, 0, 2),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            tblBottom.Controls.Add(lblTongTienTamTinh, 0, 0);
            tblBottom.Controls.Add(btnLamMoiPhieu, 1, 0);
            tblBottom.Controls.Add(btnXacNhanPhieu, 2, 0);
            pnlBottom.Controls.Add(tblBottom);

            dgvGioPhuTung = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            tabLapPhieu.Controls.Add(dgvGioPhuTung);
            tabLapPhieu.Controls.Add(pnlBottom);
            tabLapPhieu.Controls.Add(grpChonPT);
            tabLapPhieu.Controls.Add(grpThongTinPhieu);
        }

        private void InitTabHoaDon()
        {
            var tblHoaDon = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            tblHoaDon.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            tblHoaDon.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));

            // --- CỘT TRÁI: BỘ LỌC + BẢNG PHIẾU + BẢNG CHI TIẾT ---
            var pnlLeft = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 6, 0) };

            var grpFilterHD = new GroupBox { Text = "Danh sách Phiếu sửa chữa", Dock = DockStyle.Top, Height = 68 };
            grpFilterHD.Controls.Add(new Label { Text = "Lọc theo trạng thái:", Location = new Point(15, 30), AutoSize = true });
            cboLocTrangThaiPhieu = new ComboBox { Location = new Point(165, 26), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTrangThaiPhieu.Items.AddRange(new object[] { "Tất cả", "Đang sửa", "Đã hoàn thành", "Đã thanh toán" });
            cboLocTrangThaiPhieu.SelectedIndex = 0;

            btnLamMoiHoaDon = new Button { Text = "Tải lại Danh sách", Location = new Point(385, 24), Size = new Size(145, 30) };
            grpFilterHD.Controls.AddRange(new Control[] { cboLocTrangThaiPhieu, btnLamMoiHoaDon });

            var tblGridsHD = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0, 6, 0, 0)
            };
            tblGridsHD.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            tblGridsHD.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));

            dgvPhieuSuaChua = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            var grpCTHD = new GroupBox { Text = "Chi tiết phụ tùng && tiền công của phiếu đang chọn", Dock = DockStyle.Fill, Margin = new Padding(0, 6, 0, 0) };
            dgvChiTietHoaDon = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };
            grpCTHD.Controls.Add(dgvChiTietHoaDon);

            tblGridsHD.Controls.Add(dgvPhieuSuaChua, 0, 0);
            tblGridsHD.Controls.Add(grpCTHD, 0, 1);

            pnlLeft.Controls.Add(tblGridsHD);
            pnlLeft.Controls.Add(grpFilterHD);

            // --- CỘT PHẢI: XEM TRƯỚC HÓA ĐƠN & NÚT THANH TOÁN ---
            var grpPreviewHD = new GroupBox
            {
                Text = "Hóa đơn Dịch vụ Sửa chữa && Thanh toán",
                Dock = DockStyle.Fill,
                Margin = new Padding(6, 0, 0, 0),
                Padding = new Padding(10)
            };

            var tblRightHD = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            tblRightHD.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRightHD.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));

            rtbHoaDon = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10F),
                BackColor = Color.Ivory
            };

            btnThanhToan = new Button
            {
                Text = "Xác nhận Thanh toán && Xuất Hóa đơn",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 0),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            tblRightHD.Controls.Add(rtbHoaDon, 0, 0);
            tblRightHD.Controls.Add(btnThanhToan, 0, 1);
            grpPreviewHD.Controls.Add(tblRightHD);

            tblHoaDon.Controls.Add(pnlLeft, 0, 0);
            tblHoaDon.Controls.Add(grpPreviewHD, 1, 0);
            tabHoaDon.Controls.Add(tblHoaDon);
        }
    }
}