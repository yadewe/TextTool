using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextTool
{
    public static class NotificationTool
    {
        public static void ShowWindowsTip(string tip, string message = null, int showSeconds = 3, ToolTipIcon balloonTipIcon = ToolTipIcon.Info)
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = System.Drawing.SystemIcons.Application,
                BalloonTipIcon = balloonTipIcon,
                BalloonTipTitle = message,
                BalloonTipText = tip,
                Text = tip,
            };

            var showMinSeconds = showSeconds * 1000;
            //notification.BalloonTipClosed += (sender, args) => notification.Dispose();
            //notification.BalloonTipClicked += (sender, args) => notification.Dispose();
            // Display for 5 seconds.
            notification.ShowBalloonTip(showMinSeconds);

            // This will let the balloon close after it's 5 second timeout
            // for demonstration purposes. Comment this out to see what happens
            // when dispose is called while a balloon is still visible.
            Thread.Sleep(showMinSeconds);

            // The notification should be disposed when you don't need it anymore,
            // but doing so will immediately close the balloon if it's visible.
            notification.Dispose();
        }
    }
}
