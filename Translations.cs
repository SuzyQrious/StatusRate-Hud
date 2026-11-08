using System;
using System.Collections.Generic;
using System.Globalization;

namespace StatusRateHud;

public sealed class Translation
{
    public readonly string Code;
    public readonly string[] Text;
    public readonly CultureInfo Culture;
    public Translation(string code, string text)
    {
        Code = code; Text = text.Split('|');
        // MelonLoader runs with invariant globalization. Never construct a named
        // culture: that throws before MelonMod can even be instantiated.
        // Only decimal formatting is needed, so customize an invariant clone.
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        bool comma = code is "de" or "ru" or "fr" or "es" or "fi" or "nb"
            or "pt-PT" or "pt-BR" or "sv" or "tr" or "nl" or "it" or "pl" or "uk";
        culture.NumberFormat.NumberDecimalSeparator = comma ? "," : ".";
        culture.NumberFormat.NumberGroupSeparator = "";
        Culture = CultureInfo.ReadOnly(culture);
        if (Text.Length != 10) throw new ArgumentException("Expected ten translations: " + code);
    }
}

public static class Translations
{
    public static string Preview(string code) => code switch
    {
        "ja" => "プレビュー", "de" => "Vorschau", "ru" => "Предпросмотр",
        "fr" => "Aperçu", "es" => "Vista previa", "fi" => "Esikatselu",
        "ko" => "미리보기", "nb" => "Forhåndsvisning", "pt-PT" => "Pré-visualização",
        "pt-BR" => "Prévia", "zh-CN" => "预览", "zh-TW" => "預覽",
        "sv" => "Förhandsvisning", "tr" => "Önizleme", "nl" => "Voorbeeld",
        "it" => "Anteprima", "pl" => "Podgląd", "uk" => "Попередній перегляд", _ => "Preview"
    };
    // Five remaining-status labels, per-second unit, four settings labels.
    public static readonly Dictionary<string, Translation> All = new(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = new("en", "Warmth|Energy|Hydration|Satiety|Condition|%/s|Show HUD during gameplay|Text size|Horizontal position (%)|Vertical position (%)"),
        ["ja"] = new("ja", "体温|元気|水分|満腹|体調|%/秒|プレイ中の表示を有効にする|文字の大きさ|横位置（画面の％）|縦位置（画面の％）"),
        ["de"] = new("de", "Wärme|Energie|Flüssigkeit|Sättigung|Zustand|%/s|HUD beim Spielen anzeigen|Schriftgröße|Horizontale Position (%)|Vertikale Position (%)"),
        ["ru"] = new("ru", "Тепло|Энергия|Вода|Сытость|Состояние|%/с|Показывать HUD во время игры|Размер текста|Положение по горизонтали (%)|Положение по вертикали (%)"),
        ["fr"] = new("fr", "Chaleur|Énergie|Hydratation|Satiété|État|%/s|Afficher le HUD pendant le jeu|Taille du texte|Position horizontale (%)|Position verticale (%)"),
        ["es"] = new("es", "Calor|Energía|Hidratación|Saciedad|Estado|%/s|Mostrar el HUD durante el juego|Tamaño del texto|Posición horizontal (%)|Posición vertical (%)"),
        ["fi"] = new("fi", "Lämpö|Energia|Nesteytys|Kylläisyys|Kunto|%/s|Näytä HUD pelatessa|Tekstin koko|Vaakasijainti (%)|Pystysijainti (%)"),
        ["ko"] = new("ko", "체온|기력|수분|포만감|건강|%/초|플레이 중 HUD 표시|글자 크기|가로 위치 (%)|세로 위치 (%)"),
        ["nb"] = new("nb", "Varme|Energi|Væske|Metthet|Tilstand|%/s|Vis HUD under spilling|Tekststørrelse|Vannrett posisjon (%)|Loddrett posisjon (%)"),
        ["pt-PT"] = new("pt-PT", "Calor|Energia|Hidratação|Saciedade|Condição|%/s|Mostrar HUD durante o jogo|Tamanho do texto|Posição horizontal (%)|Posição vertical (%)"),
        ["pt-BR"] = new("pt-BR", "Calor|Energia|Hidratação|Saciedade|Condição|%/s|Exibir HUD durante o jogo|Tamanho do texto|Posição horizontal (%)|Posição vertical (%)"),
        ["zh-CN"] = new("zh-CN", "温暖|精力|水分|饱食度|健康|%/秒|游玩时显示状态栏|字号|水平位置（%）|垂直位置（%）"),
        ["zh-TW"] = new("zh-TW", "溫暖|精力|水分|飽食度|健康|%/秒|遊玩時顯示狀態列|字型大小|水平位置（%）|垂直位置（%）"),
        ["sv"] = new("sv", "Värme|Energi|Vätska|Mättnad|Tillstånd|%/s|Visa HUD under spel|Textstorlek|Vågrät position (%)|Lodrät position (%)"),
        ["tr"] = new("tr", "Sıcaklık|Enerji|Su|Tokluk|Kondisyon|%/sn|Oyun sırasında HUD göster|Yazı boyutu|Yatay konum (%)|Dikey konum (%)"),
        ["nl"] = new("nl", "Warmte|Energie|Hydratatie|Verzadiging|Conditie|%/s|HUD tijdens het spelen tonen|Tekstgrootte|Horizontale positie (%)|Verticale positie (%)"),
        ["it"] = new("it", "Calore|Energia|Idratazione|Sazietà|Condizione|%/s|Mostra HUD durante il gioco|Dimensione del testo|Posizione orizzontale (%)|Posizione verticale (%)"),
        ["pl"] = new("pl", "Ciepło|Energia|Nawodnienie|Sytość|Kondycja|%/s|Pokazuj HUD podczas gry|Rozmiar tekstu|Pozycja pozioma (%)|Pozycja pionowa (%)"),
        ["uk"] = new("uk", "Тепло|Енергія|Вода|Ситість|Стан|%/с|Показувати HUD під час гри|Розмір тексту|Положення по горизонталі (%)|Положення по вертикалі (%)")
    };
    public static Translation Resolve(string language)
    {
        string s = (language ?? "").Trim().ToLowerInvariant();
        if (All.TryGetValue(s, out var direct)) return direct;
        s = s.Replace("_", "-");
        if (All.TryGetValue(s, out direct)) return direct;
        string code = "en";
        if (s.Contains("japan") || s.Contains("日本") || s.StartsWith("ja-")) code = "ja";
        else if (s.Contains("german") || s.Contains("deutsch") || s.StartsWith("de-")) code = "de";
        else if (s.Contains("russian") || s.Contains("рус") || s.StartsWith("ru-")) code = "ru";
        else if (s.Contains("french") || s.Contains("français") || s.StartsWith("fr-")) code = "fr";
        else if (s.Contains("spanish") || s.Contains("español") || s.StartsWith("es-")) code = "es";
        else if (s.Contains("finnish") || s == "suomi" || s.StartsWith("fi-")) code = "fi";
        else if (s.Contains("korean") || s.Contains("한국") || s.StartsWith("ko-")) code = "ko";
        else if (s.Contains("norwegian") || s.Contains("norsk") || s == "no" || s.StartsWith("nb-") || s.StartsWith("no-")) code = "nb";
        else if (s.Contains("brazil") || s.Contains("brasil") || s == "brazilian") code = "pt-BR";
        else if (s.Contains("portugu") || s == "pt") code = "pt-PT";
        else if (s.Contains("traditional") || s.Contains("繁") || s.Contains("hant") || s == "tchinese") code = "zh-TW";
        else if (s.Contains("chinese") || s.Contains("简") || s.Contains("中文") || s.StartsWith("zh")) code = "zh-CN";
        else if (s.Contains("swedish") || s == "svenska" || s.StartsWith("sv-")) code = "sv";
        else if (s.Contains("turkish") || s.Contains("türk") || s.StartsWith("tr-")) code = "tr";
        else if (s.Contains("dutch") || s.Contains("nederlands") || s.StartsWith("nl-")) code = "nl";
        else if (s.Contains("italian") || s.StartsWith("it-")) code = "it";
        else if (s.Contains("polish") || s == "polski" || s.StartsWith("pl-")) code = "pl";
        else if (s.Contains("ukrain") || s.Contains("укра") || s.StartsWith("uk-")) code = "uk";
        return All[code];
    }
    public static int SettingIndex(string key) => key switch
    {
        "SRHUD_Enabled" => 6, "SRHUD_FontSize" => 7, "SRHUD_X" => 8, "SRHUD_Y" => 9, _ => -1
    };
}
