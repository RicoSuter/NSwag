import generated = require("serviceClientsJQueryPromises");

class Person extends generated.PersonBase {
    get fullName() {
        return this.firstName + " " + this.lastName;
    }
}