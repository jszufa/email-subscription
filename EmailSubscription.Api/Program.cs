using EmailSubscription.Api;
using EmailSubscription.Api.Models;
using EmailSubscription.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/groups", async (AppDbContext dbContext) =>
{
    var groups = await dbContext.Groups.Select(g => g.Name).ToListAsync();

    return groups.Count == 0 ? Results.NoContent() : Results.Ok(groups);
});

app.MapPost("/subscribe", async (SubscribeRequest request, AppDbContext dbContext) =>
{
    if (await dbContext.Users.AnyAsync(u => u.Email == request.Email || u.Name == request.Name))
        return Results.Conflict("Użytkownik o podanej nazwie lub adresie email już istnieje");
    
    var user = new User()
    {
        Name = request.Name,
        Email = request.Email,
    };

    var selectedGroups = await dbContext.Groups.Where(g => request.Groups.Contains(g.Name)).ToListAsync();
    user.UserGroups = selectedGroups;
    
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    
    //TODO: Wysłać mail potwierdzający rejestrację użytkownika
    Console.WriteLine("Send confirmation email");
    return Results.Ok();
});

app.MapPost("/send-to-group", async (EmailService service) =>
{
    var subject = "Default subject";
    var message = "Lorem ipsum";
    
    //TODO: Dodać wysyłanie maila do wszystkich członków grupy.
    await service.SendEmailAsync("example@example.com", "Example", subject, message);
    
    return Results.Ok();
});

app.MapPost("/send-to-all", () =>
{
    Console.WriteLine("Send to all subscribers");
});

app.MapPost("/groups", async (CreateGroupRequest request, AppDbContext dbContext) =>
{
    var groupName = request.Name;

    if (string.IsNullOrWhiteSpace(groupName))
        return Results.BadRequest("Nazwa grupy nie może być pusta");
    
    if (await dbContext.Groups.AnyAsync(g => EF.Functions.Like(g.Name, groupName)))
        return Results.Conflict("Grupa o podanej nazwie już istnieje");
    
    dbContext.Groups.Add(new Group(){Name= groupName});
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});



app.Run();