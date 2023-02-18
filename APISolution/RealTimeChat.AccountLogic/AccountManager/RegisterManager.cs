﻿using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RealTimeChat.AccountLogic.Enums;
using RealTimeChat.AccountLogic.Interfaces;
using RealTimeChat.AccountLogic.Models;

namespace RealTimeChat.AccountLogic.AccountManager;


public class RegisterManager : IRegisterManager
{

    private SignInManager<IdentityUser> _signInManager;
    private UserManager<IdentityUser> _userManager;
    private IAccountValidator AccountValidator { get;  }
    private SignInManager<IdentityUser> SignInManager
    {
        get
        {
            //TODO: Implement custom exception
            if (_signInManager == null)
                throw new Exception("Server Error");

            return _signInManager;
        }
    }
    private UserManager<IdentityUser> UserManager
    {
        get
        {
            //TODO: Implement custom exception
            if (_signInManager == null)
                throw new Exception("Server Error");

            return _userManager;
        }
    }


    public RegisterManager(IAccountValidator accountValidator, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        AccountValidator = accountValidator;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<ResponseModel> RegisterUserAsync(IUserModel userToRegister, CancellationToken token)
    {
        string message = string.Empty;
        
        var isPasswordValid = AccountValidator.IsPasswordValid(userToRegister.Password, userToRegister.ConfirmPassword, ref message);
        if (isPasswordValid)
        {
            IdentityResult registerResult = await  CreateUserAsync(userToRegister, token);

            if (registerResult.Succeeded)
                return ResponseModel.CreateResponse(ResponseIdentityResult.Success);
            else
                return ResponseModel.CreateResponse(ResponseIdentityResult.UserNotCreated);
        }
        else
        {
            return ResponseModel.CreateResponse(ResponseIdentityResult.ValidationPasswordFailed, message);
        }
    }
    
    private async Task<IdentityResult> CreateUserAsync(IUserModel userToRegister, CancellationToken token)
    {
        IdentityResult result;
        var user = userToRegister.ConvertToIdentityUser();

        result = await UserManager.CreateAsync(user, userToRegister.Password);

        if(result.Succeeded)
            result = await UserManager.AddClaimAsync(user, new Claim("GUID", await UserManager.GetUserIdAsync(user)));

        return result;

    }

}