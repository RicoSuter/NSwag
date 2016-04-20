// Generated using the NSwag toolchain v2.6.5954.30417 (http://NSwag.org)

export interface IPersonsClient {
    xyz(data: any, onSuccess?: (result: string) => void, onFail?: (exception: string, reason: string) => void): void;
    getAll(onSuccess?: (result: Person[]) => void, onFail?: (exception: string, reason: string) => void): void;
    /**
     * Gets a person.
     * @id The ID of the person.
     * @return The person.
     */
    get(id: number, onSuccess?: (result: Person) => void, onFail?: (exception: PersonNotFoundException | string, reason: string) => void): void;
    /**
     * Creates a new person.
     * @value (optional) The person.
     */
    post(value: Person, onSuccess?: () => void, onFail?: (exception: string, reason: string) => void): void;
    /**
     * Updates the existing person.
     * @id The ID.
     * @value (optional) The person.
     */
    put(id: number, value: Person, onSuccess?: () => void, onFail?: (exception: string, reason: string) => void): void;
    delete(id: number, onSuccess?: () => void, onFail?: (exception: string, reason: string) => void): void;
    /**
     * Calculates the sum of a, b and c.
     */
    calculate(a: number, b: number, c: number, onSuccess?: (result: number) => void, onFail?: (exception: string, reason: string) => void): void;
    addHour(time: Date, onSuccess?: (result: Date) => void, onFail?: (exception: string, reason: string) => void): void;
    test(onSuccess?: (result: number) => void, onFail?: (exception: string, reason: string) => void): void;
    loadComplexObject(onSuccess?: (result: Car) => void, onFail?: (exception: string, reason: string) => void): void;
}

export class PersonsClient implements IPersonsClient {
    baseUrl: string = undefined;
    beforeSend: any = undefined;

    constructor(baseUrl?: string) {
        this.baseUrl = baseUrl !== undefined ? baseUrl : "";
    }

