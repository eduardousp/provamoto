using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Microsoft.OpenApi.Models;
using ProvaRental.Services;
using ProvaRental.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProvaRental.Data.Models;

var builder = WebApplication.CreateBuilder(args);

#region JWT
// Adicionar Autenticação com JWT
var key = Encoding.ASCII.GetBytes("sua_super_secreta_chave_segura123!");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Adicionar Autorização com Políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
#endregion

//PostgreSQL
builder.Services.AddDbContext<ProvaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));


builder.Services.AddScoped<ILocacaoService, LocacaoService>();
builder.Services.AddScoped<IEntregadorService, EntregadorService>();
builder.Services.AddScoped<IMotoService, MotoService>();

builder.Services.AddScoped<ILocacaoRepository, LocacaoRepository>();
builder.Services.AddScoped<IEntregadorRepository, EntregadorRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
//  RabbitMQ Publisher
builder.Services.AddSingleton<IRabbitMQPublisher>(sp =>
    new RabbitMQPublisher("localhost", "fila_motos"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProvaRental API", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Bearer esquema de autenticação",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securitySchema);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
});


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProvaRental API v1"));
}

app.UseHttpsRedirection();

// Endpoints minimal API
#region Moto


app.MapPost("/api/motos", async (Moto moto, IMotoService motoService) =>
{
    try
    {
        var novaMoto = await motoService.CadastrarMoto(moto);
        return Results.Created($"/api/motos/{novaMoto.Id}", novaMoto);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
}).RequireAuthorization("AdminOnly");

app.MapGet("/api/motos", async (IMotoService motoService) =>
{
    var motos = await motoService.ConsultarMotosAsync();
    return Results.Ok(motos);
});

app.MapGet("/api/motos/{id:int}", async (int id, IMotoService motoService) =>
{
    try
    {
        var moto = await motoService.ConsultarMotoPorIdAsync(id);
        return Results.Ok(moto);
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.MapPut("/api/motos/{id:int}/placa", async (int id, string novaPlaca, IMotoService motoService) =>
{
    try
    {
        var motoAtualizada = await motoService.AlterarPlacaAsync(id, novaPlaca);
        return Results.Ok(motoAtualizada);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
}).RequireAuthorization("AdminOnly");

app.MapDelete("/api/motos/{id:int}", async (int id, IMotoService motoService) =>
{
    try
    {
        var sucesso = await motoService.RemoverMotoAsync(id);
        return sucesso ? Results.NoContent() : Results.BadRequest(new { message = "Erro ao remover moto." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
}).RequireAuthorization("AdminOnly");
#endregion
#region Entregador
app.MapGet("/entregadores/{id}", async (int id, IEntregadorService entregadorService) =>
{
    var entregador = await entregadorService.GetEntregadorByIdAsync(id);
    if (entregador is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(entregador);
});

app.MapGet("/entregadores", async (IEntregadorService entregadorService) =>
{
    var entregadores = await entregadorService.GetAllEntregadoresAsync();
    return Results.Ok(entregadores);
}).RequireAuthorization("AdminOnly");

app.MapPost("/entregadores", async (Entregador entregador, IEntregadorService entregadorService) =>
{
    await entregadorService.AddEntregadorAsync(entregador);
    return Results.Created($"/entregadores/{entregador.Id}", entregador);
});

app.MapPut("/entregadores/{id}", async (int id, Entregador entregador, IEntregadorService entregadorService) =>
{
    var existingEntregador = await entregadorService.GetEntregadorByIdAsync(id);
    if (existingEntregador is null)
    {
        return Results.NotFound();
    }

    entregador.Id = id; // Mantém o ID
    await entregadorService.UpdateEntregadorAsync(entregador);
    return Results.NoContent();
});

app.MapDelete("/entregadores/{id}", async (int id, IEntregadorService entregadorService) =>
{
    var entregador = await entregadorService.GetEntregadorByIdAsync(id);
    if (entregador is null)
    {
        return Results.NotFound();
    }

    await entregadorService.DeleteEntregadorAsync(id);
    return Results.NoContent();
}).RequireAuthorization("AdminOnly");

#endregion
#region Locacao
app.MapPost("/locacoes", async (LocacaoDto locacaoDto, ILocacaoService locacaoService, IMotoService motoService, IEntregadorService entregadorService) =>
{
    // Validações de entregador e moto
    var entregador = await entregadorService.GetEntregadorByIdAsync(locacaoDto.EntregadorId);
    if (entregador == null || entregador.CNHTipo != "A")
        return Results.BadRequest("Entregador inválido ou sem habilitação 'A'.");

    var moto = await motoService.ConsultarMotoPorIdAsync(locacaoDto.MotoId);
    if (moto == null)
        return Results.BadRequest("Moto não encontrada.");

    // Validações do plano de locação
    if (!Enum.IsDefined(typeof(PlanoLocacao), locacaoDto.Plano))
        return Results.BadRequest("Plano de locação inválido.");

    // Criar a locação
    var locacao = new Locacao
    {
        EntregadorId = locacaoDto.EntregadorId,
        MotoId = locacaoDto.MotoId,
        DataInicio = DateTime.UtcNow.AddDays(1),  // Início no dia seguinte
        DataPrevisaoTermino = DateTime.UtcNow.AddDays((int)locacaoDto.Plano),
        Plano = locacaoDto.Plano
    };

    await locacaoService.CriarLocacaoAsync(locacao);
    return Results.Created($"/locacoes/{locacao.Id}", locacao);
});
app.MapGet("/locacoes/{id}", async (int id, ILocacaoService locacaoService) =>
{
    var locacao = await locacaoService.GetLocacaoByIdAsync(id);
    if (locacao == null)
        return Results.NotFound();

    return Results.Ok(locacao);
}).RequireAuthorization("AdminOnly");
app.MapPut("/locacoes/{id}/devolucao", async (int id, DateTime dataDevolucao, ILocacaoService locacaoService) =>
{
    var locacao = await locacaoService.GetLocacaoByIdAsync(id);
    if (locacao == null)
        return Results.NotFound();

    locacao.DataDevolucao = dataDevolucao;

    // Calcular valor total da locação
    var valorTotal = locacaoService.CalcularValorTotal(locacao);

    await locacaoService.AtualizarLocacaoAsync(locacao);
    return Results.Ok(new { ValorTotal = valorTotal });
});
app.MapGet("/locacoes", async (ILocacaoService locacaoService) =>
{
    var locacoes = await locacaoService.GetAllLocacoesAsync();
    return Results.Ok(locacoes);
}).RequireAuthorization("AdminOnly");

#endregion
#region Login
app.MapPost("/login", (UserLoginDTO login) =>
{
    // Aqui você pode implementar sua lógica de verificação de credenciais
    if (login.Username == "admin" && login.Password == "admin123")  // Simulação de validação de usuário
    {
        var token = GenerateJwtToken(login.Username, "Admin");  // Gera o token com role Admin
        return Results.Ok(new { Token = token });
    }

    return Results.Unauthorized();
});
#endregion
// Rodando a aplicação
app.Run();

static string GenerateJwtToken(string username, string role)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("sua_super_secreta_chave_segura123!");

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role)  // Claim do papel do usuário (Admin)
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

