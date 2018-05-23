﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Fields
{
    public interface IFieldFactoryService
    {
        IResponse<IField> CreateField(Type fieldKey);
        IResponse<Dictionary<string, Type>> GetAllFieldKeys();
    }
}
