import generated = require("serviceClientsFetch");

class GeoClient extends generated.GeoClientBase {
    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super(baseUrl, http);

        //this.jsonParseReviver = (key: string, value: any) => value;
    }
}