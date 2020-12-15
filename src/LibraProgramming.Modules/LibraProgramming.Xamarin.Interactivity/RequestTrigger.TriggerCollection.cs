using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Interactivity
{
    public partial class RequestTrigger
    {
        private sealed class RequestTriggerActionCollection : IList<RequestTriggerAction>
        {
            private readonly RequestTrigger trigger;
            private readonly ArrayList collection;

            public int Count => collection.Count;

            public bool IsReadOnly => collection.IsReadOnly;

            public RequestTriggerAction this[int index]
            {
                get => (RequestTriggerAction)collection[index];
                set
                {
                    var action = (RequestTriggerAction)collection[index];

                    trigger.DoActionRemoved(index, action);

                    collection[index] = value;

                    trigger.DoActionInserted(index, value);
                }
            }

            public RequestTriggerActionCollection(RequestTrigger trigger)
            {
                this.trigger = trigger;
                collection = new ArrayList();
            }

            public void Add(RequestTriggerAction action)
            {
                if (null == action)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                var index = collection.Add(action);

                trigger.DoActionInserted(index, action);
            }

            public void Clear()
            {
                var actions = collection.ToArray();

                collection.Clear();

                for (var index = 0; index < actions.Length; index++)
                {
                    var action = (RequestTriggerAction)actions[index];
                    trigger.DoActionRemoved(index, action);
                }
            }

            public bool Contains(RequestTriggerAction action) => collection.Contains(action);

            public void CopyTo(RequestTriggerAction[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

            public IEnumerator<RequestTriggerAction> GetEnumerator()
            {
                for (var index = 0; index < collection.Count; index++)
                {
                    var action = (RequestTriggerAction)collection[index];
                    yield return action;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(RequestTriggerAction action) => collection.IndexOf(action);

            public void Insert(int index, RequestTriggerAction item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(RequestTriggerAction action)
            {
                var index = collection.IndexOf(action);

                if (0 <= index)
                {
                    collection.RemoveAt(index);
                    trigger.DoActionRemoved(index, action);

                    return true;
                }

                return false;
            }

            public void RemoveAt(int index)
            {
                if (0 > index)
                {
                    return;
                }

                var action = this[index];

                collection.RemoveAt(index);
                trigger.DoActionRemoved(index, action);
            }
        }
    }
}
