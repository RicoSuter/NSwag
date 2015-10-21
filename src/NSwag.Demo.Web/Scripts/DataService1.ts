// Generated using the NSwag toolchain v0.12.5772.33331 (http://NSwag.org)

export interface SwaggerException {
    ExceptionType?: string;
    Message?: string;
    StackTrace?: string;
}

export interface Person {
    firstName?: string;
    LastName?: string;
}

export interface Car {
    Name?: string;
    Driver?: Person;
}

export interface IDataService {
    /**
     */
    getAll(onSuccess?: (result: Person[]) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    get(id: number, onSuccess?: (result: Person) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    post(request: Person, onSuccess?: (result: any) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    put(id: number, request: Person, onSuccess?: (result: any) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    delete(id: number, onSuccess?: (result: any) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    calculate(a: number, b: number, c: number, onSuccess?: (result: number) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    addHour(time: Date, onSuccess?: (result: Date) => void, onFail?: (exception: any, reason: string) => void);

    /**
     */
    loadComplexObject(onSuccess?: (result: Car) => void, onFail?: (exception: any, reason: string) => void);

}

export class DataService implements IDataService {
    baseUrl = ""; 
    beforeSend: any = undefined; 

    /**
     */
    getAll(onSuccess?: (result: Person[]) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Get?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <Person[]>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    get(id: number, onSuccess?: (result: Person) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Get/{id}?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <Person>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    post(request: Person, onSuccess?: (result: any) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Post?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <any>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    put(id: number, request: Person, onSuccess?: (result: any) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Put/{id}?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <any>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    delete(id: number, onSuccess?: (result: any) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/Delete/{id}?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <any>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    calculate(a: number, b: number, c: number, onSuccess?: (result: number) => void, onFail?: (exception: any, reason: string) => void) {
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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <number>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    addHour(time: Date, onSuccess?: (result: Date) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/AddHour?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = new Date(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

    /**
     */
    loadComplexObject(onSuccess?: (result: Car) => void, onFail?: (exception: any, reason: string) => void) {
        var url = this.baseUrl + "/api/Persons/LoadComplexObject?"; 

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
        var status = xhr.status; 

        if (status === 200) {
            try { 
                var result = <Car>jQuery.parseJSON(data);
                if (onSuccess !== undefined)
                    onSuccess(result);
            } catch(e) { 
                if (onFail !== undefined)
                    onFail(null, "error_parsing");
            }
        }
        else
        {
            if (onFail !== undefined)
                onFail(null, "error_no_callback_for_status");
        }
    }

}

