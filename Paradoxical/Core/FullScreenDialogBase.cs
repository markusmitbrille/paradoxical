using Paradoxical.View;

namespace Paradoxical.Core;

public abstract class FullScreenDialogViewModelBase : DialogViewModelBase
{
    public override string DialogIdentifier => MainWindow.ROOT_DIALOG_IDENTIFIER;
}
