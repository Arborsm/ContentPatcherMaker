using System;

namespace ContentPatcherMaker.Services.Abstractions;

public interface IParameterEditorFactory
{
    object CreateEditorViewModel(string typeName);
    void Register(IParameterTypeProvider provider);
}

