﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.UserName.Template
{
    [DataContract(Name = "UserNameField")]
    public class UserNameFieldTemplateDto : FieldTemplateDtoBase
    {
        [DataMember]
        public override string Name { get; set; }
        [DataMember]
        public string TextToReplace { get; set; }
    }
}
