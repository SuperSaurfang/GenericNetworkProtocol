using System;
using System.Collections.Generic;
using System.Text;

namespace ComProtocol
{
    /// <summary>
    /// Connection States
    /// </summary>
    public enum ComState
    {
        Connect = 0b_0001,
        Connected,
        Disconnect,
        Disconnected,
        Reconnect
    }
}
