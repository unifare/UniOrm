﻿using MyProject.STS.Identity.Configuration.Intefaces;

namespace MyProject.STS.Identity.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "http://localhost:9000";
    }
}