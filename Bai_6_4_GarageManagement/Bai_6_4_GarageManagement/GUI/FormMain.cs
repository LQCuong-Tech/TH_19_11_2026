using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bai_6_4_GarageManagement.BUS;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement
{
    public partial class FormMain : Form
    {
        private readonly XeBUS _xeBus = new XeBUS();
        private readonly PhuTungBUS _ptBus = new PhuTungBUS();
        private readonly PhieuSuaChuaBUS _phieuBus = new PhieuSuaChuaBUS();

        private BindingList<ChiTietSuaChuaDTO> _gioPhuTung = new BindingList<ChiTietSuaChuaDTO>();
        private int _maKHCurrent = 0;
        private int _maPhieuDangChon = 0;

        public FormMain()
        {
            InitializeComponent();
            DangKySuKien();
            this.Load += FrmMain_Load;
        }

        private void DangKySuKien()
        {
            btnTraCuuXe.Click += BtnTraCuuXe_Click;
            btnTiepNhanXe.Click += BtnTiepNhanXe_Click;
            btnSuaXe.Click += BtnSuaXe_Click;
            btnXoaXe.Click += BtnXoaXe_Click;
            btnMoiXe.Click += (s, e) => LamMoiFormXe();
            dgvXe.CellClick += DgvXe_CellClick;

            btnThemPT.Click += BtnThemPT_Click;
            btnSuaPT.Click += BtnSuaPT_Click;
            btnXoaPT.Click += BtnXoaPT_Click;
            btnMoiPT.Click += (s, e) => LamMoiFormPT();
            dgvPhuTung.CellClick += DgvPhuTung_CellClick;

            cboPhieuBienSo.SelectedIndexChanged += CboPhieuBienSo_SelectedIndexChanged;
            btnThemVaoPhieu.Click += BtnThemVaoPhieu_Click;
            btnXoaKhoiPhieu.Click += BtnXoaKhoiPhieu_Click;
            btnLamMoiPhieu.Click += (s, e) => LamMoiFormPhieu();
            btnXacNhanPhieu.Click += BtnXacNhanPhieu_Click;

            cboLocTrangThaiPhieu.SelectedIndexChanged += (s, e) => TaiDanhSachPhieu();
            btnLamMoiHoaDon.Click += (s, e) => TaiDanhSachPhieu();
            dgvPhieuSuaChua.CellClick += DgvPhieuSuaChua_CellClick;
            btnThanhToan.Click += BtnThanhToan_Click;
            
            tabMain.SelectedIndexChanged += (s, e) =>
            {
                if (tabMain.SelectedTab == tabHoaDon)
                    TaiDanhSachPhieu();
            };
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                dgvGioPhuTung.DataSource = _gioPhuTung;
                DinhDangCotGioPhuTung();
                TaiToanBoDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL GarageDB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaiToanBoDuLieu()
        {
            TaiDanhSachXe();
            TaiDanhSachPhuTung();
            TaiDanhSachPhieu();
        }

        #region 1. TIẾP NHẬN XE, CHỦ XE & QUẢN LÝ PHỤ TÙNG
        private void TaiDanhSachXe()
        {
            var dsXe = _xeBus.LayTatCa();
            dgvXe.DataSource = dsXe;

            if (dgvXe.Columns["MaKH"] != null) dgvXe.Columns["MaKH"].Visible = false;
            if (dgvXe.Columns["BienSo"] != null) { dgvXe.Columns["BienSo"].HeaderText = "Biển số"; dgvXe.Columns["BienSo"].FillWeight = 95; }
            if (dgvXe.Columns["TenKH"] != null) { dgvXe.Columns["TenKH"].HeaderText = "Chủ xe"; dgvXe.Columns["TenKH"].FillWeight = 135; }
            if (dgvXe.Columns["DienThoai"] != null) { dgvXe.Columns["DienThoai"].HeaderText = "Điện thoại"; dgvXe.Columns["DienThoai"].FillWeight = 95; }
            if (dgvXe.Columns["DiaChi"] != null) { dgvXe.Columns["DiaChi"].HeaderText = "Địa chỉ"; dgvXe.Columns["DiaChi"].FillWeight = 125; }
            if (dgvXe.Columns["HieuXe"] != null) { dgvXe.Columns["HieuXe"].HeaderText = "Hiệu xe"; dgvXe.Columns["HieuXe"].FillWeight = 85; }
            if (dgvXe.Columns["NamSanXuat"] != null) { dgvXe.Columns["NamSanXuat"].HeaderText = "Năm SX"; dgvXe.Columns["NamSanXuat"].FillWeight = 70; }

            cboPhieuBienSo.DataSource = new List<XeDTO>(dsXe);
            cboPhieuBienSo.DisplayMember = "BienSo";
            cboPhieuBienSo.ValueMember = "BienSo";
        }

        private void BtnTraCuuXe_Click(object sender, EventArgs e)
        {
            try
            {
                string bienSo = txtTraCuuBienSo.Text.Trim();
                var xe = _xeBus.TraCuuTheoBienSo(bienSo);

                if (xe != null)
                {
                    _maKHCurrent = xe.MaKH;
                    txtBienSo.Text = xe.BienSo;
                    txtBienSo.ReadOnly = true;
                    txtTenKH.Text = xe.TenKH;
                    txtDienThoai.Text = xe.DienThoai;
                    txtDiaChi.Text = xe.DiaChi;
                    txtHieuXe.Text = xe.HieuXe;
                    numNamSX.Value = xe.NamSanXuat;

                    MessageBox.Show($"Đã tìm thấy xe '{xe.BienSo}' của khách hàng {xe.TenKH}.", "Tra cứu thành công");
                }
                else
                {
                    _maKHCurrent = 0;
                    txtBienSo.ReadOnly = false;
                    txtBienSo.Text = bienSo.ToUpper();
                    txtTenKH.Clear();
                    txtDienThoai.Clear();
                    txtDiaChi.Clear();
                    txtHieuXe.Clear();
                    numNamSX.Value = DateTime.Now.Year;
                    txtTenKH.Focus();

                    MessageBox.Show("Biển số xe chưa có trong hệ thống. Vui lòng nhập thông tin Chủ xe và Xe mới để tiếp nhận.", "Xe mới");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
        }

        private void DgvXe_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvXe.Rows[e.RowIndex].DataBoundItem is XeDTO xe)
            {
                _maKHCurrent = xe.MaKH;
                txtBienSo.Text = xe.BienSo;
                txtBienSo.ReadOnly = true;
                txtTenKH.Text = xe.TenKH;
                txtDienThoai.Text = xe.DienThoai;
                txtDiaChi.Text = xe.DiaChi;
                txtHieuXe.Text = xe.HieuXe;
                numNamSX.Value = xe.NamSanXuat;
            }
        }

        private void BtnTiepNhanXe_Click(object sender, EventArgs e)
        {
            try
            {
                var xe = new XeDTO
                {
                    BienSo = txtBienSo.Text.Trim().ToUpper(),
                    TenKH = txtTenKH.Text.Trim(),
                    DienThoai = txtDienThoai.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim(),
                    HieuXe = txtHieuXe.Text.Trim(),
                    NamSanXuat = (int)numNamSX.Value
                };

                _xeBus.TiepNhanXeMoi(xe);
                MessageBox.Show($"Tiếp nhận xe '{xe.BienSo}' thành công!", "Thông báo");
                LamMoiFormXe();
                TaiDanhSachXe();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaXe_Click(object sender, EventArgs e)
        {
            try
            {
                var xe = new XeDTO
                {
                    MaKH = _maKHCurrent,
                    BienSo = txtBienSo.Text.Trim().ToUpper(),
                    TenKH = txtTenKH.Text.Trim(),
                    DienThoai = txtDienThoai.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim(),
                    HieuXe = txtHieuXe.Text.Trim(),
                    NamSanXuat = (int)numNamSX.Value
                };

                _xeBus.CapNhat(xe);
                MessageBox.Show("Cập nhật thông tin xe và chủ xe thành công!", "Thông báo");
                TaiDanhSachXe();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaXe_Click(object sender, EventArgs e)
        {
            try
            {
                string bienSo = txtBienSo.Text.Trim();
                if (MessageBox.Show($"Xóa xe '{bienSo}' khỏi danh sách?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _xeBus.Xoa(bienSo);
                    LamMoiFormXe();
                    TaiDanhSachXe();
                }
            }
            catch (Exception ex) { MessageBox.Show("Không thể xóa xe đã có lịch sử sửa chữa.\nChi tiết: " + ex.Message, "Lỗi"); }
        }

        private void LamMoiFormXe()
        {
            _maKHCurrent = 0;
            txtTraCuuBienSo.Clear();
            txtBienSo.ReadOnly = false;
            txtBienSo.Clear();
            txtTenKH.Clear();
            txtDienThoai.Clear();
            txtDiaChi.Clear();
            txtHieuXe.Clear();
            numNamSX.Value = DateTime.Now.Year;
        }

        private void TaiDanhSachPhuTung()
        {
            var dsPT = _ptBus.LayTatCa();
            dgvPhuTung.DataSource = dsPT;

            if (dgvPhuTung.Columns["MaPhuTung"] != null) { dgvPhuTung.Columns["MaPhuTung"].HeaderText = "Mã PT"; dgvPhuTung.Columns["MaPhuTung"].FillWeight = 55; }
            if (dgvPhuTung.Columns["TenPhuTung"] != null) { dgvPhuTung.Columns["TenPhuTung"].HeaderText = "Tên phụ tùng"; dgvPhuTung.Columns["TenPhuTung"].FillWeight = 180; }
            if (dgvPhuTung.Columns["DonGia"] != null)
            {
                dgvPhuTung.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvPhuTung.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvPhuTung.Columns["DonGia"].FillWeight = 95;
            }
            if (dgvPhuTung.Columns["SoLuongTon"] != null) { dgvPhuTung.Columns["SoLuongTon"].HeaderText = "Tồn kho"; dgvPhuTung.Columns["SoLuongTon"].FillWeight = 75; }

            cboChonPhuTung.DataSource = new List<PhuTungDTO>(dsPT);
            cboChonPhuTung.DisplayMember = "TenPhuTung";
            cboChonPhuTung.ValueMember = "MaPhuTung";
        }

        private void DgvPhuTung_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPhuTung.Rows[e.RowIndex].DataBoundItem is PhuTungDTO pt)
            {
                txtMaPT.Text = pt.MaPhuTung.ToString();
                txtTenPT.Text = pt.TenPhuTung;
                txtDonGiaPT.Text = pt.DonGia.ToString("0");
                numTonKhoPT.Value = pt.SoLuongTon;
            }
        }

        private void BtnThemPT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtDonGiaPT.Text, out decimal donGia))
                    throw new Exception("Đơn giá phải là số hợp lệ.");

                _ptBus.Them(new PhuTungDTO
                {
                    TenPhuTung = txtTenPT.Text.Trim(),
                    DonGia = donGia,
                    SoLuongTon = (int)numTonKhoPT.Value
                });
                MessageBox.Show("Thêm phụ tùng thành công!", "Thông báo");
                LamMoiFormPT();
                TaiDanhSachPhuTung();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaPT_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaPT.Text, out int maPT);
                if (!decimal.TryParse(txtDonGiaPT.Text, out decimal donGia))
                    throw new Exception("Đơn giá phải là số hợp lệ.");

                _ptBus.Sua(new PhuTungDTO
                {
                    MaPhuTung = maPT,
                    TenPhuTung = txtTenPT.Text.Trim(),
                    DonGia = donGia,
                    SoLuongTon = (int)numTonKhoPT.Value
                });
                MessageBox.Show("Cập nhật phụ tùng thành công!", "Thông báo");
                TaiDanhSachPhuTung();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaPT_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaPT.Text, out int maPT);
                if (MessageBox.Show("Xóa phụ tùng này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _ptBus.Xoa(maPT);
                    LamMoiFormPT();
                    TaiDanhSachPhuTung();
                }
            }
            catch (Exception ex) { MessageBox.Show("Không thể xóa phụ tùng đã nằm trong phiếu sửa chữa.\n" + ex.Message, "Lỗi"); }
        }

        private void LamMoiFormPT()
        {
            txtMaPT.Clear();
            txtTenPT.Clear();
            txtDonGiaPT.Clear();
            numTonKhoPT.Value = 10;
        }
        #endregion

        #region 2. LẬP PHIẾU SỬA CHỮA & XUẤT KHO (SQLTRANSACTION)
        private void CboPhieuBienSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPhieuBienSo.SelectedItem is XeDTO xe)
            {
                txtPhieuChuXe.Text = $"{xe.TenKH} ({xe.DienThoai})";
                txtPhieuHieuXe.Text = $"{xe.HieuXe} - {xe.NamSanXuat}";
            }
        }

        private void DinhDangCotGioPhuTung()
        {
            if (dgvGioPhuTung.Columns["MaPhieu"] != null) dgvGioPhuTung.Columns["MaPhieu"].Visible = false;
            if (dgvGioPhuTung.Columns["MaPhuTung"] != null) { dgvGioPhuTung.Columns["MaPhuTung"].HeaderText = "Mã PT"; dgvGioPhuTung.Columns["MaPhuTung"].FillWeight = 55; }
            if (dgvGioPhuTung.Columns["TenPhuTung"] != null) { dgvGioPhuTung.Columns["TenPhuTung"].HeaderText = "Tên phụ tùng"; dgvGioPhuTung.Columns["TenPhuTung"].FillWeight = 180; }
            if (dgvGioPhuTung.Columns["DonGia"] != null)
            {
                dgvGioPhuTung.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvGioPhuTung.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvGioPhuTung.Columns["DonGia"].FillWeight = 90;
            }
            if (dgvGioPhuTung.Columns["SoLuong"] != null) { dgvGioPhuTung.Columns["SoLuong"].HeaderText = "Số lượng"; dgvGioPhuTung.Columns["SoLuong"].FillWeight = 70; }
            if (dgvGioPhuTung.Columns["TienCong"] != null)
            {
                dgvGioPhuTung.Columns["TienCong"].HeaderText = "Tiền công";
                dgvGioPhuTung.Columns["TienCong"].DefaultCellStyle.Format = "N0";
                dgvGioPhuTung.Columns["TienCong"].FillWeight = 90;
            }
            if (dgvGioPhuTung.Columns["ThanhTien"] != null)
            {
                dgvGioPhuTung.Columns["ThanhTien"].HeaderText = "Thành tiền";
                dgvGioPhuTung.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                dgvGioPhuTung.Columns["ThanhTien"].FillWeight = 105;
            }
        }

        private void BtnThemVaoPhieu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(cboChonPhuTung.SelectedItem is PhuTungDTO pt))
                    throw new Exception("Vui lòng chọn phụ tùng.");

                int soLuong = (int)numSoLuongThay.Value;
                if (!decimal.TryParse(txtTienCong.Text, out decimal tienCong) || tienCong < 0)
                    throw new Exception("Tiền công phải là số >= 0.");

                var daCo = _gioPhuTung.FirstOrDefault(x => x.MaPhuTung == pt.MaPhuTung);
                if (daCo != null)
                {
                    daCo.SoLuong += soLuong;
                    daCo.TienCong += tienCong;
                    _gioPhuTung.ResetBindings();
                }
                else
                {
                    _gioPhuTung.Add(new ChiTietSuaChuaDTO
                    {
                        MaPhuTung = pt.MaPhuTung,
                        TenPhuTung = pt.TenPhuTung,
                        DonGia = pt.DonGia,
                        SoLuong = soLuong,
                        TienCong = tienCong
                    });
                }

                CapNhatTongTienTamTinh();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaKhoiPhieu_Click(object sender, EventArgs e)
        {
            if (dgvGioPhuTung.CurrentRow?.DataBoundItem is ChiTietSuaChuaDTO item)
            {
                _gioPhuTung.Remove(item);
                CapNhatTongTienTamTinh();
            }
        }

        private void CapNhatTongTienTamTinh()
        {
            decimal tong = _phieuBus.TinhTongChiPhi(_gioPhuTung.ToList());
            lblTongTienTamTinh.Text = $"Tổng chi phí dự kiến: {tong:N0} VNĐ";
        }

        private void LamMoiFormPhieu()
        {
            txtNoiDungSua.Clear();
            numSoLuongThay.Value = 1;
            txtTienCong.Text = "100000";
            _gioPhuTung.Clear();
            CapNhatTongTienTamTinh();
        }

        private void BtnXacNhanPhieu_Click(object sender, EventArgs e)
        {
            try
            {
                string bienSo = cboPhieuBienSo.SelectedValue?.ToString();
                var phieu = new PhieuSuaChuaDTO
                {
                    BienSo = bienSo,
                    NoiDungSua = txtNoiDungSua.Text.Trim(),
                    TrangThai = cboPhieuTrangThai.Text
                };

                int maPhieuMoi = _phieuBus.XacNhanLapPhieu(phieu, _gioPhuTung.ToList());

                MessageBox.Show(
                    $"Lập phiếu sửa chữa #{maPhieuMoi} thành công!\nĐã lưu chi tiết và tự động trừ số lượng phụ tùng trong kho.",
                    "Giao dịch hoàn tất (Committed)",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LamMoiFormPhieu();
                TaiDanhSachPhuTung();
                TaiDanhSachPhieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Giao dịch đã bị hủy bỏ (Rollback)!\nLý do: " + ex.Message,
                    "Cảnh báo Tồn kho / Giao dịch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region 3. THANH TOÁN & XUẤT HÓA ĐƠN DỊCH VỤ
        // 2. Thay thế hàm TaiDanhSachPhieu() bằng bản sau:
        private void TaiDanhSachPhieu()
        {
            string trangThai = cboLocTrangThaiPhieu.SelectedItem?.ToString();
            var dsPhieu = _phieuBus.LayDanhSachPhieu(trangThai);
            dgvPhieuSuaChua.DataSource = dsPhieu;

            if (dgvPhieuSuaChua.Columns["MaPhieu"] != null) { dgvPhieuSuaChua.Columns["MaPhieu"].HeaderText = "Mã phiếu"; dgvPhieuSuaChua.Columns["MaPhieu"].FillWeight = 60; }
            if (dgvPhieuSuaChua.Columns["BienSo"] != null) { dgvPhieuSuaChua.Columns["BienSo"].HeaderText = "Biển số"; dgvPhieuSuaChua.Columns["BienSo"].FillWeight = 85; }
            if (dgvPhieuSuaChua.Columns["TenKH"] != null) { dgvPhieuSuaChua.Columns["TenKH"].HeaderText = "Chủ xe"; dgvPhieuSuaChua.Columns["TenKH"].FillWeight = 115; }
            if (dgvPhieuSuaChua.Columns["DienThoai"] != null) dgvPhieuSuaChua.Columns["DienThoai"].Visible = false;
            if (dgvPhieuSuaChua.Columns["HieuXe"] != null) { dgvPhieuSuaChua.Columns["HieuXe"].HeaderText = "Hiệu xe"; dgvPhieuSuaChua.Columns["HieuXe"].FillWeight = 75; }
            if (dgvPhieuSuaChua.Columns["NgayNhan"] != null)
            {
                dgvPhieuSuaChua.Columns["NgayNhan"].HeaderText = "Ngày nhận";
                dgvPhieuSuaChua.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvPhieuSuaChua.Columns["NgayNhan"].FillWeight = 105;
            }
            if (dgvPhieuSuaChua.Columns["NoiDungSua"] != null) { dgvPhieuSuaChua.Columns["NoiDungSua"].HeaderText = "Nội dung sửa"; dgvPhieuSuaChua.Columns["NoiDungSua"].FillWeight = 130; }
            if (dgvPhieuSuaChua.Columns["TongTien"] != null)
            {
                dgvPhieuSuaChua.Columns["TongTien"].HeaderText = "Tổng tiền";
                dgvPhieuSuaChua.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                dgvPhieuSuaChua.Columns["TongTien"].FillWeight = 90;
            }
            if (dgvPhieuSuaChua.Columns["TrangThai"] != null) { dgvPhieuSuaChua.Columns["TrangThai"].HeaderText = "Trạng thái"; dgvPhieuSuaChua.Columns["TrangThai"].FillWeight = 95; }

            if (dsPhieu.Count > 0)
            {
                DgvPhieuSuaChua_CellClick(dgvPhieuSuaChua, new DataGridViewCellEventArgs(0, 0));
            }
            else
            {
                _maPhieuDangChon = 0;
                dgvChiTietHoaDon.DataSource = null;
                rtbHoaDon.Clear();
            }
        }

        private void DgvPhieuSuaChua_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPhieuSuaChua.Rows[e.RowIndex].DataBoundItem is PhieuSuaChuaDTO phieu)
            {
                _maPhieuDangChon = phieu.MaPhieu;
                var dsCT = _phieuBus.LayChiTietPhieu(phieu.MaPhieu);
                dgvChiTietHoaDon.DataSource = dsCT;

                if (dgvChiTietHoaDon.Columns["MaPhieu"] != null) dgvChiTietHoaDon.Columns["MaPhieu"].Visible = false;
                if (dgvChiTietHoaDon.Columns["MaPhuTung"] != null) { dgvChiTietHoaDon.Columns["MaPhuTung"].HeaderText = "Mã PT"; dgvChiTietHoaDon.Columns["MaPhuTung"].FillWeight = 55; }
                if (dgvChiTietHoaDon.Columns["TenPhuTung"] != null) { dgvChiTietHoaDon.Columns["TenPhuTung"].HeaderText = "Tên phụ tùng"; dgvChiTietHoaDon.Columns["TenPhuTung"].FillWeight = 165; }
                if (dgvChiTietHoaDon.Columns["DonGia"] != null) { dgvChiTietHoaDon.Columns["DonGia"].HeaderText = "Đơn giá"; dgvChiTietHoaDon.Columns["DonGia"].DefaultCellStyle.Format = "N0"; dgvChiTietHoaDon.Columns["DonGia"].FillWeight = 85; }
                if (dgvChiTietHoaDon.Columns["SoLuong"] != null) { dgvChiTietHoaDon.Columns["SoLuong"].HeaderText = "SL"; dgvChiTietHoaDon.Columns["SoLuong"].FillWeight = 45; }
                if (dgvChiTietHoaDon.Columns["TienCong"] != null) { dgvChiTietHoaDon.Columns["TienCong"].HeaderText = "Tiền công"; dgvChiTietHoaDon.Columns["TienCong"].DefaultCellStyle.Format = "N0"; dgvChiTietHoaDon.Columns["TienCong"].FillWeight = 85; }
                if (dgvChiTietHoaDon.Columns["ThanhTien"] != null) { dgvChiTietHoaDon.Columns["ThanhTien"].HeaderText = "Thành tiền"; dgvChiTietHoaDon.Columns["ThanhTien"].DefaultCellStyle.Format = "N0"; dgvChiTietHoaDon.Columns["ThanhTien"].FillWeight = 95; }

                HienThiHoaDon(phieu, dsCT);
            }
        }

        private void HienThiHoaDon(PhieuSuaChuaDTO phieu, List<ChiTietSuaChuaDTO> dsCT)
        {
            decimal tongPT = dsCT.Sum(x => x.SoLuong * x.DonGia);
            decimal tongCong = dsCT.Sum(x => x.TienCong);
            decimal tongThanhToan = tongPT + tongCong;

            var sb = new StringBuilder();
            sb.AppendLine("======================================================");
            sb.AppendLine("        GARAGE Ô TÔ & BẢO DƯỠNG CHUYÊN NGHIỆP         ");
            sb.AppendLine("             HÓA ĐƠN DỊCH VỤ SỬA CHỮA                 ");
            sb.AppendLine("======================================================");
            sb.AppendLine($"Mã phiếu   : #{phieu.MaPhieu}");
            sb.AppendLine($"Ngày lập   : {phieu.NgayNhan:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Khách hàng : {phieu.TenKH} - SĐT: {phieu.DienThoai}");
            sb.AppendLine($"Xe / Biển  : {phieu.HieuXe} ({phieu.BienSo})");
            sb.AppendLine($"Nội dung   : {phieu.NoiDungSua}");
            sb.AppendLine($"Trạng thái : {phieu.TrangThai.ToUpper()}");
            sb.AppendLine("------------------------------------------------------");
            sb.AppendLine(string.Format("{0,-24} {1,3} {2,11} {3,12}", "Phụ tùng", "SL", "Tiền công", "Thành tiền"));
            sb.AppendLine("------------------------------------------------------");

            foreach (var item in dsCT)
            {
                string ten = item.TenPhuTung.Length > 23 ? item.TenPhuTung.Substring(0, 20) + "..." : item.TenPhuTung;
                sb.AppendLine(string.Format("{0,-24} {1,3} {2,11:N0} {3,12:N0}", ten, item.SoLuong, item.TienCong, item.ThanhTien));
            }

            sb.AppendLine("------------------------------------------------------");
            sb.AppendLine($"Tổng tiền phụ tùng  : {tongPT,28:N0} VNĐ");
            sb.AppendLine($"Tổng tiền công thợ  : {tongCong,28:N0} VNĐ");
            sb.AppendLine($"TỔNG THANH TOÁN     : {tongThanhToan,28:N0} VNĐ");
            sb.AppendLine("======================================================");
            sb.AppendLine("         Cảm ơn quý khách & Hẹn gặp lại!              ");

            rtbHoaDon.Text = sb.ToString();
        }

        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_maPhieuDangChon <= 0)
                    throw new Exception("Vui lòng chọn một phiếu sửa chữa ở bảng bên trái.");

                _phieuBus.ThanhToan(_maPhieuDangChon);
                MessageBox.Show($"Đã thanh toán thành công Phiếu #{_maPhieuDangChon}!", "Xuất Hóa đơn");

                TaiDanhSachPhieu();
                var phieu = _phieuBus.LayDanhSachPhieu("Tất cả").FirstOrDefault(x => x.MaPhieu == _maPhieuDangChon);
                if (phieu != null)
                {
                    HienThiHoaDon(phieu, _phieuBus.LayChiTietPhieu(phieu.MaPhieu));
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }
        #endregion
    }
}