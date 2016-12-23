using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class BehaviorTester : MonoBehaviour {

    public Transform targetLocation;
    public Vector3 originalLocation;
    private NPCBehavior g_Agent, g_AgentB;
    public GameObject agent;
    public GameObject secondAgent;
    public GameObject FirstOrientation;
    public bool Enabled = false;

    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start() {
        if(Enabled) {
            g_Agent = agent.GetComponent<NPCBehavior>();
            g_AgentB = secondAgent.GetComponent<NPCBehavior>();
            behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
            BehaviorManager.Instance.Register(behaviorAgent);
            behaviorAgent.StartBehavior();
        }
    }

    protected Node ApproachAndWait(Transform target) {
        // We are using the methods specified in the NPCBehavior class
        return new Sequence(g_Agent.NPCBehavior_GoTo(target, true), new LeafWait(1000));
    }

    protected Node BuildTreeRoot() {
        originalLocation = agent.transform.position;
        Func<bool> act = () => (Vector3.Distance(originalLocation, targetLocation.position) > 5);
        Node goTo = new Sequence(
                            g_Agent.NPCBehavior_OrientTowards(FirstOrientation.transform.position),
                            g_Agent.NPCBehavior_LookAt(secondAgent.transform, true),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_LookAt(null, false),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.DISSAPOINTMENT),
                            ApproachAndWait(targetLocation),
                            g_Agent.NPCBehavior_OrientTowards(secondAgent.transform.position),
                            new SequenceParallel(
                                g_Agent.NPCBehavior_DoGesture(GESTURE_CODE.GREET_AT_DISTANCE),
                                g_AgentB.NPCBehavior_GoTo(g_Agent.transform,false)
                                ),
                            new SequenceParallel(
                                    g_Agent.NPCBehavior_LookAt(g_AgentB.transform,true),
                                    g_AgentB.NPCBehavior_LookAt(g_Agent.transform, true)
                                ),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG),
                            g_AgentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.HURRAY),
                            g_AgentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG)
                        );
        Node trigger = new DecoratorLoop(new LeafAssert(act));
        Node root = new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new SequenceParallel(trigger, goTo)));
        return root;
    }
}