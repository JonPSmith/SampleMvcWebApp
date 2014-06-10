//This is a mock for a small part of SignalR's javascript client
//it only mocks those items that are needed by ActionRunner.comms

var mockSignalRClient = (function ($) {

    var mock = {};

    mock.callLog = null;
    mock.onFunctionDict = null;
    mock.doneFunc = null;
    mock.failFunc = null;
    mock.errorFunc = null;
    mock.logStep = function (stepName) {
        mock.callLog.push(stepName);
    };
    mock.reset = function() {
        mock.callLog = [];
        mock.onFunctionDict = {}
        mock.doneFunc = null;
        mock.failFunc = null;
        mock.errorFunc = null;
    }

    var doneFail = {};
    doneFail.done = function (startFunc) {
        mock.logStep('connection.start.done');
        mock.doneFunc = startFunc;
        return doneFail;
    };
    doneFail.fail = function (failFunc) {
        mock.logStep('connection.start.fail');
        mock.failFunc = failFunc;
        return doneFail;
    }

    var channel = {};
    channel.on = function (namedMessage, functionToCall) {
        mock.logStep('channel.on(' + namedMessage);
        mock.onFunctionDict[namedMessage] = functionToCall;
    };
    channel.invoke = function (actionName, actionId) {
        mock.logStep('channel.invoke(' + actionName);
    };

    var connection = {};
    connection.createHubProxy = function (hubName) {
        mock.logStep('connection.createHubProxy(' + hubName);
        return channel;
    };
    connection.error = function (errorFunc) {
        mock.logStep('connection.error');
        mock.errorFunc = errorFunc;
    };
    connection.start = function () {
        mock.logStep('connection.start');
        return doneFail;
    };


    //now we run once the method to add the hubConnection function to jQuery
    $.hubConnection = function () {
        return connection;
    }

    //Return the mock base which has all the error feedback information in it
    return mock;

}(window.jQuery));