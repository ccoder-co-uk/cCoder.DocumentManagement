// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models.Exceptions;

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class CurrentAppResolverProcessingService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ArgumentException innerException)
        {
            throw new DocumentManagementValidationException(innerException: innerException);
        }
        catch (DocumentManagementDependencyException innerException)
        {
            throw new DocumentManagementDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new DocumentManagementServiceException(innerException: innerException);
        }
    }
}