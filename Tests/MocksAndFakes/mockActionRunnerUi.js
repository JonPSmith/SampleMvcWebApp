//This is a mock for the ActionRunner.ui.js file

var ActionRunner = (function (actionRunner) {

    actionRunner.callLog = null;
    //This logs a string with the caller's function name and the parameters
    //Include a funcName parameter if the function is anonymous or 
    //if you want to better define the function name, e.g. connection.on
    actionRunner.logStep = function (funcName) {
        var log = funcName || arguments.callee.caller.name;
        log += '(';
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
    
    actionRunner.createActionPanel = function (actionGuid) {
        actionRunner.logStep('createActionPanel');
    };

    actionRunner.removeActionPanel = function (actionGuid) {
        actionRunner.logStep('removeActionPanel');
    };

    actionRunner.addMessageToProgressList = function (actionGuid, messageType, messageText) {
        actionRunner.logStep('addMessageToProgressList');
    };

    actionRunner.updateProgress = function (actionGuid, percentage, numErrors) {
        actionRunner.logStep('updateProgress');
    };

    actionRunner.displayGlobalMessage = function (message, stayUp, messageType) {
        actionRunner.logStep('displayGlobalMessage');
    };

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