using OpenCvSharp;

namespace FaceRecognition.Tanima
{
    // Yüz örneklerini "Ornekler/<etiket>/*.png" düzeninde saklar.
    // Bu klasör kişisel veri içerdiği için .gitignore ile repodan hariç tutulur.
    public class OrnekDeposu
    {
        public string KokKlasor { get; }

        public OrnekDeposu(string kokKlasor)
        {
            KokKlasor = kokKlasor;
        }

        public string Kaydet(string etiket, Mat hazirYuz)
        {
            string klasor = Path.Combine(KokKlasor, GuvenliAd(etiket));
            Directory.CreateDirectory(klasor);
            string dosya = Path.Combine(klasor, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");

            // Cv2.ImWrite Türkçe karakterli yolları yazamıyor; bu yüzden önce belleğe kodlanır
            Cv2.ImEncode(".png", hazirYuz, out byte[] veri);
            File.WriteAllBytes(dosya, veri);
            return dosya;
        }

        public List<YuzOrnegi> TumunuYukle()
        {
            List<YuzOrnegi> ornekler = new List<YuzOrnegi>();
            if (!Directory.Exists(KokKlasor))
                return ornekler;

            foreach (string etiketKlasoru in Directory.GetDirectories(KokKlasor))
            {
                string etiket = Path.GetFileName(etiketKlasoru);
                foreach (string dosya in Directory.GetFiles(etiketKlasoru, "*.png"))
                {
                    Mat goruntu = Cv2.ImDecode(File.ReadAllBytes(dosya), ImreadModes.Grayscale);
                    if (goruntu.Empty())
                    {
                        goruntu.Dispose();
                        continue; // bozuk dosya atlanır
                    }
                    if (goruntu.Width != YuzAlgilayici.YuzBoyutu || goruntu.Height != YuzAlgilayici.YuzBoyutu)
                        Cv2.Resize(goruntu, goruntu, new OpenCvSharp.Size(YuzAlgilayici.YuzBoyutu, YuzAlgilayici.YuzBoyutu));
                    ornekler.Add(new YuzOrnegi(etiket, goruntu));
                }
            }
            return ornekler;
        }

        public Dictionary<string, int> EtiketSayilari()
        {
            Dictionary<string, int> sayilar = new Dictionary<string, int>();
            if (!Directory.Exists(KokKlasor))
                return sayilar;

            foreach (string etiketKlasoru in Directory.GetDirectories(KokKlasor))
            {
                int adet = Directory.GetFiles(etiketKlasoru, "*.png").Length;
                if (adet > 0)
                    sayilar[Path.GetFileName(etiketKlasoru)] = adet;
            }
            return sayilar;
        }

        // Klasör adında kullanılamayan karakterleri temizler
        public static string GuvenliAd(string etiket)
        {
            string temiz = etiket.Trim();
            foreach (char c in Path.GetInvalidFileNameChars())
                temiz = temiz.Replace(c, '_');
            return temiz;
        }
    }
}
