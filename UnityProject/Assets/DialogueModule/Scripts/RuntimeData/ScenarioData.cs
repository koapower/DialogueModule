namespace DialogueModule
{
    class ScenarioData
    {
        public string name { get; private set; }
        public StringGrid grid { get; private set; }

        public ScenarioData(StringGrid grid)
        {
            name = grid.Name;
            this.grid = grid;
        }

        public void Init()
        {
            // parse commands
        }
    }
}