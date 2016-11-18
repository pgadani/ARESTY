using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class GreetingB : MonoBehaviour
{

    enum status { idle, work, social };
    public GameObject w1;
    public GameObject w2;
    public Transform p1;
    public Transform p2;
    public Transform p3;
    public Transform p4;
    //private PERCEIVEABLE_TYPE trig = NPC;
    Animator gAnimator;
    Animator g2Animator;
    private Func<bool> w1moving;
    private Func<bool> w2moving;
    private BehaviorAgent bAgent;
    
    // Use this for initialization
    void Start()
    {
        //Vector3 place = p1.transform.position;
        //w1.GetComponent<NPCController>().GoTo(place);
        //w2.GetComponent<NPCController>().GoTo(w1.transform.position);
        gAnimator = w1.GetComponent<Animator>();
        g2Animator = w2.GetComponent<Animator>();
        //gPer = w1.GetComponent<NPCPerception>();
        Func<bool> w1moving = () => false;
        Func<bool> w2moving = () => false;
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
        //w1.GetComponent<NPCPerception>().FirstNPC();
        return new LeafAssert(a);
    }
    protected Node GoWithin(GameObject target, GameObject chaser)
    {
        return 
            new LeafInvoke(() => chaser.GetComponent<NPCController>().GoTo(target.transform.position))
            ;

        //return new LeafInvoke(() =>
        //w2.GetComponent<NPCController>().GoTo(w1.transform.position));
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
    //protected Node waveto(GameObject a)
    //{
    //    foreach(IPerceivable p in a.GetComponent<NPCPerception>().PerceivedAgents)
    //    {
    //        Debug.Log(p);
    //        return
    //            new Sequence(
    //                new LeafInvoke(() => a.GetComponent<Animator>().Play("Wave")));
    //                //new LeafInvoke(() => p.GetNPCEntityType().GetComponent<Animator>().Play("Wave")));
    //    }
    //    return new LeafWait(1);
    //}
    protected Node BuildRoot()
    {
        Func<bool> distance = () => (Vector3.Distance(w1.transform.position, w2.transform.position) < 6);
        Func<bool> percieve = () => w1.GetComponent<NPCPerception>().Perceiving;
        Func<bool> percieve2 = () => w2.GetComponent<NPCPerception>().Perceiving;


        return new DecoratorLoop(
            new Sequence(
                //new LeafWait(10000),
                new DecoratorForceStatus(RunStatus.Success, (wander(w1, p1, p2))),
                new DecoratorForceStatus(RunStatus.Success, (wander(w2, p3, p4))),
                new DecoratorForceStatus(RunStatus.Success,
                new Sequence(
                    trigger(percieve),
                    new DecoratorForceStatus(RunStatus.Success, new Sequence(
                        new LeafProbability(0.5f),
                        //waveto(w1),
                        //wave(w1, ),
                        //new LeafInvoke(() => w1.GetComponent<NPCPerception>().PerceivedAgents.getComponent<Animator>.Play("Wave")),
                        //new LeafInvoke(() => (w1.GetComponent<NPCPerception>().FirstP()).getComponent<Animator>().Play("Wave")),
                        //new LeafInvoke(() => (w1.GetComponent<NPCPerception>().FirstP())),
                        new LeafInvoke(() => g2Animator.Play("Wave")),
                        new LeafInvoke(() => gAnimator.Play("Wave")))),
                        new LeafWait(10000)
                    
                )
            )
        ));
    }
}