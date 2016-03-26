using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 汉字听写大会专用系统
{
    class words
    {
        public static string ShowWord()
        {
            //随机查询一个word
            string sql = " select top 1 word from words order by NEWID()";
            string word = SqlHelper.ExecuteScalar(sql).ToString();
            return word;
        }
    }
}
