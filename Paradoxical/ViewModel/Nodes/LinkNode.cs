using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public sealed class LinkNode : ObservableNode<LinkViewModel>
{
    public EventLeaf? eventNode;
    public EventLeaf? EventNode
    {
        get => eventNode;
        set => SetProperty(ref eventNode, value);
    }

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public override IEnumerable<Node> Children
    {
        get
        {
            if (EventNode != null)
            {
                yield return EventNode;
            }
        }
    }
}
