﻿using System;
using System.Collections.Generic;
using eFormAPI.Web.Abstractions;
using eFormAPI.Web.Abstractions.Advanced;
using eFormAPI.Web.Infrastructure.Models;
using eFormCore;
using eFormShared;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace eFormAPI.Web.Services
{
    public class SimpleSitesService : ISimpleSitesService
    {
        private readonly IEFormCoreService _coreHelper;
        private readonly ILocalizationService _localizationService;

        public SimpleSitesService(ILocalizationService localizationService, 
            IEFormCoreService coreHelper)
        {
            _localizationService = localizationService;
            _coreHelper = coreHelper;
        }

        public OperationDataResult<List<Site_Dto>> Index()
        {
            Core core = _coreHelper.GetCore();
            List<Site_Dto> siteDto = core.SiteReadAll(false);
            return new OperationDataResult<List<Site_Dto>>(true, siteDto);
        }

        public OperationResult Create(SimpleSiteModel simpleSiteModel)
        {
            Core core = _coreHelper.GetCore();
            string siteName = simpleSiteModel.UserFirstName + " " + simpleSiteModel.UserLastName;

            try
            {
                Site_Dto siteDto = core.SiteCreate(siteName, simpleSiteModel.UserFirstName, simpleSiteModel.UserLastName,
                    null);

                return siteDto != null
                    ? new OperationResult(true,
                        _localizationService.GetString("DeviceUserParamCreatedSuccessfully", siteDto.SiteName))
                    : new OperationResult(false, _localizationService.GetString("DeviceUserCouldNotBeCreated"));
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.InnerException.Message == "The remote server returned an error: (402) Payment Required.")
                    {
                        return new OperationResult(false, _localizationService.GetString("YouNeedToBuyMoreLicenses"));
                    }
                    else
                    {
                        return new OperationResult(false, _localizationService.GetString("DeviceUserCouldNotBeCreated"));
                    }
                }
                catch
                {
                    return new OperationResult(false, _localizationService.GetString("DeviceUserCouldNotBeCreated"));
                }
            }
        }

        public OperationDataResult<Site_Dto> Edit(int id)
        {
            Core core = _coreHelper.GetCore();
            Site_Dto siteDto = core.SiteRead(id);

            return siteDto != null
                ? new OperationDataResult<Site_Dto>(true, siteDto)
                : new OperationDataResult<Site_Dto>(false,
                    _localizationService.GetString("DeviceUserParamCouldNotBeEdited", id));
        }

        public OperationResult Update(SimpleSiteModel simpleSiteModel)
        {
            try
            {
                Core core = _coreHelper.GetCore();
                Site_Dto siteDto = core.SiteRead(simpleSiteModel.Id);
                if (siteDto.WorkerUid != null)
                {
                    Worker_Dto workerDto = core.Advanced_WorkerRead((int) siteDto.WorkerUid);
                    if (workerDto != null)
                    {
                        string fullName = simpleSiteModel.UserFirstName + " " + simpleSiteModel.UserLastName;
                        bool isUpdated = core.SiteUpdate(simpleSiteModel.Id, fullName, simpleSiteModel.UserFirstName,
                            simpleSiteModel.UserLastName, workerDto.Email);

                        return isUpdated
                            ? new OperationResult(true, _localizationService.GetString("DeviceUserUpdatedSuccessfully"))
                            : new OperationResult(false,
                                _localizationService.GetString("DeviceUserParamCouldNotBeUpdated", simpleSiteModel.Id));
                    }

                    return new OperationResult(false, _localizationService.GetString("DeviceUserCouldNotBeObtained"));
                }

                return new OperationResult(false, _localizationService.GetString("DeviceUserNotFound"));
            }
            catch (Exception)
            {
                return new OperationResult(false, _localizationService.GetString("DeviceUserCouldNotBeUpdated"));
            }
        }

        public OperationResult Delete(int id)
        {
            try
            {
                Core core = _coreHelper.GetCore();
                SiteName_Dto siteNameDto = core.Advanced_SiteItemRead(id);

                return core.SiteDelete(siteNameDto.SiteUId)
                    ? new OperationResult(true,
                        _localizationService.GetString("DeviceUserParamDeletedSuccessfully", siteNameDto.SiteName))
                    : new OperationResult(false,
                        _localizationService.GetString("DeviceUserParamCouldNotBeDeleted", siteNameDto.SiteName));
            }
            catch (Exception)
            {
                return new OperationResult(false, _localizationService.GetString("DeviceUserParamCouldNotBeDeleted", id));
            }
        }
    }
}