    xyz(data: any, onSuccess?: (result: string) => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/api/Person/xyz/{data}?";

        if (data === undefined || data === null)
            throw new Error("The parameter 'data' must be defined.");
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
        }).done((data, textStatus, xhr) => {
            this.processXyz(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processXyz(xhr, onSuccess, onFail);
        });
    }

    private processXyz(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: string = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = data === "" ? null : <string>jQuery.parseJSON(
                        data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    getAll(onSuccess?: (result: Person[]) => void, onFail?: (exception: string, reason: string) => void) {
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
        }).done((data, textStatus, xhr) => {
            this.processGetAll(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processGetAll(xhr, onSuccess, onFail);
        });
    }

    private processGetAll(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: Person[] = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = data === "" ? null : <Person[]>jQuery.parseJSON(
                        data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    /**
     * Gets a person.
     * @id The ID of the person.
     * @return The person.
     */
    get(id: number, onSuccess?: (result: Person) => void, onFail?: (exception: PersonNotFoundException | string, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Get/{id}?";

        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
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
        }).done((data, textStatus, xhr) => {
            this.processGet(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processGet(xhr, onSuccess, onFail);
        });
    }

    private processGet(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: Person = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = data === "" ? null : <Person>jQuery.parseJSON(
                        data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
            if (status === "500") {
                var result500: PersonNotFoundException = null;
                if (data !== undefined && data !== null && data !== "") {
                    try {
                        result500 = data === "" ? null : <PersonNotFoundException>jQuery.parseJSON(
                            data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                    } catch (e) {
                        if (onFail !== undefined)
                            onFail(null, "error_parsing", e);
                        return;
                    }
                }
                if (onFail !== undefined)
                    onFail(result500, "error_exception");
                return;
            }
            else {
                if (onFail !== undefined)
                    onFail(null, "error_no_callback_for_the_received_http_status");
            }
    }

    /**
     * Creates a new person.
     * @value (optional) The person.
     */
    post(value: Person, onSuccess?: () => void, onFail?: (exception: string, reason: string) => void) {
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
        }).done((data, textStatus, xhr) => {
            this.processPost(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processPost(xhr, onSuccess, onFail);
        });
    }

    private processPost(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "204") {
            var result204: any = undefined;

            if (onSuccess !== undefined)
                onSuccess(result204);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    /**
     * Updates the existing person.
     * @id The ID.
     * @value (optional) The person.
     */
    put(id: number, value: Person, onSuccess?: () => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Put/{id}?";

        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
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
        }).done((data, textStatus, xhr) => {
            this.processPut(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processPut(xhr, onSuccess, onFail);
        });
    }

    private processPut(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "204") {
            var result204: any = undefined;

            if (onSuccess !== undefined)
                onSuccess(result204);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    delete(id: number, onSuccess?: () => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Delete/{id}?";

        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
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
        }).done((data, textStatus, xhr) => {
            this.processDelete(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processDelete(xhr, onSuccess, onFail);
        });
    }

    private processDelete(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "204") {
            var result204: any = undefined;

            if (onSuccess !== undefined)
                onSuccess(result204);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    /**
     * Calculates the sum of a, b and c.
     */
    calculate(a: number, b: number, c: number, onSuccess?: (result: number) => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/api/Person/Calculate/{a}/{b}?";

        if (a === undefined || a === null)
            throw new Error("The parameter 'a' must be defined.");
        url = url.replace("{a}", "" + a);
        if (b === undefined || b === null)
            throw new Error("The parameter 'b' must be defined.");
        url = url.replace("{b}", "" + b);

        if (c === undefined || c === null)
            throw new Error("The parameter 'c' must be defined.");
        else
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
        }).done((data, textStatus, xhr) => {
            this.processCalculate(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processCalculate(xhr, onSuccess, onFail);
        });
    }

    private processCalculate(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: number = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = data === "" ? null : <number>jQuery.parseJSON(
                        data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    addHour(time: Date, onSuccess?: (result: Date) => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/AddHour?";

        if (time === undefined || time === null)
            throw new Error("The parameter 'time' must be defined.");
        else
            url += "time=" + encodeURIComponent("" + time.toJSON()) + "&";

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
        }).done((data, textStatus, xhr) => {
            this.processAddHour(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processAddHour(xhr, onSuccess, onFail);
        });
    }

    private processAddHour(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: Date = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = new Date(data);
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    test(onSuccess?: (result: number) => void, onFail?: (exception: string, reason: string) => void) {
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
        }).done((data, textStatus, xhr) => {
            this.processTest(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processTest(xhr, onSuccess, onFail);
        });
    }

    private processTest(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: number = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = data === "" ? null : <number>jQuery.parseJSON(
                        data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }

    loadComplexObject(onSuccess?: (result: Car) => void, onFail?: (exception: string, reason: string) => void) {
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
        }).done((data, textStatus, xhr) => {
            this.processLoadComplexObject(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processLoadComplexObject(xhr, onSuccess, onFail);
        });
    }

    private processLoadComplexObject(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText;
        var status = xhr.status.toString();

        if (status === "200") {
            var result200: Car = null;
            if (data !== undefined && data !== null && data !== "") {
                try {
                    result200 = data === "" ? null : <Car>jQuery.parseJSON(
                        data.replace(/\/Date((-?\d*))\//, (a: string, b: string) => { return new Date(+b); }));
                } catch (e) {
                    if (onFail !== undefined)
                        onFail(null, "error_parsing", e);
                    return;
                }
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_the_received_http_status");
        }
    }
}

/** The DTO class for a person. */
export interface Person {
    /** Gets or sets the first name. */
    firstName?: string;
    LastName?: string;
    Birthday?: Date;
    /** Gets or sets the height in cm. */
    Height?: number;
    Cars?: Car[];
    Type?: ObjectType;
}

export interface Car {
    Name?: string;
    Driver?: Person;
    Type?: ObjectType;
}

export enum ObjectType {
    Foo = <any>"Foo",
    Bar = <any>"Bar",
}

export interface PersonNotFoundException extends Exception {
    PersonId?: number;
}

export interface Exception {
    Message?: string;
    InnerException?: Exception;
    StackTrace?: string;
    Source?: string;
}
