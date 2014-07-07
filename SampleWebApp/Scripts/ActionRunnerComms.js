/*
  The ActionRunnerComms is a global Ctor which creates a new connection instance to the ActionHub via SignalR
  The ActionRunnerComms object handles all the comms between the client (browser) and the long-running task
  running on the service (MVC application).
*/

//The first parameter is the UI part of the ActionRunner
function ActionRunnerComms(actionUi) {
    'use strict';

    //These are all the text messages output by this module. Placed in one place to allow localisation
    var commsResources = {
        nojQuery: 'jQuery was not found. Please ensure jQuery is referenced before this ActionRinner JavaScript file.',
        noSignalR: 'SignalR was not found. Please ensure SignalR is referenced before this ActionRunner JavaScript file.',
        confirmExitOnRunningSys: 'The system is waiting for the server to respond. Do you want to exit anyway? Press OK to exit.',
        confirmExitOnNonCancellable: 'The action is not cancellable and is still running.\n' +
            'If you exit now the action will still continue,\n' +
            'but its output lost. Press OK if you really want to exit.',
    };

    //contructor checks
    if (typeof ($) !== 'function') {
        // no jQuery!
        throw commsResources.nojQuery;
    }
    if (typeof ($.hubConnection) !== 'function') {
        //no signalR
        throw commsResources.noSignalR;
    }

    //more private 'constants' 

    //The text in the $ActionButton button is the control for the state machine. The text consists of one of the actionsStates
    //Note: These states are shown via the button (if visible) so are set here to allow language changes.
    var actionStates = {
        transientSuffix: '...', //if the state ends with this it is transitary. Used to convey that to user and also allow forced abort
        //items that must end with transientSuffix
        connectingTransient: 'Connecting...',
        startingTransient: 'Starting...',
        cancellingTransient: 'Cancelling...',
        runningNoCancel: 'Running...', //this is shown when the method does not support cancel
        //now normal states
        cancel: 'Cancel', //when method that supports cancelling is running then this allows user to cancel action
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

    var messageTypesThatAreErrors = ['Error', 'Critical', 'Failed'];

    //----------------------------
    //private variables

    var that = this;

    var actionChannel = null;
    var actionGuid = null;

    var actionConfig = null;           //this is set to an object containing various information about what the server action supports
    var jsonResult = null;

    //----------------------------
    //private functions

    function endsWith(str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    };

    //This decodes actionConfigString into cleaner booleans and returns the object
    //This places all actionConfig flags/decodes in one place
    function decodeActionConfig(actionConfigString) {
        actionConfig = {};
        actionConfig.exitOnSuccess = actionConfigString.indexOf('ExitOnSuccess') > -1;
        actionConfig.noProgressSent = actionConfigString.indexOf('NoProgressSent') > -1;
        actionConfig.noMessagesSent = actionConfigString.indexOf('NoMessagesSent') > -1;
        actionConfig.cancelNotSupported = actionConfigString.indexOf('CancelNotSupported') > -1;
    }

    //This deals with setting up the SignalR connections and events
    function setupTaskChannel() {

        actionUi.setActionState(actionStates.connectingTransient);

        that.numErrorMessages = 0;

        //Setup connection and actionChannel with the functions to call
        var connection = $.hubConnection();

        //connection.logging = true;
        actionChannel = connection.createHubProxy('ActionHub');
        setupTaskFunctions();

        //Now make sure connection errors are handled
        connection.error(function (error) {
            actionUi.setActionState(actionStates.failedLink);
            actionUi.reportSystemError('SignalR error: ' + error);
        });
        //and start the connection and send the start message
        connection.start()
            .done(function () {
                startAction();
            })
            .fail(function (error) {
                actionUi.setActionState(actionStates.failedConnecting);
                actionUi.reportSystemError('SignalR connection error: ' + error);
            });
    }

    //------------------------------------------------------------------------
    //code to deal with the SignalR connections and events

    //This is called by setupTaskChannel to link to the SignalR events
    function setupTaskFunctions() {
        actionChannel.on('Progress', function (serverTaskId, percentDone, message) {
            if (serverTaskId === actionGuid) {
                incNumErrorsIfMessageTypeIsError(message.MessageTypeString);
                actionUi.updateProgress(percentDone, that.numErrorMessages);
                logMessage(message);
            }
        });
        actionChannel.on('Started', function (serverActionId, actionConfigFlags) {
            if (serverActionId === actionGuid) {
                decodeActionConfig(actionConfigFlags);
                actionUi.startActionUi(actionConfig);
                if (actionConfig.cancelNotSupported) {
                    actionUi.setActionState(actionStates.runningNoCancel);
                } else {
                    actionUi.setActionState(actionStates.cancel);
                }
            }
        });
        actionChannel.on('Stopped', function (serverTaskId, message, jsonFromServer) {
            if (serverTaskId === actionGuid) {
                incNumErrorsIfMessageTypeIsError(message.MessageTypeString);
                logMessage(message);
                jsonResult = jsonFromServer;
                actionChannel.invoke('EndAction', actionGuid); //this cleans up the action at the server end
                if (message.MessageTypeString === messageTypes.finished) {
                    actionUi.updateProgress(100);
                    if (that.numErrorMessages === 0) {
                        actionUi.setActionState(actionStates.finishedOk);
                        if (actionConfig.exitOnSuccess) {
                            exitComms(true);
                        }
                    } else {
                        actionUi.setActionState(actionStates.finishedErrors);
                    }
                } else if (message.MessageTypeString === messageTypes.cancelled) {
                    actionUi.setActionState(actionStates.cancelled);
                } else {
                    actionUi.setActionState(actionStates.failed);
                }
            }
        });
    }

    function startAction() {
        //we set the state first so that if the invoke fails the user can exit
        actionUi.setActionState(actionStates.startingTransient);
        actionChannel.invoke('StartAction', actionGuid);
    }

    function cancelAction() {
        //we set the state first so that if the invoke fails the user can exit
        actionUi.setActionState(actionStates.cancellingTransient);
        actionChannel.invoke('CancelAction', actionGuid);
    }

    //------------------------------------------------------------
    //messages and stuff

    //This increments the numErrorMessages if the message was deemed to be an error
    function incNumErrorsIfMessageTypeIsError (messageType) {
        if ($.inArray(messageType, messageTypesThatAreErrors) > -1)
            that.numErrorMessages++;
    }

    function logMessage(actionMessage) {
        if (actionMessage == null || !actionMessage.MessageTypeString || !actionMessage.MessageText) {
            return;
        }
        actionUi.addMessageToProgressList(actionMessage.MessageTypeString, actionMessage.MessageText);
    }

    function exitComms(successExit) {
        actionUi.endActionUi(successExit, jsonResult);    //close the panel
    }

    //------------------------------------------------------------
    //public variables and methods

    this.numErrorMessages = 0;          //number of error messages send from server

    //This will show the action window as a modal window and then starts the action.
    //It then monitors the action channel for feeback to the user, and can send a cancel command
    //if the user presses cancel. When the action finishes it changes the button to say 'Finished'
    //
    //It expects the jsonContent to contain TaskId (for communication) and TaskName, to show the user
    //if it has a setup error it returns that error, else returns null
    this.runAction = function (jsonContent) {
        if (jsonContent.errorsDict) {
            //there are validation errors so ask ui to display them
            actionUi.displayValidationErrors(jsonContent.errorsDict);
        } else if (jsonContent.ActionGuid) {
            //Got back a sensible content so we run start the action 
            actionGuid = jsonContent.ActionGuid;
            setupTaskChannel();
        } else {
            actionUi.reportSystemError('bad call or bad json format data in response to ajax submit', true);
        }
    };

    //This should be called when the user wants to execute the state that is currently shown
    //It recieves the current state and steps on to the next state, e.g. Cancel moves to Cancelling...
    //If the new state means that the action has finished the  the finishAction method is called
    this.respondToStateChangeRequest = function (currentState) {
        if (currentState === actionStates.cancel) {
            cancelAction();
        } else if (endsWith(currentState, actionStates.transientSuffix)) {
            //The system is in the middle of an operation. Might be hung so give user chance to abandon, but only after confirmation.
            var messageToShow = currentState == actionStates.runningNoCancel ?
                commsResources.confirmExitOnNonCancellable :
                commsResources.confirmExitOnRunningSys;
            if (actionUi.confirmDialog(messageToShow)) {
                //If user says OK then we exit
                exitComms(false);
            }
        } else {
            //we need to exit
            exitComms(currentState === actionStates.finishedOk);
        }
    };

};



