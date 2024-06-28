﻿using Microsoft.AspNetCore.Mvc;

namespace LAS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        // Get api/sample/get
        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            var sample = new { Name = "Sample", Value = 1 };

            return Ok(sample);
        }

        // Post api/sample
        [HttpPost]
        [Route("")]
        public IActionResult Post()
        {
            return Ok("post");
        }
    }
}