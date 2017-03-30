using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using NPC;

public class Argument {

	public GameObject player;
	private BehaviorAgent ba;
	
	// Use this for initialization
	public void Init(List<GameObject> players) {
		if (players.Count == 1) {
			player = players[0];
			ba = new BehaviorAgent(this.BuildTreeRoot());
			BehaviorManager.Instance.Register(ba);
			ba.StartBehavior();
		}
	}

	protected Node BuildTreeRoot() {
		NPCBehavior pb = player.GetComponent<NPCBehavior>();
		return new DecoratorLoop (
			new Sequence (
				pb.NPCBehavior_DoGesture(GESTURE_CODE.ANGRY, null, true),
				new LeafWait(2000L),
				pb.NPCBehavior_DoGesture(GESTURE_CODE.ANNOYED, null, true),
				new LeafWait(2000L),
				pb.NPCBehavior_DoGesture(GESTURE_CODE.WHY, null, true),
				new LeafWait(2000L),
				pb.NPCBehavior_DoGesture(GESTURE_CODE.DISMISS, null, true),
				new LeafWait(2000L)
			)
		);
	}

}
