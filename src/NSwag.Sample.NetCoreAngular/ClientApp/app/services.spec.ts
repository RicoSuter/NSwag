import { } from 'jasmine';
import { TestBed } from '@angular/core/testing';
import { HttpModule } from "@angular/http";
import { async, inject } from '@angular/core/testing';

import { SampleDataService, API_BASE_URL, FileType, FileService, EnumerationService } from '../app/services';

describe('SampleDataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpModule],
      providers: [
        SampleDataService,
        FileService,
        EnumerationService,
        { provide: API_BASE_URL, useValue: "http://localhost:5000" },
      ]
    });
    TestBed.compileComponents();
  });

  it('getFile', async(inject([FileService],
    // arrange
    async (service: FileService) => {

      // act
      let response = await service.getFile("myfile.xlsx").toPromise();

      // assert
      expect(response.fileName).toBe('myfile.xlsx');
      expect(response.data.size).toBe(3);
    })));

  it('getFile with filename spaces', async(inject([FileService],
    // arrange
    async (service: FileService) => {

      // act
      let response = await service.getFile("my file.xlsx").toPromise();

      // assert
      expect(response.fileName).toBe('my file.xlsx');
      expect(response.headers['content-disposition']).toBeDefined();
      expect(response.data.size).toBe(3);
    })));

  it('weatherForecasts to return a list of items', async(inject([SampleDataService],
    // arrange
    async (service: SampleDataService) => {

      // act
      let response = await service.weatherForecasts().toPromise();

      // assert
      expect(response.length).toBeGreaterThan(2);
    })));

  it('getRoles', async(inject([SampleDataService],
    // arrange
    async (service: SampleDataService) => {

      // act
      let response = await service.getRoles(null, null).toPromise();

      // assert
      expect(response.length).toBe(2);
    })));

  it('ReverseQueryEnumList', async(inject([EnumerationService],
    // arrange
    async (service: EnumerationService) => {

      // act
      let response = await service.reverseQueryEnumList([FileType.Audio, FileType.Document]).toPromise();

      // assert
      expect(response.length).toBe(2);
      expect(response[0]).toBe(FileType.Document);
      expect(response[1]).toBe(FileType.Audio);
    })));
});