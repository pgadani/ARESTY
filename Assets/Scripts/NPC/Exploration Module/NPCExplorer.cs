using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NPC;
using Pathfinding;
using System;

public class NPCExplorer : MonoBehaviour, INPCModule {

    #region Members
    public float UpdateInSeconds = 1;
    private long g_UpdateCycle;
    private Stopwatch g_Stopwatch;
    private NPCController g_NPCController;

    [SerializeField]
    private bool g_Enabled = true;
    private Dictionary<NavNode, float> g_NodeValues;
    private NavGrid g_Grid;
    #endregion

    #region Public_Functions
    #endregion

    #region Unity_Methods
    // Use this for initialization
    void Start () {
        g_NPCController = GetComponent<NPCController>();
        g_UpdateCycle = (long) (UpdateInSeconds * 1000);
        g_Stopwatch = System.Diagnostics.Stopwatch.StartNew();
        g_Stopwatch.Start();
        RaycastHit hit;
        if(Physics.Raycast(new Ray(transform.position + (transform.up * 0.2f), -1 * transform.up), out hit)) {
            g_Grid = hit.collider.GetComponent<NavGrid>();
            g_NPCController.Debug(this + " - Grid Initialized ok");
        }
        if (g_Grid == null) this.enabled = false;
    }
	
	/// <summary>
    /// No npc module should be updated from here but from its TickModule method
    /// which will be only called from the NPCController on FixedUpdate
    /// </summary>
	void Update () { }

    #endregion

    #region NPCModule
    public bool IsEnabled() {
        return g_Enabled;
    }

    public string NPCModuleName() {
        return "NavGrid Exporation";
    }

    public NPC_MODULE_TARGET NPCModuleTarget() {
        return NPC_MODULE_TARGET.AI;
    }

    public NPC_MODULE_TYPE NPCModuleType() {
        return NPC_MODULE_TYPE.EXPLORATION;
    }

    public void RemoveNPCModule() {
        // destroy components in memroy here
    }

    public void SetEnable(bool e) {
        g_Enabled = e;
    }

    public bool IsUpdateable() {
        return true;
    }

    public void TickModule() {
        if(g_Enabled) { 
            if(Tick()) {
                g_NPCController.Debug("Updating NPC Module: " + NPCModuleName());
            }
        }
    }
    #endregion

    #region Private_Functions
    private bool Tick() {
        if(g_Stopwatch.ElapsedMilliseconds > g_UpdateCycle) {
            g_Stopwatch.Reset();
            g_Stopwatch.Start();
            return true;
        }
        return false;
    }
    #endregion
}
