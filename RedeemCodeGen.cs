using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeemCodeGen
{
    public class RedeemCodeGen
    {
        public const int TypeNum = 0;
        public const int TypeChar = 1;
        public const int TypeMix = 2;

        public static string[] CreateCode(int count, int type, int length, string prefix, string suffix)
        {
            string[] codes = null;
            if (length > 16)
            {
                switch (type)
                {
                    case TypeNum:
                        codes = GenUniNumCode(count, length - 16);
                        break;
                    case TypeChar:
                        codes = GenUniCharCode(count, length - 16);
                        break;
                    case TypeMix:
                        codes = GenUniMixCode(count, length - 16);
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case TypeNum:
                        codes = GenNumCode(count, length);
                        break;
                    case TypeChar:
                        codes = GenCharCode(count, length);
                        break;
                    case TypeMix:
                        codes = GenMixCode(count, length);
                        break;
                }

            }

            string[] redeem = new string[count];
            for (int i = 0; i < count; i++)
            {
                redeem[i] = codes[i].ToUpper();
            }
            return redeem;
        }

        private static string[] GenUniNumCode(int count, int length)
        {
            var codes = GenNumCode(count, length);
            if (codes == null)
            {
                return null;
            }

            var stamp = DateTime.Now.ToString("yyMMddhhmmss");
            var prefix = stamp.Substring(0, 5);
            var suffix = stamp.Substring(6, 6);

            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = prefix + codes[i] + suffix;
            }

            return codes;
        }

        private static string[] GenNumCode(int count, int length)
        {
            if (count.ToString().Length > length)
            {
                Console.WriteLine("生成数量大于位数，无法生成");
                return null;
            }

            var seed = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            var random = new Random((int)seed.TotalSeconds);
            int[] bitTimes = new int[length];
            int total_count = 1;
            int idx = 0;
            while (idx < length)
            {
                int f = random.Next(0, 10);
                bitTimes[idx] = f;
                total_count *= f > 0 ? f : 1;
                idx++;
                if (total_count >= count)
                {
                    break;
                }
            }

            while (total_count < count)
            {
                for (int i = 0; i < length; i++)
                {
                    if (bitTimes[i] < 10)
                    {
                        var c = bitTimes[i];
                        bitTimes[i] += 1;
                        total_count = total_count + total_count / c;
                    }

                    if (total_count >= count)
                    {
                        break;
                    }
                }
            }

            var codes = new string[total_count];
            for (int i = 0; i < length; i++)
            {
                int coloum = bitTimes[i];
                coloum = Math.Max(1, coloum);
                int row = total_count / coloum;

                int[] num_array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                int[] num = new int[coloum];
                int x = 0, tmp = 0, n = 0;
                for (int m = num_array.Length - 1; m > 0; m--)
                {
                    x = random.Next(0, m + 1);
                    tmp = num_array[m];
                    num_array[m] = num_array[x];
                    num_array[x] = tmp;
                    num[n] = num_array[m];
                    n++;
                    if (n >= coloum)
                    {
                        break;
                    }
                }

                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < coloum; k++)
                    {
                        var index = j * coloum + k;
                        codes[index] = i == 0 ? num[k].ToString() : codes[index] + num[k];
                    }
                }
            }

            return codes;
        }

        private static string[] GenUniCharCode(int count, int length)
        {
            var codes = GenNumCode(count, length);
            if (codes == null)
            {
                return null;
            }

            var stamp = DateTime.Now.ToString("yyMMddhhmmss");
            string prefix = "";
            for (int i = 0; i < 12; i += 2)
            {
                var num = int.Parse(stamp.Substring(i, 2));
                prefix += num.ToString("x2");
            }

            char[] predo = new[]
            {
                    'A','B','C','D','E','F',
                    'G','H','I','J','K','L',
                    'M','N','P','Q','R','S',
                    'T','U','V','W','X','Y','Z'
            };

            for (int i = 0; i < predo.Length; i++)
            {
                prefix = prefix.Replace(i.ToString(), predo[i].ToString());
            }

            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = prefix + codes[i];
            }

            return codes;
        }
        private static string[] GenCharCode(int count, int length)
        {
            char[] predo = new[]
            {
                    'A','B','C','D','E','F',
                    'G','H','I','J','K','L',
                    'M','N','P','Q','R','S',
                    'T','U','V','W','X','Y','Z'
            };

            if (Math.Pow(predo.Length, length) < count)
            {
                Console.WriteLine("生成数量大于位数，无法生成");
                return null;
            }

            var seed = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            var random = new Random((int)seed.TotalSeconds);
            int[] bitTimes = new int[length];
            int total_count = 1;

            int idx = 0;
            while (idx < length)
            {
                int f = random.Next(0, predo.Length);
                bitTimes[idx] = f;
                total_count *= f > 0 ? f : 1;
                idx++;
                if (total_count >= count)
                {
                    break;
                }
            }

            var codes = new string[total_count];
            for (int i = 0; i < length; i++)
            {
                int coloum = bitTimes[i];
                coloum = Math.Max(1, coloum);
                int row = total_count / coloum;


                char[] num = new char[coloum];
                int x = 0, n = 0;
                char tmp = ' ';
                for (int m = predo.Length - 1; m > 0; m--)
                {
                    x = random.Next(0, m + 1);
                    tmp = predo[m];
                    predo[m] = predo[x];
                    predo[x] = tmp;
                    num[n] = predo[m];
                    n++;
                    if (n >= coloum)
                    {
                        break;
                    }
                }

                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < coloum; k++)
                    {
                        var index = j * coloum + k;
                        codes[index] = i == 0 ? num[k].ToString() : codes[index] + num[k];
                    }
                }
            }

            return codes;
        }

        private static string[] GenUniMixCode(int count, int length)
        {
            var codes = GenMixCode(count, length);
            if (codes == null)
            {
                return null;
            }

            var stamp = DateTime.Now.ToString("yyMMddhhmmss");
            string prefix = "";
            for (int i = 0; i < 12; i += 2)
            {
                var num = int.Parse(stamp.Substring(i, 2));
                prefix += num.ToString("x2");
            }

            char[] predo = new[]
            {
                    'G','H','I','J','K','L',
                    'M','N','P','Q','R','S',
            };

            var c = predo[DateTime.Now.Month - 1];
            prefix = prefix.Replace("0", c.ToString());

            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = prefix + codes[i];
            }

            return codes;
        }

        private static string[] GenMixCode(int count, int length)
        {
            char[] predo = new[]
            {
                    'A','B','C','D','E','F',
                    'G','H','I','J','K','L',
                    'M','N','P','Q','R','S',
                    'T','U','V','W','X','Y','Z',
                    '1','2','3','4','5','6','7','8','9','0'
            };
            if (Math.Pow(25, length) < count)
            {
                Console.WriteLine("生成数量大于位数，无法生成");
                return null;
            }
            var seed = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            var random = new Random((int)seed.TotalSeconds);
            int[] bitTimes = new int[length];
            int total_count = 1;

            int idx = 0;
            while (idx < length)
            {
                int f = random.Next(0, predo.Length);
                bitTimes[idx] = f;
                total_count *= f > 0 ? f : 1;
                idx++;
                if (total_count >= count)
                {
                    break;
                }
            }

            var codes = new string[total_count];
            for (int i = 0; i < length; i++)
            {
                int coloum = bitTimes[i];
                coloum = Math.Max(1, coloum);
                int row = total_count / coloum;


                char[] num = new char[coloum];
                int x = 0, n = 0;
                char tmp = ' ';
                for (int m = predo.Length - 1; m > 0; m--)
                {
                    x = random.Next(0, m + 1);
                    tmp = predo[m];
                    predo[m] = predo[x];
                    predo[x] = tmp;
                    num[n] = predo[m];
                    n++;
                    if (n >= coloum)
                    {
                        break;
                    }
                }

                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < coloum; k++)
                    {
                        var index = j * coloum + k;
                        codes[index] = i == 0 ? num[k].ToString() : codes[index] + num[k];
                    }
                }
            }

            return codes;
        }
    }
}
