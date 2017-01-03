using UnityEngine;
using UnityEngine.AI;
using TreeSharpPlus;
using NPC;

public class SelectionController : MonoBehaviour {

	private NPCBehavior aiScript;

	void Awake() {
		
		aiScript = GetComponentInParent <NPCBehavior> ();
	}

	void Update() {
		
		if (Input.GetMouseButtonDown (1)) {
			
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000.0f)) {
				aiScript.NPCBehavior_Stop ();
				aiScript.NPCBehavior_GoTo (Val.V(() => hit.point), true);
			}
		}
			
	}
}

