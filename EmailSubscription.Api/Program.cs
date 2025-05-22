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

app.MapPost("/subscribe", async (SubscribeRequest request, AppDbContext dbContext, EmailService service) =>
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

    await service.SendEmailAsync(user.Email, user.Name, "Witaj w EmailSubscriptionApp",
        "Twój adres e-mail został pomyślnie zarejestrowany.");
    return Results.Ok();
});

app.MapPost("/send-to-group", async (EmailService service, AppDbContext dbContext, string groupName = "Grupa Niebieska") =>
{
    var subject = "Default subject";
    var message = "Lorem ipsum";
    
    var group = await dbContext.Groups
        .Include(g => g.Users)
        .FirstOrDefaultAsync(g => g.Name == groupName);

    if (group == null)
        return Results.NotFound("Grupa nie istnieje");

    var tasks = group.Users.Select(user =>
        service.SendEmailAsync(user.Email, user.Name ?? "", subject, message));

    await Task.WhenAll(tasks);

    return Results.Ok("Wiadomości zostały wysłane.");
});

app.MapPost("/send-to-all", async (EmailService service, AppDbContext dbContext) =>
{
    var users = await dbContext.Users.ToListAsync();
    if (users.Count == 0)
        return Results.NotFound("Nie istnieje żaden zarejestrowany użytkownik.");
    
    var tasks = users.Select(user =>
        service.SendEmailAsync(user.Email, user.Name ?? "", "Default subject", "Lorem ipsum"));

    await Task.WhenAll(tasks);

    return Results.Ok("Wiadomości zostały wysłane.");
});

app.MapPost("/groups", async (CreateGroupRequest request, AppDbContext dbContext) =>
{
    var groupName = request.Name;

    if (string.IsNullOrWhiteSpace(groupName))
        return Results.BadRequest("Nazwa grupy nie może być pusta");

    if (await dbContext.Groups.AnyAsync(g => EF.Functions.Like(g.Name, groupName)))
        return Results.Conflict("Grupa o podanej nazwie już istnieje");

    dbContext.Groups.Add(new Group() { Name = groupName });
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});


app.Run();