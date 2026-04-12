using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using YG;

public class translator : MonoBehaviour
{
    private enum SupportedLanguage
    {
        Russian,
        English
    }

    [Serializable]
    private class LocalizedTextBinding
    {
        [SerializeField] private TMP_Text target;
        [SerializeField] private string russianText;
        [SerializeField] private string englishText;

        public void Apply(string languageCode)
        {
            if (target == null)
                return;

            target.text = languageCode == "ru" ? russianText : englishText;
        }
    }

    [Header("Language")]
    [SerializeField] private SupportedLanguage fallbackLanguage = SupportedLanguage.English;
    [SerializeField] private bool applyOnStart = true;
    [SerializeField] private bool debugLogs;

    [Header("Auto Translate Buttons (ru/en)")]
    [SerializeField] private List<LocalizedTextBinding> buttonTexts = new List<LocalizedTextBinding>();

    public static string CurrentLanguageCode { get; private set; } = "en";
    public static bool IsRussian => CurrentLanguageCode == "ru";
    public static string ScoreLabel => IsRussian ? "����" : "SCORE";
    public static string HighScoreLabel => IsRussian ? "������" : "BEST";

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern IntPtr GetYandexUserLanguage_js();

    [DllImport("__Internal")]
    private static extern void FreeBuffer_js(IntPtr ptr);
#endif

    private string currentLanguage;

    private void OnEnable()
    {
        YG2.onGetSDKData += ApplyLanguageFromYandex;
    }

    private void Start()
    {
        if (applyOnStart)
            ApplyLanguageFromYandex();
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= ApplyLanguageFromYandex;
    }

    public void ApplyLanguageFromYandex()
    {
        string lang = ResolveLanguageCode();
        if (string.IsNullOrEmpty(lang))
            return;

        CurrentLanguageCode = lang;

        if (lang == currentLanguage)
            return;

        currentLanguage = lang;
        ApplyButtonTexts(lang);

        bool i18nApplied = TryApplyToI18N(lang);

        if (debugLogs)
        {
            if (i18nApplied)
                Debug.Log($"[translator] Language applied: {lang} (buttons + I18N)");
            else
                Debug.LogWarning($"[translator] Language applied: {lang} (buttons only). I18N API was not found.");
        }
    }

    private void ApplyButtonTexts(string languageCode)
    {
        for (int i = 0; i < buttonTexts.Count; i++)
            buttonTexts[i].Apply(languageCode);
    }

    private string ResolveLanguageCode()
    {
        string yandexLang = ToSupportedLanguageCode(TryGetYandexLanguage());
        if (!string.IsNullOrEmpty(yandexLang))
            return yandexLang;

        string systemLang = SystemLanguageToCode(Application.systemLanguage);
        if (!string.IsNullOrEmpty(systemLang))
            return systemLang;

        return FallbackToCode(fallbackLanguage);
    }

    private static string TryGetYandexLanguage()
    {
        string webGLLanguage = TryGetYandexLanguageFromWebGL();
        if (!string.IsNullOrEmpty(webGLLanguage))
            return webGLLanguage;

        try
        {
            Type yg2Type = typeof(YG2);
            string[] members = { "lang", "language", "currentLanguage", "Language" };

            for (int i = 0; i < members.Length; i++)
            {
                PropertyInfo prop = yg2Type.GetProperty(members[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    string value = prop.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }

                FieldInfo field = yg2Type.GetField(members[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null && field.FieldType == typeof(string))
                {
                    string value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }

#if UNITY_EDITOR
            if (YG2.infoYG != null && YG2.infoYG.Simulation != null && !string.IsNullOrEmpty(YG2.infoYG.Simulation.language))
                return YG2.infoYG.Simulation.language;
#endif
        }
        catch
        {
            // SDK language is optional here. Fallback logic below handles this.
        }

        return null;
    }

    private static string TryGetYandexLanguageFromWebGL()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        IntPtr langPtr = IntPtr.Zero;
        try
        {
            langPtr = GetYandexUserLanguage_js();
            if (langPtr == IntPtr.Zero)
                return null;

            string lang = Marshal.PtrToStringAnsi(langPtr);
            return string.IsNullOrWhiteSpace(lang) ? null : lang;
        }
        catch
        {
            return null;
        }
        finally
        {
            if (langPtr != IntPtr.Zero)
                FreeBuffer_js(langPtr);
        }
#else
        return null;
#endif
    }

    private static bool TryApplyToI18N(string languageCode)
    {
        if (TryInvokeStaticMethod("I18N", "SetLanguage", languageCode))
            return true;

        if (TryInvokeStaticMethod("I18N.I18N", "SetLanguage", languageCode))
            return true;

        Type i2Type = Type.GetType("I2.Loc.LocalizationManager, I2.Loc");
        if (i2Type != null)
        {
            PropertyInfo currentLanguage = i2Type.GetProperty("CurrentLanguage", BindingFlags.Public | BindingFlags.Static);
            if (currentLanguage != null && currentLanguage.CanWrite)
            {
                currentLanguage.SetValue(null, languageCode);
                return true;
            }

            if (TryInvokeStaticMethod(i2Type, "SetLanguage", languageCode))
                return true;
        }

        return false;
    }

    private static bool TryInvokeStaticMethod(string typeName, string methodName, string languageCode)
    {
        Type type = Type.GetType(typeName);
        if (type == null)
            return false;

        return TryInvokeStaticMethod(type, methodName, languageCode);
    }

    private static bool TryInvokeStaticMethod(Type type, string methodName, string languageCode)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
        if (method == null)
            return false;

        method.Invoke(null, new object[] { languageCode });
        return true;
    }

    private static string ToSupportedLanguageCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        code = code.Trim().Replace('_', '-').ToLowerInvariant();

        if (code.StartsWith("ru") || code == "russian")
            return "ru";

        if (code.StartsWith("en") || code == "english")
            return "en";

        return null;
    }

    private static string SystemLanguageToCode(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Russian:
                return "ru";
            case SystemLanguage.English:
                return "en";
            default:
                return null;
        }
    }

    private static string FallbackToCode(SupportedLanguage fallback)
    {
        return fallback == SupportedLanguage.Russian ? "ru" : "en";
    }
}
//vless://9d5d9127-e76a-4fb1-8970-dbeec242357f@91.184.242.28:443?type=tcp&security=reality&sni=botapi.max.ru&fp=chrome&pbk=pDM-rfIQAshI7SKfjvz8ZPcxUP6mN_1JzNuD2CYAjm0&sid=a983d364ce1cdb7d&flow=xtls-rprx-vision#reality-443