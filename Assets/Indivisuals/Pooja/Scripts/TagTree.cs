using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using TreeSharpPlus;
using NPC;

public class TagTree : MonoBehaviour {

	public GameObject player;
	public GameObject player2;
	public float touchDistance;
	public float runDist = 5;
	public int[] angles = new int[6] {15, -15, 30, -30, 45, -45};

	private BehaviorAgent ba;
	// private BehaviorAgent ba2;
	// private bool on;
	private NPCBehavior npcBehavior1;
	private NPCBehavior npcBehavior2;
	public GameObject selectionIndicator1;
	public GameObject selectionIndicator2;
	private RaycastHit hitTemp;

	void Awake () {
		npcBehavior1 = player.GetComponent <NPCBehavior> ();
		npcBehavior2 = player2.GetComponent <NPCBehavior> ();
		selectionIndicator1 = player.transform.Find ("SelectionEffect").gameObject;
		selectionIndicator2 = player2.transform.Find ("SelectionEffect").gameObject;
	}

	// Use this for initialization
	void Start() {
		ba = new BehaviorAgent(this.BuildTreeRoot());
		BehaviorManager.Instance.Register(ba);
		// BehaviorManager.Instance.Register(ba2);
		ba.StartBehavior();
		// on = true;
	}

	// Update is called once per frame
	void Update() {
		// if (Input.GetKeyUp(KeyCode.Space)) {
		// 	Debug.Log("Swap");
		// 	if (on) {
		// 		ba2 = new BehaviorAgent(this.BuildTreeRoot2());
		// 		BehaviorManager.Instance.ClearReceivers();
		// 		BehaviorManager.Instance.Register(ba2);
		// 		ba2.StartBehavior();
		// 	}
		// 	else {
		// 		ba = new BehaviorAgent(this.BuildTreeRoot());
		// 		BehaviorManager.Instance.ClearReceivers();
		// 		BehaviorManager.Instance.Register(ba);
		// 		ba.StartBehavior();
		// 	}
		// 	on = !on;
		// }
	}

	protected Node Tag(GameObject it, GameObject p) {
		NPCBehavior pb = p.GetComponent<NPCBehavior>();
		NPCBehavior itb = it.GetComponent<NPCBehavior>();

		int townMask = 1 << NavMesh.GetAreaFromName("Town");

		// change evade to use raycasts to find better direction to run in
		Func<Vector3> evFunc = delegate() {
			Vector3 delta = (p.transform.position - it.transform.position);
			delta.Normalize();
			Vector3 pos = p.transform.position + runDist*delta;
			NavMeshHit hit;
			NavMesh.SamplePosition(pos, out hit, runDist, NavMesh.AllAreas);
			Vector3 maxPos = hit.position;
			if ((maxPos-p.transform.position).magnitude<runDist) {
				Vector3 rot;
				foreach (int a in angles) {
					rot = Quaternion.Euler(0,a,0) * delta;
					pos = p.transform.position + runDist*rot;
					NavMesh.SamplePosition(pos, out hit, runDist, townMask);
					if ((hit.position-p.transform.position).magnitude - (maxPos-p.transform.position).magnitude > 0.3) {
						maxPos = hit.position;
					}
				}
			}
			return maxPos;
		};
		Val<Vector3> evade = Val.V(evFunc);

		Func<Vector3> evInvFunc = delegate() {
			Vector3 delta = (it.transform.position - p.transform.position);
			delta.Normalize();
			Vector3 pos = it.transform.position + runDist*delta;
			NavMeshHit hit;
			NavMesh.SamplePosition(pos, out hit, runDist, townMask);
			Vector3 maxPos = hit.position;
			if ((maxPos-it.transform.position).magnitude<runDist) {
				Vector3 rot;
				foreach (int a in angles) {
					rot = Quaternion.Euler(0,a,0) * delta;
					pos = it.transform.position + runDist*rot;
					NavMesh.SamplePosition(pos, out hit, runDist, NavMesh.AllAreas);
					if ((hit.position-it.transform.position).magnitude - (maxPos-it.transform.position).magnitude > 0.3) {
						maxPos = hit.position;
					}
				}
			}
			return maxPos;
		};
		Val<Vector3> evadeInv = Val.V(evInvFunc);

		return new Sequence (
			new DecoratorForceStatus (RunStatus.Success, new SequenceParallel (
				// new DecoratorLoop (new LeafTrace(it+" chasing "+p+" for "+(it.transform.position-p.transform.position).magnitude)),
				new LeafTrace(it+" chasing "+p),
				new DecoratorLoop (new LeafAssert(() => (it.transform.position-p.transform.position).magnitude > touchDistance)),
				new DecoratorLoop (pb.NPCBehavior_GoTo(evade, true)),
				new DecoratorLoop (itb.NPCBehavior_GoTo(p.transform, true))
			)),
			itb.NPCBehavior_DoGesture(GESTURE_CODE.GRAB_FRONT),
			new LeafTrace("SWITCH"),
			pb.NPCBehavior_Stop(),
			new DecoratorForceStatus (RunStatus.Success, new Race (
				new LeafWait(Val.V(() => 3000L)), // so the waiting times out in 3 s
				new DecoratorLoop (new LeafAssert(() => (it.transform.position-p.transform.position).magnitude < 1.5*touchDistance)),
				new DecoratorLoop (itb.NPCBehavior_GoTo(evadeInv, true))
			))
		);

	}

	protected Node ClickMove1 () {
		return new Sequence (
			new LeafAssert (() => selectionIndicator1.activeInHierarchy),
			new LeafAssert (() => Input.GetMouseButtonDown (1)),
			new LeafAssert (() => Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitTemp, 1000.0f)),
			npcBehavior1.NPCBehavior_Stop (),
			npcBehavior1.NPCBehavior_GoTo (Val.V(() => hitTemp.point), true)
		);
	}

	protected Node ClickMove2 () {
		return new Sequence (
			new LeafAssert (() => selectionIndicator2.activeInHierarchy),
			new LeafAssert (() => Input.GetMouseButtonDown (1)),
			new LeafAssert (() => Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitTemp, 1000.0f)),
			npcBehavior2.NPCBehavior_Stop (),
			npcBehavior2.NPCBehavior_GoTo (Val.V(() => hitTemp.point), true)
		);
	}

	protected Node BuildTreeRoot() {
		return new DecoratorLoop (
			new DecoratorForceStatus (RunStatus.Success, 
				new Selector (
					ClickMove1 (),
					ClickMove2 (),
					new Sequence (
						new DecoratorForceStatus (RunStatus.Success, Tag(player, player2)),
						new DecoratorForceStatus (RunStatus.Success, Tag(player2, player))
					)
				)
			)
		);
	}

	// protected Node BuildTreeRoot2() {
	// 	return new DecoratorLoop (
	// 		new DecoratorForceStatus (RunStatus.Success, 
	// 		new Sequence (
	// 			new DecoratorForceStatus (RunStatus.Success, Tag(player2, player)),
	// 			new DecoratorForceStatus (RunStatus.Success, Tag(player, player2))
	// 		))
	// 	);
	// }
}