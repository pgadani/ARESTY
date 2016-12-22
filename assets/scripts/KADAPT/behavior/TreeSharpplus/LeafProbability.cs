using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;
using Random = System.Random;
using Debug = UnityEngine.Debug;

namespace TreeSharpPlus
{
    public class LeafProbability : Node
    {

        //protected Runstatus r;
        protected float p;
        
        public LeafProbability(Val<float> probability)//, RunStatus result)
        {
            this.p = probability.Value;
            //this.r = result;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override sealed IEnumerable<RunStatus> Execute()
        {
            while (true)
            {
                Random rand = new Random();
                int sel = rand.Next(1, 10000001);
                if (sel <= p*10000001)
                {
                    Debug.Log("suc");
                    yield return RunStatus.Success;
                }
                Debug.Log("fail");
                yield return RunStatus.Failure;
            }
        }
    }
}