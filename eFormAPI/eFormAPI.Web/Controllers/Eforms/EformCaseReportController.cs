﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2021 Microting A/S

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

using System.IO;

namespace eFormAPI.Web.Controllers.Eforms
{
    using System.Text;
    using Infrastructure.Models.ReportEformCase;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microting.eFormApi.BasePn.Infrastructure.Models.API;
    using eFormAPI.Web.Abstractions.Eforms;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Reflection.Metadata;
    using Microsoft.AspNetCore.Http;

    [Authorize]
    public class EformCaseReportController : Controller
    {
        private readonly IEformCaseReportService _eformCaseReportService;
        public EformCaseReportController(IEformCaseReportService eformCaseReportService)
        {
            _eformCaseReportService = eformCaseReportService;
        }

        /// <summary>
        /// Get report case by eForm model
        /// </summary>
        /// <param name="eFormCaseReportRequesteFormId">request model</param>
        /// <returns>Report case by eForm model</returns>
        [HttpGet]
        [Route("api/templates/caseReport")]
        public async Task<OperationDataResult<EFormCasesReportModel>> GetReport([Required] EFormCaseReportRequest eFormCaseReportRequesteFormId)
        {
            return await _eformCaseReportService.GetReportEformCases(eFormCaseReportRequesteFormId);
        }

        /// <summary>
        /// Get report case file by eForm
        /// </summary>
        /// <param name="eFormCaseReportRequesteFormId">request model</param>
        /// <returns>Report file which cases by eForm</returns>
        [HttpGet]
        [Route("api/templates/caseReportWord")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Blob), StatusCodes.Status200OK, Type = typeof(Blob))]
        public async Task GetReportWord([Required] EFormCaseReportRequest eFormCaseReportRequesteFormId)
        {
            var result = await _eformCaseReportService.GenerateReportFile(eFormCaseReportRequesteFormId);
            const int bufferSize = 4086;
            var buffer = new byte[bufferSize];
            Response.OnStarting(async () =>
            {
                if (!result.Success)
                {
                    Response.ContentLength = result.Message.Length;
                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    var bytes = Encoding.UTF8.GetBytes(result.Message);
                    await Response.Body.WriteAsync(bytes, 0, result.Message.Length);
                    await Response.Body.FlushAsync();
                }
                else
                {
                    using (var wordStream = result.Model)
                    {
                        int bytesRead;
                        Response.ContentLength = wordStream.Length;
                        Response.ContentType =
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        while ((bytesRead = wordStream.Read(buffer, 0, buffer.Length)) > 0 &&
                               !HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            await Response.Body.WriteAsync(buffer, 0, bytesRead);
                            await Response.Body.FlushAsync();
                        }
                    }
                }
            });
        }
    }
}
