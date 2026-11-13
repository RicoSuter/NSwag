using Newtonsoft.Json.Linq;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;

namespace NSwag.ConsoleCore.Tests
{
    public class AngularResourceCommandTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void When_resource_setting_is_serialized_then_command_round_trips_it(bool enabled)
        {
            // Arrange
            var document = new NSwagDocument();
            var command = new OpenApiToTypeScriptClientCommand();
            command.Settings.GenerateAngularResourceMethods = enabled;
            document.CodeGenerators.OpenApiToTypeScriptClientCommand = command;

            // Act
            var json = document.ToJson();
            var restored = NSwagDocument.FromJson<NSwagDocument>(null, json);

            // Assert
            var data = JObject.Parse(json);
            var settings = (JObject)data["codeGenerators"]["openApiToTypeScriptClient"];
            Assert.Equal(enabled, settings["generateAngularResourceMethods"].Value<bool>());
            Assert.Equal(enabled, restored.CodeGenerators.OpenApiToTypeScriptClientCommand.Settings.GenerateAngularResourceMethods);

            settings.Remove("generateAngularResourceMethods");
            var withoutOption = NSwagDocument.FromJson<NSwagDocument>(null, data.ToString());
            Assert.False(withoutOption.CodeGenerators.OpenApiToTypeScriptClientCommand.Settings.GenerateAngularResourceMethods);
        }
    }
}
