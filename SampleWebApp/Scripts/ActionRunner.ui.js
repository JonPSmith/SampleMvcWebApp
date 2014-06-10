

var ActionRunner = (function (actionRunner, $, window) {
    'use strict';

    var uiResources = {
        nojQuery: 'jQuery was not found. Please ensure jQuery is referenced before this ActionRunner JavaScript files.',
        nojQueryUi: 'jQuery UI was not found. This module currently relies on JQuery UI for Dialog and Progress Bar.',
        systemError: 'There was a system error. Please talk to your administrator.',
        pleaseTryLater: 'An error occured while talking to the server. Please try again later.',
    };

    if (typeof ($) !== 'function') {
        // no jQuery!
        throw uiResources.nojQuery;
    }
    if (!$.ui) {
        // no jQuery Ui
        throw uiResources.nojQueryUi;
    }
    
    //-----------------------------------------------------
    //local methods to do with ajax call

    function submitSuccess(responseContent, statusString, responseObject) {
        actionRunner.runAction(responseContent);
    }

    function submitFailed(args) {
        actionRunner.reportSystemError('submit failed. args =' + args, true);
    }

    //------------------------------------------------------------------------------------------------
    //explicit methods to access the user interface elements. 
    //Put here to allow a the developer to replace the externally used UI elements with their own choice
    //
    //The design is that a 'panel' (generic name for Dialog or window) is always created, but may be hidden
    //It only adds a menu progress icon if 

    var $actionPanel = $('#action-panel');
    var $actionButton = $('#action-button');
    var progressBarId = '#progressbar';
    var $progressBar = $(progressBarId);
    var messagesTableId = '#messages';
    var $messagesContainer = $('#message-container');
    var $notification = $('#notification');

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

    //we use the jQuery Notify plugin from http://www.vicreative.nl/Projects/Notify if present (otherwise alerts)
    var useNotify = $.notify && $notification.length > 0;

    //This takes the actionConfig and returns jQuery Ui Dialog options object
    function CreatejQueryUiDialogOptions(actionConfig) {
        this.modal = true;
        this.draggable = true;
        this.resizable = false;             //resizable would be nice, but fiddly. Turn off for now.
        this.closeOnEscape = false;

        //now the optional items 
        if (actionConfig.PanelClass)
            this.dialogClass = actionConfig.PanelClass;
        if (actionConfig.HeaderText)
            this.title = actionConfig.HeaderText;
    }

    //This sets the ui dialog height, width and position relative to the screen
    //and sorts out the action message table height to make it useful
    function setVariousHeightsEtc(dialogOptions) {
        var browserHeight = window.innerHeight;
        var browerWidth = window.innerWidth;

        //we make the dialog 60% height and 60% width and place in middle of screen
        dialogOptions.height = browserHeight * 0.6;
        dialogOptions.width = browerWidth * 0.6;
        dialogOptions.position = [
            (browerWidth - dialogOptions.width) / 2,
            (browserHeight - dialogOptions.height) / 2          
        ];

        //now we need to work out the height of the other elements inside the panel to get the right size for message table
        var sumOtherHeights = 38 /* ui top height */ + $progressBar.height() + $('.action-lower-menu').height();
        $('#message-container').css( 'max-height', dialogOptions.height - sumOtherHeights - 70);       //last number allows for padding

    }

    //------------------------------------------------------
    //public methods returned

    //This will setup a panel of some for. It must use the data in the actionConfig to define what sort of panel
    //to create and whether to display it
    actionRunner.createActionPanel = function (actionId, actionConfig) {
        $(messagesTableId + ' tr').remove();
        $actionButton.unbind('click').on('click', function(eventObject) {
            actionRunner.respondToStateChangeRequest(eventObject.target.innerText);
        });
        $progressBar.progressbar({ value: 0 });              //need to define this first as next function needs to know its height
        $(progressBarId + ' > div').css({ 'background': '#468847' });   //we set the bar to bootstrap's success colour
        var dialogOptions = new CreatejQueryUiDialogOptions(actionConfig);
        setVariousHeightsEtc(dialogOptions);
        $actionPanel.dialog(dialogOptions);
        $actionPanel.removeClass('hidden');
    };

    actionRunner.removeActionPanel = function(actionId) {
        $actionButton.unbind('click');
        $actionPanel.addClass('hidden');
        $actionPanel.dialog('close');
        $actionPanel.dialog('destroy');
        $progressBar.progressbar('destroy');
    };

    actionRunner.addMessageToProgressList = function (actionId, messageType, messageText) {
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
        $messagesContainer.scrollTop(rowPos.top);
    };

    actionRunner.updateProgress = function(actionId, percentage, numErrors) {
        if (typeof (percentage) !== 'number' || percentage > 100 || percentage < 0) {
            return;
        }
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

    //This sets up the form element on the page to use an Ajax submit method.
    //It runs the normal MVC validation on the form
    //This allows the result to be captured and then the appropriate progress form to be displayed
    actionRunner.setupActionForm = function() {

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

    return actionRunner;

}(ActionRunner || {}, window.jQuery, window));