namespace DialogueModule
{
    class CommandCharacter : CommandBase
    {
        protected CharacterSettings settings;
        protected string layerName;
        protected float fadeTime;

        public CommandCharacter(GridInfo grid, StringGridRow row) : base(CommandID.Character, row)
        {
            //todo
        }

        public override void Execute(DialogueEngine engine)
        {
            //todo
        }
    }
}