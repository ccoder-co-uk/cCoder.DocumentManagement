// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT001, STXFORMAT002, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Authentication;
using System.Threading.Tasks;

using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Brokers.Loggings;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Exposures.Controllers;
using cCoder.DocumentManagement.Models.Exceptions;
using cCoder.DocumentManagement.Models.OData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace cCoder.DocumentManagement.Tests.Exposures;

public sealed partial class ControllerRuntimeContractTests
{
    public static IEnumerable<object[]> Actions => CreateActions()
        .Select(action => new object[] { action });

    public static IEnumerable<object[]> FailureCases =>
        from action in CreateActions()
        from failure in new[] { "Validation", "Argument", "Authentication", "Authorization", "Unexpected" }
        select new object[] { action, failure };

    public static IEnumerable<object[]> ModelStateActions => CreateActions()
        .Where(action => action.HasModelStateBranch)
        .Select(action => new object[] { action });

    [Theory]
    [MemberData(nameof(Actions))]
    public async Task Action_WhenDependencySucceeds_ReturnsSuccessAndLogsInformation(
        ControllerAction action)
    {
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        IActionResult result = await InvokeAsync(
            action: action,
            exception: null,
            loggingBrokerMock: loggingBrokerMock);

        int expectedStatus = action.Name switch
        {
            "FileContent.Post" or "File.Post" or "Folder.Post" or "FolderRole.Post" => StatusCodes.Status201Created,
            "FileContent.Delete" or "File.Delete" or "Folder.Delete" or "FolderRole.Delete" => StatusCodes.Status204NoContent,
            _ => StatusCodes.Status200OK,
        };

        GetStatusCode(result).Should().Be(expectedStatus);
        loggingBrokerMock.Verify(
            broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
        loggingBrokerMock.Verify(
            broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
    }

    [Theory]
    [MemberData(nameof(FailureCases))]
    public async Task Action_WhenDependencyFails_ReturnsClassifiedResponseAndExactLog(
        ControllerAction action,
        string failure)
    {
        Exception exception = CreateException(failure);
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        IActionResult result = await InvokeAsync(
            action: action,
            exception: exception,
            loggingBrokerMock: loggingBrokerMock);

        int expectedStatus = failure switch
        {
            "Validation" or "Argument" => StatusCodes.Status400BadRequest,
            "Authentication" => StatusCodes.Status401Unauthorized,
            "Authorization" => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        GetStatusCode(result).Should().Be(expectedStatus);
        GetBody(result).Should().Be(failure switch
        {
            "Validation" => "The document management request is invalid.",
            "Argument" => "The request is invalid.",
            "Authentication" => "Authentication failed.",
            "Authorization" => "Authorization failed.",
            _ => null,
        });

        loggingBrokerMock.Verify(
            broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);

        if (failure == "Unexpected")
        {
            loggingBrokerMock.Verify(
                broker => broker.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
                Times.Never);
            loggingBrokerMock.Verify(
                broker => broker.LogError(exception, It.IsAny<string>(), It.IsAny<object[]>()),
                Times.Once);
        }
        else
        {
            loggingBrokerMock.Verify(
                broker => broker.LogWarning(exception, It.IsAny<string>(), It.IsAny<object[]>()),
                Times.Once);
            loggingBrokerMock.Verify(
                broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
                Times.Never);
        }
    }

    [Theory]
    [MemberData(nameof(ModelStateActions))]
    public async Task Action_WhenModelStateIsInvalid_Returns400AndLogsWarning(ControllerAction action)
    {
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        ControllerBase controller = CreateController(
            action: action,
            exception: null,
            loggingBrokerMock: loggingBrokerMock);

        controller.ModelState.AddModelError(key: "value", errorMessage: "invalid");

        IActionResult result = await InvokeControllerAsync(controller: controller, action: action);

        GetStatusCode(result).Should().Be(StatusCodes.Status400BadRequest);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        VerifyNoInformationOrError(loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [InlineData("FileContent.Get")]
    [InlineData("File.Get")]
    [InlineData("Folder.Get")]
    public async Task Get_WhenEntityIsMissing_Returns404AndLogsWarning(string actionName)
    {
        ControllerAction action = CreateActions().Single(item => item.Name == actionName);
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        IActionResult result = await InvokeAsync(
            action: action,
            exception: null,
            loggingBrokerMock: loggingBrokerMock,
            emptyCollections: true);

        GetStatusCode(result).Should().Be(StatusCodes.Status404NotFound);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        VerifyNoInformationOrError(loggingBrokerMock: loggingBrokerMock);
    }

    [Fact]
    public async Task FolderRolePost_WhenRoleAlreadyExists_Returns409AndLogsWarning()
    {
        ControllerAction action = CreateActions().Single(item => item.Name == "FolderRole.Post");
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var exception = new DocumentManagementServiceException(
            innerException: new DuplicateFolderRoleException());

        IActionResult result = await InvokeAsync(
            action: action,
            exception: exception,
            loggingBrokerMock: loggingBrokerMock);

        GetStatusCode(result).Should().Be(StatusCodes.Status409Conflict);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(exception, It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        VerifyNoInformationOrError(loggingBrokerMock: loggingBrokerMock);
    }

    [Fact]
    public async Task FolderRoleDeleteAll_WhenDependencySucceeds_ReturnsOkAndLogsInformation()
    {
        ControllerAction action = CreateActions().Single(item => item.Name == "FolderRole.DeleteAll");
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        IActionResult result = await InvokeAsync(
            action: action,
            exception: null,
            loggingBrokerMock: loggingBrokerMock);

        GetStatusCode(result).Should().Be(StatusCodes.Status200OK);
        loggingBrokerMock.Verify(
            broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
        loggingBrokerMock.Verify(
            broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
    }

    private static async Task<IActionResult> InvokeAsync(
        ControllerAction action,
        Exception exception,
        Mock<ILoggingBroker> loggingBrokerMock,
        bool emptyCollections = false)
    {
        ControllerBase controller = CreateController(
            action: action,
            exception: exception,
            loggingBrokerMock: loggingBrokerMock,
            emptyCollections: emptyCollections);

        return await InvokeControllerAsync(controller: controller, action: action);
    }

    private static ControllerBase CreateController(
        ControllerAction action,
        Exception exception,
        Mock<ILoggingBroker> loggingBrokerMock,
        bool emptyCollections = false)
    {
        object manager = DispatchProxy.Create(
            interfaceType: action.ManagerType,
            proxyType: typeof(ManagerProxy));

        ((ManagerProxy)manager).Exception = exception;
        ((ManagerProxy)manager).EmptyCollections = emptyCollections;

        return (ControllerBase)Activator.CreateInstance(
            type: action.ControllerType,
            args: [manager, loggingBrokerMock.Object]);
    }

    private static async Task<IActionResult> InvokeControllerAsync(
        ControllerBase controller,
        ControllerAction action)
    {
        MethodInfo method = action.ControllerType.GetMethod(
            name: action.MethodName,
            types: action.ParameterTypes);

        object result = method.Invoke(obj: controller, parameters: action.Arguments);

        return result is Task<IActionResult> task
            ? await task
            : (IActionResult)result;
    }

    private static int GetStatusCode(IActionResult result) =>
        result is IStatusCodeActionResult statusCodeResult
            ? statusCodeResult.StatusCode.Value
            : throw new InvalidOperationException(message: $"{result.GetType().Name} has no status code.");

    private static object GetBody(IActionResult result) =>
        result is ObjectResult objectResult ? objectResult.Value : null;

    private static Exception CreateException(string failure) => failure switch
    {
        "Validation" => new DocumentManagementValidationException(
            innerException: new Exception(message: "sensitive-validation-detail")),
        "Argument" => new ArgumentException(message: "sensitive-argument-detail"),
        "Authentication" => new AuthenticationException(message: "sensitive-authentication-detail"),
        "Authorization" => new SecurityException(message: "sensitive-authorization-detail"),
        _ => new InvalidOperationException(message: "sensitive-unexpected-detail"),
    };

    private static void VerifyNoInformationOrError(Mock<ILoggingBroker> loggingBrokerMock)
    {
        loggingBrokerMock.Verify(
            broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
        loggingBrokerMock.Verify(
            broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
    }

    private static ControllerAction[] CreateActions() =>
    [
        new("FileContent.GetAll", typeof(FileContentController), typeof(IFileContentManager), "GetAll", [typeof(Microsoft.AspNetCore.OData.Query.ODataQueryOptions<FileContent>)], [null]),
        new("FileContent.Get", typeof(FileContentController), typeof(IFileContentManager), "Get", [typeof(Guid)], [Guid.Empty]),
        new("FileContent.Post", typeof(FileContentController), typeof(IFileContentManager), "Post", [typeof(FileContent)], [new FileContent()], true),
        new("FileContent.Put", typeof(FileContentController), typeof(IFileContentManager), "Put", [typeof(Guid), typeof(FileContent)], [Guid.NewGuid(), new FileContent()], true),
        new("FileContent.Delete", typeof(FileContentController), typeof(IFileContentManager), "Delete", [typeof(Guid)], [Guid.NewGuid()]),
        new("File.GetAll", typeof(FileController), typeof(IFileManager), "GetAll", [typeof(Microsoft.AspNetCore.OData.Query.ODataQueryOptions<cCoder.Data.Models.DMS.File>)], [null]),
        new("File.Get", typeof(FileController), typeof(IFileManager), "Get", [typeof(Guid)], [Guid.NewGuid()]),
        new("File.Post", typeof(FileController), typeof(IFileManager), "Post", [typeof(cCoder.Data.Models.DMS.File)], [new cCoder.Data.Models.DMS.File()], true),
        new("File.Put", typeof(FileController), typeof(IFileManager), "Put", [typeof(Guid), typeof(cCoder.Data.Models.DMS.File)], [Guid.NewGuid(), new cCoder.Data.Models.DMS.File()], true),
        new("File.Delete", typeof(FileController), typeof(IFileManager), "Delete", [typeof(Guid)], [Guid.NewGuid()]),
        new("Folder.Copy", typeof(FolderController), typeof(IFolderManager), "PostCopyAsync", [typeof(string), typeof(string), typeof(int), typeof(int)], ["source", "destination", 1, 2]),
        new("Folder.GetAll", typeof(FolderController), typeof(IFolderManager), "GetAll", [typeof(Microsoft.AspNetCore.OData.Query.ODataQueryOptions<Folder>)], [null]),
        new("Folder.Get", typeof(FolderController), typeof(IFolderManager), "Get", [typeof(Guid)], [Guid.NewGuid()]),
        new("Folder.Post", typeof(FolderController), typeof(IFolderManager), "Post", [typeof(Folder)], [new Folder()], true),
        new("Folder.Put", typeof(FolderController), typeof(IFolderManager), "Put", [typeof(Guid), typeof(Folder)], [Guid.NewGuid(), new Folder()], true),
        new("Folder.Delete", typeof(FolderController), typeof(IFolderManager), "Delete", [typeof(Guid)], [Guid.NewGuid()]),
        new("FolderRole.GetAll", typeof(FolderRoleController), typeof(IFolderRoleManager), "GetAll", [], []),
        new("FolderRole.Post", typeof(FolderRoleController), typeof(IFolderRoleManager), "Post", [typeof(FolderRole)], [new FolderRole()], true),
        new("FolderRole.DeleteAll", typeof(FolderRoleController), typeof(IFolderRoleManager), "DeleteAll", [typeof(ODataCollection<FolderRole>)], [new ODataCollection<FolderRole> { Value = [] }], true),
        new("FolderRole.Delete", typeof(FolderRoleController), typeof(IFolderRoleManager), "Delete", [typeof(Guid), typeof(Guid)], [Guid.NewGuid(), Guid.NewGuid()]),
    ];

    public sealed record ControllerAction(
        string Name,
        Type ControllerType,
        Type ManagerType,
        string MethodName,
        Type[] ParameterTypes,
        object[] Arguments,
        bool HasModelStateBranch = false)
    {
        public override string ToString() => Name;
    }

    public class ManagerProxy : DispatchProxy
    {
        public Exception Exception { get; set; }
        public bool EmptyCollections { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (Exception is not null)
            {
                throw Exception;
            }

            if (EmptyCollections && targetMethod.Name == "Get")
            {
                return null;
            }

            return CreateDefault(type: targetMethod.ReturnType);
        }

        private object CreateDefault(Type type)
        {
            if (type == typeof(void))
            {
                return null;
            }

            if (type == typeof(ValueTask))
            {
                return ValueTask.CompletedTask;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                Type valueType = type.GetGenericArguments()[0];
                object value = CreateDefault(type: valueType);
                return Activator.CreateInstance(type: type, args: [value]);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                Type valueType = type.GetGenericArguments()[0];
                Array values = Array.CreateInstance(
                    elementType: valueType,
                    length: EmptyCollections ? 0 : 1);

                if (!EmptyCollections)
                {
                    values.SetValue(
                        value: Activator.CreateInstance(type: valueType),
                        index: 0);
                }

                return typeof(Queryable).GetMethods()
                    .Single(method => method.Name == nameof(Queryable.AsQueryable) && method.IsGenericMethod && method.GetParameters().Length == 1)
                    .MakeGenericMethod(valueType)
                    .Invoke(obj: null, parameters: [values]);
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                Type valueType = type.GetGenericArguments()[0];

                return type.IsInterface
                    ? Array.CreateInstance(elementType: valueType, length: 0)
                    : Activator.CreateInstance(type: type);
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type: type);
            }

            ConstructorInfo constructor = type.GetConstructor(types: Type.EmptyTypes);
            return constructor is null ? null : constructor.Invoke(parameters: null);
        }
    }
}

#pragma warning restore STXFORMAT001, STXFORMAT002, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005
