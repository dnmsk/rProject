window.ninjaBag.functionsOnReady.append({
    'setGefaultGmtCookie': function () {
        window.ninjaBag.baseFunctions.setCookie(window.ninjaBag.documentObjects.Const.CookieGmt, -new Date().getTimezoneOffset(), { expires: 365 * 24 * 60 * 60 * 1000 });
    },
    'setFullWidthController': function () {
        var onResize = function () {
            var newBodyWidth = $(window).width();
            $.each($('[data-fullwidth]'), function (idx, el) {
                var jqEl = $(el);
                var halfElWidth = (newBodyWidth - jqEl.parent().outerWidth()) / 2;
                jqEl.css({
                    marginLeft: -halfElWidth,
                    marginRight: -halfElWidth
                });
            });
        };
        onResize();
        $(window).on('resize', onResize);
    }
});
