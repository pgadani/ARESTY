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

public class NPCBehavior : MonoBehaviour, INPCModule, IHasBehaviorObject {

    #region Members
    
    private NPCController g_NPCController;
    public bool Enabled = true;
    private bool g_GestureRunning = false;
    BehaviorObject g_BehaviorObject;

    public BehaviorObject Behavior {
        get {
            return g_BehaviorObject;
        }
    }

    #endregion

    #region Unity_Methods

    void Awake() {
        g_NPCController = GetComponent<NPCController>();
        g_BehaviorObject = new BehaviorObject();
        g_BehaviorObject =  new BehaviorAgent(
                                new DecoratorLoop(
                                    new LeafAssert(() => true)));
        BehaviorManager.Instance.Register( (IBehaviorUpdate) g_BehaviorObject );
        g_NPCController.Debug("NPCBehavior - Initialized: " + name);
    }

    #endregion

    #region Public_Functions

    public void InitializeNPCBehavior() {
        Awake();
    }


    public Node NPCBehavior_TakeSit(Transform t) {
        return new Sequence(
                NPCBehavior_GoTo(t, true)
                // NPCBehavior_OrientTowards(t.position + t.forward)//,
                //new SequenceParallel(
                //    NPCBehavior_DoGesture(GESTURE_CODE.SIT),
                //    NPCBehavior_DoGesture(GESTURE_CODE.SITTING,true)
                //    )
            );
    }

    public Node NPCBehavior_OrientTowards(Vector3 t) {
        return new LeafInvoke(() => Behavior_OrientTowards(t));
    }

    public Node NPCBehavior_LookAt(Transform t, bool start) {
        if(start)
            return new LeafInvoke(() => Behavior_LookAt(t));
        else
            return new LeafInvoke(() => Behavior_StopLookAt());
    }

    public Node NPCBehavior_GoTo(Transform t, bool run) {
        return new LeafInvoke(
            () => Behavior_GoTo(t, run)
        );
    }

    public Node NPCBehavior_GoTo(Val<Vector3> t, bool run) {
        return new LeafInvoke(
            () => Behavior_GoTo(t, run)
        );
    }

    public Node NPCBehavior_GoNear(Transform t, float threshold, bool run) {
        return new LeafInvoke(
            () => Behavior_GoNear(t, threshold, run)
        );
    }

    public Node NPCBehavior_Stop() {
        g_NPCController.Debug("Stopping");
        return new LeafInvoke(
            () => Behavior_Stop()
        );
    }

    public Node NPCBehavior_DoTimedGesture(GESTURE_CODE gesture, System.Object o = null) {
        return NPCBehavior_DoGesture(gesture, o, true);
    }

    public Node NPCBehavior_DoGesture(GESTURE_CODE gesture, System.Object o = null, bool timed = false) {
        return new LeafInvoke(
            () => Behavior_DoGesture(gesture,o, timed)
        );
    }

    public override string ToString() {
        return g_NPCController.name;
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

    private RunStatus Behavior_GoTo(Val<Vector3> t, bool run) {
        Vector3 val = t.Value;
        if (g_NPCController.Body.IsAtTargetLocation(val)) {
            g_NPCController.Debug("Finished go to");
            return RunStatus.Success;
            // return RunStatus.Running; // may be better to keep node running since val could change?
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

    private RunStatus Behavior_GoTo(Transform t, bool run) {
        if (g_NPCController.Body.IsAtTargetLocation(t.position)) {
            g_NPCController.Debug("Finished go to");
            return RunStatus.Success;
        }
        else {
            try {
                if (run)
                    g_NPCController.RunTo(t.position);
                else g_NPCController.GoTo(t.position);
                return RunStatus.Running;
            } catch(System.Exception e) {
                // this will occur if the target is unreacheable
                return RunStatus.Failure;
            }
        }
    }

    private RunStatus Behavior_GoNear(Transform t, float threshold, bool run) {
        if ( Vector3.Distance(transform.position, t.position) < threshold) {
            // g_NPCController.Debug("Finished go to");
            return RunStatus.Success;
        }
        else {
            try {
                if (run)
                    g_NPCController.RunTo(t.position);
                else g_NPCController.GoTo(t.position);
                return RunStatus.Running;
            } catch(System.Exception e) {
                // this will occur if the target is unreacheable
                return RunStatus.Failure;
            }
        }
    }

    private RunStatus Behavior_Stop() {
        g_NPCController.Body.StopNavigation();
        return RunStatus.Success;
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