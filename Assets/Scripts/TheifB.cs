using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class TheifB : MonoBehaviour
{
    public GameObject theif;
    public GameObject dude;
    public GameObject guard;
    public Transform bound1;
    public Transform bound2;
    public Transform bound3;
    public GameObject target;
    private BehaviorAgent bAgent;
    Animator gAnimator;
    Animator g2Animator;
    private TagTree tag;  //so we want to use the tag behavior to do something

    // Use this for initialization
    void Start()
    {
        gAnimator = theif.GetComponent<Animator>();
        g2Animator = dude.GetComponent<Animator>();
        g3Animator = guard.GetComponent<Animator>();
        bAgent = new BehaviorAgent(this.Root());
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
    protected Node checkGP()
    {
        Func<bool> nearguard = () => (Vector3.Distance(theif.transform.position, guard.transform.position) < 10);
        return new Sequence(
            trigger(nearguard),
            tag.Tag(guard, theif),
            new LeafWait(500)
        );
    }
    protected Node Root()
    {
        Func<bool> atTarget = () => (Vector3.Distance(theif.transform.position, target.transform.position) > 3);
        //Func<bool> seen = () => theif.GetComponent<NPCPerception>().Perceiving;//does he see 
        Func<bool> notseen = () => theif.GetComponent<NPCPerception>().PerceivedAgents.Count <= 0;
        return new DecoratorLoop(
            new Sequence(
                new LeafInvoke(() => dude.GetComponent<NPCBody>().LookAround(true)),
                new DecoratorForceStatus(RunStatus.Success,
                    wander(theif, bound1, bound2)),//idle wander behavior
                new DecoratorForceStatus(RunStatus.Success,
                    new Sequence(
            //new LeafProbability(0.1f),
                        Goto(theif, target.transform),//goto target location
                        new DecoratorPrintResult(new Race(//am i within distance of my target
                            new DecoratorInvert(new Sequence(new LeafWait(100000), new LeafWait(10))),//if this terminates first, target is far away, so we abort this
                            new DecoratorInvert(
                                new Sequence(//check to see if i at the target location at intervals
                                    trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
                                    trigger(atTarget),
                                    new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000)
                                )
                            )
                        )),
                        Goto(theif, theif.transform),//stop in place if pass race (i.e. i am a the target
                        new Race(
                            new Sequence(
            //new LeafInvoke(() => theif.GetComponent<NPCBody>().LookAround(true)),//look for people
            //new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            // new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //  new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //   new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //    new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //     new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //     new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //     new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
            //     new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10),
                                new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, 5.0f)), new LeafWait(10), new LeafInvoke(() => theif.transform.Rotate(gameObject.transform.up, -5.0f)),
                                new LeafWait(6000), new LeafWait(60)//give me time to look before
                            ),
                            new DecoratorInvert(//check to see I have seen anything at intervals
                                new Sequence(//this should be changed to see if people are specifically looking at me
                                    trigger(notseen),
                                    new LeafWait(1000),
                                    trigger(notseen),
                                    new LeafWait(1000),
                                    trigger(notseen),
                                    new LeafWait(1000),
                                    trigger(notseen),
                                    new LeafWait(1000),
                                    trigger(notseen),
                                    new LeafWait(1000),
                                    trigger(notseen),
                                    new LeafWait(1000)
                                )
                            )
                        ),
                        new LeafInvoke(() => theif.GetComponent<NPCBody>().LookAround(true)),
                        new Race(
                            new Sequence(
                                new LeafWait(500), new LeafInvoke(() => gAnimator.Play("Take")), new LeafWait(1000), new LeafWait(100)),
                            new Sequence(
                                 new LeafWait(500), new LeafInvoke(() => Destroy(target)), new LeafWait(500))
            //,new Sequence( Node seen()) if fail, tag.Tag(theif, police), resolution action
                        ),
                        wander(theif, bound1, bound2),
                        new Race(
                            new Sequence(new LeafWait(1000000), new LeafWait(100)),//arbitrary wait time until safe,
                            new Selector(
                                checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),
                                checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),
                                checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),
                                checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP(),checkGP()
                            )
                        )
                    )
                )
            )
        );
    }
}
