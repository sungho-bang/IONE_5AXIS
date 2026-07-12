using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.GEM
{
    public class SetAttrRequest
    {
        public string ObjectSpec { get; private set; }
        public string ObjectType { get; private set; }
        public string[] ObjectIDs { get; private set; }

        private Dictionary<string, string> _items = new Dictionary<string, string>();

        public IEnumerable<KeyValuePair<string, string>> GetItems()
        {
            return _items.Select(x => new KeyValuePair<string, string>(x.Key, x.Value));
        }

        public IEnumerable<string> GetKeys()
        {
            return GetItems().Select(x => x.Key);
        }

        public bool IsSubset(IEnumerable<string> keys)
        {
            return GetKeys().Intersect(keys).Count() == keys.Count();
        }

        public int Count()
        {
            return _items.Count();
        }

        public string FirstObjectID()
        {
            if (ObjectIDs != null)
            {
                if (ObjectIDs.Length > 0)
                    return ObjectIDs[0];
            }

            return string.Empty;
        }

        public void Parse(AxEZGEMLib.AxEZGEM gem, MessageArgs args)
        {
            gem.GetListItemOpen(args.MsgID);

            string objectSpec = string.Empty;
            gem.GetAsciiItem(args.MsgID, ref objectSpec);
            ObjectSpec = objectSpec;

            string objectType = string.Empty;
            gem.GetAsciiItem(args.MsgID, ref objectType);
            ObjectType = objectType;

            // for object instances requested
            var idCound = gem.GetListItemOpen(args.MsgID);
            if (idCound > 0)
            {
                ObjectIDs = new string[idCound];
                for (int i = 0; i < idCound; i++)
                {
                    string objectID = string.Empty;
                    gem.GetAsciiItem(args.MsgID, ref objectID);
                    ObjectIDs[i] = objectID;
                }
            }

            gem.GetListItemClose(args.MsgID);

            // for arrtibute settings
            var count = gem.GetListItemOpen(args.MsgID);
            for (int i = 0; i < count; i++)
            {
                string id = string.Empty;
                string data = string.Empty;

                gem.GetListItemOpen(args.MsgID);
                gem.GetAsciiItem(args.MsgID, ref id);
                gem.GetAsciiItem(args.MsgID, ref data);
                gem.GetListItemClose(args.MsgID);

                _items.Add(id, data);
            }

            gem.GetListItemClose(args.MsgID);

            gem.GetListItemClose(args.MsgID);
        }

        /// <summary>
        /// 현재 객체를 기반으로 응답 메시지를 작성한다.
        /// </summary>
        /// <param name="gem">gem instance</param>
        /// <param name="args">Set Attr(S14F3)의 MessageArgs</param>
        /// <param name="errors">에러 (코드, 텍스트)의 리스트</param>
        public void SendAckMessage(AxEZGEMLib.AxEZGEM gem, MessageArgs args, IEnumerable<KeyValuePair<string, string>> errors)
        {
            var replyMsg = gem.CreateReplyMsg(args.MsgID);

            gem.OpenListItem(replyMsg);

            #region reply
            {
                gem.OpenListItem(replyMsg);
                gem.OpenListItem(replyMsg);

                gem.AddAsciiItem(replyMsg, FirstObjectID(), FirstObjectID().Length);

                {
                    gem.OpenListItem(replyMsg);

                    {
                        var item = _items.First();
                        gem.OpenListItem(replyMsg);

                        gem.AddAsciiItem(replyMsg, item.Key, item.Key.Length);
                        gem.AddAsciiItem(replyMsg, item.Value, item.Value.Length);

                        gem.CloseListItem(replyMsg);
                    }

                    gem.CloseListItem(replyMsg);
                    gem.CloseListItem(replyMsg);
                }

                gem.CloseListItem(replyMsg);
            }
            #endregion

            #region error
            {
                short obj = 0;
                gem.OpenListItem(replyMsg);
                gem.AddU1Item(replyMsg, ref obj, 1);
                gem.OpenListItem(replyMsg);
                gem.CloseListItem(replyMsg);
                if (errors != null && errors.Count() > 0)
                {
                    foreach (var error in errors)
                    {
                        gem.OpenListItem(replyMsg);
                        gem.AddAsciiItem(replyMsg, error.Key, error.Key.Length);
                        gem.AddAsciiItem(replyMsg, error.Value, error.Value.Length);
                        gem.CloseListItem(replyMsg);
                    }
                }

                gem.CloseListItem(replyMsg);
            }
            #endregion

            gem.CloseListItem(replyMsg);

            gem.SendMsg(replyMsg);
        }

        public void SendAckMessageAllAttribute(AxEZGEMLib.AxEZGEM gem, MessageArgs args, IEnumerable<KeyValuePair<string, string>> errors)
        {
            var replyMsg = gem.CreateReplyMsg(args.MsgID);

            gem.OpenListItem(replyMsg);

            #region reply
            {
                gem.OpenListItem(replyMsg);
                gem.OpenListItem(replyMsg);

                gem.AddAsciiItem(replyMsg, FirstObjectID(), FirstObjectID().Length);

                {
                    gem.OpenListItem(replyMsg);

                    {
                        foreach (var item in _items)
                        {
                            gem.OpenListItem(replyMsg);
                            gem.AddAsciiItem(replyMsg, item.Key, item.Key.Length);
                            gem.AddAsciiItem(replyMsg, item.Value, item.Value.Length);
                            gem.CloseListItem(replyMsg);
                        }
                    }

                    gem.CloseListItem(replyMsg);

                    gem.CloseListItem(replyMsg);
                }

                gem.CloseListItem(replyMsg);
            }
            #endregion

            #region error
            {
                short obj = 0;
                gem.OpenListItem(replyMsg);
                gem.AddU1Item(replyMsg, ref obj, 1);
                gem.OpenListItem(replyMsg);
                gem.CloseListItem(replyMsg);
                if (errors != null && errors.Count() > 0)
                {
                    foreach (var error in errors)
                    {
                        gem.OpenListItem(replyMsg);
                        gem.AddAsciiItem(replyMsg, error.Key, error.Key.Length);
                        gem.AddAsciiItem(replyMsg, error.Value, error.Value.Length);
                        gem.CloseListItem(replyMsg);
                    }
                }

                gem.CloseListItem(replyMsg);
            }
            #endregion

            gem.CloseListItem(replyMsg);

            gem.SendMsg(replyMsg);
        }
    }
}
