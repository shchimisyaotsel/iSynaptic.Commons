﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSynaptic.Commons
{
    public static class UnsafeCast
    {
        public static TDestination To<TSource, TDestination>(TSource source)
        {
            throw new NotImplementedException();
        }
    }
}