using System.Collections;
using UnityEngine;

namespace DialogueModule
{
    [AddComponentMenu("Dialogue System/Dialogue Engine")]
    public class DialogueEngine : MonoBehaviour
    {
        internal DataManager dataManager => GetComponent<DataManager>();

        IEnumerator Init()
        {
            dataManager.Init();
            yield return null;
        }

    }
}