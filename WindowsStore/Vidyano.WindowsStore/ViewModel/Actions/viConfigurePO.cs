﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidyano.ViewModel.Actions
{
    class viConfigurePO : ActionBase
    {
        public viConfigurePO(Definition definition, PersistentObject parent, Query query) :
            base(definition, parent, query)
        {
            IsVisible = false;
        }
    }
}