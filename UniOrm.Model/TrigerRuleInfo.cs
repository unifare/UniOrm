﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm.Model
{
    public class TrigerRuleInfo
    {
        public TrigerRuleInfo()
        {
            AddTime = DateTime.Now;
        }
        public int Id { get; set; }
        public string RuleName { get; set; }
        public string Rule { get; set; }
        public string ComposityId { get; set; }
        public string HttpMethod { get; set; }
        public bool? IsEnable { get; set; }
        public string VersionNum { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
