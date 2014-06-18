/// <reference path="../MocksAndFakes/mockActionRunnerUi.js" />

//Test suite
describe('Test04 - check mock ActionRunner.ui', function () {
    beforeEach(function () {
        ActionRunner.reset();
    });

    it('that callLog exists', function() {
        expect(ActionRunner.callLog).toBeDefined();
        expect(ActionRunner.callLog.length).toBe(0);
    });

    it('that callLog works with named function', function () {
        function test(str, val) {
            ActionRunner.logStep();
        };
        test('xxx', 456);
        expect(ActionRunner.callLog.length).toBe(1);
        expect(ActionRunner.callLog[0]).toBe('test(xxx, 456)');
    });

    it('that callLog works with anonymous function', function () {
        var test = function (str, val) {
            ActionRunner.logStep('anonymousFunc');
        };
        test('xxx', 456);
        expect(ActionRunner.callLog.length).toBe(1);
        expect(ActionRunner.callLog[0]).toBe('anonymousFunc(xxx, 456)');
    });

    it('that callLog works with function as argument', function () {
        function test(func) {
            ActionRunner.logStep();
        };
        test(function () { return 'xxx'; });
        expect(ActionRunner.callLog.length).toBe(1);
        expect(ActionRunner.callLog[0]).toBe('test(function)');
    });

    describe('check actionState works', function () {
        beforeEach(function () {
            ActionRunner.actionState = "Cancel";
        });

        it('mock getActionState exists', function () {
            expect(ActionRunner.getActionState).toBeDefined();
        });

        it('mock setActionState exists', function () {
            expect(ActionRunner.getActionState).toBeDefined();
        });

        it('mock getActionState gets value', function () {
            expect(ActionRunner.getActionState()).toBe('Cancel');
        });

        it('mock setActionState sets value', function () {
            ActionRunner.setActionState('Finished');
            expect(ActionRunner.actionState).toBe('Finished');
            expect(ActionRunner.getActionState()).toBe('Finished');
        });

    });

    describe('check other ui methods exist', function () {

        it('mock createActionPanel exists', function () {
            expect(ActionRunner.createActionPanel).toBeDefined();
        });

        it('mock removeActionPanel exists', function () {
            expect(ActionRunner.removeActionPanel).toBeDefined();
        });

        it('mock addMessageToProgressList exists', function () {
            expect(ActionRunner.addMessageToProgressList).toBeDefined();
        });

        it('mock updateProgress exists', function () {
            expect(ActionRunner.updateProgress).toBeDefined();
        });

        it('mock displayGlobalMessage exists', function () {
            expect(ActionRunner.displayGlobalMessage).toBeDefined();
        });

        it('mock reportSystemError exists', function () {
            expect(ActionRunner.reportSystemError).toBeDefined();
        });

    });

    describe('check other ui methods are logged', function () {

        it('mock createActionPanel exists', function () {
            ActionRunner.createActionPanel();
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('createActionPanel()');
        });

        it('mock removeActionPanel exists', function () {
            ActionRunner.removeActionPanel();
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('removeActionPanel()');
        });

        it('mock addMessageToProgressList exists', function () {
            ActionRunner.addMessageToProgressList('info', 'Hello world');
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('addMessageToProgressList(info, Hello world)');
        });

        it('mock updateProgress exists', function () {
            ActionRunner.updateProgress('abcd', 55, 0);
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('updateProgress(abcd, 55, 0)');
        });

        it('mock displayGlobalMessage exists', function () {
            ActionRunner.displayGlobalMessage('Hello world', true, 'Error');
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('displayGlobalMessage(Hello world, true, Error)');
        });

        it('mock reportSystemError exists', function () {
            ActionRunner.reportSystemError('This is a test', false);
            expect(ActionRunner.callLog.length).toBe(1);
            expect(ActionRunner.callLog[0]).toBe('reportSystemError(This is a test, false)');
        });

    });
});
