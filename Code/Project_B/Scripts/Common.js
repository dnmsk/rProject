function Declinate(forms, number, showNumber) {
    var mod = number % 10;
    var mod100 = number % 100;
    var index = 2;

    if (mod100 <= 10 || mod100 >= 20) {
        if (mod == 1) {
            index = 0;
        }
        if (mod >= 2 && mod <= 4) {
            index = 1;
        }
    }
    return (showNumber ? number + " " : "") + $.validator.format(forms[index], number);
}

window.inputEncoding = function (obj) {
    if (!obj && typeof (obj) != 'boolean') {
        return undefined;
    }
    var str = '';
    if (typeof obj == 'object') {
        for (var index in obj) {
            str += '&' + encodeURIComponent(index) + '=' + encodeURIComponent(inputEncoding(obj[index]));
        }
        return str.length > 0 ? str.substring(1) : undefined;
    }
    return obj;
};

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};
String.prototype.replaceAll = function (search, replace) {
    return this.split(search).join(replace);
};
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}