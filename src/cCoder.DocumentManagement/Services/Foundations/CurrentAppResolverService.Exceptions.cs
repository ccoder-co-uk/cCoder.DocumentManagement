// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models.Exceptions;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class CurrentAppResolverService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (DocumentManagementValidationException innerException)
        {
            throw new DocumentManagementValidationException(
                innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new DocumentManagementValidationException(
                innerException: innerException);
        }
        catch (DocumentManagementDependencyException innerException)
        {
            throw new DocumentManagementDependencyException(
                innerException: innerException);
        }
        catch (DocumentManagementServiceException innerException)
        {
            throw new DocumentManagementServiceException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new DocumentManagementServiceException(
                innerException: innerException);
        }
    }
}