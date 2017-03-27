import * as generated from "./serviceClientsFetch";

class GeoClient extends generated.GeoClient {
    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super(baseUrl, http);

        //this.jsonParseReviver = (key: string, value: any) => value;
    }
}