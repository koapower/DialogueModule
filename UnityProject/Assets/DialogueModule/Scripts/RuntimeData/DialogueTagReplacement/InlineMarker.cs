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
