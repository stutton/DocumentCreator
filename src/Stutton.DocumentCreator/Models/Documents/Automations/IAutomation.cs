﻿using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Documents.Automations
{
    public interface IAutomation
    {
        string Name { get; }

        string Description { get; }

        Task<IResponse> Execute();
    }
}