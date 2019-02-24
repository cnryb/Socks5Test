using System;
using System.Data;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Socks5Test.Core
{
    /// <summary>
    /// TCPSocks5Server
    /// </summary>
    public class TCPSocks5Server
    {
        /// <summary>
        /// TCPSocks5Server
        /// </summary>
        public TCPSocks5Server() : this(1080) { }
        /// <summary>
        /// TCPSocks5Server
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public TCPSocks5Server(string userName, string password) : this(1080, userName, password) { }
        /// <summary>
        /// TCPSocks5Server
        /// </summary>
        /// <param name="port">端口号</param>
        public TCPSocks5Server(ushort port)
        {
            this.Port = port;
        }
        /// <summary>
        /// TCPSocks5Server
        /// </summary>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public TCPSocks5Server(ushort port, string userName, string password)
        {
            this.Port = port;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// 绑定的端口号
        /// </summary>
        public ushort Port
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get;
            private set;
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否需要验证身份
        /// </summary>
        public bool RequireValidate
        {
            get
            {
                return !string.IsNullOrEmpty(this.UserName) || !string.IsNullOrEmpty(this.Password);
            }
        }



        /// <summary>
        /// 监听者
        /// </summary>
        private TcpListener _Listener;

        /// <summary>
        /// 是否正在运行中
        /// </summary>
        internal bool IsStarting { get; private set; }

        public Logger Logger { get; set; } = new Logger();

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (!this.IsStarting)
            {
                //没有做任何异常处理
                this._Listener = new TcpListener(IPAddress.Any, this.Port);
                this._Listener.Start();
                this._Listener.BeginAcceptSocket(this.OnBeginAcceptSocket, this._Listener);
                this.IsStarting = true;
                this.Logger.WriteLine(string.Format("服务已于{0}启动....", DateTime.Now));
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (this.IsStarting)
            {
                this.IsStarting = false;
                this._Listener.Stop();
                this._Listener = null;
                this.Logger.WriteLine(string.Format("服务已于{0}停止....", DateTime.Now));
            }
        }

        /// <summary>
        /// 接收连接
        /// </summary>
        /// <param name="async"></param>
        private void OnBeginAcceptSocket(IAsyncResult async)
        {
            TcpListener listener = async.AsyncState as TcpListener;
            try
            {
                Socket client = listener.EndAcceptSocket(async);
                this.Logger.WriteLine(string.Format("于{0}接收到{1}的连接请求...", DateTime.Now, client.RemoteEndPoint));

                ThreadPool.QueueUserWorkItem(cb =>
                {
                    TCPSocks5Connection connection = new TCPSocks5Connection(this, client);
                    connection.DoRequest();
                });
                if (this.IsStarting)
                {
                    listener.BeginAcceptSocket(this.OnBeginAcceptSocket, listener);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                this.Logger.WriteLine(string.Format("于{0}发生错误,错误信息:{1}", DateTime.Now, ex.Message));
            }
        }
    }
}
