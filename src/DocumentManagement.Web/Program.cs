// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DocumentManagement.Web.Hosting;


namespace DocumentManagement.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddDocumentManagementApplication(configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseDocumentManagementApplication();
        app.Run();
    }
}