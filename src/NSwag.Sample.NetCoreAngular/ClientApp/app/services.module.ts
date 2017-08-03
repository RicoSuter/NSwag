import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { SampleDataService, EnumerationService, FileService } from './services';

@NgModule({
    imports: [
        HttpModule
    ],
    providers: [
        SampleDataService,
        EnumerationService,
        FileService
    ]
})
export class ServicesModule {
}