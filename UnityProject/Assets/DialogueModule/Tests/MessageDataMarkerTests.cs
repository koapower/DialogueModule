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
