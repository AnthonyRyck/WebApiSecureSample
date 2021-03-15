using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiSecureSample.Models;
using WebApiSecureSample.SecureApi;

namespace WebApiSecureSample.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MyConnectController : Controller
	{
		private readonly IJwtAuthenticationManager jwtAuthenticationManager;

		public MyConnectController(IJwtAuthenticationManager jwtAuthentication)
		{
			jwtAuthenticationManager = jwtAuthentication;
		}

		[HttpPost("authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] UserCredential userCred)
		{
			var token = await jwtAuthenticationManager.Authenticate(userCred.Login, userCred.Password);
			if (token == null)
				return Unauthorized();
			
			return Ok(token);
		}

	}
}
