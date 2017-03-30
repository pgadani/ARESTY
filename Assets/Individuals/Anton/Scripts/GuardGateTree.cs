using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using System;
using NPC;

public class GuardGateTree : MonoBehaviour {
	
    public GameObject guest;
    public GameObject gate;
	public GameObject gateHandle;
	public GameObject guardPosition;

	private BehaviorAgent behaviorAgent;
	private GameObject guard;
	private NPCBehavior npcBehavior;
	private bool gateOpened = false;
	private RaycastHit hit;
	public GameObject selectionIndicator;

	void Awake () {
		npcBehavior = gameObject.GetComponent <NPCBehavior> ();
		selectionIndicator = gameObject.transform.Find ("SelectionEffect").gameObject;
	}

    void Start() {
        guard = gameObject;
        behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
        BehaviorManager.Instance.Register (behaviorAgent);
        behaviorAgent.StartBehavior ();
    }

	protected Node OpenGate () {
		return new Sequence (
			new LeafAssert (() => Vector3.Distance (gate.transform.position, guest.transform.position) < 20.0f),
			new LeafAssert (() => !gateOpened),
			npcBehavior.NPCBehavior_Stop (),
			new Selector (
				new Sequence (
					new LeafAssert (() => Vector3.Distance (gate.transform.position, gameObject.transform.position) > 40.0f),
					npcBehavior.NPCBehavior_GoTo (gateHandle.transform, true)
				),
				new Sequence (
					new LeafAssert (() => Vector3.Distance (gate.transform.position, gameObject.transform.position) <= 40.0f),
					npcBehavior.NPCBehavior_GoTo (gateHandle.transform, false)
				)
			),
			npcBehavior.NPCBehavior_DoGesture (GESTURE_CODE.GRAB_FRONT),
			new LeafInvoke (() => gate.GetComponent <Animation> ().Play ("gate open")),
			new LeafInvoke (() => gateOpened = true)
		);
	}

	protected Node CloseGate () {
		return new Sequence (
			new LeafAssert (() => Vector3.Distance (gate.transform.position, guest.transform.position) > 22.0f),
			new LeafAssert (() => gateOpened),
			npcBehavior.NPCBehavior_Stop (),
			new Selector (
				new Sequence (
					new LeafAssert (() => Vector3.Distance (gate.transform.position, gameObject.transform.position) > 40.0f),
					npcBehavior.NPCBehavior_GoTo (gateHandle.transform, true)
				),
				new Sequence (
					new LeafAssert (() => Vector3.Distance (gate.transform.position, gameObject.transform.position) <= 40.0f),
					npcBehavior.NPCBehavior_GoTo (gateHandle.transform, false)
				)
			),
			npcBehavior.NPCBehavior_DoGesture (GESTURE_CODE.GRAB_FRONT),
			new LeafInvoke (() => gate.GetComponent <Animation> ().Play ("gate close")),
			new LeafInvoke (() => gateOpened = false)
		);
	}

	protected Node StandGuard () {
		return new Sequence (
			npcBehavior.NPCBehavior_GoTo (guardPosition.transform, false),
			npcBehavior.NPCBehavior_OrientTowards (gate.transform.position)
		);
	}

	protected Node ClickMove () {
		return new Sequence (
			new LeafAssert (() => selectionIndicator.activeInHierarchy),
			new LeafAssert (() => Input.GetMouseButtonDown (1)),
			new LeafAssert (() => Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000.0f)),
			npcBehavior.NPCBehavior_GoTo (Val.V(() => hit.point), true)
		);
	}

	protected Node BuildTreeRoot () {
		return new DecoratorLoop (
			new DecoratorForceStatus (
				RunStatus.Success,
				new Selector (
					ClickMove (),
					OpenGate (),
					CloseGate (),
					StandGuard ()
				)
			)
		);
	}
}
/*
Need to:
Fix up animations for opening/closing gate
Complete wander method (or integrate with nav mechanic)
Is guest object preset? May need to restructure code around gate
*/