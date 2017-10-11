﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.Devices.Edge.Agent.Core
{
    public interface ICommandFactory
    {
        ICommand Create(IModuleWithIdentity module);

        ICommand Pull(IModule module);

        ICommand Update(IModule current, IModuleWithIdentity next);

        ICommand Remove(IModule module);

        ICommand Start(IModule module);

        ICommand Restart(IModule module);

        ICommand Stop(IModule module);

        ICommand Wrap(ICommand command);
    }
}
