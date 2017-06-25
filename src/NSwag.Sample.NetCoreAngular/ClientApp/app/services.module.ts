import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { SampleDataService } from './services';

@NgModule({
    imports: [
        HttpModule
    ],
    providers: [
        SampleDataService
    ]
})
export class ServicesModule {
}