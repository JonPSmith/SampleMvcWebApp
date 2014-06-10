/// <reference path="../../SampleWebApp/Scripts/jquery-1.10.2.js" />
/// <reference path="../MocksAndFakes/mockSignalRClient.js" />
/// <reference path="../MocksAndFakes/mockActionRunnerUi.js" />
/// <reference path="../../SampleWebApp/Scripts/ActionRunner.comms.js" />
/// <reference path="../TestData/actionRunnerData - default.js" />

//Test suite
describe('Test05 - check ActionRunner.comms', function () {

    function createMessage(messageType, message) {
        message = message || 'This is a test';
        return {
            MessageTypeString: messageType,
            MessageText: message
        };
    };

    beforeEach(function () {
        mockSignalRClient.reset();
        ActionRunner.reset();
    });

    it('that ActionRunner has comms part in it', function () {
        expect(ActionRunner.runAction).toBeDefined();
        expect(ActionRunner.respondToStateChangeRequest).toBeDefined();
    });

    it('that ActionRunner has mock ui part', function () {
        expect(ActionRunner.getActionState).toBeDefined();
    });

    it('that callLog works', function () {
        ActionRunner.logStep('Hello world');
        expect(ActionRunner.callLog.length).toBe(1);
        expect(ActionRunner.callLog[0]).toBe('Hello world');
    });

    describe('Call runAction to start', function () {
        beforeEach(function() {
            ActionRunner.runAction(jsonData);
        });

        it('Should call createActionPanel in ui', function () {
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('createActionPanel');
        });

        it('Should call various in SignalR', function () {
            //for (var i = 0; i < mockSignalRClient.callLog.length; i++) {
            //    $('.results').append('<div>' + mockSignalRClient.callLog[i] + '</div>');
            //}
            expect(mockSignalRClient.callLog).toEqual(
                ['connection.createHubProxy(ActionHub',
                'channel.on(Progress',
                'channel.on(Started', 
                'channel.on(Stopped',
                'connection.error',
                'connection.start',
                'connection.start.done',
                'connection.start.fail']);
        });

        it('check connection.error func is set', function () {
            expect(mockSignalRClient.errorFunc).toBeDefined();
        });

        it('Calling connection.error func should result in reportSystemError', function () {
            mockSignalRClient.errorFunc('test');
            expect(ActionRunner.callLog.length).toBe(2);
            expect(ActionRunner.callLog[1]).toBe('reportSystemError(SignalR error: test, ');
        });

        it('check connection.start.fail func is set', function () {
            expect(mockSignalRClient.failFunc).toBeDefined();
        });

        it('Calling connection.start.fail func should result in reportSystemError', function () {
            mockSignalRClient.failFunc('test');
            expect(ActionRunner.callLog.length).toBe(2);
            expect(ActionRunner.callLog[1]).toBe('reportSystemError(SignalR connection error: test, ');
        });

        it('Calling connection.start.fail func should set actionState', function () {
            mockSignalRClient.failFunc('test');
            expect(ActionRunner.getActionState()).toBe('Failed (connecting)');
        });

        it('check connection.start.done func is set', function () {
            expect(mockSignalRClient.doneFunc).toBeDefined();
        });

        it('Calling connection.start.done func should set actionState', function () {
            mockSignalRClient.doneFunc();
            expect(ActionRunner.getActionState()).toBe('Starting...');
        });

        it('Calling connection.start.done func should call invoke StartAction', function () {
            mockSignalRClient.doneFunc();
            //for (var i = 0; i < mockSignalRClient.callLog.length; i++) {
            //    $('.results').append('<div>' + mockSignalRClient.callLog[i] + '</div>');
            //}
            expect(mockSignalRClient.callLog.length).toBe(9);
            expect(mockSignalRClient.callLog[8]).toBe('channel.invoke(StartAction');
        });


        it('check "on" functions are set', function () {
            expect(mockSignalRClient.onFunctionDict.Progress).toBeDefined();
            expect(mockSignalRClient.onFunctionDict.Started).toBeDefined();
            expect(mockSignalRClient.onFunctionDict.Stopped).toBeDefined();
        });

        describe('Check progress', function () {
            beforeEach(function () {
                mockSignalRClient.onFunctionDict.Started('123');
            });

            it('Called Progress with Info message', function () {
                var message = createMessage('Info');
                mockSignalRClient.onFunctionDict.Progress('123', 55, message);
                expect(ActionRunner.callLog.length).toBe(3);
                expect(ActionRunner.callLog[1]).toBe('updateProgress(123, 55, 0');
                expect(ActionRunner.callLog[2]).toBe('addMessageToProgressList(123, Info, This is a test');
            });

            it('Called Progress with Error message', function () {
                var message = createMessage('Error');
                mockSignalRClient.onFunctionDict.Progress('123', 55, message);
                expect(ActionRunner.callLog.length).toBe(3);
                expect(ActionRunner.callLog[1]).toBe('updateProgress(123, 55, 1');
                expect(ActionRunner.callLog[2]).toBe('addMessageToProgressList(123, Error, This is a test');
            });

        });

        it('Call connection.on( Started) should set state', function () {
            mockSignalRClient.onFunctionDict.Started('123');
            expect(ActionRunner.getActionState()).toBe('Cancel');
        });

        it('Call connection.on( Stopped) with messagetype finished should set state', function () {
            var message = {
                MessageTypeString: 'Finished',
                message: 'we have finished'
            };
            mockSignalRClient.onFunctionDict.Stopped('123', message );
            expect(ActionRunner.getActionState()).toBe('Finished Ok');
        });

        it('Call connection.on( Stopped) with messagetype cancelled should set state', function () {
            var message = {
                MessageTypeString: 'Cancelled',
                message: 'we have finished'
            };
            mockSignalRClient.onFunctionDict.Stopped('123', message);
            expect(ActionRunner.getActionState()).toBe('Cancelled');
        });

        it('Call connection.on( Stopped) with messagetype failed should set state', function () {
            var message = {
                MessageTypeString: 'Failed',
                message: 'we have finished'
            };
            mockSignalRClient.onFunctionDict.Stopped('123', message);
            $('.results').append('<div>' + ActionRunner.getActionState() + '</div>');
            expect(ActionRunner.getActionState()).toBe('Failed');
        });

        describe('Call respondToStateChangeRequest with Cancel', function () {

            it('Called respondToStateChangeRequest should call invoke(Cancel', function () {
                ActionRunner.respondToStateChangeRequest('Cancel');
                expect(mockSignalRClient.callLog.length).toBe(9);
                expect(mockSignalRClient.callLog[8]).toBe('channel.invoke(CancelAction');
            });

            it('Called respondToStateChangeRequest should move to cancelling...', function () {
                ActionRunner.respondToStateChangeRequest('Cancel');
                expect(ActionRunner.getActionState()).toBe('Cancelling...');
            });

        });

        describe('Call respondToStateChangeRequest with various finishes', function () {

            it('Called respondToStateChangeRequest with Finished Ok should remove the panel', function () {
                ActionRunner.respondToStateChangeRequest('Finished Ok');
                expect(ActionRunner.callLog.length).toBe(2);
                expect(ActionRunner.callLog[1]).toBe('removeActionPanel');
            });

            it('Called respondToStateChangeRequest with Cancelled should remove the panel', function () {
                ActionRunner.respondToStateChangeRequest('Cancelled');
                expect(ActionRunner.callLog.length).toBe(2);
                expect(ActionRunner.callLog[1]).toBe('removeActionPanel');
            });

            it('Called respondToStateChangeRequest with Failed xxx should remove the panel', function () {
                ActionRunner.respondToStateChangeRequest('Failed xxx');
                expect(ActionRunner.callLog.length).toBe(2);
                expect(ActionRunner.callLog[1]).toBe('removeActionPanel');
            });

        });

    });

});
