using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Data
{
    public interface IDeviceService
    {
        bool IsUpperVersion(int major, int minor);
        string GetDeviceVersion();
        string GetManufacturerName();
        string GetModelName();
        string GetCpuType();
        string GetID();
    }
}
