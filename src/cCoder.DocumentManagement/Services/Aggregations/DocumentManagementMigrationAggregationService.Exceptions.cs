// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models.Exceptions;

namespace cCoder.DocumentManagement.Services.Aggregations;

internal sealed partial class DocumentManagementMigrationAggregationService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
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

    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
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

    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
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