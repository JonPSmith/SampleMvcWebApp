//This is a mock for the ActionRunner.ui.js file

var ActionRunner = (function (actionRunner) {

    actionRunner.callLog = null;
    actionRunner.logStep = function (stepName) {
        actionRunner.callLog.push(stepName);
    };
    actionRunner.reset = function () {
        actionRunner.callLog = [];
        actionRunner.actionState = null;
    }
    actionRunner.actionState = null;

    //----------------------------------------------------------------
    //now the ActionRunner ui public methods
    
    actionRunner.createActionPanel = function (actionGuid, actionConfig) {
        actionRunner.logStep('createActionPanel');
    };

    actionRunner.removeActionPanel = function (actionGuid) {
        actionRunner.logStep('removeActionPanel');
    };

    actionRunner.addMessageToProgressList = function (actionGuid, messageType, messageText) {
        actionRunner.logStep('addMessageToProgressList(' + actionGuid + ', ' + messageType + ', ' + messageText);
    };

    actionRunner.updateProgress = function (actionGuid, percentage, numErrors) {
        actionRunner.logStep('updateProgress(' + actionGuid + ', ' + percentage + ', ' + numErrors);
    };

    actionRunner.displayGlobalMessage = function (message, stayUp, messageType) {
        actionRunner.logStep('displayGlobalMessage(' + message + ', ' + stayUp + ', ' + messageType);
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
        tryAgain = tryAgain || '';
        actionRunner.logStep('reportSystemError(' + additionalInfo + ', ' + tryAgain);
    };

    //Return the mock base which the ui methods in it plus the test items
    return actionRunner;

}(ActionRunner || {}));