﻿using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class InfoViewModel : PageViewModelBase
{
    public override string PageName => "Mod Info";

    public IDataService Data { get; }

    public InfoViewModel(IDataService data)
    {
        Data = data;
    }
}