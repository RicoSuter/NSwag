var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
define(["require", "exports", "serviceClientsFetch"], function (require, exports, generated) {
    "use strict";
    var GeoClient = (function (_super) {
        __extends(GeoClient, _super);
        function GeoClient(baseUrl, http) {
            _super.call(this, baseUrl, http);
            //this.jsonParseReviver = (key: string, value: any) => value;
        }
        return GeoClient;
    }(generated.GeoClientBase));
});
//# sourceMappingURL=serviceClientsFetch.extensions.js.map