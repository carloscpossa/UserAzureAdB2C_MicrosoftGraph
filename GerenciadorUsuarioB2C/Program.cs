using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;

using IHost host = Host.CreateDefaultBuilder(args).Build();

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")            
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

var dadosAzureB2C = config.GetSection("azureadb2c");

var tenantId = dadosAzureB2C.GetSection("tenantId").Value;
var clientId = dadosAzureB2C.GetSection("clientId").Value;
var clientSecret = dadosAzureB2C.GetSection("clientSecret").Value;

var options = new TokenCredentialOptions
{
    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
};

var clientSecretCredential = new ClientSecretCredential(
    tenantId, clientId, clientSecret, options);

var graphClient = new GraphServiceClient(clientSecretCredential);


var user = new User
{
    AccountEnabled = true,
    DisplayName = "Usuário Novo",
    MailNickname = "usuarionovo",
    UserPrincipalName = "usuarionovo@dammer2.onmicrosoft.com",    

    Identities = new List<ObjectIdentity>()
    {
        new ObjectIdentity
        {
            SignInType = "emailAddress",
            Issuer = "dammer2.onmicrosoft.com",
            IssuerAssignedId = "usuarionovo@gmail.com"
        }
    },
    
    PasswordProfile = new PasswordProfile
    {
        ForceChangePasswordNextSignIn = false,
        Password = "senha"
    },
    Mail = "usuarionovo@gmail.com",
    PreferredLanguage = "PT-BR",
    UserType = "Guest",
    CompanyName = "Minha Empresa"
};

var usuario = await graphClient.Users
    .Request()
    .AddAsync(user);


Console.WriteLine($"Usuário criado com sucesso! Id: {usuario.Id}");
Console.ReadKey();

await host.RunAsync();
