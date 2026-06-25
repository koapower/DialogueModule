# DialogueModule Engine Additions Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add `CommandGrant`, a typewriter speed tag, and AV trigger tags to DialogueModule so the host project (HolyHell) can build a reward bridge on top, without DialogueModule ever referencing host-project concepts.

**Architecture:** Two independent, pure-C# additions: (1) a new `CommandGrant : CommandBase` that fires a typed `ScenarioUIAdapter` event carrying raw `(type, id, qty)` strings — no interpretation of what they mean; (2) an `InlineMarkerParser` that strips `<speed=...>`/`<fx=...>`/`<sfx=...>` tags out of dialogue text and returns position-tagged markers, consumed by `DialogueWindow`'s existing typewriter coroutine to vary reveal speed and fire effect/SFX events at the right character index.

**Tech Stack:** Unity 6000.3.2f1, C# (no async libraries used in this repo), `com.unity.test-framework` (already in `Packages/manifest.json`, not yet wired to any assembly).

## Global Constraints

- Namespace stays `DialogueModule` for every new file (matches every existing file in this repo).
- No new external package dependencies.
- DialogueModule must not reference anything outside its own `Assets/DialogueModule/Scripts` tree — it doesn't know what "item", "gold", or "card" mean, only that it grants `(type, id, qty)`.
- Existing public API of `ScenarioUIAdapter`, `MessageData`, `CommandBase` subclasses must remain source-compatible (this repo has no other consumers yet, but the port plan in HolyHell will read these exact signatures — don't rename without checking the HolyHell plan).

---

### Task 1: Test asmdef scaffold + `CommandGrant`

**Files:**
- Create: `UnityProject/Assets/DialogueModule/Scripts/DialogueModule.Runtime.asmdef`
- Create: `UnityProject/Assets/DialogueModule/Tests/DialogueModule.Tests.asmdef`
- Create: `UnityProject/Assets/DialogueModule/Tests/CommandGrantTests.cs`
- Create: `UnityProject/Assets/DialogueModule/Scripts/Command/Logic/CommandGrant.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Command/CommandID.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Command/CommandFactory.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/ScenarioUIAdapter.cs`

**Interfaces:**
- Consumes: `StringGridRow` ([GameLib/StringGrid/StringGridRow.cs](C:\Users\kelen\K\DialogueModule\UnityProject\Assets\DialogueModule\Scripts\GameLib\StringGrid\StringGridRow.cs)), `StringGrid`, `GridInfo`, `DataParser.GetCell(GridInfo, StringGridRow, ColumnName)`, `CommandBase(CommandID id, StringGridRow row)` constructor.
- Produces: `CommandID.Grant`, `class CommandGrant : CommandBase` with public ctor `CommandGrant(GridInfo grid, StringGridRow row)`, `ScenarioUIAdapter.onGrantRequested` event of type `Action<string, string, int>` — Task 4's HolyHell-side bridge subscribes to this exact signature.

There is no existing `Tests` folder or test asmdef in this repo (confirmed: `com.unity.test-framework` is in the manifest but unused). This task creates the minimal scaffold alongside the first real test, rather than as a separate empty setup task.

- [ ] **Step 1: Create the runtime asmdef so an editor-only test assembly can reference it**

Create `UnityProject/Assets/DialogueModule/Scripts/DialogueModule.Runtime.asmdef`:

```json
{
    "name": "DialogueModule.Runtime",
    "rootNamespace": "DialogueModule",
    "references": [
        "Unity.TextMeshPro",
        "Unity.InputSystem"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

Unity will regenerate the `.meta` file automatically; do not hand-write it.

- [ ] **Step 2: Create the test asmdef**

Create `UnityProject/Assets/DialogueModule/Tests/DialogueModule.Tests.asmdef`:

```json
{
    "name": "DialogueModule.Tests",
    "rootNamespace": "DialogueModule.Tests",
    "references": [
        "DialogueModule.Runtime",
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": true,
    "precompiledReferences": [
        "nunit.framework.dll"
    ],
    "autoReferenced": false,
    "defineConstraints": [
        "UNITY_INCLUDE_TESTS"
    ],
    "versionDefines": [],
    "noEngineReferences": false
}
```

- [ ] **Step 3: Add `Grant` to the command ID enum**

Modify `UnityProject/Assets/DialogueModule/Scripts/Command/CommandID.cs`:

```csharp
namespace DialogueModule
{
    public enum CommandID
    {
        UNKNOWN = 0,
        Character,
        CharacterOff,
        Text,

        Selection,
        SelectionEnd,
        Jump,
        EndScenario,
        ScenarioLabel,
        Grant,
    }
}
```

- [ ] **Step 4: Write the failing test for `CommandGrant` parsing**

Create `UnityProject/Assets/DialogueModule/Tests/CommandGrantTests.cs`:

```csharp
using System.Collections.Generic;
using NUnit.Framework;

namespace DialogueModule.Tests
{
    public class CommandGrantTests
    {
        private static (GridInfo grid, StringGridRow row) BuildRow(params string[] cells)
        {
            var grid = new StringGrid("TestSheet");
            grid.AddRow(new StringGridRow(new List<string> { "Command", "Arg1", "Arg2", "Arg3", "Text" }));
            grid.AddRow(new StringGridRow(new List<string>(cells)));
            return (grid.CreateGridInfo(), grid.GetRow(1));
        }

        [Test]
        public void Execute_FiresOnGrantRequested_WithParsedTypeIdAndQuantity()
        {
            var (grid, row) = BuildRow("Grant", "item", "potion_001", "2", "");
            var command = new CommandGrant(grid, row);

            // DialogueEngine resolves adapter/dataManager/scenarioManager via
            // GetComponent<T>() on its own GameObject, so all three must live
            // on the same GameObject as the engine for Execute() to find them.
            var engineGo = new UnityEngine.GameObject("TestEngine");
            engineGo.AddComponent<DataManager>();
            engineGo.AddComponent<ScenarioManager>();
            var adapter = engineGo.AddComponent<ScenarioUIAdapter>();
            var engine = engineGo.AddComponent<DialogueEngine>();

            string capturedType = null;
            string capturedId = null;
            int capturedQty = -1;
            adapter.onGrantRequested += (type, id, qty) =>
            {
                capturedType = type;
                capturedId = id;
                capturedQty = qty;
            };

            command.Execute(engine);

            Assert.AreEqual("item", capturedType);
            Assert.AreEqual("potion_001", capturedId);
            Assert.AreEqual(2, capturedQty);

            UnityEngine.Object.DestroyImmediate(engineGo);
        }

        [Test]
        public void Constructor_DefaultsQuantityToOne_WhenArg3IsBlank()
        {
            var (grid, row) = BuildRow("Grant", "gold", "", "", "");
            var command = new CommandGrant(grid, row);

            Assert.AreEqual("gold", command.GrantType);
            Assert.AreEqual("", command.GrantId);
            Assert.AreEqual(1, command.Quantity);
        }
    }
}
```

- [ ] **Step 5: Run the test to verify it fails**

Run (PowerShell):
```
& "C:\Program Files\Unity\Hub\Editor\6000.3.2f1\Editor\Unity.exe" -batchmode -projectPath "C:\Users\kelen\K\DialogueModule\UnityProject" -runTests -testPlatform EditMode -testResults "C:\Users\kelen\K\DialogueModule\test-results.xml" -quit
```
Expected: compile error or test failure — `CommandGrant` does not exist yet, `onGrantRequested` does not exist yet.

- [ ] **Step 6: Implement `CommandGrant` and wire the adapter event**

Create `UnityProject/Assets/DialogueModule/Scripts/Command/Logic/CommandGrant.cs`:

```csharp
namespace DialogueModule
{
    class CommandGrant : CommandBase
    {
        public string GrantType { get; }
        public string GrantId { get; }
        public int Quantity { get; }

        public CommandGrant(GridInfo grid, StringGridRow row) : base(CommandID.Grant, row)
        {
            GrantType = DataParser.GetCell(grid, row, ColumnName.Arg1);
            GrantId = DataParser.GetCell(grid, row, ColumnName.Arg2);
            var qtyStr = DataParser.GetCell(grid, row, ColumnName.Arg3);
            Quantity = int.TryParse(qtyStr, out var parsed) ? parsed : 1;
        }

        public override void Execute(DialogueEngine engine)
        {
            engine.adapter.RequestGrant(GrantType, GrantId, Quantity);
        }
    }
}
```

Modify `UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/ScenarioUIAdapter.cs` — add alongside the other `event Action` declarations (after `onSetClickHandler`):

```csharp
        public event Action<string, string, int> onGrantRequested;
```

And add this method alongside `AddSelection`:

```csharp
        internal void RequestGrant(string type, string id, int quantity)
        {
            onGrantRequested?.Invoke(type, id, quantity);
        }
```

Modify `UnityProject/Assets/DialogueModule/Scripts/Command/CommandFactory.cs` — add inside the static constructor, after the `ScenarioLabel` registration:

```csharp
            Register(CommandID.Grant.ToStringFast(), (grid, row) => new CommandGrant(grid, row));
```

- [ ] **Step 7: Run the test to verify it passes**

Run the same command as Step 5.
Expected: PASS, 2 tests green, 0 failures. Check `test-results.xml` for `result="Passed"` on both `CommandGrantTests` cases.

- [ ] **Step 8: Commit**

```bash
git add UnityProject/Assets/DialogueModule/Scripts/DialogueModule.Runtime.asmdef* \
        UnityProject/Assets/DialogueModule/Tests/ \
        UnityProject/Assets/DialogueModule/Scripts/Command/Logic/CommandGrant.cs \
        UnityProject/Assets/DialogueModule/Scripts/Command/CommandID.cs \
        UnityProject/Assets/DialogueModule/Scripts/Command/CommandFactory.cs \
        UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/ScenarioUIAdapter.cs
git commit -m "feat: add CommandGrant and onGrantRequested event"
```

---

### Task 2: `InlineMarkerParser` (speed/fx/sfx tag extraction)

**Files:**
- Create: `UnityProject/Assets/DialogueModule/Scripts/RuntimeData/DialogueTagReplacement/InlineMarker.cs`
- Create: `UnityProject/Assets/DialogueModule/Scripts/RuntimeData/DialogueTagReplacement/InlineMarkerParser.cs`
- Create: `UnityProject/Assets/DialogueModule/Tests/InlineMarkerParserTests.cs`

**Interfaces:**
- Consumes: nothing from other tasks (pure string processing, no Unity API).
- Produces: `enum MarkerKind { Speed, Fx, Sfx }`, `struct InlineMarker { public int charIndex; public MarkerKind kind; public string value; }`, `static class InlineMarkerParser { public static string Parse(string input, out List<InlineMarker> markers); }`. Task 3 consumes this exact signature.

This finding changes the plan from the original design doc: `ScenarioUIAdapter.onEndScenario` **already exists and already fires** when a label's commands run out (`ScenarioPlayer.EndPlaying()` → `engine.adapter.EndScenario()` → `onEndScenario?.Invoke()`, see [ScenarioPlayer.cs:37-43](C:\Users\kelen\K\DialogueModule\UnityProject\Assets\DialogueModule\Scripts\Scenario\ScenarioPlayer.cs)). `CommandEndScenario` itself is a no-op, but nothing needs to be built to get a "scenario ended" notification — the HolyHell-side bridge (separate plan) should subscribe to the existing `onEndScenario`, not a new event. No task in this plan touches `CommandEndScenario`.

- [ ] **Step 1: Write the failing tests**

Create `UnityProject/Assets/DialogueModule/Tests/InlineMarkerParserTests.cs`:

```csharp
using System.Collections.Generic;
using NUnit.Framework;

namespace DialogueModule.Tests
{
    public class InlineMarkerParserTests
    {
        [Test]
        public void Parse_PlainText_ReturnsUnchangedWithNoMarkers()
        {
            var clean = InlineMarkerParser.Parse("hello world", out var markers);

            Assert.AreEqual("hello world", clean);
            Assert.AreEqual(0, markers.Count);
        }

        [Test]
        public void Parse_SpeedTag_StripsTagAndRecordsMarkerAtStripPosition()
        {
            var clean = InlineMarkerParser.Parse("slow<speed=0.2>fast", out var markers);

            Assert.AreEqual("slowfast", clean);
            Assert.AreEqual(1, markers.Count);
            Assert.AreEqual(4, markers[0].charIndex);
            Assert.AreEqual(MarkerKind.Speed, markers[0].kind);
            Assert.AreEqual("0.2", markers[0].value);
        }

        [Test]
        public void Parse_FxAndSfxTags_RecordBothInOrder()
        {
            var clean = InlineMarkerParser.Parse("a<fx=flashback_white>b<sfx=heartbeat>c", out var markers);

            Assert.AreEqual("abc", clean);
            Assert.AreEqual(2, markers.Count);
            Assert.AreEqual(1, markers[0].charIndex);
            Assert.AreEqual(MarkerKind.Fx, markers[0].kind);
            Assert.AreEqual("flashback_white", markers[0].value);
            Assert.AreEqual(2, markers[1].charIndex);
            Assert.AreEqual(MarkerKind.Sfx, markers[1].kind);
            Assert.AreEqual("heartbeat", markers[1].value);
        }

        [Test]
        public void Parse_NullInput_ReturnsEmptyStringAndNoMarkers()
        {
            var clean = InlineMarkerParser.Parse(null, out var markers);

            Assert.AreEqual(string.Empty, clean);
            Assert.AreEqual(0, markers.Count);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:
```
& "C:\Program Files\Unity\Hub\Editor\6000.3.2f1\Editor\Unity.exe" -batchmode -projectPath "C:\Users\kelen\K\DialogueModule\UnityProject" -runTests -testPlatform EditMode -testResults "C:\Users\kelen\K\DialogueModule\test-results.xml" -quit
```
Expected: compile error — `InlineMarkerParser`, `InlineMarker`, `MarkerKind` do not exist yet.

- [ ] **Step 3: Implement `InlineMarker` and `InlineMarkerParser`**

Create `UnityProject/Assets/DialogueModule/Scripts/RuntimeData/DialogueTagReplacement/InlineMarker.cs`:

```csharp
namespace DialogueModule
{
    public enum MarkerKind
    {
        Speed,
        Fx,
        Sfx,
    }

    public struct InlineMarker
    {
        public int charIndex;
        public MarkerKind kind;
        public string value;
    }
}
```

Create `UnityProject/Assets/DialogueModule/Scripts/RuntimeData/DialogueTagReplacement/InlineMarkerParser.cs`:

```csharp
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogueModule
{
    public static class InlineMarkerParser
    {
        private static readonly Regex markerRegex = new Regex(@"<(speed|fx|sfx)=([^>]+)>", RegexOptions.Compiled);

        public static string Parse(string input, out List<InlineMarker> markers)
        {
            markers = new List<InlineMarker>();
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var output = new StringBuilder();
            int lastIndex = 0;

            foreach (Match match in markerRegex.Matches(input))
            {
                output.Append(input, lastIndex, match.Index - lastIndex);
                lastIndex = match.Index + match.Length;

                var kind = match.Groups[1].Value switch
                {
                    "speed" => MarkerKind.Speed,
                    "fx" => MarkerKind.Fx,
                    "sfx" => MarkerKind.Sfx,
                    _ => MarkerKind.Speed,
                };

                markers.Add(new InlineMarker
                {
                    charIndex = output.Length,
                    kind = kind,
                    value = match.Groups[2].Value,
                });
            }

            output.Append(input, lastIndex, input.Length - lastIndex);
            return output.ToString();
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run the same command as Step 2.
Expected: PASS, 4 tests green.

- [ ] **Step 5: Commit**

```bash
git add UnityProject/Assets/DialogueModule/Scripts/RuntimeData/DialogueTagReplacement/InlineMarker.cs \
        UnityProject/Assets/DialogueModule/Scripts/RuntimeData/DialogueTagReplacement/InlineMarkerParser.cs \
        UnityProject/Assets/DialogueModule/Tests/InlineMarkerParserTests.cs
git commit -m "feat: add InlineMarkerParser for speed/fx/sfx dialogue tags"
```

---

### Task 3: Wire markers into `MessageData`, `ScenarioUIAdapter`, and `DialogueWindow`

**Files:**
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/MessageData.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/ScenarioUIAdapter.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Command/Graphic/CommandText.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/Command/Graphic/CommandCharacter.cs`
- Modify: `UnityProject/Assets/DialogueModule/Scripts/UI/DialogueWindow.cs`
- Create: `UnityProject/Assets/DialogueModule/Tests/MessageDataMarkerTests.cs`

**Interfaces:**
- Consumes: `InlineMarkerParser.Parse(string, out List<InlineMarker>)` from Task 2; `ScenarioUIAdapter.onGrantRequested` is unrelated and untouched.
- Produces: `ScenarioUIAdapter.PlayText(string, string, AudioClip, float, List<InlineMarker>)` (new 5-arg overload), `ScenarioUIAdapter.onEffectTriggered : Action<string>`, `ScenarioUIAdapter.onSfxTriggered : Action<string>`. These three are what the HolyHell-side VFX/audio bridge subscribes to.

The typewriter timing loop in `DialogueWindow.TypeRoutine` cannot be meaningfully unit-tested in EditMode (it depends on `Time.deltaTime` across frames and TextMeshPro mesh layout, both of which require Play Mode). This task unit-tests the data plumbing up to `MessageData` and documents a manual Play Mode check for the visual behavior — do not write a fake test that doesn't actually exercise the coroutine.

- [ ] **Step 1: Write the failing test for marker plumbing into `MessageData`**

Create `UnityProject/Assets/DialogueModule/Tests/MessageDataMarkerTests.cs`:

```csharp
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DialogueModule.Tests
{
    public class MessageDataMarkerTests
    {
        [Test]
        public void PlayText_WithMarkers_PopulatesMessageDataMarkers()
        {
            var go = new GameObject("TestAdapter");
            var adapter = go.AddComponent<ScenarioUIAdapter>();
            MessageData captured = default;
            adapter.onPlayText += data => captured = data;

            var markers = new List<InlineMarker>
            {
                new InlineMarker { charIndex = 2, kind = MarkerKind.Sfx, value = "heartbeat" },
            };

            adapter.PlayText("Alice", "hi", null, 1f, markers);

            Assert.AreSame(markers, captured.markers);

            Object.DestroyImmediate(go);
        }
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run:
```
& "C:\Program Files\Unity\Hub\Editor\6000.3.2f1\Editor\Unity.exe" -batchmode -projectPath "C:\Users\kelen\K\DialogueModule\UnityProject" -runTests -testPlatform EditMode -testResults "C:\Users\kelen\K\DialogueModule\test-results.xml" -quit
```
Expected: compile error — no 5-argument `PlayText` overload exists, `MessageData.markers` does not exist.

- [ ] **Step 3: Extend `MessageData` and `ScenarioUIAdapter`**

Modify `UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/MessageData.cs`:

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    public struct MessageData
    {
        public string name;
        public string message;
        public AudioClip voiceClip;
        public float voiceSpeedMultiplier;
        public bool hasNameCardColor;
        public Color nameCardColor;
        public List<InlineMarker> markers;
    }
}
```

Modify `UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/ScenarioUIAdapter.cs`:

Replace the existing `PlayText` method with an overload that defaults `markers` to keep the old 4-arg call sites compiling, plus the two new events:

```csharp
        public event Action<string> onEffectTriggered;
        public event Action<string> onSfxTriggered;

        public void PlayText(string characterDisplayName, string fullText, AudioClip voiceClip, float voiceSpeedMultiplier)
            => PlayText(characterDisplayName, fullText, voiceClip, voiceSpeedMultiplier, null);

        public void PlayText(string characterDisplayName, string fullText, AudioClip voiceClip, float voiceSpeedMultiplier, List<InlineMarker> markers)
        {
            currentLine.Value = fullText;
            controllerStatus = ControllerStatus.TypingText;
            onPlayText?.Invoke(new MessageData()
            {
                name = characterDisplayName,
                message = fullText,
                voiceClip = voiceClip,
                voiceSpeedMultiplier = voiceSpeedMultiplier,
                markers = markers,
            });
        }

        internal void TriggerEffect(string fxId) => onEffectTriggered?.Invoke(fxId);
        internal void TriggerSfx(string sfxId) => onSfxTriggered?.Invoke(sfxId);
```

Remove the old single `PlayText` method body (the one being replaced) — there must be exactly one 4-arg and one 5-arg overload after this edit, not two definitions of the 4-arg one.

- [ ] **Step 4: Run the test to verify it passes**

Run the same command as Step 2.
Expected: PASS.

- [ ] **Step 5: Route `CommandText` and `CommandCharacter` through `InlineMarkerParser`**

Modify `UnityProject/Assets/DialogueModule/Scripts/Command/Graphic/CommandText.cs` — replace the body of `Execute`:

```csharp
        public override void Execute(DialogueEngine engine)
        {
            engine.adapter.characterAdapter.HideLayer("");
            var tagParsedText = engine.dataManager.ParseDialogueText(textContent);
            var cleanText = InlineMarkerParser.Parse(tagParsedText, out var markers);
            engine.adapter.PlayText("", cleanText, null, 1f, markers);
            isWaiting = true;
        }
```

Modify `UnityProject/Assets/DialogueModule/Scripts/Command/Graphic/CommandCharacter.cs` — replace the block starting at `var parsedText = engine.dataManager.ParseDialogueText(textContent);` through the `engine.adapter.PlayText(...)` call:

```csharp
                var tagParsedText = engine.dataManager.ParseDialogueText(textContent);
                var cleanText = InlineMarkerParser.Parse(tagParsedText, out var markers);
                AudioClip voiceClip = null;
                if (!string.IsNullOrEmpty(characterSettingData.voiceFileName))
                {
                    if (engine.assetManager.CurrentUsingAssetDict.TryGetValue(characterSettingData.voiceFileName, out var audioObj))
                        voiceClip = audioObj as AudioClip;
                    else
                        Debug.LogError($"Cannot find character voice clip in assetmanager! fileName: {characterSettingData.voiceFileName}, characterID {characterId}");
                }

                engine.adapter.PlayText(
                    characterSettingData.displayName,
                    cleanText,
                    voiceClip,
                    characterSettingData.voiceSpeedMultiplier <= 0f ? 1f : characterSettingData.voiceSpeedMultiplier,
                    markers);
                isWaiting = true;
```

- [ ] **Step 6: Consume markers in `DialogueWindow`'s typewriter loop**

Modify `UnityProject/Assets/DialogueModule/Scripts/UI/DialogueWindow.cs`.

Add two fields near the existing private fields (after `private MessageData pendingMessage;`):

```csharp
        private System.Collections.Generic.List<InlineMarker> currentMarkers;
        private int nextMarkerIndex = 0;
```

In `DisplayMessage`, after the existing `typeWaitTime = Mathf.Max(typeWaitTime, MinClipWaitTime);` line and before `StopAllCoroutines();`, add:

```csharp
            currentMarkers = data.markers;
            nextMarkerIndex = 0;
```

In `TypeRoutine`, inside the `for (int i = 0; i < steps && visibleCharacterCount < targetCharacterCount; i++)` loop, immediately after the existing `TryPlayVoiceTick(visibleCharacterCount - 1);` line, add:

```csharp
                    ApplyDueMarkers();
```

Add a new private method (place it after `TryPlayVoiceTick`):

```csharp
        private void ApplyDueMarkers()
        {
            if (currentMarkers == null)
                return;

            while (nextMarkerIndex < currentMarkers.Count && currentMarkers[nextMarkerIndex].charIndex <= visibleCharacterCount)
            {
                var marker = currentMarkers[nextMarkerIndex];
                switch (marker.kind)
                {
                    case MarkerKind.Speed:
                        if (float.TryParse(marker.value, System.Globalization.CultureInfo.InvariantCulture, out var seconds))
                            typeWaitTime = Mathf.Max(seconds, MinTypeWaitTime);
                        break;
                    case MarkerKind.Fx:
                        adapter?.TriggerEffect(marker.value);
                        break;
                    case MarkerKind.Sfx:
                        adapter?.TriggerSfx(marker.value);
                        break;
                }
                nextMarkerIndex++;
            }
        }
```

Also clear `currentMarkers` in `Clear()` (after `voicePlaybackCooldown = 0f;`):

```csharp
            currentMarkers = null;
            nextMarkerIndex = 0;
```

- [ ] **Step 7: Run the full EditMode suite to verify nothing broke**

Run:
```
& "C:\Program Files\Unity\Hub\Editor\6000.3.2f1\Editor\Unity.exe" -batchmode -projectPath "C:\Users\kelen\K\DialogueModule\UnityProject" -runTests -testPlatform EditMode -testResults "C:\Users\kelen\K\DialogueModule\test-results.xml" -quit
```
Expected: PASS, all tests from Tasks 1-3 green.

- [ ] **Step 8: Manual Play Mode verification (not automatable — record the result in the commit message body)**

Open the project in the Unity Editor, enter Play Mode with any scene that has a `DialogueEngine` + bound `DialogueWindow`. Author a temporary scenario row with `Text` = `slow<speed=0.2>fast<fx=test_fx><sfx=test_sfx>end` and confirm:
- The text up to "fast" types at the default speed, then visibly slows down for "fast...end".
- `onEffectTriggered`/`onSfxTriggered` fire at the right character (add a temporary `Debug.Log` subscriber if no UI hook exists yet).

Remove the temporary test row before committing.

- [ ] **Step 9: Commit**

```bash
git add UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/MessageData.cs \
        UnityProject/Assets/DialogueModule/Scripts/Logic/UIAdapter/ScenarioUIAdapter.cs \
        UnityProject/Assets/DialogueModule/Scripts/Command/Graphic/CommandText.cs \
        UnityProject/Assets/DialogueModule/Scripts/Command/Graphic/CommandCharacter.cs \
        UnityProject/Assets/DialogueModule/Scripts/UI/DialogueWindow.cs \
        UnityProject/Assets/DialogueModule/Tests/MessageDataMarkerTests.cs
git commit -m "feat: consume speed/fx/sfx markers in DialogueWindow typewriter"
```

---

### Task 4: Push to `koapower/DialogueModule`

**Files:** none (git operations only).

- [ ] **Step 1: Create a feature branch from main and confirm the three task commits are on it**

```bash
git checkout -b feature/grant-and-display-polish
git log --oneline -8
```
Expected: see the three commits from Tasks 1-3 (asmdef/CommandGrant, InlineMarkerParser, marker wiring) at the top of the log.

- [ ] **Step 2: Push the branch**

```bash
git push -u origin feature/grant-and-display-polish
```
Expected: branch appears on `https://github.com/koapower/DialogueModule`. Do not open a PR or merge to `main` as part of this task — leave that decision to the user.

## Self-Review Notes

- **Spec coverage:** all four DialogueModule-side items from the design doc are covered — `CommandGrant` (Task 1), AV trigger tags and typewriter speed (Tasks 2-3). The fourth item (`onScenarioEnded`) turned out to already exist as `ScenarioUIAdapter.onEndScenario`; documented in Task 2 instead of building a duplicate.
- **Type consistency:** `onGrantRequested : Action<string,string,int>`, `PlayText(..., List<InlineMarker>)`, `onEffectTriggered`/`onSfxTriggered : Action<string>` are the exact names/signatures the HolyHell-side plan will subscribe to — do not rename during implementation without updating that plan.
- **No placeholders:** every step has runnable code and an exact test-runner command.
