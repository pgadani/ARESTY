﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace NPC {

    public class NPCController : MonoBehaviour, IPerceivable {

        #region Members

        [SerializeField]
        private NPCAI gAI;

        [SerializeField]
        private NPCBody gBody;

        [SerializeField]
        private NPCPerception gPerception;

        [SerializeField]
        Dictionary<string, INPCModule> g_NPCModules;

        [SerializeField]
        GameObject g_SelectedEffect;

        [SerializeField]
        private bool gMainAgent = false;

        [SerializeField]
        private bool gInitialized = false;

        private bool g_Selected;

        public bool DisplaySelectedHighlight = true;

        private static string SELECTION_EFFECT = "SelectionEffect";

        #endregion

        #region Properties

        [SerializeField]
        public bool TestNPC;

        [SerializeField]
        public Transform TestTargetLocation;

        public HashSet<IPerceivable> PerceivedEntities {
            get {
                return gPerception.PerceivedEntities;
            }
        }

        public HashSet<IPerceivable> PerceivedAgents {
            get {
                return gPerception.PerceivedAgents;
            }
        }

        [SerializeField]
        public bool DebugMode = true;
        
        public INPCModule[] NPCModules {
            get {
                if (g_NPCModules == null) return new INPCModule[0];
                INPCModule[] mods = new INPCModule[g_NPCModules.Count];
                g_NPCModules.Values.CopyTo(mods, 0);
                return mods;
            }
        }

        [SerializeField]
        public PERCEIVEABLE_TYPE EntityType;

        [SerializeField]
        public NPCPerception Perception {
            get { return gPerception; }
        }

        [SerializeField]
        public NPCAI AI {
            get { return gAI; }
        }

        [SerializeField]
        public NPCBody Body {
            get { return gBody; }
        }

        [SerializeField]
        public bool MainAgent {
            get { return gMainAgent; }
            set { gMainAgent = value; }
        }
        #endregion
       
        #region Public_Functions

        public  void Debug(string msg) {
            if(DebugMode) {
                UnityEngine.Debug.Log(msg);
            }
        }

        public void DebugLine(Vector3 from, Vector3 to, Color c) {
            if (DebugMode) {
                UnityEngine.Debug.DrawLine(from, to, c);
            }
        }

        public void LoadNPCModules() {
            if(g_NPCModules == null)
                g_NPCModules = new Dictionary<string, INPCModule>();
            INPCModule[] modules = gameObject.GetComponents<INPCModule>();
            foreach (INPCModule m in modules) {
                if (!ContainsModule(m)) {
                    Debug("Loading NPC Module -> " + m.NPCModuleName());
                    if (!AddNPCModule(m)) {
                        GameObject.DestroyImmediate((UnityEngine.Object)m);
                    }
                }
            }
        }

        public void RemoveNPCModule(INPCModule mod) {
            if(g_NPCModules.ContainsKey(mod.NPCModuleName()))
                g_NPCModules.Remove(mod.NPCModuleName());
        }

        public void SetSelected(bool sel) {
            g_Selected = sel;
            g_SelectedEffect.SetActive(sel && DisplaySelectedHighlight);
        }

        public bool ContainsModule(INPCModule mod) {
            return g_NPCModules != null && g_NPCModules.ContainsKey(mod.NPCModuleName());
        }

        public void StopNavigation() {
            gBody.StopNavigation();
        }

        public void RunTo(Vector3 t) {
            if (Vector3.Distance(t, gBody.TargetLocation) >= gBody.AgentRadius) {
                List<Vector3> path =  gAI.FindPath(t); 
                if (path.Count < 1) {
                    Debug("NPCController --> No path found to target location");
                } else {
                    gBody.RunTo(path);
                }
            }
        }
        
        public void GoTo(Vector3 t) {
            if (Vector3.Distance(t,gBody.TargetLocation) >= gBody.AgentRadius) {
                List<Vector3> path = gAI.FindPath(t);
                if (path.Count < 1) {
                    Debug("NPCController --> No path found to target location");
                } else {
                    gBody.GoTo(path);
                }
            }
        }

        public void OrientTowards(Vector3 t) {
            gBody.OrientTowards(t);
        }
        
        public bool AddNPCModule(INPCModule mod) {
            if (g_NPCModules == null) g_NPCModules = new Dictionary<string, INPCModule>();
            if (g_NPCModules.ContainsKey(mod.NPCModuleName())) return false;
            switch(mod.NPCModuleTarget()) {
                case NPC_MODULE_TARGET.AI:
                    gAI.SetNPCModule(mod);
                    break;
                case NPC_MODULE_TARGET.BODY:
                    break;
                case NPC_MODULE_TARGET.PERCEPTION:
                    break;
            }
            g_NPCModules.Add(mod.NPCModuleName(), mod);
            return true;
        }
        #endregion 

        #region Unity_Runtime

        void Awake () {
            LoadNPCModules();
            g_SelectedEffect = transform.FindChild(SELECTION_EFFECT).gameObject;
            SetSelected(MainAgent);
        }
	
        void FixedUpdate() {

            gPerception.UpdatePerception();
            gBody.UpdateBody();

            // Main NPC Modular updating call
            foreach(INPCModule m in g_NPCModules.Values) {
                if (m.IsUpdateable()) {
                    m.TickModule();
                }
            }
        }
        
	    void Update () {
            if(g_Selected) {
                g_SelectedEffect.transform.Rotate(gameObject.transform.up, 1.0f);        
            }
        }
        
        void Reset() {
            if(!gInitialized) {
                g_NPCModules = new Dictionary<string, INPCModule>();
                Debug("Creating NPCController");
                gMainAgent = false;
                if (GetComponent<NPCBody>() != null) DestroyImmediate(GetComponent<NPCBody>());
                if (GetComponent<NPCPerception>() != null) DestroyImmediate(GetComponent<NPCPerception>());
                InitializeNPCComponents();
                gInitialized = true;
            } else {
                Debug("Loading existing NPCController settings");
            }
        }

        #endregion

        #region Private_Functions

        private void InitializeNPCComponents() {
            gAI = gameObject.AddComponent<NPCAI>();
            gPerception = gameObject.AddComponent<NPCPerception>();
            gBody = gameObject.AddComponent<NPCBody>();
            CreateSelectedEffect();
            // hide flags
            gAI.hideFlags = HideFlags.HideInInspector;
            gBody.hideFlags = HideFlags.HideInInspector;
            gPerception.hideFlags = HideFlags.HideInInspector;
        }

        private void CreateSelectedEffect() {
            Material m = Resources.Load<Material>("Materials/NPCSelectedCircle");
            if (m != null) {
                g_SelectedEffect = GameObject.CreatePrimitive(PrimitiveType.Plane);
                g_SelectedEffect.transform.parent = gameObject.transform;
                g_SelectedEffect.layer = LayerMask.NameToLayer("Ignore Raycast");
                g_SelectedEffect.GetComponent<MeshCollider>().enabled = false;
                g_SelectedEffect.transform.localScale = new Vector3(0.15f, 1.0f, 0.15f);
                g_SelectedEffect.transform.rotation = transform.rotation;
                g_SelectedEffect.name = SELECTION_EFFECT;
                g_SelectedEffect.transform.localPosition = new Vector3(0f, 0.4f, 0f);
                g_SelectedEffect.AddComponent<MeshRenderer>();
                MeshRenderer mr = g_SelectedEffect.GetComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.material = m;
            } else {
                Debug("NPCController --> Couldn't load NPC materials, do not forget to add the to the Resources folder");
            }
        }

        #endregion

        #region IPerceivable

        public virtual PERCEIVE_WEIGHT GetPerceptionWeightType() {
            return PERCEIVE_WEIGHT.WEIGHTED;
        }

        public virtual Transform GetTransform() {
            return this.transform;
        }

        public Vector3 GetCurrentVelocity() {
            return gBody.Velocity;
        }

        public virtual Vector3 GetPosition() {
            return transform.position;
        }

        public Vector3 GetForwardDirection() {
            return transform.forward;
        }

        public float GetAgentRadius() {
            return gBody.AgentRadius;
        }

        public virtual PERCEIVEABLE_TYPE GetNPCEntityType() {
            return PERCEIVEABLE_TYPE.NPC;
        }

        public virtual Transform GetMainLookAtPoint() {
            return gBody.Head;
        }

        #endregion

    }

}
