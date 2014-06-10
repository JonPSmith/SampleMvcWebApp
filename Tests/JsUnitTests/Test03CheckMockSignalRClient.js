/// <reference path="../../SampleWebApp/Scripts/jquery-1.10.2.js" />
/// <reference path="../MocksAndFakes/mockSignalRClient.js" />

//Test suite
describe('Test03 - check mockSignalRClient', function () {
    beforeEach(function () {
        mockSignalRClient.reset();
    });

    it('mock appears in jQuery', function () {
        expect($.hubConnection).toBeDefined();
    });

    it('that callLog exists', function () {
        expect(mockSignalRClient.callLog).toBeDefined();
        expect(mockSignalRClient.callLog.length).toBe(0);
    });

    it('that callLog works', function () {
        mockSignalRClient.logStep('Hello world');
        expect(mockSignalRClient.callLog.length).toBe(1);
        expect(mockSignalRClient.callLog[0]).toBe('Hello world');
    });

    describe('with connection', function() {
        beforeEach(function() {
            this.connection = $.hubConnection();
        });
        afterEach(function() {
            this.connection = null;
        });

        it('mock connection exists', function() {
            expect(this.connection).toBeDefined();
        });

        it('check error exists', function () {
            expect(this.connection.error).toBeDefined();
        });

        it('check error records func', function () {
            var i = 0;
            function myFunction() { i++; }
            this.connection.error(myFunction);
            expect(mockSignalRClient.errorFunc).toBe(myFunction);
        });

        it('check start exists', function () {
            expect(this.connection.start).toBeDefined();
        });

        describe('check start', function() {
            it('check start().done() exists', function () {
                expect(this.connection.start().done).toBeDefined();
            });

            it('check start().fail() exists', function () {
                expect(this.connection.start().fail).toBeDefined();
            });

            it('check start().done() records func', function () {
                var i = 0;
                function myFunction() { i++; }
                this.connection.start().done(myFunction);
                expect(mockSignalRClient.doneFunc).toBe(myFunction);
            });

            it('check start().fail() records func', function () {
                var i = 0;
                function myFunction() { i++; }
                this.connection.start().fail(myFunction);
                expect(mockSignalRClient.failFunc).toBe(myFunction);
            });

            it('check start().done().fail() chaining works', function () {
                var i = 0;
                function myFunction() { i++; }
                expect(this.connection.start().done(myFunction).fail).toBeDefined();
            });

        });

        it('check createProxy exists', function() {
            expect(this.connection.createHubProxy).toBeDefined();
        });

        it('check createHubProxy calling works', function() {
            var channel = this.connection.createHubProxy('ActionHub');
            expect(channel).toBeDefined();
        });

        describe('with channel', function() {
            beforeEach(function() {
                this.channel = this.connection.createHubProxy('ActionHub');
            });
            afterEach(function() {
                this.channel = null;
            });

            it('that channel create was logged', function () {
                expect(mockSignalRClient.callLog.length).toBe(1);
                expect(mockSignalRClient.callLog[0]).toBe('connection.createHubProxy(ActionHub');
            });

            it('that channel on method exists', function() {
                expect(this.channel.on).toBeDefined();
            });

            it('that channel invoke method exists', function () {
                expect(this.channel.invoke).toBeDefined();
            });

            it('that channel invoke method is logged', function () {
                this.channel.invoke('An action', 'aaa');
                expect(mockSignalRClient.callLog.length).toBe(2);
                expect(mockSignalRClient.callLog[1]).toBe('channel.invoke(An action');
            });

            it('that channel on method is logged', function () {
                this.channel.on( 'xxx', function() {});
                expect(mockSignalRClient.callLog.length).toBe(2);
                expect(mockSignalRClient.callLog[1]).toBe('channel.on(xxx');
            });

            it('that channel on method adds to dict', function () {
                this.channel.on('xxx', function () { });
                expect(mockSignalRClient.onFunctionDict.xxx).toBeDefined();
            });

            it('that channel on method can be called', function () {
                var i = 0;
                function myFunction() { i++; return 123; }
                this.channel.on('xxx', myFunction);
                expect(mockSignalRClient.onFunctionDict.xxx()).toBe(123);
                expect(i).toBe(1);
            });
        });
    });
});
