/*
 * File:        jquery-notify.js
 * Version:     0.4
 * Author:      Vincent Keizer (www.vicreative.nl)
 * Info:        www.vicreative.nl/projects/notify
 *
 * Copyright 2013 Vincent Keizer, all rights reserved.
 *
 * Dual licensed under the MIT or GPL Version 2 licenses.
 *
 */
(function ($) {
    // Events for notify

    var helpers = {
        // gets the position of the notifier
        getYPosition: function (data, queue) {
            if (data.container.attr('data-notify-adjust') != 'scroll')
            {
                // just the position in the queue
                return queue.getYPosition(data.notifier);
            }
            // calculate position for scrolling.
            var queuePos = queue.getYPosition(data.notifier);
            var pos = queuePos + $(document).scrollTop() - data.container.position().top;
            return pos < 0 ? queuePos : pos; // do not position above psoition of container
        }
    };

    var events = {
        // Event fired when notify shows.
        show: function () {
            var $this = $(this);
            var data = $this.data('notify');
            if (data) {
                // trigger before show event
                data.notifier.trigger('beforeshow', { element: this, settings: data.settings });
                // get corresponding queue
                var queue = $.notify.queue[data.container.attr('data-notify-id')];
                // add element to notify queue
                if (!queue.add(data.notifier)) {
                    // notifier isnt visible yet, show it and position it.
                    data.notifier.show()
                        .animate({ 'opacity': data.settings.opacity }, data.settings.animationDuration, function () { $(this).trigger('aftershow', { element: $this[0], settings: data.settings }); })
                        .css({ 'top': helpers.getYPosition(data, queue) });

                    if (data.settings.displayTime && !data.settings.sticky) {
                        // there is a display time set, trigger hide when time expires.
                        setTimeout(function () { $this.trigger('hide'); }, data.settings.displayTime);
                    }
                }
                else {
                    // update of notifier position. start animation to move to new position.
                    var newYPos = helpers.getYPosition(data, queue);
                    if (data.notifier.position().top != newYPos) {
                        data.notifier.animate({ 'top': newYPos }, { queue : false });
                    }
                    if (data.settings.widthAdjust) {
                        // reset width when adjusted
                        data.notifier.width("");
                        data.settings.widthAdjust = false;
                    }
                }
                if (data.notifier.outerWidth(true) > $(document).width()) {
                    // resize to window size when size exteeds window
                    data.settings.widthAdjust = true;
                    data.notifier.outerWidth($(window).width() - data.notifier.css('padding-left').replace('px', ''));
                };

                if (data.container.attr('data-notify-adjust') == 'content' && queue.isLast(data.notifier)) {
                    // update padding of container to not hide behind notfications.
                    data.container.animate({ 'padding-top': queue.getHeight() }, { queue: false }, data.settings.animationDuration);
                }
            }
        },
        // Event fired when notify hides.
        hide: function () {
            var $this = $(this);
            var data = $this.data('notify');
            if (data && data.notifier.css('opacity')) {
                // trigger before hide event
                data.notifier.trigger('beforehide', { element: this, settings: data.settings });
                // hide notifier
                data.notifier.animate({ 'opacity': 0 }, data.settings.animationDuration, function () {
                    var notifier = $(this);
                    // hide it 
                    notifier.hide();
                    // remove item from queue
                    $.notify.queue[data.container.attr('data-notify-id')].remove(data.notifier);

                    if (data.container.attr('data-notify-adjust') == 'content') {
                        // update top padding of container.
                        data.container.animate({ 'padding-top': $.notify.queue[data.container.attr('data-notify-id')].getHeight() }, { queue : false }, data.settings.animationDuration);
                    }
                    // trigger after hide event
                    notifier.trigger('afterhide', { element: $this[0], settings: data.settings });
                });
            }
        }
    };

    var methods = {
        init: function (args) {
            // create default settings
            var defaults = {
                'animationDuration': 500,
                'displayTime': 3000,
                'appendTo': 'body',
                'autoShow': true,
                'closeText': 'x',
                'sticky': false,
                'style': 'bar',
                'type': 'info',
                'adjustContent': false,
                'adjustScroll': false,
                'notifyClass': '',
                'opacity': 1,

                // callbacks
                'beforeShow': null,
                'beforeHide': null,
                'afterShow': null,
                'afterHide': null
            };

            //get notification type
            var type = args && args.type ? args.type : defaults.type;

            // extend default settings with type settings
            var settings = $.extend({}, defaults, $.notify.settings[type]);

            // extend default settings with arguments
            settings = $.extend(settings, args);

            // make sure settings has the correct type
            settings.type = $.notify.settings[type] ? type : defaults.type;

            return $(this).each(function () {
                var $this = $(this);
                var data = $this.data('notify');

                // if the plugin hasn't been initialized yet
                if (!data) {
                    // create notifier
                    var notifier = $('<div />', {
                        'class': 'notify ' + settings.style + ' ' + settings.type + (settings.notifyClass ? ' ' + settings.notifyClass : ''),
                        'data-notifier-id': new Date().getTime(),
                        'css': {
                            'display': 'none',
                            'opacity': 0,
                            'position': 'absolute'
                        }
                    }).bind({
                        'update': function () { $this.trigger('show'); },
                        'beforeshow': settings.beforeShow,
                        'beforehide': settings.beforeHide,
                        'aftershow': settings.afterShow,
                        'afterhide': settings.afterHide
                    }).append($('<span />', { // add icon for customization
                        'class': 'icon' 
                    }))
                      .append($('<span />', { // add close icon
                          'class': 'close',
                          'text': settings.closeText,
                          'click': function () { $this.trigger('hide'); }
                      }));
                    // bind events
                    $this.bind({
                        'show': events.show,
                        'hide': events.hide
                    });
                    // append element to notifier
                    notifier.append($this);
                    var container = $(settings.appendTo);
                    if (!container.attr('data-notify-id')) {
                        // initialize notify container
                        var containerId = new Date().getTime();
                        $.notify.queue[containerId] = new Queue();
                        container.attr('data-notify-id', containerId);
                        var containerPosCss = container.css('position');
                        if (containerPosCss == 'static') {
                            // set position to relative for notification positioning.
                            container.css('position', 'relative');
                        }
                        if (settings.adjustContent) {
                            // set style of container to adjust for later adjust detection.
                            container.attr('data-notify-adjust', 'content');
                        }
                        if (settings.adjustScroll) {
                            container.attr('data-notify-adjust', 'scroll');
                        }

                    }
                    container.append(notifier);
                    $this.data('notify', {
                        settings: settings,
                        notifier: notifier,
                        container: container
                    });
                    if (settings.autoShow) {
                        // show notification
                        $this.trigger('show');
                    }
                }
            });
        },
        destroy: function () {
            return this.each(function () {
                var $this = $(this),
                    data = $this.data('notify');
                if (data) {
                    data.notifier.remove();
                }
            });
        },
        show: function () {
            return this.each(function () {
                var data = $(this).data('notify');
                if (data) {
                    $(this).trigger('show');
                }
            });
        },
        hide: function () {
            return this.each(function () {
                var data = $(this).data('notify');
                if (data) {
                    $(this).trigger('hide');
                }
            });
        }
    };

    $.fn.notify = function (options) {
        var method = options;
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery notify');
        }
    };

    function Queue() {
        this._items = new Array();
    }

    $.extend(Queue.prototype, {
        add: function (element) {
            var inQueue = $.inArray(element, this._items) > -1;
            if (!inQueue) {
                // add item to end of queue
                this._items.push(element);
            }
            return inQueue;
        },
        remove: function (element) {
            // remove item from queue.
            var index = $.inArray(element, this._items);
            this._items.splice(index, 1);
            this.update(index);
        },
        update: function (startIndex) {
            var index = startIndex || 0;
            for (var i = index; i < this._items.length; i++) {
                // trigger redraw..
                var el = this._items[i];
                if (el && el.length) {
                    el.trigger('update');
                }
            }
        },
        getYPosition: function (element) {
            // get Y position of element in queue.
            var yPos = 0;
            for (var i = 0; i < this._items.length; i++) {
                var el = this._items[i];
                if (el == element) { break; }
                yPos += el.outerHeight(true);
            }
            return yPos;
        },
        getHeight: function () {
            var height = 0;
            for (var i = 0; i < this._items.length; i++) {
                height += this._items[i].outerHeight(true);
            }
            return height;
        },
        isLast: function (element) {
            return $.inArray(element, this._items) == this._items.length - 1;
        }
    });

    // create singleton queue object.
    $.notify = {
        queue: {},
        settings: {
            'info': {},
            'success': {
                'sticky': true,
                'type': 'success'
            },
            'error': {
                'sticky': true,
                'type': 'error'
            },
            'warning': {
                'sticky': true,
                'type': 'warning'
            }
        },
        create: function (text, options) {
            return $("<span />", { text : text }).notify(options);
        }
    };

    // update positioning of notifications after resize.
    $(window).resize(function () {
        for (var key in $.notify.queue) {
            $.notify.queue[key].update();
        }
    });

    // update positioning of notifications after scroll when adjustScroll is set
    $(window).scroll(function () {
        for (var key in $.notify.queue) {
            $.notify.queue[key].update();
        }
    });

})(jQuery);