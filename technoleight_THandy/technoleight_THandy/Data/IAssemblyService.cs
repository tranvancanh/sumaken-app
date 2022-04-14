using System;
using System.Collections.Generic;
using System.Text;

namespace THandy.Data
{
    public interface IAssemblyService
    {
        string GetPackageName();
        string GetVersionName();
        string GetVersionCode();
    }
}
