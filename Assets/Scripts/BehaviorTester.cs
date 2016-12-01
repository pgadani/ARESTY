using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class BehaviorTester : MonoBehaviour {

    public Transform targetLocation;
    public Vector3 originalLocation;
    public GameObject agent;

    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start() {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    protected Node ApproachAndWait(Vector3 target) {
        Val<Vector3> position = Val.V(() => target);
        // We are using the methods specified in the NPCBehavior class
        return new Sequence(agent.GetComponent<NPCBehavior>().NPCBehavior_GoTo(target), new LeafWait(1000));
    }

    protected Node BuildTreeRoot() {
        originalLocation = agent.transform.position;
        Func<bool> act = () => (Vector3.Distance(originalLocation, targetLocation.position) > 5);
        Node goTo = new DecoratorLoop(
                        new Sequence(
                            ApproachAndWait(targetLocation.position)));
        Node trigger = new DecoratorLoop(new LeafAssert(act));
        Node root = new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new SequenceParallel(trigger, goTo)));
        return root;
    }
}