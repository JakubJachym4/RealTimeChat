﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using RealTimeChat.API.DataAccess.IdentityContext;
using RealTimeChat.API.DataAccess.Models;
using RealTimeChat.BusinessLogic.FriendsLogic.Enums;
using RealTimeChat.BusinessLogic.FriendsLogic.Interfaces;
using RealTimeChat.BusinessLogic.FriendsLogic.Models;

namespace RealTimeChat.BusinessLogic.FriendsLogic.FriendsManagers;

public class InvitationsManager : IInvitationsManager
{
    private ApplicationContext Context { get; set; }
    private IDbUserHelper DbUserHelper { get; }


    public InvitationsManager(ApplicationContext context, IDbUserHelper dbUserHelper)
    {
        Context = context;
        DbUserHelper = dbUserHelper;
    }
    
    public async Task<ResponseModel> CreateInvitation(string userId, string friendId)
    {
        InvitationModel newInvitation = new InvitationModel()
        {
            Status = "Pending",
            SenderId = userId,
            ResponderId = friendId
        };
        
        var dbStatus = await Context.Invitations.AddAsync(newInvitation);
        if (dbStatus.State != EntityState.Added)
            throw new Exception("Database fail");
        
        await Context.SaveChangesAsync();
        return ResponseModel.CreateResponse(FriendsResponseResult.Success, "Invitation send");

    }

    public async Task<InvitationStatus> UpdateInvitation(string friendUsername, string userId, bool response)
    {
        var senderId = await DbUserHelper.FriendUsernameToId(friendUsername, userId);
        
        var invitation = await DbUserHelper.FindInvitation(senderId, userId);

        if (invitation == null || invitation.Status == "Declined")
            throw new ArgumentException("No invitation from this user");

        InvitationStatus invitationStatus;
        EntityEntry<InvitationModel> dbResponse;

        if (response == false)
        {
            invitation.Status = "Declined";
            dbResponse =  Context.Invitations.Update(invitation);
            invitationStatus = InvitationStatus.Declined;
        }
        else
        {
            invitation.Status = "Accepted";
            dbResponse = Context.Invitations.Remove(invitation);
            invitationStatus = InvitationStatus.Accepted;
        }

        if (dbResponse.State != EntityState.Modified && dbResponse.State != EntityState.Deleted)
            throw new Exception("Database fail");

      
        
        // check for conflict in invitations, or remaining invitation from opposite user, remove opposite invitation if necessary
        var oppositeInvitation = await DbUserHelper.FindInvitation(userId, senderId);

        if (oppositeInvitation != null)
        {
            dbResponse = Context.Invitations.Remove(oppositeInvitation);
            if(dbResponse.State != EntityState.Deleted)
                throw new Exception("Database fail");
        }
        
        
        await Context.SaveChangesAsync();
        
        return invitationStatus;
    }

    public async Task<ResponseModel> GetAllInvitations(string userId)
    {
        var invitations = DbUserHelper.GetAllAvailableInvitationsSenders(userId);
        
        if (invitations == null || invitations.Count == 0)
            return ResponseModel.CreateResponse(FriendsResponseResult.Fail, "No invitations");

        var invitationsDtoList = new List<InvitationDto>();
        
        foreach (var sender in invitations)
        {
            invitationsDtoList.Add(new InvitationDto(sender.UserName));
        }

        var result = JsonConvert.SerializeObject(invitationsDtoList);
        
        return ResponseModel.CreateResponse(FriendsResponseResult.Success, result);
    }
    
    
    private class InvitationDto
    {
        public string Sender;

        public InvitationDto(string sender)
        {
            Sender = sender;
        }
    }
}