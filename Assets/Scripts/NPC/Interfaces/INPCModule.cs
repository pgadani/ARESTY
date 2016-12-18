using UnityEngine;
using System.Collections;

namespace NPC {
    
    public enum NPC_MODULE_TARGET {
        BODY,
        PERCEPTION,
        AI
    }

    public enum NPC_MODULE_TYPE {
<<<<<<< HEAD
        PATHFINDER
    }

    // Anything which implements this interface might be a module of the NPC
=======
        PATHFINDER,
        BEHAVIOR,
        EXPLORATION
    }

    // Anything which implements this interface might be a module of the NPC
>>>>>>> e0987625e7ad608f43f81d2f93a9d7d2ef0ad0aa
    public interface INPCModule {

        bool IsUpdateable();

        void TickModule();

        bool IsEnabled();

        void SetEnable(bool e);

        void RemoveNPCModule();

        NPC_MODULE_TYPE NPCModuleType();

        NPC_MODULE_TARGET NPCModuleTarget();

        string NPCModuleName();

    }
}
