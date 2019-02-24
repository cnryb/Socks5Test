using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Socks5Test.Core
{
    public class Logger
    {
        #region 日记看守者处理
        /// <summary>
        /// 日记看守者
        /// </summary>
        public TextWriter LogWatcher { get; set; } = Console.Out;

        /// <summary>
        /// 记录一行日记
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            if (this.LogWatcher != null)
                this.LogWatcher.WriteLine(message);
        }
        #endregion
    }
}
