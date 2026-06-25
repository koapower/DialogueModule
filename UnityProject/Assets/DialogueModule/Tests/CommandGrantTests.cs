using System.Collections.Generic;
using NUnit.Framework;

namespace DialogueModule.Tests
{
    public class CommandGrantTests
    {
        private static (GridInfo grid, StringGridRow row) BuildRow(params string[] cells)
        {
            var grid = new StringGrid("TestSheet");
            grid.AddRow(new StringGridRow(new List<string> { "Command", "Arg1", "Arg2", "Arg3", "Text" }));
            grid.AddRow(new StringGridRow(new List<string>(cells)));
            return (grid.CreateGridInfo(), grid.GetRow(1));
        }

        [Test]
        public void Execute_FiresOnGrantRequested_WithParsedTypeIdAndQuantity()
        {
            var (grid, row) = BuildRow("Grant", "item", "potion_001", "2", "");
            var command = new CommandGrant(grid, row);

            // DialogueEngine resolves adapter/dataManager/scenarioManager via
            // GetComponent<T>() on its own GameObject, so all three must live
            // on the same GameObject as the engine for Execute() to find them.
            var engineGo = new UnityEngine.GameObject("TestEngine");
            engineGo.AddComponent<DataManager>();
            engineGo.AddComponent<ScenarioManager>();
            var adapter = engineGo.AddComponent<ScenarioUIAdapter>();
            var engine = engineGo.AddComponent<DialogueEngine>();

            string capturedType = null;
            string capturedId = null;
            int capturedQty = -1;
            adapter.onGrantRequested += (type, id, qty) =>
            {
                capturedType = type;
                capturedId = id;
                capturedQty = qty;
            };

            command.Execute(engine);

            Assert.AreEqual("item", capturedType);
            Assert.AreEqual("potion_001", capturedId);
            Assert.AreEqual(2, capturedQty);

            UnityEngine.Object.DestroyImmediate(engineGo);
        }

        [Test]
        public void Constructor_DefaultsQuantityToOne_WhenArg3IsBlank()
        {
            var (grid, row) = BuildRow("Grant", "gold", "", "", "");
            var command = new CommandGrant(grid, row);

            Assert.AreEqual("gold", command.GrantType);
            Assert.AreEqual("", command.GrantId);
            Assert.AreEqual(1, command.Quantity);
        }
    }
}
