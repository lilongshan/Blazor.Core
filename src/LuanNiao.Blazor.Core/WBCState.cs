﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LuanNiao.Blazor.Core
{
    /// <summary>
    /// You can get the component tree, this tree will initial with the first WaveComponent
    /// </summary>
    public sealed class WBCState
    {

        private WBCState()
        { }
        public static WBCState Instance = new WBCState();
        private int _currentID = 0;
        public int GetID()
        {
            return System.Threading.Interlocked.Increment(ref _currentID);
        }
    }


}
