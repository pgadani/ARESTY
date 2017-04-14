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
	GameObject buyer;
	GameObject obj;
	Vector3 startloc;
	private BehaviorAgent ba;
	
	// Use this for initialization	
	public void Init(List<GameObject> players) {
		if (players.Count >= 2) {
			seller = players[0];
			buyer = players[1];
			startloc = buyer.transform.position;
			obj = GameObject.Find("Apple (9)"); // testing IK stuff for now
			ba = new BehaviorAgent(this.BuildTreeRoot());
			BehaviorManager.Instance.Register(ba);
			ba.StartBehavior();
		}
	}

	protected Node UntilNear(GameObject p1, GameObject p2, float dist) {
		return new DecoratorForceStatus (RunStatus.Success, 
			new DecoratorLoop (
				new LeafAssert(() => Vector3.Distance(p1.transform.position, p2.transform.position) > dist)
			)
		);
	}

	protected Node BuildTreeRoot() {
		NPCBehavior sb = seller.GetComponent<NPCBehavior>();
		NPCBehavior bb = buyer.GetComponent<NPCBehavior>();
		
		Action pickUp = delegate() {
			Animator banim = buyer.GetComponent<Animator>();
			if (banim!=null && obj!=null) {
				banim.SetLookAtWeight(1);
				banim.SetLookAtPosition(obj.transform.position);
				banim.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
				banim.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
				banim.SetIKPosition(AvatarIKGoal.RightHand,obj.transform.position);
				banim.SetIKRotation(AvatarIKGoal.RightHand,obj.transform.rotation);
			}
		};

		return new DecoratorLoop (
			new DecoratorForceStatus(RunStatus.Success, new Sequence (
				bb.NPCBehavior_LookAt(seller.transform, true),
				new SequenceParallel (
					bb.NPCBehavior_GoNear(seller.transform, threshold, false),
					new Sequence (
						this.UntilNear(buyer, seller, threshold+5),
						new LeafTrace("Reached seller"),
						sb.NPCBehavior_LookAt(buyer.transform, true),
						new LeafWait(2000l),
						sb.NPCBehavior_DoGesture(GESTURE_CODE.GREET_AT_DISTANCE, null, true),
						new LeafWait(2000l)
					)
				),
				new LeafTrace("Talking"),
				new SequenceParallel (
					sb.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT, null, true),
					bb.NPCBehavior_DoGesture(GESTURE_CODE.TALK_SHORT, null, true)
				),
				bb.NPCBehavior_GoNear(obj.transform, 4, false),
				this.UntilNear(buyer, obj, 4),
				new LeafInvoke(pickUp),
				bb.NPCBehavior_GoTo(startloc, false)
			))
		);
	}

}

// buyers go towards seller
// seller waves hello
// buyer reaches seller
// seller gestures talking
// buyer gestures talking and nodding
