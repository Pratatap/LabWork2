using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWork2._1
{
    public class Paginator
    {
        public static List<Threat[]> pagedThreatList;
        public static int page = 0;
        public static void Paginate(Threat[] threats)
        {
            List<Threat> localThreats = threats.ToList();
            List<Threat[]> list = new List<Threat[]>();
            int length = threats.Length % 20 == 0 ? threats.Length / 20 : (threats.Length / 20) + 1;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    list.Add(localThreats.Take(20).ToArray());
                    localThreats.RemoveRange(0, 20);
                }
                catch (Exception)
                {
                    //list.Add(localThreats.Take(localThreats.Count).ToArray());
                    break;
                }
            }
            pagedThreatList = list;
            page = 0;
        }
        public static bool Next()
        {
            if (page < pagedThreatList.Count-1)
            {
                page++;
                return true;
            }
            return false;
        }
        public static bool Prev()
        {
            if (page > 0)
            {
                page--;
                return true;
            }
            return false;
        }
    }
}
