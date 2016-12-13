using UnityEngine;
using System.Collections;
using TreeSharpPlus;

using NPC;
using System;

/// <summary>
/// We will use this module as the connector of NPC with
/// TreeSharpPlus. The main purpose of this module is to
/// be a shell for the already implemented affordances of
/// the agents. Nodes will tick here and the module on each
/// tick will return the current status of each node.
/// 
/// Note that for each affordance, we will need two functions:
///     1. Node return to create the actual Behavior Tree component
///     2. A wrapper controller on the affordance which will be ticked on
///        and need to return, on tick, the status of the node.
/// Although this will add an extra layer of complexity, I will concentrate
/// all the implementation on this module rather than mixing TreeSharpPlus
/// with the NPC affordances. That will allow, in the long run, for more
/// freedom, scalable, maitainable and clear code.
/// 
/// </summary>

public class NPCBehavior : MonoBehaviour, INPCModule {

    #region Members
    
    private NPCController g_NPCController;
    public bool Enabled = true;
    private bool g_GestureRunning = false;

    #endregion

    #region Unity_Methods

    void Awake() {
        g_NPCController = GetComponent<NPCController>();
    }

    #endregion

    #region Public_Functions
    
    public Node NPCBehavior_OrientTowards(Vector3 t) {
        g_NPCController.Debug("Orienting");
        return new LeafInvoke(() => Behavior_OrientTowards(t));
    }

    public Node NPCBehavior_LookAt(Transform t, bool start) {
        g_NPCController.Debug("Looking");
        if(start)
            return new LeafInvoke(() => Behavior_LookAt(t));
        else
            return new LeafInvoke(() => Behavior_StopLookAt());
    }

    public Node NPCBehavior_GoTo(Transform t, bool run) {
        g_NPCController.Debug("Going");
        return new LeafInvoke(
            () => Behavior_GoTo(t, run)
        );
    }

    public Node NPCBehavior_DoTimedGesture(GESTURE_CODE gesture, System.Object o = null) {
        return NPCBehavior_DoGesture(gesture, o, true);
    }

    public Node NPCBehavior_DoGesture(GESTURE_CODE gesture, System.Object o = null, bool timed = false) {
        g_NPCController.Debug("Gesturing");
        return new LeafInvoke(
            () => Behavior_DoGesture(gesture,o, timed)
        );
    }

    #endregion

    #region Private_Functions

    private RunStatus Behavior_OrientTowards(Vector3 t) {
        if(g_NPCController.Body.TargetOrientation != t ) {
            g_NPCController.OrientTowards(t);
        }
        return g_NPCController.Body.Oriented ? RunStatus.Success : RunStatus.Running;
    }

    private RunStatus Behavior_DoGesture(GESTURE_CODE gest, System.Object o = null, bool timed = false) {
        if (g_NPCController.Body.IsGesturePlaying(gest)) {
            return RunStatus.Running;
        } else if (g_GestureRunning) {
            g_GestureRunning = false;
            g_NPCController.Debug("Finished gesture");
            return RunStatus.Success;
        }  else {
            try {
                g_NPCController.Body.DoGesture(gest, o, timed);
                g_GestureRunning = true;
                return RunStatus.Running;
            } catch (System.Exception e ) {
                g_NPCController.Debug("Could not initialize gesture with error: " + e.Message);
                return RunStatus.Failure;
            }
        }
        
    }

    private RunStatus Behavior_GoTo(Transform t, bool run) {
        Vector3 val = t.position;
        if (g_NPCController.Body.IsAtTargetLocation(val)) {
            g_NPCController.Debug("Finished go to");
            return RunStatus.Success;
        }
        else {
            try {
                if (run)
                    g_NPCController.RunTo(val);
                else g_NPCController.GoTo(val);
                return RunStatus.Running;
            } catch(System.Exception e) {
                // this will occur if the target is unreacheable
                return RunStatus.Failure;
            }
        }
    }

    private RunStatus Behavior_StopLookAt() {
        g_NPCController.Body.StopLookAt();
        return RunStatus.Success;
    }

    private RunStatus Behavior_LookAt(Transform t) {
        g_NPCController.Body.StartLookAt(t);
        g_NPCController.Debug("Finished look at");
        return RunStatus.Success;
    }

    #endregion

    #region INPCModule

    public bool IsEnabled() {
        return Enabled;
    }

    public string NPCModuleName() {
        return "NPC TreeSP/Connector";
    }

    public NPC_MODULE_TARGET NPCModuleTarget() {
        return NPC_MODULE_TARGET.AI;
    }

    public NPC_MODULE_TYPE NPCModuleType() {
        return NPC_MODULE_TYPE.BEHAVIOR;
    }

    public void RemoveNPCModule() {
        throw new NotImplementedException();
    }

    public void SetEnable(bool e) {
        Enabled = e;
    }

    public bool IsUpdateable() {
        return false;
    }

    public void TickModule() {
        throw new NotImplementedException();
    }

    #endregion
}
