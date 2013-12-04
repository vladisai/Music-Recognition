using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Dsp;

namespace MusicRecognitionClassLibrary
{
    public class Hash
    {
        private Complex[][] comData;
        private int width, height;

        private List<KeyValuePair<int, double>>[] peaks;

        public Hash(Complex[][] comData)
        {
            this.comData = comData;
            width = comData.Length;
            if (width != 0)
                height = 600;
        }

        public void getPeaks()
        {
            

            var frequencyPeaks = getPeakTimesNew2();
            List<KeyValuePair<int, double>>[] maxPeaks = new List<KeyValuePair<int, double>>[width];

            for (int i = 0; i < width; i++)
            {
                maxPeaks[i] = cullPeaksNew(frequencyPeaks[i]);
            }

            peaks = maxPeaks;
        }

        private List<Peak> getAdjacentPeaks(Peak peak)
        {

            List<Peak> result = new List<Peak>();

            int minDistance = (int)ParamsParser.getParam("minDistance");
            int maxDistance = (int)ParamsParser.getParam("maxDistance");
            int zoneHeight = (int)ParamsParser.getParam("zoneHeight");

            int posStart = Math.Min(width - 1, peak.time + minDistance);
            int posFinish = Math.Min(width - 1, peak.time + maxDistance);
            int freqStart = peak.frequency - zoneHeight;
            int freqFinish = peak.frequency + zoneHeight;

            for (int i = posStart; i <= posFinish; i++)
            {
                for (int j = 0; j < peaks[i].Count; j++)
                {
                    if (peaks[i][j].Key >= freqStart && peaks[i][j].Key <= freqFinish)
                        result.Add(new Peak(peaks[i][j].Key, i));
                }
            }

            return result;
        }

        public List<PeaksPair> getPeakPairs()
        {
            List<PeaksPair> result = new List<PeaksPair>();
            getPeaks();
           

            for (int i = 0; i < peaks.Length; i++)
            {
                for (int j = 0; j < peaks[i].Count; j++)
                {
                    var currentPeak = new Peak(peaks[i][j].Key, i);
                    var adjacentPeaks = getAdjacentPeaks(currentPeak);

                    foreach (var peak in adjacentPeaks)
                    {
                        result.Add(new PeaksPair(currentPeak, peak));
                    }
                }
            }

            return result;
        }

        private List<KeyValuePair<int, double>>[] getPeakTimes()
        {
            var result = new List<KeyValuePair<int, double>>[width];

            for (int i = 0; i < width; i++)
                result[i] = new List<KeyValuePair<int, double>>();

            for (int row = 0; row < height; row++)
            {
                double thresholdDecaySpeed = 0.01;
                double currentThreshold = 0;
                int startPoint = 10;

                for (int i = 0; i < width; i++)
                {
                    currentThreshold = Math.Max(currentThreshold - thresholdDecaySpeed, 0);
                    if (getVal(comData[i][row]) > currentThreshold)
                    {
                        if (i > startPoint)
                            result[i].Add(new KeyValuePair<int, double>(row, getVal(comData[i][row]) - currentThreshold));
                        currentThreshold = getVal(comData[i][row]);
                        thresholdDecaySpeed = .001 * currentThreshold;
                    }
                }
            }

            return result;
        }

        private List<KeyValuePair<int, double>>[] getPeakTimesNew()
        {
            var result = new List<KeyValuePair<int, double>>[width];

            double[] threshold = new double[height];
            double[] thresholdDecaySpeed = new double[height];

            for (int i = 0; i < height; i++)
            {
                threshold[i] = -1;
                thresholdDecaySpeed[i] = 0;
            }

            for (int i = 0; i < width; i++)
                result[i] = new List<KeyValuePair<int, double>>();

            int influenceDistance = 20;
            double influenceCoefficient = 0.95;
            int startPoint = 3;

            var indices = new List<int>();
            for (int i = 0; i < height; i++)
                indices.Add(i);

            for (int time = 0; time < width; time++)
            {                
                indices.Sort(delegate(int a, int b)
                                 {
                                     double difA = getVal(comData[time][a]) - threshold[a];
                                     double difB = getVal(comData[time][b]) - threshold[b];

                                     if (difA > difB)
                                         return -1;
                                     else if (difA < difB)
                                         return 1;
                                     else
                                         return 0;
                                 });
                int added = 0;
                for (int freqInd = 0; freqInd < height; freqInd++)
                {
                    var freq = indices[freqInd];
                    threshold[freq] = Math.Max(threshold[freq] - thresholdDecaySpeed[freq], 0);
                    if (getVal(comData[time][freq]) > threshold[freq])
                    {
                        double dif = getVal(comData[time][freq]) - threshold[freq];
                        if (time > startPoint)
                            result[time].Add(new KeyValuePair<int, double>(freq, dif));

                        added++;
                        double k = 1;

                        for (int i = Math.Max(0, freq-influenceDistance); i < Math.Min(freq + influenceDistance, height); i++)
                        {
                            threshold[i] += dif * k;
                            thresholdDecaySpeed[i] = .1 * threshold[i];
                            k *= influenceCoefficient;
                        }
                    }
                }
            }

            return result;
        }

