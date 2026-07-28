// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace DocumentManagement.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddDocumentManagementWeb(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseDocumentManagementApplication()
            .Run();
    }
}