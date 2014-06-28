//This is a mock for the ActionRunner.ui.js file

var ActionRunner = (function (actionRunner) {

    actionRunner.callLog = null;
    //This logs a string with the caller's function name and the parameters
    //you must provide the function name, bit it finds the function arguments itself
    actionRunner.logStep = function (funcName) {
        var log = funcName + '(';
        var callerArgs = arguments.callee.caller.arguments;
        for (var i = 0; i < callerArgs.length; i++) {
            log += (typeof callerArgs[i] === 'function') ? 'function, ' : callerArgs[i] + ', ';
        };
        if (callerArgs.length > 0)
            log = log.substr(0, log.length - 2);
        actionRunner.callLog.push(log + ')');
    };
    actionRunner.reset = function () {
        actionRunner.callLog = [];
        actionRunner.actionState = null;
    }
    actionRunner.actionState = null;

    //----------------------------------------------------------------
    //now the ActionRunner ui public methods
    
    actionRunner.startActionUi = function ( actionConfig) {
        actionRunner.logStep('startActionUi');
    };

    actionRunner.endActionUi = function (successfulEnd, jsonData) {
        actionRunner.logStep('endActionUi');
    };

    actionRunner.addMessageToProgressList = function (messageType, messageText) {
        actionRunner.logStep('addMessageToProgressList');
    };

    actionRunner.updateProgress = function (percentage, numErrors) {
        actionRunner.logStep('updateProgress');
    };

    actionRunner.displayGlobalMessage = function (message, stayUp, messageType) {
        actionRunner.logStep('displayGlobalMessage');
    };

    actionRunner.confirmDialog = function(message) {
        actionRunner.logStep('confirmDialog');
        return true;
    }

    //This sets the text in the ui element, which is also the state of the state machine
    actionRunner.setActionState = function (text) {
        actionRunner.actionState = text;
    };

    //Gets the current action state
    actionRunner.getActionState = function () {
        return actionRunner.actionState;
    };
    //----------------------------------------------------

    //support routine for reporting an error
    actionRunner.reportSystemError = function (additionalInfo, tryAgain) {
        actionRunner.logStep('reportSystemError');
    };

    //Return the mock base which the ui methods in it plus the test items
    return actionRunner;

}(ActionRunner || {}));