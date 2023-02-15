﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RealTimeChat.API.DataAccess.Models;
using RealTimeChat.BusinessLogic.AccountLogic.Enums;
using RealTimeChat.BusinessLogic.AccountLogic.Interfaces;
using RealTimeChat.BusinessLogic.AccountLogic.Messages;
using RealTimeChat.BusinessLogic.AccountLogic.Models;
using RealTimeChat.BusinessLogic.AccountLogic.SessionManager;
using RealTimeChat.BusinessLogic.AccountLogic.Validators;

namespace RealTimeChat.BusinessLogic.AccountLogic.AccountManager;

public class LoginManager : ILoginManager
{
    private SignInManager<ApplicationUser> _singInManager;
    private readonly ISessionHandler _sessionHandler;

    public IAccountValidator Validator { get; }
    public SignInManager<ApplicationUser> SignInManager
    {
        get
        {
            //TODO implement custom exception
            if (_singInManager == null)
                throw new Exception("Server error");

            return _singInManager;
        }
    }
    
    public LoginManager(IAccountValidator validator, SignInManager<ApplicationUser> singInManager, ISessionHandler sessionHandler)
    {
        Validator = validator;
        _singInManager = singInManager;
        _sessionHandler = sessionHandler;
    }

    public async Task<ResponseModel> LoginUserAsync(IUserModel user)
    {
        var isValid = Validator.IsPasswordValid(user.Password);
        if (isValid)
        {
            var result = await SignInAsync(user);

            if (result.Result == ResponseIdentityResult.WrongCredentials)
            {
                throw new Exception(ResponseResultMessage.WrongCredentials);
            }
            
            await _sessionHandler.InitializeSession(user);

            return result;
        }
        else
        { 
            return ResponseModel.CreateResponse(ResponseIdentityResult.WrongCredentials, "Password is Too short");
        }

    }

    public async Task<ResponseModel> SignInAsync(IUserModel user)
    {
        SignInResult signInResult =  await SignInManager.PasswordSignInAsync(user.Username, user.Password, false, false);

        if (signInResult.Succeeded)
            return ResponseModel.CreateResponse(ResponseIdentityResult.Success);
        else
            return ResponseModel.CreateResponse(ResponseIdentityResult.WrongCredentials);
    }

  

    public async Task SignOutAsync()
    {
        await SignInManager.SignOutAsync();

        var guid = SignInManager.Context.Session.Id;
        
        _sessionHandler.TerminateSession(guid);
    }
}