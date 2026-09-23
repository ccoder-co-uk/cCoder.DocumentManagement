// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class FolderRoleProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFolderRoleOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFolderRoleOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateFolderRoleOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFolderRoleOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}