using System;
using System.Globalization;
using Il2Cpp;
using MelonLoader;
using ModSettings;
using UnityEngine;

[assembly: MelonInfo(typeof(StatusRateHud.Entry), "Status Rate HUD", "1.3.2", "Local custom mod")]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace StatusRateHud;

public sealed class Settings : JsonModSettings
{
    [Name("SRHUD_Enabled", Localize = true)]
    public bool Enabled = true;
    [Name("SRHUD_FontSize", Localize = true), Slider(14, 32)]
    public int FontSize = 20;
    [Name("SRHUD_X", Localize = true), Slider(0, 100)]
    public int X = 2;
    [Name("SRHUD_Y", Localize = true), Slider(0, 100)]
    public int Y = 25;
}

public sealed class Entry : MelonMod
{
    Settings settings;
    readonly RateMeter[] meters = { new(), new(), new(), new(), new() };
    readonly double[] values = new double[5];
    Translation language = Translations.All["en"];
    string fontLanguage;
    bool ownsFont;
    GUIStyle style;
    Font font;
    bool visible;
    bool loggedSample;
    bool loggedDraw;
    float nextError;
    GameObject settingsGrid;
    public override void OnInitializeMelon()
    {
        settings = new Settings();
        settings.AddToModSettings("Status Rate HUD");
        LoggerInstance.Msg("Ready: read-only percentage HUD; rates use real seconds; refresh every 0.5 seconds.");
    }
    void Reset()
    {
        visible = false;
        foreach (var meter in meters) meter.Reset();
    }
    public override void OnSceneWasLoaded(int index, string name) => Reset();
    static double Remaining(float current, float maximum) =>
        maximum > 0 ? Math.Clamp(100.0 * (1 - current / maximum), 0, 100) : 0;
    static bool GameplayVisible()
    {
        if (GameManager.IsMainMenuActive() || InterfaceManager.IsPanelLoadingEnabledOrLoading()
            || InterfaceManager.IsPanelEnabled<Panel_PauseMenu>()
            || InterfaceManager.IsPanelEnabled<Panel_OptionsMenu>()
            || !InterfaceManager.IsPanelEnabled<Panel_HUD>()) return false;
        var hud = InterfaceManager.GetPanel<Panel_HUD>();
        return hud != null && !hud.IsNonRadialOverlayActive();
    }
    bool SettingsPreviewVisible()
    {
        if (settings == null || !settings.IsVisible()
            || !InterfaceManager.IsPanelEnabled<Panel_OptionsMenu>()) return false;
        // ModSettings.IsVisible applies to every mod, not just the selected tab.
        // Its selected grid alone is active; check our own grid specifically.
        if (settingsGrid == null || !settingsGrid.activeInHierarchy)
            settingsGrid = GameObject.Find("Mod settings grid (Status Rate HUD)");
        return settingsGrid != null && settingsGrid.activeInHierarchy;
    }
    public override void OnLateUpdate()
    {
        try
        {
            if (!settings.Enabled || !GameplayVisible()) { Reset(); return; }
            var cold = GameManager.GetFreezingComponent();
            var fatigue = GameManager.GetFatigueComponent();
            var thirst = GameManager.GetThirstComponent();
            var hunger = GameManager.GetHungerComponent();
            var condition = GameManager.GetConditionComponent();
            var tod = GameManager.GetTimeOfDayComponent();
            if (cold == null || fatigue == null || thirst == null || hunger == null || condition == null || tod == null || !tod.IsInitializedForScene()) { Reset(); return; }
            values[0] = Remaining(cold.m_CurrentFreezing, cold.m_MaxFreezing);
            values[1] = Remaining(fatigue.m_CurrentFatigue, fatigue.m_MaxFatigue);
            values[2] = Remaining(thirst.m_CurrentThirst, thirst.m_MaxThirst);
            values[3] = Math.Clamp(100.0 * hunger.GetCalorieReserves() / Math.Max(1, hunger.GetAdjustedMaxReserveCalories()), 0, 100);
            values[4] = 100.0 * condition.m_CurrentHP / Math.Max(1, condition.m_MaxHP);
            // Unscaled wall clock: reflects the actual on-screen change, including time acceleration.
            double seconds = Time.realtimeSinceStartupAsDouble;
            for (int i = 0; i < meters.Length; i++) meters[i].Sample(seconds, values[i]);
            visible = true;
            if (!loggedSample) { loggedSample = true; LoggerInstance.Msg("Live status values acquired."); }
        }
        catch (Exception e) { Reset(); Report(e); }
    }
    void Report(Exception e)
    {
        if (Time.realtimeSinceStartup < nextError) return;
        nextError = Time.realtimeSinceStartup + 30;
        LoggerInstance.Error(e.ToString());
    }
    public override void OnGUI()
    {
        if (settings == null) return;
        try
        {
            bool preview = SettingsPreviewVisible();
            // Check again at draw time so opening Esc cannot leave a stale overlay.
            if (!preview && (!visible || !settings.Enabled || !GameplayVisible())) { Reset(); return; }
            language = Translations.Resolve(Localization.Language);
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.richText = false;
                style.normal.textColor = Color.white;
            }
            if (fontLanguage != language.Code || font == null)
            {
                if (ownsFont && font != null) UnityEngine.Object.Destroy(font);
                // Prefer the game's localized font; OS fonts are only a fallback.
                font = InterfaceManager.IsPanelLoaded<Panel_HUD>()
                    ? InterfaceManager.GetPanel<Panel_HUD>()?.m_Label_Condition?.trueTypeFont : null;
                ownsFont = font == null;
                if (ownsFont)
                {
                    string[] fonts = language.Code switch
                    {
                        "ja" => new[] { "Yu Gothic", "Meiryo", "MS Gothic", "Arial" },
                        "ko" => new[] { "Malgun Gothic", "Gulim", "Arial" },
                        "zh-CN" => new[] { "Microsoft YaHei", "SimHei", "Arial" },
                        "zh-TW" => new[] { "Microsoft JhengHei", "PMingLiU", "Arial" },
                        _ => new[] { "Segoe UI", "Arial", "Tahoma" }
                    };
                    font = Font.CreateDynamicFontFromOSFont(fonts, 20);
                }
                style.font = font;
                fontLanguage = language.Code;
                LoggerInstance.Msg("HUD language: " + Localization.Language + " -> " + language.Code);
            }
            int size = Math.Clamp(settings.FontSize, 14, 32);
            style.fontSize = size;
            float labelWidth = size * 3;
            for (int i = 0; i < 5; i++) labelWidth = Math.Max(labelWidth, style.CalcSize(new GUIContent(language.Text[i])).x + size * 0.5f);
            float valueWidth = Math.Max(size * 4.4f, style.CalcSize(new GUIContent("105.0%".Replace(".", language.Culture.NumberFormat.NumberDecimalSeparator))).x + size * 0.5f);
            float rateWidth = Math.Max(size * 9, style.CalcSize(new GUIContent("-100.000" + language.Text[5])).x + size);
            float rateX = labelWidth + valueWidth + size * 0.6f;
            float width = rateX + rateWidth, row = size * 2.0f, height = row * 5;
            float x = Math.Max(0, Screen.width - width) * settings.X / 100f;
            float y = Math.Max(0, Screen.height - height) * settings.Y / 100f;
            var oldColor = GUI.color;
            try
            {
                GUI.color = Color.white;
                if (preview)
                    DrawText(new Rect(x, Math.Max(0, y - size * 1.5f), width, size * 1.5f), Translations.Preview(language.Code), new Color(0.75f, 0.80f, 0.85f), TextAnchor.UpperLeft);
                for (int i = 0; i < meters.Length; i++)
                {
                    var meter = meters[i];
                    double displayValue = preview && !loggedSample ? 75.0 : values[i];
                    string rate = preview ? 0.0.ToString("0.000", language.Culture) + language.Text[5] : "—";
                    Color rateColor = new Color(0.72f, 0.73f, 0.72f);
                    if (!preview && meter.Ready)
                    {
                        double r = Math.Abs(meter.Rate) < 0.0005 ? 0 : meter.Rate;
                        rateColor = r > 0 ? new Color(0.70f, 0.84f, 0.77f) : r < 0 ? new Color(0.88f, 0.69f, 0.62f) : rateColor;
                        rate = r.ToString("+0.000;-0.000;0.000", language.Culture) + language.Text[5];
                    }
                    float top = y + row * i;
                    DrawText(new Rect(x, top, labelWidth, row), language.Text[i], new Color(0.88f, 0.89f, 0.86f), TextAnchor.UpperLeft);
                    DrawText(new Rect(x + labelWidth, top, valueWidth, row), displayValue.ToString("0.0", language.Culture) + "%", new Color(0.94f, 0.94f, 0.90f), TextAnchor.UpperRight);
                    DrawText(new Rect(x + rateX, top, rateWidth, row), rate, rateColor, TextAnchor.UpperRight);
                    // Slim, neutral gauge instead of a framed information panel.
                    GUI.color = new Color(0, 0, 0, 0.40f);
                    GUI.DrawTexture(new Rect(x + 3, top + size * 1.5f, width - 6, 3), Texture2D.whiteTexture);
                    GUI.color = new Color(0.86f, 0.88f, 0.85f, 0.55f);
                    GUI.DrawTexture(new Rect(x + 3, top + size * 1.5f, (width - 6) * (float)Math.Clamp(displayValue / 100, 0, 1), 2), Texture2D.whiteTexture);
                }
            }
            finally { GUI.color = oldColor; }
            if (!loggedDraw) { loggedDraw = true; LoggerInstance.Msg("HUD rendered successfully."); }
        }
        catch (Exception e) { Report(e); }
    }
    void DrawText(Rect rect, string text, Color color, TextAnchor alignment)
    {
        style.alignment = alignment;
        GUI.color = new Color(0, 0, 0, 0.85f);
        GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), text, style);
        GUI.color = color;
        GUI.Label(rect, text, style);
    }
}

