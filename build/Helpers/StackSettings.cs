using System.Collections.Generic;

namespace Build.Aws
{
    public class StackSettings
    {
        public ICollection<string> NotificationArns { get; set; }
    }
}