using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Wiring.Editor
{
    // Queue class used to feedback user actions from UI component classes.
    public class FeedbackQueue
    {
        #region Feedback record class

        public class Record
        {
            public Node node { get { return _node; } }
            public Inlet inlet { get { return _inlet; } }
            public Outlet outlet { get { return _outlet; } }

            public Record(Node node, Inlet inlet)
            {
                _node = node;
                _inlet = inlet;
            }

            public Record(Node node, Outlet outlet)
            {
                _node = node;
                _outlet = outlet;
            }

            Node _node;
            Inlet _inlet;
            Outlet _outlet;
        }

        #endregion

        public static void Reset()
        {
            _queue = new Queue<Record>();
        }

        public static void EnqueueButtonPress(Node node, Inlet inlet)
        {
            _queue.Enqueue(new Record(node, inlet));
        }

        public static void EnqueueButtonPress(Node node, Outlet outlet)
        {
            _queue.Enqueue(new Record(node, outlet));
        }

        public static bool IsEmpty {
            get {
                return _queue.Count == 0;
            }
        }

        public static Record Dequeue()
        {
            return _queue.Dequeue();
        }

        static Queue<Record> _queue;
    }
}

