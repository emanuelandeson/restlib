﻿using System;

namespace Master.Berest.Utils.Extesions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
    }
}
