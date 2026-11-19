using System.Drawing;
using System.Windows.Forms;

namespace Bai_6_5_KaraokeManagement
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabSoDoPhong;
        private TabPage tabDanhMuc;

        // Tab 1: Sơ đồ phòng hát & Gọi dịch vụ - Thanh toán
        private ComboBox cboLocTrangThaiPhong;
        private Button btnTaiLaiSoDo, btnMoPhong, btnThemDichVu, btnXoaDichVu, btnThanhToan;
        private FlowLayoutPanel flpPhongHat;
        private Label lblPhongDangChon, lblLoaiGiaPhong, lblMaHD, lblGioVao, lblThoiLuongHat;
        private Label lblTienPhong, lblTienDichVu, lblTongThanhToan;
        private ComboBox cboChonDichVu;
        private NumericUpDown numSoLuongDV;
        private DataGridView dgvChiTietDichVu;
        private System.Windows.Forms.Timer timerTinhGio;

        // Tab 2: Quản lý Danh mục Phòng hát & Dịch vụ kho
        private TextBox txtMaPhong, txtTenPhong, txtGiaTheoGio;
        private ComboBox cboLoaiPhong;
        private Button btnThemPhong, btnSuaPhong, btnXoaPhong, btnMoiPhong;
        private DataGridView dgvPhongHat;

        private TextBox txtMaDV, txtTenDV, txtDonGiaDV;
        private NumericUpDown numTonKhoDV;
        private Button btnThemDV, btnSuaDV, btnXoaDV, btnMoiDV;
        private DataGridView dgvDichVu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.None;
            this.Text = "BÀI 6.5 - HỆ THỐNG QUẢN LÝ ĐẶT PHÒNG && DỊCH VỤ KARAOKE (3-TIER)";
            this.Size = new Size(1380, 800);
            this.MinimumSize = new Size(1300, 740);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            timerTinhGio = new System.Windows.Forms.Timer(this.components) { Interval = 1000 };

            tabMain = new TabControl { Dock = DockStyle.Fill };
            tabSoDoPhong = new TabPage("1. Sơ đồ Phòng hát && Gọi dịch vụ - Thanh toán");
            tabDanhMuc = new TabPage("2. Quản lý Danh mục Phòng hát && Kho Dịch vụ");

            InitTabSoDoPhong();
            InitTabDanhMuc();

            tabMain.TabPages.AddRange(new[] { tabSoDoPhong, tabDanhMuc });
            this.Controls.Add(tabMain);
        }

        private void InitTabSoDoPhong()
        {
            var tblMain = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));

            // ================= CỘT TRÁI: SƠ ĐỒ PHÒNG HÁT =================
            var grpSoDo = new GroupBox
            {
                Text = "Sơ đồ Phòng hát Trực quan (Visual Room Grid)",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 6, 0),
                Padding = new Padding(10)
            };

            var tblLeft = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            // Thanh lọc & Chú thích màu
            var pnlFilter = new Panel { Dock = DockStyle.Fill };
            pnlFilter.Controls.Add(new Label { Text = "Lọc trạng thái:", Location = new Point(4, 12), AutoSize = true });
            cboLocTrangThaiPhong = new ComboBox { Location = new Point(115, 9), Width = 135, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTrangThaiPhong.Items.AddRange(new object[] { "Tất cả", "Trống", "Đang hát" });
            cboLocTrangThaiPhong.SelectedIndex = 0;

            var lblLegendTrong = new Label
            {
                Text = "  Trống  ",
                Location = new Point(265, 11),
                AutoSize = true,
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            var lblLegendHat = new Label
            {
                Text = " Đang hát ",
                Location = new Point(340, 11),
                AutoSize = true,
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            btnTaiLaiSoDo = new Button { Text = "Làm mới sơ đồ", Location = new Point(445, 7), Size = new Size(135, 30) };
            pnlFilter.Controls.AddRange(new Control[] { cboLocTrangThaiPhong, lblLegendTrong, lblLegendHat, btnTaiLaiSoDo });

            // Lưới Button phòng động
            flpPhongHat = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };

            // Thanh Mở phòng phía dưới
            var tblMoPhong = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(0, 6, 0, 0) };
            tblMoPhong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblMoPhong.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F));

            lblPhongDangChon = new Label
            {
                Text = "Đang chọn: Chưa chọn phòng",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            btnMoPhong = new Button
            {
                Text = "Mở phòng (Check-in)",
                Dock = DockStyle.Fill,
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            tblMoPhong.Controls.Add(lblPhongDangChon, 0, 0);
            tblMoPhong.Controls.Add(btnMoPhong, 1, 0);

            tblLeft.Controls.Add(pnlFilter, 0, 0);
            tblLeft.Controls.Add(flpPhongHat, 0, 1);
            tblLeft.Controls.Add(tblMoPhong, 0, 2);
            grpSoDo.Controls.Add(tblLeft);

            // ================= CỘT PHẢI: HÓA ĐƠN, GỌI DỊCH VỤ & THANH TOÁN =================
            var tblRight = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(6, 0, 0, 0)
            };
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 135F));

            // 1. Thông tin giờ hát
            var grpGioHat = new GroupBox
            {
                Text = "Thông tin Hóa đơn && Tính giờ tự động",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            var tblInfo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3 };
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int i = 0; i < 3; i++) tblInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));

            lblMaHD = new Label { Text = "--", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            lblLoaiGiaPhong = new Label { Text = "--", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblGioVao = new Label { Text = "--", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblThoiLuongHat = new Label { Text = "0 giờ 0 phút", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.DarkRed };

            tblInfo.Controls.Add(new Label { Text = "Mã Hóa đơn:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblInfo.Controls.Add(lblMaHD, 1, 0);
            tblInfo.Controls.Add(new Label { Text = "Loại / Giá giờ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);
            tblInfo.Controls.Add(lblLoaiGiaPhong, 3, 0);

            tblInfo.Controls.Add(new Label { Text = "Giờ vào:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblInfo.Controls.Add(lblGioVao, 1, 1);
            tblInfo.Controls.Add(new Label { Text = "Thời gian hát:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 1);
            tblInfo.Controls.Add(lblThoiLuongHat, 3, 1);

            tblInfo.Controls.Add(new Label { Text = "Mô hình dịch vụ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            tblInfo.Controls.Add(new Label { Text = "Disconnected Model (SqlDataAdapter + DataTable)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray }, 1, 2);
            tblInfo.SetColumnSpan(tblInfo.GetControlFromPosition(1, 2), 3);

            grpGioHat.Controls.Add(tblInfo);

            // 2. Gọi dịch vụ / Đồ uống (DataTable)
            var grpDichVu = new GroupBox
            {
                Text = "Gọi Dịch vụ / Đồ uống (Disconnected DataTable)",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 6),
                Padding = new Padding(10)
            };

            var tblDV = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            tblDV.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tblDV.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var tblOrderBar = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1 };
            tblOrderBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblOrderBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tblOrderBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tblOrderBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tblOrderBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));

            cboChonDichVu = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            numSoLuongDV = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 500, Value = 1 };
            btnThemDichVu = new Button { Text = "+ Thêm món", Dock = DockStyle.Fill, Margin = new Padding(6, 0, 4, 4), BackColor = Color.LightSkyBlue };
            btnXoaDichVu = new Button { Text = "Xóa món", Dock = DockStyle.Fill, Margin = new Padding(2, 0, 0, 4) };

            tblOrderBar.Controls.Add(cboChonDichVu, 0, 0);
            tblOrderBar.Controls.Add(new Label { Text = "SL:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 1, 0);
            tblOrderBar.Controls.Add(numSoLuongDV, 2, 0);
            tblOrderBar.Controls.Add(btnThemDichVu, 3, 0);
            tblOrderBar.Controls.Add(btnXoaDichVu, 4, 0);

            dgvChiTietDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            tblDV.Controls.Add(tblOrderBar, 0, 0);
            tblDV.Controls.Add(dgvChiTietDichVu, 0, 1);
            grpDichVu.Controls.Add(tblDV);

            // 3. Tổng hợp & Thanh toán (SqlTransaction)
            var grpThanhToan = new GroupBox
            {
                Text = "Tính tiền && Thanh toán Hóa đơn (SqlTransaction)",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var tblPay = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            tblPay.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165F));
            tblPay.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblPay.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            for (int i = 0; i < 3; i++) tblPay.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));

            lblTienPhong = new Label { Text = "0 VNĐ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblTienDichVu = new Label { Text = "0 VNĐ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblTongThanhToan = new Label
            {
                Text = "0 VNĐ",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            btnThanhToan = new Button
            {
                Text = "Thanh toán && Trả phòng\n(SqlTransaction)",
                Dock = DockStyle.Fill,
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            tblPay.Controls.Add(new Label { Text = "Tiền giờ phòng tạm tính:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblPay.Controls.Add(lblTienPhong, 1, 0);

            tblPay.Controls.Add(new Label { Text = "Tổng tiền dịch vụ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblPay.Controls.Add(lblTienDichVu, 1, 1);

            tblPay.Controls.Add(new Label { Text = "TỔNG THANH TOÁN:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }, 0, 2);
            tblPay.Controls.Add(lblTongThanhToan, 1, 2);

            tblPay.Controls.Add(btnThanhToan, 2, 0);
            tblPay.SetRowSpan(btnThanhToan, 3);

            grpThanhToan.Controls.Add(tblPay);

            tblRight.Controls.Add(grpGioHat, 0, 0);
            tblRight.Controls.Add(grpDichVu, 0, 1);
            tblRight.Controls.Add(grpThanhToan, 0, 2);

            tblMain.Controls.Add(grpSoDo, 0, 0);
            tblMain.Controls.Add(tblRight, 1, 0);
            tabSoDoPhong.Controls.Add(tblMain);
        }

        private void InitTabDanhMuc()
        {
            var tblDM = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            tblDM.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblDM.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // --- CỘT TRÁI: QUẢN LÝ PHÒNG HÁT ---
            var grpQLPhong = new GroupBox { Text = "Quản lý Danh sách Phòng hát", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 6, 0), Padding = new Padding(12) };
            var tblQLP = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6 };
            tblQLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblQLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 4; i++) tblQLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblQLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tblQLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            txtMaPhong = new TextBox { Width = 100, ReadOnly = true };
            txtTenPhong = new TextBox { Dock = DockStyle.Fill };
            cboLoaiPhong = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoaiPhong.Items.AddRange(new object[] { "VIP", "Thường" });
            cboLoaiPhong.SelectedIndex = 0;
            txtGiaTheoGio = new TextBox { Dock = DockStyle.Fill };

            tblQLP.Controls.Add(new Label { Text = "Mã phòng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblQLP.Controls.Add(txtMaPhong, 1, 0);
            tblQLP.Controls.Add(new Label { Text = "Tên phòng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblQLP.Controls.Add(txtTenPhong, 1, 1);
            tblQLP.Controls.Add(new Label { Text = "Loại phòng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            tblQLP.Controls.Add(cboLoaiPhong, 1, 2);
            tblQLP.Controls.Add(new Label { Text = "Giá theo giờ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            tblQLP.Controls.Add(txtGiaTheoGio, 1, 3);

            var tblBtnP = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Margin = new Padding(0, 4, 0, 6) };
            for (int i = 0; i < 4; i++) tblBtnP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            btnThemPhong = new Button { Text = "Thêm phòng", Dock = DockStyle.Fill };
            btnSuaPhong = new Button { Text = "Sửa phòng", Dock = DockStyle.Fill };
            btnXoaPhong = new Button { Text = "Xóa phòng", Dock = DockStyle.Fill };
            btnMoiPhong = new Button { Text = "Làm mới", Dock = DockStyle.Fill };
            tblBtnP.Controls.AddRange(new Control[] { btnThemPhong, btnSuaPhong, btnXoaPhong, btnMoiPhong });

            dgvPhongHat = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            tblQLP.Controls.Add(tblBtnP, 0, 4);
            tblQLP.SetColumnSpan(tblBtnP, 2);
            tblQLP.Controls.Add(dgvPhongHat, 0, 5);
            tblQLP.SetColumnSpan(dgvPhongHat, 2);
            grpQLPhong.Controls.Add(tblQLP);

            // --- CỘT PHẢI: QUẢN LÝ DỊCH VỤ & TỒN KHO ---
            var grpQLDV = new GroupBox { Text = "Quản lý Danh mục Dịch vụ && Kho Đồ uống", Dock = DockStyle.Fill, Margin = new Padding(6, 0, 0, 0), Padding = new Padding(12) };
            var tblQLDV = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6 };
            tblQLDV.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblQLDV.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 4; i++) tblQLDV.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblQLDV.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tblQLDV.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            txtMaDV = new TextBox { Width = 100, ReadOnly = true };
            txtTenDV = new TextBox { Dock = DockStyle.Fill };
            txtDonGiaDV = new TextBox { Dock = DockStyle.Fill };
            numTonKhoDV = new NumericUpDown { Width = 150, Minimum = 0, Maximum = 100000, Value = 50 };

            tblQLDV.Controls.Add(new Label { Text = "Mã dịch vụ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblQLDV.Controls.Add(txtMaDV, 1, 0);
            tblQLDV.Controls.Add(new Label { Text = "Tên dịch vụ:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblQLDV.Controls.Add(txtTenDV, 1, 1);
            tblQLDV.Controls.Add(new Label { Text = "Đơn giá:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            tblQLDV.Controls.Add(txtDonGiaDV, 1, 2);
            tblQLDV.Controls.Add(new Label { Text = "Tồn kho:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            tblQLDV.Controls.Add(numTonKhoDV, 1, 3);

            var tblBtnDV = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Margin = new Padding(0, 4, 0, 6) };
            for (int i = 0; i < 4; i++) tblBtnDV.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            btnThemDV = new Button { Text = "Thêm DV", Dock = DockStyle.Fill };
            btnSuaDV = new Button { Text = "Sửa DV", Dock = DockStyle.Fill };
            btnXoaDV = new Button { Text = "Xóa DV", Dock = DockStyle.Fill };
            btnMoiDV = new Button { Text = "Làm mới", Dock = DockStyle.Fill };
            tblBtnDV.Controls.AddRange(new Control[] { btnThemDV, btnSuaDV, btnXoaDV, btnMoiDV });

            dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            tblQLDV.Controls.Add(tblBtnDV, 0, 4);
            tblQLDV.SetColumnSpan(tblBtnDV, 2);
            tblQLDV.Controls.Add(dgvDichVu, 0, 5);
            tblQLDV.SetColumnSpan(dgvDichVu, 2);
            grpQLDV.Controls.Add(tblQLDV);

            tblDM.Controls.Add(grpQLPhong, 0, 0);
            tblDM.Controls.Add(grpQLDV, 1, 0);
            tabDanhMuc.Controls.Add(tblDM);
        }
    }
}