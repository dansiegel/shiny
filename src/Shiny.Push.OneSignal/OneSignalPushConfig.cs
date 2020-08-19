﻿using System;
using Com.OneSignal.Abstractions;


namespace Shiny.Push.OneSignal
{
    public class OneSignalPushConfig
    {
        public string AppId { get; set; }

        public LOG_LEVEL LogLevel { get; set; } = LOG_LEVEL.ERROR;
        public LOG_LEVEL VisualLogLevel { get; set; } = LOG_LEVEL.FATAL;
        public OSInFocusDisplayOption InFocusDisplay { get; set; } = OSInFocusDisplayOption.Notification;
    }
}