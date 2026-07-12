using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.GEM
{
    public class MessageHandler
    {
        private Action<AxEZGEMLib.AxEZGEM, MessageArgs> _action;
        public short Stream { get; set; }
        public short Function { get; set; }

        public MessageHandler(short stream, short function, Action<AxEZGEMLib.AxEZGEM, MessageArgs> action)
        {
            Stream = stream;
            Function = function;
            _action = action;
        }

        public void OnMsgRequest(object sender, AxEZGEMLib._DEZGEMEvents_OnMsgRequestedEvent e)
        {
            var gem = sender as AxEZGEMLib.AxEZGEM;
            int msgId = e.lMsgId;
            short stream = 0;
            short function = 0;
            short wbit = 0;
            int length = 0;

            gem.GetMsgInfo(msgId, ref stream, ref function, ref wbit, ref length);

            if (stream == Stream && function == Function)
            {
                var args = new MessageArgs
                {
                    MsgID = msgId,
                    Stream = stream,
                    Function = function,
                    Wbit = wbit,
                    Length = length
                };

                _action?.Invoke(gem, args);
            }
        }
    }
}
