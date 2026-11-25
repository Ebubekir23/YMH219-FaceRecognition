namespace FaceRecognition
{
    partial class AnaForm
    {
        private System.ComponentModel.IContainer components = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblDurum = new Label();
            lblKamera = new Label();
            cmbKamera = new ComboBox();
            btnKameraBaslat = new Button();
            btnKameraDurdur = new Button();
            btnGorselSec = new Button();
            pbGoruntu = new PictureBox();
            grpEgitim = new GroupBox();
            lblEtiket = new Label();
            txtEtiket = new TextBox();
            btnOrnekEkle = new Button();
            lblOrnekler = new Label();
            lstEtiketler = new ListBox();
            btnEgit = new Button();
            lblModel = new Label();
            grpSonuc = new GroupBox();
            lblEsik = new Label();
            nudEsik = new NumericUpDown();
            lblSonuc = new Label();
            lblYuzSayisi = new Label();
            chkYuzleriKaydet = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pbGoruntu).BeginInit();
            grpEgitim.SuspendLayout();
            grpSonuc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudEsik).BeginInit();
            SuspendLayout();
            //
            // lblDurum
            //
            lblDurum.Dock = DockStyle.Top;
            lblDurum.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblDurum.ForeColor = Color.White;
            lblDurum.Location = new Point(0, 0);
            lblDurum.Name = "lblDurum";
            lblDurum.Padding = new Padding(10, 0, 0, 0);
            lblDurum.Size = new Size(1000, 34);
            lblDurum.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblKamera
            //
            lblKamera.AutoSize = true;
            lblKamera.Location = new Point(12, 50);
            lblKamera.Name = "lblKamera";
            lblKamera.Text = "Kamera:";
            //
            // cmbKamera
            //
            cmbKamera.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKamera.Items.AddRange(new object[] { "Kamera 0", "Kamera 1", "Kamera 2", "Kamera 3" });
            cmbKamera.Location = new Point(80, 46);
            cmbKamera.Name = "cmbKamera";
            cmbKamera.Size = new Size(120, 28);
            cmbKamera.TabIndex = 0;
            //
            // btnKameraBaslat
            //
            btnKameraBaslat.Location = new Point(210, 42);
            btnKameraBaslat.Name = "btnKameraBaslat";
            btnKameraBaslat.Size = new Size(150, 36);
            btnKameraBaslat.TabIndex = 1;
            btnKameraBaslat.Text = "●  Kamera Başlat";
            btnKameraBaslat.UseVisualStyleBackColor = true;
            btnKameraBaslat.Click += btnKameraBaslat_Click;
            //
            // btnKameraDurdur
            //
            btnKameraDurdur.Location = new Point(366, 42);
            btnKameraDurdur.Name = "btnKameraDurdur";
            btnKameraDurdur.Size = new Size(150, 36);
            btnKameraDurdur.TabIndex = 2;
            btnKameraDurdur.Text = "■  Kamera Durdur";
            btnKameraDurdur.UseVisualStyleBackColor = true;
            btnKameraDurdur.Click += btnKameraDurdur_Click;
            //
            // btnGorselSec
            //
            btnGorselSec.Location = new Point(522, 42);
            btnGorselSec.Name = "btnGorselSec";
            btnGorselSec.Size = new Size(150, 36);
            btnGorselSec.TabIndex = 3;
            btnGorselSec.Text = "Görüntü Seç...";
            btnGorselSec.UseVisualStyleBackColor = true;
            btnGorselSec.Click += btnGorselSec_Click;
            //
            // pbGoruntu
            //
            pbGoruntu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbGoruntu.BackColor = Color.FromArgb(30, 30, 30);
            pbGoruntu.Location = new Point(12, 86);
            pbGoruntu.Name = "pbGoruntu";
            pbGoruntu.Size = new Size(640, 480);
            pbGoruntu.SizeMode = PictureBoxSizeMode.Zoom;
            pbGoruntu.TabStop = false;
            //
            // grpEgitim
            //
            grpEgitim.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpEgitim.Controls.Add(lblEtiket);
            grpEgitim.Controls.Add(txtEtiket);
            grpEgitim.Controls.Add(btnOrnekEkle);
            grpEgitim.Controls.Add(lblOrnekler);
            grpEgitim.Controls.Add(lstEtiketler);
            grpEgitim.Controls.Add(btnEgit);
            grpEgitim.Controls.Add(lblModel);
            grpEgitim.Location = new Point(664, 80);
            grpEgitim.Name = "grpEgitim";
            grpEgitim.Size = new Size(324, 336);
            grpEgitim.TabIndex = 4;
            grpEgitim.TabStop = false;
            grpEgitim.Text = "Örnekler ve Eğitim";
            //
            // lblEtiket
            //
            lblEtiket.AutoSize = true;
            lblEtiket.Location = new Point(10, 28);
            lblEtiket.Name = "lblEtiket";
            lblEtiket.Text = "Etiket (kişi adı):";
            //
            // txtEtiket
            //
            txtEtiket.Location = new Point(10, 50);
            txtEtiket.MaxLength = 30;
            txtEtiket.Name = "txtEtiket";
            txtEtiket.Size = new Size(302, 27);
            txtEtiket.TabIndex = 0;
            //
            // btnOrnekEkle
            //
            btnOrnekEkle.Location = new Point(10, 84);
            btnOrnekEkle.Name = "btnOrnekEkle";
            btnOrnekEkle.Size = new Size(302, 36);
            btnOrnekEkle.TabIndex = 1;
            btnOrnekEkle.Text = "Karedeki yüzü örnek olarak ekle";
            btnOrnekEkle.UseVisualStyleBackColor = true;
            btnOrnekEkle.Click += btnOrnekEkle_Click;
            //
            // lblOrnekler
            //
            lblOrnekler.AutoSize = true;
            lblOrnekler.Location = new Point(10, 128);
            lblOrnekler.Name = "lblOrnekler";
            lblOrnekler.Text = "Kayıtlı örnekler (etiket: adet):";
            //
            // lstEtiketler
            //
            lstEtiketler.IntegralHeight = false;
            lstEtiketler.Location = new Point(10, 150);
            lstEtiketler.Name = "lstEtiketler";
            lstEtiketler.Size = new Size(302, 106);
            lstEtiketler.TabIndex = 2;
            //
            // btnEgit
            //
            btnEgit.Location = new Point(10, 262);
            btnEgit.Name = "btnEgit";
            btnEgit.Size = new Size(302, 36);
            btnEgit.TabIndex = 3;
            btnEgit.Text = "Modeli Eğit";
            btnEgit.UseVisualStyleBackColor = true;
            btnEgit.Click += btnEgit_Click;
            //
            // lblModel
            //
            lblModel.ForeColor = Color.DimGray;
            lblModel.Location = new Point(10, 304);
            lblModel.Name = "lblModel";
            lblModel.Size = new Size(302, 24);
            lblModel.Text = "Model henüz eğitilmedi.";
            //
            // grpSonuc
            //
            grpSonuc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpSonuc.Controls.Add(lblEsik);
            grpSonuc.Controls.Add(nudEsik);
            grpSonuc.Controls.Add(lblSonuc);
            grpSonuc.Controls.Add(lblYuzSayisi);
            grpSonuc.Location = new Point(664, 422);
            grpSonuc.Name = "grpSonuc";
            grpSonuc.Size = new Size(324, 144);
            grpSonuc.TabIndex = 5;
            grpSonuc.TabStop = false;
            grpSonuc.Text = "Tahmin";
            //
            // lblEsik
            //
            lblEsik.AutoSize = true;
            lblEsik.Location = new Point(10, 30);
            lblEsik.Name = "lblEsik";
            lblEsik.Text = "Eşik uzaklık:";
            //
            // nudEsik
            //
            nudEsik.Location = new Point(110, 26);
            nudEsik.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudEsik.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            nudEsik.Name = "nudEsik";
            nudEsik.Size = new Size(80, 27);
            nudEsik.TabIndex = 0;
            nudEsik.Value = new decimal(new int[] { 80, 0, 0, 0 });
            nudEsik.ValueChanged += nudEsik_ValueChanged;
            //
            // lblSonuc
            //
            lblSonuc.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSonuc.Location = new Point(10, 62);
            lblSonuc.Name = "lblSonuc";
            lblSonuc.Size = new Size(304, 50);
            lblSonuc.Text = "-";
            //
            // lblYuzSayisi
            //
            lblYuzSayisi.AutoSize = true;
            lblYuzSayisi.ForeColor = Color.DimGray;
            lblYuzSayisi.Location = new Point(10, 114);
            lblYuzSayisi.Name = "lblYuzSayisi";
            lblYuzSayisi.Text = "Algılanan yüz: 0";
            //
            // chkYuzleriKaydet
            //
            chkYuzleriKaydet.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkYuzleriKaydet.AutoSize = true;
            chkYuzleriKaydet.Location = new Point(12, 576);
            chkYuzleriKaydet.Name = "chkYuzleriKaydet";
            chkYuzleriKaydet.TabIndex = 6;
            chkYuzleriKaydet.Text = "Algılanan yüzleri \"AlgilananYuzler\" klasörüne kaydet (2 saniyede bir)";
            chkYuzleriKaydet.UseVisualStyleBackColor = true;
            chkYuzleriKaydet.CheckedChanged += chkYuzleriKaydet_CheckedChanged;
            //
            // AnaForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 610);
            Controls.Add(chkYuzleriKaydet);
            Controls.Add(grpSonuc);
            Controls.Add(grpEgitim);
            Controls.Add(pbGoruntu);
            Controls.Add(btnGorselSec);
            Controls.Add(btnKameraDurdur);
            Controls.Add(btnKameraBaslat);
            Controls.Add(cmbKamera);
            Controls.Add(lblKamera);
            Controls.Add(lblDurum);
            MinimumSize = new Size(1018, 600);
            Name = "AnaForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Yüz Algılama ve Tanıma (OpenCV)";
            FormClosing += AnaForm_FormClosing;
            Load += AnaForm_Load;
            ((System.ComponentModel.ISupportInitialize)pbGoruntu).EndInit();
            grpEgitim.ResumeLayout(false);
            grpEgitim.PerformLayout();
            grpSonuc.ResumeLayout(false);
            grpSonuc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudEsik).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblDurum = null!;
        private Label lblKamera = null!;
        private ComboBox cmbKamera = null!;
        private Button btnKameraBaslat = null!;
        private Button btnKameraDurdur = null!;
        private Button btnGorselSec = null!;
        private PictureBox pbGoruntu = null!;
        private GroupBox grpEgitim = null!;
        private Label lblEtiket = null!;
        private TextBox txtEtiket = null!;
        private Button btnOrnekEkle = null!;
        private Label lblOrnekler = null!;
        private ListBox lstEtiketler = null!;
        private Button btnEgit = null!;
        private Label lblModel = null!;
        private GroupBox grpSonuc = null!;
        private Label lblEsik = null!;
        private NumericUpDown nudEsik = null!;
        private Label lblSonuc = null!;
        private Label lblYuzSayisi = null!;
        private CheckBox chkYuzleriKaydet = null!;
    }
}
