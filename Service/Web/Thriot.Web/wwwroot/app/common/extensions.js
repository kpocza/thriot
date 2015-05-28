//region Array extensions

//Contains
Array.prototype.contains = function (item) {
    if (!item) {
        return false;
    }

    return this.indexOf(item) !== -1;
};

//Contains Expression
Array.prototype.containsExpression = function (expression) {
    var item = this.firstOrDefault(expression);

    return hasValue(item);
};

//Count
Array.prototype.count = function (expression) {
    if (!expression) {
        throw Error('expression cannot be null');
    }

    var count = 0;

    for (var i = 0; i < this.length; i++) {
        if (expression(this[i])) {
            count++;
        }
    }
    return count;
};

//Push Array
Array.prototype.pushArray = function (list) {
    if (!list) {
        throw Error('list cannot be null');
    }

    for (var i = 0; i < list.length; i++) {
        this.push(list[i]);
    }

    return this;
};

//Distinct
Array.prototype.distinct = function () {
    var unique = [];

    for (var i = 0, len = this.length; i < len; i++) {
        var item = this[i];
        if (unique.indexOf(item) === -1) {
            unique.push(item);
        }
    }

    return unique;
};

//Append
Array.prototype.append = function (list) {
    if (!list) {
        throw Error('list cannot be null');
    }

    var returnList = [];
    returnList.pushArray(this);

    for (var i = 0; i < list.length; i++) {
        returnList.push(list[i]);
    }

    return returnList;
};

//Clone
Array.prototype.cloneArray = function () {
    var returnArray = [];

    for (var i = 0; i < this.length; i++) {
        returnArray.push(this[i]);
    }
    return returnArray;
};

//Remove
Array.prototype.remove = function (item) {
    var i = this.indexOf(item);

    if (i === -1) {
        return;
    }
    this.splice(i, 1);
};

//Remove with expression
Array.prototype.removeWithExpression = function (expression) {
    for (var i = 0; i < this.length; i++) {
        if (expression(this[i])) {
            this.remove(this[i]);
            return;
        }
    }
};

//foreach with expression
Array.prototype.forEachExpression = function (expression) {
    for (var i = 0; i < this.length; i++) {
        expression(this[i], i);
    }
};

//First Or Default
Array.prototype.firstOrDefault = function (expression) {
    for (var i = 0; i < this.length; i++) {
        if (expression(this[i])) {
            return this[i];
        }
    }
    return null;
};

//Last
Array.prototype.last = function () {
    return this[this.length - 1];
};

//Any
Array.prototype.any = function (expression) {
    var found = this.firstOrDefault(expression);
    return found !== null;
};

//All
Array.prototype.all = function (expression) {
    for (var i = 0; i < this.length; i++) {
        if (!expression(this[i])) {
            return false;
        }
    }
    return true;
};

//Where
Array.prototype.where = function (expression) {
    var returnList = [];

    for (var i = 0; i < this.length; i++) {
        if (expression(this[i])) {
            returnList.push(this[i]);
        }
    }
    return returnList;
};

Array.prototype.takeRange = function (startIdx, count) {
    var returnList = [];

    for (var i = startIdx; i < startIdx + count && i < this.length; i++) {
        returnList.push(this[i]);
    }
    return returnList;
};

//Clear
Array.prototype.clear = function () {
    this.length = 0;
    return this;
};

//Select
Array.prototype.select = function (property) {
    var returnArray = [];

    for (var i = 0; i < this.length; i++) {
        returnArray.push(this[i][property]);
    }
    return returnArray;
};

//Select New
Array.prototype.selectNew = function (makeFunction) {
    var returnArray = [];

    for (var i = 0; i < this.length; i++) {
        returnArray.push(makeFunction(this[i], i));
    }
    return returnArray;
};

//IsEqual
Array.prototype.isEqual = function (array) {
    if (this.length !== array.length) {
        return false;
    }

    for (var i = 0; i < this.length; i++) {
        if (array[i] !== this[i]) {
            return false;
        }
    }

    return true;
};

//region Array extensions

//region Number extensions

Number.prototype.toSigned = function () {
    if (this > 0) {
        return '+' + this;
    }

    return this;
};

//endregion Number extensions

//region String extensions

//Contains
String.prototype.contains = function (text) {
    return this.indexOf(text) !== -1;
};

//InsertAtIndex
String.prototype.insertAtIndex = function (index, text) {
    return this.slice(0, index) + text + this.slice(index);
};

//Count
String.prototype.count = function (charac) {
    if (charac.length > 1) {
        throw new Error('only one character');
    }

    var count = 0;
    for (var i = 0; i < this.length; i++) {
        if (this[i] === charac) {
            count++;
        }
    }
    return count;
};

//Truncate
String.prototype.truncate = function (length) {
    if (length < 6) {
        return this;
    }

    return this.substring(0, length - 3) + (length < this.length ? '...' : '');
};

//Equals
String.prototype.isEqual = function (string) {
    return this.toLowerCase() === string.toLowerCase();
};

//ToUpperStart
String.prototype.toUpperStart = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
};

//Format
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

//endregion String extensions

//region Object extensions

function hasValue(obj) {
    return obj !== null && obj !== undefined;
}

//endregion  Object extensions
