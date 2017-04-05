﻿using Coerce.Networking.Core.Channels;
using System.Net.Sockets;
using System;
using System.Text;
using Coerce.Networking.Api.Context.Channels;
using Coerce.Networking.Api.Channels;

namespace Coerce.Networking.Core.Sockets
{
    partial class AsyncServerSocket
    {
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            this.StartAccept();

            if(acceptEventArgs.SocketError != SocketError.Success)
            {
                this.CancelAccept(acceptEventArgs);
                return;
            }

            SocketAsyncEventArgs ioArgs = this._ioArgsPool.Take();
            
            if(ioArgs != null)
            {
                CoreChannel channel = ioArgs.UserToken as CoreChannel;

                channel.Socket = acceptEventArgs.AcceptSocket;
                channel.SendArgs.AcceptSocket = channel.Socket;

                acceptEventArgs.AcceptSocket = null;
                this._acceptArgsPool.Return(acceptEventArgs);

                channel.Pipeline.OnChannelConnected(new ChannelHandlerContext(channel));

                StartReceive(ioArgs);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            Channel channel = receiveEventArgs.UserToken as Channel;

            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                byte[] dataReceived = new byte[receiveEventArgs.BytesTransferred];

                System.Buffer.BlockCopy(receiveEventArgs.Buffer, receiveEventArgs.Offset, dataReceived, 0, receiveEventArgs.BytesTransferred);

                _log.Trace("Received buffer {0}", Encoding.UTF8.GetString(dataReceived));

                this.StartReceive(receiveEventArgs);
            }
            else
            {
                // Disconnect socket!
                _log.Trace("Client socket closed");

                channel.Pipeline.OnChannelDisconnected(new ChannelHandlerContext(channel));

                this.CancelReceive(receiveEventArgs);
            }
        }
        
        private void ProcessSend(SocketAsyncEventArgs acceptEventArgs)
        {
            _log.Trace("Processing send operation");
        }
    }
}