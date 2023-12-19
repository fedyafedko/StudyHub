﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtorAPI.Extensions;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO;

namespace StudyHub.Controllers;

[ApiController]
[Route("[controller]")]
public class InvitingUserController : Controller
{
    private readonly IUserInvitedService _userInvitingService;
    public InvitingUserController(IUserInvitedService adminService)
    {
        _userInvitingService = adminService;
    }

    [HttpPost("Teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> InviteStudents([FromBody]InviteStudentsRequest request)
    {
        await _userInvitingService.InviteStudentsAsync(request);
        return Ok();
    }

    [HttpPost("Admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteAsync(string email, string role)
    {
        await _userInvitingService.InviteAsync(email, role);
        return Ok();
    }
}
