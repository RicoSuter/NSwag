<<<<<<< HEAD
﻿import generated = require("serviceClientsJQueryPromises");
=======
﻿import * as generated from "./serviceClientsJQueryPromises";
>>>>>>> refs/remotes/NSwag/master

class Person extends generated.PersonBase {
    get fullName() {
        return this.firstName + " " + this.lastName;
    }
}