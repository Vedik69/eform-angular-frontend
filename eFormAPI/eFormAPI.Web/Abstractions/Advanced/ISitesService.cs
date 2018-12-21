﻿using System.Collections.Generic;
using eFormAPI.Web.Infrastructure.Models;
using eFormShared;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace eFormAPI.Web.Abstractions.Advanced
{
    public interface ISitesService
    {
        OperationDataResult<List<SiteName_Dto>> Index();
        OperationDataResult<SiteName_Dto> Edit(int id);
        OperationResult Update(SiteNameModel siteNameModel);
        OperationResult Delete(int id);
    }
}