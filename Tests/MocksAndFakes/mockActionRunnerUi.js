//This is a mock for the ActionRunnerUi.js file

var ActionRunnerUi = (function () {

    var actionRunnerUi = {};
    actionRunnerUi.callLog = null;
    //This logs a string with the caller's function name and the parameters
    //you must provide the function name, bit it finds the function arguments itself
    actionRunnerUi.logStep = function (funcName) {
        var log = funcName + '(';
        var callerArgs = arguments.callee.caller.arguments;
        for (var i = 0; i < callerArgs.length; i++) {
            log += (typeof callerArgs[i] === 'function') ? 'function, ' : callerArgs[i] + ', ';
        };
        if (callerArgs.length > 0)
            log = log.substr(0, log.length - 2);
        actionRunnerUi.callLog.push(log + ')');
    };
    actionRunnerUi.reset = function () {
        actionRunnerUi.callLog = [];
        actionRunnerUi.actionState = null;
    }
    actionRunnerUi.actionState = null;

    //----------------------------------------------------------------
    //now the ActionRunner ui public methods
    
    actionRunnerUi.startActionUi = function ( actionConfig) {
        actionRunnerUi.logStep('startActionUi');
    };

    actionRunnerUi.endActionUi = function (successfulEnd, jsonData) {
        actionRunnerUi.logStep('endActionUi');
    };

    actionRunnerUi.addMessageToProgressList = function (messageType, messageText) {
        actionRunnerUi.logStep('addMessageToProgressList');
    };

    actionRunnerUi.updateProgress = function (percentage, numErrors) {
        actionRunnerUi.logStep('updateProgress');
    };

    actionRunnerUi.displayGlobalMessage = function (message, stayUp, messageType) {
        actionRunnerUi.logStep('displayGlobalMessage');
    };

    actionRunnerUi.confirmDialog = function(message) {
        actionRunnerUi.logStep('confirmDialog');
        return true;
    }

    //This sets the text in the ui element, which is also the state of the state machine
    actionRunnerUi.setActionState = function (text) {
        actionRunnerUi.actionState = text;
    };

    //Gets the current action state
    actionRunnerUi.getActionState = function () {
        return actionRunnerUi.actionState;
    };
    //----------------------------------------------------

    //support routine for reporting an error
    actionRunnerUi.reportSystemError = function (additionalInfo, tryAgain) {
        actionRunnerUi.logStep('reportSystemError');
    };

    //Return the mock base which the ui methods in it plus the test items
    return actionRunnerUi;

}());