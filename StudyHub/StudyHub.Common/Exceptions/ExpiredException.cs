﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Common.Exceptions;
public class ExpiredException : Exception
{
    public ExpiredException(string? message) 
        : base(message) { }
}
