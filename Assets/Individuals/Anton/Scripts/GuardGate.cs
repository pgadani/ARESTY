using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using NPC;

public class GuardGate {
	
	private GameObject guest;
	private GameObject[] gate;
	private GameObject[] gateHandle;
	private GameObject[] guardPosition;
	private GameObject[] guard;
	private bool[] gateOpened;

	private BehaviorAgent ba;

	// input given as guard 1, guard 2, guest
	public void Init(List<GameObject> players) {
		if (players.Count == 3) {
			guard = new GameObject[2];
			gate = new GameObject[2];
			gateHandle = new GameObject[2];
			guardPosition = new GameObject[2];
			gateOpened = new bool[2];

			guest = players[2];
			players.CopyTo(0, guard, 0, 2); // copy first two characters, who are guards
			gate[0] = GameObject.Find("Gate 1");
			gate[1] = GameObject.Find("Gate 2");
			gateHandle[0] = GameObject.Find("Gate 1 Handle");
			gateHandle[1] = GameObject.Find("Gate 2 Handle");
			guardPosition[0] = GameObject.Find("Guard Position 1");
			guardPosition[1] = GameObject.Find("Guard Position 2");

			ba = new BehaviorAgent(this.BuildTreeRoot());
			BehaviorManager.Instance.Register(ba);
			ba.StartBehavior();
		}
	}

	protected Node OpenGate(int gnum) {
		NPCBehavior npcBehavior = guard[gnum].GetComponent<NPCBehavior>();
		return new Sequence (
			new LeafAssert(() => !gateOpened[gnum]),
			npcBehavior.NPCBehavior_Stop(),
			new Selector (
				new Sequence (
					new LeafAssert(() => Vector3.Distance(gate[gnum].transform.position, guard[gnum].transform.position) > 40.0f),
					npcBehavior.NPCBehavior_GoTo(gateHandle[gnum].transform, true)
				),
				new Sequence (
					new LeafAssert(() => Vector3.Distance(gate[gnum].transform.position, guard[gnum].transform.position) <= 40.0f),
					npcBehavior.NPCBehavior_GoTo(gateHandle[gnum].transform, false)
				)
			),
			npcBehavior.NPCBehavior_DoGesture(GESTURE_CODE.GRAB_FRONT),
			new LeafInvoke(() => gate[gnum].GetComponent<Animation>().Play("gate open")),
			new LeafInvoke(() => gateOpened[gnum] = true)
		);
	}

	protected Node CloseGate(int gnum) {
		NPCBehavior npcBehavior = guard[gnum].GetComponent<NPCBehavior>();
		return new Sequence (
			new LeafAssert(() => gateOpened[gnum]),
			npcBehavior.NPCBehavior_Stop(),
			new Selector (
				new Sequence (
					new LeafAssert(() => Vector3.Distance(gate[gnum].transform.position, guard[gnum].transform.position) > 40.0f),
					npcBehavior.NPCBehavior_GoTo(gateHandle[gnum].transform, true)
				),
				new Sequence (
					new LeafAssert(() => Vector3.Distance(gate[gnum].transform.position, guard[gnum].transform.position) <= 40.0f),
					npcBehavior.NPCBehavior_GoTo(gateHandle[gnum].transform, false)
				)
			),
			npcBehavior.NPCBehavior_DoGesture(GESTURE_CODE.GRAB_FRONT),
			new LeafInvoke(() => gate[gnum].GetComponent<Animation>().Play("gate close")),
			new LeafInvoke(() => gateOpened[gnum] = false)
		);
	}

	protected Node StandGuard(int gnum) {
		NPCBehavior npcBehavior = guard[gnum].GetComponent<NPCBehavior>();
		return new Sequence (
			npcBehavior.NPCBehavior_GoTo (guardPosition[gnum].transform, false),
			npcBehavior.NPCBehavior_OrientTowards (gate[gnum].transform.position)
		);
	}

	protected Node BuildTreeRoot () {
		return new DecoratorLoop (
			new DecoratorForceStatus (
				RunStatus.Success,
				new Sequence (
					new DecoratorForceStatus (RunStatus.Success, new DecoratorLoop (
						new LeafAssert(() => Vector3.Distance (gate[0].transform.position, guest.transform.position) > 20.0f)
					)),
					new SequenceParallel (
						OpenGate(0),
						OpenGate(1)
					),
					new DecoratorForceStatus (RunStatus.Success, new DecoratorLoop (
						new LeafAssert(() => Vector3.Distance(gate[0].transform.position, guest.transform.position) < 20.0f)
					)),
					new SequenceParallel (
						CloseGate(0),
						CloseGate(1)
					),
					new SequenceParallel (
						StandGuard(0),
						StandGuard(1)
					)
				)
			)
		);
	}
}