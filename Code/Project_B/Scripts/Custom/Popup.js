window.ninjaBag.baseObjects.Popup = function (config) {
    this.consts = {
        minimalWindowHeight: 640,
    };
    this.settings = {
        onShowFunc: undefined,
        onHideFunc: undefined,
        container: undefined,
        requestUrlFunc: undefined,
        onLoadFunc: undefined,
        requestData: undefined,
        realoadAllTimes: undefined,

        verticalCenter: false,
        marginTop: 60,
        marginBottom: 100,
        transparentMask: false,
        borderRound: true,
        padding: true,
        height: undefined,
        width: 600,
        zIndex: 1000,
        contentClose: false,

        resizable: false,
        onResizeFunc: undefined
    };

    this.content = undefined;
    this.contentLoaded = false;

    var that = this;
    if (config != undefined) {
        $.extend(that.settings, config);
    }

    var draw = function () {
        that.settings.container.css("z-index", that.settings.zIndex);
        if (that.settings.height) {
            that.content.css("height", that.settings.height + "px");
        }
        if (that.settings['min-height']) {
            that.content.css("min-height", that.settings['min-height'] + "px");
        }
        var marginBottom = that.settings.marginBottom;
        var marginTop = that.settings.marginTop;
        if ($(window).height() <= that.consts.minimalWindowHeight || that.settings.height >= $(window).height()) {
            marginTop = 0;
            marginBottom = 0;
            that.settings.verticalCenter = false;
        }
        that.content
            .css("z-index", that.settings.zIndex + 1)
            .css("margin-top", marginTop + "px")
            .css("margin-bottom", marginBottom + "px")
            .css("width", that.settings.width + "px");
        var crossDoubleMargin = 120;
        if ($(window).width() - that.content.width() - crossDoubleMargin <= 0) {
            that.settings.container.find('.c-popup__close').hide();
            that.content.find('.c-popup__content-close').show();
        } else {
            that.settings.container.find('.c-popup__close').show();
            that.content.find('.c-popup__content-close').hide();
        }

    };

    var appendHtml = function () {
        that.content = that.settings.container.find('.c-popup__content');
        that.content.append('<div class="c-popup__content-close" data-type="close-popup">×</div>');
        that.settings.container.append('<div class="c-popup__close" data-type="close-popup"></div>');
        if (that.settings.contentClose) {
            that.settings.container.find('.c-popup__close').hide();
        } else {
            that.content.find('.c-popup__content-close').hide();
        }
        if (that.settings.borderRound) {
            that.content.addClass("c-popup__content_round");
        }
        if (that.settings.padding) {
            that.content.addClass("c-popup__content_default-padding");
        }
    };

    var bindEvents = function () {
        that.settings.container.on('mouseover', function () {
            that.settings.container.addClass('c-popup_hover');
        });
        that.settings.container.on('mouseout', function () {
            that.settings.container.removeClass('c-popup_hover');
        });
        that.settings.container.on('click', that.hide);
        that.settings.container.find('[data-type="close-popup"]')
            .css("z-index", that.settings.zIndex + 2)
            .on('click', that.hide);
        that.content.on('click', function (event) {
            event.stopPropagation();
        });
        that.content.on('mouseover', function (event) {
            event.stopPropagation();
        });
        that.content.on('mouseout', function (event) {
            event.stopPropagation();
        });

        if (that.settings.resizable) {
            var redraw = function (currentWindowHeight, currentWindowWidth) {
                if (that.settings.onRedrawFunc) {
                    that.settings.onRedrawFunc(currentWindowHeight, currentWindowWidth);
                }
                draw();
            };
            $(window).resize(function () {
                var windowHeight = $(window).height();
                var windowWidth = $(window).width();
                setTimeout(function (e) {
                    var currentWindowHeight = $(window).height();
                    var currentWindowWidth = $(window).width();
                    if (windowHeight !== currentWindowHeight || windowWidth !== currentWindowWidth) {
                        windowHeight = currentWindowHeight;
                        windowWidth = currentWindowWidth;
                    } else {
                        redraw(currentWindowHeight, currentWindowWidth);
                    }
                }, 500);
            });

            $(window).on('orientationchange', function () {
                redraw($(window).height(), $(window).width());
            });
        }
    };

    var setPosition = function () {
        if (that.settings.verticalCenter && that.content.height() > 0) {
            that.settings.marginTop = ($(window).height() - that.content.height()) / 2;
            that.settings.verticalCenter = false;
            that.content.css("marginTop", that.settings.marginTop);
        }
    };

    this.isVisible = function () {
        return that.settings.container.is(':visible');
    };

    this.show = function () {
        if (!that.isVisible()) {
            if (that.settings.requestUrlFunc && (!that.contentLoaded || that.settings.realoadAllTimes)) {
                if (that.settings.height) {
                    that.content.css("height", that.settings.height + "px");
                }

                that.content.html('<div class="c-popup__loading-content"></div>');
                var params = {
                    type: 'GET',
                    traditional: true,
                    success: function (data) {
                        that.contentLoaded = true;
                        that.content
                            .css("height", "")
                            .html(data);
                        setPosition();
                        //$.validator.unobtrusive.parse(that.content);

                        if (that.settings.onLoadFunc && that.settings.onLoadFunc()) {}

                        //$(that).trigger(window.bag.data.Events.Popup.onShow);
                    }
                };
                if (that.settings.requestData) {
                    params.data = that.settings.requestData;
                }
                $.ajax(that.settings.requestUrlFunc(), params);
            } else {
                //$(that).trigger(window.bag.data.Events.Popup.onShow);
            }
            that.settings.container.show();
            setPosition();
            $('body').css("overflow", "hidden");
            if (that.settings.onShowFunc) {
                that.settings.onShowFunc();
            }
        }
    };

    this.showAlone = function () {
        $('.c-popup').hide();
        this.show();
    };

    this.hide = function () {
        if (that.isVisible()) {
            that.settings.container.hide();
            if ($('.c-popup:visible').length === 0) {
                $('body').css("overflow", "visible");
            }
            if (that.settings.onHideFunc) {
                that.settings.onHideFunc();
            }
            //$(that).trigger(window.bag.data.Events.Popup.onHide);
        }
    };

    this.init = function () {
        appendHtml();
        draw();
        bindEvents();
    };

    this.init();
}