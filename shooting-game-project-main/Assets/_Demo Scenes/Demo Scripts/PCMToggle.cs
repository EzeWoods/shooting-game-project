using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityPBR.Old
{
    [System.Serializable]
    public class PCMToggle : MonoBehaviour
    {
        private PrefabChildManager pcm;
        public GameObject player;
        public bool findPlayerByTag = true;
        public float turnOnDistance = 10f;

        private List<bool> groupStatus = new List<bool>();

        private bool playerInRange = false;

        void Awake()
        {
            pcm = GetComponent<PrefabChildManager>();
            if (findPlayerByTag)
            {
                player = GameObject.FindWithTag("Player");
            }

            if (pcm != null) // Fix: Ensure pcm is not null
            {
                foreach (PrefabGroup group in pcm.prefabGroups)
                {
                    groupStatus.Add(group.isActive);
                }
            }
        }

        void Update()
        {
            if (pcm == null) // Fix: Use `== null` instead of `!pcm`
                return;

            if (ShouldToggleOn())
                ToggleOn(true);
            else if (ShouldToggleOff())
                ToggleOn(false);
        }

        private bool ShouldToggleOn()
        {
            if (player != null && Vector3.Distance(player.transform.position, transform.position) < turnOnDistance && !playerInRange)
            {
                playerInRange = true;
                return true;
            }

            return false;
        }

        private bool ShouldToggleOff()
        {
            if (player != null && Vector3.Distance(player.transform.position, transform.position) > turnOnDistance && playerInRange)
            {
                playerInRange = false;
                return true;
            }
            return false;
        }

        private void ToggleOn(bool on)
        {
            if (pcm == null)
                return;

            List<PrefabGroup> groups = new List<PrefabGroup>(pcm.prefabGroups); // Fix: Convert to List

            for (int i = 0; i < groups.Count; i++)
            {
                if (groupStatus[i])
                {
                    if (on)
                        pcm.ActivateGroup(i); // Fix: Ensure this method exists
                    else
                        pcm.DeactivateGroup(i); // Fix: Ensure this method exists
                }
            }
        }
    }

    public class PrefabGroup
    {
        public bool isActive;
    }

    public class PrefabChildManager : MonoBehaviour
    {
        public List<PrefabGroup> prefabGroups = new List<PrefabGroup>();

        // Fix: Add missing ActivateGroup and DeactivateGroup methods
        public void ActivateGroup(int index)
        {
            if (index >= 0 && index < prefabGroups.Count)
            {
                prefabGroups[index].isActive = true;
                Debug.Log($"Activated Group {index}");
            }
        }

        public void DeactivateGroup(int index)
        {
            if (index >= 0 && index < prefabGroups.Count)
            {
                prefabGroups[index].isActive = false;
                Debug.Log($"Deactivated Group {index}");
            }
        }
    }
}
