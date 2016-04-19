using System;
using System.Data;
using System.Data.OleDb;

namespace 汉字听写大会专用系统
{
    class words
    {
        public static string ShowWord()
        {
            //随机查询一个word
            string sql = " select top 1 id, [Word] from words  where Used=0 order by NEWID();  ";
            DataTable dt = SqlHelper.ExecuteDt(sql);
            string word = string.Empty;
            if (dt.Rows.Count > 0)
            {
                string id = dt.Rows[0]["id"].ToString();
                word = dt.Rows[0]["Word"].ToString();
                string sql1 = " update words set Used=1 where ID=" + id;
                SqlHelper.ExecuteSql(sql1);
            }

            return word;
        }

        /// <summary>
        /// 导入excel数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Import(string filePath)
        {
            OleDbDataReader odr = null;
            try
            {
                //Excel就好比一个数据源一般使用
                //这里可以根据判断excel文件是03的还是07的，然后写相应的连接字符串
                string strConn = "Provider = Microsoft.ACE.OLEDB.12.0 ; Data Source =" + filePath + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1'";
                //"Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
              

                string strCom = " select * from [Sheet1$]";
                string insertSql = "INSERT INTO [dbo].[words]  VALUES('{0}','{1}',0)";
                OleDbConnection con = new OleDbConnection(strConn);
                con.Open();
                DateTime now = DateTime.Now;

                OleDbCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(strCom);
                odr = cmd.ExecuteReader();
                while (odr.Read())
                {

                    string word = odr[0].ToString();
                    SqlHelper.ExecuteSql(string.Format(insertSql, word, now));


                }
                odr.Close();

                return true;
            }
            catch (Exception)
            {
                if (odr != null) odr.Close();
                return false;
            }
        }
    }
}
