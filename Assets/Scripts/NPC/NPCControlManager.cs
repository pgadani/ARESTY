﻿using UnityEngine;
using System.Collections;

namespace NPC {
	/// <summary>
	/// 
	/// The NPCControlManager is the main set of [IO, UI and Camera] controllers for the scene's NPCs.
	/// 
	/// To set up the controller:
	/// 1) Before anything:
	///	 Ensure there exist a Camera with tag "MainCamera" (default)
	/// 2) Create an empty GameObject in the scene - for example "NPC Controllers"
	/// 3) Add the NPCControlManager to the "NPC Controllers" object.
	///	 - The camera will be set as a child of the "NPC Controllers" game object.
	/// 4) If you want to enable the UI, add a Canvas as a child of the "NPC Controllers"
	///	and add it to the NPCControlManager Canvas public member from the inspector.
	///	You can also create a secondary Canvas as the context menu one. Set its camera to be
	///	the main one.
	/// 
	/// </summary>
	public class NPCControlManager : MonoBehaviour {

		#region Members
		NPCController NPCController = null;
		public Canvas MainCanvas;
		public Canvas ContextCanvas;
		private static string mNPCIO = "NPC IO";
		private static string mNPCUI = "NPC UI";
		#endregion Members

		#region Properties
		public bool EnableIOController = true;
		public bool EnableCameraController = true;
		#endregion
		
		NPCCamController g_NPCCamera;
		NPCIO g_IO;

		#region Unity_Functions

		void Reset() {
			
			if (FindObjectsOfType<NPCControlManager>().Length > 1)
				throw new System.Exception("NPCControlManager --> ERROR - a NPCControlManager has already been added!");

			// CAM
			Transform cam = Camera.main.transform;
			Camera.main.nearClipPlane = 0.001f;
			if (cam.gameObject.GetComponent<NPCCamController>() == null) {
				cam.gameObject.AddComponent<NPCCamController>();
			}
			cam.parent = this.transform;

			// UI
			// Canvas canvas = FindObjectOfType<Canvas>();
			// if (canvas != null) {
			// 	GameObject io = null;
			// 	if (GetComponent<NPCUIController>() == null) {
			// 		io = new GameObject();
			// 		io.AddComponent<NPCUIController>();
			// 	}
			// 	io.transform.parent = this.transform;
			// 	canvas.transform.SetParent(io.transform);
			// 	io.name = mNPCUI;
			// }

			// IO
			GameObject npcIO = new GameObject();
			npcIO.name = mNPCIO;
			npcIO.AddComponent<NPCIO>();
			npcIO.transform.parent = transform;

		}

		void Awake () {
			try {
				transform.rotation = Quaternion.identity;
				// FindMainNPC();
				g_NPCCamera = Camera.main.GetComponent<NPCCamController>();
				g_IO = GetComponentInChildren<NPCIO>();

				// if (NPCController != null) {
				// 	g_IO.SetTarget(NPCController);
				// 	g_NPCCamera.SetTarget(NPCController);
				// 	g_NPCCamera.UpdateCameraMode(NPCCamController.CAMERA_MODE.THIRD_PERSON);
				// }
			} catch(System.Exception e) {
				Debug.Log("NPCControlManager --> Components missing from the controller, please add them. Disabling controller: " + e.Message);
				this.enabled = false;
			}
		}

		void FixedUpdate() {
			// if (EnableCameraController) {
			// 	if (Input.GetKey((KeyCode)CAMERA_MODE_KEYS.FREE_FLIGHT)) {
			// 		g_NPCCamera.UpdateCameraMode(NPCCamController.CAMERA_MODE.FREE);
			// 	} else if (Input.GetKey((KeyCode)CAMERA_MODE_KEYS.THIRD_PERSON)) {
			// 		g_NPCCamera.UpdateCameraMode(NPCCamController.CAMERA_MODE.THIRD_PERSON);
			// 	} else if (Input.GetKey((KeyCode)CAMERA_MODE_KEYS.FIRST_PERSON)) {
			// 		g_NPCCamera.UpdateCameraMode(NPCCamController.CAMERA_MODE.FIRST_PERSON);
			// 	} else if (Input.GetKey((KeyCode)CAMERA_MODE_KEYS.ISOMETRIC)) {
			// 		g_NPCCamera.UpdateCameraMode(NPCCamController.CAMERA_MODE.ISOMETRIC);
			// 	}
			// }

			if (EnableIOController) {
				g_IO.UpdateIO();
			}
			if (EnableCameraController) {
				g_NPCCamera.UpdateCamera();
			}

		}
		#endregion

	}

}
