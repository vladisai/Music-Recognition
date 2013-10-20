using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicRecognitionClassLibrary
{
    public class Peak
    {
        public int frequency;
        public int time;
        public Peak(int frequency, int time)
        {
            this.frequency = frequency;
            this.time = time;
        }
    }
}
