import * as generated from "./serviceClientsJQueryPromises";

class Person extends generated.Person {
    get fullName() {
        return this.firstName + " " + this.lastName;
    }
}