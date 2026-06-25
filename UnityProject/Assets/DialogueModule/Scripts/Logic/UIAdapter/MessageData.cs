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
