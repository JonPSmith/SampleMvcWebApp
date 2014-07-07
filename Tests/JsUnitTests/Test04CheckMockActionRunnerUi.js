/// <reference path="../MocksAndFakes/mockActionRunnerUi.js" />

//Test suite
describe('Test04 - check mock ActionRunnerUi.ui', function () {
    beforeEach(function () {
        ActionRunnerUi.reset();
    });

    it('that callLog exists', function() {
        expect(ActionRunnerUi.callLog).toBeDefined();
        expect(ActionRunnerUi.callLog.length).toBe(0);
    });

    it('that callLog works with named function', function () {
        function test(str, val) {
            ActionRunnerUi.logStep('test');
        };
        test('xxx', 456);
        expect(ActionRunnerUi.callLog.length).toBe(1);
        expect(ActionRunnerUi.callLog[0]).toBe('test(xxx, 456)');
    });

    describe('check actionState works', function () {
        beforeEach(function () {
            ActionRunnerUi.actionState = "Cancel";
        });

        it('mock getActionState exists', function () {
            expect(ActionRunnerUi.getActionState).toBeDefined();
        });

        it('mock setActionState exists', function () {
            expect(ActionRunnerUi.getActionState).toBeDefined();
        });

        it('mock getActionState gets value', function () {
            expect(ActionRunnerUi.getActionState()).toBe('Cancel');
        });

        it('mock setActionState sets value', function () {
            ActionRunnerUi.setActionState('Finished');
            expect(ActionRunnerUi.actionState).toBe('Finished');
            expect(ActionRunnerUi.getActionState()).toBe('Finished');
        });

    });

    describe('check other ui methods exist', function () {

        it('mock startActionUi exists', function () {
            expect(ActionRunnerUi.startActionUi).toBeDefined();
        });

        it('mock endActionUi exists', function () {
            expect(ActionRunnerUi.endActionUi).toBeDefined();
        });

        it('mock addMessageToProgressList exists', function () {
            expect(ActionRunnerUi.addMessageToProgressList).toBeDefined();
        });

        it('mock updateProgress exists', function () {
            expect(ActionRunnerUi.updateProgress).toBeDefined();
        });

        it('mock displayGlobalMessage exists', function () {
            expect(ActionRunnerUi.displayGlobalMessage).toBeDefined();
        });

        it('mock reportSystemError exists', function () {
            expect(ActionRunnerUi.reportSystemError).toBeDefined();
        });

        it('mock confirmDialog exists', function () {
            expect(ActionRunnerUi.confirmDialog).toBeDefined();
        });

    });

    describe('check other ui methods are logged', function () {

        it('mock startActionUi exists', function () {
            ActionRunnerUi.startActionUi();
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('startActionUi()');
        });

        it('mock endActionUi exists', function () {
            ActionRunnerUi.endActionUi(true);
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('endActionUi(true)');
        });

        it('mock addMessageToProgressList exists', function () {
            ActionRunnerUi.addMessageToProgressList('info', 'Hello world');
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('addMessageToProgressList(info, Hello world)');
        });

        it('mock updateProgress exists', function () {
            ActionRunnerUi.updateProgress('abcd', 55, 0);
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('updateProgress(abcd, 55, 0)');
        });

        it('mock displayGlobalMessage exists', function () {
            ActionRunnerUi.displayGlobalMessage('Hello world', true, 'Error');
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('displayGlobalMessage(Hello world, true, Error)');
        });

        it('mock reportSystemError exists', function () {
            ActionRunnerUi.reportSystemError('This is a test', false);
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('reportSystemError(This is a test, false)');
        });

        it('mock confirmDialog exists', function () {
            ActionRunnerUi.confirmDialog('This is a test');
            expect(ActionRunnerUi.callLog.length).toBe(1);
            expect(ActionRunnerUi.callLog[0]).toBe('confirmDialog(This is a test)');
        });

    });
});
