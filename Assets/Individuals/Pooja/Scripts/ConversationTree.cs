﻿using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using NPC;

public class ConversationTree : MonoBehaviour {

	public float threshold;
	public List<Transform> locations;
	public List<GameObject> conversants;
	private Vector3[] startingLocations;
	private BehaviorAgent ba;

	// Use this for initialization
	void Start () {
		int numc = conversants.Count;
		startingLocations = new Vector3[numc];
		for (int i = 0; i<numc; i++) {
			startingLocations[i] = conversants[i].GetComponent<Transform>().position;
		}
		ba = new BehaviorAgent(this.BuildTreeRoot());
		BehaviorManager.Instance.Register(ba);
		ba.StartBehavior();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected Node BuildTreeRoot() {
		int numl = locations.Count;
		int numc = conversants.Count;
		int numGestures = 2;
		Node[] parents = new Node[numl+numGestures+1];
		Node[] children = new Node[numc+1];
		for (int ci = 0; ci<numc; ci++) {
			children[ci] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_GoNear(locations[0], threshold, false);
		}
		children[numc] = new LeafTrace("Going to loc "+0);
		parents[0] = new SequenceParallel(children);

		children = new Node[2*numc+1];
		for (int ci = 0; ci<numc; ci++) {
			children[2*ci] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_LookAt(locations[numc-ci-1], true);
			children[2*ci+1] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_DoGesture(GESTURE_CODE.WAVE_HELLO, null, true);
		}
		children[2*numc] = new LeafTrace("Waving");
		parents[1] = new SequenceParallel(children);	

		for (int li = 1; li<numl; li++) {
			children = new Node[numc+1];
			for (int ci = 0; ci<numc; ci++) {
				children[ci] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_GoNear(locations[li], threshold, false);
			}
			children[numc] = new LeafTrace("Going to loc "+li);
			parents[li+1] = new SequenceParallel(children);
		}

		children = new Node[2*numc+1];
		for (int ci = 0; ci<numc; ci++) {
			children[2*ci] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_LookAt(locations[numc-ci-1], true);
			children[2*ci+1] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_DoGesture(GESTURE_CODE.WAVE_HELLO, null, true);
		}
		children[2*numc] = new LeafTrace("Waving");
		parents[numl+1] = new SequenceParallel(children);

		children = new Node[numc+1];
		for (int ci = 0; ci<numc; ci++) {
			children[ci] = conversants[ci].GetComponent<NPCBehavior>().NPCBehavior_GoTo(startingLocations[ci], false);
		}
		children[numc] = new LeafTrace("Returning to original locations");
		parents[numl+numGestures] = new SequenceParallel(children);		
		return new DecoratorLoop(new Sequence(parents));
	}
}



// Go to Meeting location
// Start talking, gesturing
// Walk around path while talking and gesturing
// Wave goodbye
// Go to original locations



// LATER
// UI should make it possible to set meeting location and points of path (also possibly use this to meet with other people?)