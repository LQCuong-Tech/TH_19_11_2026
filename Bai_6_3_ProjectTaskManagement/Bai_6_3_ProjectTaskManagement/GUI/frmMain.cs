using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Bai_6_3_ProjectTaskManagement.BUS;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement
{
    public partial class frmMain : Form
    {
        private readonly DuAnBUS _duAnBus = new DuAnBUS();
        private readonly ThanhVienBUS _tvBus = new ThanhVienBUS();
        private readonly CongViecBUS _cvBus = new CongViecBUS();
        private readonly PhanCongBUS _pcBus = new PhanCongBUS();

        private bool _isLoadingCombo = false;

        public frmMain()
        {
            InitializeComponent();
            DangKySuKien();
            this.Load += FrmMain_Load;
        }

        private void DangKySuKien()
        {
            // Tab 1: Dự án & Thành viên
            btnThemDA.Click += BtnThemDA_Click;
            btnSuaDA.Click += BtnSuaDA_Click;
            btnXoaDA.Click += BtnXoaDA_Click;
            btnMoiDA.Click += (s, e) => LamMoiFormDuAn();
            dgvDuAn.CellClick += DgvDuAn_CellClick;

            btnThemTV.Click += BtnThemTV_Click;
            btnSuaTV.Click += BtnSuaTV_Click;
            btnXoaTV.Click += BtnXoaTV_Click;
            btnMoiTV.Click += (s, e) => LamMoiFormThanhVien();
            dgvThanhVien.CellClick += DgvThanhVien_CellClick;

            // Tab 2: Task & Phân công
            cboLocDuAn.SelectedIndexChanged += (s, e) => { if (!_isLoadingCombo) TaiDanhSachTask(); };
            cboLocTrangThai.SelectedIndexChanged += (s, e) => { if (!_isLoadingCombo) TaiDanhSachTask(); };
            dgvTasks.CellClick += DgvTasks_CellClick;

            btnThemTask.Click += BtnThemTask_Click;
            btnSuaTask.Click += BtnSuaTask_Click;
            btnXoaTask.Click += BtnXoaTask_Click;
            btnMoiTask.Click += (s, e) => LamMoiFormTask();
            btnApDungTT.Click += BtnApDungTT_Click;

            // Tab 3: Kanban
            cboKanbanDuAn.SelectedIndexChanged += (s, e) => { if (!_isLoadingCombo) TaiKanbanBoard(); };
            btnKanbanTaiLai.Click += (s, e) => TaiKanbanBoard();
            btnKanbanLui.Click += (s, e) => DiChuyenKanban(-1);
            btnKanbanTien.Click += (s, e) => DiChuyenKanban(1);
            btnKanbanDone.Click += BtnKanbanDone_Click;

            // Đảm bảo tại 1 thời điểm chỉ chọn 1 thẻ trên 4 cột Kanban
            lstToDo.SelectedIndexChanged += (s, e) => BoChonCacCotKhac(lstToDo);
            lstInProgress.SelectedIndexChanged += (s, e) => BoChonCacCotKhac(lstInProgress);
            lstReview.SelectedIndexChanged += (s, e) => BoChonCacCotKhac(lstReview);
            lstDone.SelectedIndexChanged += (s, e) => BoChonCacCotKhac(lstDone);

            // Tab 4: Báo cáo
            btnLamMoiBaoCao.Click += (s, e) => { TaiCanhBaoTreHan(); TaiThongKeDuAn(); };
            cboThongKeDuAn.SelectedIndexChanged += (s, e) => { if (!_isLoadingCombo) TaiThongKeDuAn(); };
            btnKiemTraScalar.Click += BtnKiemTraScalar_Click;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                TaiDuLieuToanHeThong();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL ProjectTaskDB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaiDuLieuToanHeThong()
        {
            TaiDanhSachDuAn();
            TaiDanhSachThanhVien();
            TaiDanhSachTask();
            TaiKanbanBoard();
            TaiCanhBaoTreHan();
            TaiThongKeDuAn();
        }

        #region 1. QUẢN LÝ DỰ ÁN & THÀNH VIÊN
        private void TaiDanhSachDuAn()
        {
            var dsDuAn = _duAnBus.LayTatCa();
            dgvDuAn.DataSource = dsDuAn;

            _isLoadingCombo = true;

            // ComboBox lọc (có thêm lựa chọn Tất cả)
            var dsLoc = new List<DuAnDTO> { new DuAnDTO { MaDuAn = 0, TenDuAn = "-- Tất cả Dự án --" } };
            dsLoc.AddRange(dsDuAn);
            cboLocDuAn.DataSource = dsLoc;
            cboLocDuAn.DisplayMember = "TenDuAn";
            cboLocDuAn.ValueMember = "MaDuAn";

            cboTaskDuAn.DataSource = new List<DuAnDTO>(dsDuAn);
            cboTaskDuAn.DisplayMember = "TenDuAn";
            cboTaskDuAn.ValueMember = "MaDuAn";

            cboKanbanDuAn.DataSource = new List<DuAnDTO>(dsDuAn);
            cboKanbanDuAn.DisplayMember = "TenDuAn";
            cboKanbanDuAn.ValueMember = "MaDuAn";

            cboThongKeDuAn.DataSource = new List<DuAnDTO>(dsDuAn);
            cboThongKeDuAn.DisplayMember = "TenDuAn";
            cboThongKeDuAn.ValueMember = "MaDuAn";

            _isLoadingCombo = false;
        }

        private void DgvDuAn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvDuAn.Rows[e.RowIndex];
            txtMaDuAn.Text = row.Cells["MaDuAn"].Value.ToString();
            txtTenDuAn.Text = row.Cells["TenDuAn"].Value.ToString();
            dtpNgayBD.Value = Convert.ToDateTime(row.Cells["NgayBatDau"].Value);
            dtpNgayKT.Value = Convert.ToDateTime(row.Cells["NgayKetThuc"].Value);
            cboTrangThaiDA.SelectedItem = row.Cells["TrangThai"].Value.ToString();
        }

        private void BtnThemDA_Click(object sender, EventArgs e)
        {
            try
            {
                _duAnBus.Them(new DuAnDTO
                {
                    TenDuAn = txtTenDuAn.Text.Trim(),
                    NgayBatDau = dtpNgayBD.Value,
                    NgayKetThuc = dtpNgayKT.Value,
                    TrangThai = cboTrangThaiDA.Text
                });
                MessageBox.Show("Thêm dự án thành công!", "Thông báo");
                LamMoiFormDuAn();
                TaiDuLieuToanHeThong();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaDA_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaDuAn.Text, out int maDA);
                _duAnBus.Sua(new DuAnDTO
                {
                    MaDuAn = maDA,
                    TenDuAn = txtTenDuAn.Text.Trim(),
                    NgayBatDau = dtpNgayBD.Value,
                    NgayKetThuc = dtpNgayKT.Value,
                    TrangThai = cboTrangThaiDA.Text
                });
                MessageBox.Show("Cập nhật dự án thành công!", "Thông báo");
                TaiDuLieuToanHeThong();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaDA_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaDuAn.Text, out int maDA);
                if (MessageBox.Show("Xóa dự án sẽ xóa toàn bộ công việc thuộc dự án này. Tiếp tục?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _duAnBus.Xoa(maDA);
                    LamMoiFormDuAn();
                    TaiDuLieuToanHeThong();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void LamMoiFormDuAn()
        {
            txtMaDuAn.Clear();
            txtTenDuAn.Clear();
            dtpNgayBD.Value = DateTime.Now;
            dtpNgayKT.Value = DateTime.Now.AddMonths(1);
            cboTrangThaiDA.SelectedIndex = 0;
        }

        private void TaiDanhSachThanhVien()
        {
            var dsTV = _tvBus.LayTatCa();
            dgvThanhVien.DataSource = dsTV;

            clbThanhVien.Items.Clear();
            foreach (var tv in dsTV)
            {
                clbThanhVien.Items.Add(tv, false);
            }

            cboThongKeThanhVien.DataSource = new List<ThanhVienDTO>(dsTV);
            cboThongKeThanhVien.DisplayMember = "HoTen";
            cboThongKeThanhVien.ValueMember = "MaThanhVien";
        }

        private void DgvThanhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvThanhVien.Rows[e.RowIndex];
            txtMaTV.Text = row.Cells["MaThanhVien"].Value.ToString();
            txtHoTenTV.Text = row.Cells["HoTen"].Value.ToString();
            txtEmailTV.Text = row.Cells["Email"].Value?.ToString();
            txtChuyenMonTV.Text = row.Cells["ChuyenMon"].Value?.ToString();
        }

        private void BtnThemTV_Click(object sender, EventArgs e)
        {
            try
            {
                _tvBus.Them(new ThanhVienDTO
                {
                    HoTen = txtHoTenTV.Text.Trim(),
                    Email = txtEmailTV.Text.Trim(),
                    ChuyenMon = txtChuyenMonTV.Text.Trim()
                });
                MessageBox.Show("Thêm thành viên thành công!", "Thông báo");
                LamMoiFormThanhVien();
                TaiDanhSachThanhVien();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaTV_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaTV.Text, out int maTV);
                _tvBus.Sua(new ThanhVienDTO
                {
                    MaThanhVien = maTV,
                    HoTen = txtHoTenTV.Text.Trim(),
                    Email = txtEmailTV.Text.Trim(),
                    ChuyenMon = txtChuyenMonTV.Text.Trim()
                });
                MessageBox.Show("Cập nhật thành viên thành công!", "Thông báo");
                TaiDanhSachThanhVien();
                TaiDanhSachTask();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaTV_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaTV.Text, out int maTV);
                if (MessageBox.Show("Xóa thành viên này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _tvBus.Xoa(maTV);
                    LamMoiFormThanhVien();
                    TaiDanhSachThanhVien();
                    TaiDanhSachTask();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void LamMoiFormThanhVien()
        {
            txtMaTV.Clear();
            txtHoTenTV.Clear();
            txtEmailTV.Clear();
            txtChuyenMonTV.Clear();
        }
        #endregion

        #region 2. DANH SÁCH TASK & PHÂN CÔNG (LIST VIEW)
        private void TaiDanhSachTask()
        {
            int? maDuAn = null;
            if (cboLocDuAn.SelectedValue is int id && id > 0)
                maDuAn = id;

            string trangThai = cboLocTrangThai.SelectedItem?.ToString();
            dgvTasks.DataSource = _cvBus.LayDanhSachTask(maDuAn, trangThai);

            if (dgvTasks.Columns["MaDuAn"] != null)
                dgvTasks.Columns["MaDuAn"].Visible = false;
        }

        private void DgvTasks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var task = dgvTasks.Rows[e.RowIndex].DataBoundItem as CongViecDTO;
            if (task == null) return;

            txtMaTask.Text = task.MaTask.ToString();
            cboTaskDuAn.SelectedValue = task.MaDuAn;
            txtTenTask.Text = task.TenTask;
            txtMoTaTask.Text = task.MoTa;
            dtpHanNop.Value = task.HanNop;
            cboTaskUuTien.SelectedItem = task.DoUuTien;
            cboTaskTrangThai.SelectedItem = task.TrangThai;

            // Đánh dấu check những thành viên đang phụ trách Task này
            var dsMaTV = _pcBus.LayMaThanhVienTheoTask(task.MaTask);
            for (int i = 0; i < clbThanhVien.Items.Count; i++)
            {
                var tv = (ThanhVienDTO)clbThanhVien.Items[i];
                clbThanhVien.SetItemChecked(i, dsMaTV.Contains(tv.MaThanhVien));
            }
        }

        private List<int> LayDanhSachThanhVienDuocCheck()
        {
            var result = new List<int>();
            foreach (var item in clbThanhVien.CheckedItems)
            {
                if (item is ThanhVienDTO tv)
                    result.Add(tv.MaThanhVien);
            }
            return result;
        }

        private void BtnThemTask_Click(object sender, EventArgs e)
        {
            try
            {
                var cv = new CongViecDTO
                {
                    MaDuAn = cboTaskDuAn.SelectedValue != null ? (int)cboTaskDuAn.SelectedValue : 0,
                    TenTask = txtTenTask.Text.Trim(),
                    MoTa = txtMoTaTask.Text.Trim(),
                    HanNop = dtpHanNop.Value,
                    DoUuTien = cboTaskUuTien.Text,
                    TrangThai = cboTaskTrangThai.Text
                };

                _cvBus.TaoTaskVaPhanCong(cv, LayDanhSachThanhVienDuocCheck());
                MessageBox.Show("Tạo công việc & phân công thành công!", "Thông báo");
                LamMoiFormTask();
                CapNhatSauKhiDoiTask();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnSuaTask_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaTask.Text, out int maTask);
                var cv = new CongViecDTO
                {
                    MaTask = maTask,
                    MaDuAn = cboTaskDuAn.SelectedValue != null ? (int)cboTaskDuAn.SelectedValue : 0,
                    TenTask = txtTenTask.Text.Trim(),
                    MoTa = txtMoTaTask.Text.Trim(),
                    HanNop = dtpHanNop.Value,
                    DoUuTien = cboTaskUuTien.Text,
                    TrangThai = cboTaskTrangThai.Text
                };

                _cvBus.CapNhatTaskVaPhanCong(cv, LayDanhSachThanhVienDuocCheck());
                MessageBox.Show("Cập nhật công việc thành công!", "Thông báo");
                CapNhatSauKhiDoiTask();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnXoaTask_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaTask.Text, out int maTask);
                if (MessageBox.Show("Xóa công việc này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _cvBus.Xoa(maTask);
                    LamMoiFormTask();
                    CapNhatSauKhiDoiTask();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void BtnApDungTT_Click(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtMaTask.Text, out int maTask);
                string ttMoi = cboChuyenNhanhTT.Text;
                _cvBus.ChuyenTrangThai(maTask, ttMoi);
                cboTaskTrangThai.SelectedItem = ttMoi;
                CapNhatSauKhiDoiTask();
                MessageBox.Show($"Đã chuyển trạng thái Task #{maTask} sang '{ttMoi}'.", "Thành công");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Cảnh báo"); }
        }

        private void LamMoiFormTask()
        {
            txtMaTask.Clear();
            txtTenTask.Clear();
            txtMoTaTask.Clear();
            dtpHanNop.Value = DateTime.Now.AddDays(7);
            cboTaskUuTien.SelectedIndex = 0;
            cboTaskTrangThai.SelectedIndex = 0;
            for (int i = 0; i < clbThanhVien.Items.Count; i++)
                clbThanhVien.SetItemChecked(i, false);
        }

        private void CapNhatSauKhiDoiTask()
        {
            TaiDanhSachTask();
            TaiKanbanBoard();
            TaiCanhBaoTreHan();
            TaiThongKeDuAn();
        }
        #endregion

        #region 3. BẢNG KANBAN (KANBAN VIEW)
        private void TaiKanbanBoard()
        {
            lstToDo.Items.Clear();
            lstInProgress.Items.Clear();
            lstReview.Items.Clear();
            lstDone.Items.Clear();

            if (!(cboKanbanDuAn.SelectedValue is int maDuAn)) return;

            var tasks = _cvBus.LayDanhSachTask(maDuAn, null);
            foreach (var t in tasks)
            {
                switch (t.TrangThai)
                {
                    case "To Do": lstToDo.Items.Add(t); break;
                    case "In Progress": lstInProgress.Items.Add(t); break;
                    case "Review": lstReview.Items.Add(t); break;
                    case "Done": lstDone.Items.Add(t); break;
                }
            }
        }

        private void BoChonCacCotKhac(ListBox activeList)
        {
            if (activeList.SelectedIndex < 0) return;
            if (activeList != lstToDo) lstToDo.ClearSelected();
            if (activeList != lstInProgress) lstInProgress.ClearSelected();
            if (activeList != lstReview) lstReview.ClearSelected();
            if (activeList != lstDone) lstDone.ClearSelected();
        }

        private CongViecDTO LayTaskDangChonTrenKanban()
        {
            return (lstToDo.SelectedItem ?? lstInProgress.SelectedItem ?? lstReview.SelectedItem ?? lstDone.SelectedItem) as CongViecDTO;
        }

        private void DiChuyenKanban(int buoc)
        {
            var task = LayTaskDangChonTrenKanban();
            if (task == null)
            {
                MessageBox.Show("Vui lòng chọn một thẻ công việc trên bảng Kanban.", "Thông báo");
                return;
            }

            var states = CongViecBUS.DanhSachTrangThai;
            int idxHienTai = Array.IndexOf(states, task.TrangThai);
            int idxMoi = idxHienTai + buoc;

            if (idxMoi < 0 || idxMoi >= states.Length)
            {
                MessageBox.Show("Công việc đã ở trạng thái biên, không thể di chuyển thêm.", "Thông báo");
                return;
            }

            try
            {
                _cvBus.ChuyenTrangThai(task.MaTask, states[idxMoi]);
                CapNhatSauKhiDoiTask();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi"); }
        }

        private void BtnKanbanDone_Click(object sender, EventArgs e)
        {
            var task = LayTaskDangChonTrenKanban();
            if (task == null)
            {
                MessageBox.Show("Vui lòng chọn một thẻ công việc trên bảng Kanban.", "Thông báo");
                return;
            }

            try
            {
                _cvBus.ChuyenTrangThai(task.MaTask, "Done");
                CapNhatSauKhiDoiTask();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi"); }
        }
        #endregion

        #region 4. CẢNH BÁO TRỄ HẠN & THỐNG KÊ
        private void TaiCanhBaoTreHan()
        {
            var dsTreHan = _cvBus.LayTaskTreHan();
            dgvTreHan.DataSource = dsTreHan;

            if (dgvTreHan.Columns["MaDuAn"] != null)
                dgvTreHan.Columns["MaDuAn"].Visible = false;

            foreach (DataGridViewRow row in dgvTreHan.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
            }
        }

        private void TaiThongKeDuAn()
        {
            if (!(cboThongKeDuAn.SelectedValue is int maDuAn)) return;
            dgvThongKe.DataSource = _pcBus.ThongKeTyLeHoanThanh(maDuAn);
        }

        private void BtnKiemTraScalar_Click(object sender, EventArgs e)
        {
            if (!(cboThongKeDuAn.SelectedValue is int maDuAn) || !(cboThongKeThanhVien.SelectedValue is int maTV))
                return;

            double tyLe = _pcBus.TinhTyLeHoanThanhCaNhan(maDuAn, maTV);
            lblKetQuaScalar.Text = $"Hoàn thành: {tyLe}%";
        }
        #endregion
    }
}