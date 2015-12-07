window.ninjaBag = new (function () {
    this.baseFunctions = {
        append: function (jsObject, target) {
            if (!jsObject) {
                return;
            }
            var bag = target ? target : this;
            var parseData = function (scope, element) {
                for (var index in element) {
                    var val = element[index];
                    if (typeof (val) != 'object' || !val) {
                        scope[index] = val;
                    } else {
                        var nextDeep = scope[index];
                        if (nextDeep == undefined) {
                            nextDeep = {};
                            scope[index] = nextDeep;
                        }
                        parseData(nextDeep, val);
                    }
                }
            };
            parseData(bag, jsObject);
        }
    };
    var that = this;
    this.baseObjects = {
        Store: function (data) {
            this.append = that.baseFunctions.append;
            this.append(data);
        },
        StoreReadonly: function (data) {
            this.append = that.baseFunctions.append;
            this.append(data);
            this.append = function() {
                alert("Object can't modify");
            };
            if (Object.freeze) {
                Object.freeze(this);
            }
        },
    };
})();

window.ninjaBag.documentObjects = new window.ninjaBag.baseObjects.Store();
window.ninjaBag.functions = new window.ninjaBag.baseObjects.Store();
window.ninjaBag.functionsOnReady = new window.ninjaBag.baseObjects.Store();
window.ninjaBag.deferredObjects = [];
