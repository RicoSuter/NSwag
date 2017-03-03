export class MyBaseClass {
    protected transformOptions(options: any) {
        return options;
    }

    protected transformResult(_: string, response: any, processor: (response: any) => any) {
        return processor(response);
    }
}