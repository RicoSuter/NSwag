// Generated using the NSwag toolchain v1.15.5831.1699 (http://NSwag.org)
define(["require", "exports"], function (require, exports) {
    (function (ObjectType) {
        ObjectType[ObjectType["Foo"] = "Foo"] = "Foo";
        ObjectType[ObjectType["Bar"] = "Bar"] = "Bar";
    })(exports.ObjectType || (exports.ObjectType = {}));
    var ObjectType = exports.ObjectType;
    var Client = (function () {
        function Client(baseUrl) {
            this.baseUrl = undefined;
            this.beforeSend = undefined;
            this.baseUrl = baseUrl !== undefined ? baseUrl : "";
        }
        Client.prototype.xyz = function (data, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Person/xyz/{data}?";
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
        Client.prototype.processXyz = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
        Client.prototype.getAll = function (onSuccess, onFail) {
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
        Client.prototype.processGetAll = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
        Client.prototype.get = function (id, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Get/{id}?";
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
        Client.prototype.processGet = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
                if (onSuccess !== undefined)
                    onSuccess(result200);
                return;
            }
            else if (status === "500") {
                var result500 = null;
                try {
                    result500 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
         * @value The person.
         */
        Client.prototype.post = function (value, onSuccess, onFail) {
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
        Client.prototype.processPost = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "204") {
                var result204 = null;
                try {
                    result204 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
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
         * @value The person.
         */
        Client.prototype.put = function (id, value, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Put/{id}?";
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
        Client.prototype.processPut = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "204") {
                var result204 = null;
                try {
                    result204 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
                if (onSuccess !== undefined)
                    onSuccess(result204);
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
        };
        Client.prototype.delete = function (id, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/Delete/{id}?";
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
        Client.prototype.processDelete = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "204") {
                var result204 = null;
                try {
                    result204 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
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
        Client.prototype.calculate = function (a, b, c, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Person/Calculate/{a}/{b}?";
            url = url.replace("{a}", "" + a);
            url = url.replace("{b}", "" + b);
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
        Client.prototype.processCalculate = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
        Client.prototype.addHour = function (time, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/api/Persons/AddHour?";
            url += "time=" + encodeURIComponent("" + time) + "&";
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
        Client.prototype.processAddHour = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = new Date(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
        Client.prototype.test = function (onSuccess, onFail) {
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
        Client.prototype.processTest = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
        Client.prototype.loadComplexObject = function (onSuccess, onFail) {
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
        Client.prototype.processLoadComplexObject = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = data === "" ? null : jQuery.parseJSON(data);
                }
                catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
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
        return Client;
    })();
    exports.Client = Client;
});
//# sourceMappingURL=DataService.js.map