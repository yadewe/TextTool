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
        public static void ShowWindowsTip(string tip, string title = null, int showSeconds = 3, ToolTipIcon balloonTipIcon = ToolTipIcon.Info)
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                // 在右下角显示的图标
                Icon = TextTool.Properties.Resources.app,// 用App的图标
                BalloonTipIcon = balloonTipIcon,
                BalloonTipTitle = title,
                BalloonTipText = tip,
                Text = tip,
            };

            var showMinSeconds = showSeconds * 1000;

            // 没有预想的效果
            bool isClosed = false;
            // 不确定主线程不等待，这个还会不会被执行（会有个图片在右下角那显示，不会自动取消）
            notification.BalloonTipClosed += (sender, args) => { isClosed = true; };
            notification.BalloonTipClicked += (sender, args) => { isClosed = true; };
            // Display for 5 seconds.
            notification.ShowBalloonTip(showMinSeconds);

            // This will let the balloon close after it's 5 second timeout
            // for demonstration purposes. Comment this out to see what happens
            // when dispose is called while a balloon is still visible.
            var timeout = DateTime.Now.AddMilliseconds(showMinSeconds);
            while(!isClosed && DateTime.Now < timeout)
            {
                Thread.Sleep(200);
            }

            // The notification should be disposed when you don't need it anymore,
            // but doing so will immediately close the balloon if it's visible.
            notification.Dispose();
        }
    }
}
