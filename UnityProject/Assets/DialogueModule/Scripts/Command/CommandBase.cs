namespace DialogueModule
{
    public abstract class CommandBase
    {
        public StringGridRow rowData { get; set; }

        protected CommandBase(StringGridRow row)
        {
            this.rowData = row;
        }

        public abstract void Execute();
    }
}