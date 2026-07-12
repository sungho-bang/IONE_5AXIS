using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;
using System.Runtime.Remoting.Channels.Tcp;
using FALibrary.Utility;

namespace FALibrary.Device.Network
{
    public class FADeviceTCPRemotingClient : FADeviceRemoting
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }

        private FARemoteObject _service = null;
        private TimeCriticalWork _setDataMethod = new TimeCriticalWork();
        private TimeCriticalWork _getDataMethod = new TimeCriticalWork();

        public FADeviceTCPRemotingClient()
        {
            Timeout = 50;
        }        

        public override void Open()
        {
            TcpChannel channel = new TcpChannel();
            
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);

            string host = string.Format("tcp://{0}:{1}", Address, Port);

            System.Runtime.Remoting.WellKnownClientTypeEntry remoteType =
                new System.Runtime.Remoting.WellKnownClientTypeEntry(
                    typeof(FARemoteObject), host + "/RemoteObject.rem");

            System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(remoteType);
            
            string objectUri;
            System.Runtime.Remoting.Messaging.IMessageSink messageSink = channel.CreateMessageSink(
                    host + "/RemoteObject.rem", null,
                    out objectUri);
            
            if (messageSink == null)
            {
                throw new Exception(string.Format("{0} Open fail. fail create message sink", Name));
            }

            _service = new FARemoteObject();

            try
            {
                _service.ConnectTest(); // 최초 접속을 시도한다.
            }
            catch
            {
                // 접속에 실패해도 넘어간다. 이후 SetData(), GetData() 가 호출될 때 접속이 되는지 확인한다.
            }
        }

        public override void Close()
        {
            base.Close();

            _setDataMethod.Dispose();
            _getDataMethod.Dispose();
        }        

        public override bool SetData(string data)
        {
            bool result = false;
            try
            {
                result = _setDataMethod.Execute(
                    delegate
                    {
                        try
                        {
                            _service.Data = data;
                        }
                        catch
                        {
                        }
                    }, Timeout);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public override bool GetData(out string data)
        {
            data = string.Empty;
            string gettedData = string.Empty;
            bool result = false;

            try
            {
                result = _getDataMethod.Execute(
                    delegate
                    {
                        try
                        {
                            gettedData = _service.Data;
                        }
                        catch
                        {
                        }
                    }, Timeout);

                data = gettedData;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
