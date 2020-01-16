﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frg2089.CSF.Core
{
    /// <summary>
    /// 字符串 值
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// 字符串值标记 为" RTS"或"WRTS"
        /// </summary>
        string ValueTag { get; }
        /// <summary>
        /// 字符串 值长度(Unicode 即 实际长度/2)
        /// </summary>
        int ValueLength { get; }
        /// <summary>
        /// 字符串 值字符串
        /// </summary>
        string ValueString { get; set; }
        /// <summary>
        /// 额外值长度(ASCII)
        /// </summary>
        int? ExtraValueLength { get; }
        /// <summary>
        /// 额外值字符串
        /// </summary>
        string ExtraValue { get; set; }
    }
}
