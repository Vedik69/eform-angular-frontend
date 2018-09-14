﻿using Microsoft.AspNetCore.Identity;

namespace eFormAPI.Database.Entities
{
    public class EformUserRole : IdentityUserRole<int>
    {
        public virtual EformUser User { get; set; }
        public virtual EformRole Role { get; set; }
    }
}