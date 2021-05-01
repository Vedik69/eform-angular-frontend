﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;

namespace OuterInnerResource.Pn.Controllers
{
    [Authorize]
    public class OuterResourcesController : Controller
    {
        private readonly IOuterResourceService _outerResourceService;

        public OuterResourcesController(IOuterResourceService outerResourceService)
        {
            _outerResourceService = outerResourceService;
        }


        [HttpGet]
        [Route("api/outer-inner-resource-pn/outer-resources")]
        public async Task<OperationDataResult<OuterResourcesModel>> GetAllAreas(OuterResourceRequestModel requestModel)
        {
            return await _outerResourceService.Index(requestModel);
        }

        [HttpGet]
        [Route("api/outer-inner-resource-pn/outer-resources/{id}")]
        public async Task<OperationDataResult<OuterResourceModel>> GetSingleArea(int id)
        {
            return await _outerResourceService.Get(id);
        }

        [HttpPost]
        [Route("api/outer-inner-resource-pn/outer-resources")]
        public async Task<OperationResult> CreateArea([FromBody] OuterResourceModel model)
        {
            return await _outerResourceService.Create(model);
        }

        [HttpPut]
        [Route("api/outer-inner-resource-pn/outer-resources")]
        public async Task<OperationResult> UpdateArea([FromBody] OuterResourceModel model)
        {
            return await _outerResourceService.Update(model);
        }

        [HttpDelete]
        [Route("api/outer-inner-resource-pn/outer-resources/{id}")]
        public async Task<OperationResult> DeleteArea(int id)
        {
            return await _outerResourceService.Delete(id);
        }
    }
}