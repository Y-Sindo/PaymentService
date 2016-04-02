using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace _2_PaymentService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        //String SQLstring = "Server=.;Database=PaymentAPP;uid=sa;pwd=443";
        string SQLstring = "data source=.;initial catalog=PaymentAPP;user id=sa;pwd=443";
        //测试用
        public string getBill(GetBillPara para)
        {
            return "Your id is " + para.id + ", name is "+ para.username+"\n";
        }

        public Rs_Pki register1(string username)
        {
            throw new NotImplementedException();
        }

        public Rs_Pki login1(string username)
        {
            throw new NotImplementedException();
        }

        /*
        public int register1(User_PKI user_pki)
        {
            string username = user_pki.username;
            string pki = user_pki.pki;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();
            if (MySQL.Exist_User_id(lo_conn, username))
            {
                lo_conn.Close();//关闭连接
                return -1;//用户名已存在，返回错误码
            }
            else
            {
                MySQL.Insert_User_1(lo_conn, username, pki);
                lo_conn.Close();//关闭连接 
                return 0;
            }
        }

           public int login1(User_PKI user_pki)
        {
            string username = user_pki.username;
            string pki = user_pki.pki;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();
            if (!MySQL.Exist_User_id(lo_conn, username))
            {
                lo_conn.Close();//关闭连接
                return -1;//用户名不存在，返回错误码
            }
            else//用户存在
            {
                MySQL.Insert_Temp_User(lo_conn, username, pki);//在temp表中插入一条记录
                lo_conn.Close();//关闭连接 
                return 0;
            }
        }
        */
        public int register2(User_Cyphertext user_cyphertext)
        {
            string username = user_cyphertext.username;
            string Epassword = user_cyphertext.cyphertext;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();
            //获得公钥
            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            string pki = sqlDataReader["Public_key"].ToString();
            string Dpassword = MyRAS.RSADecrypt(pki, Epassword);//解密
            string[] passwords = Dpassword.Split('#');
            sqlDataReader.Close();
            MySQL.Insert_User_2(lo_conn, username, passwords[0], passwords[1]);
            sqlDataReader.Close();
            lo_conn.Close();
            return 0;
        }

 

        public int login2(User_Cyphertext user_cyphertext)
        {
            string username = user_cyphertext.username;
            string loginPassword = user_cyphertext.cyphertext;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            SqlDataReader sqlDataReader_temp = MySQL.Temp_Info_of_a_user(lo_conn, username);
            string pki = sqlDataReader_temp["Public_key"].ToString();
            sqlDataReader_temp.Close();

            string Password = MyRAS.RSADecrypt(pki, loginPassword);//解密
            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            string a = sqlDataReader["Password"].ToString();
            if (Password.CompareTo(sqlDataReader["Password"].ToString()) == 0)
            {
                sqlDataReader.Close();
                MySQL.Update_Public_key(lo_conn, username, pki);
                MySQL.Delete_Temp_User(lo_conn, username);
                lo_conn.Close();
                return 0;
            }
            else
            {
                sqlDataReader.Close();
                lo_conn.Close();
                return -1;
            }
        }
        //有点问题
        public int turnToPasswordFreeMode(User_Cyphertext user_cyphertext)
        {
            string username = user_cyphertext.username;
            string paymentPassword = user_cyphertext.cyphertext;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            int rs = MySQL.Timeup_Reset_Remain_try(lo_conn, username);//timeup reset
            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            string pki = sqlDataReader["Public_key"].ToString();
            string Payword = MyRAS.RSADecrypt(pki, paymentPassword);//解密

            if (rs < 0)//锁定
            {
                sqlDataReader.Close();
                lo_conn.Close();
                return -rs;
            }

            if (Payword.CompareTo(sqlDataReader["Payword"].ToString()) == 0)//密码正确
            {
                MySQL.Set_No_payword(lo_conn, username, 1);
                MySQL.Reset_Remain_try(lo_conn, username, DateTime.Now);
                sqlDataReader.Close();
                lo_conn.Close();
                return 0;
            }
            else//密码错误
            {
                MySQL.Decrease_Remain_try(lo_conn, username, DateTime.Now);
                int a = int.Parse(sqlDataReader["Remain_try"].ToString()) - 1;
                sqlDataReader.Close();
                lo_conn.Close();
                return -a;
            }
        }

        public int exitPasswordFreeMode(User_Cyphertext user_cyphertext)
        {
            string username = user_cyphertext.username;
            string loginPassword = user_cyphertext.cyphertext;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            string pki = sqlDataReader["Public_key"].ToString();
            string Password = MyRAS.RSADecrypt(pki, loginPassword);//解密

            if (Password.CompareTo(sqlDataReader["Password"].ToString()) == 0)
            {
                MySQL.Set_No_payword(lo_conn, username, 0);
                sqlDataReader.Close();
                lo_conn.Close();
                return 0;
            }
            else
            {
                sqlDataReader.Close();
                lo_conn.Close();
                return -1;
            }
        }

        public int getMode(string username)
        {
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            int No_payword = int.Parse(sqlDataReader["No_payword"].ToString());
            sqlDataReader.Close();
            lo_conn.Close();
            return No_payword;
        }

        public double getBalance(string username)
        {
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            double balance = double.Parse(sqlDataReader["Balance"].ToString());
            sqlDataReader.Close();
            lo_conn.Close();
            return balance;
        }

        public Bill[] getBills(string username)//bill_id改成int
        {
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            System.Collections.ArrayList all_bills = new ArrayList();
            SqlDataReader sqlDataReader = MySQL.Get_bills_of_a_User(lo_conn, username);

            while (sqlDataReader.Read())
            {
                Bill temp = new Bill();
                temp.bill_id = int.Parse(sqlDataReader["Bill_id"].ToString());
                temp.user_id = sqlDataReader["User_id"].ToString();
                temp.merchant_id = sqlDataReader["Merchant_id"].ToString();
                temp.date = DateTime.Parse(sqlDataReader["Date"].ToString());
                temp.amount = float.Parse(sqlDataReader["Amount"].ToString());
                temp.detail = sqlDataReader["Detail"].ToString();
                all_bills.Add(temp);
            }

            Bill[] Bill_Array = (Bill[])all_bills.ToArray(typeof(Bill));
            sqlDataReader.Close();
            lo_conn.Close();
            return Bill_Array;
        }

        public int payWithPassword(PayInfo payInfo)
        {
            string username = payInfo.username;
            string cyphertext = payInfo.cyphertext;
            string detail = payInfo.detail;
            SqlConnection lo_conn = new SqlConnection(SQLstring);//连接方式选择sqlServer验证方式而不是windows
            lo_conn.Open();

            int rs = MySQL.Timeup_Reset_Remain_try(lo_conn, username);//timeup reset
            SqlDataReader sqlDataReader = MySQL.Info_of_a_user(lo_conn, username);
            string pki = sqlDataReader["Public_key"].ToString();

            string Plaintext = MyRAS.RSADecrypt(pki, cyphertext);//解密
            string[] Info = Plaintext.Split('#');
            string Payword = Info[0];
            DateTime Datestamp = DateTime.Parse(Info[1]);
            string Merchant_id = Info[2];
            float Amount = float.Parse(Info[3]);

            TimeSpan tSpan = DateTime.Now - Datestamp;
            int secondCount = (int)tSpan.TotalSeconds;
            if (secondCount > 20)//时间戳与现在相差20秒，认为无效
            {
                sqlDataReader.Close();
                lo_conn.Close();
                return -1;//时间戳错误
            }

            if (rs < 0)//支付密码锁定状态
            {
                sqlDataReader.Close();
                lo_conn.Close();
                return -rs;
            }

            if (Payword.CompareTo(sqlDataReader["Payword"].ToString()) == 0)//支付密码正确
            {
                MySQL.Reset_Remain_try(lo_conn, username, DateTime.Now);
            }
            else//支付密码错误，返回剩余尝试次数
            {
                MySQL.Decrease_Remain_try(lo_conn, username, DateTime.Now);
                int a = int.Parse(sqlDataReader["Remain_try"].ToString()) - 1;
                sqlDataReader.Close();
                lo_conn.Close();
                return -3 - a;
            }

            if (int.Parse(sqlDataReader["Balance"].ToString()) < Amount)//余额不足
            {
                sqlDataReader.Close();
                lo_conn.Close();
                return -2;//余额不足
            }
            else
            {
                MySQL.GAIN_of_merchant(lo_conn, Merchant_id, Amount);
                MySQL.PAY_of_user(lo_conn, username, Amount);
                MySQL.Insert_A_new_Bill(lo_conn, username, Merchant_id, Datestamp, Amount, detail);
                sqlDataReader.Close();
                lo_conn.Close();
                return 0;//成功
            }
        }

        public int payWithoutPassword(PayInfo payInfo)
        {
            return payWithPassword(payInfo);
        }


    }
}
