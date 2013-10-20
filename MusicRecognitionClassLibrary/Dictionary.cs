using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicRecognitionClassLibrary
{
    public class Dictionary
    {
        private class hash
        {
            public PeaksPair pair;
            public int songNum;
            public hash(PeaksPair pair, int songNum)
            {
                this.pair = pair;
                this.songNum = songNum;
            }
        }

        private string[] songNames;

        private List<hash>[] hTable; 

        private int tSize = 1000003;

        public Dictionary(string path)
        {
            hTable = new List<hash>[tSize];

            for(int i = 0; i<hTable.Length; i++)
                hTable[i] = new List<hash>();

            if (Directory.Exists(path) == false)
                return;

            string[] files = Directory.GetFiles(path);
            songNames = new string[files.Length];
            for(int i = 0; i<files.Length; i++)
            {
                addFile(files[i], i);
                songNames[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
        }

        private void addFile(string name, int num)
        {
            string text = File.ReadAllText(name);
            string[] sNums = text.Split(' ');          

            for(int i = 0; i<sNums.Length; i++)
            {
                if (sNums[i].Length == 0) continue;
                string[] peak = sNums[i].Split('-');
                PeaksPair cur = parsePair(peak);
                int val = cur.getHashID;
                hTable[(int)(val%tSize)].Add(new hash(cur, num));
            }
        }

        public string[] findMatches(List<PeaksPair> peakPairs)
        {
            var matchPos = new List<int>[songNames.Length];
            for (int i = 0; i < matchPos.Length; i++)
            {
                matchPos[i] = new List<int>();
            }
            for (int i = 0; i < peakPairs.Count; i++)
            {
                long cur = peakPairs[i].getHashID % tSize;
                for (int j = 0; j < hTable[cur].Count; j++)
                {
                    if (hTable[cur][j].pair == peakPairs[i])
                        matchPos[hTable[cur][j].songNum].Add(hTable[cur][j].pair.position - peakPairs[i].position);
                }
            }

            var matches = new List<KeyValuePair<int, int>>[songNames.Length];
            

            for (int i = 0; i < matchPos.Length; i++)
            {
                matches[i] = new List<KeyValuePair<int, int>>();
                if (matchPos[i].Count == 0)
                {
                    matches[i].Add(new KeyValuePair<int, int>(0, 0));
                    continue;
                }
                int approximationFactor = 3;
                
                var positions = new List<KeyValuePair<int, int>>();

                matchPos[i].Sort();
                int mx = -1, curSum = 1;
                for(int j = 1; j<matchPos[i].Count; j++)
                {
                    if (matchPos[i][j] != matchPos[i][j - 1])
                    {
                        positions.Add(new KeyValuePair<int, int>(matchPos[i][j-1], curSum));
                        curSum = 1;
                    }
                    else
                        curSum++;                    
                }
                positions.Add(new KeyValuePair<int, int>(matchPos[i].Last(), curSum));

                for(int j = 0; j<positions.Count; j++)
                {
                    int steps = 0, cur = j+1, sum = positions[j].Value;
                    while (cur<positions.Count)
                    {
                        if (positions[cur].Key - positions[j].Key > approximationFactor)
                            break;
                        sum += positions[cur].Value;
                        cur++;
                    }
                    matches[i].Add(new KeyValuePair<int, int>(sum, positions[j].Key));                    
                }

                matches[i].Sort(delegate(KeyValuePair<int, int> p1, KeyValuePair<int, int> p2)
                                    {
                                        if (p1.Key > p2.Key)
                                        {
                                            return -1;
                                        }
                                        else if (p1.Key == p2.Key)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return 1;
                                        }
                                    });
                
            }

            string[] mSongs = new string[songNames.Length];
            for (int i = 0; i < songNames.Length; i++)
            {
                mSongs[i] = songNames[i];
            }

            for (int i = 0; i < mSongs.Length; i++)
            {
                for (int j = i + 1; j < mSongs.Length; j++)
                {
                    if (matches[i][0].Key < matches[j][0].Key)
                    {
                        var temp = matches[i];
                        matches[i] = matches[j];
                        matches[j] = temp;

                        var sTemp = mSongs[i];
                        mSongs[i] = mSongs[j];
                        mSongs[j] = sTemp;
                    }
                }
            }

            for (int i = 0; i < mSongs.Length; i++)
            {                
                int showedPairs = 1;
                mSongs[i] += " ";
                for (int j = 0; j < Math.Min(showedPairs, matches[i].Count); j++)
                {
                    //mSongs[i] += matches[i][j].Value.ToString() +"th - " + matches[i][j].Key.ToString() + "; ";
                    mSongs[i] += matches[i][j].Key.ToString();
                }

            }
            
            return mSongs;
        }      

        private PeaksPair parsePair(string[] s)
        {
            int f1 = int.Parse(s[0]);
            int f2 = int.Parse(s[1]);
            int t1 = int.Parse(s[2]);
            int dt = int.Parse(s[3]);
            return new PeaksPair(new Peak(f1, t1), new Peak(f2, t1+dt));
        }

    }

}
