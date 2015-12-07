window.errors = [];
window.logError = function (type, target) {
    window.errors.push(type);
    var url = window.ninjaBag.links.Log.JsError;
    var data = {
        message: encodeURIComponent(inputEncoding(type)),
        target: encodeURIComponent(inputEncoding(target)),
        location: encodeURIComponent(window.location)
    };
    try {
        var request = window.ninjaBag.baseFunctions.getXmlHttp();
        request.open("POST", url + '?' + inputEncoding(data), true);
        request.setRequestHeader('Content-Type', 'xml');
        request.send(null);
    } catch (e) {
    }
    if (typeof (console) != 'undefined' && typeof (console.log) != 'undefined') {
        console.log("Catched error: " + type);
    }
};
window.ninjaBag.baseFunctions.append({
    'logFeature': function (featureId, objectId) {
        var data = { featureID: featureId };
        if (objectId) {
            data.objectID = objectId;
        }
        var request = window.ninjaBag.baseFunctions.getXmlHttp();
        request.open("POST", window.ninjaBag.links.Log.Feature + '?' + inputEncoding(data));
        request.setRequestHeader('Content-Type', 'xml');
        request.send(null);
    },
    'getXmlHttp': function() {
        if (typeof XMLHttpRequest != 'undefined') {
            return new XMLHttpRequest();
        }
        try {
            return new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                return new ActiveXObject("Microsoft.XMLHTTP");
            } catch (E) {}
        }
        return false;
    },
    'addAutoReloadEvent': function (action, objectId) {
        var cookie = window.ninjaBag.baseFunctions.getCookie('actions');
        if (cookie != null) {
            window.ninjaBag.baseFunctions.setCookie('actions', cookie + ',' + action + (objectId == undefined ? "" : ":" + objectId), { path: '/' });
        } else {
            window.ninjaBag.baseFunctions.setCookie('actions', action + (objectId == undefined ? "" : ":" + objectId), { path: '/' });
        }
    },
    'postAutoReloadEvents': function () {
        var cookie = window.ninjaBag.baseFunctions.getCookie('actions');
        if (cookie != null) {
            var valuesToSend = cookie.split(',');
            for (var i in valuesToSend) {
                if (valuesToSend.hasOwnProperty(i)) {
                    var pars = valuesToSend[i].split(':');
                    window.ninjaBag.baseFunctions.logFeature(pars[0], pars[1]);
                }
            }
            window.ninjaBag.baseFunctions.setCookie('actions', null, { path: '/', expires: -1 });
        }
    },
    'setCookie': function(name, value, options) {
        options = options || {};
        var expires = options.expires;
        if (typeof expires == "number" && expires) {
            var d = new Date();
            d.setTime(d.getTime() + expires * 1000);
            expires = options.expires = d;
        }
        if (expires && expires.toUTCString) {
            options.expires = expires.toUTCString();
        }
        value = encodeURIComponent(value);
        var updatedCookie = name + "=" + value;
        for (var propName in options) {
            if (options.hasOwnProperty(propName)) {
                updatedCookie += "; " + propName;
                var propValue = options[propName];
                if (propValue !== true) {
                    updatedCookie += "=" + propValue;
                }
            }
        }
        document.cookie = updatedCookie;
    },
    'getCookie': function(name) {
        var matches = document.cookie.match(new RegExp("(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"));
        return matches ? decodeURIComponent(matches[1]) : undefined;
    },
    'watchForGoals' : function(contentContainer, itemSelector, func) {
        if (!contentContainer) {
            contentContainer = document.getElementsByTagName('body')[0];
        }
        var querySelectorAll = contentContainer.querySelectorAll(itemSelector);
        for (var index in querySelectorAll) {
            if (querySelectorAll.hasOwnProperty(index)) {
                if (querySelectorAll[index] && querySelectorAll[index].addEventListener) {
                    querySelectorAll[index]
                        .addEventListener('click', function() {
                            func(this);
                        }, this);
                }
            }
        }
    },
    'watchForFeatures' : function(contentContainer) {
        window.ninjaBag.baseFunctions.watchForGoals(contentContainer, '[data-feature]', function (item) {
            var dataFeature = item.attributes['data-feature'];
            dataFeature = dataFeature ? dataFeature.value : undefined;

            var dataFeatureId = item.attributes['data-feature-id'];
            dataFeatureId = dataFeatureId ? dataFeatureId.value : undefined;

            if (item.tagName === 'A' && item.href && !item.href.endsWith('#') && item.target != "_blank") {
                window.ninjaBag.baseFunctions.addAutoReloadEvent(dataFeature, dataFeatureId);
                return true;
            }
            window.ninjaBag.baseFunctions.logFeature(dataFeature, dataFeatureId);
        });
    }
});
window.ninjaBag.functionsOnReady.append({ 'setWatchUseFeatures': window.ninjaBag.baseFunctions.watchForFeatures });