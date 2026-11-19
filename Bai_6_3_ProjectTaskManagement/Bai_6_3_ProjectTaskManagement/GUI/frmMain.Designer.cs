using System.Drawing;
using System.Windows.Forms;

namespace Bai_6_3_ProjectTaskManagement
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabDuAnTV;
        private TabPage tabTaskList;
        private TabPage tabKanban;
        private TabPage tabBaoCao;

        // Tab 1: Dự án & Thành viên
        private GroupBox grpDuAn, grpThanhVien;
        private TextBox txtMaDuAn, txtTenDuAn, txtMaTV, txtHoTenTV, txtEmailTV, txtChuyenMonTV;
        private DateTimePicker dtpNgayBD, dtpNgayKT;
        private ComboBox cboTrangThaiDA;
        private Button btnThemDA, btnSuaDA, btnXoaDA, btnMoiDA;
        private Button btnThemTV, btnSuaTV, btnXoaTV, btnMoiTV;
        private DataGridView dgvDuAn, dgvThanhVien;

        // Tab 2: List View & Phân công
        private ComboBox cboLocDuAn, cboLocTrangThai, cboTaskDuAn, cboTaskUuTien, cboTaskTrangThai, cboChuyenNhanhTT;
        private TextBox txtMaTask, txtTenTask, txtMoTaTask;
        private DateTimePicker dtpHanNop;
        private CheckedListBox clbThanhVien;
        private DataGridView dgvTasks;
        private Button btnThemTask, btnSuaTask, btnXoaTask, btnMoiTask, btnApDungTT;

        // Tab 3: Kanban Board
        private ComboBox cboKanbanDuAn;
        private ListBox lstToDo, lstInProgress, lstReview, lstDone;
        private Button btnKanbanLui, btnKanbanTien, btnKanbanDone, btnKanbanTaiLai;

        // Tab 4: Cảnh báo & Thống kê
        private DataGridView dgvTreHan, dgvThongKe;
        private ComboBox cboThongKeDuAn, cboThongKeThanhVien;
        private Label lblKetQuaScalar;
        private Button btnLamMoiBaoCao, btnKiemTraScalar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Text = "BÀI 6.3 - HỆ THỐNG QUẢN LÝ CÔNG VIỆC & TIẾN ĐỘ DỰ ÁN (3-TIER)";
            this.Size = new Size(1340, 780);
            this.MinimumSize = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            tabMain = new TabControl { Dock = DockStyle.Fill };
            tabDuAnTV = new TabPage("1. Quản lý Dự án && Thành viên");
            tabTaskList = new TabPage("2. Danh sách Task && Phân công (List View)");
            tabKanban = new TabPage("3. Bảng Kanban (Kanban View)");
            tabBaoCao = new TabPage("4. Cảnh báo Trễ hạn && Thống kê");

            InitTabDuAnThanhVien();
            InitTabTaskList();
            InitTabKanban();
            InitTabBaoCao();

            tabMain.TabPages.AddRange(new[] { tabDuAnTV, tabTaskList, tabKanban, tabBaoCao });
            this.Controls.Add(tabMain);
        }

        private void InitTabDuAnThanhVien()
        {
            var tblTab1 = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            tblTab1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblTab1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // --- GROUP DỰ ÁN ---
            grpDuAn = new GroupBox { Text = "Quản lý Dự án", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 6, 0) };

            grpDuAn.Controls.Add(new Label { Text = "Mã DA:", Location = new Point(15, 32), AutoSize = true });
            txtMaDuAn = new TextBox { Location = new Point(115, 29), Width = 85, ReadOnly = true };

            grpDuAn.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(225, 32), AutoSize = true });
            cboTrangThaiDA = new ComboBox { Location = new Point(315, 29), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            cboTrangThaiDA.Items.AddRange(new object[] { "Đang chạy", "Hoàn thành", "Tạm dừng" });
            cboTrangThaiDA.SelectedIndex = 0;

            grpDuAn.Controls.Add(new Label { Text = "Tên dự án:", Location = new Point(15, 68), AutoSize = true });
            txtTenDuAn = new TextBox { Location = new Point(115, 65), Width = 490, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            grpDuAn.Controls.Add(new Label { Text = "Ngày bắt đầu:", Location = new Point(15, 104), AutoSize = true });
            dtpNgayBD = new DateTimePicker { Location = new Point(115, 101), Width = 155, Format = DateTimePickerFormat.Short };

            grpDuAn.Controls.Add(new Label { Text = "Ngày kết thúc:", Location = new Point(290, 104), AutoSize = true });
            dtpNgayKT = new DateTimePicker { Location = new Point(400, 101), Width = 155, Format = DateTimePickerFormat.Short };

            btnThemDA = new Button { Text = "Thêm DA", Location = new Point(115, 142), Size = new Size(110, 32) };
            btnSuaDA = new Button { Text = "Sửa DA", Location = new Point(235, 142), Size = new Size(110, 32) };
            btnXoaDA = new Button { Text = "Xóa DA", Location = new Point(355, 142), Size = new Size(110, 32) };
            btnMoiDA = new Button { Text = "Làm mới", Location = new Point(475, 142), Size = new Size(110, 32) };

            dgvDuAn = new DataGridView
            {
                Location = new Point(15, 190),
                Size = new Size(605, 485),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            grpDuAn.Controls.AddRange(new Control[] { txtMaDuAn, cboTrangThaiDA, txtTenDuAn, dtpNgayBD, dtpNgayKT, btnThemDA, btnSuaDA, btnXoaDA, btnMoiDA, dgvDuAn });

            // --- GROUP THÀNH VIÊN ---
            grpThanhVien = new GroupBox { Text = "Quản lý Thành viên nhóm", Dock = DockStyle.Fill, Margin = new Padding(6, 0, 0, 0) };

            grpThanhVien.Controls.Add(new Label { Text = "Mã TV:", Location = new Point(15, 32), AutoSize = true });
            txtMaTV = new TextBox { Location = new Point(115, 29), Width = 80, ReadOnly = true };

            grpThanhVien.Controls.Add(new Label { Text = "Họ tên:", Location = new Point(215, 32), AutoSize = true });
            txtHoTenTV = new TextBox { Location = new Point(280, 29), Width = 325, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            grpThanhVien.Controls.Add(new Label { Text = "Email:", Location = new Point(15, 68), AutoSize = true });
            txtEmailTV = new TextBox { Location = new Point(115, 65), Width = 490, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            grpThanhVien.Controls.Add(new Label { Text = "Chuyên môn:", Location = new Point(15, 104), AutoSize = true });
            txtChuyenMonTV = new TextBox { Location = new Point(115, 101), Width = 490, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            btnThemTV = new Button { Text = "Thêm TV", Location = new Point(115, 142), Size = new Size(110, 32) };
            btnSuaTV = new Button { Text = "Sửa TV", Location = new Point(235, 142), Size = new Size(110, 32) };
            btnXoaTV = new Button { Text = "Xóa TV", Location = new Point(355, 142), Size = new Size(110, 32) };
            btnMoiTV = new Button { Text = "Làm mới", Location = new Point(475, 142), Size = new Size(110, 32) };

            dgvThanhVien = new DataGridView
            {
                Location = new Point(15, 190),
                Size = new Size(605, 485),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            grpThanhVien.Controls.AddRange(new Control[] { txtMaTV, txtHoTenTV, txtEmailTV, txtChuyenMonTV, btnThemTV, btnSuaTV, btnXoaTV, btnMoiTV, dgvThanhVien });

            tblTab1.Controls.Add(grpDuAn, 0, 0);
            tblTab1.Controls.Add(grpThanhVien, 1, 0);
            tabDuAnTV.Controls.Add(tblTab1);
        }

        private void InitTabTaskList()
        {
            tabTaskList.Padding = new Padding(10);

            // Bộ lọc phía trên (tăng khoảng cách để không bao giờ đè chữ)
            var grpFilter = new GroupBox
            {
                Text = "Bộ lọc && Chuyển trạng thái nhanh",
                Dock = DockStyle.Top,
                Height = 68
            };

            grpFilter.Controls.Add(new Label { Text = "Lọc Dự án:", Location = new Point(15, 30), AutoSize = true });
            cboLocDuAn = new ComboBox { Location = new Point(95, 26), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            grpFilter.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(365, 30), AutoSize = true });
            cboLocTrangThai = new ComboBox { Location = new Point(445, 26), Width = 145, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTrangThai.Items.AddRange(new object[] { "Tất cả", "To Do", "In Progress", "Review", "Done" });
            cboLocTrangThai.SelectedIndex = 0;

            grpFilter.Controls.Add(new Label { Text = "Đổi nhanh TT Task chọn:", Location = new Point(615, 30), AutoSize = true, ForeColor = Color.DarkBlue });
            cboChuyenNhanhTT = new ComboBox { Location = new Point(810, 26), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboChuyenNhanhTT.Items.AddRange(new object[] { "To Do", "In Progress", "Review", "Done" });
            cboChuyenNhanhTT.SelectedIndex = 3;

            btnApDungTT = new Button { Text = "Cập nhật TT", Location = new Point(975, 24), Size = new Size(135, 30), BackColor = Color.LightSteelBlue };

            grpFilter.Controls.AddRange(new Control[] { cboLocDuAn, cboLocTrangThai, cboChuyenNhanhTT, btnApDungTT });

            // Chia 2 cột tỷ lệ 62% (Bảng Task) - 38% (Form chi tiết)
            var tblTask = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 8, 0, 0)
            };
            tblTask.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tblTask.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));

            dgvTasks = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 6, 0),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            var grpTaskDetail = new GroupBox
            {
                Text = "Thông tin Công việc (Task) && Phân công",
                Dock = DockStyle.Fill,
                Margin = new Padding(6, 0, 0, 0)
            };

            grpTaskDetail.Controls.Add(new Label { Text = "Mã Task:", Location = new Point(15, 32), AutoSize = true });
            txtMaTask = new TextBox { Location = new Point(110, 29), Width = 80, ReadOnly = true };

            grpTaskDetail.Controls.Add(new Label { Text = "Ưu tiên:", Location = new Point(215, 32), AutoSize = true });
            cboTaskUuTien = new ComboBox { Location = new Point(285, 29), Width = 175, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            cboTaskUuTien.Items.AddRange(new object[] { "Cao", "Trung bình", "Thấp" });
            cboTaskUuTien.SelectedIndex = 0;

            grpTaskDetail.Controls.Add(new Label { Text = "Thuộc Dự án:", Location = new Point(15, 68), AutoSize = true });
            cboTaskDuAn = new ComboBox { Location = new Point(110, 65), Width = 350, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            grpTaskDetail.Controls.Add(new Label { Text = "Tên Task:", Location = new Point(15, 104), AutoSize = true });
            txtTenTask = new TextBox { Location = new Point(110, 101), Width = 350, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            grpTaskDetail.Controls.Add(new Label { Text = "Hạn nộp:", Location = new Point(15, 140), AutoSize = true });
            dtpHanNop = new DateTimePicker { Location = new Point(110, 137), Width = 130, Format = DateTimePickerFormat.Short };

            grpTaskDetail.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(255, 140), AutoSize = true });
            cboTaskTrangThai = new ComboBox { Location = new Point(340, 137), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            cboTaskTrangThai.Items.AddRange(new object[] { "To Do", "In Progress", "Review", "Done" });
            cboTaskTrangThai.SelectedIndex = 0;

            grpTaskDetail.Controls.Add(new Label { Text = "Mô tả:", Location = new Point(15, 176), AutoSize = true });
            txtMoTaTask = new TextBox { Location = new Point(110, 173), Size = new Size(350, 72), Multiline = true, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            grpTaskDetail.Controls.Add(new Label { Text = "Phân công cho (chọn 1 hoặc nhiều thành viên):", Location = new Point(15, 258), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) });
            clbThanhVien = new CheckedListBox
            {
                Location = new Point(15, 284),
                Size = new Size(445, 255),
                CheckOnClick = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            btnThemTask = new Button { Text = "Tạo && Giao việc", Location = new Point(15, 552), Size = new Size(130, 36), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnSuaTask = new Button { Text = "Cập nhật", Location = new Point(153, 552), Size = new Size(98, 36), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnXoaTask = new Button { Text = "Xóa Task", Location = new Point(259, 552), Size = new Size(98, 36), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnMoiTask = new Button { Text = "Làm mới", Location = new Point(365, 552), Size = new Size(95, 36), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };

            grpTaskDetail.Controls.AddRange(new Control[]
            {
                txtMaTask, cboTaskUuTien, cboTaskDuAn, txtTenTask, dtpHanNop,
                cboTaskTrangThai, txtMoTaTask, clbThanhVien,
                btnThemTask, btnSuaTask, btnXoaTask, btnMoiTask
            });

            tblTask.Controls.Add(dgvTasks, 0, 0);
            tblTask.Controls.Add(grpTaskDetail, 1, 0);

            tabTaskList.Controls.Add(tblTask);
            tabTaskList.Controls.Add(grpFilter);
        }

        private void InitTabKanban()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 56 };
            pnlTop.Controls.Add(new Label { Text = "Chọn Dự án hiển thị:", Location = new Point(15, 18), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) });
            cboKanbanDuAn = new ComboBox { Location = new Point(235, 15), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };

            btnKanbanLui = new Button { Text = "<< Lùi trạng thái", Location = new Point(575, 12), Size = new Size(145, 32) };
            btnKanbanTien = new Button { Text = "Tiến trạng thái >>", Location = new Point(730, 12), Size = new Size(150, 32), BackColor = Color.LightSkyBlue };
            btnKanbanDone = new Button { Text = "Hoàn thành (Done)", Location = new Point(890, 12), Size = new Size(155, 32), BackColor = Color.LightGreen };
            btnKanbanTaiLai = new Button { Text = "Tải lại Board", Location = new Point(1055, 12), Size = new Size(125, 32) };

            pnlTop.Controls.AddRange(new Control[] { cboKanbanDuAn, btnKanbanLui, btnKanbanTien, btnKanbanDone, btnKanbanTaiLai });

            var tblBoard = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(10)
            };
            for (int i = 0; i < 4; i++)
                tblBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            lstToDo = CreateKanbanColumn(tblBoard, "TO DO (Chờ xử lý)", Color.MistyRose, 0);
            lstInProgress = CreateKanbanColumn(tblBoard, "IN PROGRESS (Đang làm)", Color.LemonChiffon, 1);
            lstReview = CreateKanbanColumn(tblBoard, "REVIEW (Chờ duyệt)", Color.LightCyan, 2);
            lstDone = CreateKanbanColumn(tblBoard, "DONE (Hoàn thành)", Color.Honeydew, 3);

            tabKanban.Controls.Add(tblBoard);
            tabKanban.Controls.Add(pnlTop);
        }

        private ListBox CreateKanbanColumn(TableLayoutPanel parent, string title, Color backColor, int colIndex)
        {
            var grp = new GroupBox
            {
                Text = title,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            var lst = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = backColor,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                HorizontalScrollbar = true
            };
            grp.Controls.Add(lst);
            parent.Controls.Add(grp, colIndex, 0);
            return lst;
        }

        private void InitTabBaoCao()
        {
            var tblBaoCao = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };
            tblBaoCao.RowStyles.Add(new RowStyle(SizeType.Percent, 48F));
            tblBaoCao.RowStyles.Add(new RowStyle(SizeType.Percent, 52F));

            // Nửa trên: Cảnh báo trễ hạn (SqlDataReader)
            var grpTreHan = new GroupBox
            {
                Text = "CẢNH BÁO CÔNG VIỆC TRỄ HẠN (HanNop < Hiện tại && Trạng thái != 'Done' - Đọc qua SqlDataReader)",
                Dock = DockStyle.Fill,
                ForeColor = Color.DarkRed,
                Margin = new Padding(0, 0, 0, 6)
            };

            btnLamMoiBaoCao = new Button { Text = "Làm mới Báo cáo && Cảnh báo", Location = new Point(15, 26), Size = new Size(235, 30), ForeColor = Color.Black };
            dgvTreHan = new DataGridView
            {
                Location = new Point(15, 64),
                Size = new Size(1265, 235),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };
            grpTreHan.Controls.AddRange(new Control[] { btnLamMoiBaoCao, dgvTreHan });

            // Nửa dưới: Thống kê % hoàn thành (GROUP BY & ExecuteScalar)
            var grpThongKe = new GroupBox
            {
                Text = "THỐNG KÊ TỶ LỆ % HOÀN THÀNH CÔNG VIỆC CỦA THÀNH VIÊN TRONG DỰ ÁN",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 0)
            };

            grpThongKe.Controls.Add(new Label { Text = "Chọn Dự án:", Location = new Point(15, 32), AutoSize = true });
            cboThongKeDuAn = new ComboBox { Location = new Point(105, 28), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };

            grpThongKe.Controls.Add(new Label { Text = "Kiểm tra nhanh (ExecuteScalar) TV:", Location = new Point(405, 32), AutoSize = true });
            cboThongKeThanhVien = new ComboBox { Location = new Point(660, 28), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };

            btnKiemTraScalar = new Button { Text = "Tính % (ExecuteScalar)", Location = new Point(895, 26), Size = new Size(175, 30) };
            lblKetQuaScalar = new Label { Text = "Kết quả: --%", Location = new Point(1085, 32), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.DarkGreen };

            dgvThongKe = new DataGridView
            {
                Location = new Point(15, 68),
                Size = new Size(1265, 260),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };

            grpThongKe.Controls.AddRange(new Control[] { cboThongKeDuAn, cboThongKeThanhVien, btnKiemTraScalar, lblKetQuaScalar, dgvThongKe });

            tblBaoCao.Controls.Add(grpTreHan, 0, 0);
            tblBaoCao.Controls.Add(grpThongKe, 0, 1);
            tabBaoCao.Controls.Add(tblBaoCao);
        }
    }
}