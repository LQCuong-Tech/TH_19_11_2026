using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bai_6_5_KaraokeManagement.BUS;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement
{
    public partial class FormMain : Form
    {
        private readonly PhongHatBUS _phongBus = new PhongHatBUS();
        private readonly DichVuBUS _dvBus = new DichVuBUS();
        private readonly HoaDonBUS _hdBus = new HoaDonBUS();

        private PhongHatDTO _phongDangChon = null;
        private HoaDonDTO _hoaDonHienTai = null;
        private DataTable _dtChiTietDichVu = null;

        public FormMain()
        {
            InitializeComponent();
            DangKySuKien();
            this.Load += FrmMain_Load;
        }

        private void DangKySuKien()
        {
            // Tab 1: Sơ đồ phòng & Gọi dịch vụ & Thanh toán
            cboLocTrangThaiPhong.SelectedIndexChanged += (s, e) => TaiSoDoPhong();
            btnTaiLaiSoDo.Click += (s, e) => TaiSoDoPhong();
            btnMoPhong.Click += BtnMoPhong_Click;
            btnThemDichVu.Click += BtnThemDichVu_Click;
            btnXoaDichVu.Click += BtnXoaDichVu_Click;
            btnThanhToan.Click += BtnThanhToan_Click;
            timerTinhGio.Tick += (s, e) => CapNhatThongTinTinhTienTamThoi();

            // Tab 2: Quản lý danh mục Phòng & Dịch vụ
            dgvPhongHat.CellClick += DgvPhongHat_CellClick;
            btnThemPhong.Click += BtnThemPhong_Click;
            btnSuaPhong.Click += BtnSuaPhong_Click;
            btnXoaPhong.Click += BtnXoaPhong_Click;
            btnMoiPhong.Click += (s, e) => LamMoiFormPhong();

            dgvDichVu.CellClick += DgvDichVu_CellClick;
            btnThemDV.Click += BtnThemDV_Click;
            btnSuaDV.Click += BtnSuaDV_Click;
            btnXoaDV.Click += BtnXoaDV_Click;
            btnMoiDV.Click += (s, e) => LamMoiFormDV();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                TaiToanBoDuLieu();
                timerTinhGio.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL KaraokeDB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaiToanBoDuLieu()
        {
            TaiSoDoPhong();
            TaiDanhSachDichVu();
            TaiBangQuanLyPhong();
        }

        #region 1. SƠ ĐỒ PHÒNG HÁT & MỞ PHÒNG (VISUAL GRID)
        private void TaiSoDoPhong()
        {
            string trangThaiLoc = cboLocTrangThaiPhong.SelectedItem?.ToString();
            var dsPhong = _phongBus.LayTatCa(trangThaiLoc);

            flpPhongHat.Controls.Clear();

            foreach (var phong in dsPhong)
            {
                bool dangHat = phong.TrangThai == "Đang hát";
                var btn = new Button
                {
                    Width = 165,
                    Height = 96,
                    Margin = new Padding(8),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = dangHat ? Color.IndianRed : Color.MediumSeaGreen,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Text = $"{phong.TenPhong}\n[{phong.LoaiPhong}] - {phong.GiaTheoGio:N0}đ/h\n● {phong.TrangThai}",
                    Tag = phong,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = (_phongDangChon != null && _phongDangChon.MaPhong == phong.MaPhong) ? 3 : 1;
                btn.FlatAppearance.BorderColor = (_phongDangChon != null && _phongDangChon.MaPhong == phong.MaPhong) ? Color.Navy : Color.DimGray;

                btn.Click += BtnPhong_Click;
                flpPhongHat.Controls.Add(btn);
            }

            // Cập nhật lại trạng thái phòng đang chọn nếu vẫn tồn tại
            if (_phongDangChon != null)
            {
                var updated = dsPhong.FirstOrDefault(x => x.MaPhong == _phongDangChon.MaPhong);
                if (updated != null)
                    ChonPhong(updated);
                else
                    DatLaiKhungHoaDon();
            }
        }

        private void BtnPhong_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedBtn && clickedBtn.Tag is PhongHatDTO phong)
            {
                foreach (Control ctrl in flpPhongHat.Controls)
                {
                    if (ctrl is Button b)
                    {
                        b.FlatAppearance.BorderSize = 1;
                        b.FlatAppearance.BorderColor = Color.DimGray;
                    }
                }
                clickedBtn.FlatAppearance.BorderSize = 3;
                clickedBtn.FlatAppearance.BorderColor = Color.Navy;

                ChonPhong(phong);
            }
        }

        private void ChonPhong(PhongHatDTO phong)
        {
            _phongDangChon = phong;
            lblPhongDangChon.Text = $"Đang chọn: {phong.TenPhong} ({phong.TrangThai})";
            lblLoaiGiaPhong.Text = $"{phong.LoaiPhong} - {phong.GiaTheoGio:N0} VNĐ/giờ";

            if (phong.TrangThai == "Trống")
            {
                _hoaDonHienTai = null;
                _dtChiTietDichVu = null;
                dgvChiTietDichVu.DataSource = null;

                btnMoPhong.Enabled = true;
                btnThemDichVu.Enabled = false;
                btnXoaDichVu.Enabled = false;
                btnThanhToan.Enabled = false;

                lblMaHD.Text = "Chưa mở phòng";
                lblGioVao.Text = "--";
                lblThoiLuongHat.Text = "0 giờ 0 phút";
                lblTienPhong.Text = "0 VNĐ";
                lblTienDichVu.Text = "0 VNĐ";
                lblTongThanhToan.Text = "0 VNĐ";
            }
            else
            {
                btnMoPhong.Enabled = false;
                btnThemDichVu.Enabled = true;
                btnXoaDichVu.Enabled = true;
                btnThanhToan.Enabled = true;

                _hoaDonHienTai = _hdBus.LayHoaDonDangHoatDong(phong.MaPhong);
                if (_hoaDonHienTai != null)
                {
                    lblMaHD.Text = $"#{_hoaDonHienTai.MaHD}";
                    lblGioVao.Text = _hoaDonHienTai.GioVao.ToString("dd/MM/yyyy HH:mm:ss");

                    // Tải danh sách dịch vụ đã đặt qua mô hình ngắt kết nối (SqlDataAdapter + DataTable)
                    _dtChiTietDichVu = _hdBus.LayDataTableChiTietDichVu(_hoaDonHienTai.MaHD);
                    dgvChiTietDichVu.DataSource = _dtChiTietDichVu;
                    DinhDangCotChiTietDichVu();

                    CapNhatThongTinTinhTienTamThoi();
                }
            }
        }

        private void DatLaiKhungHoaDon()
        {
            _phongDangChon = null;
            _hoaDonHienTai = null;
            _dtChiTietDichVu = null;
            dgvChiTietDichVu.DataSource = null;
            lblPhongDangChon.Text = "Đang chọn: Chưa chọn phòng";
            lblLoaiGiaPhong.Text = "--";
            lblMaHD.Text = "--";
            lblGioVao.Text = "--";
            lblThoiLuongHat.Text = "0 giờ 0 phút";
            lblTienPhong.Text = "0 VNĐ";
            lblTienDichVu.Text = "0 VNĐ";
            lblTongThanhToan.Text = "0 VNĐ";
        }

        private void BtnMoPhong_Click(object sender, EventArgs e)
        {
            try
            {
                if (_phongDangChon == null)
                    throw new Exception("Vui lòng chọn một phòng trên sơ đồ trước khi mở phòng.");

                int maHDMoi = _hdBus.MoPhong(_phongDangChon.MaPhong);
                MessageBox.Show($"Đã mở {_phongDangChon.TenPhong} thành công! (Mã hóa đơn: #{maHDMoi})", "Mở phòng");

                TaiSoDoPhong();
                TaiBangQuanLyPhong();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }
        #endregion

        #region 2. GỌI DỊCH VỤ / ĐỒ UỐNG (DISCONNECTED MODEL - DATATABLE)
        private void DinhDangCotChiTietDichVu()
        {
            if (dgvChiTietDichVu.Columns.Count == 0) return;

            if (dgvChiTietDichVu.Columns["MaHD"] != null) dgvChiTietDichVu.Columns["MaHD"].Visible = false;
            if (dgvChiTietDichVu.Columns["MaDV"] != null) { dgvChiTietDichVu.Columns["MaDV"].HeaderText = "Mã DV"; dgvChiTietDichVu.Columns["MaDV"].FillWeight = 55; }
            if (dgvChiTietDichVu.Columns["TenDV"] != null) { dgvChiTietDichVu.Columns["TenDV"].HeaderText = "Tên dịch vụ / Đồ uống"; dgvChiTietDichVu.Columns["TenDV"].FillWeight = 180; }
            if (dgvChiTietDichVu.Columns["SoLuong"] != null) { dgvChiTietDichVu.Columns["SoLuong"].HeaderText = "SL"; dgvChiTietDichVu.Columns["SoLuong"].FillWeight = 55; }
            if (dgvChiTietDichVu.Columns["DonGia"] != null)
            {
                dgvChiTietDichVu.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvChiTietDichVu.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvChiTietDichVu.Columns["DonGia"].FillWeight = 95;
            }
            if (dgvChiTietDichVu.Columns["ThanhTien"] != null)
            {
                dgvChiTietDichVu.Columns["ThanhTien"].HeaderText = "Thành tiền";
                dgvChiTietDichVu.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                dgvChiTietDichVu.Columns["ThanhTien"].FillWeight = 110;
            }
        }

        private void BtnThemDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                if (_hoaDonHienTai == null || _dtChiTietDichVu == null)
                    throw new Exception("Vui lòng chọn phòng đang hát để gọi dịch vụ.");

                var dv = cboChonDichVu.SelectedItem as DichVuDTO;
                int soLuong = (int)numSoLuongDV.Value;

                _hdBus.ThemDichVuVaoDataTable(_dtChiTietDichVu, _hoaDonHienTai.MaHD, dv, soLuong);
                numSoLuongDV.Value = 1;
                CapNhatThongTinTinhTienTamThoi();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dtChiTietDichVu == null || dgvChiTietDichVu.CurrentRow == null) return;
                _hdBus.XoaDongDichVuTrongDataTable(_dtChiTietDichVu, dgvChiTietDichVu.CurrentRow.Index);
                CapNhatThongTinTinhTienTamThoi();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi"); }
        }

        private void CapNhatThongTinTinhTienTamThoi()
        {
            if (_phongDangChon == null || _hoaDonHienTai == null || _phongDangChon.TrangThai != "Đang hát")
                return;

            DateTime hienTai = DateTime.Now;
            TimeSpan thoiGian = hienTai - _hoaDonHienTai.GioVao;
            if (thoiGian.TotalSeconds < 0) thoiGian = TimeSpan.Zero;

            int tongPhut = (int)thoiGian.TotalMinutes;
            lblThoiLuongHat.Text = $"{tongPhut / 60} giờ {tongPhut % 60} phút ({thoiGian.TotalHours:F2}h)";

            decimal tienPhong = _hdBus.TinhTienPhong(_hoaDonHienTai.GioVao, hienTai, _phongDangChon.GiaTheoGio);
            decimal tienDV = _hdBus.TinhTongTienDichVu(_dtChiTietDichVu);
            decimal tongCong = tienPhong + tienDV;

            lblTienPhong.Text = $"{tienPhong:N0} VNĐ";
            lblTienDichVu.Text = $"{tienDV:N0} VNĐ";
            lblTongThanhToan.Text = $"{tongCong:N0} VNĐ";
        }
        #endregion

        #region 3. THANH TOÁN HÓA ĐƠN (SQLTRANSACTION)
        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_phongDangChon == null || _hoaDonHienTai == null)
                    throw new Exception("Vui lòng chọn phòng đang hát cần thanh toán.");

                DateTime gioRa = DateTime.Now;
                decimal tienPhong = _hdBus.TinhTienPhong(_hoaDonHienTai.GioVao, gioRa, _phongDangChon.GiaTheoGio);
                decimal tienDV = _hdBus.TinhTongTienDichVu(_dtChiTietDichVu);
                decimal tongThanhToan = tienPhong + tienDV;

                string xacNhan = $"Xác nhận thanh toán cho {_phongDangChon.TenPhong} (Hóa đơn #{_hoaDonHienTai.MaHD})?\n\n" +
                                 $"- Giờ vào: {_hoaDonHienTai.GioVao:dd/MM/yyyy HH:mm}\n" +
                                 $"- Giờ ra : {gioRa:dd/MM/yyyy HH:mm}\n" +
                                 $"- Tiền phòng : {tienPhong:N0} VNĐ\n" +
                                 $"- Tiền dịch vụ: {tienDV:N0} VNĐ\n" +
                                 $"- TỔNG CỘNG  : {tongThanhToan:N0} VNĐ";

                if (MessageBox.Show(xacNhan, "Xác nhận Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                // Chụp nội dung hóa đơn trước khi xóa DataTable
                string bienLai = TaoNoiDungHoaDon(_phongDangChon, _hoaDonHienTai, gioRa, tienPhong, tienDV, _dtChiTietDichVu);

                // Thực thi SqlTransaction: Cập nhật HoaDon -> Trừ TonKho DichVu -> Đưa PhongHat về "Trống"
                _hdBus.ThanhToan(_hoaDonHienTai.MaHD, _phongDangChon.MaPhong, _hoaDonHienTai.GioVao, gioRa, _phongDangChon.GiaTheoGio, _dtChiTietDichVu);

                MessageBox.Show(bienLai, "Thanh toán thành công (Committed)", MessageBoxButtons.OK, MessageBoxIcon.Information);

                TaiToanBoDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giao dịch thanh toán bị hủy (Rollback)!\nLý do: " + ex.Message, "Cảnh báo Giao dịch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string TaoNoiDungHoaDon(PhongHatDTO phong, HoaDonDTO hd, DateTime gioRa, decimal tienPhong, decimal tienDV, DataTable dtCT)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========== HÓA ĐƠN THANH TOÁN KARAOKE ==========");
            sb.AppendLine($"Hóa đơn : #{hd.MaHD} - {phong.TenPhong} ({phong.LoaiPhong})");
            sb.AppendLine($"Giờ vào : {hd.GioVao:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Giờ ra  : {gioRa:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Đơn giá : {phong.GiaTheoGio:N0} VNĐ/giờ");
            sb.AppendLine("------------------------------------------------");
            if (dtCT != null && dtCT.Rows.Count > 0)
            {
                sb.AppendLine("Dịch vụ đã sử dụng:");
                foreach (DataRow r in dtCT.Rows)
                {
                    if (r.RowState != DataRowState.Deleted)
                        sb.AppendLine($" • {r["TenDV"]} x{r["SoLuong"]} = {Convert.ToDecimal(r["ThanhTien"]):N0}đ");
                }
                sb.AppendLine("------------------------------------------------");
            }
            sb.AppendLine($"Tiền phòng   : {tienPhong:N0} VNĐ");
            sb.AppendLine($"Tiền dịch vụ : {tienDV:N0} VNĐ");
            sb.AppendLine($"TỔNG CỘNG    : {(tienPhong + tienDV):N0} VNĐ");
            sb.AppendLine("================================================");
            return sb.ToString();
        }
        #endregion

        #region 4. QUẢN LÝ DANH MỤC PHÒNG HÁT & KHO DỊCH VỤ (TAB 2)
        private void TaiBangQuanLyPhong()
        {
            dgvPhongHat.DataSource = _phongBus.LayTatCa();
            if (dgvPhongHat.Columns["MaPhong"] != null) { dgvPhongHat.Columns["MaPhong"].HeaderText = "Mã"; dgvPhongHat.Columns["MaPhong"].FillWeight = 50; }
            if (dgvPhongHat.Columns["TenPhong"] != null) { dgvPhongHat.Columns["TenPhong"].HeaderText = "Tên phòng"; dgvPhongHat.Columns["TenPhong"].FillWeight = 135; }
            if (dgvPhongHat.Columns["LoaiPhong"] != null) { dgvPhongHat.Columns["LoaiPhong"].HeaderText = "Loại"; dgvPhongHat.Columns["LoaiPhong"].FillWeight = 75; }
            if (dgvPhongHat.Columns["GiaTheoGio"] != null)
            {
                dgvPhongHat.Columns["GiaTheoGio"].HeaderText = "Giá/giờ";
                dgvPhongHat.Columns["GiaTheoGio"].DefaultCellStyle.Format = "N0";
                dgvPhongHat.Columns["GiaTheoGio"].FillWeight = 95;
            }
            if (dgvPhongHat.Columns["TrangThai"] != null) { dgvPhongHat.Columns["TrangThai"].HeaderText = "Trạng thái"; dgvPhongHat.Columns["TrangThai"].FillWeight = 85; }
        }

        private void DgvPhongHat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPhongHat.Rows[e.RowIndex].DataBoundItem is PhongHatDTO p)
            {
                txtMaPhong.Text = p.MaPhong.ToString();
                txtTenPhong.Text = p.TenPhong;
                cboLoaiPhong.SelectedItem = p.LoaiPhong;
                txtGiaTheoGio.Text = p.GiaTheoGio.ToString("0");
            }
        }

        private void BtnThemPhong_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtGiaTheoGio.Text, out decimal gia))
                    throw new Exception("Giá theo giờ phải là số hợp lệ.");

                _phongBus.Them(new PhongHatDTO
                {
                    TenPhong = txtTenPhong.Text.Trim(),
                    LoaiPhong = cboLoaiPhong.Text,
                    GiaTheoGio = gia
                });
                LamMoiFormPhong();
                TaiBangQuanLyPhong();
                TaiSoDoPhong();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaPhong_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaPhong.Text, out int maP);
                if (!decimal.TryParse(txtGiaTheoGio.Text, out decimal gia))
                    throw new Exception("Giá theo giờ phải là số hợp lệ.");

                _phongBus.Sua(new PhongHatDTO
                {
                    MaPhong = maP,
                    TenPhong = txtTenPhong.Text.Trim(),
                    LoaiPhong = cboLoaiPhong.Text,
                    GiaTheoGio = gia
                });
                TaiBangQuanLyPhong();
                TaiSoDoPhong();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaPhong_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaPhong.Text, out int maP);
                if (MessageBox.Show("Xóa phòng hát này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _phongBus.Xoa(maP);
                    LamMoiFormPhong();
                    TaiBangQuanLyPhong();
                    TaiSoDoPhong();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi"); }
        }

        private void LamMoiFormPhong()
        {
            txtMaPhong.Clear();
            txtTenPhong.Clear();
            cboLoaiPhong.SelectedIndex = 0;
            txtGiaTheoGio.Clear();
        }

        private void TaiDanhSachDichVu()
        {
            var dsDV = _dvBus.LayTatCa();
            dgvDichVu.DataSource = dsDV;

            if (dgvDichVu.Columns["MaDV"] != null) { dgvDichVu.Columns["MaDV"].HeaderText = "Mã"; dgvDichVu.Columns["MaDV"].FillWeight = 50; }
            if (dgvDichVu.Columns["TenDV"] != null) { dgvDichVu.Columns["TenDV"].HeaderText = "Tên dịch vụ / Đồ uống"; dgvDichVu.Columns["TenDV"].FillWeight = 180; }
            if (dgvDichVu.Columns["DonGia"] != null)
            {
                dgvDichVu.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvDichVu.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvDichVu.Columns["DonGia"].FillWeight = 90;
            }
            if (dgvDichVu.Columns["TonKho"] != null) { dgvDichVu.Columns["TonKho"].HeaderText = "Tồn kho"; dgvDichVu.Columns["TonKho"].FillWeight = 75; }

            cboChonDichVu.DataSource = new List<DichVuDTO>(dsDV);
            cboChonDichVu.DisplayMember = "TenDV";
            cboChonDichVu.ValueMember = "MaDV";
        }

        private void DgvDichVu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvDichVu.Rows[e.RowIndex].DataBoundItem is DichVuDTO dv)
            {
                txtMaDV.Text = dv.MaDV.ToString();
                txtTenDV.Text = dv.TenDV;
                txtDonGiaDV.Text = dv.DonGia.ToString("0");
                numTonKhoDV.Value = dv.TonKho;
            }
        }

        private void BtnThemDV_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtDonGiaDV.Text, out decimal gia))
                    throw new Exception("Đơn giá phải là số hợp lệ.");

                _dvBus.Them(new DichVuDTO
                {
                    TenDV = txtTenDV.Text.Trim(),
                    DonGia = gia,
                    TonKho = (int)numTonKhoDV.Value
                });
                LamMoiFormDV();
                TaiDanhSachDichVu();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaDV_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaDV.Text, out int maDV);
                if (!decimal.TryParse(txtDonGiaDV.Text, out decimal gia))
                    throw new Exception("Đơn giá phải là số hợp lệ.");

                _dvBus.Sua(new DichVuDTO
                {
                    MaDV = maDV,
                    TenDV = txtTenDV.Text.Trim(),
                    DonGia = gia,
                    TonKho = (int)numTonKhoDV.Value
                });
                TaiDanhSachDichVu();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaDV_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaDV.Text, out int maDV);
                if (MessageBox.Show("Xóa dịch vụ này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _dvBus.Xoa(maDV);
                    LamMoiFormDV();
                    TaiDanhSachDichVu();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi"); }
        }

        private void LamMoiFormDV()
        {
            txtMaDV.Clear();
            txtTenDV.Clear();
            txtDonGiaDV.Clear();
            numTonKhoDV.Value = 50;
        }
        #endregion
    }
}