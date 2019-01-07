﻿using System;
using System.Threading.Tasks;
using eFormAPI.Web.Abstractions.Eforms;
using eFormAPI.Web.Infrastructure;
using eFormAPI.Web.Infrastructure.Models;
using eFormAPI.Web.Infrastructure.Models.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace eFormAPI.Web.Controllers.Eforms
{
    [Authorize]
    public class TemplatesController : Controller
    {
        private readonly ITemplatesService _templatesService;

        public TemplatesController(ITemplatesService templatesService)
        {
            _templatesService = templatesService;
        }

        [HttpPost]
        [Authorize(Policy = AuthConsts.EformPolicies.Eforms.Read)]
        public async Task<IActionResult> Index([FromBody] TemplateRequestModel templateRequestModel)
        {
            try
            {
                return Ok(await _templatesService.Index(templateRequestModel));
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Authorize(Policy = AuthConsts.EformPolicies.Eforms.Read)]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(_templatesService.Get(id));
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Authorize(Policy = AuthConsts.EformPolicies.Eforms.Create)]
        public OperationResult Create([FromBody] EFormXmlModel eFormXmlModel)
        {
            return _templatesService.Create(eFormXmlModel);
        }

        [HttpGet]
        [Authorize(Policy = AuthConsts.EformPolicies.Eforms.Delete)]
        public OperationResult Delete(int id)
        {
            return _templatesService.Delete(id);
        }

        [HttpPost]
        [Authorize(Policy = AuthConsts.EformPolicies.Eforms.PairingUpdate)]
        public OperationResult Deploy([FromBody] DeployModel deployModel)
        {
            return _templatesService.Deploy(deployModel);
        }
    }
}