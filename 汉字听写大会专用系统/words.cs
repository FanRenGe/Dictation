using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
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

        /// <summary>
        /// 导入excel数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Import(string filePath)
        {
            try
            {
                //Excel就好比一个数据源一般使用
                //这里可以根据判断excel文件是03的还是07的，然后写相应的连接字符串
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
                string strCom = " select * from [Sheet1$]";
                string insertSql = "INSERT INTO [dbo].[words]  VALUES({0},{1},null)";
                OleDbConnection con = new OleDbConnection(strConn);
                con.Open();
                DateTime now = DateTime.Now;

                OleDbCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(strCom);//[sheetName]要如此格式
                OleDbDataReader odr = cmd.ExecuteReader();
                while (odr.Read())
                {
                    if (odr[0].ToString() == "序号")//过滤列头  按你的实际Excel文件
                        continue;
                    //数据库添加操作
                    /*进行非法值的判断
                     * 添加数据到数据表中
                     * 添加数据时引用事物机制，避免部分数据提交
                     * Add(odr[1].ToString(), odr[2].ToString(), odr[3].ToString());//数据库添加操作，Add方法自己写的
                     * */
                    string word = odr[0].ToString();
                    SqlHelper.ExecuteSql(string.Format(insertSql, word, now));


                }
                odr.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
