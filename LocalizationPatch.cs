using HarmonyLib;
using Il2Cpp;

namespace StatusRateHud;

// Only our four keys are handled; all game/other-mod translations are untouched.
[HarmonyPatch(typeof(Localization), nameof(Localization.Get), new[] { typeof(string) })]
internal static class LocalizationPatch
{
    static bool Prefix(string __0, ref string __result)
    {
        int index = Translations.SettingIndex(__0);
        if (index < 0) return true;
        __result = Translations.Resolve(Localization.Language).Text[index];
        return false;
    }
}
