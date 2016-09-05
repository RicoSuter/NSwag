var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
define(["require", "exports", "serviceClientsJQueryPromises"], function (require, exports, generated) {
    "use strict";
    var Person = (function (_super) {
        __extends(Person, _super);
        function Person() {
            _super.apply(this, arguments);
        }
        Object.defineProperty(Person.prototype, "fullName", {
            get: function () {
                return this.firstName + " " + this.lastName;
            },
            enumerable: true,
            configurable: true
        });
        return Person;
    }(generated.PersonBase));
});
//# sourceMappingURL=serviceClientsJQueryPromises.extensions.js.map