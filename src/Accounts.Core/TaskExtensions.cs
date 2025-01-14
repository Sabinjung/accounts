﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Extensions
{
    public static class TaskEx
    {
        public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }
    }
}
