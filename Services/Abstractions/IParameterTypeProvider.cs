using System;

namespace ContentPatcherMaker.Services.Abstractions;

public interface IParameterTypeProvider
{
    string TypeName { get; }
    Type EditorViewModelType { get; }
}

