using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class GuardThief : MonoBehaviour {
    private BehaviorAgent behaviorAgent;
    private GameObject guard;
    public GameObject thief;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        guard = gameObject;
        anim = guard.GetComponent<Animator>();
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    protected Node trigger(Func<bool> a)
    {
        return new LeafAssert(a);
    }

    protected Node Runto(GameObject p, Vector3 place)
    {
        return new LeafInvoke(() => p.GetComponent<NPCController>().RunTo(place));
    }

    protected Node BuildTreeRoot()
    {
        return new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new Sequence(
            trigger(() => guard.GetComponent<NPCPerception>().PerceivedAgents.Contains(thief.GetComponent<IPerceivable>())),
            new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new Sequence(
                trigger(() => !guard.GetComponent<NPCBody>().IsAtTargetLocation(thief.transform.position)),
                Runto(guard, thief.transform.position)
            )))
        )));
    }
}
/*
Need to:
Fix up perceivable of thief, use tags to generalize
*/