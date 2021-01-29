﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2020 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace eFormAPI.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microting.eForm.Infrastructure.Constants;
    using Microting.eFormApi.BasePn.Infrastructure.Models.API;
    using Abstractions;
    using Microting.eFormApi.BasePn.Abstractions;
    using Abstractions.Eforms;
    using Infrastructure.Models.ReportEformCase;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Extensions.Logging;

    public class EformCaseReportService: IEformCaseReportService
    {
        private readonly IEFormCoreService _coreHelper;
        private readonly ILocalizationService _localizationService;
        //private readonly BaseDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly ILogger<EformCaseReportService> _logger;
        //private readonly ICasePostBaseService _casePostBaseService;
        private readonly IWordService _wordService;

        public EformCaseReportService(
            IEFormCoreService coreHelper,
            IUserService userService,
            //ICasePostBaseService casePostBaseService,
            ILocalizationService localizationService,
            //BaseDbContext dbContext,
            ILogger<EformCaseReportService> logger,
            IWordService wordService)
        {
            _coreHelper = coreHelper;
            _localizationService = localizationService;
            //_dbContext = dbContext;
            _logger = logger;
            _userService = userService;
            //_casePostBaseService = casePostBaseService;
            _wordService = wordService;
        }

        public async Task<OperationDataResult<EFormCasesReportModel>> GetReportEformCases(EFormCaseReportRequest eFormCaseReportRequest)
        {
            var core = await _coreHelper.GetCore();
            var localeString = await _userService.GetCurrentUserLocale();
            var sdkDbContext = core.DbContextHelper.GetDbContext();
            var timeZoneInfo = await _userService.GetCurrentUserTimeZoneInfo();
            var language = sdkDbContext.Languages.Single(x => string.Equals(x.LanguageCode, localeString, StringComparison.CurrentCultureIgnoreCase));
            var template = await core.TemplateItemRead(eFormCaseReportRequest.TemplateId, language);
            if (template == null)
            {
                return new OperationDataResult<EFormCasesReportModel>(false, _localizationService.GetString("TemplateNotFound"));
            }

            var casesQueryable = sdkDbContext.Cases
                .Include(x => x.Worker)
                .Where(x => x.DoneAt != null)
                .Where(x => x.CheckListId == template.Id);
            
            if(eFormCaseReportRequest.DateFrom != null)
            {
                casesQueryable = casesQueryable.Where(x => x.DoneAt >= eFormCaseReportRequest.DateFrom);
            }

            if (eFormCaseReportRequest.DateTo != null)
            {
                casesQueryable = casesQueryable.Where(x => x.DoneAt <= eFormCaseReportRequest.DateTo);
            }

            var cases = casesQueryable.ToList();

            if (cases.Count <= 0)
            {
                return new OperationDataResult<EFormCasesReportModel>(false, _localizationService.GetString("CasesNotFound"));
            }

            var casesIds = cases.Select(x => x.Id).ToList();

            // Exclude field types: None, Picture, Audio, Movie, Signature, Show PDF, FieldGroup, SaveButton
            var excludedFieldTypeIds = new List<string>()
            {
                Constants.FieldTypes.None,
                Constants.FieldTypes.Picture,
                Constants.FieldTypes.Audio,
                Constants.FieldTypes.Movie,
                Constants.FieldTypes.Signature,
                Constants.FieldTypes.ShowPdf,
                Constants.FieldTypes.FieldGroup,
                Constants.FieldTypes.SaveButton
            };

            var templateName = sdkDbContext.CheckListTranslations.Single(x => x.CheckListId == template.Id).Text;
            var result = new EFormCasesReportModel()
            {
                TemplateName = templateName,
                TextHeaders = new EformReportHeaders()
                {
                    Header1 = template.ReportH1 ?? templateName,
                    Header2 = template.ReportH2 == template.ReportH1 ? null : template.ReportH2,
                    Header3 = template.ReportH3 == template.ReportH2 ? null : template.ReportH3,
                    Header4 = template.ReportH4 == template.ReportH3 ? null : template.ReportH4,
                    Header5 = template.ReportH5 == template.ReportH4 ? null : template.ReportH5,
                },
                DescriptionBlocks = new List<string>(),
                //Posts = new List<ReportEformCasePostModel>(),
                CasesList = new List<ReportEformCaseModel>(),
                ImageNames = new List<KeyValuePair<List<string>, List<string>>>(),
                ItemHeaders = new List<KeyValuePair<int, string>>(),
                FromDate = eFormCaseReportRequest.DateFrom,
                ToDate = eFormCaseReportRequest.DateTo,
            };

            var fields = await core.Advanced_TemplateFieldReadAll(template.Id, language);

            foreach (var fieldDto in fields)
            {
                if (fieldDto.FieldType == Constants.FieldTypes.None)
                {
                    result.DescriptionBlocks.Add(fieldDto.Label);
                }
                if (!excludedFieldTypeIds.Contains(fieldDto.FieldType))
                {
                    var kvp = new KeyValuePair<int, string>(fieldDto.Id, fieldDto.Label);

                    result.ItemHeaders.Add(kvp);
                }
            }

            foreach (var oneCase in cases)
            {

                var imagesForEform = await sdkDbContext.FieldValues
                        .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                        .Where(x => x.Field.FieldTypeId == 5)
                        .Where(x => casesIds.Contains((int)x.CaseId))
                        .OrderBy(x => x.CaseId)
                        .ToListAsync();

                foreach (var imageField in imagesForEform)
                {
                    if (imageField.UploadedDataId != null)
                    {
                        var bla = cases.Single(x => x.Id == imageField.CaseId);
                        if (bla.DoneAt != null)
                        {
                            var doneAt = (DateTime)bla.DoneAt;
                            doneAt = TimeZoneInfo.ConvertTimeFromUtc(doneAt, timeZoneInfo);
                            var label = $"{imageField.CaseId} - {doneAt:yyyy-MM-dd HH:mm:ss};";
                            var geoTag = "";
                            if (!string.IsNullOrEmpty((imageField.Latitude)))
                            {
                                geoTag =
                                    $"https://www.google.com/maps/place/{imageField.Latitude},{imageField.Longitude}";
                            }

                            var keyList = new List<string> {imageField.CaseId.ToString(), label};
                            var list = new List<string>();
                            var uploadedData =
                                await sdkDbContext.UploadedDatas.SingleAsync(x => x.Id == imageField.UploadedDataId);
                            list.Add(uploadedData.FileName);
                            list.Add(geoTag);
                            result.ImageNames.Add(new KeyValuePair<List<string>, List<string>>(keyList, list));
                        }
                    }
                }

                // posts
                /*var casePostRequest = new CasePostsRequestCommonModel
                {
                    Offset = 0,
                    PageSize = int.MaxValue,
                    TemplateId = oneCase.CheckListId,
                };

                var casePostListResult = await _casePostBaseService.GetCommonPosts(casePostRequest);

                if (!casePostListResult.Success)
                {
                    return new OperationDataResult<EFormCasesReportModel>(
                        false,
                        casePostListResult.Message);
                }

                foreach (var casePostCommonModel in casePostListResult.Model.Entities)
                {
                    result.Posts.Add(new ReportEformCasePostModel
                    {
                        CaseId = casePostCommonModel.CaseId,
                        PostId = casePostCommonModel.PostId,
                        Comment = casePostCommonModel.Text,
                        SentTo = casePostCommonModel.ToRecipients,
                        SentToTags = casePostCommonModel.ToRecipientsTags,
                        PostDate = casePostCommonModel.PostDate
                    });
                }*/

                // add cases
                foreach (var caseDto in cases.OrderBy(x => x.DoneAt))
                {
                    var reportEformCaseModel = new ReportEformCaseModel
                    {
                        Id = caseDto.Id,
                        MicrotingSdkCaseId = caseDto.Id,
                        MicrotingSdkCaseDoneAt = TimeZoneInfo.ConvertTimeFromUtc((DateTime)caseDto.DoneAt, timeZoneInfo),
                        EFormId = caseDto.CheckListId,
                        DoneBy = $"{caseDto.Worker.FirstName} {caseDto.Worker.LastName}",
                    };

                    var fieldValues = sdkDbContext.FieldValues
                        .Where(x => x.CaseId == oneCase.Id)
                        .Include(x => x.Field)
                        .Include(x => x.Field.FieldType)
                        .AsNoTracking()
                        .ToList();


                    foreach (var itemHeader in result.ItemHeaders)
                    {
                        var caseField = fieldValues
                            .FirstOrDefault(x => x.FieldId == itemHeader.Key);

                        if (caseField != null)
                        {
                            switch (caseField.Field.FieldType.Type)
                            {
                                case Constants.FieldTypes.MultiSelect:
                                    reportEformCaseModel.CaseFields.Add(caseField.Value.Replace("|", "<br>"));
                                    break;
                                case Constants.FieldTypes.EntitySearch:
                                case Constants.FieldTypes.EntitySelect:
                                case Constants.FieldTypes.SingleSelect:
                                    reportEformCaseModel.CaseFields.Add(caseField.Value);
                                    break;
                                default:
                                    reportEformCaseModel.CaseFields.Add(caseField.Value);
                                    break;
                            }
                        }
                    }

                    reportEformCaseModel.ImagesCount = await sdkDbContext.FieldValues
                        .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                        .Where(x => x.Field.FieldTypeId == 5)
                        .Where(x => x.CaseId == caseDto.Id)
                        .Select(x => x.Id)
                        .CountAsync();

                    //reportEformCaseModel.PostsCount = casePostListResult.Model.Entities
                    //    .Where(x => x.CaseId == caseDto.Id)
                    //    .Select(x => x.PostId)
                    //    .Count();

                    result.CasesList.Add(reportEformCaseModel);
                }
            }

            return new OperationDataResult<EFormCasesReportModel>(true, result);
        }

        public async Task<OperationDataResult<Stream>> GenerateReportFile(EFormCaseReportRequest model)
        {
            try
            {
                var reportDataResult = await GetReportEformCases(model);
                if (!reportDataResult.Success)
                {
                    return new OperationDataResult<Stream>(false, reportDataResult.Message);
                }

                var wordDataResult = await _wordService
                    .GenerateWordDashboard(reportDataResult.Model);

                if (!wordDataResult.Success)
                {
                    return new OperationDataResult<Stream>(false, wordDataResult.Message);
                }

                return new OperationDataResult<Stream>(true, wordDataResult.Model);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<Stream>(
                    false,
                    _localizationService.GetString("ErrorWhileGeneratingReportFile"));
            }
        }
    }
}
