mergeInto(LibraryManager.library, {
    GetYandexUserLanguage_js: function () {
        var lang = '';

        try {
            if (typeof ysdk !== 'undefined' && ysdk && ysdk.environment && ysdk.environment.i18n && ysdk.environment.i18n.lang) {
                lang = ysdk.environment.i18n.lang;
            }
        } catch (e) {
            lang = '';
        }

        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    }
});
