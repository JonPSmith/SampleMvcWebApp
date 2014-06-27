

var ActionRunner = (function (actionRunner, $) {
    'use strict';

    var commsResources = {
        nojQuery: 'jQuery was not found. Please ensure jQuery is referenced before this ActionRinner JavaScript file.',
        noSignalR: 'SignalR was not found. Please ensure SignalR is referenced before this ActionRunner JavaScript file.',
        confirmExitOnRunningSys: 'The system is waiting for the server to respond. Do you want to exit anyway? Press OK to exit.',
        confirmExitOnNonCancellable: 'The action is not cancellable and is still running. If you exit now you will the action will still continue?' +
                                        ' Press OK if you really want to exit.',
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
    //Note: These states are shown via the button (if visible) so are set here to allow language changes.
    var actionStates = {
        transientSuffix: '...',         //if the state ends with this it is transitary. Used to convey that to user and also allow forced abort
        //items that must end with transientSuffix
        connectingTransient: 'Connecting...',
        startingTransient: 'Starting...',
        cancellingTransient: 'Cancelling...', 
        runningNoCancel: 'Running...',      //this is shown when the method does not support cancel
        //now normal states
        cancel: 'Cancel',               //when method that supports cancelling is running then this allows user to cancel action
        cancelled: 'Cancelled',
        finishedOk: 'Finished Ok',     
        finishedErrors: 'Finished (errors)',
        failed: 'Failed',
        failedLink: 'Failed (link)',   
        failedConnecting: 'Failed (connecting)'
    };

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
    var connection = null;
    var actionChannel = null;
    var actionConfig = null;

    var endsWith = function(str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    };

    function decodeActionConfig(actionConfigFlags) {
        //The action has started and we have the flags on what the action supports
        actionConfig = {};
        actionConfig.exitOnSuccess = actionConfigFlags.indexOf('ExitOnSuccess') > -1;
        actionConfig.noProgressSent = actionConfigFlags.indexOf('NoProgressSent') > -1;
        actionConfig.noMessagesSent = actionConfigFlags.indexOf('NoMessagesSent') > -1;
        actionConfig.cancelNotSupported = actionConfigFlags.indexOf('CancelNotSupported') > -1;
    }

    function exitComms(successExit) {
        //if (connection != null)
        //    //need to clean up the connection in case the user wants to run again (doesn't work if you don't do this)
        //    connection.stop();

        actionRunner.removeActionPanel(successExit);    //close the panel
    }

    //------------------------------------------------------------------------
    //code to deal with the SignalR connections and events

    //This deals with setting up the SignalR connections and events
    function setupTaskChannel() {

        actionRunner.setActionState(actionStates.connectingTransient);

        actionRunner.numErrorMessages = 0;

        //Setup connection and actionChannel with the functions to call
        connection = $.hubConnection();

        connection.logging = true;
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
                actionRunner.updateProgress(percentDone, actionRunner.numErrorMessages);
                logMessage(message);
            }
        });
        actionChannel.on('Started', function (serverActionId, actionConfigFlags) {
            if (serverActionId === actionGuid) {
                decodeActionConfig(actionConfigFlags);
                actionRunner.createActionPanel(actionConfig);
                if (actionConfig.cancelNotSupported) {
                    actionRunner.setActionState(actionStates.runningNoCancel);
                } else {               
                    actionRunner.setActionState(actionStates.cancel);
                }

            }
        });
        actionChannel.on('Stopped', function (serverTaskId, message, jsonResult) {
            if (serverTaskId === actionGuid) {
                incNumErrorsIfMessageTypeIsError(message.MessageTypeString);
                logMessage(message);
                actionChannel.invoke('EndAction', actionGuid); //this cleans up the action at the server end
                if (message.MessageTypeString === messageTypes.finished) {
                    actionRunner.updateProgress(100);
                    if (actionRunner.numErrorMessages === 0) {
                        actionRunner.setActionState(actionStates.finishedOk);
                        if (actionConfig.exitOnSuccess) {                          
                            exitComms(true);
                        } 
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
        actionRunner.addMessageToProgressList(actionMessage.MessageTypeString, actionMessage.MessageText);
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
            var messageToShow = currentState == actionStates.runningNoCancel ?
                commsResources.confirmExitOnNonCancellable :
                commsResources.confirmExitOnRunningSys;
            if (confirm(messageToShow)) {
                //If user says OK then we exit
                exitComms(false);
            }
        } else {
            //we need to exit
            exitComms(currentState === actionStates.finishedOk);
        }
    };

    return actionRunner;

}(ActionRunner || {}, window.jQuery));