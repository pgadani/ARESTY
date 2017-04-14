using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using NPC;

public class Market {

	float threshold = 15.0f;
	GameObject seller;
	GameObject[] buyers;
	private BehaviorAgent ba;
	
	// Use this for initialization	
	public void Init(List<GameObject> players) {
		if (players.Count >= 2) {
			seller = players[0];
			buyers = new GameObject[players.Count-1];
			players.CopyTo(1, buyers, 0, players.Count-1);
			ba = new BehaviorAgent(this.BuildTreeRoot());
			BehaviorManager.Instance.Register(ba);
			ba.StartBehavior();
		}
	}

	protected Node BuildTreeRoot() {
		NPCBehavior sb = seller.GetComponent<NPCBehavior>();
		Node[] goToSeller = new Node[buyers.Length];
		Node[] talkingNodding= new Node[buyers.Length+1];
		for (int i = 0; i<buyers.Length; i++) {
			NPCBehavior nb = buyers[i].GetComponent<NPCBehavior>();
			goToSeller[i] = nb.NPCBehavior_GoNear(seller.transform, threshold, false);
			talkingNodding[i] = new Sequence(
				nb.NPCBehavior_LookAt(seller.transform, true),
				nb.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT, null, true),
				new LeafWait(1000L),
				nb.NPCBehavior_DoGesture(GESTURE_CODE.ACKNOWLEDGE)
			);
		}
		talkingNodding[buyers.Length] = sb.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT, null, true);
		return new DecoratorLoop (
			new Sequence (
				new SequenceParallel(goToSeller),
				new LeafTrace("Reached seller"),
				sb.NPCBehavior_LookAt(buyers[0].transform, true),
				sb.NPCBehavior_DoGesture(GESTURE_CODE.GREET_AT_DISTANCE, null, true),
				new LeafWait(2000L),
				new LeafTrace("Talking"),
				new SequenceParallel(talkingNodding)
			)
		);
	}

}

// buyers go towards seller
// seller waves hello
// buyer reaches seller
// seller gestures talking
// buyer gestures talking and nodding
