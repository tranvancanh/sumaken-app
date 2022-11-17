using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Data
{
    public interface IAssemblyService
    {
        string GetPackageName();
        string GetVersionName();
        string GetVersionCode();
    }
}
