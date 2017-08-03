import { Observable } from 'rxjs/Observable'; // ignore
import { Response, RequestOptionsArgs } from '@angular/http'; // ignore

export class ServiceBase {
    protected transformOptions(options: RequestOptionsArgs) {
        return Promise.resolve(options);
    }

    protected transformResult(url: string, response: Response, processor: (response: Response) => any): Observable<any> {
        return processor(response);
    }
}