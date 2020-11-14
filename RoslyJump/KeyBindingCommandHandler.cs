using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace RoslyJump
{
    [Export(typeof(ICommandHandler))]
    [ContentType("text")]
    [Name("KeyBindingTest")]
    class KeyBindingCommandHandler : ICommandHandler<TypeCharCommandArgs>
    {
        public string DisplayName => "RoslyJump";

        public bool ExecuteCommand(TypeCharCommandArgs args, CommandExecutionContext executionContext)
        {
            if (args.TypedChar == '+')
            {
                bool alreadyAdorned = args.TextView.Properties.TryGetProperty(
                    "KeyBindingTextAdorned", out bool adorned) && adorned;
                if (!alreadyAdorned)
                {
                    new TextAdornment1((IWpfTextView)args.TextView);
                    args.TextView.Properties.AddProperty("KeyBindingTextAdorned", true);
                }
            }

            return false;
        }

        public CommandState GetCommandState(TypeCharCommandArgs args)
        {
            return CommandState.Unspecified;
        }
    }
}
