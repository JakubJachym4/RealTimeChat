﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.API.DataAccess.Models;
using RealTimeChat.DataAccess.Models;
using System.Reflection.Emit;

namespace RealTimeChat.DataAccess.IdentityContext;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserConversationConnector>()
            .HasKey(connector => new { connector.ConversationID, connector.UserGUID });


        builder.Entity<UserConversationConnector>()
            .HasOne(connector => connector.User)
            .WithMany(user => user.Connectors)
            .HasForeignKey(connector => connector.UserGUID);

        builder.Entity<UserConversationConnector>()
            .HasOne(connector => connector.Conversation)
            .WithMany(conversation => conversation.Connectors)
            .HasForeignKey(connector => connector.ConversationID);

        builder.Entity<Session>()
            .HasOne(session => session.User)
            .WithOne(user => user.ThisSession)
            .HasForeignKey<Session>(session => session.UserGUID);

        base.OnModelCreating(builder);
    }

    public DbSet<Session> Session { get; set; }
    public DbSet<Conversation> Conversation { get; set; }
    public DbSet<UserConversationConnector> UsersConversation { get; set; }

}
