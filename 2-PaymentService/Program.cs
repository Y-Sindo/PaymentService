using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; //连接SQLServer 数据库专用
using System.Security.Cryptography;

namespace _2_PaymentService
{

    abstract class MyRAS
    {
        /// <summary>
        /// RAS加密
        /// </summary>
        /// <param name="PrivateKey">私钥</param>
        /// <param name="EncryptString">明文</param>
        /// <returns>密文</returns>
        public static string RSAEncrypt(string PrivateKey, string EncryptString)
        {
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            string Result = String.Empty;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PrivateKey);
            int t = (int)(Math.Ceiling((double)EncryptString.Length / (double)50));
            //分割明文
            for (int i = 0; i <= t - 1; i++)
            {
                PlainTextBArray = (new UnicodeEncoding()).GetBytes(EncryptString.Substring(i * 50, EncryptString.Length - (i * 50) > 50 ? 50 : EncryptString.Length - (i * 50)));
                CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
                Result += Convert.ToBase64String(CypherTextBArray) + "ThisIsSplit";
            }
            return Result;
        }
        /// <summary>
        /// RAS解密
        /// </summary>
        /// <param name="PublicKey">公钥</param>
        /// <param name="DecryptString">密文</param>
        /// <returns>明文</returns>
        public static string RSADecrypt(string PublicKey, string DecryptString)
        {
            byte[] PlainTextBArray;
            byte[] DypherTextBArray;
            string Result = String.Empty;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PublicKey);
            string[] Split = new string[1];
            Split[0] = "ThisIsSplit";
            //分割密文
            string[] mis = DecryptString.Split(Split, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < mis.Length; i++)
            {
                PlainTextBArray = Convert.FromBase64String(mis[i]);
                DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
                Result += (new UnicodeEncoding()).GetString(DypherTextBArray);
            }
            return Result;
        }
        /// <summary>
        /// 产生公钥和私钥对
        /// </summary>
        /// <returns>string[] 0:私钥;1:公钥</returns>
        public static string[] RSAKey()
        {
            string[] keys = new string[2];
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            keys[0] = rsa.ToXmlString(true);
            keys[1] = rsa.ToXmlString(false);
            return keys;
        }
    }


    abstract class MySQL
    {
        //temp表中用户名是否存在
        public static Boolean Exist_Temp_User_id(SqlConnection lo_conn, string User_id)
        {
            Boolean flag = false;
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "select * from TEMP where User_id = \'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            if (lo_reader.Read())
                flag = true;
            lo_reader.Close();
            return flag;
        }

        //在temp表中插入一个新用户的用户名和公钥,如果存在该用户名则仅更新公钥
        public static void Insert_Temp_User(SqlConnection lo_conn, string User_id, string Public_key)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            if (Exist_Temp_User_id(lo_conn, User_id))
                lo_cmd.CommandText = "UPDATE TEMP SET Public_key=\'" + Public_key + "\' WHERE User_id=\'" + User_id + "\';";   //写SQL语句  
            else
                lo_cmd.CommandText = "INSERT INTO TEMP(User_id,Public_key) VALUES(\'" + User_id + "\',\'" + Public_key + "\')";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //在temp表中删除一个用户的信息
        public static void Delete_Temp_User(SqlConnection lo_conn, string User_id)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "DELETE FROM TEMP WHERE User_id=\'" + User_id + "\';";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //查询temp表中用户信息
        public static SqlDataReader Temp_Info_of_a_user(SqlConnection lo_conn, string User_id)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "select * from TEMP where User_id=\'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            if (lo_reader.Read())
                return lo_reader;   //reader["User_id"].ToString();方式调用结果
            return null;
        }



        //更新用户公钥
        public static void Update_Public_key(SqlConnection lo_conn, string user_id, string Public_key)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "UPDATE USERinf SET Public_key=\'" + Public_key + "\' WHERE User_id=\'" + user_id + "\';";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //用户名是否存在
        public static Boolean Exist_User_id(SqlConnection lo_conn, string User_id)
        {
            Boolean flag = false;
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "select * from USERinf where User_id = \'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            if (lo_reader.Read())
                flag = true;
            lo_reader.Close();
            return flag;
        }

        //商家是否存在
        public static Boolean Exist_merchant_id(SqlConnection lo_conn, string Merchant_id)
        {
            Boolean flag = false;
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "select * from Merchant where Merchant_id=\'" + Merchant_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            if (lo_reader.Read())
                flag = true;
            lo_reader.Close();
            return flag;
        }

        //插入一个新用户的用户名和公钥
        public static void Insert_User_1(SqlConnection lo_conn, string User_id, string Public_key)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "INSERT INTO USERinf(User_id,Password,Payword,Public_key) VALUES(\'" + User_id + "\',\'" + "\',\'" + "\',\'" + Public_key + "\')";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //补全一个新用户的密码
        public static void Insert_User_2(SqlConnection lo_conn, string user_id, string password, string payword)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "UPDATE USERinf SET Password=\'" + password + "\' , Payword=\'" + payword + "\' WHERE User_id=\'" + user_id + "\';";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //返回特定用户名的信息，该用户不存在返回null
        public static SqlDataReader Info_of_a_user(SqlConnection lo_conn, string User_id)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "select * from USERinf where User_id=\'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            if (lo_reader.Read())
                return lo_reader;   //reader["User_id"].ToString();方式调用结果
            return null;
        }

        //设置免密模式
        public static void Set_No_payword(SqlConnection lo_conn, string User_id, int No_payword)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            string s = "update USERinf set No_payword=" + No_payword + " where User_id=\'" + User_id + "\'";
            lo_cmd.CommandText = s;    //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //用户消费扣款
        public static void PAY_of_user(SqlConnection lo_conn, string User_id, float consumption)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "update USERinf set Balance=Balance -" + consumption.ToString() + " where User_id=\'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。  
        }

        //商家得到收入，商家不存在返回0，成功返回1
        public static int GAIN_of_merchant(SqlConnection lo_conn, string Merchant_id, float consumption)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            if (!Exist_merchant_id(lo_conn, Merchant_id))
                return 0;
            lo_cmd.CommandText = "update MERCHANT set Balance=Balance +" + consumption.ToString() + " where Merchant_id=\'" + Merchant_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。
            return 1;
        }

        //插入一个新账单,返回0（不存在该商家），返回1（不存在该用户），返回2（正确完成）
        public static int Insert_A_new_Bill(SqlConnection lo_conn, string User_id, string Merchant_id, DateTime Date, float Amount, string Detail)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            if (!Exist_merchant_id(lo_conn, Merchant_id))
                return 0;
            if (!Exist_User_id(lo_conn, User_id))
                return 1;
            lo_cmd.CommandText = "INSERT INTO BILL(User_id ,Merchant_id ,Date,Amount,Detail) VALUES(\'" + User_id + "\',\'" + Merchant_id + "\',\'" + Date + "\'," + Amount + ",\'" + Detail + "\')";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。 
            return 2;
        }

        //得到用户的所有账单（按时间顺序）
        public static SqlDataReader Get_bills_of_a_User(SqlConnection lo_conn, string User_id)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "select * from BILL where User_id=\'" + User_id + "\' ORDER BY Date";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            return lo_reader;  //可用while(lo_reader.Read())得到所有结果
        }

        //Remain_try剩余尝试次数递减（为0只更新时间戳）
        public static void Decrease_Remain_try(SqlConnection lo_conn, string User_id, DateTime Last_trytime)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "update USERinf set Last_trytime =\'" + Last_trytime + "\',Remain_try= Remain_try-1 " + "where  Remain_try>0 and User_id=\'" + User_id + "\';" + "update USERinf set Last_trytime=\'" + Last_trytime + "\',Remain_try=0 " + "where  Remain_try=0 and User_id=\'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。 
        }

        //Remain_try置5（初始状态）
        public static void Reset_Remain_try(SqlConnection lo_conn, string User_id, DateTime Last_trytime)
        {
            //向数据库发送SQL命令要使用SqlCommand:  
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象 
            lo_cmd.CommandText = "update USERinf set Last_trytime=\'" + Last_trytime + "\',Remain_try=5 " + "where User_id=\'" + User_id + "\'";   //写SQL语句  
            lo_cmd.Connection = lo_conn;            //指定连接对象，即上面创建的
            lo_cmd.ExecuteNonQuery();               //这个仅仅执行SQL命令，不返回结果集，实用于建表、批量更新等不需要返回结果的操作。 
        }

        //超过120s剩余尝试次数置5（初始状态）并返回1；锁定状态返回-1*剩余秒数；否则返回0
        public static int Timeup_Reset_Remain_try(SqlConnection lo_conn, string User_id)
        {
            SqlDataReader Reader = MySQL.Info_of_a_user(lo_conn, User_id);
            int remain_try=Int32.Parse(Reader["Remain_try"].ToString());
            if (remain_try > 0) return 0;//还有机会，直接返回

            DateTime Last_trytime = DateTime.Parse(Reader["Last_trytime"].ToString());
            TimeSpan tSpan = DateTime.Now - Last_trytime;
            int secondCount = (int)tSpan.TotalSeconds;
            if (secondCount >= 120)//过了锁定时间
            { 
                MySQL.Reset_Remain_try(lo_conn, User_id, DateTime.Now);
                Reader.Close();
                return 1;
            }
            else if (int.Parse(Reader["Remain_try"].ToString()) == 0)//锁定状态
            {
                Reader.Close();
                return -(120 - secondCount);
            }
            else
            {
                Reader.Close();
                return 0;
            }
        }
    }



    class Program
    {
         
    }
}
  /* 
            string[] key  = MyRAS.RSAKey();
            Console.WriteLine(key[0].Length);
            Console.WriteLine(key[1].Length);
            string chphertxet=MyRAS.RSAEncrypt(key[1], "你知道什么呀!");
            Console.WriteLine(chphertxet);
            string plaintxet = MyRAS.RSADecrypt(key[0], chphertxet);
            Console.WriteLine(plaintxet);
            Console.Read(); 
 */