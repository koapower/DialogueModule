namespace DialogueModule
{
    public abstract class CommandBase
    {
        public readonly CommandID id;
        public readonly StringGridRow rowData;
        public bool isWaiting { get; private set; } = false;

        protected CommandBase(CommandID id, StringGridRow row)
        {
            this.id = id;
            this.rowData = row;
        }

        public abstract void Execute(DialogueEngine engine);
    }
}