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
