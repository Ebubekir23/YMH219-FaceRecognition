using OpenCvSharp;

namespace FaceRecognition.Goruntu
{
    // Kameradan sürekli kare okur ve her kareyi olayla bildirir (arka plan iş parçacığında)
    public class KameraYakalayici : IDisposable
    {
        private VideoCapture? kamera;
        private CancellationTokenSource? iptal;
        private Task? okumaGorevi;

        public bool Calisiyor { get; private set; }

        // Olayı dinleyen taraf kareyi kullandıktan sonra Dispose etmelidir
        public event Action<Mat>? KareAlindi;
        public event Action<string>? HataOlustu;

        public void Baslat(int kameraNo)
        {
            if (Calisiyor)
                return;

            // DirectShow, Windows'ta kamerayı daha hızlı açar
            kamera = new VideoCapture(kameraNo, VideoCaptureAPIs.DSHOW);
            if (!kamera.IsOpened())
            {
                kamera.Dispose();
                kamera = null;
                throw new InvalidOperationException(
                    $"{kameraNo} numaralı kamera açılamadı. Kameranın bağlı olduğundan, başka bir uygulamanın kullanmadığından " +
                    "ve Windows gizlilik ayarlarında kamera izninin açık olduğundan emin olun.");
            }

            kamera.Set(VideoCaptureProperties.FrameWidth, 640);
            kamera.Set(VideoCaptureProperties.FrameHeight, 480);

            iptal = new CancellationTokenSource();
            Calisiyor = true;
            okumaGorevi = Task.Run(() => OkumaDongusu(kamera, iptal.Token));
        }

        public void Durdur()
        {
            if (!Calisiyor)
                return;

            iptal!.Cancel();
            okumaGorevi?.Wait(2000);
            kamera?.Release();
            kamera?.Dispose();
            kamera = null;
            Calisiyor = false;
        }

        private void OkumaDongusu(VideoCapture kaynak, CancellationToken iptalJetonu)
        {
            int bosKareSayisi = 0;
            while (!iptalJetonu.IsCancellationRequested)
            {
                Mat kare = new Mat();
                if (!kaynak.Read(kare) || kare.Empty())
                {
                    kare.Dispose();
                    // Kamera çıkarılırsa veya görüntü gelmezse kullanıcı bilgilendirilir
                    if (++bosKareSayisi == 50)
                        HataOlustu?.Invoke("Kameradan görüntü alınamıyor. Kamera çıkarılmış veya başka bir uygulama tarafından kullanılıyor olabilir.");
                    Thread.Sleep(20);
                    continue;
                }

                bosKareSayisi = 0;
                KareAlindi?.Invoke(kare);
            }
        }

        public void Dispose()
        {
            Durdur();
        }
    }
}
