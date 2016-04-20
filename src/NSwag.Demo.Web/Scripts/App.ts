import dataService = require("DataService");

var client = new dataService.PersonsClient();
client.baseUrl = "http://localhost:22093";

client.addHour(new Date(2012, 12, 25, 10, 15, 20), result => {
    var x = 10; 
});

client.calculate(1, 2, 3, (sum) => {
    document.getElementById("content").innerHTML = "Sum: " + sum;
});