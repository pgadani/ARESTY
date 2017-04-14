using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NPC {

	public class NPCIO : MonoBehaviour {
		
		// IO parameters
		public List<GameObject> selected;
		private NPCCamController g_Camera;
		NPCControlManager g_NPCControlManager;
		bool g_SelectDragging = false;

		// UI Parameters
		public Text gTxtLabel;
		public Dropdown gDropdown;
		public Button gSubmit;
		public bool Debug = true;

		private float gMouseDragSmootFactor = 2.0f;

		void Start() {
			g_Camera = FindObjectOfType<NPCCamController>();
			g_NPCControlManager = FindObjectOfType<NPCControlManager>();
			if (g_NPCControlManager == null) {
				UnityEngine.Debug.Log("NPCIO --> Using NPCIO withou a NPCControlManager");
			}
			selected = new List<GameObject>();
			SetupUI();
		}

		private void ClearSelected() {
			foreach (GameObject g in selected) {
				g.GetComponent<NPCController>().SetSelected(false);
			}
			selected.Clear();
		}
		
		// leave directional controls for camera
		public enum INPUT_KEY {
			MODIFIER = KeyCode.LeftShift, 		// looking
			MODIFIER_SEC = KeyCode.LeftControl, // running
			CONTEXT_ACTION = KeyCode.Mouse1, 	// secondary mouse button
			SELECT_AGENT = KeyCode.Mouse0, 		// primary mouse button
			STOP = KeyCode.Space				// stop
		};
	
		// Update is called by NPCControlManager
		public void UpdateIO () {
			UpdateKeys();
			UpdateUI();
		}

		private void UpdateKeys() {
			// select agent
			if (!EventSystem.current.IsPointerOverGameObject() && Input.GetKeyDown((KeyCode)INPUT_KEY.SELECT_AGENT)) {
				if (g_SelectDragging) {
					// TODO
				}
				else {
					RaycastHit hitInfo = new RaycastHit();
					bool clickedOn = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
					if (clickedOn) {
						// handle clicking selection
						GameObject player = hitInfo.transform.gameObject;
						NPCController npc = player.GetComponent<NPCController>();
						
						// ctrl+click for multiselect (find a better way?)
						if (!Input.GetKey((KeyCode)INPUT_KEY.MODIFIER_SEC)) {
							// deselect
							ClearSelected();
						}
						if (npc != null) {
							// toggle selection of player
							if (selected.Contains(player)) {
								selected.Remove(player);
								npc.SetSelected(false);
							}
							else {
								selected.Add(player);
								npc.SetSelected(true);
							}
						}
						UpdateBehaviors();
					}
				}
			}

			// other context action - stopping current movement
			if (Input.GetKey((KeyCode)INPUT_KEY.STOP)) {
				foreach (GameObject g in selected) {
					g.GetComponent<NPCController>().StopNavigation();
				}
			}
			// context actions - walking/running to location
			else if (Input.GetKeyDown((KeyCode)INPUT_KEY.CONTEXT_ACTION)) {
				RaycastHit hitInfo = new RaycastHit();
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
					if (Input.GetKey((KeyCode)INPUT_KEY.MODIFIER)) {
						foreach (GameObject g in selected) {
							g.GetComponent<NPCController>().OrientTowards(hitInfo.point);
						}
					} 
					else if (Input.GetKey((KeyCode)INPUT_KEY.MODIFIER_SEC)) {
						foreach (GameObject g in selected) {
							g.GetComponent<NPCController>().RunTo(hitInfo.point);
						}
					}
					else {
						foreach (GameObject g in selected) {
							g.GetComponent<NPCController>().GoTo(hitInfo.point);
						}
					}
				}
			}

		}

		private void SetupUI() {
			gTxtLabel = GetComponentInChildren<Text>();
			gDropdown = GetComponentInChildren<Dropdown>();
			gSubmit = GetComponentInChildren<Button>();
			gSubmit.onClick.AddListener(Submit);
			UpdateBehaviors();
		}

		// activate currently selected behavior
		private void Submit() {
			Dictionary<String, Action> behaviors = new Dictionary<String, Action>{
				{"Peddler (1)", () => (new Peddler().Init(selected))}, // make better trees
				{"Argument (1)", () => (new Argument().Init(selected))},
				{"Tag (2)", () => (new Tag().Init(selected))},
				{"Opening Gates (3)", () => (new GuardGate().Init(selected))},
				{"Conversation (any)", () => (new Conversation().Init(selected))},
				{"Market (2+)", () => (new Market().Init(selected))}
			};
			try {
				String opt = gDropdown.options[gDropdown.value].text;
				behaviors[opt]();
			}
			catch (System.Exception e) {
				UnityEngine.Debug.Log("Invalid selection "+e);
			}
		}

		// Update is called once per frame
		private void UpdateUI() {
			try {
				if (Debug) {
					if (g_Camera != null) {
						CameraInfo();
					}
				}
			} 
			catch (System.Exception e) {
				UnityEngine.Debug.Log("NPCUIController --> Failed at updating text field: " + e.Message + " - disabling component");
			}
		}

		// call this when character selection changes to update GUI options
		// updates dropdown list of menus based on number of selected characters
		// TODO: add additional parameters for behavior trees
		private void UpdateBehaviors() {
			// Make this code more modular when there are more behaviors
			// Map number of selected characters to list of behaviors, use with existing map of behaviors to functions
			gDropdown.ClearOptions();
			if (selected.Count == 0) {
				gSubmit.enabled = false; // disable creation of behaviors with no players
			}
			else {
				gSubmit.enabled = true;
				List<String> options = new List<String>();
				if (selected.Count == 1) {
					// Peddler and Argument
					options.Add("Peddler (1)");
					options.Add("Argument (1)");
				}
				else if (selected.Count == 2) {
					options.Add("Tag (2)");
					options.Add("Market (2+)");
				}
				else if (selected.Count == 3) {
					options.Add("Opening Gates (3)");
					options.Add("Market (2+)");
				}
				options.Add("Conversation (any)");
				gDropdown.AddOptions(options);
			}
		}

		public void SetText(Text t) {
			gTxtLabel = t;
		}

		// Displays Camera Data
		void CameraInfo() {
			gTxtLabel.text = "Camera Mode: ";
			switch (g_Camera.CurrentMode) {
				case NPCCamController.CAMERA_MODE.FIRST_PERSON:
					gTxtLabel.text += "First Person";
					break;
				case NPCCamController.CAMERA_MODE.THIRD_PERSON:
					gTxtLabel.text += "Third Person";
					break;
				case NPCCamController.CAMERA_MODE.FREE:
					gTxtLabel.text += "Free Flight";
					break;
			}
		}

	}

}
