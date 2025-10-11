namespace DialogueModule
{
    class CommandCharacterOff : CommandBase
    {
        string specifiedCharacterId;
        float fadeTime;

        public CommandCharacterOff(GridInfo grid, StringGridRow row) : base(CommandID.CharacterOff, row)
        {
            specifiedCharacterId = row.GetCell(grid.GetColumnIndex(ColumnName.Arg1));
            if (!float.TryParse(row.GetCell(grid.GetColumnIndex(ColumnName.Arg2)), out fadeTime))
                fadeTime = 0.2f;
        }

        public override void Execute(DialogueEngine engine)
        {
            //todo
        }
    }
}