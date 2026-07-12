using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FAFramework.GUI;

namespace FAFramework.Utility
{
    public class QueryMessageResultState
    {
        private bool _cancel = false;
        public Func<bool> Action { get; set; }
        public QuestionMessageBoxWindow.QuestionResult Result { get; set; } = QuestionMessageBoxWindow.QuestionResult.None;
        public bool Showed { get; private set; }
        public void Show()
        {
            _cancel = false;

            Task.Factory.StartNew(
                () =>
                {
                    Showed = true;
                    while (!Action())
                    {
                        if (_cancel)
                            break;
                    }
                });
        }

        public void Clear()
        {
            _cancel = true;
            Action = null;
            Result = QuestionMessageBoxWindow.QuestionResult.None;
            Showed = false;
        }
    }
}
