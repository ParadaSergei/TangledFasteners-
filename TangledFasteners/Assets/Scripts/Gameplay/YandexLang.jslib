mergeInto(LibraryManager.library, {
    GetYandexLanguage_js: function() {
        var lang = "ru";
        try {
            if (typeof ysdk !== 'undefined' && ysdk && ysdk.environment && ysdk.environment.i18n && ysdk.environment.i18n.lang) {
                lang = ysdk.environment.i18n.lang;
            } else if (typeof navigator !== 'undefined' && navigator && navigator.language) {
                lang = navigator.language;
            }
        } catch (e) {
            console.error("GetYandexLanguage_js error: ", e);
        }
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    }
});
