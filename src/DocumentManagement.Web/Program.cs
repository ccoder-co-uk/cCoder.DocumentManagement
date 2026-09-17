// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace DocumentManagement.Web;

using cCoder.DocumentManagement;
using cCoder.Eventing;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddWeb(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();

        app.Services
            .GetRequiredService<IEventHub>()
            .ListenToDocumentManagementEvents();

        app.UseDocumentManagementApplication()
            .Run();
    }
}