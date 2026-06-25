using DialogueModule;
using UnityEngine;

/// <summary>
/// Play Mode test harness for SampleScene. Auto-starts a scenario label on
/// Start and logs the events that have no visual representation yet
/// (CommandGrant, fx/sfx triggers) so they're observable in the Console.
/// Not part of the DialogueModule package — sample-scene-only.
/// </summary>
public class ExampleDialogueRunner : MonoBehaviour
{
    [SerializeField] private string startLabel = "story_test4";

    private DialogueEngine engine;

    private void Start()
    {
        engine = FindFirstObjectByType<DialogueEngine>();
        if (engine == null)
        {
            Debug.LogError("[ExampleDialogueRunner] No DialogueEngine found in scene.");
            return;
        }

        engine.adapter.onGrantRequested += OnGrantRequested;
        engine.adapter.onEffectTriggered += OnEffectTriggered;
        engine.adapter.onSfxTriggered += OnSfxTriggered;

        engine.StartDialogue(startLabel);
    }

    private void OnDestroy()
    {
        if (engine == null) return;
        engine.adapter.onGrantRequested -= OnGrantRequested;
        engine.adapter.onEffectTriggered -= OnEffectTriggered;
        engine.adapter.onSfxTriggered -= OnSfxTriggered;
    }

    private void OnGrantRequested(string type, string id, int qty)
    {
        Debug.Log($"[Grant] type={type} id={id} qty={qty}");
    }

    private void OnEffectTriggered(string fxId)
    {
        Debug.Log($"[fx] {fxId}");
    }

    private void OnSfxTriggered(string sfxId)
    {
        Debug.Log($"[sfx] {sfxId}");
    }
}