        private List<KeyValuePair<int, double>>[] getPeakTimesNew2()
        {
            var result = new List<KeyValuePair<int, double>>[width];

            double[] threshold = new double[height];
            double[] thresholdDecaySpeed = new double[height];

            double[] thresholdNext = new double[height];
            double[] thresholdDecaySpeedNext = new double[height];

            for (int i = 0; i < height; i++)
            {
                threshold[i] = -1;
                thresholdNext[i] = -1;
                thresholdDecaySpeed[i] = 0;
                thresholdDecaySpeed[i] = 0;
            }

            for (int i = 0; i < width; i++)
                result[i] = new List<KeyValuePair<int, double>>();

            int influenceDistance = (int)ParamsParser.getParam("influenceDistance");
            double influenceCoefficient = ParamsParser.getParam("influenceCoefficient");
            int startPoint = 3;
            double speedCoefficient = ParamsParser.getParam("speedCoefficient");


            for (int time = 0; time < width; time++)
            {
                for (int i = 0; i < height; i++)
                {
                    threshold[i] = thresholdNext[i];
                    thresholdDecaySpeed[i] = thresholdDecaySpeedNext[i];
                }

                int added = 0;
                for (int freq = 0; freq < height; freq++)
                {
                    thresholdNext[freq] = threshold[freq] = Math.Max(threshold[freq] - thresholdDecaySpeed[freq], 0);
                    if (getVal(comData[time][freq]) > threshold[freq])
                    {
                        double dif = getVal(comData[time][freq]) - threshold[freq];
                        if (time > startPoint)
                            result[time].Add(new KeyValuePair<int, double>(freq, dif));

                        added++;
                        double k = 1;

                        thresholdNext[freq] += dif;
                        thresholdDecaySpeedNext[freq] = speedCoefficient*thresholdNext[freq];
                        
                        for (int i = 1; i < influenceDistance; i++)
                        {
                            if (freq - i > 0 && thresholdNext[freq - i] < thresholdNext[freq])
                            {
                                thresholdNext[freq - i] += dif*k;
                                thresholdDecaySpeedNext[freq - i] = speedCoefficient * thresholdNext[freq - i];
                            }
                            if (freq + i < height && thresholdNext[freq + i] < thresholdNext[freq])
                            {
                                thresholdNext[freq + i] += dif*k;
                                thresholdDecaySpeedNext[freq + i] = speedCoefficient * thresholdNext[freq + i];
                            }
                            
                            k *= influenceCoefficient;
                        }
                    }
                }
            }

            return result;
        }

        public static double getVal(Complex v)
        {
            return Math.Log(1 + Math.Sqrt(v.X * v.X + v.Y * v.Y));
        }

        List<KeyValuePair<int, double>> cullPeaksNew(List<KeyValuePair<int, double>> peaks, int desiredQuantity = 4, int exclusionDistance = 15)
        {
            desiredQuantity = (int)ParamsParser.getParam("desiredQuantity");
            exclusionDistance = (int)ParamsParser.getParam("exclusionDistance");

            bool[] excluded = new bool[height];

            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();

            peaks.Sort(delegate(KeyValuePair<int, double> p1, KeyValuePair<int, double> p2)
                                    {
                                        if (p1.Value > p2.Value)
                                        {
                                            return -1;
                                        }
                                        else if (p1.Value == p2.Value)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return 1;
                                        }
                                    });

            for (int i = 0; i < peaks.Count; i++)
            {
                if (peaks[i].Key >= 15 && excluded[peaks[i].Key] == false)
                {
                    result.Add(peaks[i]);

                    for (int j = Math.Max(0, peaks[i].Key - exclusionDistance);
                         j < Math.Min(height - 1, peaks[i].Key + exclusionDistance);
                         j++)
                        excluded[j] = true;

                    if (result.Count >= desiredQuantity)
                        break;
                }
            }

            return result;

        }

        List<KeyValuePair<int, double>> cullPeaks(List<KeyValuePair<int, double>> peaks)
        {
            var result = new List<KeyValuePair<int, double>>();

            peaks.Sort(delegate(KeyValuePair<int, double> p1, KeyValuePair<int, double> p2)
                                {
                                    if (p1.Key > p2.Key)
                                    {
                                        return 1;
                                    }
                                    else if (p1.Key == p2.Key)
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                });
            int stepWidth = 20;
            for (int j = 10; j < peaks.Count; j += stepWidth)
            {
                double mxf = 0;
                int mxi = 0;

                for (int k = j; k < Math.Min(j + stepWidth, peaks.Count); k++)
                {
                    if (mxf < peaks[j].Value)
                    {
                        mxf = peaks[j].Value;
                        mxi = peaks[j].Key;
                    }
                }
                if (mxf != 0.0 && mxi >= 15)
                    result.Add(new KeyValuePair<int, double>(mxi, mxf));
            }

            return result;

        }

    }
}
