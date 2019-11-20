﻿// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Mmm.Platform.IoT.StorageAdapter.WebService.Wrappers
{
    public class GuidKeyGenerator : IKeyGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
