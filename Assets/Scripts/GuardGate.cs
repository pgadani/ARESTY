using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class GuardGate : MonoBehaviour {
    private BehaviorAgent behaviorAgent;
    private GameObject guard;
    public GameObject guest;
    public GameObject gate;
    private Animator anim;

    // Use this for initialization
    void Start() {
        guard = gameObject;
        anim = guard.GetComponent<Animator>();
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void FixedUpdate() {

    }

    protected Node trigger(Func<bool> a)
    {
        return new LeafAssert(a);
    }

    protected Node Goto(GameObject p, Vector3 place)
    {
        return new LeafInvoke(()=>p.GetComponent<NPCController>().GoTo(place));
    }

    /*protected Node wander(GameObject p)
    {
        int milli = System.DateTime.Now.Millisecond;
        UnityEngine.Random.InitState(milli);
        float rand = UnityEngine.Random.value * 20 - 10;
        float rand2 = UnityEngine.Random.value * 20 - 10;
        return new Sequence(new LeafTrace(milli.ToString()), new LeafTrace(rand.ToString()), new LeafTrace(rand2.ToString()), Goto(p, p.transform.position + new Vector3(rand, 0, rand2)), new LeafWait(10000));
        return new Sequence(Goto(p, p.transform.position + new Vector3(1, 0, 1)), new LeafWait(5000));
    }*/

    protected Node BuildTreeRoot()
    {
        return new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new Sequence(
                //Goto(guest, gate.transform.position + new Vector3(0, 0, 2)),
                new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new Sequence(
                    trigger(() => Vector3.Distance(guest.transform.position, gate.transform.position) < 4),
                    Goto(guard, gate.transform.position + new Vector3(0, 0, -2)),
                    new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new Sequence(
                        trigger(() => guard.GetComponent<NPCBody>().IsAtTargetLocation(gate.transform.position + new Vector3(0, 0, -2))),
                        new LeafWait(1000),
                        // GATE OPENING ANIMATION
                        new LeafInvoke(() => anim.Play("Grab_Front")),
                        //Goto(guest, gate.transform.position + new Vector3(15, 0, -5)),
                        new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new Sequence(
                            trigger(() => Vector3.Distance(guest.transform.position, gate.transform.position) > 10),
                            new LeafWait(1000),
                            // GATE CLOSING ANIMATION
                            new LeafInvoke(() => anim.Play("Grab_Front")),
                            Goto(guard, gate.transform.position + new Vector3(10, 0, -10))
                        )))
                    )))
                )))
        )));
    }
}
/*
Need to:
Fix up animations for opening/closing gate
Complete wander method (or integrate with nav mechanic)
Is guest object preset? May need to restructure code around gate
*/