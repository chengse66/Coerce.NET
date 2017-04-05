﻿using Coerce.Commons.Collections;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Coerce.Networking.Core.Sockets
{
    class SocketAsyncEventArgsPool : QueuedObjectPool<SocketAsyncEventArgs>
    {
        protected override SocketAsyncEventArgs Create()
        {
            // Create async args
            return null;
        }
    }
}