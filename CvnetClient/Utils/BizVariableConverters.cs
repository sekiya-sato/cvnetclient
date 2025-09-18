using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Utils
{
    /// <summary>
    /// BizV Array variable convert to c#
    /// </summary>
    public class BizArray
    { 
        List<string> list;
        public int Count => list.Count;

        // 🔹 Indexer for array-like access
        public string this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public BizArray() 
        {
            list = new List<string>();
        }

        public BizArray(string[] wrk_para)
        {
            list = new List<string>();
            foreach (string wrk in wrk_para)
            {
                list.Add(wrk);
            }
        }

        public void Add(string value)
        {
            list.Add(value);
        }

        public void Set(int index, string value)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            while (list.Count <= index)
            {
                list.Add(value);
            }
            list[index] = value;
        }

        public string? Get(int index)
        {
            if (index < 0 || index >= list.Count)
                throw new IndexOutOfRangeException($"Index {index} is out of range. Current size: {list.Count}");
            return (index < list.Count) ? list[index] : null;
        }

        public string[] ToArray()
        {
            return list.ToArray();
        }
    }
}
