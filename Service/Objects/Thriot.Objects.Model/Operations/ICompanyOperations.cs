﻿namespace Thriot.Objects.Model.Operations
{
    public interface ICompanyOperations
    {
        Company Get(string id);
    }

    public interface IPersistedCompanyOperations : ICompanyOperations { }
}
