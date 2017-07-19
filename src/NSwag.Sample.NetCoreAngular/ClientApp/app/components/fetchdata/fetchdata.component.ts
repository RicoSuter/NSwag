import { Component } from '@angular/core';
import { SampleDataService, WeatherForecast } from '../../services';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public forecasts: WeatherForecast[];

    constructor(sampleDataService: SampleDataService) {
        sampleDataService.weatherForecasts().toPromise().then(result => {
            this.forecasts = result;
        });
    }
}