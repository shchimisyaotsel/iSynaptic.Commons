﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSynaptic.Commons.Data
{
    public interface IExodataBindingSource
    {
        IEnumerable<IExodataBinding> GetBindingsFor<TExodata, TSubject>(IExodataRequest<TSubject> request);
    }
}