using Microsoft.Xrm.Sdk;
using System;

namespace kogerohu.Actions
{
    public class TextToUpperCase : IPlugin
    {
        /// <summary>
        /// Action to convert an input text to upper case and return
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var inputText = (string)executionContext.InputParameters["inputText"];

            if (string.IsNullOrWhiteSpace(inputText)) return;

            var outputText = inputText.ToUpper();

            executionContext.OutputParameters["outputText"] = outputText;
        }
    }
}
