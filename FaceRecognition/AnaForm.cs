using FaceRecognition.Goruntu;
using FaceRecognition.Tanima;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace FaceRecognition
{
    public partial class AnaForm : Form
    {
        private readonly KameraYakalayici kamera = new KameraYakalayici();
        private readonly YuzTanimaModeli model = new YuzTanimaModeli();
        private readonly OrnekDeposu ornekDeposu;
        private readonly OrnekDeposu algilananDeposu;
        private readonly YuzAlgilayici? algilayici;
        private readonly string? algilayiciHatasi;

        // Son kare "örnek ekle" için saklanır; kamera iş parçacığı ile arayüz aynı anda erişmesin
        private readonly object kareKilidi = new object();
        private Mat? sonKare;

        private volatile bool kareIsleniyor;
        private volatile bool yuzleriKaydet;
        private DateTime sonOtomatikKayit = DateTime.MinValue;

        public AnaForm()
        {
            InitializeComponent();
            cmbKamera.SelectedIndex = 0;

            string kok = ProjeKokKlasoru();
            ornekDeposu = new OrnekDeposu(Path.Combine(kok, "Ornekler"));
            algilananDeposu = new OrnekDeposu(Path.Combine(kok, "AlgilananYuzler"));

            try
            {
                algilayici = new YuzAlgilayici(Path.Combine(AppContext.BaseDirectory, "Kaynaklar", "haarcascade_frontalface_default.xml"));
            }
            catch (Exception hata) when (hata is FileNotFoundException || hata is InvalidOperationException || hata is OpenCVException)
            {
                algilayiciHatasi = hata.Message;
            }

            kamera.KareAlindi += Kamera_KareAlindi;
            kamera.HataOlustu += mesaj => ArayuzdeCalistir(() => HataGoster(mesaj));
        }

        private void AnaForm_Load(object? sender, EventArgs e)
        {
            EtiketleriGoster();
            ArayuzuGuncelle();

            if (algilayiciHatasi != null)
            {
                HataGoster(algilayiciHatasi);
                return;
            }

            // Daha önce kaydedilmiş örnekler varsa model açılışta eğitilir
            if (ornekDeposu.EtiketSayilari().Count > 0)
                ModeliEgit(sessiz: true);
        }

        // ---------- Kamera ve görüntü ----------

        private void btnKameraBaslat_Click(object? sender, EventArgs e)
        {
            try
            {
                kamera.Baslat(cmbKamera.SelectedIndex);
            }
            catch (Exception hata) when (hata is InvalidOperationException || hata is OpenCVException)
            {
                HataGoster(hata.Message);
            }
            ArayuzuGuncelle();
        }

        private void btnKameraDurdur_Click(object? sender, EventArgs e)
        {
            kamera.Durdur();
            ArayuzuGuncelle();
        }

        private void btnGorselSec_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog pencere = new OpenFileDialog())
            {
                pencere.Filter = "Resim dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                if (pencere.ShowDialog(this) != DialogResult.OK)
                    return;

                kamera.Durdur();
                try
                {
                    // Cv2.ImRead Türkçe karakterli yolları açamadığı için dosya önce belleğe okunur
                    Mat goruntu = Cv2.ImDecode(File.ReadAllBytes(pencere.FileName), ImreadModes.Color);
                    if (goruntu.Empty())
                        throw new InvalidOperationException("Dosya bir resim olarak okunamadı.");

                    BuyukseKucult(goruntu);
                    KareyiIsleVeGoster(goruntu);
                }
                catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException || hata is InvalidOperationException || hata is OpenCVException)
                {
                    HataGoster($"Görüntü açılamadı:\n{hata.Message}");
                }
                ArayuzuGuncelle();
            }
        }

        // Kamera iş parçacığından gelir. Arayüz yetişemiyorsa kare atlanır.
        private void Kamera_KareAlindi(Mat kare)
        {
            if (kareIsleniyor)
            {
                kare.Dispose();
                return;
            }

            kareIsleniyor = true;
            try
            {
                Bitmap resim = KareyiIsle(kare, out string sonuc, out int yuzSayisi);
                ArayuzdeCalistir(() =>
                {
                    ResmiGoster(resim, sonuc, yuzSayisi);
                    kareIsleniyor = false;
                });
            }
            catch (OpenCVException)
            {
                kareIsleniyor = false; // bozuk kare atlanır
            }
        }

        private void KareyiIsleVeGoster(Mat kare)
        {
            Bitmap resim = KareyiIsle(kare, out string sonuc, out int yuzSayisi);
            ResmiGoster(resim, sonuc, yuzSayisi);
        }

        // Yüzleri bulur, tanır ve kutularını çizer. Kare sonKare olarak saklanır (sahipliği alınır).
        private Bitmap KareyiIsle(Mat kare, out string sonucMetni, out int yuzSayisi)
        {
            lock (kareKilidi)
            {
                sonKare?.Dispose();
                sonKare = kare;
            }

            Rect[] yuzler = algilayici != null ? algilayici.Algila(kare) : Array.Empty<Rect>();
            yuzSayisi = yuzler.Length;

            List<(Rect Bolge, TahminSonucu? Tahmin)> bulunanlar = new List<(Rect, TahminSonucu?)>();
            foreach (Rect bolge in yuzler)
            {
                using (Mat hazirYuz = YuzAlgilayici.YuzuHazirla(kare, bolge))
                {
                    TahminSonucu? tahmin = model.Tahmin(hazirYuz);
                    bulunanlar.Add((bolge, tahmin));
                    OtomatikKaydet(hazirYuz, tahmin);
                }
            }

            sonucMetni = SonucMetniOlustur(bulunanlar);
            return KutulariCiz(kare, bulunanlar);
        }

        // OpenCV'nin PutText'i Türkçe harfleri çizemediği için kutular ve yazılar GDI+ ile çizilir
        private static Bitmap KutulariCiz(Mat kare, List<(Rect Bolge, TahminSonucu? Tahmin)> bulunanlar)
        {
            Bitmap resim = BitmapConverter.ToBitmap(kare);
            using (Graphics g = Graphics.FromImage(resim))
            using (Font yazi = new Font("Segoe UI", 12F, FontStyle.Bold))
            {
                foreach ((Rect bolge, TahminSonucu? tahmin) in bulunanlar)
                {
                    Color renk = tahmin == null ? Color.Gold : tahmin.Tanindi ? Color.LimeGreen : Color.OrangeRed;
                    string etiket = tahmin == null ? "Yüz (model eğitilmedi)" : $"{tahmin.Etiket} %{tahmin.GuvenYuzdesi}";

                    using (Pen kalem = new Pen(renk, 3))
                        g.DrawRectangle(kalem, bolge.X, bolge.Y, bolge.Width, bolge.Height);

                    SizeF boyut = g.MeasureString(etiket, yazi);
                    float y = Math.Max(0, bolge.Y - boyut.Height - 2);
                    using (Brush zemin = new SolidBrush(renk))
                        g.FillRectangle(zemin, bolge.X, y, boyut.Width + 6, boyut.Height);
                    g.DrawString(etiket, yazi, Brushes.Black, bolge.X + 3, y);
                }
            }
            return resim;
        }

        private static string SonucMetniOlustur(List<(Rect Bolge, TahminSonucu? Tahmin)> bulunanlar)
        {
            if (bulunanlar.Count == 0)
                return "Yüz algılanmadı";
            if (bulunanlar[0].Tahmin == null)
                return "Yüz algılandı (tanıma için modeli eğitin)";

            // Birden fazla yüz varsa en büyük (kameraya en yakın) yüzün sonucu gösterilir
            (Rect Bolge, TahminSonucu? Tahmin) enBuyuk = bulunanlar.OrderByDescending(b => b.Bolge.Width * b.Bolge.Height).First();
            return enBuyuk.Tahmin!.ToString();
        }

        private void OtomatikKaydet(Mat hazirYuz, TahminSonucu? tahmin)
        {
            if (!yuzleriKaydet || (DateTime.Now - sonOtomatikKayit).TotalSeconds < 2)
                return;

            try
            {
                algilananDeposu.Kaydet(tahmin?.Etiket ?? "Algilanan", hazirYuz);
                sonOtomatikKayit = DateTime.Now;
            }
            catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException)
            {
                yuzleriKaydet = false;
                ArayuzdeCalistir(() =>
                {
                    chkYuzleriKaydet.Checked = false;
                    HataGoster($"Algılanan yüz kaydedilemedi:\n{hata.Message}");
                });
            }
        }

        private void ResmiGoster(Bitmap resim, string sonuc, int yuzSayisi)
        {
            Image? eski = pbGoruntu.Image;
            pbGoruntu.Image = resim;
            eski?.Dispose();

            lblSonuc.Text = sonuc;
            lblYuzSayisi.Text = $"Algılanan yüz: {yuzSayisi}";
        }

        // ---------- Örnek ekleme ve eğitim ----------

        private void btnOrnekEkle_Click(object? sender, EventArgs e)
        {
            string etiket = OrnekDeposu.GuvenliAd(txtEtiket.Text);
            if (etiket.Length == 0)
            {
                HataGoster("Önce bir etiket (kişi adı) yazın.");
                txtEtiket.Focus();
                return;
            }

            Mat? kopya;
            lock (kareKilidi)
                kopya = sonKare?.Clone();

            if (kopya == null)
            {
                HataGoster("Önce kamerayı başlatın veya bir görüntü seçin.");
                return;
            }

            using (kopya)
            {
                Rect[] yuzler = algilayici!.Algila(kopya);
                if (yuzler.Length != 1)
                {
                    HataGoster($"Örnek eklemek için karede tek bir yüz olmalı (şu an {yuzler.Length} yüz var).");
                    return;
                }

                try
                {
                    using (Mat hazirYuz = YuzAlgilayici.YuzuHazirla(kopya, yuzler[0]))
                        ornekDeposu.Kaydet(etiket, hazirYuz);
                }
                catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException)
                {
                    HataGoster($"Örnek kaydedilemedi:\n{hata.Message}");
                    return;
                }
            }

            EtiketleriGoster();
            lblModel.Text = "Yeni örnek eklendi, modeli yeniden eğitin.";
        }

        private void btnEgit_Click(object? sender, EventArgs e)
        {
            ModeliEgit(sessiz: false);
        }

        private void ModeliEgit(bool sessiz)
        {
            List<YuzOrnegi> ornekler = ornekDeposu.TumunuYukle();
            try
            {
                model.Egit(ornekler);
                int etiketSayisi = model.Etiketler.Count;
                lblModel.Text = $"Model eğitildi: {etiketSayisi} etiket, {ornekler.Count} örnek.";

                if (!sessiz && etiketSayisi < 2)
                    MessageBox.Show(this, "Model tek etiketle eğitildi. Tanımanın anlamlı olması için en az iki farklı etiket ekleyin.",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception hata) when (hata is InvalidOperationException || hata is OpenCVException)
            {
                if (!sessiz)
                    HataGoster(hata.Message);
            }
            finally
            {
                foreach (YuzOrnegi ornek in ornekler)
                    ornek.Dispose();
            }

            // Kamera kapalıyken ekrandaki görüntünün sonucu yeni modelle güncellensin
            if (!kamera.Calisiyor)
            {
                Mat? kopya;
                lock (kareKilidi)
                    kopya = sonKare?.Clone();
                if (kopya != null)
                    KareyiIsleVeGoster(kopya);
            }
        }

        private void EtiketleriGoster()
        {
            lstEtiketler.Items.Clear();
            foreach (KeyValuePair<string, int> etiket in ornekDeposu.EtiketSayilari())
                lstEtiketler.Items.Add($"{etiket.Key}: {etiket.Value} örnek");
        }

        private void nudEsik_ValueChanged(object? sender, EventArgs e)
        {
            model.EsikMesafe = (double)nudEsik.Value;
        }

        private void chkYuzleriKaydet_CheckedChanged(object? sender, EventArgs e)
        {
            yuzleriKaydet = chkYuzleriKaydet.Checked;
        }

        // ---------- Ortak ----------

        private void ArayuzuGuncelle()
        {
            bool hazir = algilayici != null;
            btnKameraBaslat.Enabled = hazir && !kamera.Calisiyor;
            btnKameraDurdur.Enabled = kamera.Calisiyor;
            cmbKamera.Enabled = !kamera.Calisiyor;
            btnGorselSec.Enabled = hazir;
            btnOrnekEkle.Enabled = hazir;
            btnEgit.Enabled = hazir;

            if (!hazir)
            {
                lblDurum.Text = "Yüz algılama modeli bulunamadı";
                lblDurum.BackColor = Color.Firebrick;
            }
            else if (kamera.Calisiyor)
            {
                lblDurum.Text = $"●  KAMERA AÇIK ({cmbKamera.Text})";
                lblDurum.BackColor = Color.SeaGreen;
            }
            else
            {
                lblDurum.Text = "Hazır - kamerayı başlatın veya bir görüntü seçin";
                lblDurum.BackColor = Color.DimGray;
            }
        }

        private void HataGoster(string mesaj)
        {
            MessageBox.Show(this, mesaj, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ArayuzdeCalistir(Action islem)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
                BeginInvoke(islem);
            else
                islem();
        }

        // Çok büyük fotoğraflarda algılama yavaşlamasın diye uzun kenar 1280 piksele indirilir
        private static void BuyukseKucult(Mat goruntu)
        {
            int uzunKenar = Math.Max(goruntu.Width, goruntu.Height);
            if (uzunKenar <= 1280)
                return;

            double oran = 1280.0 / uzunKenar;
            Cv2.Resize(goruntu, goruntu, new OpenCvSharp.Size(), oran, oran);
        }

        // Örnek klasörü çözüm dosyasının bulunduğu klasörde tutulur (yoksa programın yanında)
        private static string ProjeKokKlasoru()
        {
            DirectoryInfo? klasor = new DirectoryInfo(AppContext.BaseDirectory);
            while (klasor != null)
            {
                if (klasor.GetFiles("*.slnx").Length > 0)
                    return klasor.FullName;
                klasor = klasor.Parent;
            }
            return AppContext.BaseDirectory;
        }

        private void AnaForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            kamera.Dispose();
            model.Dispose();
            algilayici?.Dispose();
            lock (kareKilidi)
                sonKare?.Dispose();
        }
    }
}
