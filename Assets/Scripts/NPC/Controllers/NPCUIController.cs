using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NPC {

	public class NPCUIController : MonoBehaviour {

		public Text gTxtLabel;
		public Dropdown gDropdown;
		public Button gSubmit;
		public NPCCamController NPCCamera;
		public NPCIO g_IO; // easier to just combine UI controller and IO, especially for functionality like choosing locations
		public bool Debug = true;


		public void SetupUI() {
			gTxtLabel = GetComponentInChildren<Text>();
			gDropdown = GetComponentInChildren<Dropdown>();
			gSubmit = GetComponentInChildren<Button>();
			gSubmit.onClick.AddListener(Submit);
		}

		// activate currently selected behavior
		public void Submit() {
			Dictionary<String, Action> behaviors = new Dictionary<String, Action>{
				{"Peddler (1)", () => (new Peddler().Init(g_IO.selected))}, // make better trees
				{"Argument (1)", () => (new Argument().Init(g_IO.selected))},
				{"Tag (2)", () => (new Tag().Init(g_IO.selected))},
				{"Conversation (any)", () => (new Conversation().Init(g_IO.selected))}
			};
			try {
				String opt = gDropdown.options[gDropdown.value].text;
				behaviors[opt]();
			}
			catch (System.Exception e) {
				UnityEngine.Debug.Log("Invalid selection");
			}
		}

		// Update is called once per frame
		public void UpdateUI() {
			try {
				if (Debug) {
					if (NPCCamera != null) {
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
		public void UpdateBehaviors() {
			// Make this code more modular when there are more behaviors
			// Map number of selected characters to list of behaviors, use with existing map of behaviors to functions
			gDropdown.ClearOptions();
			if (g_IO.selected.Count == 0) {
				gSubmit.enabled = false; // disable creation of behaviors with no players
			}
			else {
				gSubmit.enabled = true;
				List<String> options = new List<String>();
				options.Add("Conversation (any)");
				if (g_IO.selected.Count == 1) {
					// Peddler and Argument
					options.Add("Peddler (1)");
					options.Add("Argument (1)");
				}
				else if (g_IO.selected.Count == 2) {
					options.Add("Tag (2)");
				}
				gDropdown.AddOptions(options);
			}
		}

		public void SetText(Text t) {
			gTxtLabel = t;
		}

		// Displays Camera Data
		void CameraInfo() {
			gTxtLabel.text = "Camera Mode: ";
			switch (NPCCamera.CurrentMode) {
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
