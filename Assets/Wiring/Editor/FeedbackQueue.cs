using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Wiring.Editor
{
    // Queue class used to feedback user actions from UI controls
    public static class FeedbackQueue
    {
        #region Feedback record class

        // Base record class
        public abstract class RecordBase {}

        // Record class: delete a node
        public class DeleteNodeRecord : RecordBase
        {
            public Node node { get; private set; }

            public DeleteNodeRecord(Node node)
            {
                this.node = node;
            }
        }

        // Record class: an inlet button was pressed
        public class InletButtonRecord : RecordBase
        {
            public Node node { get; private set; }
            public Inlet inlet { get; private set; }

            public InletButtonRecord(Node node, Inlet inlet)
            {
                this.node = node;
                this.inlet = inlet;
            }
        }

        // Record class: an outlet button was pressed
        public class OutletButtonRecord : RecordBase
        {
            public Node node { get; private set; }
            public Outlet outlet { get; private set; }

            public OutletButtonRecord(Node node, Outlet outlet)
            {
                this.node = node;
                this.outlet = outlet;
            }
        }

        #endregion

        #region Queuing properties and methods

        public static bool IsEmpty {
            get { return _queue.Count == 0; }
        }

        public static void Reset()
        {
            _queue.Clear();
        }

        public static RecordBase Dequeue()
        {
            return _queue.Dequeue();
        }

        public static void Enqueue(RecordBase record)
        {
            _queue.Enqueue(record);
        }

        static Queue<RecordBase> _queue = new Queue<RecordBase>();

        #endregion
    }
}

