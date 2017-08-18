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

        public static char[] sm_char_set{ get; set; }
        public static char[] sm_mix_set{get; set;}
        public static char[] sm_replacer{get; set;}

        public static string[] CreateCode(int count, int type, int length, string prefix, string suffix, bool replace, bool normal)
        {
            string[] codes = null;
            if (!normal)
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
                        codes = GenUniMixCode(count, length - 16, replace);
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
                redeem[i] = prefix + codes[i] + suffix;
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

            for (int i = 0; i < sm_replacer.Length; i++)
            {
                prefix = prefix.Replace(i.ToString(), sm_replacer[i].ToString());
            }

            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = prefix + codes[i];
            }

            return codes;
        }
        private static string[] GenCharCode(int count, int length)
        {
            if (Math.Pow(sm_char_set.Length, length) < count)
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
                int f = random.Next(0, sm_char_set.Length);
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
                for (int m = sm_char_set.Length - 1; m > 0; m--)
                {
                    x = random.Next(0, m + 1);
                    tmp = sm_char_set[m];
                    sm_char_set[m] = sm_char_set[x];
                    sm_char_set[x] = tmp;
                    num[n] = sm_char_set[m];
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

        private static string[] GenUniMixCode(int count, int length, bool replace)
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

            if(replace)
            {
                var c = sm_replacer[DateTime.Now.Month - 1];
                prefix = prefix.Replace("0", c.ToString());
            }

            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = prefix + codes[i];
            }

            return codes;
        }

        private static string[] GenMixCode(int count, int length)
        {
            if (Math.Pow(sm_mix_set.Length, length) < count)
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
                int f = random.Next(0, sm_mix_set.Length);
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
                for (int m = sm_mix_set.Length - 1; m > 0; m--)
                {
                    x = random.Next(0, m + 1);
                    tmp = sm_mix_set[m];
                    sm_mix_set[m] = sm_mix_set[x];
                    sm_mix_set[x] = tmp;
                    num[n] = sm_mix_set[m];
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
