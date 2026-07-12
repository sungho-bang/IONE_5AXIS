using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.GEM
{
    public class RemoteCommandHandler
    {
        private Action<AxEZGEMLib.AxEZGEM, RemoteCommandArgs> _action;
        public string Command { get; set; }
        public short Stream { get; set; }
        public short Function { get; set; }

        public RemoteCommandHandler(string command, short stream, short function, Action<AxEZGEMLib.AxEZGEM, RemoteCommandArgs> action)
        {
            Command = command;
            Stream = stream;
            Function = function;
            _action = action;
        }

        public void OnRemoteCommand(object sender, AxEZGEMLib._DEZGEMEvents_OnRemoteCommandEvent e)
        {
            var gem = sender as AxEZGEMLib.AxEZGEM;
            int msgId = e.lMsgId;
            short stream = 2;
            short function = 41;
            short wbit = 0;
            int length = 0;
            var command = e.strCommand;

            gem.GetMsgInfo(msgId, ref stream, ref function, ref wbit, ref length);

            if (string.IsNullOrEmpty(Command) || command == Command)
            {
                if (stream == Stream && function == Function)
                {
                    var args = new RemoteCommandArgs
                    {
                        MsgID = msgId,
                        Stream = stream,
                        Function = function,
                        Wbit = wbit,
                        Length = length,
                        Command = command
                    };

                    _action?.Invoke(gem, args);
                }
            }
        }

        private void GemOnTerminalMessageSingle(object sender, AxEZGEMLib._DEZGEMEvents_OnTerminalMessageSingleEvent e)
        {
            var gem = sender as AxEZGEMLib.AxEZGEM;

            int lMsgId = e.lMsgId;
            string strMsg = e.strMsg;
            gem.AcceptTerminalMessage(lMsgId);
        }
    }
}
