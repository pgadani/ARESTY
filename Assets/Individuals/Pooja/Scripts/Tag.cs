using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using NPC;

public class Tag {

	GameObject player;
	GameObject player2;
	float touchDistance = 0.8f;
	float runDist = 10;
	int[] angles =  new int[2] {5, -5}; // new int[6] {15, -15, 30, -30, 45, -45};

	private BehaviorAgent ba;

	// Use this for initialization
	public void Init(List<GameObject> players) {
		if (players.Count == 2) {
			player = players[0];
			player2 = players[1];
			ba = new BehaviorAgent(this.BuildTreeRoot());
			BehaviorManager.Instance.Register(ba);
			ba.StartBehavior();
		}
	}

	protected Node TagNode(GameObject it, GameObject p) {
		NPCBehavior pb = p.GetComponent<NPCBehavior>();
		NPCBehavior itb = it.GetComponent<NPCBehavior>();

		int townMask = 1 << NavMesh.GetAreaFromName("Town");

		// change evade to use raycasts to find better direction to run in
		Func<Vector3> evFunc = delegate() {
			Vector3 delta = (p.transform.position - it.transform.position);
			delta.Normalize();
			Vector3 pos = p.transform.position + runDist*delta;
			NavMeshHit hit;
			NavMesh.SamplePosition(pos, out hit, runDist, townMask);
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
					NavMesh.SamplePosition(pos, out hit, runDist, townMask);
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
				// new LeafTrace(it+" chasing "+p),
				new DecoratorLoop (new LeafAssert(() => (it.transform.position-p.transform.position).magnitude > touchDistance)),
				new DecoratorLoop (pb.NPCBehavior_GoTo(evade, true)),
				new DecoratorLoop (itb.NPCBehavior_GoTo(p.transform, true))
			)),
			itb.NPCBehavior_DoGesture(GESTURE_CODE.GRAB_FRONT),
			new LeafTrace("SWITCH"),
			// pb.NPCBehavior_Stop(),
			new DecoratorForceStatus (RunStatus.Success, new Race (
				new LeafWait(Val.V(() => 3000L)), // so the waiting times out in 3 s
				new DecoratorLoop (new LeafAssert(() => (it.transform.position-p.transform.position).magnitude < 1.5*touchDistance)),
				new DecoratorLoop (itb.NPCBehavior_GoTo(evadeInv, true))
			))
		);

	}

	protected Node BuildTreeRoot() {
		return new DecoratorLoop (
			new DecoratorForceStatus (RunStatus.Success, 
			new Sequence (
				new DecoratorForceStatus (RunStatus.Success, TagNode(player, player2)),
				new DecoratorForceStatus (RunStatus.Success, TagNode(player2, player))
			))
		);
	}

}
