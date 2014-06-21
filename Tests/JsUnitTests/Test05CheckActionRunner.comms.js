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

    describe('Call runAction to start', function () {
        beforeEach(function() {
            ActionRunner.runAction(jsonData);
        });

        it('Should call various in SignalR', function () {
            //for (var i = 0; i < mockSignalRClient.callLog.length; i++) {
            //    $('.results').append('<div>' + mockSignalRClient.callLog[i] + '</div>');
            //}
            expect(mockSignalRClient.callLog).toEqual(
                ['connection.createHubProxy(ActionHub)',
                    'channel.on(Progress, function)',
                    'channel.on(Started, function)',
                    'channel.on(Stopped, function)',
                    'connection.error(function)',
                    'connection.start()',
                    'connection.start.done(function)',
                    'connection.start.fail(function)']);
        });

        it('check connection.error func is set', function () {
            expect(mockSignalRClient.errorFunc).toBeDefined();
        });

        it('Calling connection.error func should result in reportSystemError', function () {
            mockSignalRClient.errorFunc('test');
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('reportSystemError(SignalR error: test)');
        });

        it('check connection.start.fail func is set', function () {
            expect(mockSignalRClient.failFunc).toBeDefined();
        });

        it('Calling connection.start.fail func should result in reportSystemError', function () {
            mockSignalRClient.failFunc('test');
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('reportSystemError(SignalR connection error: test)');
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
            expect(mockSignalRClient.callLog[8]).toBe('channel.invoke(StartAction, abcd)');
        });


        it('check "on" functions are set', function () {
            expect(mockSignalRClient.onFunctionDict.Progress).toBeDefined();
            expect(mockSignalRClient.onFunctionDict.Started).toBeDefined();
            expect(mockSignalRClient.onFunctionDict.Stopped).toBeDefined();
        });

        describe('Check progress', function () {
            beforeEach(function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'Normal');
            });

            it('Called Progress with Info message', function () {
                var message = createMessage('Info');
                mockSignalRClient.onFunctionDict.Progress('abcd', 55, message);
                expect(ActionRunner.callLog.length).toBe(3);
                expect(ActionRunner.callLog[1]).toBe('updateProgress(55, 0)');
                expect(ActionRunner.callLog[2]).toBe('addMessageToProgressList(Info, This is a test)');
            });

            it('Called Progress with Error message', function () {
                var message = createMessage('Error');
                mockSignalRClient.onFunctionDict.Progress('abcd', 55, message);
                expect(ActionRunner.callLog.length).toBe(3);
                expect(ActionRunner.callLog[1]).toBe('updateProgress(55, 1)');
                expect(ActionRunner.callLog[2]).toBe('addMessageToProgressList(Error, This is a test)');
            });
        });

        describe('Call Started with configFlags', function () {

            it('normal state should be cancel', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'Normal');
                expect(ActionRunner.getActionState()).toBe('Cancel');
            });

            it('if CancelNotSupported then state should be running...', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'CancelNotSupported');
                expect(ActionRunner.getActionState()).toBe('Running...');
            });

            it('Should call createActionPanel in ui', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'Normal');
                expect(ActionRunner.callLog.length).toBe(1);
                expect(ActionRunner.callLog[0]).toBe('createActionPanel([object Object])');
            });

        });

        describe('Call connection.on( Stopped)', function () {

            it('messagetype finished should set state', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'Normal');
                var message = {
                    MessageTypeString: 'Finished',
                    message: 'we have finished'
                };
                mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                expect(ActionRunner.getActionState()).toBe('Finished Ok');
            });

            it('messagetype finished with normal start should not removePanel', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'Normal');
                var message = {
                    MessageTypeString: 'Finished',
                    message: 'we have finished'
                };
                mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                expect(ActionRunner.callLog).toEqual(
                ['createActionPanel([object Object])',
                 'updateProgress(100)']);
            });

            it('messagetype finished with ExitOnSuccess start should close connection', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'ExitOnSuccess');
                var message = {
                    MessageTypeString: 'Finished',
                    message: 'we have finished'
                };
                mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                //mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                //for (var i = 0; i < mockSignalRClient.callLog.length; i++) {
                //    $('.results').append('<div>' + mockSignalRClient.callLog[i] + '</div>');
                //}
                expect(mockSignalRClient.callLog.length).toBe(10);
                expect(mockSignalRClient.callLog[9]).toBe('connection.stop()');
            });

            it('messagetype finished with ExitOnSuccess start should remove Panel', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'ExitOnSuccess');
                var message = {
                    MessageTypeString: 'Finished',
                    message: 'we have finished'
                };
                mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                expect(ActionRunner.callLog.length).toBe(3);
                expect(ActionRunner.callLog[2]).toBe('removeActionPanel(true)');
            });

            it('messagetype cancelled should set state', function () {
                var message = {
                    MessageTypeString: 'Cancelled',
                    message: 'we have finished'
                };
                mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                expect(ActionRunner.getActionState()).toBe('Cancelled');
            });

            it('messagetype failed should set state', function () {
                mockSignalRClient.onFunctionDict.Started('abcd', 'Normal');
                var message = {
                    MessageTypeString: 'Failed',
                    message: 'we have finished'
                };
                mockSignalRClient.onFunctionDict.Stopped('abcd', message);
                $('.results').append('<div>' + ActionRunner.getActionState() + '</div>');
                expect(ActionRunner.getActionState()).toBe('Failed');
            });
        });

        describe('Call respondToStateChangeRequest with Cancel', function () {

            it('Called respondToStateChangeRequest should call invoke(Cancel', function () {
                ActionRunner.respondToStateChangeRequest('Cancel');
                expect(mockSignalRClient.callLog.length).toBe(9);
                expect(mockSignalRClient.callLog[8]).toBe('channel.invoke(CancelAction, abcd)');
            });

            it('Called respondToStateChangeRequest should move to cancelling...', function () {
                ActionRunner.respondToStateChangeRequest('Cancel');
                expect(ActionRunner.getActionState()).toBe('Cancelling...');
            });

        });

        describe('Call respondToStateChangeRequest with various finishes', function () {

            it('Called respondToStateChangeRequest with Finished Ok should remove the panel', function () {
                ActionRunner.respondToStateChangeRequest('Finished Ok');
                expect(ActionRunner.callLog.length).toBe(1);
                expect(ActionRunner.callLog[0]).toBe('removeActionPanel(true)');
            });

            it('Called respondToStateChangeRequest with Cancelled should remove the panel', function () {
                ActionRunner.respondToStateChangeRequest('Cancelled');
                expect(ActionRunner.callLog.length).toBe(1);
                expect(ActionRunner.callLog[0]).toBe('removeActionPanel(false)');
            });

            it('Called respondToStateChangeRequest with Failed xxx should remove the panel', function () {
                ActionRunner.respondToStateChangeRequest('Failed xxx');
                expect(ActionRunner.callLog.length).toBe(1);
                expect(ActionRunner.callLog[0]).toBe('removeActionPanel(false)');
            });

        });

    });

});
