using System;
using System.ComponentModel.DataAnnotations;

namespace HappyTools.Notification
{
    [Flags]
    public enum NotificationSendingType
    {
        [Display(Name = "None")]
        None = 0,

        [Display(Name = "SMS")]
        SMS = 1,

        [Display(Name = "Push")]
        Push = 2,

        [Display(Name = "Email")]
        Email = 3
    }
}
