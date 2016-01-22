//https://github.com/powerbuoy/SleekWP/blob/master/js/foot-2-live-search.js
//http://andreaslagerkvist.com/projects/live-ajax-search/
(function ($, window, document) {
 /**
 * LiveSearch 1.0
 *
 * TODO
 */
    var LiveSearch = {
        init: function(input, conf) {
            var config = {
                url: conf.url || false,
                appendTo: conf.appendTo || 'after',
                data: conf.data,
                onSuccessFunc: conf.onSuccessFunc
            };

            var appendTo = appendTo || 'after';

            input.setAttribute('autocomplete', 'off');

            // Create search container
            var container = document.createElement('div');

            container.id = 'live-search-' + input.name;

            container.classList.add('live-search');

            // Append search container
            if (config.appendTo === 'after') {
                input.parentNode.classList.add('live-search-wrap');
                input.parentNode.insertBefore(container, input.nextSibling);
            } else {
                config.appendTo.append(container);
            }

            var doSearch = function (input) {
                if (input.value != input.liveSearchLastValue) {
                    input.classList.add('loading');

                    var q = input.value;

                    // Clear previous ajax request
                    if (input.liveSearchTimer) {
                        clearTimeout(input.liveSearchTimer);
                    }

                    // Build the URL
                    var url = config.url + q;

                    if (config.data) {
                        if (url.indexOf('&') != -1 || url.indexOf('?') != -1) {
                            url += '&' + LiveSearch.serialize(config.data);
                        } else {
                            url += '?' + LiveSearch.serialize(config.data);
                        }
                    }

                    // Wait a little then send the request

                    input.liveSearchTimer = setTimeout(function () {
                        if (q) {
                            $.ajax(url, {
                                type: "GET",
                                traditional: true,
                                success: function (data) {
                                    input.classList.remove('loading');
                                    container.innerHTML = data;
                                    if (config.onSuccessFunc && config.onSuccessFunc(data)) {
                                    }
                                }
                            });
                        } else {
                            container.innerHTML = '';
                        }
                    }, 300);

                    input.liveSearchLastValue = input.value;
                }
            };

            // Hook up keyup on input
            input.addEventListener('keyup', function(e) {
                doSearch(this);
            });
            
            doSearch(input);
        },

        // http://stackoverflow.com/questions/1714786/querystring-encoding-of-a-javascript-object
        serialize: function (obj) {
            var str = [];

            for (var p in obj) {
                if (obj.hasOwnProperty(p)) {
                    str.push(encodeURIComponent(p) + '=' + encodeURIComponent(obj[p]));
                }
            }

            return str.join('&');
        }
    };

    jQuery.fn.liveSearch = function (conf) {
        return this.each(function () {
            LiveSearch.init(this, conf);
        });
    };
}(jQuery, this, document));
