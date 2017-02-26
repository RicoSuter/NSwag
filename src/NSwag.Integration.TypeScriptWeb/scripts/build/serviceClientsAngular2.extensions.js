class MyBaseClass {
    transformOptions(options) {
        return options;
    }
    transformResult(url, response, processor) {
        return processor(response);
    }
}
exports.MyBaseClass = MyBaseClass;
//# sourceMappingURL=serviceClientsAngular2.extensions.js.map