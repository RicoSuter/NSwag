// Generated using the NSwag toolchain v1.12.5825.38522 (http://NSwag.org)
define(["require", "exports"], function (require, exports) {
    var DataService = (function () {
        function DataService(baseUrl) {
            this.baseUrl = undefined;
            this.beforeSend = undefined;
            this.baseUrl = baseUrl !== undefined ? baseUrl : "";
        }
        DataService.prototype.getAll = function (onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Get?";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processGetAll(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processGetAll(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processGetAll = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        /**
         * Gets a person.
         * @id The ID of the person.
         * @return The person.
         */
        DataService.prototype.get = function (id, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Get/{id}?";
            url = url.replace("{id}", "" + id);
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processGet(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processGet(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processGet = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    result500 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        /**
         * Creates a new person.
         * @request The person.
         */
        DataService.prototype.post = function (request, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Post?";
            var content = JSON.stringify(request);
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "post",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processPost(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processPost(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processPost = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        /**
         * Updates the existing person.
         * @id The ID.
         * @request The person.
         */
        DataService.prototype.put = function (id, request, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Put/{id}?";
            url = url.replace("{id}", "" + id);
            var content = JSON.stringify(request);
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "put",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processPut(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processPut(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processPut = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        DataService.prototype.delete = function (id, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Delete/{id}?";
            url = url.replace("{id}", "" + id);
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "delete",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processDelete(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processDelete(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processDelete = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        /**
         * Calculates the sum of a, b and c.
         */
        DataService.prototype.calculate = function (a, b, c, onSuccess, onFail) {
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
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processCalculate(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processCalculate(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processCalculate = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        DataService.prototype.addHour = function (time, onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/AddHour?";
            url += "time=" + encodeURIComponent("" + time.toJSON()) + "&";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processAddHour(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processAddHour(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processAddHour = function (xhr, onSuccess, onFail) {
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        DataService.prototype.loadComplexObject = function (onSuccess, onFail) {
            var _this = this;
            var url = this.baseUrl + "/MyWorldCalculators/api/Persons/LoadComplexObject?";
            var content = "";
            $.ajax({
                url: url,
                beforeSend: this.beforeSend,
                type: "get",
                data: content,
                dataType: "text",
                contentType: "application/json; charset=UTF-8"
            }).done(function (data, textStatus, xhr) {
                _this.processLoadComplexObject(xhr, onSuccess, onFail);
            }).fail(function (xhr) {
                _this.processLoadComplexObject(xhr, onSuccess, onFail);
            });
        };
        DataService.prototype.processLoadComplexObject = function (xhr, onSuccess, onFail) {
            var data = xhr.responseText;
            var status = xhr.status.toString();
            if (status === "200") {
                var result200 = null;
                try {
                    result200 = jQuery.parseJSON(data);
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
                    onFail(null, "error_no_callback_for_status");
            }
        };
        return DataService;
    })();
    exports.DataService = DataService;
});
//# sourceMappingURL=DataService.js.map