import { Observable } from 'rxjs/Observable'; // ignore
import { HttpResponse, HttpHeaders, HttpParams } from '@angular/common/http'; // ignore

export class ServiceBase {
    protected transformOptions(options: any) {
        return Promise.resolve(options);
    }

    protected transformResult(url: string, response: HttpResponse<Blob>, processor: (response: HttpResponse<Blob>) => any): Observable<any> {
        return processor(response);
    }
}