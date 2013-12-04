using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicRecognitionClassLibrary
{
    public class PeaksPair
    {
        public int position, peakFrequency1, peakFrequency2, dTime;

        public static bool operator == (PeaksPair pair1, PeaksPair pair2)
        {         
            if (pair1.peakFrequency1 == pair2.peakFrequency1)
                if (pair1.peakFrequency2 == pair2.peakFrequency2)                    
                        if (pair1.dTime == pair2.dTime) 
                            return true;

            return false;
        }

        public static bool operator != (PeaksPair pair1, PeaksPair pair2)
        {
            return !(pair1 == pair2);
        }


        public PeaksPair(Peak p1, Peak p2)
        {
            int mod = 2;
            position = p1.time;

            peakFrequency1 = p1.frequency;
            peakFrequency1 -= peakFrequency1 % mod;

            peakFrequency2 = p2.frequency;
            peakFrequency2 -= peakFrequency2 % mod;

            dTime = (p2.time - p1.time);
            dTime -= dTime%mod;
        }

        public int getHashID
        {
            get { return 31*31*peakFrequency1 + 31*peakFrequency2 + dTime; }
        }
    }
}
