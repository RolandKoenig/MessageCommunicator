﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IConnectionConfigViewService : IViewService
    {
        Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template, IEnumerable<ConnectionParameters> allConnections);
    }
}
