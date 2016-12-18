<<<<<<< HEAD:Assets/Scripts/NPC/Components/NPCObstacle.cs
﻿using UnityEngine;
using System.Collections;
using System;

namespace NPC {

    public class NPCObstacle : MonoBehaviour, IPerceivable {

        public Vector3 CalculateAgentRepulsionForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public Vector3 CalculateAgentSlidingForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public Vector3 CalculateRepulsionForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public Vector3 CalculateSlidingForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public float GetAgentRadius() {
            return 0f;
        }

        public Vector3 GetCurrentVelocity() {
            return new Vector3(0f,0f,0f);
        }

        public Vector3 GetForwardDirection() {
            return transform.forward;
        }

        public PERCEIVEABLE_TYPE GetNPCEntityType() {
            return PERCEIVEABLE_TYPE.OBJECT;
        }

        public PERCEIVE_WEIGHT GetPerceptionWeightType() {
            Rigidbody rb = GetComponent<Rigidbody>();
            return PERCEIVE_WEIGHT.TOTAL;
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Transform GetTransform() {
            return transform;
        }
    }

=======
﻿using UnityEngine;
using System.Collections;
using System;

namespace NPC {

    public class NPCObstacle : MonoBehaviour, IPerceivable {

        public Vector3 CalculateAgentRepulsionForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public Vector3 CalculateAgentSlidingForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public Vector3 CalculateRepulsionForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public Vector3 CalculateSlidingForce(IPerceivable p) {
            throw new NotImplementedException();
        }

        public float GetAgentRadius() {
            return 0f;
        }

        public Vector3 GetCurrentVelocity() {
            return new Vector3(0f,0f,0f);
        }

        public Vector3 GetForwardDirection() {
            return transform.forward;
        }

        public Transform GetMainLookAtPoint() {
            return transform;
        }

        public PERCEIVEABLE_TYPE GetNPCEntityType() {
            return PERCEIVEABLE_TYPE.OBJECT;
        }

        public PERCEIVE_WEIGHT GetPerceptionWeightType() {
            Rigidbody rb = GetComponent<Rigidbody>();
            return PERCEIVE_WEIGHT.TOTAL;
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Transform GetTransform() {
            return transform;
        }
    }

>>>>>>> e0987625e7ad608f43f81d2f93a9d7d2ef0ad0aa:Assets/Scripts/NPC/Components/Subcomponents/NPCObstacle.cs
}