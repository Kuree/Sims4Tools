using System.Collections.Generic;
using s4pi.Interfaces;

namespace s4pi.Animation
{
    public class ClipResourceHandler : AResourceHandler
    {
        public ClipResourceHandler()
        {
            Add(typeof (ClipResource), new List<string> {"0x6B20C4F3"});
        }
    }
}
