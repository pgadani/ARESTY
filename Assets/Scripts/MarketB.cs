using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class MarketB : MonoBehaviour {

    private NPCBehavior b_Agent, s_Agent;
    public GameObject seller;
    public GameObject buyer;
    public Transform p1;//bounds on wander
    public Transform p2;
    public GameObject target;
    public Transform booth_p; // position in from of market booth
    Animator gAnimator;
    Animator sAnimator;
    Animator bAnimator;
    private BehaviorAgent bAgent;
    private int price;

    // Use this for initialization
    void Start()
    {
        b_Agent = buyer.GetComponent<NPCBehavior>();
        s_Agent = seller.GetComponent<NPCBehavior>();
        sAnimator = seller.GetComponent<Animator>();
        bAnimator = buyer.GetComponent<Animator>();
        bAgent = new BehaviorAgent(this.BuildRoot());
        BehaviorManager.Instance.Register(bAgent);
        bAgent.StartBehavior();
        price = 6;
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
    protected Node barter()
    {
        price = price - 1;
        return new LeafWait(1000);
    }
    protected Node BuildRoot()
    {
        Func<bool> badprice = () => price > 4;
        return new DecoratorLoop(
            new Sequence(
                new DecoratorForceStatus(RunStatus.Success,
                    wander(buyer, p1, p2)),//idle wander behavior
            //new LeafProbability(0.1f),
                new DecoratorForceStatus(RunStatus.Success,
                    new Sequence(
                        new Sequence(b_Agent.NPCBehavior_GoTo(booth_p, true), new LeafWait(500)),//approach booth front
                        b_Agent.NPCBehavior_OrientTowards(seller.transform.position),
                        new LeafInvoke(() => bAnimator.Play("Wave")),
                        new LeafInvoke(() => sAnimator.Play("Wave")),
                        new LeafInvoke(() => buyer.GetComponent<NPCBody>().LookAround(true)),
                        new DecoratorForceStatus(
                            RunStatus.Success,
                            new Sequence(
                                new DecoratorInvert(trigger(badprice)),
            //play some appropriate animation
                                new LeafProbability(0.1f),
                                barter()
                            )
                        ),
                        new Sequence(//buy
                            new DecoratorInvert(
                                trigger(badprice)),
                                new SequenceParallel(
                                    b_Agent.NPCBehavior_DoGesture(GESTURE_CODE.HURRAY, true),
                                    s_Agent.NPCBehavior_DoGesture(GESTURE_CODE.HURRAY, true)
                                ),
                                new Race(
                                    new Sequence(
                                        new LeafWait(500),
                                        new LeafInvoke(() => gAnimator.Play("Take")),
                                        new LeafWait(1000),
                                        new LeafWait(100)
                                    ),
                                    new Sequence(
                                        new LeafWait(500),
                                        new LeafInvoke(() => Destroy(target)),
                                        new LeafWait(500)
                                    )
                                )
                        )
                    )
                )
            )
        );
    }
}