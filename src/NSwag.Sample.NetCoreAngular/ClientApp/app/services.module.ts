import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { SampleDataService, EnumerationService, FileService, DateService } from './services';

@NgModule({
    imports: [
        HttpModule
    ],
    providers: [
        SampleDataService,
        DateService,
        EnumerationService,
        FileService
    ]
})
export class ServicesModule {
}