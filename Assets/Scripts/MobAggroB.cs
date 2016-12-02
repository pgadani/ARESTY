using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class MobAggroB : MonoBehaviour {

    //enum status { idle = 0, aware, aggro, fear };
    //status mobStatus;
    public GameObject mob;
    public Transform p1;
    public Transform p2;
    //public Transform p3;
    //public Transform p4;
    Animator gAnimator;
    private BehaviorAgent bAgent;

    // Use this for initialization
    void Start()
    {
        //mobStatus = idle;
        bAgent = new BehaviorAgent(this.BuildRoot());
        BehaviorManager.Instance.Register(bAgent);
        bAgent.StartBehavior();
    }

    // Update is called once per frame
    void Update()
    {

    }
    protected Node trigger(Func<bool> a)
    {
        return new LeafAssert(a);
    }
    protected Node GoWithin(GameObject target, GameObject chaser)
    {
        return
            new LeafInvoke(() => chaser.GetComponent<NPCController>().GoTo(target.transform.position))
            ;
    }
    protected Node Goto(GameObject p, Transform place)
    {
        return new LeafInvoke(() => p.GetComponent<NPCController>().GoTo(place.transform.position));
    }
    protected Node wander(GameObject p, Transform place1, Transform place2)
    {
        Func<bool> at1 = () => (Vector3.Distance(p.transform.position, place1.transform.position) < 1);
        Func<bool> at2 = () => (Vector3.Distance(p.transform.position, place2.transform.position) < 1);
        Func<bool> moving = () => (p.GetComponent<NPCBody>().HasTarget());

        return
            new Sequence(
                new DecoratorInvert(new Sequence(
                    new DecoratorInvert(trigger(moving)),
                    new DecoratorInvert(trigger(at1)),
                    Goto(p, place1))
                ),
                new DecoratorInvert(new Sequence(
                    new DecoratorInvert(trigger(moving)),
                    new DecoratorInvert(trigger(at2)),
                    Goto(p, place2))
                )
            );
    }
    protected Node BuildRoot()
    {
        //Func<bool> distance = () => (Vector3.Distance(w1.transform.position, w2.transform.position) < 6);
        //Func<bool> percieve = () => w1.GetComponent<NPCPerception>().Perceiving;
        //Func<bool> percieve2 = () => w2.GetComponent<NPCPerception>().Perceiving;
        //Func<bool> isIdle = () => (mobStatus == idle);
        //Func<bool> isAware = () => (mobStatus == aware);
        //Func<bool> isAggro = () => (mobStatus == aggro);
        //Func<bool> isFear = () => (mobStatus == fear);
        return new DecoratorLoop(
            new Sequence(
                new DecoratorForceStatus(RunStatus.Success,
                    wander(mob, p1, p2)),new LeafWait(1000)
            )
        );

        //return new DecoratorLoop(
        //    new Sequence(
        //        new DecoratorForceStatus(RunStatus.Success,
        //            new Sequence(trigger(isIdle), (wander(mob, p1, p2)))),
        //        new DecoratorForceStatus(RunStatus.Success,
        //            new Sequence(trigger(isAware), new LeafWait(1000))),
        //        new DecoratorForceStatus(RunStatus.Success,
        //            new Sequence(trigger(isAggro), new LeafWait(1000))),
        //        new DecoratorForceStatus(RunStatus.Success,
        //            new Sequence(trigger(isFear), new LeafWait(1000)))
        //    )
        //);               
    }
}