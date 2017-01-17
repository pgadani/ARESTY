using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using System;
using NPC;

public class BehaviorTester_Office : MonoBehaviour {

    private static string SPAWN_POINT_TAG = "Spawn_Point";
    private static string BUILDING_PATROL_POINTS_TAG = "Building_Patrol_Points";
    private static string ACTOR_STREET_TAG = "Actor_Street";
    private static string ACTOR_TAG = "Actor";
    private static string GUARD_A_BUILDING = "O'Hara";
    private static string POLICEMAN = "Policeman_Charles";

    [SerializeField]
    public Transform[] OutBuildingPatrolPoints;
    public Transform[] PolicemanPatrolPoints;

    static int g_LastSpawPoint = 0;

    public int MaximumNPCAgents = 5;
    private int g_CurrentNPCs = 0;
    
    public Transform targetLocation;
    public Transform BusSitLocation;
    public Transform BusSitLocationB;
    public Vector3 originalLocation;
    public Transform BenchSitPoint;
    private NPCBehavior g_Agent, g_AgentB, g_Guard_A, g_Policeman, g_BusTaker, g_BusTakerB;
    public GameObject agent;
    public GameObject secondAgent;
    public GameObject FirstOrientation;

    [SerializeField]
    public bool Enabled = false;

    public GameObject[] NPCStreetAgents;
    GameObject[] g_SpawningPoints;
    HashSet<NPCBehavior> g_ActiveAgents;

    Dictionary<GameObject, NPCBehavior> g_NPCBehaviorActors;

    private BehaviorAgent behaviorAgent;

    #region Unity_Methods

    void Start() {
        NPCStreetAgents = GameObject.FindGameObjectsWithTag(ACTOR_STREET_TAG);
        g_SpawningPoints = GameObject.FindGameObjectsWithTag(SPAWN_POINT_TAG);
        g_Guard_A = GameObject.Find(GUARD_A_BUILDING).GetComponent<NPCBehavior>();
        g_Policeman = GameObject.Find(POLICEMAN).GetComponent<NPCBehavior>();
        g_BusTaker = GameObject.Find("Jordan").GetComponent<NPCBehavior>();
        g_BusTakerB = GameObject.Find("Alfred").GetComponent<NPCBehavior>();
        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        if (Enabled) {

            /* Just for testing */
            g_ActiveAgents = new HashSet<NPCBehavior>();
            g_NPCBehaviorActors = new Dictionary<GameObject, NPCBehavior>();
            g_Agent = agent.GetComponent<NPCBehavior>();
            g_AgentB = secondAgent.GetComponent<NPCBehavior>();
            BehaviorEvent behEvent = new BehaviorEvent(doEvent => this.BuildTreeRoot(), new IHasBehaviorObject[] { g_Agent });
            behEvent.StartEvent(1f);
            /* ---------------- */

            /* Outside guards */
            BehaviorEvent guarding = new BehaviorEvent(action => g_Guard_A.NPCBehavior_PatrolRandomPoints(OutBuildingPatrolPoints),
                new IHasBehaviorObject[] { g_Guard_A });
            guarding.StartEvent(1f);

            BehaviorEvent patroling = new BehaviorEvent(action => g_Policeman.NPCBehavior_PatrolRandomPoints(PolicemanPatrolPoints),
                new IHasBehaviorObject[] { g_Policeman });
            patroling.StartEvent(1f);

            /* Bus takers */
            BehaviorEvent busTaker = new BehaviorEvent(action => StartBusConversation(),
                new IHasBehaviorObject[] { g_BusTaker, g_BusTakerB });
            busTaker.StartEvent(1f);

            /* Ourside chatters */
            NPCBehavior rachel = GameObject.Find("Rachel").GetComponent<NPCBehavior>();
            NPCBehavior susan = GameObject.Find("Susan").GetComponent<NPCBehavior>();
            BehaviorEvent outsideConversation = new BehaviorEvent(doAction => CasualConversation(rachel, susan),
                new IHasBehaviorObject[] { rachel, susan });
            outsideConversation.StartEvent(1f);

            /* Waiting for the bus */
            NPCBehavior morty = GameObject.Find("Morty").GetComponent<NPCBehavior>();
            BehaviorEvent waitingForBus = new BehaviorEvent(doAction => WaitInPlace(morty),
                new IHasBehaviorObject[] { morty });
            waitingForBus.StartEvent(1f);

            /* Fred's cubicle */
            NPCBehavior fred = GameObject.Find("Fred").GetComponent<NPCBehavior>();
            NPCObject fredCubicle = GameObject.Find("Fred-Cubicle").GetComponent<NPCObject>();
            BehaviorEvent fredWork = new BehaviorEvent(doEvent => DoCubicleWork(fred, fredCubicle), new IHasBehaviorObject[] { fred });
            fredWork.StartEvent(1f);

            /* Indoor Bodyguard Ramirez */
            NPCBehavior ramirez = GameObject.Find("Ramirez").GetComponent<NPCBehavior>();
            BehaviorEvent ramirezPhone = new BehaviorEvent(doEvent => OnThePhoneDistracted(ramirez), new IHasBehaviorObject[] { ramirez });
            ramirezPhone.StartEvent(1f);

            
            NPCBehavior vicky = GameObject.Find("Victoria").GetComponent<NPCBehavior>();
            BehaviorEvent vicSetup = new BehaviorEvent(doEvent => vicky.NPCBehavior_TakeSit(
                GameObject.Find("Chair-Victoria").GetComponent<NPCObject>()), new IHasBehaviorObject[] { vicky });
            vicSetup.StartEvent(1f);

            /* Upper Floor */
            NPCBehavior mac = GameObject.Find("Mackenzie").GetComponent<NPCBehavior>();
            BehaviorEvent macSetup = new BehaviorEvent(doEvent => DoCubicleWork(mac,
                GameObject.Find("Mackenzie-Cubicle").GetComponent<NPCObject>()), new IHasBehaviorObject[] { mac });
            macSetup.StartEvent(1f);

        }
    }
    
