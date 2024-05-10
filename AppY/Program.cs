using AppY.Abstractions;
using AppY.ChatHub;
using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddResponseCompression(Opt =>
{
    Opt.EnableForHttps = true;
    Opt.MimeTypes = new[] { "/application/javascript" };
    Opt.ExcludedMimeTypes = new[] { "/plain/text" };
    Opt.MimeTypes = new[] { "/application/json" };
});
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient(typeof(IBase<>), typeof(Base<>));
builder.Services.AddTransient<IAccount, Account>();
builder.Services.AddTransient<IUser, UserRepository>();
builder.Services.AddTransient<INotification, Notification>();
builder.Services.AddTransient<IDiscussion, DiscussionRepository>();
builder.Services.AddTransient<IChat, ChatRepository>();
builder.Services.AddTransient<IMailMessages, MailMessages>();
builder.Services.AddTransient<ChatMessageAbstraction, ChatMessageRepository>();
builder.Services.AddTransient<Message, DiscussionMessageRepository>();
builder.Services.AddTransient<Answer, DiscussionMessageAnswersRepository>();
builder.Services.AddTransient<ReactionAbstraction, DiscussionMessageReactionRepository>();
builder.Services.AddTransient<Image, ImageRepository>();
builder.Services.AddTransient<ICommandLine, CommandLineTool>();
builder.Services.AddTransient<ISavedMessage, SavedMessageRepository>();

builder.Services.AddIdentity<User, IdentityRole<int>>(Opt =>
{
    Opt.Password.RequiredUniqueChars = 0;
    Opt.Password.RequiredLength = 8;
    Opt.Password.RequireNonAlphanumeric = false;
    Opt.Password.RequireUppercase = false;
    Opt.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<Context>().AddRoles<IdentityRole<int>>().AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);
builder.Services.AddDbContext<Context>(Opt => Opt.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.MapHub<ChatHub>("/chat");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
