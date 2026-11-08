using System;
using StatusRateHud;
class Tests
{
    static void Check(bool ok, string name) { if (!ok) throw new Exception(name); }
    static void Main()
    {
        var m = new RateMeter();
        m.Sample(0, 80); m.Sample(0.25, 79.975);
        Check(!m.Ready, "wait for half-second sample");
        m.Sample(0.5, 79.95);
        Check(m.Ready && Math.Abs(m.Rate + 0.1) < 0.00001, "depletion per real second");
        m.Sample(1, 80.05);
        Check(Math.Abs(m.Rate - 0.2) < 0.00001, "action switches to recovery");
        m.Sample(1.5, 80.05);
        Check(m.Rate == 0, "pause clears old rate");
        m.Sample(2, 79.55);
        Check(Math.Abs(m.Rate + 1) < 0.00001, "10x acceleration reflects 10x visible depletion");
        m.Sample(0, 90); Check(!m.Ready, "clock reset");
        m.Sample(10, 80); Check(!m.Ready, "long gap resets stale sample");
        m.Sample(10.5, 80); Check(m.Ready && m.Rate == 0, "clamped or steady value");
        m.Sample(double.NaN, 0); Check(!m.Ready, "invalid sample");
        Console.WriteLine("9 real-time rate scenarios passed.");
        string[] gameNames = { "English", "Japanese", "German", "Russian", "French", "Spanish - Spain", "Finnish", "Korean", "Norwegian", "Portuguese - Portugal", "Portuguese - Brazil", "Simplified Chinese", "Traditional Chinese", "Swedish", "Turkish", "Dutch", "Italian", "Polish", "Ukrainian" };
        string[] codes = { "en", "ja", "de", "ru", "fr", "es", "fi", "ko", "nb", "pt-PT", "pt-BR", "zh-CN", "zh-TW", "sv", "tr", "nl", "it", "pl", "uk" };
        Check(Translations.All.Count == 19, "nineteen supported languages");
        for (int i = 0; i < gameNames.Length; i++)
        {
            var tr = Translations.Resolve(gameNames[i]);
            Check(tr.Code == codes[i], "language resolution: " + gameNames[i]);
            Check(Translations.Resolve(codes[i]).Code == codes[i], "locale code: " + codes[i]);
            foreach (string text in tr.Text) Check(!string.IsNullOrWhiteSpace(text), "missing translation: " + codes[i]);
        }
        Check(Translations.Resolve("Chinese (Traditional)").Code == "zh-TW", "traditional Chinese alias");
        Check(Translations.Resolve("zh_Hans").Code == "zh-CN", "simplified Chinese locale alias");
        Check(Translations.Resolve("zh_Hant").Code == "zh-TW", "traditional Chinese locale alias");
        Check(Translations.Resolve(null).Code == "en" && Translations.Resolve("Workshop language").Code == "en", "English fallback");
        Check(Translations.SettingIndex("GAME_KEY") == -1, "leave other localization keys alone");
        Check(Translations.SettingIndex("SRHUD_Enabled") == 6 && Translations.SettingIndex("SRHUD_Y") == 9, "settings translation mapping");
        Check(1.25.ToString("0.00", Translations.Resolve("German").Culture) == "1,25", "localized decimals");
        Console.WriteLine("19 languages: all labels, locale codes, aliases, fallback, settings keys and decimal formatting passed.");
    }
}
