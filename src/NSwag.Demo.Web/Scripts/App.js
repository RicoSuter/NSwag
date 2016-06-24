define(["require", "exports", "DataService"], function (require, exports, dataService) {
    "use strict";
    var client = new dataService.DataService();
    client.baseUrl = "http://localhost:22093";
    client.addHour(new Date(2012, 12, 25, 10, 15, 20), function (result) {
        var x = 10;
    });
    client.calculate(1, 2, 3, function (sum) {
        document.getElementById("content").innerHTML = "Sum: " + sum;
    });
});
//# sourceMappingURL=App.js.map