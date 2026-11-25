using OpenCvSharp;

namespace FaceRecognition.Tanima
{
    // Eğitimde kullanılan tek bir yüz görüntüsü ve kime ait olduğu
    public class YuzOrnegi : IDisposable
    {
        public string Etiket { get; }
        public Mat Goruntu { get; } // 100x100 gri

        public YuzOrnegi(string etiket, Mat goruntu)
        {
            Etiket = etiket;
            Goruntu = goruntu;
        }

        public void Dispose()
        {
            Goruntu.Dispose();
        }
    }
}
