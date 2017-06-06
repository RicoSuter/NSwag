import { HttpClient } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { SampleDataClient, WeatherForecast } from "../../../clients";

@inject(SampleDataClient)
export class Fetchdata {
    public forecasts: WeatherForecast[];

    constructor(sampleDataClient: SampleDataClient) {
        sampleDataClient.weatherForecasts().then(result => {
            this.forecasts = result;
        });
    }
}