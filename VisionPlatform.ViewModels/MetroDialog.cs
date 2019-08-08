using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisionPlatform.ViewModels
{
    public static class MetroDialog
    {
        /// <summary>
        /// 显示消息窗口
        /// </summary>
        /// <param name="context">环境(调用层传递this)</param>
        /// <param name="mainMsg">主消息</param>
        public static void ShowMessageDialog(object context, string mainMsg)
        {
            ShowMessageDialog(context, mainMsg, "");
        }

        /// <summary>
        /// 显示消息窗口
        /// </summary>
        /// <param name="context">环境(调用层传递this)</param>
        /// <param name="mainMsg">主消息</param>
        /// <param name="subMsg">副消息</param>
        public static void ShowMessageDialog(object context, string mainMsg, string subMsg)
        {
            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") },
            };

            DialogCoordinator.Instance.ShowMessageAsync(context, mainMsg, subMsg, MessageDialogStyle.Affirmative, metroDialogSettings);
        }

        /// <summary>
        /// 显示输入窗口
        /// </summary>
        /// <param name="context">环境(调用层传递this)</param>
        /// <param name="mainMsg">主消息</param>
        /// <returns>输入结果</returns>
        public static async Task<string> ShowInputDialog(object context, string mainMsg)
        {

            return await ShowInputDialog(context, mainMsg, "");
        }

        /// <summary>
        /// 显示输入窗口
        /// </summary>
        /// <param name="context">环境(调用层传递this)</param>
        /// <param name="mainMsg">主消息</param>
        /// <param name="subMsg">副消息</param>
        /// <returns>输入结果</returns>
        public static async Task<string> ShowInputDialog(object context, string mainMsg, string subMsg)
        {
            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") },
                NegativeButtonText = "CANCEL"
            };

            return await DialogCoordinator.Instance.ShowInputAsync(context, mainMsg, subMsg, metroDialogSettings);
        }

    }
}
