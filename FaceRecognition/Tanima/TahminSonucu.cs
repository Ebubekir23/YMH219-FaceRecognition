namespace FaceRecognition.Tanima
{
    public class TahminSonucu
    {
        public const string BilinmeyenEtiket = "Bilinmiyor";

        public string Etiket { get; }
        public double Mesafe { get; }   // LBPH uzaklığı: küçüldükçe benzerlik artar
        public bool Tanindi { get; }

        public TahminSonucu(string etiket, double mesafe, bool tanindi)
        {
            Etiket = tanindi ? etiket : BilinmeyenEtiket;
            Mesafe = mesafe;
            Tanindi = tanindi;
        }

        // Uzaklığı kullanıcıya anlaşılır bir yüzdeye çevirir (0 uzaklık = %100)
        public int GuvenYuzdesi
        {
            get { return (int)Math.Clamp(100 - Mesafe, 0, 100); }
        }

        public override string ToString()
        {
            return $"{Etiket}  (güven %{GuvenYuzdesi}, uzaklık {Mesafe:0.0})";
        }
    }
}
