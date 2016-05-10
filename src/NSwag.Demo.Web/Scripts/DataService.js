// Generated using the NSwag toolchain v2.18.5973.40474 (http://NSwag.org)
define(["require", "exports"], function (require, exports) {
    var PersonsClient = (function () {
        function PersonsClient(baseUrl) {
            this.baseUrl = undefined;
            this.beforeSend = undefined;
            this.baseUrl = baseUrl !== undefined ? baseUrl : "";
        }
        PersonsClient.prototype.xyz = function (data, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Person/xyz/{data}?";
            if (data === undefined || data === null)
                throw new Error("The parameter 'data' must be defined.");
            url = url.replace("{data}", "" + data);
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "put",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processXyz(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processXyz(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processXyz = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        /**
         * @deprecated
         */
        PersonsClient.prototype.getAll = function (onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Get?";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processGetAll(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processGetAll(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processGetAll = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        /**
         * Gets a person.
         * @id The ID of the person.
         * @return The person.
         */
        PersonsClient.prototype.get = function (id, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Get/{id}?";
            if (id === undefined || id === null)
                throw new Error("The parameter 'id' must be defined.");
            url = url.replace("{id}", "" + id);
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processGet(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processGet(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processGet = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else if (status === "500") {
                var result500 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result500 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onFail !== undefined)
                    onFail(result500, "error_exception");
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        /**
         * Creates a new person.
         * @value (optional) The person.
         */
        PersonsClient.prototype.post = function (value, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Post?";
            var content = JSON.stringify(value);
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "post",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processPost(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processPost(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processPost = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "204") {
                var result204 = undefined;
                if (onSuccess !== undefined)
                    onSuccess(result204);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        /**
         * Updates the existing person.
         * @id The ID.
         * @value (optional) The person.
         */
        PersonsClient.prototype.put = function (id, value, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Put/{id}?";
            if (id === undefined || id === null)
                throw new Error("The parameter 'id' must be defined.");
            url = url.replace("{id}", "" + id);
            var content = JSON.stringify(value);
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "put",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processPut(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processPut(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processPut = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "204") {
                var result204 = undefined;
                if (onSuccess !== undefined)
                    onSuccess(result204);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        PersonsClient.prototype.delete = function (id, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Delete/{id}?";
            if (id === undefined || id === null)
                throw new Error("The parameter 'id' must be defined.");
            url = url.replace("{id}", "" + id);
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "delete",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processDelete(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processDelete(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processDelete = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "204") {
                var result204 = undefined;
                if (onSuccess !== undefined)
                    onSuccess(result204);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        /**
         * Calculates the sum of a, b and c.
         */
        PersonsClient.prototype.calculate = function (a, b, c, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Person/Calculate/{a}/{b}?";
            if (a === undefined || a === null)
                throw new Error("The parameter 'a' must be defined.");
            url = url.replace("{a}", "" + a);
            if (b === undefined || b === null)
                throw new Error("The parameter 'b' must be defined.");
            url = url.replace("{b}", "" + b);
            if (c === undefined || c === null)
                throw new Error("The parameter 'c' must be defined.");
            else
                url += "c=" + encodeURIComponent("" + c) + "&";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processCalculate(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processCalculate(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processCalculate = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        PersonsClient.prototype.addHour = function (time, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/AddHour?";
            if (time === undefined || time === null)
                throw new Error("The parameter 'time' must be defined.");
            else
                url += "time=" + encodeURIComponent("" + time.toJSON()) + "&";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processAddHour(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processAddHour(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processAddHour = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = new Date(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        PersonsClient.prototype.test = function (onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/TestAsync?";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processTest(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processTest(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processTest = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        PersonsClient.prototype.loadComplexObject = function (onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/LoadComplexObject?";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8"
                }
            }).done(function (data, textStatus, xhr) {
                _this.processLoadComplexObject(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processLoadComplexObject(xhr, onSuccess, onFail);
            });
        };
        PersonsClient.prototype.processLoadComplexObject = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result200 = data === "" ? null : jQuery.parseJSON(data);
                    }
                    catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        return PersonsClient;
    })();
    exports.PersonsClient = PersonsClient;
    (function (ObjectType) {
        ObjectType[ObjectType["Foo"] = "Foo"] = "Foo";
        ObjectType[ObjectType["Bar"] = "Bar"] = "Bar";
    })(exports.ObjectType || (exports.ObjectType = {}));
    var ObjectType = exports.ObjectType;
});
//# sourceMappingURL=DataService.js.map