using System.Collections.Generic;

namespace ZTourist.Infrastructure
{
    internal class ModelStateTransferValue
    {
        public string Key { get; set; }
        public string AttemptedValue { get; set; }
        public object RawValue { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}