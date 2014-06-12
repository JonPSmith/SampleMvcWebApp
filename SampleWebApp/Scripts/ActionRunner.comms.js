

var ActionRunner = (function (actionRunner, $, window) {
    'use strict';

    var commsResources = {
        nojQuery: 'jQuery was not found. Please ensure jQuery is referenced before this ActionRinner JavaScript file.',
        noSignalR: 'SignalR was not found. Please ensure SignalR is referenced before this ActionRunner JavaScript file.',
        confirmExitOnRunningSys: 'The system is waiting for the server to respond. Do you want to exit anyway? Press OK to exit.',
    };

    if (typeof ($) !== 'function') {
        // no jQuery!
        throw commsResources.nojQuery;
    }
    if (typeof ($.hubConnection) !== 'function') {
        //no signalR
        throw commsResources.noSignalR;
    }

    //The text in the $ActionButton button is the control for the state machine. The text consists of one of the actionsStates
    //Note: some actionStates are only fragments
    var actionStates = {
        transientSuffix: '...',         //if the state ends with this it is transitary. Used to convey that to user and also allow forced abort
        startingPrefix: 'Starting',
        cancel: 'Cancel',               // allows user to cancel action
        cancellingPrefix: 'Cancelling', //prefix for transitory state where cancel has been sent by no response yet
        cancelled: 'Cancelled',         //finished cancelling and user can exit.
        failedPrefix: 'Failed',         //some sort of system error happend. Can have extra info on end
        finishedPrefix: 'Finished',     //generalised finish. Can have extra info on end
    };
    //now add the states that are composites of the above.
    actionStates.connectingTransient = 'Connecting' + actionStates.transientSuffix; // the connection is set up but not finished
    actionStates.startingTransient = actionStates.startingPrefix + actionStates.transientSuffix; // action is being started. waiting for confirmation
    actionStates.cancellingTransient = actionStates.cancellingPrefix + actionStates.transientSuffix; // user cancelled the action; but cancel hasn't finished
    actionStates.failed = actionStates.failedPrefix;
    actionStates.failedLink = actionStates.failedPrefix + ' (link)'; // error with the SignalR link to the host 
    actionStates.failedConnecting = actionStates.failedPrefix + ' (connecting)'; // error when connecting to host via SignalR
    actionStates.finishedOk = actionStates.finishedPrefix + ' Ok'; // The action has finished successfully and the user can exit. 
    actionStates.finishedErrors = actionStates.finishedPrefix + ' (errors)'; // The action finished with errors

    //These are the ProgressMessageTypes defined in the ProgressMessage class at the Server end
    var messageTypes = {
        verbose: 'Verbose',
        info: 'Info',
        warning: 'Warning',
        error: 'Error',
        critical: 'Critical',
        cancelled: 'Cancelled',
        finished: 'Finished',
        failed: 'Failed'
    };

    //We use this to check if the message type is an error
    var messageTypesThatAreErrors = ['Error', 'Critical', 'Failed'];

    function incNumErrorsIfMessageTypeIsError(messageType) {
        if ($.inArray(messageType, messageTypesThatAreErrors) > -1)
            actionRunner.numErrorMessages++;
    }

    var actionGuid = null;
    var actionChannel = null;

    var startsWith = function(str, prefix) {
        return str.lastIndexOf(prefix, 0) === 0;
    };

    var endsWith = function(str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    };

    //------------------------------------------------------------------------
    //code to deal with the SignalR connections and events

    //This deals with setting up the SignalR connections and events
    function setupTaskChannel() {

        actionRunner.setActionState(actionStates.connectingTransient);

        actionRunner.numErrorMessages = 0;

        //Setup connection and actionChannel with the functions to call
        var connection = $.hubConnection();

        //connection.logging = true;
        actionChannel = connection.createHubProxy('ActionHub');
        setupTaskFunctions();

        //Now make sure connection errors are handled
        connection.error(function(error) {
            actionRunner.setActionState(actionStates.failedLink);
            actionRunner.reportSystemError('SignalR error: ' + error);
        });
        //and start the connection and send the start message
        connection.start()
            .done(function() {
                startAction();
            })
            .fail(function(error) {
                actionRunner.setActionState(actionStates.failedConnecting);
                actionRunner.reportSystemError('SignalR connection error: ' + error);
            });
    }

    //This is called by setupTaskChannel to link to the SignalR events
    function setupTaskFunctions() {
        actionChannel.on('Progress', function(serverTaskId, percentDone, message) {
            if (serverTaskId === actionGuid) {
                incNumErrorsIfMessageTypeIsError(message.MessageTypeString);
                actionRunner.updateProgress(actionGuid, percentDone, actionRunner.numErrorMessages);
                logMessage(message);
            }
        });
        actionChannel.on('Started', function (serverActionId) {
            if (serverActionId === actionGuid) {
                actionRunner.setActionState(actionStates.cancel);
            }
        });
        actionChannel.on('Stopped', function (serverTaskId, message) {
            if (serverTaskId === actionGuid) {
                incNumErrorsIfMessageTypeIsError(message.MessageTypeString);
                logMessage(message);
                actionChannel.invoke('EndAction', actionGuid); //this cleans up the action at the server end
                if (message.MessageTypeString === messageTypes.finished) {
                    actionRunner.updateProgress(actionGuid, 100);
                    if (actionRunner.numErrorMessages === 0) {
                        actionRunner.setActionState(actionStates.finishedOk);
                    } else {
                        actionRunner.setActionState(actionStates.finishedErrors);
                    }
                } else if (message.MessageTypeString === messageTypes.cancelled) {
                    actionRunner.setActionState(actionStates.cancelled);
                } else {
                    actionRunner.setActionState(actionStates.failed);
                }
            }
        });
    }

    function startAction() {
        //we set the state first so that if the invoke fails the user can exit
        actionRunner.setActionState(actionStates.startingTransient);
        actionChannel.invoke('StartAction', actionGuid);
    }

    function cancelAction() {
        //we set the state first so that if the invoke fails the user can exit
        actionRunner.setActionState(actionStates.cancellingTransient);
        actionChannel.invoke('CancelAction', actionGuid);

    }

    //------------------------------------------------------------
    //button and window items

    function logMessage(actionMessage) {
        if (actionMessage == null || !actionMessage.MessageTypeString || !actionMessage.MessageText) {
            return;
        }
        actionRunner.addMessageToProgressList(actionGuid, actionMessage.MessageTypeString, actionMessage.MessageText);
    }

    //------------------------------------------------------
    //public variables and  methods

    actionRunner.numErrorMessages = 0;


    //This will show the action window as a modal window and then starts the action.
    //It then monitors the action channel for feeback to the user, and can send a cancel command
    //if the user presses cancel. When the action finishes it changes the button to say 'Finished'
    //
    //It expects the jsonContent to contain TaskId (for communication) and TaskName, to show the user
    //if it has a setup error it returns that error, else returns null
    actionRunner.runAction = function (jsonContent) {
        if (jsonContent.errorsDict) {
            //there are validation errors so ask ui to display them
            actionRunner.displayValidationErrors(jsonContent.errorsDict);
        } else if (jsonContent.ActionGuid) {
            //Got back a sensible content so we run start the action 
            actionGuid = jsonContent.ActionGuid;
            setupTaskChannel();

            actionRunner.createActionPanel(actionGuid); //this sets up the dialog display and show it
        } else {
            actionRunner.reportSystemError('bad call or bad json format data in response to ajax submit', true);
        }
    };

    //This should be called when the user wants to execute the state that is currently shown
    //It recieves the current state and steps on to the next state, e.g. Cancel moves to Cancelling...
    //If the new state means that the action has finished the  the finishAction method is called
    actionRunner.respondToStateChangeRequest = function (currentState) {
        if (currentState === actionStates.cancel) {
            cancelAction();
        } else if (endsWith(currentState, actionStates.transientSuffix)) {
            //The system is in the middle of an operation. Might be hung so give user chance to abandon, but only after confirmation.
            if (confirm(commsResources.confirmExitOnRunningSys)) {
                //If user says OK then we exit then remove modal panel
                actionRunner.removeActionPanel(actionGuid);
            }
        } else {
            //we need to exit
            actionRunner.removeActionPanel(actionGuid);
        }
    };

    return actionRunner;

}(ActionRunner || {}, window.jQuery, window));