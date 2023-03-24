using Paradoxical.View;

namespace Paradoxical.Core;

public abstract class PopupViewModel : DialogViewModel
{
    public override string DialogIdentifier => MainWindow.ROOT_DIALOG_IDENTIFIER;
}
