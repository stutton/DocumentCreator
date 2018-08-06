using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Updating
{
    internal interface IUpdaterService
    {
        Task<IResponse<CheckForUpdateResult>> Update();
    }
}
