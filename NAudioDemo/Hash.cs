using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioDemo
{
    class Hash
    {
        public long key;
        public string val;

        public Hash(int[] recordPoints, string val)
        {
            key = recordPoints[3] + recordPoints[2]*1000 + recordPoints[1]*1000000 + recordPoints[0]*100000000;
            this.val = val;
        }

        public void outputIntoFile(string FileName)
        {                        
            File.AppendAllText(FileName, key.ToString() + " <" + val + ">  \n");            
        }
    }
}
