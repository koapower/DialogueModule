namespace DialogueModule
{
    class CommandText : CommandBase
    {
        string textContent;

        public CommandText(GridInfo grid, StringGridRow row) : base(CommandID.Text, row)
        {
            textContent = DataParser.GetCell(grid, row, ColumnName.Text);
        }

        public override void Execute(DialogueEngine engine)
        {
            engine.adapter.characterAdapter.HideLayer("");
            var tagParsedText = engine.dataManager.ParseDialogueText(textContent);
            var cleanText = InlineMarkerParser.Parse(tagParsedText, out var markers);
            engine.adapter.PlayText("", cleanText, null, 1f, markers);
            isWaiting = true;
        }
    }
}
