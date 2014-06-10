/// <reference path="../../SampleWebApp/Scripts/jquery-1.10.2.js" />

//Test suite
var outerVariable = 456;
describe('Test01 - does jasmine work', function () {
    var i;
    beforeEach(function () {
        i = 10;
    });
    afterEach(function () {
        i = 0;
    });

    it('BeforeEach works', function () {
        expect(i).toBe(10);
    });

    it('jQuery found from SampleWebApp', function () {
        expect(window.jQuery).toBeDefined();
        expect($).toBeDefined();
    });

    it('try output', function () {
        $('.results').append('<div>Hello from a test. The value of i is ' + i + '</div>');
    });

    it('try pause', function () {
        var localVal = 1234;
        //jasmine.getEnv().currentRunner_.finishCallback = function () { };  //1.3 version
        //ReSharperReporter.prototype.jasmineDone = function () { };      //2.0 version
    });

});
