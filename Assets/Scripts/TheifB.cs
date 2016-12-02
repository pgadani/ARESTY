using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class TheifB : MonoBehaviour {
    public GameObject theif;
    public GameObject dude;
    public Transform bound1;
    public Transform bound2;
    public Transform bound3;
    public GameObject target;
    private BehaviorAgent bAgent;
    Animator gAnimator;
    Animator g2Animator;

    // Use this for initialization
    void Start () {
        gAnimator = theif.GetComponent<Animator>();
        g2Animator = dude.GetComponent<Animator>();
        bAgent = new BehaviorAgent(this.Root());
        BehaviorManager.Instance.Register(bAgent);
        bAgent.StartBehavior();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    protected Node trigger(Func<bool> a)
    {
        return new LeafAssert(a);
    }
    protected Node Goto(GameObject p, Transform place)
    {
        return new LeafInvoke(() => p.GetComponent<NPCController>().RunTo(place.transform.position));
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
    protected Node Root()
    {
        Func<bool> atTarget = () => (Vector3.Distance(theif.transform.position, target.transform.position) > 3);
        //Func<bool> seen = () => theif.GetComponent<NPCPerception>().Perceiving;//does he see 
        Func<bool> seen = () => theif.GetComponent<NPCPerception>().PerceivedAgents.Count<=0;
        return new DecoratorLoop(
            new Sequence(
                new LeafInvoke(() => dude.GetComponent<NPCBody>().LookAround(true)),
                new DecoratorForceStatus(RunStatus.Success,
                    wander(theif, bound1, bound2)),//idle wander behavior
                new DecoratorForceStatus(RunStatus.Success,
                    new Sequence(
                        //new LeafProbability(0.1f),
                        Goto(theif, target.transform),//goto target location
                        new DecoratorPrintResult( new Race(//am i within distance of my target
                            new DecoratorInvert(new Sequence(new LeafWait(100000), new LeafWait(10))),//if this terminates first, target is far away, so we abort this
                            new DecoratorInvert(
                                new Sequence(//check to see if i at the target location at intervals
                                    trigger(atTarget),new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000), trigger(atTarget), new LeafWait(1000),
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
                                new LeafInvoke(() => theif.GetComponent<NPCBody>().LookAround(true)),//look for people
                                new LeafWait(6000), new LeafWait(60)//give me time to look before
                            ),
                            new DecoratorInvert(//check to see I have seen anything at intervals
                                new Sequence(//this should be changed to see if people are specifically looking at me
                                    trigger(seen),
                                    new LeafWait(1000),
                                    trigger(seen),
                                    new LeafWait(1000),
                                    trigger(seen),
                                    new LeafWait(1000),
                                    trigger(seen),
                                    new LeafWait(1000),
                                    trigger(seen),
                                    new LeafWait(1000),
                                    trigger(seen),
                                    new LeafWait(1000)
                                )
                            )
                        ),
                        new LeafInvoke(() => theif.GetComponent<NPCBody>().LookAround(true)),
                        new LeafWait(1000000)//placeholder for do something
                    )
                ),
                new DecoratorForceStatus(RunStatus.Success,new LeafWait(1000))
            )
        );
    }
}