    void FixedUpdate() {
        if(Enabled) {
            foreach (GameObject b in NPCStreetAgents) {
                NPCBehavior behAgent = b.GetComponent<NPCBehavior>();
                Transform point = g_SpawningPoints[g_LastSpawPoint].transform;
                g_LastSpawPoint = ((int) (UnityEngine.Random.value * 1399))  % g_SpawningPoints.Length;
                if (!g_ActiveAgents.Contains(behAgent) && behAgent.Behavior.Status != BehaviorStatus.InEvent) {
                    Transform targetLoc = g_SpawningPoints[g_LastSpawPoint].transform;
                    BehaviorEvent e = new BehaviorEvent(doEvent => behAgent.ApproachAndWait(targetLoc, (g_LastSpawPoint % 2 == 0)), new IHasBehaviorObject[] { (IHasBehaviorObject) behAgent });
                    e.StartEvent(1.0f);
                    g_ActiveAgents.Add(behAgent);
                }
                if(behAgent.Behavior.CurrentEvent != null) {
                    g_ActiveAgents.Remove(behAgent);
                }
            }
        }
    }

    #endregion
    
    private Node OnThePhoneDistracted(NPCBehavior agent) {
        return new Sequence(agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.STAND_PHONE_CALL, true));
    }


    

    private Node DoCubibleChat(NPCBehavior agentA, NPCBehavior agentB, NPCObject cubicle) {
        return new Sequence(
                DoCubicleWork(agentA, cubicle),
                new LeafWait(5000),
                agentB.NPCBehavior_GoTo(agentA.transform, false),
                agentA.NPCBehavior_DoGesture(GESTURE_CODE.DESK_WRITING, false),
                agentA.NPCBehavior_DoGesture(GESTURE_CODE.SITTING, false),
                agentA.NPCBehavior_OrientTowards(agentB.transform.position),
                agentB.NPCBehavior_LookAt(agentA.transform,true),
                agentB.NPCBehavior_DoGesture(GESTURE_CODE.WAVE_HELLO),
                new DecoratorLoop(
                        new SequenceShuffle(
                            agentA.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT),
                            agentB.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT),
                            agentB.NPCBehavior_DoGesture(GESTURE_CODE.ACKNOWLEDGE),
                            agentA.NPCBehavior_DoGesture(GESTURE_CODE.NEGATE),
                            agentA.NPCBehavior_DoGesture(GESTURE_CODE.THINK),
                            agentB.NPCBehavior_DoGesture(GESTURE_CODE.DISSAPOINTMENT),
                            agentA.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG)
                            )
                    )
            );
    }

    private Node SitAndWork(NPCBehavior agent, NPCObject chair) {
        return new Sequence(agent.NPCBehavior_TakeSit(chair),
                            new SequenceShuffle(
                                agent.NPCBehavior_DoGesture(GESTURE_CODE.DESK_WRITING, true)
                            )
                        );
    }

    private Node DoCubicleWork(NPCBehavior agent, NPCObject cubicle) {
        NPCObject chair = cubicle.transform.Find("Chair").GetComponent<NPCObject>();
        return new Sequence(agent.NPCBehavior_TakeSit(chair),
                            new SequenceShuffle(
                                agent.NPCBehavior_DoGesture(GESTURE_CODE.DESK_WRITING,true)
                            )
                        );
    }

    private Node CasualConversation(NPCBehavior agentA, NPCBehavior agentB) {
        return new Sequence(
                agentB.NPCBehavior_GoTo(agentA.transform,false),
                agentB.NPCBehavior_OrientTowards(agentA.transform),
                agentB.NPCBehavior_LookAt(agentA.transform,true),
                agentB.NPCBehavior_DoGesture(GESTURE_CODE.WAVE_HELLO),
                agentA.NPCBehavior_OrientTowards(agentB.transform),
                agentA.NPCBehavior_LookAt(agentB.transform, true),
                new DecoratorForceStatus ( RunStatus.Success,
                    new DecoratorLoop(
                            new Sequence(
                                agentA.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_SHORT),
                                agentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG),
                                agentA.NPCBehavior_DoTimedGesture(GESTURE_CODE.ACKNOWLEDGE),
                                agentA.NPCBehavior_DoTimedGesture(GESTURE_CODE.HURRAY)
                                )
                    )
                ),
                agentB.NPCBehavior_LookAt(null, false),
                agentA.NPCBehavior_LookAt(null, false),
                agentB.NPCBehavior_DoGesture(GESTURE_CODE.WAVE_HELLO)
            );
    }

    private Node WaitInPlace(NPCBehavior agent) {
        Quaternion rotation = Quaternion.Euler(0, 30, 0);
        Vector3 lookAt = rotation * agent.transform.forward;
        return new DecoratorLoop(
            new SequenceShuffle(
                    agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.BORED_IDLE),
                    agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.IDLE_SMALL_STEPS),
                    new LeafWait(2500),
                    agent.NPCBehavior_DoGesture(GESTURE_CODE.THINK),
                    new LeafWait(3500),
                    agent.NPCBehavior_DoGesture(GESTURE_CODE.LOOK_AROUND),
                    agent.NPCBehavior_OrientTowards(agent.transform.position + agent.transform.forward),
                    new LeafWait(4000),
                    agent.NPCBehavior_OrientTowards(lookAt)
                )
            );
    }

    private Node StartBusConversation() {
        return new Sequence(
            new SequenceParallel(   g_BusTaker.NPCBehavior_TakeSit(BusSitLocation), 
                g_BusTakerB.NPCBehavior_TakeSit(BusSitLocationB)),
                g_BusTaker.NPCBehavior_LookAt(g_BusTakerB.transform, true),
                g_BusTaker.NPCBehavior_DoGesture(GESTURE_CODE.WAVE_HELLO),
                new DecoratorLoop(
                        new SequenceShuffle(
                            g_BusTaker.NPCBehavior_LookAt(g_BusTakerB.transform, true),
                            g_BusTaker.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT),
                            g_BusTakerB.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG),
                            g_BusTaker.NPCBehavior_DoGesture(GESTURE_CODE.NEGATE),
                            g_BusTakerB.NPCBehavior_LookAt(g_BusTaker.transform,true),
                            g_BusTakerB.NPCBehavior_DoGesture(GESTURE_CODE.ACKNOWLEDGE),
                            g_BusTakerB.NPCBehavior_DoTimedGesture(GESTURE_CODE.WARNING),
                            g_BusTaker.NPCBehavior_LookAt(g_BusTakerB.transform,false)
                            )
                    )
            );
    }

    protected Node BuildTreeRoot() {
        originalLocation = agent.transform.position;
        Func<bool> act = () => false;
        Node tree = new Sequence(
                            g_AgentB.NPCBehavior_TakeSit(BenchSitPoint),
                            g_Agent.NPCBehavior_OrientTowards(FirstOrientation.transform.position),
                            g_Agent.NPCBehavior_LookAt(secondAgent.transform, true),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_LookAt(null, false),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.DISSAPOINTMENT),
                            g_Agent.ApproachAndWait(targetLocation, false),
                            g_Agent.NPCBehavior_OrientTowards(secondAgent.transform.position),
                            new SequenceParallel(
                                g_Agent.NPCBehavior_DoGesture(GESTURE_CODE.GREET_AT_DISTANCE),
                                new Sequence(
                                    g_AgentB.NPCBehavior_DoGesture(GESTURE_CODE.SITTING,false),
                                    g_AgentB.NPCBehavior_GoTo(g_Agent.transform,false)
                                    )
                                ),
                            new SequenceParallel(
                                    g_Agent.NPCBehavior_LookAt(g_AgentB.transform,true),
                                    g_AgentB.NPCBehavior_LookAt(g_Agent.transform, true)
                                ),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG),
                            g_AgentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.HURRAY),
                            g_AgentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG)
                        );
        return tree;
    }
}