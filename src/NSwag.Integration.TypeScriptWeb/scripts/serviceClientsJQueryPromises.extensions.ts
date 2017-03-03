import * as generated from "./serviceClientsJQueryPromises";

class Person extends generated.PersonBase {
    get fullName() {
        return this.firstName + " " + this.lastName;
    }
}