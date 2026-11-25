using OpenCvSharp;

namespace FaceRecognition.Tanima
{
    // Haar Cascade ile görüntüdeki yüzlerin yerini bulur
    public class YuzAlgilayici : IDisposable
    {
        public const int YuzBoyutu = 100; // tanıma için tüm yüzler bu boyuta getirilir

        private readonly CascadeClassifier siniflandirici;

        public YuzAlgilayici(string modelDosyasi)
        {
            if (!File.Exists(modelDosyasi))
                throw new FileNotFoundException($"Yüz algılama modeli bulunamadı:\n{modelDosyasi}");

            siniflandirici = new CascadeClassifier(modelDosyasi);
            if (siniflandirici.Empty())
                throw new InvalidOperationException($"Yüz algılama modeli okunamadı:\n{modelDosyasi}");
        }

        public Rect[] Algila(Mat renkliGoruntu)
        {
            using (Mat gri = new Mat())
            {
                Cv2.CvtColor(renkliGoruntu, gri, ColorConversionCodes.BGR2GRAY);
                Cv2.EqualizeHist(gri, gri);
                return siniflandirici.DetectMultiScale(gri, scaleFactor: 1.1, minNeighbors: 6, minSize: new OpenCvSharp.Size(80, 80));
            }
        }

        // Yüz bölgesini keser, griye çevirir, 100x100 yapar ve ışığını dengeler
        public static Mat YuzuHazirla(Mat renkliGoruntu, Rect bolge)
        {
            using (Mat kesit = new Mat(renkliGoruntu, bolge))
            using (Mat gri = new Mat())
            {
                Cv2.CvtColor(kesit, gri, ColorConversionCodes.BGR2GRAY);
                Mat hazir = new Mat();
                Cv2.Resize(gri, hazir, new OpenCvSharp.Size(YuzBoyutu, YuzBoyutu));
                Cv2.EqualizeHist(hazir, hazir);
                return hazir;
            }
        }

        public void Dispose()
        {
            siniflandirici.Dispose();
        }
    }
}
