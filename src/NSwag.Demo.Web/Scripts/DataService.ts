// Generated using the NSwag toolchain v0.19.5784.33760 (http://NSwag.org)

/** The DTO class for a person. */
export interface Person {
    /** Gets or sets the first name. */
    firstName?: string;
    LastName?: string;
    Cars?: Car[];
}

export interface Car {
    Name?: string;
    Driver?: Person;
}

export interface PersonNotFoundException extends Exception {
    PersonId: number;
}

export interface Exception {
    Message?: string;
    InnerException?: Exception;
    StackTrace?: string;
    Source?: string;
}

export interface IDataService {
    getAll(onSuccess?: (result: Person[]) => void, onFail?: (exception: string, reason: string) => void);

    /**
     * Gets a person.
     * @id The ID of the person.
     * @return The person.
     */
    get(id: number, onSuccess?: (result: Person) => void, onFail?: (exception: PersonNotFoundException | string, reason: string) => void);

    /**
     * Creates a new person.
     * @request The person.
     */
    post(request: Person, onSuccess?: (result: any) => void, onFail?: (exception: string, reason: string) => void);

    /**
     * Updates the existing person.
     * @id The ID.
     * @request The person.
     */
    put(id: number, request: Person, onSuccess?: (result: any) => void, onFail?: (exception: string, reason: string) => void);

    delete(id: number, onSuccess?: (result: any) => void, onFail?: (exception: string, reason: string) => void);

    /**
     * Calculates the sum of a, b and c.
     */
    calculate(a: number, b: number, c: number, onSuccess?: (result: number) => void, onFail?: (exception: string, reason: string) => void);

    addHour(time: Date, onSuccess?: (result: Date) => void, onFail?: (exception: string, reason: string) => void);

    loadComplexObject(onSuccess?: (result: Car) => void, onFail?: (exception: string, reason: string) => void);

}

export class DataService implements IDataService {
    baseUrl = ""; 
    beforeSend: any = undefined; 

    getAll(onSuccess?: (result: Person[]) => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Get?"; 

        var content = "";

        $.ajax({
            url: url,
            beforeSend: this.beforeSend,
            type: "get",
            data: content,
            dataType: "text",
            contentType: "application/json; charset=UTF-8"
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
            var result200 = null; 
            try { 
                result200 = <Person[]>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     * Gets a person.
     * @id The ID of the person.
     * @return The person.
     */
    get(id: number, onSuccess?: (result: Person) => void, onFail?: (exception: PersonNotFoundException | string, reason: string) => void) {
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
            var result200 = null; 
            try { 
                result200 = <Person>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        if (status === "500") {
            var result500 = null; 
            try { 
                result500 = <PersonNotFoundException>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onFail !== undefined)
                onFail(result500);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     * Creates a new person.
     * @request The person.
     */
    post(request: Person, onSuccess?: (result: any) => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/MyWorldCalculators/api/Persons/Post?"; 

        var content = JSON.stringify(request);
        $.ajax({
            url: url,
            beforeSend: this.beforeSend,
            type: "post",
            data: content,
            dataType: "text",
            contentType: "application/json; charset=UTF-8"
        }).done((data, textStatus, xhr) => {
            this.processPost(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processPost(xhr, onSuccess, onFail);
        });
    }

    private processPost(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText; 
        var status = xhr.status.toString(); 

        if (status === "200") {
            var result200 = null; 
            try { 
                result200 = <any>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     * Updates the existing person.
     * @id The ID.
     * @request The person.
     */
    put(id: number, request: Person, onSuccess?: (result: any) => void, onFail?: (exception: string, reason: string) => void) {
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
        }).done((data, textStatus, xhr) => {
            this.processPut(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processPut(xhr, onSuccess, onFail);
        });
    }

    private processPut(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText; 
        var status = xhr.status.toString(); 

        if (status === "200") {
            var result200 = null; 
            try { 
                result200 = <any>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    delete(id: number, onSuccess?: (result: any) => void, onFail?: (exception: string, reason: string) => void) {
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
        }).done((data, textStatus, xhr) => {
            this.processDelete(xhr, onSuccess, onFail);
        }).fail((xhr) => {
            this.processDelete(xhr, onSuccess, onFail);
        });
    }

    private processDelete(xhr: any, onSuccess?: any, onFail?: any) {
        var data = xhr.responseText; 
        var status = xhr.status.toString(); 

        if (status === "200") {
            var result200 = null; 
            try { 
                result200 = <any>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     * Calculates the sum of a, b and c.
     */
    calculate(a: number, b: number, c: number, onSuccess?: (result: number) => void, onFail?: (exception: string, reason: string) => void) {
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
            var result200 = null; 
            try { 
                result200 = <number>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    addHour(time: Date, onSuccess?: (result: Date) => void, onFail?: (exception: string, reason: string) => void) {
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
            var result200 = null; 
            try { 
                result200 = new Date(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    loadComplexObject(onSuccess?: (result: Car) => void, onFail?: (exception: string, reason: string) => void) {
        var url = this.baseUrl + "/MyWorldCalculators/api/Persons/LoadComplexObject?"; 

        var content = "";

        $.ajax({
            url: url,
            beforeSend: this.beforeSend,
            type: "get",
            data: content,
            dataType: "text",
            contentType: "application/json; charset=UTF-8"
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
            var result200 = null; 
            try { 
                result200 = <Car>jQuery.parseJSON(data);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing", e);
                return;
            }
            if (onSuccess !== undefined)
                onSuccess(result200);
            return;
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

}

