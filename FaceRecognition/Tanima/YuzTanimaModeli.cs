using OpenCvSharp;
using OpenCvSharp.Face;

namespace FaceRecognition.Tanima
{
    // LBPH (Local Binary Patterns Histograms) ile yüz tanıma
    public class YuzTanimaModeli : IDisposable
    {
        private readonly Dictionary<int, string> etiketler = new Dictionary<int, string>();
        private readonly object kilit = new object(); // kamera iş parçacığı ve eğitim aynı anda erişmesin
        private LBPHFaceRecognizer? tanima;

        public bool Egitildi { get; private set; }

        // Uzaklık bu değerin üstündeyse yüz "Bilinmiyor" sayılır
        public double EsikMesafe { get; set; } = 80;

        public IReadOnlyCollection<string> Etiketler
        {
            get
            {
                lock (kilit)
                    return etiketler.Values.ToList();
            }
        }

        public void Egit(IReadOnlyList<YuzOrnegi> ornekler)
        {
            if (ornekler.Count == 0)
                throw new InvalidOperationException("Eğitim için hiç yüz örneği yok. Önce örnek ekleyin.");

            // Her farklı etikete bir sayı verilir; LBPH etiketleri sayı olarak ister
            Dictionary<string, int> numaralar = new Dictionary<string, int>();
            List<Mat> goruntuler = new List<Mat>();
            List<int> numaraListesi = new List<int>();
            foreach (YuzOrnegi ornek in ornekler)
            {
                if (!numaralar.TryGetValue(ornek.Etiket, out int numara))
                {
                    numara = numaralar.Count;
                    numaralar[ornek.Etiket] = numara;
                }
                goruntuler.Add(ornek.Goruntu);
                numaraListesi.Add(numara);
            }

            LBPHFaceRecognizer yeniModel = LBPHFaceRecognizer.Create();
            yeniModel.Train(goruntuler, numaraListesi);

            lock (kilit)
            {
                tanima?.Dispose();
                tanima = yeniModel;
                etiketler.Clear();
                foreach (KeyValuePair<string, int> cift in numaralar)
                    etiketler[cift.Value] = cift.Key;
                Egitildi = true;
            }
        }

        public TahminSonucu? Tahmin(Mat hazirYuz)
        {
            lock (kilit)
            {
                if (tanima == null)
                    return null;

                tanima.Predict(hazirYuz, out int numara, out double mesafe);
                bool tanindi = numara >= 0 && mesafe <= EsikMesafe;
                string etiket = etiketler.TryGetValue(numara, out string? ad) ? ad : TahminSonucu.BilinmeyenEtiket;
                return new TahminSonucu(etiket, mesafe, tanindi);
            }
        }

        public void Dispose()
        {
            lock (kilit)
                tanima?.Dispose();
        }
    }
}
