using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using System.Reflection;

namespace Wiring.Editor
{
    public static class LinkUtility
    {
        #region Public functions

        // Try to create a link between two nodes.
        // Returns true if the link is established successfully.
        public static bool TryLinkNodes(
            NodeBase nodeFrom, UnityEventBase triggerEvent,
            NodeBase nodeTo, MethodInfo targetMethod
        )
        {
            // Determine the type of the target action.
            var actionType = GetUnityActionToInvokeMethod(targetMethod);

            if (actionType == null) return false; // invalid target method type

            // Create an action that is bound to the target method.
            var targetAction = System.Delegate.CreateDelegate(
                actionType, nodeTo, targetMethod
            );

            if (triggerEvent is UnityEvent)
            {
                // The trigger event has no parameter.
                // Add the action to the event with a default parameter.
                if (actionType == typeof(UnityAction))
                {
                    UnityEventTools.AddVoidPersistentListener(
                        triggerEvent, (UnityAction)targetAction
                    );
                    return true;
                }
                if (actionType == typeof(UnityAction<bool>))
                {
                    UnityEventTools.AddBoolPersistentListener(
                        triggerEvent, (UnityAction<bool>)targetAction, false
                    );
                    return true;
                }
                if (actionType == typeof(UnityAction<int>))
                {
                    UnityEventTools.AddIntPersistentListener(
                        triggerEvent, (UnityAction<int>)targetAction, 0
                    );
                    return true;
                }
                if (actionType == typeof(UnityAction<float>))
                {
                    UnityEventTools.AddFloatPersistentListener(
                        triggerEvent, (UnityAction<float>)targetAction, 0.0f
                    );
                    return true;
                }
            }
            else if (triggerEvent is UnityEvent<bool>)
            {
                // The trigger event has a bool parameter.
                // Then the target method should have a bool parameter too.
                if (actionType == typeof(UnityAction<bool>))
                {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                       (UnityEvent<bool>)triggerEvent,
                       (UnityAction<bool>)targetAction
                    );
                    return true;
                }
            }
            else if (triggerEvent is UnityEvent<int>)
            {
                // The trigger event has an int parameter.
                // Then the target method should have an int parameter too.
                if (actionType == typeof(UnityAction<int>))
                {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                       (UnityEvent<int>)triggerEvent,
                       (UnityAction<int>)targetAction
                    );
                    return true;
                }
            }
            else if (triggerEvent is UnityEvent<float>)
            {
                // The trigger event has a float parameter.
                // Then the target method should have a float parameter too.
                if (actionType == typeof(UnityAction<float>))
                {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                       (UnityEvent<float>)triggerEvent,
                       (UnityAction<float>)targetAction
                    );
                    return true;
                }
            }

            return false; // trigger-target mismatch
        }

        // Remove a link between two nodes.
        public static void RemoveLinkNodes(
            NodeBase nodeFrom, UnityEventBase triggerEvent,
            NodeBase nodeTo, MethodInfo targetMethod
        )
        {
            var methodName = targetMethod.Name;

            var eventCount = triggerEvent.GetPersistentEventCount();
            for (var i = 0; i < eventCount; i++)
            {
                if (nodeTo == triggerEvent.GetPersistentTarget(i) &&
                    methodName == triggerEvent.GetPersistentMethodName(i))
                {
                    UnityEventTools.RemovePersistentListener(triggerEvent, i);
                    break;
                }
            }
        }

        #endregion

        #region Private functions

        // Returns a UnityAction type that can be used to call the given method.
        static System.Type GetUnityActionToInvokeMethod(MethodInfo method)
        {
            var args = method.GetParameters();

            // The method has no parameter: Use UnityAction.
            if (args.Length == 0) return typeof(UnityAction);

            // Only refer to the first parameter.
            var paramType = args[0].ParameterType;

            // Returns one of the corrensponding action types.
            if (paramType == typeof(bool )) return typeof(UnityAction<bool >);
            if (paramType == typeof(int  )) return typeof(UnityAction<int  >);
            if (paramType == typeof(float)) return typeof(UnityAction<float>);

            // No one matches the method type.
            return null;
        }

        #endregion
    }
}
