using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace FAFramework.Utility
{
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        public ThreadSafeObservableCollection()
        {
        }

        public ThreadSafeObservableCollection(System.Collections.Generic.IEnumerable<T> collection)
            : base(collection)
        {
        }

        public ThreadSafeObservableCollection(System.Collections.Generic.List<T> list)
            : base(list)
        {
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                var eh = CollectionChanged;
                if (eh != null)
                {
                    Dispatcher dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                             let dpo = nh.Target as DispatcherObject
                                             where dpo != null
                                             select dpo.Dispatcher).FirstOrDefault();

                    if (dispatcher != null && dispatcher.CheckAccess() == false)
                    {
                        dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                    }
                    else
                    {
                        foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                            nh.Invoke(this, e);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
