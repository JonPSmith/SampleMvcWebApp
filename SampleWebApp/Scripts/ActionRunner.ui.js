

var ActionRunner = (function (actionRunner, $, window) {
    'use strict';

    var uiResources = {
        nojQuery: 'jQuery was not found. Please ensure jQuery is referenced before this ActionRunner JavaScript files.',
        nojQueryUi: 'jQuery UI was not found. This module currently relies on JQuery UI for Dialog and Progress Bar.',
        systemError: 'There was a system error. Please talk to your administrator.',
        pleaseTryLater: 'An error occured while talking to the server. Please try again later.',
        noActionPanel: 'You must have a <div> sction with the id of "action-panel" for ActionRunner to work',
        noActionButton: 'You must have a <button> or <a> with the id of "action-button" for ActionRunner to work',
    };

    if (typeof ($) !== 'function') {
        // no jQuery!
        throw uiResources.nojQuery;
    }
    if (!$.ui) {
        // no jQuery Ui
        throw uiResources.nojQueryUi;
    }

    //------------------------------------------------------------------------------------------------
    //explicit methods to access the user interface elements. 
    //Put here to allow a the developer to replace the externally used UI elements with their own choice
    //
    //The design is that a 'panel' (generic name for Dialog or window) is always created, but may be hidden
    //It only adds a menu progress icon if 

    var $actionPanel = $('#action-panel');
    if ($actionPanel.length === 0) {
        // no jQuery!
        throw uiResources.noActionPanel;
    }
    var $actionButton = $('#action-button');
    if ($actionButton.length === 0) {
        // no jQuery!
        throw uiResources.noActionButton;
    }
    var progressBarId = '#progressbar';
    var $progressBar = $(progressBarId);
    var messagesTableId = '#messages';
    var $messageContainer = $('#message-container');
    var $notification = $('#notification');

    //we use the jQuery Notify plugin from http://www.vicreative.nl/Projects/Notify if present (otherwise alerts)
    var useNotify = $.notify && $notification.length > 0;

    //this uses the bootstrap label classes
    //TODO: swap to SASS and have separate label classes
    var messageTypeClassLookup = {
        Verbose: 'label label-default',
        Info: 'label label-info',
        Warning: 'label label-warning',
        Error: 'label label-danger',
        Critical: 'label label-danger',
        Cancelled: 'label label-primary',
        Finished: 'label label-primary',
        Failed: 'label label-danger'
    };

    //Current options are:
    var panelHeaderText = null;         //optional replacement text for the 
    var successAction = null;           //optional url or function to call on successful exit

    function setOptions(optionsObject) {
        if (optionsObject == null) return;
        panelHeaderText = optionsObject.header;
        successAction = optionsObject.successAction
    };

    //This takes the actionConfig and returns jQuery Ui Dialog options object
    function CreatejQueryUiDialogOptions() {
        this.modal = true;
        this.buttons = [];                  //no buttons as must close via any other button (note: this didn't work)
        this.draggable = true;
        this.resizable = false;             //resizable would be nice, but quite fiddly. Turn off for now.
        this.closeOnEscape = false;

        //now the optional items 
        if (panelHeaderText != null)
            this.title = panelHeaderText;
    }

    //This sets the ui dialog height, width and position relative to the screen
    //and sorts out the action message table height to make it useful
    function setVariousHeightsEtc(dialogOptions) {
        var browserHeight = window.innerHeight;
        var browerWidth = window.innerWidth;

        if (actionConfig.noMessagesSent) {
            dialogOptions.width = 350;      //wide enough for default header text and auto height
        } else {
            //we make the dialog 60% height and 60% width. This gives room for messages
            dialogOptions.height = browserHeight * 0.6;
            dialogOptions.width = browerWidth * 0.6;
        }
        dialogOptions.position = [
            (browerWidth - dialogOptions.width) / 2,
            (browserHeight - dialogOptions.height) / 2          
        ];

        //now we need to work out the height of the other elements inside the panel to get the right size for message table
        var sumOtherHeights = 38 /* ui top height */ + $progressBar.height() + $('.action-lower-menu').height();
        $('#message-container').css( 'max-height', dialogOptions.height - sumOtherHeights - 70);       //last number allows for padding

    }

    var actionConfig = null;            //this holds the flags saying what functions the started action supports

    //This shows a panel with a progress bar and a message area, plus a button to cancel with
    function setupPanelProgress() {
        if (actionConfig.noProgressSent) {
            //use spinning progress icon
            $progressBar.html('<div class="centeredImage"><br /><img id="loading" alt="Running ..." src="../../Content/img/ajax-loader.gif" style="float:" /><p>&nbsp;</p></div>');
        } else {
            $progressBar.html('');          //clear a possible spinning logo
            $progressBar.progressbar({ value: 0 });              
            $(progressBarId + ' > div').css({ 'background': '#468847' });   //we set the bar to bootstrap's success colour
        }

        if (!actionConfig.noMessagesSent) {
            $messageContainer.html('<table id="messages" class="table table-condensed"><tbody></tbody></table>');
        }
        
        $actionButton.on('click', function (eventObject) {
            actionRunner.respondToStateChangeRequest(eventObject.target.innerText);
        });
    }

    //------------------------------------------------------
    //public methods returned

    //we create the panel. The indeterminate flag swaps between two sorts of panel.
    actionRunner.createActionPanel = function (configFromStartMessage) {
        actionConfig = configFromStartMessage;
        $(messagesTableId + ' tr').remove();
        $actionButton.unbind('click');
        setupPanelProgress(actionConfig);
        var dialogOptions = new CreatejQueryUiDialogOptions();
        //setVariousHeightsEtc must be called after the setupPanelProgress and CreatejQueryUiDialogOptions
        setVariousHeightsEtc(dialogOptions);
        $actionPanel.dialog(dialogOptions);
        $('.ui-dialog-titlebar-close').hide();          //setting dialog buttons to empty array didn't stop the top rigth close button appearing!
        $actionPanel.removeClass('hidden');
    };

    actionRunner.removeActionPanel = function(successfulEnd) {
        $actionButton.unbind('click');
        $actionPanel.addClass('hidden');
        $actionPanel.dialog('close');
        $actionPanel.dialog('destroy');
        if ($progressBar.hasClass('ui-progressbar')) {           
            $progressBar.progressbar('destroy');
        }
        if (successfulEnd && successAction != null) {
            //It was successful end and we have something to do

            //delay as needs time for SignalR to stop (crude, but needs something)
            setTimeout(function () { window.location.href = successAction; }, 200);
        }
    };

    actionRunner.addMessageToProgressList = function (messageType, messageText) {
        if (actionConfig.noMessagesSent) return;
        var rowData = '<tr><td class="' + messageTypeClassLookup[messageType] + '">' + messageType + '</td><td>' + messageText + '</td></tr>';
        var $lastRow = $(messagesTableId + ' tr:last');
        if ($lastRow.length == 0) {
            //no rows at the moment, so add the first
            $(messagesTableId + ' tbody').append(rowData);

        } else {
            //add after the last row
            $lastRow.after(rowData);
        }
        var rowPos = $(messagesTableId + ' tr:last').position();
        $messageContainer.scrollTop(rowPos.top);
    };

    actionRunner.updateProgress = function (percentage, numErrors) {
        if (actionConfig.noProgressSent) return;
        if (typeof (percentage) !== 'number' || percentage > 100 || percentage < 0) return;

        if (numErrors > 0)
            $(progressBarId + ' > div').css({ 'background': '#b94a48' });   //we set the bar to bootstrap's danger colour      
        $progressBar.progressbar("value", percentage);
    };

    actionRunner.displayGlobalMessage = function (message, stayUp, notifyType) {       
        if (useNotify) {
            var type = notifyType || 'error';
            $notification.notify({ appendTo: '.???', opacity: 0.8, adjustScroll: false, type: type, sticky: stayUp });
        } else {
            //use a very simple alter to warn the user
            alert(message);
        }
    };

    //This takes an error dictionary in the form of object with keys that hold arrays of errors
    //This version simply concatentates the error messages and shows them in the global message
    actionRunner.displayValidationErrors = function (errorDict) {
        var combinedErrors = 'Validation errors:\n';
        for (var property in errorDict) {
            for (var i = 0; i < errorDict[property].errors.length; i++) {
                combinedErrors += errorDict[property].errors[i] + '\n';
            }
        }
        actionRunner.displayGlobalMessage(combinedErrors, true);
    };

    //----------------------------------------------------
    //get/set the action state. This is the text that appears somewhere in the UI. 
    //The text controls the state machine inside ActionRunner.comms.js

    //This sets the text in the ui element, which is also the state of the state machine
    actionRunner.setActionState = function(text) {
        $actionButton.text(text);
    };

    //Gets the current action state
    actionRunner.getActionState = function() {
        return $actionButton.text();
    };
    //----------------------------------------------------
    
    //support routine for reporting an error
    actionRunner.reportSystemError = function(additionalInfo, tryAgain) {
        if (tryAgain) {
            actionRunner.displayGlobalMessage(uiResources.pleaseTryLater);
        } else {
            actionRunner.displayGlobalMessage(uiResources.systemError, true);
            console.log('ActionRunning system error: ' + additionalInfo);
        }
    };

    //-----------------------------------------------------
    //local methods to do with ajax call

    function submitSuccess(responseContent, statusString, responseObject) {
        actionRunner.runAction(responseContent);
    }

    function submitFailed(args) {
        actionRunner.reportSystemError('submit failed. args =' + args, true);
    }

    //==========================================================
    //now the functions that are called from the view

    //1) Action which is supplied with setup data from a form and then runs
    //
    //This sets up the form element on the page to use an Ajax submit method.
    //It runs the normal MVC validation on the form
    //This allows the result to be captured and then the appropriate progress form to be displayed
    //options 
    actionRunner.startActionFromForm = function( options) {

        setOptions(options);

        $('form').submit(function() {

            $.validator.unobtrusive.parse($('form'));
            var data = $('form').serialize();
            data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();

            if ($('form').valid()) {
                $.ajax({
                    url: this.action,
                    type: 'POST',
                    data: data,
                    success: submitSuccess,
                    fail: submitFailed
                });
            }
            return false;       //needed to stop default form submit action
        });
    };

    //2) Action with is triggered from a link
    //You must supply a jQueryElementSelector for the element to bind to the click
    //and a url that the post should be sent to.
    //options allow seeting of various items - see setOptions method
    actionRunner.startActionFromLink = function (jQueryElementSelector, actionUrl, options) {
        $(jQueryElementSelector).unbind('click').on('click',
                function (event) {
                    setOptions(options);            //must only set options when triggered
                    var data = event.target.dataset;
                    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
                    $.post(actionUrl,
                        data,
                        submitSuccess);
                }
            );
    };

    return actionRunner;

}(ActionRunner || {}, window.jQuery, window));