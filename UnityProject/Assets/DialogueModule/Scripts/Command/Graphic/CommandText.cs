namespace DialogueModule
{
    class CommandText : CommandBase
    {
        string content;

        public CommandText(GridInfo grid, StringGridRow row) : base(CommandID.Text, row)
        {
            content = DataParser.GetCell(grid, row, ColumnName.Text);
        }

        public override void Execute(DialogueEngine engine)
        {
            engine.adapter.currentGlobalCharacter.Value = null;
            engine.adapter.currentLine.Value = content;
            engine.adapter.PlayText();
            isWaiting = true;
        }
    }
}